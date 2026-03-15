using System;
using System.Drawing;
using Gwen.Net.New.Bindings;
using Gwen.Net.New.Input;

namespace Gwen.Net.New.Visuals;

/// <summary>
///     The root visual for a GWEN user interface.
/// </summary>
/// <seealso cref="Controls.Canvas" />
public class Canvas : Visual
{
    private readonly Slot<InputHandler?> input = new(null);

    /// <summary>
    ///     The input handler for this canvas, responsible for processing input events and determining which visual should
    ///     receive them.
    /// </summary>
    public ReadOnlySlot<InputHandler?> Input => input;

    /// <summary>
    ///     Gets or sets the single child visual.
    /// </summary>
    public Visual? Child
    {
        get => Children.Count > 0 ? Children[0] : null;
        set => SetChild(value);
    }

    /// <inheritdoc />
    public override void OnAttach()
    {
        if (input.GetValue() == null)
            input.SetValue(new InputHandler(this));
    }

    /// <inheritdoc />
    public override void OnDetach(Boolean isReparenting)
    {
        if (isReparenting)
            return;

        input.GetValue()?.Dispose();
        input.SetValue(null);
    }

    /// <inheritdoc />
    public override void OnBoundsChanged(RectangleF oldBounds, RectangleF newBounds)
    {
        InvalidateMeasure();
    }

    /// <inheritdoc />
    public override void Render()
    {
        Renderer.Begin();

        Renderer.PushOffset(Point.Empty);
        Renderer.PushClip(Bounds);

        base.Render();

        Renderer.EndClip();

        Renderer.PopClip();
        Renderer.PopOffset();

        Renderer.End();
    }
}
