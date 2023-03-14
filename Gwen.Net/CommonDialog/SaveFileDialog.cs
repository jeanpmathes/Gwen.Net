﻿using Gwen.Net.Control;
using static Gwen.Net.Platform.GwenPlatform;

namespace Gwen.Net.CommonDialog
{
    /// <summary>
    ///     Dialog for selecting a file name for saving or creating.
    /// </summary>
    public class SaveFileDialog : FileDialog
    {
        public SaveFileDialog(ControlBase parent)
            : base(parent) {}

        protected override void OnCreated()
        {
            base.OnCreated();

            Title = "Save File";
            OkButtonText = "Save";
        }

        protected override void OnItemSelected(string selectedPath)
        {
            if (FileExists(selectedPath))
            {
                SetCurrentItem(GetFileName(selectedPath));
            }
        }

        protected override bool IsSubmittedNameOk(string submittedPath)
        {
            if (DirectoryExists(submittedPath))
            {
                SetPath(submittedPath);
            }
            else if (FileExists(submittedPath))
            {
                return true;
            }

            return false;
        }

        protected override bool ValidateFileName(string pathToValidate)
        {
            if (DirectoryExists(pathToValidate))
            {
                return false;
            }

            if (FileExists(pathToValidate))
            {
                MessageBox win = MessageBox.Show(
                    View,
                    $"File '{GetFileName(pathToValidate)}' already exists. Do you want to replace it?",
                    Title,
                    buttons: MessageBoxButtons.YesNo);

                win.Dismissed += OnMessageBoxDismissed;
                win.UserData = pathToValidate;

                return false;
            }

            return true;
        }

        private void OnMessageBoxDismissed(ControlBase sender, MessageBoxResultEventArgs args)
        {
            if (args.Result == MessageBoxResult.Yes)
            {
                Close(sender.UserData as string);
            }
        }
    }
}
