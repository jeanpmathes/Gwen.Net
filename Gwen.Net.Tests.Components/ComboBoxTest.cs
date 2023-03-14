using System;
using Gwen.Net.Control;
using Gwen.Net.Control.Layout;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Standard", Order = 204)]
    public class ComboBoxTest : GUnit
    {
        public ComboBoxTest(ControlBase parent)
            : base(parent)
        {
            VerticalLayout layout = new(this);

            {
                ComboBox combo = new(layout);
                combo.Margin = Margin.Five;
                combo.Width = 200;

                combo.AddItem("Option One", "one");
                combo.AddItem("Number Two", "two");
                combo.AddItem("Door Three", "three");
                combo.AddItem("Four Legs", "four");
                combo.AddItem("Five Birds", "five");

                combo.ItemSelected += OnComboSelect;
            }

            {
                // Empty
                ComboBox combo = new(layout);
                combo.Margin = Margin.Five;
                combo.Width = 200;
            }

            {
                // Lots of things
                ComboBox combo = new(layout);
                combo.Margin = Margin.Five;
                combo.Width = 200;

                for (var i = 0; i < 500; i++)
                {
                    combo.AddItem($"Option {i}");
                }

                combo.ItemSelected += OnComboSelect;
            }

            {
                // Editable
                EditableComboBox combo = new(layout);
                combo.Margin = Margin.Five;
                combo.Width = 200;

                combo.AddItem("Option One", "one");
                combo.AddItem("Number Two", "two");
                combo.AddItem("Door Three", "three");
                combo.AddItem("Four Legs", "four");
                combo.AddItem("Five Birds", "five");

                combo.ItemSelected += (_, _) =>
                    UnitPrint($"ComboBox: OnComboSelect: {combo.SelectedItem.Text}");

                combo.TextChanged += (_, _) => UnitPrint($"ComboBox: OnTextChanged: {combo.Text}");
                combo.SubmitPressed += (_, _) => UnitPrint($"ComboBox: OnSubmitPressed: {combo.Text}");
            }

            {
                HorizontalLayout hlayout = new(layout);

                {
                    // In-Code Item Change
                    ComboBox combo = new(hlayout);
                    combo.Margin = Margin.Five;
                    combo.Width = 200;

                    MenuItem triangle = combo.AddItem("Triangle");
                    combo.AddItem("Red", "color");
                    combo.AddItem("Apple", "fruit");
                    combo.AddItem("Blue", "color");
                    combo.AddItem("Green", "color", userData: 12);
                    combo.ItemSelected += OnComboSelect;

                    //Select by Menu Item
                    {
                        Button triangleButton = new(hlayout);
                        triangleButton.Text = "Triangle";
                        triangleButton.Width = 100;

                        triangleButton.Clicked += delegate { combo.SelectedItem = triangle; };
                    }

                    //Select by Text
                    {
                        Button testButton = new(hlayout);
                        testButton.Text = "Red";
                        testButton.Width = 100;

                        testButton.Clicked += delegate { combo.SelectByText("Red"); };
                    }

                    //Select by Name
                    {
                        Button testButton = new(hlayout);
                        testButton.Text = "Apple";
                        testButton.Width = 100;

                        testButton.Clicked += delegate { combo.SelectByName("fruit"); };
                    }

                    //Select by UserData
                    {
                        Button testButton = new(hlayout);
                        testButton.Text = "Green";
                        testButton.Width = 100;

                        testButton.Clicked += delegate { combo.SelectByUserData(userdata: 12); };
                    }
                }
            }
        }

        private void OnComboSelect(ControlBase control, EventArgs args)
        {
            var combo = (ComboBox) control;
            UnitPrint($"ComboBox: OnComboSelect: {combo.SelectedItem.Text}");
        }
    }
}
