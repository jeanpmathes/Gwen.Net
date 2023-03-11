﻿using System;
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
        private readonly ScrollArea m_ScrollArea;
        private readonly Text m_Text;
        protected Rectangle m_CaretBounds;
        private int m_CursorEnd;

        private int m_CursorPos;

        protected float m_LastInputTime;

        private bool m_SelectAll;

        protected Rectangle m_SelectionBounds;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TextBox(ControlBase parent)
            : base(parent)
        {
            Padding = Padding.Three;

            m_ScrollArea = new ScrollArea(this);
            m_ScrollArea.Dock = Dock.Fill;
            m_ScrollArea.EnableScroll(horizontal: true, vertical: false);

            m_Text = new Text(m_ScrollArea);
            m_Text.TextColor = Skin.Colors.TextBox.Text;
            m_Text.BoundsChanged += (s, a) => RefreshCursorBounds();

            MouseInputEnabled = true;
            KeyboardInputEnabled = true;
            KeyboardNeeded = true;

            m_CursorPos = 0;
            m_CursorEnd = 0;
            m_SelectAll = false;

            IsTabable = true;

            AddAccelerator("Ctrl + C", OnCopy);
            AddAccelerator("Ctrl + X", OnCut);
            AddAccelerator("Ctrl + V", OnPaste);
            AddAccelerator("Ctrl + A", OnSelectAll);

            IsVirtualControl = true;
        }

        protected override bool AccelOnlyFocus => true;
        protected override bool NeedsInputChars => true;

        /// <summary>
        ///     Determines whether text should be selected when the control is focused.
        /// </summary>
        public bool SelectAllOnFocus
        {
            get => m_SelectAll;
            set
            {
                m_SelectAll = value;

                if (value)
                {
                    OnSelectAll(this, EventArgs.Empty);
                }
            }
        }

        public bool LooseFocusOnSubmit { get; set; } = true;

        /// <summary>
        ///     Indicates whether the text has active selection.
        /// </summary>
        public virtual bool HasSelection => m_CursorPos != m_CursorEnd;

        /// <summary>
        ///     Current cursor position (character index).
        /// </summary>
        public int CursorPos
        {
            get => m_CursorPos;
            set
            {
                if (m_CursorPos == value)
                {
                    return;
                }

                m_CursorPos = value;
                RefreshCursorBounds();
            }
        }

        public int CursorEnd
        {
            get => m_CursorEnd;
            set
            {
                if (m_CursorEnd == value)
                {
                    return;
                }

                m_CursorEnd = value;
                RefreshCursorBounds();
            }
        }

        /// <summary>
        ///     Text.
        /// </summary>
        public virtual string Text
        {
            get => m_Text.String;
            set => SetText(value);
        }

        /// <summary>
        ///     Text color.
        /// </summary>
        public Color TextColor
        {
            get => m_Text.TextColor;
            set => m_Text.TextColor = value;
        }

        /// <summary>
        ///     Override text color (used by tooltips).
        /// </summary>
        public Color TextColorOverride
        {
            get => m_Text.TextColorOverride;
            set => m_Text.TextColorOverride = value;
        }

        /// <summary>
        ///     Text override - used to display different string.
        /// </summary>
        public string TextOverride
        {
            get => m_Text.TextOverride;
            set => m_Text.TextOverride = value;
        }

        /// <summary>
        ///     Font.
        /// </summary>
        public Font Font
        {
            get => m_Text.Font;
            set
            {
                m_Text.Font = value;
                DoFitToText();
                Invalidate();
            }
        }

        /// <summary>
        ///     Set the size of the control to be able to show the text of this property.
        /// </summary>
        public string FitToText
        {
            get => m_Text.FitToText;
            set
            {
                m_Text.FitToText = value;
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
        /// <param name="text">Text to check.</param>
        /// <param name="position">Cursor position.</param>
        /// <returns>True if allowed.</returns>
        protected virtual bool IsTextAllowed(string text, int position)
        {
            return true;
        }

        /// <summary>
        ///     Sets the label text.
        /// </summary>
        /// <param name="str">Text to set.</param>
        /// <param name="doEvents">Determines whether to invoke "text changed" event.</param>
        public virtual void SetText(string str, bool doEvents = true)
        {
            if (Text == str)
            {
                return;
            }

            m_Text.String = str;

            if (m_CursorPos > m_Text.Length)
            {
                m_CursorPos = m_Text.Length;
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
        /// <param name="text">Text to insert.</param>
        protected virtual void InsertText(string text)
        {
            // TODO: Make sure fits (implement maxlength)

            if (HasSelection)
            {
                EraseSelection();
            }

            if (m_CursorPos > m_Text.Length)
            {
                m_CursorPos = m_Text.Length;
            }

            if (!IsTextAllowed(text, m_CursorPos))
            {
                return;
            }

            string str = Text;
            str = str.Insert(m_CursorPos, text);
            SetText(str);

            m_CursorPos += text.Length;
            m_CursorEnd = m_CursorPos;

            RefreshCursorBounds();
        }

        /// <summary>
        ///     Deletes text.
        /// </summary>
        /// <param name="startPos">Starting cursor position.</param>
        /// <param name="length">Length in characters.</param>
        public virtual void DeleteText(int startPos, int length)
        {
            string str = Text;
            str = str.Remove(startPos, length);
            SetText(str);

            if (m_CursorPos > startPos)
            {
                CursorPos = m_CursorPos - length;
            }

            CursorEnd = m_CursorPos;
        }

        /// <summary>
        ///     Handler for text changed event.
        /// </summary>
        protected virtual void OnTextChanged()
        {
            if (m_CursorPos > m_Text.Length)
            {
                m_CursorPos = m_Text.Length;
            }

            if (m_CursorEnd > m_Text.Length)
            {
                m_CursorEnd = m_Text.Length;
            }

            if (TextChanged != null)
            {
                TextChanged.Invoke(this, EventArgs.Empty);
            }
        }

        private void DoFitToText()
        {
            if (!string.IsNullOrWhiteSpace(FitToText))
            {
                Size size = Skin.Renderer.MeasureText(Font, FitToText);
                m_ScrollArea.MinimumSize = size;
                Invalidate();
            }
        }

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(int x, int y, bool down)
        {
            base.OnMouseClickedLeft(x, y, down);

            if (m_SelectAll)
            {
                OnSelectAll(this, EventArgs.Empty);

                //m_SelectAll = false;
                return;
            }

            int c = GetClosestCharacter(x, y).X;

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
        protected override void OnMouseDoubleClickedLeft(int x, int y)
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
        protected override void OnMouseMoved(int x, int y, int dx, int dy)
        {
            base.OnMouseMoved(x, y, dx, dy);

            if (InputHandler.MouseFocus != this)
            {
                return;
            }

            int c = GetClosestCharacter(x, y).X;

            CursorPos = c;
        }

        /// <summary>
        ///     Handler for character input event.
        /// </summary>
        /// <param name="chr">Character typed.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnChar(char chr)
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
        protected override void OnPaste(ControlBase from, EventArgs args)
        {
            base.OnPaste(from, args);
            InsertText(GwenPlatform.GetClipboardText());
        }

        /// <summary>
        ///     Handler for Copy event.
        /// </summary>
        /// <param name="from">Source control.</param>
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
        protected override void OnSelectAll(ControlBase from, EventArgs args)
        {
            //base.OnSelectAll(from);
            m_CursorEnd = 0;
            m_CursorPos = m_Text.Length;

            RefreshCursorBounds();
        }

        /// <summary>
        ///     Handler for Return keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeyReturn(bool down)
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
        protected override bool OnKeyEscape(bool down)
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
        protected override bool OnKeyBackspace(bool down)
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

            if (m_CursorPos == 0)
            {
                return true;
            }

            DeleteText(m_CursorPos - 1, length: 1);

            return true;
        }

        /// <summary>
        ///     Handler for Delete keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeyDelete(bool down)
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

            if (m_CursorPos >= m_Text.Length)
            {
                return true;
            }

            DeleteText(m_CursorPos, length: 1);

            return true;
        }

        /// <summary>
        ///     Handler for Left Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeyLeft(bool down)
        {
            base.OnKeyLeft(down);

            if (!down)
            {
                return true;
            }

            if (m_CursorPos > 0)
            {
                m_CursorPos--;
            }

            if (!InputHandler.IsShiftDown)
            {
                m_CursorEnd = m_CursorPos;
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
        protected override bool OnKeyRight(bool down)
        {
            base.OnKeyRight(down);

            if (!down)
            {
                return true;
            }

            if (m_CursorPos < m_Text.Length)
            {
                m_CursorPos++;
            }

            if (!InputHandler.IsShiftDown)
            {
                m_CursorEnd = m_CursorPos;
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
        protected override bool OnKeyHome(bool down)
        {
            base.OnKeyHome(down);

            if (!down)
            {
                return true;
            }

            m_CursorPos = 0;

            if (!InputHandler.IsShiftDown)
            {
                m_CursorEnd = m_CursorPos;
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
        protected override bool OnKeyEnd(bool down)
        {
            base.OnKeyEnd(down);
            m_CursorPos = m_Text.Length;

            if (!InputHandler.IsShiftDown)
            {
                m_CursorEnd = m_CursorPos;
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
        public string GetSelection()
        {
            if (!HasSelection)
            {
                return string.Empty;
            }

            int start = Math.Min(m_CursorPos, m_CursorEnd);
            int end = Math.Max(m_CursorPos, m_CursorEnd);

            string str = Text;

            return str.Substring(start, end - start);
        }

        /// <summary>
        ///     Deletes selected text.
        /// </summary>
        public virtual void EraseSelection()
        {
            int start = Math.Min(m_CursorPos, m_CursorEnd);
            int end = Math.Max(m_CursorPos, m_CursorEnd);

            DeleteText(start, end - start);

            // Move the cursor to the start of the selection, 
            // since the end is probably outside of the string now.
            m_CursorPos = start;
            m_CursorEnd = start;
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
        protected virtual Point GetClosestCharacter(int x, int y)
        {
            return new(m_Text.GetClosestCharacter(m_Text.CanvasPosToLocal(new Point(x, y))), y: 0);
        }

        /// <summary>
        ///     Gets the coordinates of specified character.
        /// </summary>
        /// <param name="index">Character index.</param>
        /// <returns>Character coordinates (local).</returns>
        public virtual Point GetCharacterPosition(int index)
        {
            Point p = m_Text.GetCharacterPosition(index);

            return new Point(p.X + m_Text.ActualLeft + Padding.Left, p.Y + m_Text.ActualTop + Padding.Top);
        }

        protected virtual void MakeCaretVisible()
        {
            Size viewSize = m_ScrollArea.ViewableContentSize;
            int caretPos = GetCharacterPosition(m_CursorPos).X;
            int realCaretPos = caretPos;

            caretPos -= m_Text.ActualLeft;

            // If the caret is already in a semi-good position, leave it.
            if (realCaretPos > m_ScrollArea.ActualWidth * 0.1f && realCaretPos < m_ScrollArea.ActualWidth * 0.9f)
            {
                return;
            }

            // The ideal position is for the caret to be right in the middle
            var idealx = (int) (-caretPos + m_ScrollArea.ActualWidth * 0.5f);

            // Don't show too much whitespace to the right
            if (idealx + m_Text.MeasuredSize.Width < viewSize.Width)
            {
                idealx = -m_Text.MeasuredSize.Width + viewSize.Width;
            }

            // Or the left
            if (idealx > 0)
            {
                idealx = 0;
            }

            m_ScrollArea.SetScrollPosition(idealx, vertical: 0);
        }

        protected virtual void RefreshCursorBounds()
        {
            m_LastInputTime = GwenPlatform.GetTimeInSeconds();

            MakeCaretVisible();

            Point pA = GetCharacterPosition(m_CursorPos);
            Point pB = GetCharacterPosition(m_CursorEnd);

            m_SelectionBounds.X = Math.Min(pA.X, pB.X);
            m_SelectionBounds.Y = pA.Y;
            m_SelectionBounds.Width = Math.Max(pA.X, pB.X) - m_SelectionBounds.X;
            m_SelectionBounds.Height = m_Text.ActualHeight;

            m_CaretBounds.X = pA.X;
            m_CaretBounds.Y = pA.Y;
            m_CaretBounds.Width = 1;
            m_CaretBounds.Height = m_Text.ActualHeight;

            Redraw();
        }

        /// <summary>
        ///     Renders the focus overlay.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void RenderFocus(SkinBase skin)
        {
            // nothing
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            base.Render(skin);

            if (ShouldDrawBackground)
            {
                skin.DrawTextBox(this);
            }

            if (!HasFocus)
            {
                return;
            }

            Rectangle oldClipRegion = skin.Renderer.ClipRegion;

            Rectangle clipRect = m_ScrollArea.Bounds;
            clipRect.Width += 1; // Make space for caret
            skin.Renderer.SetClipRegion(clipRect);

            // Draw selection.. if selected..
            if (m_CursorPos != m_CursorEnd)
            {
                skin.Renderer.DrawColor = Skin.Colors.TextBox.Background_Selected;
                skin.Renderer.DrawFilledRect(m_SelectionBounds);
            }

            // Draw caret
            float time = GwenPlatform.GetTimeInSeconds() - m_LastInputTime;

            if (time % 1.0f <= 0.5f)
            {
                skin.Renderer.DrawColor = Skin.Colors.TextBox.Caret;
                skin.Renderer.DrawFilledRect(m_CaretBounds);
            }

            skin.Renderer.ClipRegion = oldClipRegion;
        }
    }
}
