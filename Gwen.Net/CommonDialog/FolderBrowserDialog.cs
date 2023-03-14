using Gwen.Net.Control;
using static Gwen.Net.Platform.GwenPlatform;

namespace Gwen.Net.CommonDialog
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

        protected override void OnItemSelected(string selectedPath)
        {
            if (DirectoryExists(selectedPath))
            {
                SetCurrentItem(GetFileName(selectedPath));
            }
        }

        protected override bool IsSubmittedNameOk(string submittedPath)
        {
            if (DirectoryExists(submittedPath))
            {
                SetPath(submittedPath);

                return true;
            }

            return false;
        }

        protected override bool ValidateFileName(string pathToValidate)
        {
            return DirectoryExists(pathToValidate);
        }
    }
}
