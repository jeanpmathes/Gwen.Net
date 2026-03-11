using Gwen.Net.Legacy;
using Gwen.Net.Legacy.CommonDialog;
using Gwen.Net.Legacy.Components;
using Gwen.Net.Legacy.Control;
using Gwen.Net.Legacy.Control.Layout;

namespace Gwen.Net.Tests.Components.Legacy;

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
            button = new Button(grid);
            button.Margin = Margin.Five;
            button.Text = "OpenFileDialog";

            Label openFile = new(grid);
            openFile.TextPadding = new Padding(left: 3, top: 0, right: 0, bottom: 0);
            openFile.Alignment = Alignment.Left | Alignment.CenterV;

            button.Clicked += (_, _) =>
            {
                openFile.Text = "";

                OpenFileDialog dialog = Component.Create<OpenFileDialog>(this);
                dialog.InitialFolder = "C:\\";
                dialog.Filters = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                dialog.Callback = path => openFile.Text = path ?? "Cancelled";
            };
        }

        {
            button = new Button(grid);
            button.Margin = Margin.Five;
            button.Text = "SaveFileDialog";

            Label saveFile = new(grid);
            saveFile.TextPadding = new Padding(left: 3, top: 0, right: 0, bottom: 0);
            saveFile.Alignment = Alignment.Left | Alignment.CenterV;

            button.Clicked += (_, _) =>
            {
                saveFile.Text = "";
                SaveFileDialog dialog = Component.Create<SaveFileDialog>(this);
                dialog.Callback = path => saveFile.Text = path ?? "Cancelled";
            };
        }

        {
            button = new Button(grid);
            button.Margin = Margin.Five;
            button.Text = "SaveFileDialog (create)";

            Label createFile = new(grid);
            createFile.TextPadding = new Padding(left: 3, top: 0, right: 0, bottom: 0);
            createFile.Alignment = Alignment.Left | Alignment.CenterV;

            button.Clicked += (_, _) =>
            {
                createFile.Text = "";
                SaveFileDialog dialog = Component.Create<SaveFileDialog>(this);
                dialog.Title = "Create File";
                dialog.OkButtonText = "Create";
                dialog.Callback = path => createFile.Text = path ?? "Cancelled";
            };
        }

        {
            button = new Button(grid);
            button.Margin = Margin.Five;
            button.Text = "FolderBrowserDialog";

            Label selectFolder = new(grid);
            selectFolder.TextPadding = new Padding(left: 3, top: 0, right: 0, bottom: 0);
            selectFolder.Alignment = Alignment.Left | Alignment.CenterV;

            button.Clicked += (_, _) =>
            {
                selectFolder.Text = "";
                FolderBrowserDialog dialog = Component.Create<FolderBrowserDialog>(this);
                dialog.InitialFolder = "C:\\";
                dialog.Callback = path => selectFolder.Text = path ?? "Cancelled";
            };
        }
    }
}
