using Gwen.Net.Control;
using Gwen.Net.Input;
using Gwen.Net.Renderer;
using Gwen.Net.Skin;

namespace Gwen.Net
{
    /// <summary>
    ///     Tooltip handling.
    /// </summary>
    public static class ToolTip
    {
        private static ControlBase toolTip;

        /// <summary>
        ///     Enables tooltip display for the specified control.
        /// </summary>
        /// <param name="control">Target control.</param>
        public static void Enable(ControlBase control)
        {
            if (null == control.ToolTip)
            {
                return;
            }

            ControlBase localToolTip = control.ToolTip;
            toolTip = control;
            localToolTip.DoMeasure(Size.Infinity);
            localToolTip.DoArrange(new Rectangle(Point.Zero, localToolTip.MeasuredSize));
        }

        /// <summary>
        ///     Disables tooltip display for the specified control.
        /// </summary>
        /// <param name="control">Target control.</param>
        public static void Disable(ControlBase control)
        {
            if (toolTip == control)
            {
                toolTip = null;
            }
        }

        /// <summary>
        ///     Disables tooltip display for the specified control.
        /// </summary>
        /// <param name="control">Target control.</param>
        public static void ControlDeleted(ControlBase control)
        {
            Disable(control);
        }

        /// <summary>
        ///     Renders the currently visible tooltip.
        /// </summary>
        /// <param name="skin"></param>
        public static void RenderToolTip(SkinBase skin)
        {
            if (null == toolTip)
            {
                return;
            }

            RendererBase render = skin.Renderer;

            Point oldRenderOffset = render.RenderOffset;
            Point mousePos = InputHandler.MousePosition;
            Rectangle bounds = toolTip.ToolTip.Bounds;

            Rectangle offset = Util.FloatRect(
                mousePos.X - (bounds.Width / 2),
                mousePos.Y - bounds.Height - 10,
                bounds.Width,
                bounds.Height);

            offset = Util.ClampRectToRect(offset, toolTip.GetCanvas().Bounds);

            //Calculate offset on screen bounds
            render.AddRenderOffset(offset);
            render.EndClip();

            skin.DrawToolTip(toolTip.ToolTip);
            toolTip.ToolTip.DoRender(skin);

            render.RenderOffset = oldRenderOffset;
        }
    }
}
