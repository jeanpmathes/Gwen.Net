using System;
using Gwen.Net.Legacy.Control;
using static Gwen.Net.Legacy.Platform.GwenPlatform;

namespace Gwen.Net.Legacy.CommonDialog
{
    /// <summary>
    ///     Dialog for selecting an existing directory.
    /// </summary>
    public class FolderBrowserDialog : FileDialog
    {
        public FolderBrowserDialog(ControlBase parent)
            : base(parent) {}

        protected override void OnCreated()
        {
            base.OnCreated();

            FoldersOnly = true;
            Title = "Select Folder";
            OkButtonText = "Select";
        }

        protected override void OnItemSelected(String selectedPath)
        {
            if (DirectoryExists(selectedPath))
            {
                SetCurrentItem(GetFileName(selectedPath));
            }
        }

        protected override Boolean IsSubmittedNameOk(String submittedPath)
        {
            if (DirectoryExists(submittedPath))
            {
                SetPath(submittedPath);

                return true;
            }

            return false;
        }

        protected override Boolean ValidateFileName(String pathToValidate)
        {
            return DirectoryExists(pathToValidate);
        }
    }
}
