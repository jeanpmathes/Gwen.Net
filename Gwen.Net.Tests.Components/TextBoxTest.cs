using System;
using System.IO;
using System.Text;
using Gwen.Net.CommonDialog;
using Gwen.Net.Components;
using Gwen.Net.Control;
using Gwen.Net.Control.Layout;
using Gwen.Net.Platform;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Standard", Order = 201)]
    public class TextBoxTest : GUnit
    {
        private readonly Font font1;
        private readonly Font font2;
        private readonly Font font3;

        public TextBoxTest(ControlBase parent)
            : base(parent)
        {
            font1 = Skin.DefaultFont.Copy();
            font1.FaceName = "Courier New"; // fixed width font!

            font2 = Skin.DefaultFont.Copy();
            font2.FaceName = "Times New Roman";
            font2.Size *= 3;

            font3 = Skin.DefaultFont.Copy();
            font3.Size += 5;

            VerticalLayout vlayout = new(this);

            {
                DockLayout dockLayout = new(vlayout);

                {
                    VerticalLayout vlayout2 = new(dockLayout);
                    vlayout2.Dock = Dock.Left;
                    vlayout2.Width = 200;

                    {
                        /* Vanilla Textbox */
                        {
                            TextBox textbox = new(vlayout2);
                            textbox.Margin = Margin.Five;
                            textbox.SetText("Type something here");
                            textbox.TextChanged += OnEdit;
                            textbox.SubmitPressed += OnSubmit;
                        }

                        {
                            TextBoxPassword textbox = new(vlayout2);
                            textbox.Margin = Margin.Five;
                            //textbox.MaskCharacter = '@';
                            textbox.SetText("secret");
                            textbox.TextChanged += OnEdit;
                        }

                        {
                            TextBox textbox = new(vlayout2);
                            textbox.Margin = Margin.Five;
                            textbox.SetText("Select All Text On Focus");
                            textbox.SelectAllOnFocus = true;
                        }

                        {
                            TextBox textbox = new(vlayout2);
                            textbox.Margin = Margin.Five;
                            textbox.SetText("Different Coloured Text, for some reason");
                            textbox.TextColor = Color.Green;
                        }

                        {
                            TextBox textbox = new TextBoxNumeric(vlayout2);
                            textbox.Margin = Margin.Five;
                            textbox.SetText("200456698");
                            textbox.TextColor = Color.Red;
                        }
                    }

                    /* Multiline Textbox */
                    {
                        MultilineTextBox textbox = new(dockLayout);
                        textbox.Dock = Dock.Fill;
                        textbox.Margin = Margin.Five;
                        textbox.Font = font1;
                        textbox.AcceptTabs = true;

                        textbox.SetText(
                            "In olden times when wishing still helped one, there lived a king whose daughters were all beautiful,\nbut the youngest was so beautiful that the sun itself, which has seen so much, \nwas astonished whenever it shone in her face. \nClose by the king's castle lay a great dark forest, \nand under an old lime-tree in the forest was a well, and when the day was very warm, \nthe king's child went out into the forest and sat down by the side of the cool fountain, \nand when she was bored she took a golden ball, and threw it up on high and caught it, \nand this ball was her favorite plaything.");
                    }

                    {
                        Button pad = new(dockLayout);
                        pad.Dock = Dock.Right;
                        pad.Margin = Margin.Five;
                        pad.Text = "Pad";
                        pad.Clicked += (_, _) => new TextPad(this);
                    }
                }

                {
                    TextBox textbox = new(vlayout);
                    textbox.Margin = Margin.Five;

                    textbox.SetText(
                        "In olden times when wishing still helped one, there lived a king whose daughters were all beautiful, but the youngest was so beautiful that the sun itself, which has seen so much, was astonished whenever it shone in her face. Close by the king's castle lay a great dark forest, and under an old lime-tree in the forest was a well, and when the day was very warm, the king's child went out into the forest and sat down by the side of the cool fountain, and when she was bored she took a golden ball, and threw it up on high and caught it, and this ball was her favorite plaything.");

                    textbox.TextColor = Color.Black;
                    textbox.Font = font3;
                }

                {
                    TextBox textbox = new(vlayout);
                    textbox.Margin = Margin.Five;
                    textbox.Width = 150;
                    textbox.HorizontalAlignment = HorizontalAlignment.Right;
                    textbox.SetText("あおい　うみから　やってきた");
                    textbox.TextColor = Color.Black;
                    textbox.Font = font3;
                }

                {
                    TextBox textbox = new(vlayout);
                    textbox.Margin = Margin.Five;
                    textbox.HorizontalAlignment = HorizontalAlignment.Left;
                    textbox.FitToText = "Fit the text";
                    textbox.SetText("FitToText");
                    textbox.TextColor = Color.Black;
                    textbox.Font = font3;
                }

                {
                    TextBox textbox = new(vlayout);
                    textbox.Margin = Margin.Five;
                    textbox.HorizontalAlignment = HorizontalAlignment.Left;
                    textbox.Width = 200;
                    textbox.SetText("Width = 200");
                    textbox.TextColor = Color.Black;
                    textbox.Font = font3;
                }

                {
                    TextBox textbox = new(vlayout);
                    textbox.Margin = Margin.Five;
                    textbox.SetText("Different Font");
                    textbox.Font = font2;
                }
            }
        }

        public override void Dispose()
        {
            font1.Dispose();
            font2.Dispose();
            font3.Dispose();
            base.Dispose();
        }

        private void OnEdit(ControlBase control, EventArgs args)
        {
            var box = (TextBox) control;
            UnitPrint($"TextBox: OnEdit: {box.Text}");
        }

        private void OnSubmit(ControlBase control, EventArgs args)
        {
            var box = (TextBox) control;
            UnitPrint($"TextBox: OnSubmit: {box.Text}");
        }

        private class TextPad : Window
        {
            private readonly Font font;
            private readonly MultilineTextBox textBox;

            private string path;

            public TextPad(ControlBase parent)
                : base(parent)
            {
                StartPosition = StartPosition.CenterParent;
                Size = new Size(width: 400, height: 300);
                Padding = new Padding(left: 1, top: 0, right: 1, bottom: 1);
                Title = "TextPad";

                DockLayout layout = new(this);
                layout.Dock = Dock.Fill;

                MenuStrip menuStrip = new(layout);
                menuStrip.Dock = Dock.Top;
                MenuItem fileMenu = menuStrip.AddItem("File");
                fileMenu.Menu.AddItem("Open...", string.Empty, "Ctrl+O").SetAction(OnOpen);
                fileMenu.Menu.AddItem("Save", string.Empty, "Ctrl+S").SetAction(OnSave);
                fileMenu.Menu.AddItem("Save As...").SetAction(OnSaveAs);
                fileMenu.Menu.AddItem("Quit", string.Empty, "Ctrl+Q").SetAction((_, _) => Close());

                font = Skin.DefaultFont.Copy();
                font.FaceName = "Courier New";

                StatusBar statusBar = new(layout);
                statusBar.Dock = Dock.Bottom;

                Label length = new(statusBar);
                length.Margin = new Margin(left: 5, top: 0, right: 5, bottom: 0);

                Label label = new(statusBar);
                label.Margin = new Margin(left: 5, top: 0, right: 5, bottom: 0);
                label.Text = "Length:";

                Label lines = new(statusBar);
                lines.Margin = new Margin(left: 5, top: 0, right: 5, bottom: 0);

                label = new Label(statusBar);
                label.Margin = new Margin(left: 5, top: 0, right: 5, bottom: 0);
                label.Text = "Lines:";

                textBox = new MultilineTextBox(layout);
                textBox.Dock = Dock.Fill;
                textBox.ShouldDrawBackground = false;
                textBox.Font = font;

                textBox.TextChanged += (_, _) =>
                {
                    lines.Text = textBox.TotalLines.ToString();
                    length.Text = textBox.Text.Length.ToString();
                };

                textBox.Text = "";

                path = null;
            }

            public override void Dispose()
            {
                font.Dispose();
                base.Dispose();
            }

            private void OnOpen(ControlBase sender, EventArgs args)
            {
                var dialog = Component.Create<OpenFileDialog>(this);
                dialog.Title = "Open Text File";
                dialog.OkButtonText = "Open";
                dialog.Filters = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                dialog.InitialFolder =
                    path == null ? GwenPlatform.CurrentDirectory : GwenPlatform.GetDirectoryName(path);

                dialog.Callback = newPath =>
                {
                    if (string.IsNullOrWhiteSpace(newPath)) return;

                    try
                    {
                        StreamReader reader =
                            new(GwenPlatform.GetFileStream(newPath, isWritable: false), Encoding.UTF8);

                        textBox.Text = reader.ReadToEnd();
                        path = newPath;
                        Title = GwenPlatform.GetFileName(path) + " - TextPad";
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                };
            }

            private void OnSave(ControlBase sender, EventArgs args)
            {
                if (path != null)
                {
                    try
                    {
                        StreamWriter writer = new(GwenPlatform.GetFileStream(path, isWritable: true), Encoding.UTF8);
                        writer.Write(textBox.Text);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                else
                {
                    OnSaveAs(sender, args);
                }
            }

            private void OnSaveAs(ControlBase sender, EventArgs args)
            {
                var dialog = Component.Create<SaveFileDialog>(this);
                dialog.Title = "Save Text File As";
                dialog.OkButtonText = "Save";
                dialog.Filters = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                if (path == null)
                {
                    dialog.InitialFolder = GwenPlatform.CurrentDirectory;
                }
                else
                {
                    dialog.CurrentItem = path;
                }

                dialog.Callback = newPath =>
                {
                    if (!string.IsNullOrWhiteSpace(newPath))
                    {
                        try
                        {
                            StreamWriter writer =
                                new(GwenPlatform.GetFileStream(newPath, isWritable: true), Encoding.UTF8);

                            writer.Write(textBox.Text);
                            path = newPath;
                            Title = GwenPlatform.GetFileName(path) + " - TextPad";
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                };
            }
        }
    }
}
