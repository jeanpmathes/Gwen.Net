using System;
using Gwen.Net.Legacy.Control.Internal;
using Gwen.Net.Legacy.Input;
using Gwen.Net.Legacy.Platform;
using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control
{
    public class MultilineTextBox : ScrollControl
    {
        private readonly Boolean selectAll;
        private readonly MultilineText text;
        protected Rectangle caretBounds;
        private Point cursorEnd;

        private Point cursorPos;

        private Single lastInputTime;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public MultilineTextBox(ControlBase parent) : base(parent)
        {
            Padding = Padding.Three;
            Cursor = Cursor.Beam;

            EnableScroll(horizontal: true, vertical: true);
            AutoHideBars = true;

            MouseInputEnabled = true;
            KeyboardInputEnabled = true;
            KeyboardNeeded = true;
            IsTabable = false;
            AcceptTabs = true;

            text = new MultilineText(this);
            text.BoundsChanged += ScrollChanged;

            cursorPos = new Point(x: 0, y: 0);
            cursorEnd = new Point(x: 0, y: 0);
            selectAll = false;

            Font = Skin.DefaultFont;

            AddAccelerator("Ctrl + C", OnCopy);
            AddAccelerator("Ctrl + X", OnCut);
            AddAccelerator("Ctrl + V", OnPaste);
            AddAccelerator("Ctrl + A", OnSelectAll);

            SetText(String.Empty);
        }

        private Point StartPoint
        {
            get
            {
                if (CursorPosition.Y == cursorEnd.Y)
                {
                    return CursorPosition.X < CursorEnd.X ? CursorPosition : CursorEnd;
                }

                return CursorPosition.Y < CursorEnd.Y ? CursorPosition : CursorEnd;
            }
        }

        private Point EndPoint
        {
            get
            {
                if (CursorPosition.Y == cursorEnd.Y)
                {
                    return CursorPosition.X > CursorEnd.X ? CursorPosition : CursorEnd;
                }

                return CursorPosition.Y > CursorEnd.Y ? CursorPosition : CursorEnd;
            }
        }

        /// <summary>
        ///     Indicates whether the text has active selection.
        /// </summary>
        public Boolean HasSelection => cursorPos != cursorEnd;

        /// <summary>
        ///     Get a point representing where the cursor physically appears on the screen.
        ///     Y is line number, X is character position on that line.
        /// </summary>
        public Point CursorPosition
        {
            get
            {
                Int32 y = Util.Clamp(cursorPos.Y, min: 0, text.TotalLines - 1);

                Int32 x = Util.Clamp(
                    cursorPos.X,
                    min: 0,
                    text[y].Length); // X may be beyond the last character, but we will want to draw it at the end of line.

                return new Point(x, y);
            }
            set
            {
                cursorPos.X = value.X;
                cursorPos.Y = value.Y;
                RefreshCursorBounds();
            }
        }

        /// <summary>
        ///     Get a point representing where the endpoint of text selection.
        ///     Y is line number, X is character position on that line.
        /// </summary>
        public Point CursorEnd
        {
            get
            {
                Int32 y = Util.Clamp(cursorEnd.Y, min: 0, text.TotalLines - 1);

                Int32 x = Util.Clamp(
                    cursorEnd.X,
                    min: 0,
                    text[y].Length); // X may be beyond the last character, but we will want to draw it at the end of line.

                return new Point(x, y);
            }
            set
            {
                cursorEnd.X = value.X;
                cursorEnd.Y = value.Y;
                RefreshCursorBounds();
            }
        }

        /// <summary>
        ///     Indicates whether the control will accept Tab characters as input.
        /// </summary>
        public Boolean AcceptTabs { get; set; }

        /// <summary>
        ///     Returns the number of lines that are in the Multiline Text Box.
        /// </summary>
        public Int32 TotalLines => text.TotalLines;

        private Int32 LineHeight => text.LineHeight;

        /// <summary>
        ///     Gets and sets the text to display to the user. Each line is separated by
        ///     an Environment.NetLine character.
        /// </summary>
        public String Text
        {
            get => text.Text;
            set => SetText(value);
        }

        public Font Font
        {
            get => text.Font;
            set => text.Font = value;
        }

        /// <summary>
        ///     Invoked when the text has changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> TextChanged;

        /// <summary>
        ///     Sets the label text.
        /// </summary>
        /// <param name="newText">Text to set.</param>
        /// <param name="doEvents">Determines whether to invoke "text changed" event.</param>
        public void SetText(String newText, Boolean doEvents = true)
        {
            this.text.SetText(newText);

            cursorPos = Point.Zero;
            cursorEnd = cursorPos;

            UpdateText();
            RefreshCursorBounds();

            if (doEvents)
            {
                OnTextChanged();
            }
        }

        /// <summary>
        ///     Inserts text at current cursor position, erasing selection if any.
        /// </summary>
        /// <param name="insertText">Text to insert.</param>
        public void InsertText(String insertText)
        {
            if (HasSelection)
            {
                EraseSelection();
            }

            cursorPos = this.text.InsertText(insertText, CursorPosition);
            cursorEnd = cursorPos;

            UpdateText();
            OnTextChanged();
            RefreshCursorBounds();
        }

        /// <summary>
        ///     Remove all text.
        /// </summary>
        public void Clear()
        {
            text.Clear();

            cursorPos = Point.Zero;
            cursorEnd = cursorPos;

            UpdateText();
            OnTextChanged();
            RefreshCursorBounds();
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

            Point coords = GetClosestCharacter(x, y);

            if (down)
            {
                CursorPosition = coords;

                if (!InputHandler.IsShiftDown)
                {
                    CursorEnd = coords;
                }

                InputHandler.MouseFocus = this;
            }
            else
            {
                if (InputHandler.MouseFocus == this)
                {
                    CursorPosition = coords;
                    InputHandler.MouseFocus = null;
                }
            }

            RefreshCursorBounds();
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

            Point c = GetClosestCharacter(x, y);

            CursorPosition = c;

            RefreshCursorBounds();
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
            //base.OnChar(chr);
            if (chr == '\t' && !AcceptTabs)
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
            cursorEnd = new Point(x: 0, y: 0);
            cursorPos = new Point(text[text.TotalLines - 1].Length, text.TotalLines);

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
            if (down)
            {
                return true;
            }

            //Split current string, putting the rhs on a new line
            String currentLine = text[cursorPos.Y];
            String lhs = currentLine.Substring(startIndex: 0, CursorPosition.X);
            String rhs = currentLine.Substring(CursorPosition.X);

            text[cursorPos.Y] = lhs;
            text.InsertLine(cursorPos.Y + 1, rhs);

            OnKeyDown(down: true);
            OnKeyHome(down: true);

            if (cursorPos.Y == TotalLines - 1)
            {
                ScrollToBottom();
            }

            UpdateText();
            OnTextChanged();
            RefreshCursorBounds();

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
            if (!down)
            {
                return true;
            }

            if (HasSelection)
            {
                EraseSelection();

                return true;
            }

            if (cursorPos.X == 0)
            {
                if (cursorPos.Y == 0)
                {
                    return true; //Nothing left to delete
                }

                String lhs = text[cursorPos.Y - 1];
                String rhs = text[cursorPos.Y];
                text.RemoveLine(cursorPos.Y);
                OnKeyUp(down: true);
                OnKeyEnd(down: true);
                text[cursorPos.Y] = lhs + rhs;
            }
            else
            {
                String currentLine = text[cursorPos.Y];
                String lhs = currentLine.Substring(startIndex: 0, CursorPosition.X - 1);
                String rhs = currentLine.Substring(CursorPosition.X);
                text[cursorPos.Y] = lhs + rhs;
                OnKeyLeft(down: true);
            }

            UpdateText();
            OnTextChanged();
            RefreshCursorBounds();

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
            if (!down)
            {
                return true;
            }

            if (HasSelection)
            {
                EraseSelection();

                return true;
            }

            if (cursorPos.X == text[cursorPos.Y].Length)
            {
                if (cursorPos.Y == text.TotalLines - 1)
                {
                    return true; //Nothing left to delete
                }

                String lhs = text[cursorPos.Y];
                String rhs = text[cursorPos.Y + 1];
                text.RemoveLine(cursorPos.Y + 1);
                OnKeyEnd(down: true);
                text[cursorPos.Y] = lhs + rhs;
            }
            else
            {
                String currentLine = text[cursorPos.Y];
                String lhs = currentLine.Substring(startIndex: 0, CursorPosition.X);
                String rhs = currentLine.Substring(CursorPosition.X + 1);
                text[cursorPos.Y] = lhs + rhs;
            }

            UpdateText();
            OnTextChanged();
            RefreshCursorBounds();

            return true;
        }

        /// <summary>
        ///     Handler for Up Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyUp(Boolean down)
        {
            if (!down)
            {
                return true;
            }

            if (cursorPos.Y > 0)
            {
                cursorPos.Y -= 1;
            }

            if (!InputHandler.IsShiftDown)
            {
                cursorEnd = cursorPos;
            }

            RefreshCursorBounds();

            return true;
        }

        /// <summary>
        ///     Handler for Down Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyDown(Boolean down)
        {
            if (!down)
            {
                return true;
            }

            if (cursorPos.Y < TotalLines - 1)
            {
                cursorPos.Y += 1;
            }

            if (!InputHandler.IsShiftDown)
            {
                cursorEnd = cursorPos;
            }

            RefreshCursorBounds();

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
            if (!down)
            {
                return true;
            }

            if (cursorPos.X > 0)
            {
                cursorPos.X = Math.Min(cursorPos.X - 1, text[cursorPos.Y].Length);
            }
            else
            {
                if (cursorPos.Y > 0)
                {
                    OnKeyUp(down);
                    OnKeyEnd(down);
                }
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
            if (!down)
            {
                return true;
            }

            if (cursorPos.X < text[cursorPos.Y].Length)
            {
                cursorPos.X = Math.Min(cursorPos.X + 1, text[cursorPos.Y].Length);
            }
            else
            {
                if (cursorPos.Y < text.TotalLines - 1)
                {
                    OnKeyDown(down);
                    OnKeyHome(down);
                }
            }

            if (!InputHandler.IsShiftDown)
            {
                cursorEnd = cursorPos;
            }

            RefreshCursorBounds();

            return true;
        }

        /// <summary>
        ///     Handler for Home Key keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyHome(Boolean down)
        {
            if (!down)
            {
                return true;
            }

            cursorPos.X = 0;

            if (!InputHandler.IsShiftDown)
            {
                cursorEnd = cursorPos;
            }

            RefreshCursorBounds();

            return true;
        }

        /// <summary>
        ///     Handler for End Key keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyEnd(Boolean down)
        {
            if (!down)
            {
                return true;
            }

            cursorPos.X = text[cursorPos.Y].Length;

            if (!InputHandler.IsShiftDown)
            {
                cursorEnd = cursorPos;
            }

            UpdateText();
            RefreshCursorBounds();

            return true;
        }

        /// <summary>
        ///     Handler for Tab Key keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override Boolean OnKeyTab(Boolean down)
        {
            if (!AcceptTabs)
            {
                return base.OnKeyTab(down);
            }

            if (!down)
            {
                return false;
            }

            OnChar(chr: '\t');

            return true;
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

            var str = String.Empty;

            if (StartPoint.Y == EndPoint.Y)
            {
                Int32 start = StartPoint.X;
                Int32 end = EndPoint.X;

                str = text[cursorPos.Y];
                str = str.Substring(start, end - start);
            }
            else
            {
                Point startPoint = StartPoint;
                Point endPoint = EndPoint;

                str = text[startPoint.Y].Substring(startPoint.X) + Environment.NewLine; //Copy start

                for (var i = 1; i < endPoint.Y - startPoint.Y; i++)
                {
                    str += text[startPoint.Y + i] + Environment.NewLine; //Copy middle
                }

                str += text[endPoint.Y].Substring(startIndex: 0, endPoint.X); //Copy end
            }

            return str;
        }

        /// <summary>
        ///     Deletes selected text.
        /// </summary>
        public void EraseSelection()
        {
            if (StartPoint.Y == EndPoint.Y)
            {
                Int32 start = StartPoint.X;
                Int32 end = EndPoint.X;

                text[StartPoint.Y] = text[StartPoint.Y].Remove(start, end - start);
            }
            else
            {
                Point startPoint = StartPoint;
                Point endPoint = EndPoint;

                /* Remove Start */
                if (startPoint.X < text[startPoint.Y].Length)
                {
                    text[startPoint.Y] = text[startPoint.Y].Remove(startPoint.X);
                }

                /* Remove Middle */
                for (var i = 1; i < endPoint.Y - startPoint.Y; i++)
                {
                    text.RemoveLine(startPoint.Y + 1);
                }

                /* Remove End */
                if (endPoint.X < text[startPoint.Y + 1].Length)
                {
                    text[startPoint.Y] += text[startPoint.Y + 1].Substring(endPoint.X);
                }

                text.RemoveLine(startPoint.Y + 1);
            }

            // Move the cursor to the start of the selection, 
            // since the end is probably outside of the string now.
            cursorPos = StartPoint;
            cursorEnd = StartPoint;

            UpdateText();
            OnTextChanged();
            RefreshCursorBounds();
        }

        /// <summary>
        ///     Refreshes the cursor location and selected area when the inner panel scrolls
        /// </summary>
        /// <param name="control">The inner panel the text is embedded in</param>
        /// <param name="args">Event arguments</param>
        private void ScrollChanged(ControlBase control, EventArgs args)
        {
            RefreshCursorBounds(makeCaretVisible: false);
        }

        /// <summary>
        ///     Handler for text changed event.
        /// </summary>
        private void OnTextChanged()
        {
            if (TextChanged != null)
            {
                TextChanged.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Invalidates the control.
        /// </summary>
        /// <remarks>
        ///     Causes layout, repaint, invalidates cached texture.
        /// </remarks>
        private void UpdateText()
        {
            Invalidate();
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

            currentSkin.Renderer.SetClipRegion(Container.Bounds);

            Int32 verticalSize = LineHeight;

            // Draw selection.. if selected..
            if (cursorPos != cursorEnd)
            {
                if (StartPoint.Y == EndPoint.Y)
                {
                    Point pA = GetCharacterPosition(StartPoint);
                    Point pB = GetCharacterPosition(EndPoint);

                    Rectangle selectionBounds = new();
                    selectionBounds.X = Math.Min(pA.X, pB.X);
                    selectionBounds.Y = pA.Y;
                    selectionBounds.Width = Math.Max(pA.X, pB.X) - selectionBounds.X;
                    selectionBounds.Height = verticalSize;

                    currentSkin.Renderer.DrawColor = Skin.colors.textBoxColors.backgroundSelected;
                    currentSkin.Renderer.DrawFilledRect(selectionBounds);
                }
                else
                {
                    /* Start */
                    Point pA = GetCharacterPosition(StartPoint);
                    Point pB = GetCharacterPosition(new Point(text[StartPoint.Y].Length, StartPoint.Y));

                    Rectangle selectionBounds = new();
                    selectionBounds.X = Math.Min(pA.X, pB.X);
                    selectionBounds.Y = pA.Y;
                    selectionBounds.Width = Math.Max(pA.X, pB.X) - selectionBounds.X;
                    selectionBounds.Height = verticalSize;

                    currentSkin.Renderer.DrawColor = Skin.colors.textBoxColors.backgroundSelected;
                    currentSkin.Renderer.DrawFilledRect(selectionBounds);

                    /* Middle */
                    for (var i = 1; i < EndPoint.Y - StartPoint.Y; i++)
                    {
                        pA = GetCharacterPosition(new Point(x: 0, StartPoint.Y + i));
                        pB = GetCharacterPosition(new Point(text[StartPoint.Y + i].Length, StartPoint.Y + i));

                        selectionBounds = new Rectangle();
                        selectionBounds.X = Math.Min(pA.X, pB.X);
                        selectionBounds.Y = pA.Y;
                        selectionBounds.Width = Math.Max(pA.X, pB.X) - selectionBounds.X;
                        selectionBounds.Height = verticalSize;

                        currentSkin.Renderer.DrawColor = Skin.colors.textBoxColors.backgroundSelected;
                        currentSkin.Renderer.DrawFilledRect(selectionBounds);
                    }

                    /* End */
                    pA = GetCharacterPosition(new Point(x: 0, EndPoint.Y));
                    pB = GetCharacterPosition(EndPoint);

                    selectionBounds = new Rectangle();
                    selectionBounds.X = Math.Min(pA.X, pB.X);
                    selectionBounds.Y = pA.Y;
                    selectionBounds.Width = Math.Max(pA.X, pB.X) - selectionBounds.X;
                    selectionBounds.Height = verticalSize;

                    currentSkin.Renderer.DrawColor = Skin.colors.textBoxColors.backgroundSelected;
                    currentSkin.Renderer.DrawFilledRect(selectionBounds);
                }
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

        private Point GetCharacterPosition(Point position)
        {
            Point p = text.GetCharacterPosition(position);

            return new Point(p.X + text.ActualLeft + Padding.Left, p.Y + text.ActualTop + Padding.Top);
        }

        private Point GetClosestCharacter(Int32 px, Int32 py)
        {
            Point p = text.CanvasPosToLocal(new Point(px, py));

            return text.GetClosestCharacter(p);
        }

        protected void RefreshCursorBounds(Boolean makeCaretVisible = true)
        {
            lastInputTime = GwenPlatform.GetTimeInSeconds();

            if (makeCaretVisible)
            {
                MakeCaretVisible();
            }

            Point pA = GetCharacterPosition(CursorPosition);

            caretBounds.X = pA.X;
            caretBounds.Y = pA.Y;

            caretBounds.Width = 1;
            caretBounds.Height = LineHeight;

            Redraw();
        }

        protected virtual void MakeCaretVisible()
        {
            Size viewSize = ViewableContentSize;
            Point caretPos = GetCharacterPosition(CursorPosition);

            caretPos.X -= Padding.Left + text.ActualLeft;
            caretPos.Y -= Padding.Top + text.ActualTop;

            EnsureVisible(
                new Rectangle(caretPos.X, caretPos.Y, width: 5, LineHeight),
                new Size(viewSize.Width / 5, height: 0));
        }
    }
}
