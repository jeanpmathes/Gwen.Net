using System.Drawing;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.Tests.Unit.New.Input;

/// <summary>
/// Utilities for sending pointer input events that are guaranteed to hit a specific control.
/// </summary>
public static class ControlInputHelper
{
    private static PointF GetHitPoint(Control control)
    {
        Visual visual = control.Visualization.GetValue()
            ?? throw new InvalidOperationException(
                "Control has no visualization. Ensure the control has been attached to a canvas and rendered.");

        PointF localCenter = new(
            visual.Bounds.X + visual.Bounds.Width / 2,
            visual.Bounds.Y + visual.Bounds.Height / 2);

        return visual.LocalPointToRoot(localCenter);
    }

    /// <param name="translator">The input translator to use.</param>
    extension(MockInputTranslator translator)
    {
        /// <summary>
        /// Simulates a full pointer click — button down followed by button up — at the center of the control.
        /// </summary>
        /// <param name="control">The control to click.</param>
        public void Click(Control control)
        {
            PointF point = GetHitPoint(control);

            translator.PointerButtonDown(point);
            translator.PointerButtonUp(point);
        }

        /// <summary>
        /// Simulates a pointer button-down event at the center of the control.
        /// </summary>
        /// <param name="control">The control to press.</param>
        public void PointerButtonDown(Control control)
        {
            translator.PointerButtonDown(GetHitPoint(control));
        }

        /// <summary>
        /// Simulates a pointer button-up event at the center of the control.
        /// </summary>
        /// <param name="control">The control to release over.</param>
        public void PointerButtonUp(Control control)
        {
            translator.PointerButtonUp(GetHitPoint(control));
        }

        /// <summary>
        /// Simulates a pointer move event to the center of the control.
        /// </summary>
        /// <param name="control">The control to move the pointer to.</param>
        public void PointerMove(Control control)
        {
            translator.PointerMove(GetHitPoint(control));
        }
    }
}
