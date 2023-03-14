using System;
using Gwen.Net.Control;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Standard", Order = 208)]
    public class MenuTest : GUnit
    {
        private readonly Menu contextMenu;

        public MenuTest(ControlBase parent)
            : base(parent)
        {
            /* Menu Strip */
            {
                MenuStrip menu = new(this);
                menu.Dock = Dock.Top;

                /* File */
                {
                    MenuItem root = menu.AddItem("File");
                    root.Menu.AddItem("Load", "test16.png", "Ctrl+L").SetAction(MenuItemSelect);
                    root.Menu.AddItem("Save", string.Empty, "Ctrl+S").SetAction(MenuItemSelect);
                    root.Menu.AddItem("Save As..", string.Empty, "Ctrl+A").SetAction(MenuItemSelect);
                    root.Menu.AddItem("Quit", string.Empty, "Ctrl+Q").SetAction(MenuItemSelect);
                }

                /* Russian */
                {
                    MenuItem pRoot = menu.AddItem("\u043F\u0438\u0440\u0430\u0442\u0441\u0442\u0432\u043E");
                    pRoot.Menu.AddItem("\u5355\u5143\u6D4B\u8BD5").SetAction(MenuItemSelect);

                    pRoot.Menu.AddItem("\u0111\u01A1n v\u1ECB th\u1EED nghi\u1EC7m", "test16.png")
                        .SetAction(MenuItemSelect);
                }

                {
                    MenuItem sRoot = menu.AddItemPath("File/s/d/s/s/d/f");
                }

                /* Embedded Menu Items */
                {
                    MenuItem pRoot = menu.AddItem("Submenu");

                    MenuItem pCheckable = pRoot.Menu.AddItem("Checkable");
                    pCheckable.IsCheckable = true;
                    pCheckable.IsCheckable = true;

                    {
                        MenuItem pRootB = pRoot.Menu.AddItem("Two");
                        pRootB.Menu.AddItem("Two.One");
                        pRootB.Menu.AddItem("Two.Two");
                        pRootB.Menu.AddItem("Two.Three");
                        pRootB.Menu.AddItem("Two.Four");
                        pRootB.Menu.AddItem("Two.Five");
                        pRootB.Menu.AddItem("Two.Six");
                        pRootB.Menu.AddItem("Two.Seven");
                        pRootB.Menu.AddItem("Two.Eight");
                        pRootB.Menu.AddItem("Two.Nine", "test16.png");
                    }

                    pRoot.Menu.AddItem("Three");
                    pRoot.Menu.AddItem("Four");
                    pRoot.Menu.AddItem("Five");

                    {
                        MenuItem pRootB = pRoot.Menu.AddItem("Six");
                        pRootB.Menu.AddItem("Six.One");
                        pRootB.Menu.AddItem("Six.Two");
                        pRootB.Menu.AddItem("Six.Three");
                        pRootB.Menu.AddItem("Six.Four");
                        pRootB.Menu.AddItem("Six.Five", "test16.png");

                        {
                            MenuItem pRootC = pRootB.Menu.AddItem("Six.Six");
                            pRootC.Menu.AddItem("Sheep");
                            pRootC.Menu.AddItem("Goose");

                            {
                                MenuItem pRootD = pRootC.Menu.AddItem("Camel");
                                pRootD.Menu.AddItem("Eyes");
                                pRootD.Menu.AddItem("Nose");

                                {
                                    MenuItem pRootE = pRootD.Menu.AddItem("Hair");
                                    pRootE.Menu.AddItem("Blonde");
                                    pRootE.Menu.AddItem("Black");

                                    {
                                        MenuItem pRootF = pRootE.Menu.AddItem("Red");
                                        pRootF.Menu.AddItem("Light");
                                        pRootF.Menu.AddItem("Medium");
                                        pRootF.Menu.AddItem("Dark");
                                    }

                                    pRootE.Menu.AddItem("Brown");
                                }

                                pRootD.Menu.AddItem("Ears");
                            }

                            pRootC.Menu.AddItem("Duck");
                        }

                        pRootB.Menu.AddItem("Six.Seven");
                        pRootB.Menu.AddItem("Six.Eight");
                        pRootB.Menu.AddItem("Six.Nine");
                    }

                    pRoot.Menu.AddItem("Seven");
                }
            }

            /* Context Menu Strip */
            {
                Label lblClickMe = new(this);
                lblClickMe.Dock = Dock.Fill;
                lblClickMe.VerticalAlignment = VerticalAlignment.Center;
                lblClickMe.Text = "Right Click Me";

                contextMenu = new Menu(this);
                contextMenu.AddItem("Test");

                contextMenu.AddItem("Clickable").Clicked += (_, _) =>
                {
                    UnitPrint("Clickable item was clicked");
                };

                lblClickMe.RightClicked += (_, args) =>
                {
                    contextMenu.Position = CanvasPosToLocal(new Point(args.X, args.Y));
                    contextMenu.Show();
                };
            }
        }

        private void MenuItemSelect(ControlBase control, EventArgs args)
        {
            var item = (MenuItem) control;
            UnitPrint($"Menu item selected: {item.Text}");
        }
    }
}
