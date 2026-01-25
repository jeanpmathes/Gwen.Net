using System;
using Gwen.Net.Legacy.Control;
using static Gwen.Net.Legacy.Platform.GwenPlatform;

namespace Gwen.Net.Legacy.CommonDialog
{
    /// <summary>
    ///     Dialog for selecting an existing file.
    /// </summary>
    public class OpenFileDialog : FileDialog
    {
        public OpenFileDialog(ControlBase parent)
            : base(parent) {}

        protected override void OnCreated()
        {
            base.OnCreated();

            Title = "Open File";
            OkButtonText = "Open";
            EnableNewFolder = false;
        }

        protected override void OnItemSelected(String selectedPath)
        {
            if (FileExists(selectedPath))
            {
                SetCurrentItem(GetFileName(selectedPath));
            }
        }

        protected override Boolean IsSubmittedNameOk(String submittedPath)
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

        protected override Boolean ValidateFileName(String pathToValidate)
        {
            return FileExists(pathToValidate);
        }
    }
}
