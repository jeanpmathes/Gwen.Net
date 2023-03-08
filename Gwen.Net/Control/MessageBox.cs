using System;
using Gwen.Net.Control.Layout;
using Gwen.Net.RichText;

namespace Gwen.Net.Control
{
    public enum MessageBoxButtons
    {
        AbortRetryIgnore,
        Ok,
        OkCancel,
        RetryCancel,
        YesNo,
        YesNoCancel
    }

    public enum MessageBoxResult
    {
        Abort,
        Retry,
        Ignore,
        Ok,
        Cancel,
        Yes,
        No
    }

    public class MessageBoxResultEventArgs : EventArgs
    {
        public MessageBoxResult Result { get; init; }
    }

    /// <summary>
    /// The text used in the message box buttons.
    /// </summary>
    public class MessageBoxButtonTexts
    {
        public string Abort { get; init; } = "Abort";
        public string Retry { get; init; } = "Retry";
        public string Ignore { get; init; } = "Ignore";
        public string Ok { get; init; } = "Ok";
        public string Cancel { get; init; } = "Cancel";
        public string Yes { get; init; } = "Yes";
        public string No { get; init; } = "No";

        /// <summary>
        /// A shared instance of the default text. It is used by the <see cref="MessageBox"/> class by default.
        /// </summary>
        public static MessageBoxButtonTexts Shared { get; set; } = new();
    }
    
    /// <summary>
    ///     Simple message box.
    /// </summary>
    public class MessageBox : Window
    {
        /// <summary>
        ///     Invoked when the message box has been dismissed.
        /// </summary>
        public GwenEventHandler<MessageBoxResultEventArgs> Dismissed { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="text">Message to display.</param>
        /// <param name="caption">Window caption.</param>
        /// <param name="buttonText">The text used in the message box buttons.</param>
        /// <param name="buttons">Message box buttons.</param>
        public MessageBox(ControlBase parent, string text, string caption, 
            MessageBoxButtonTexts buttonText, MessageBoxButtons buttons = MessageBoxButtons.Ok)
            : base(parent)
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;

            Canvas canvas = GetCanvas();
            MaximumSize = new Size((int)(canvas.ActualWidth * 0.8f), canvas.ActualHeight);

            StartPosition = StartPosition.CenterParent;
            Title = caption;
            DeleteOnClose = true;

            DockLayout layout = new(this);

            RichLabel textLabel = new(layout);
            textLabel.Dock = Dock.Fill;
            textLabel.Margin = Margin.Ten;
            textLabel.Document = new Document(text);

            HorizontalLayout buttonsLayout = new(layout);
            buttonsLayout.Dock = Dock.Bottom;
            buttonsLayout.HorizontalAlignment = HorizontalAlignment.Center;

            switch (buttons)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                    CreateButton(buttonsLayout, buttonText.Abort, MessageBoxResult.Abort);
                    CreateButton(buttonsLayout, buttonText.Retry, MessageBoxResult.Retry);
                    CreateButton(buttonsLayout, buttonText.Ignore, MessageBoxResult.Ignore);

                    break;
                case MessageBoxButtons.Ok:
                    CreateButton(buttonsLayout, buttonText.Ok, MessageBoxResult.Ok);

                    break;
                case MessageBoxButtons.OkCancel:
                    CreateButton(buttonsLayout, buttonText.Ok, MessageBoxResult.Ok);
                    CreateButton(buttonsLayout, buttonText.Cancel, MessageBoxResult.Cancel);

                    break;
                case MessageBoxButtons.RetryCancel:
                    CreateButton(buttonsLayout, buttonText.Retry, MessageBoxResult.Retry);
                    CreateButton(buttonsLayout, buttonText.Cancel, MessageBoxResult.Cancel);

                    break;
                case MessageBoxButtons.YesNo:
                    CreateButton(buttonsLayout, buttonText.Yes, MessageBoxResult.Yes);
                    CreateButton(buttonsLayout, buttonText.No, MessageBoxResult.No);

                    break;
                case MessageBoxButtons.YesNoCancel:
                    CreateButton(buttonsLayout, buttonText.Yes, MessageBoxResult.Yes);
                    CreateButton(buttonsLayout, buttonText.No, MessageBoxResult.No);
                    CreateButton(buttonsLayout, buttonText.Cancel, MessageBoxResult.Cancel);

                    break;
            }
        }

        /// <summary>
        ///     Show message box.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="text">Message to display.</param>
        /// <param name="caption">Window caption.</param>
        /// <param name="buttonText">The text used in the message box buttons.</param>
        /// <param name="buttons">Message box buttons.</param>
        /// <returns>Message box.</returns>
        public static MessageBox Show(ControlBase parent, string text,
            string caption = "", MessageBoxButtonTexts buttonText = null, MessageBoxButtons buttons = MessageBoxButtons.Ok)
        {
            MessageBox messageBox = new(parent, text, caption, buttonText ?? MessageBoxButtonTexts.Shared, buttons);

            return messageBox;
        }

        private void CreateButton(ControlBase parent, string text, MessageBoxResult result)
        {
            Button button = new(parent);
            button.Width = 70;
            button.Margin = Margin.Five;
            button.Text = text;
            button.UserData = result;
            button.Clicked += CloseButtonPressed;
            button.Clicked += DismissedHandler;
        }

        private void DismissedHandler(ControlBase control, EventArgs args)
        {
            if (Dismissed != null)
            {
                Dismissed.Invoke(this, new MessageBoxResultEventArgs {Result = (MessageBoxResult)control.UserData});
            }
        }
    }
}
