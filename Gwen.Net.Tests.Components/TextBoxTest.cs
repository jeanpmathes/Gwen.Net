﻿using System;
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
        private readonly Font m_Font1;
        private readonly Font m_Font2;
        private readonly Font m_Font3;

        public TextBoxTest(ControlBase parent)
            : base(parent)
        {
            m_Font1 = Skin.DefaultFont.Copy();
            m_Font1.FaceName = "Courier New"; // fixed width font!

            m_Font2 = Skin.DefaultFont.Copy();
            m_Font2.FaceName = "Times New Roman";
            m_Font2.Size *= 3;

            m_Font3 = Skin.DefaultFont.Copy();
            m_Font3.Size += 5;

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
                        textbox.Font = m_Font1;
                        textbox.AcceptTabs = true;

                        textbox.SetText(
                            "In olden times when wishing still helped one, there lived a king whose daughters were all beautiful,\nbut the youngest was so beautiful that the sun itself, which has seen so much, \nwas astonished whenever it shone in her face. \nClose by the king's castle lay a great dark forest, \nand under an old lime-tree in the forest was a well, and when the day was very warm, \nthe king's child went out into the forest and sat down by the side of the cool fountain, \nand when she was bored she took a golden ball, and threw it up on high and caught it, \nand this ball was her favorite plaything.");
                    }

                    {
                        Button pad = new(dockLayout);
                        pad.Dock = Dock.Right;
                        pad.Margin = Margin.Five;
                        pad.Text = "Pad";
                        pad.Clicked += (s, a) => new TextPad(this);
                    }
                }

                {
                    TextBox textbox = new(vlayout);
                    textbox.Margin = Margin.Five;

                    textbox.SetText(
                        "In olden times when wishing still helped one, there lived a king whose daughters were all beautiful, but the youngest was so beautiful that the sun itself, which has seen so much, was astonished whenever it shone in her face. Close by the king's castle lay a great dark forest, and under an old lime-tree in the forest was a well, and when the day was very warm, the king's child went out into the forest and sat down by the side of the cool fountain, and when she was bored she took a golden ball, and threw it up on high and caught it, and this ball was her favorite plaything.");

                    textbox.TextColor = Color.Black;
                    textbox.Font = m_Font3;
                }

                {
                    TextBox textbox = new(vlayout);
                    textbox.Margin = Margin.Five;
                    textbox.Width = 150;
                    textbox.HorizontalAlignment = HorizontalAlignment.Right;
                    textbox.SetText("あおい　うみから　やってきた");
                    textbox.TextColor = Color.Black;
                    textbox.Font = m_Font3;
                }

                {
                    TextBox textbox = new(vlayout);
                    textbox.Margin = Margin.Five;
                    textbox.HorizontalAlignment = HorizontalAlignment.Left;
                    textbox.FitToText = "Fit the text";
                    textbox.SetText("FitToText");
                    textbox.TextColor = Color.Black;
                    textbox.Font = m_Font3;
                }

                {
                    TextBox textbox = new(vlayout);
                    textbox.Margin = Margin.Five;
                    textbox.HorizontalAlignment = HorizontalAlignment.Left;
                    textbox.Width = 200;
                    textbox.SetText("Width = 200");
                    textbox.TextColor = Color.Black;
                    textbox.Font = m_Font3;
                }

                {
                    TextBox textbox = new(vlayout);
                    textbox.Margin = Margin.Five;
                    textbox.SetText("Different Font");
                    textbox.Font = m_Font2;
                }
            }
        }

        public override void Dispose()
        {
            m_Font1.Dispose();
            m_Font2.Dispose();
            m_Font3.Dispose();
            base.Dispose();
        }

        private void OnEdit(ControlBase control, EventArgs args)
        {
            var box = control as TextBox;
            UnitPrint(string.Format("TextBox: OnEdit: {0}", box.Text));
        }

        private void OnSubmit(ControlBase control, EventArgs args)
        {
            var box = control as TextBox;
            UnitPrint(string.Format("TextBox: OnSubmit: {0}", box.Text));
        }

        private class TextPad : Window
        {
            private readonly Font m_Font;
            private readonly MultilineTextBox m_TextBox;

            private string m_Path;

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
                fileMenu.Menu.AddItem("Open...", string.Empty, "Ctrl+O").SetAction((s, a) => OnOpen(s, a));
                fileMenu.Menu.AddItem("Save", string.Empty, "Ctrl+S").SetAction((s, a) => OnSave(s, a));
                fileMenu.Menu.AddItem("Save As...").SetAction((s, a) => OnSaveAs(s, a));
                fileMenu.Menu.AddItem("Quit", string.Empty, "Ctrl+Q").SetAction((s, a) => Close());

                m_Font = Skin.DefaultFont.Copy();
                m_Font.FaceName = "Courier New";

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

                m_TextBox = new MultilineTextBox(layout);
                m_TextBox.Dock = Dock.Fill;
                m_TextBox.ShouldDrawBackground = false;
                m_TextBox.Font = m_Font;

                m_TextBox.TextChanged += (sender, arguments) =>
                {
                    lines.Text = m_TextBox.TotalLines.ToString();
                    length.Text = m_TextBox.Text.Length.ToString();
                };

                m_TextBox.Text = "";

                m_Path = null;
            }

            public override void Dispose()
            {
                m_Font.Dispose();
                base.Dispose();
            }

            private void OnOpen(ControlBase sender, EventArgs args)
            {
                var dialog = Component.Create<OpenFileDialog>(this);
                dialog.Title = "Open Text File";
                dialog.OkButtonText = "Open";
                dialog.Filters = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                dialog.InitialFolder =
                    m_Path == null ? GwenPlatform.CurrentDirectory : GwenPlatform.GetDirectoryName(m_Path);

                dialog.Callback = path =>
                {
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        try
                        {
                            StreamReader reader =
                                new(GwenPlatform.GetFileStream(path, isWritable: false), Encoding.UTF8);

                            m_TextBox.Text = reader.ReadToEnd();
                            m_Path = path;
                            Title = GwenPlatform.GetFileName(m_Path) + " - TextPad";
                        }
                        catch (Exception) {}
                    }
                };
            }

            private void OnSave(ControlBase sender, EventArgs args)
            {
                if (m_Path != null)
                {
                    try
                    {
                        StreamWriter writer = new(GwenPlatform.GetFileStream(m_Path, isWritable: true), Encoding.UTF8);
                        writer.Write(m_TextBox.Text);
                    }
                    catch (Exception) {}
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

                if (m_Path == null)
                {
                    dialog.InitialFolder = GwenPlatform.CurrentDirectory;
                }
                else
                {
                    dialog.CurrentItem = m_Path;
                }

                dialog.Callback = path =>
                {
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        try
                        {
                            StreamWriter writer =
                                new(GwenPlatform.GetFileStream(path, isWritable: true), Encoding.UTF8);

                            writer.Write(m_TextBox.Text);
                            m_Path = path;
                            Title = GwenPlatform.GetFileName(m_Path) + " - TextPad";
                        }
                        catch (Exception) {}
                    }
                };
            }
        }
    }
}
