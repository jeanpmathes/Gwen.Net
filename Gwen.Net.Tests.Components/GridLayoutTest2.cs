using Gwen.Net.Control;
using Gwen.Net.Control.Layout;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Layout", Order = 404)]
    public class GridLayoutTest2 : GUnit
    {
        public GridLayoutTest2(ControlBase parent)
            : base(parent)
        {
            GridLayout grid = CreateGrid(this);
            grid.Dock = Dock.Fill;
        }

        private static GridLayout CreateGrid(ControlBase parent)
        {
            GridLayout grid = new(parent);

            grid.SetColumnWidths(1.0f);
            grid.SetRowHeights(0.2f, 0.8f);

            CreateControl(grid);
            CreateControl(grid);

            return grid;
        }

        private static void CreateControl(ControlBase parent)
        {
            ScrollControl scroll = new(parent);
            VerticalLayout layout = new(scroll);
            
            for (var i = 0; i < 10; i++)
            {
                Button button = new(layout);
                button.Text = $"Button {i}";
            }
        }
    }
}
