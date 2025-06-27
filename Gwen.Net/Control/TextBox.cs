using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Input;
using Gwen.Net.Platform;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Text box (editable).
    /// </summary>
    public class TextBox : ControlBase
    {
        private readonly ScrollArea scrollArea;
        private readonly Text text;
        protected Rectangle caretBounds;
        private Int32 cursorEnd;

        private Int32 cursorPos;

        protected Single lastInputTime;

        private Boolean selectAll;

        protected Rectangle selectionBounds;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TextBox(ControlBase parent)
            : base(parent)
        {
            Padding = Padding.Three;
            Cursor = Cursor.Beam;

            scrollArea = new ScrollArea(this);
            scrollArea.Dock = Dock.Fill;
            scrollArea.EnableScroll(horizontal: true, vertical: false);

            text = new Text(scrollArea);
            text.TextColor = Skin.colors.textBoxColors.text;
            text.BoundsChanged += (_, _) => RefreshCursorBounds();

            MouseInputEnabled = true;
            KeyboardInputEnabled = true;
            KeyboardNeeded = true;

            cursorPos = 0;
            cursorEnd = 0;
            selectAll = false;

            IsTabable = true;

            AddAccelerator("Ctrl + C", OnCopy);
            AddAccelerator("Ctrl + X", OnCut);
            AddAccelerator("Ctrl + V", OnPaste);
            AddAccelerator("Ctrl + A", OnSelectAll);

            IsVirtualControl = true;
        }

        protected override Boolean AccelOnlyFocus => true;
        protected override Boolean NeedsInputChars => true;

        /// <summary>
        ///     Determines whether text should be selected when the control is focused.
        /// </summary>
        public Boolean SelectAllOnFocus
        {
            get => selectAll;
            set
            {
                selectAll = value;

                if (value)
                {
                    OnSelectAll(this, EventArgs.Empty);
                }
            }
        }

        public Boolean LooseFocusOnSubmit { get; set; } = true;

        /// <summary>
        ///     Indicates whether the text has active selection.
        /// </summary>
        public virtual Boolean HasSelection => cursorPos != cursorEnd;

        /// <summary>
        ///     Current cursor position (character index).
        /// </summary>
        public Int32 CursorPos
        {
            get => cursorPos;
            set
            {
                if (cursorPos == value)
                {
                    return;
                }

                cursorPos = value;
                RefreshCursorBounds();
            }
        }

        public Int32 CursorEnd
        {
            get => cursorEnd;
            set
            {
                if (cursorEnd == value)
                {
                    return;
                }

                cursorEnd = value;
                RefreshCursorBounds();
            }
        }

        /// <summary>
        ///     Text.
        /// </summary>
        public virtual String Text
        {
            get => text.String;
            set => SetText(value);
        }

        /// <summary>
        ///     Text color.
        /// </summary>
        public Color TextColor
        {
            get => text.TextColor;
            set => text.TextColor = value;
        }

        /// <summary>
        ///     Override text color (used by tooltips).
        /// </summary>
        public Color TextColorOverride
        {
            get => text.TextColorOverride;
            set => text.TextColorOverride = value;
        }

        /// <summary>
        ///     Text override - used to display different string.
        /// </summary>
        public String TextOverride
        {
            get => text.TextOverride;
            set => text.TextOverride = value;
        }

        /// <summary>
        ///     Font.
        /// </summary>
        public Font Font
        {
            get => text.Font;
            set
            {
                text.Font = value;
                DoFitToText();
                Invalidate();
            }
        }

        /// <summary>
        ///     Set the size of the control to be able to show the text of this property.
        /// </summary>
        public String FitToText
        {
            get => text.FitToText;
            set
            {
                text.FitToText = value;
                DoFitToText();
            }
        }

        protected override void AdaptToScaleChange()
        {
            DoFitToText();
        }

        /// <summary>
        ///     Invoked when the text has changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> TextChanged;

        /// <summary>
        ///     Invoked when the submit key has been pressed.
        /// </summary>
        public event GwenEventHandler<EventArgs> SubmitPressed;

        /// <summary>
        ///     Determines whether the control can insert text at a given cursor position.
        /// </summary>
        /// <param name="textToCheck">Text to check.</param>
        /// <param name="position">Cursor position.</param>
        /// <returns>True if allowed.</returns>
        protected virtual Boolean IsTextAllowed(String textToCheck, Int32 position)
        {
            return true;
        }

        /// <summary>
        ///     Sets the label text.
        /// </summary>
        /// <param name="str">Text to set.</param>
        /// <param name="doEvents">Determines whether to invoke "text changed" event.</param>
        public virtual void SetText(String str, Boolean doEvents = true)
        {
            if (Text == str)
            {
                return;
            }

            text.String = str;

            if (cursorPos > text.Length)
            {
                cursorPos = text.Length;
            }

            if (doEvents)
            {
                OnTextChanged();
            }

            RefreshCursorBounds();
        }

        /// <summary>
        ///     Inserts text at current cursor position, erasing selection if any.
        /// </summary>
        /// <param name="insertedText">Text to insert.</param>
        protected virtual void InsertText(String insertedText)
        {
            // TODO: Make sure fits (implement maxlength)

            if (HasSelection)
            {
                EraseSelection();
            }

            if (cursorPos > this.text.Length)
            {
                cursorPos = this.text.Length;
            }

            if (!IsTextAllowed(insertedText, cursorPos))
            {
                return;
            }

            String str = Text;
            str = str.Insert(cursorPos, insertedText);
            SetText(str);

            cursorPos += insertedText.Length;
            cursorEnd = cursorPos;

            RefreshCursorBounds();
        }

        /// <summary>
        ///     Deletes text.
        /// </summary>
        /// <param name="startPos">Starting cursor position.</param>
        /// <param name="length">Length in characters.</param>
        public virtual void DeleteText(Int32 startPos, Int32 length)
        {
            String str = Text;
            str = str.Remove(startPos, length);
            SetText(str);

            if (cursorPos > startPos)
            {
                CursorPos = cursorPos - length;
            }

            CursorEnd = cursorPos;
        }

        /// <summary>
        ///     Handler for text changed event.
        /// </summary>
        protected virtual void OnTextChanged()
        {
            if (cursorPos > text.Length)
            {
                cursorPos = text.Length;
            }

            if (cursorEnd > text.Length)
            {
                cursorEnd = text.Length;
            }

            if (TextChanged != null)
            {
                TextChanged.Invoke(this, EventArgs.Empty);
            }
        }

        private void DoFitToText()
        {
            if (!String.IsNullOrWhiteSpace(FitToText))
            {
                Size size = Skin.Renderer.MeasureText(Font, FitToText);
                scrollArea.MinimumSize = size;
                Invalidate();
            }
        }

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(Int32 x, Int32 y, Boolean down)
        {
            base.OnMouseClickedLeft(x, y, down);

            if (selectAll)
            {
                OnSelectAll(this, EventArgs.Empty);
                
                return;
            }

            Int32 c = GetClosestCharacter(x, y).X;

            if (down)
            {
                CursorPos = c;

                if (!InputHandler.IsShiftDown)
                {
                    CursorEnd = c;
                }

                InputHandler.MouseFocus = this;
            }
            else
            {
                if (InputHandler.MouseFocus == this)
                {
                    CursorPos = c;
                    InputHandler.MouseFocus = null;
                }
            }
        }

        /// <summary>
        ///     Handler invoked on mouse double click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        protected override void OnMouseDoubleClickedLeft(Int32 x, Int32 y)
        {
            //base.OnMouseDoubleClickedLeft(x, y);
            OnSelectAll(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Handler invoked on mouse moved event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="dx">X change.</param>
        /// <param name="dy">Y change.</param>
        protected override void OnMouseMoved(Int32 x, Int32 y, Int32 dx, Int32 dy)
        {
            base.OnMouseMoved(x, y, dx, dy);

            if (InputHandler.MouseFocus != this)
            {
                return;
            }

            Int32 c = GetClosestCharacter(x, y).X;

            CursorPos = c;
        }

        /// <summary>
        ///     Handler for character input event.
        /// </summary>
        /// <param name="chr">Character typed.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnChar(Char chr)
        {
            base.OnChar(chr);

            if (chr == '\t')
            {
                return false;
            }

            InsertText(chr.ToString());

            return true;
        }

        /// <summary>
        ///     Handler for Paste event.
        /// </summary>
        /// <param name="from">Source control.</param>
        /// <param name="args">Event arguments.</param>
        protected override void OnPaste(ControlBase from, EventArgs args)
        {
            base.OnPaste(from, args);
            InsertText(GwenPlatform.GetClipboardText());
        }

        /// <summary>
        ///     Handler for Copy event.
        /// </summary>
        /// <param name="from">Source control.</param>
        /// <param name="args">Event arguments.</param>
        protected override void OnCopy(ControlBase from, EventArgs args)
        {
            if (!HasSelection)
            {
                return;
            }

            base.OnCopy(from, args);

            GwenPlatform.SetClipboardText(GetSelection());
        }

        /// <summary>
        ///     Handler for Cut event.
        /// </summary>
        /// <param name="from">Source control.</param>
        /// <param name="args">Event arguments.</param>
        protected override void OnCut(ControlBase from, EventArgs args)
        {
            if (!HasSelection)
            {
                return;
            }

            base.OnCut(from, args);

            GwenPlatform.SetClipboardText(GetSelection());
            EraseSelection();
        }

        /// <summary>
        ///     Handler for Select All event.
        /// </summary>
        /// <param name="from">Source control.</param>
        /// <param name="args">Event arguments.</param>
        protected override void OnSelectAll(ControlBase from, EventArgs args)
        {
            cursorEnd = 0;
            cursorPos = text.Length;

            RefreshCursorBounds();
        }

        /// <summary>
        ///     Handler for Return keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyReturn(Boolean down)
        {
            base.OnKeyReturn(down);

            if (down)
            {
                return true;
            }

            OnReturn();

            if (LooseFocusOnSubmit)
            {
                // Try to move to the next control, as if tab had been pressed
                OnKeyTab(down: true);

                // If we still have focus, blur it.
                if (HasFocus) Blur();
            }

            return true;
        }

        /// <summary>
        ///     Handler for Escape keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyEscape(Boolean down)
        {
            base.OnKeyEscape(down);

            if (down)
            {
                return true;
            }

            // If we still have focus, blur it.
            if (HasFocus)
            {
                Blur();
            }

            return true;
        }

        /// <summary>
        ///     Handler for Backspace keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyBackspace(Boolean down)
        {
            base.OnKeyBackspace(down);

            if (!down)
            {
                return true;
            }

            if (HasSelection)
            {
                EraseSelection();

                return true;
            }

            if (cursorPos == 0)
            {
                return true;
            }

            DeleteText(cursorPos - 1, length: 1);

            return true;
        }

        /// <summary>
        ///     Handler for Delete keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyDelete(Boolean down)
        {
            base.OnKeyDelete(down);

            if (!down)
            {
                return true;
            }

            if (HasSelection)
            {
                EraseSelection();

                return true;
            }

            if (cursorPos >= text.Length)
            {
                return true;
            }

            DeleteText(cursorPos, length: 1);

            return true;
        }

        /// <summary>
        ///     Handler for Left Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyLeft(Boolean down)
        {
            base.OnKeyLeft(down);

            if (!down)
            {
                return true;
            }

            if (cursorPos > 0)
            {
                cursorPos--;
            }

            if (!InputHandler.IsShiftDown)
            {
                cursorEnd = cursorPos;
            }

            RefreshCursorBounds();

            return true;
        }

        /// <summary>
        ///     Handler for Right Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyRight(Boolean down)
        {
            base.OnKeyRight(down);

            if (!down)
            {
                return true;
            }

            if (cursorPos < text.Length)
            {
                cursorPos++;
            }

            if (!InputHandler.IsShiftDown)
            {
                cursorEnd = cursorPos;
            }

            RefreshCursorBounds();

            return true;
        }

        /// <summary>
        ///     Handler for Home keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyHome(Boolean down)
        {
            base.OnKeyHome(down);

            if (!down)
            {
                return true;
            }

            cursorPos = 0;

            if (!InputHandler.IsShiftDown)
            {
                cursorEnd = cursorPos;
            }

            RefreshCursorBounds();

            return true;
        }

        /// <summary>
        ///     Handler for End keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyEnd(Boolean down)
        {
            base.OnKeyEnd(down);
            cursorPos = text.Length;

            if (!InputHandler.IsShiftDown)
            {
                cursorEnd = cursorPos;
            }

            RefreshCursorBounds();

            return true;
        }

        /// <summary>
        ///     Handler for the return key.
        /// </summary>
        protected virtual void OnReturn()
        {
            if (SubmitPressed != null)
            {
                SubmitPressed.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Returns currently selected text.
        /// </summary>
        /// <returns>Current selection.</returns>
        public String GetSelection()
        {
            if (!HasSelection)
            {
                return String.Empty;
            }

            Int32 start = Math.Min(cursorPos, cursorEnd);
            Int32 end = Math.Max(cursorPos, cursorEnd);

            String str = Text;

            return str.Substring(start, end - start);
        }

        /// <summary>
        ///     Deletes selected text.
        /// </summary>
        public virtual void EraseSelection()
        {
            Int32 start = Math.Min(cursorPos, cursorEnd);
            Int32 end = Math.Max(cursorPos, cursorEnd);

            DeleteText(start, end - start);

            // Move the cursor to the start of the selection, 
            // since the end is probably outside of the string now.
            cursorPos = start;
            cursorEnd = start;
        }

        protected override void OnBoundsChanged(Rectangle oldBounds)
        {
            RefreshCursorBounds();

            base.OnBoundsChanged(oldBounds);
        }

        /// <summary>
        ///     Returns index of the character closest to specified point (in canvas coordinates).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual Point GetClosestCharacter(Int32 x, Int32 y)
        {
            return new(text.GetClosestCharacter(text.CanvasPosToLocal(new Point(x, y))), y: 0);
        }

        /// <summary>
        ///     Gets the coordinates of specified character.
        /// </summary>
        /// <param name="index">Character index.</param>
        /// <returns>Character coordinates (local).</returns>
        public virtual Point GetCharacterPosition(Int32 index)
        {
            Point p = text.GetCharacterPosition(index);

            return new Point(p.X + text.ActualLeft + Padding.Left, p.Y + text.ActualTop + Padding.Top);
        }

        protected virtual void MakeCaretVisible()
        {
            Size viewSize = scrollArea.ViewableContentSize;
            Int32 caretPos = GetCharacterPosition(cursorPos).X;
            Int32 realCaretPos = caretPos;

            caretPos -= text.ActualLeft;

            // If the caret is already in a semi-good position, leave it.
            if (realCaretPos > scrollArea.ActualWidth * 0.1f && realCaretPos < scrollArea.ActualWidth * 0.9f)
            {
                return;
            }

            // The ideal position is for the caret to be right in the middle
            var idealx = (Int32) (-caretPos + scrollArea.ActualWidth * 0.5f);

            // Don't show too much whitespace to the right
            if (idealx + text.MeasuredSize.Width < viewSize.Width)
            {
                idealx = -text.MeasuredSize.Width + viewSize.Width;
            }

            // Or the left
            if (idealx > 0)
            {
                idealx = 0;
            }

            scrollArea.SetScrollPosition(idealx, vertical: 0);
        }

        protected virtual void RefreshCursorBounds()
        {
            lastInputTime = GwenPlatform.GetTimeInSeconds();

            MakeCaretVisible();

            Point pA = GetCharacterPosition(cursorPos);
            Point pB = GetCharacterPosition(cursorEnd);

            selectionBounds.X = Math.Min(pA.X, pB.X);
            selectionBounds.Y = pA.Y;
            selectionBounds.Width = Math.Max(pA.X, pB.X) - selectionBounds.X;
            selectionBounds.Height = text.ActualHeight;

            caretBounds.X = pA.X;
            caretBounds.Y = pA.Y;
            caretBounds.Width = 1;
            caretBounds.Height = text.ActualHeight;

            Redraw();
        }

        /// <summary>
        ///     Renders the focus overlay.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void RenderFocus(SkinBase currentSkin)
        {
            // nothing
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            base.Render(currentSkin);

            if (ShouldDrawBackground)
            {
                currentSkin.DrawTextBox(this);
            }

            if (!HasFocus)
            {
                return;
            }

            Rectangle oldClipRegion = currentSkin.Renderer.ClipRegion;

            Rectangle clipRect = scrollArea.Bounds;
            clipRect.Width += 1; // Make space for caret
            currentSkin.Renderer.SetClipRegion(clipRect);

            // Draw selection.. if selected..
            if (cursorPos != cursorEnd)
            {
                currentSkin.Renderer.DrawColor = Skin.colors.textBoxColors.backgroundSelected;
                currentSkin.Renderer.DrawFilledRect(selectionBounds);
            }

            // Draw caret
            Single time = GwenPlatform.GetTimeInSeconds() - lastInputTime;

            if (time % 1.0f <= 0.5f)
            {
                currentSkin.Renderer.DrawColor = Skin.colors.textBoxColors.caret;
                currentSkin.Renderer.DrawFilledRect(caretBounds);
            }

            currentSkin.Renderer.ClipRegion = oldClipRegion;
        }
    }
}
