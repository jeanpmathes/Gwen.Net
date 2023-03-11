using Gwen.Net.CommonDialog;
using Gwen.Net.Components;
using Gwen.Net.Control;
using Gwen.Net.Control.Layout;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Components", Order = 600)]
    public class CommonDialogsTest : GUnit
    {
        public CommonDialogsTest(ControlBase parent)
            : base(parent)
        {
            GridLayout grid = new(this);
            grid.Dock = Dock.Fill;
            grid.SetColumnWidths(GridLayout.AutoSize, GridLayout.Fill);

            Button button;

            {
                Label openFile = null;

                button = new Button(grid);
                button.Margin = Margin.Five;
                button.Text = "OpenFileDialog";

                button.Clicked += (sender, args) =>
                {
                    openFile.Text = "";

                    var dialog = Component.Create<OpenFileDialog>(this);
                    dialog.InitialFolder = "C:\\";
                    dialog.Filters = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                    dialog.Callback = path => openFile.Text = path != null ? path : "Cancelled";
                };

                openFile = new Label(grid);
                openFile.TextPadding = new Padding(left: 3, top: 0, right: 0, bottom: 0);
                openFile.Alignment = Alignment.Left | Alignment.CenterV;
            }

            {
                Label saveFile = null;

                button = new Button(grid);
                button.Margin = Margin.Five;
                button.Text = "SaveFileDialog";

                button.Clicked += (sender, args) =>
                {
                    saveFile.Text = "";
                    var dialog = Component.Create<SaveFileDialog>(this);
                    dialog.Callback = path => saveFile.Text = path != null ? path : "Cancelled";
                };

                saveFile = new Label(grid);
                saveFile.TextPadding = new Padding(left: 3, top: 0, right: 0, bottom: 0);
                saveFile.Alignment = Alignment.Left | Alignment.CenterV;
            }

            {
                Label createFile = null;

                button = new Button(grid);
                button.Margin = Margin.Five;
                button.Text = "SaveFileDialog (create)";

                button.Clicked += (sender, args) =>
                {
                    createFile.Text = "";
                    var dialog = Component.Create<SaveFileDialog>(this);
                    dialog.Title = "Create File";
                    dialog.OkButtonText = "Create";
                    dialog.Callback = path => createFile.Text = path != null ? path : "Cancelled";
                };

                createFile = new Label(grid);
                createFile.TextPadding = new Padding(left: 3, top: 0, right: 0, bottom: 0);
                createFile.Alignment = Alignment.Left | Alignment.CenterV;
            }

            {
                Label selectFolder = null;

                button = new Button(grid);
                button.Margin = Margin.Five;
                button.Text = "FolderBrowserDialog";

                button.Clicked += (sender, args) =>
                {
                    selectFolder.Text = "";
                    var dialog = Component.Create<FolderBrowserDialog>(this);
                    dialog.InitialFolder = "C:\\";
                    dialog.Callback = path => selectFolder.Text = path != null ? path : "Cancelled";
                };

                selectFolder = new Label(grid);
                selectFolder.TextPadding = new Padding(left: 3, top: 0, right: 0, bottom: 0);
                selectFolder.Alignment = Alignment.Left | Alignment.CenterV;
            }
        }
    }
}
