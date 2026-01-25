using System;
using Gwen.Net.Legacy;
using Gwen.Net.Legacy.Control;

namespace Gwen.Net.Tests.Components.Legacy
{
    [UnitTest(Category = "Non-standard", Order = 500)]
    public class CollapsibleListTest : GUnit
    {
        public CollapsibleListTest(ControlBase parent)
            : base(parent)
        {
            CollapsibleList control = new(this);
            control.Dock = Dock.Fill;
            control.HorizontalAlignment = HorizontalAlignment.Left;
            control.ItemSelected += OnSelection;
            control.CategoryCollapsed += OnCollapsed;

            {
                CollapsibleCategory cat = control.Add("Category One");
                cat.Add("Hello");
                cat.Add("Two");
                cat.Add("Three");
                cat.Add("Four");
            }

            {
                CollapsibleCategory cat = control.Add("Shopping");
                cat.Add("Special");
                cat.Add("Two Noses");
                cat.Add("Orange ears");
                cat.Add("Beer");
                cat.Add("Three Eyes");
                cat.Add("Special");
                cat.Add("Two Noses");
                cat.Add("Orange ears");
                cat.Add("Beer");
                cat.Add("Three Eyes");
                cat.Add("Special");
                cat.Add("Two Noses");
                cat.Add("Orange ears");
                cat.Add("Beer");
                cat.Add("Three Eyes");
            }

            {
                CollapsibleCategory cat = control.Add("Category One");
                cat.Add("Hello");
                cat.Add("Two");
                cat.Add("Three");
                cat.Add("Four");
            }
        }

        private void OnSelection(ControlBase control, EventArgs args)
        {
            var list = (CollapsibleList) control;
            UnitPrint($"CollapsibleList: Selected: {list.GetSelectedButton().Text}");
        }

        private void OnCollapsed(ControlBase control, EventArgs args)
        {
            var cat = (CollapsibleCategory) control;
            UnitPrint($"CollapsibleCategory: CategoryCollapsed: {cat.Text} {cat.IsCollapsed}");
        }
    }
}
