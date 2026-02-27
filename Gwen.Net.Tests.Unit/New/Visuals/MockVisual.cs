using Gwen.Net.New.Input;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.Tests.Unit.New.Visuals;

public class MockVisual : Visual
{
    public Action<InputEvent>? OnInputPreviewHandler { get; set; }
    public Action<InputEvent>? OnInputHandler { get; set; }
    public Action? OnPointerEnterHandler { get; set; }
    public Action? OnPointerLeaveHandler { get; set; }

    public void SetChildVisual(Visual? child) => SetChild(child);
    public void AddChildVisual(Visual child) => AddChild(child);

    public override void OnInputPreview(InputEvent inputEvent)
    {
        OnInputPreviewHandler?.Invoke(inputEvent);
    }

    public override void OnInput(InputEvent inputEvent)
    {
        OnInputHandler?.Invoke(inputEvent);
    }

    public override void OnPointerEnter()
    {
        OnPointerEnterHandler?.Invoke();
    }

    public override void OnPointerLeave()
    {
        OnPointerLeaveHandler?.Invoke();
    }
}
