using System;
using Gwen.Net.Legacy;
using Gwen.Net.Legacy.Control;

namespace Gwen.Net.Tests.Components.Legacy
{
    [UnitTest(Category = "Containers", Order = 302)]
    public class TreeControlTest : GUnit
    {
        public TreeControlTest(ControlBase parent)
            : base(parent)
        {
            CrossSplitter layout = new(this);
            layout.Dock = Dock.Fill;

            /* Simple Tree Control */
            {
                TreeControl ctrl = new(layout);

                ctrl.AddNode("Node One");
                TreeNode node = ctrl.AddNode("Node Two");

                {
                    node.AddNode("Node Two Inside");

                    node.AddNode("Eyes");

                    {
                        node.AddNode("Brown").AddNode("Node Two Inside").AddNode("Eyes").AddNode("Brown");
                    }

                    TreeNode imgnode = node.AddNode("Image");
                    imgnode.SetImage("Legacy/test16.png");

                    imgnode = node.AddNode("Image_Kids");
                    imgnode.SetImage("Legacy/test16.png", new Size(width: 32, height: 32), Color.Blue);

                    {
                        imgnode.AddNode("Kid1");
                        imgnode.AddNode("Kid2");
                    }

                    node.AddNode("Nodes");
                }

                ctrl.AddNode("Node Three");

                node = ctrl.AddNode("Clickables");

                {
                    TreeNode click = node.AddNode("Single Click");
                    click.Clicked += NodeClicked;
                    click.RightClicked += NodeClicked;

                    click = node.AddNode("Double Click");
                    click.DoubleClicked += NodeDoubleClicked;
                }

                ctrl.ExpandAll();

                ctrl.Selected += NodeSelected;
                ctrl.Expanded += NodeExpanded;
                ctrl.Collapsed += NodeCollapsed;
            }

            /* Multi select Tree Control */
            {
                TreeControl ctrl = new(layout);

                ctrl.AllowMultiSelect = true;

                ctrl.AddNode("Node One");
                TreeNode node = ctrl.AddNode("Node Two");
                node.AddNode("Node Two Inside");
                node.AddNode("Eyes");

                TreeNode nodeTwo = node.AddNode("Brown")
                    .AddNode("Node Two Inside")
                    .AddNode("Eyes");

                nodeTwo.AddNode("Brown");
                nodeTwo.AddNode("Green");
                nodeTwo.AddNode("Slime");
                nodeTwo.AddNode("Grass");
                nodeTwo.AddNode("Pipe");
                node.AddNode("More");
                node.AddNode("Nodes");

                ctrl.AddNode("Node Three");

                ctrl.ExpandAll();

                ctrl.Selected += NodeSelected;
                ctrl.Expanded += NodeExpanded;
                ctrl.Collapsed += NodeCollapsed;
            }

            /* Normal Tree Control (without using the AddNode function */
            {
                TreeControl ctrl = new(layout);

                TreeNode node = new(ctrl);
                node.Text = "First";

                new TreeNode(node).Text = "2nd first";

                node = new TreeNode(ctrl);
                node.Text = "Second";

                node = new TreeNode(node);
                node.Text = "Other 2nd";

                ctrl.ExpandAll();
            }

            /* Not expanded Tree Control */
            {
                TreeControl ctrl = new(layout);

                ctrl.AddNode("Node One");
                TreeNode node = ctrl.AddNode("Node Two");
                node.AddNode("Node Two Inside");
                node.AddNode("Eyes");

                TreeNode nodeTwo = node.AddNode("Brown")
                    .AddNode("Node Two Inside")
                    .AddNode("Eyes");

                nodeTwo.AddNode("Brown");
                nodeTwo.AddNode("Green");
                nodeTwo.AddNode("Slime");
                nodeTwo.AddNode("Grass");
                nodeTwo.AddNode("Pipe");
                node.AddNode("More");
                node.AddNode("Nodes");

                ctrl.AddNode("Node Three");

                ctrl.Selected += NodeSelected;
                ctrl.Expanded += NodeExpanded;
                ctrl.Collapsed += NodeCollapsed;
            }
        }

        private void NodeCollapsed(ControlBase control, EventArgs args)
        {
            var node = (TreeNode) control;
            UnitPrint($"Node collapsed: {node.Text}");
        }

        private void NodeExpanded(ControlBase control, EventArgs args)
        {
            var node = (TreeNode) control;
            UnitPrint($"Node expanded: {node.Text}");
        }

        private void NodeSelected(ControlBase control, EventArgs args)
        {
            var node = (TreeNode) control;
            UnitPrint($"Node selected: {node.Text}");
        }

        private void NodeClicked(ControlBase control, ClickedEventArgs args)
        {
            var node = (TreeNode) control;
            UnitPrint($"Node clicked: {node.Text} @({args.X}, {args.Y})");
        }

        private void NodeDoubleClicked(ControlBase control, ClickedEventArgs args)
        {
            var node = (TreeNode) control;
            UnitPrint($"Node double clicked: {node.Text}");
        }
    }
}
