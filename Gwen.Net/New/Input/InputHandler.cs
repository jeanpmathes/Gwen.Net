using System;
using System.Drawing;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Input;

/// <summary>
/// Handles input events and translates them into GWEN events.
/// </summary>
public sealed class InputHandler : IDisposable
{
    private readonly Visual root;
    
    private Visual? hoveredVisual;
    private Route hoverRoute = Route.Empty;
    
    private PointF lastPointerPosition;

    /// <summary>
    /// The keyboard focus.
    /// </summary>
    public Focus KeyboardFocus { get; }
    
    /// <summary>
    /// The pointer (mouse) focus.
    /// </summary>
    public Focus PointerFocus { get; }

    /// <summary>
    /// Creates a new <seealso cref="InputHandler"/> with the specified root visual.
    /// </summary>
    /// <param name="root">The root visual of the visual tree. Input events will be hit-tested against this visual and its descendants.</param>
    public InputHandler(Visual root)
    {
        this.root = root;
        
        KeyboardFocus = new Focus(OnKeyboardFocusChanged);
        PointerFocus = new Focus(OnPointerFocusChanged);
    }
    
    private void OnKeyboardFocusChanged()
    {
        // Nothing to do.
    }
    
    private void OnPointerFocusChanged()
    {
        UpdateHoveredVisual(PointerFocus.GetFocused() ?? PerformHitTest(lastPointerPosition));
    }
    
    private Visual? PerformHitTest(PointF point)
    {
        Visual current = root;
        
        if (!current.Bounds.Contains(point)) 
            return null;
        
        while (true)
        {
            var foundChild = false;
            
            for (Int32 index = current.Children.Count - 1; index >= 0; index--)
            {
                Visual child = current.Children[index];

                if (!child.Bounds.Contains(child.RootPointToLocal(point))) 
                    continue;
                
                current = child;
                foundChild = true;
                
                break;
            }

            if (!foundChild) 
                return current;
        }
    }

    private Visual? GetKeyboardTarget()
    {
        return KeyboardFocus.GetFocused();
    } 
    
    private Visual? GetPointerTarget(PointF point)
    {
        return PointerFocus.GetFocused() ?? PerformHitTest(point);
    }

    private static void HandleEvent(InputEvent inputEvent)
    {
        using var route = Route.Create(inputEvent.Target);

        for (var index = 0; index < route.Count; index++)
        {
            Visual visual = route.GetFromTop(index);
            
            inputEvent.SetTarget(visual);
            visual.HandleInputPreview(inputEvent);
            
            if (inputEvent.Handled) return;
        }
        
        for (var index = 0; index < route.Count; index++)
        {
            Visual visual = route.GetFromBottom(index);
            
            inputEvent.SetTarget(visual);
            visual.HandleInput(inputEvent);

            if (inputEvent.Handled) return;
        }
    }

    #region EVENTS
    
    internal void HandleKeyEvent(Key key, Boolean isDown, Boolean isRepeat, ModifierKeys modifiers)
    {
        Visual? target = GetKeyboardTarget();
        
        if (target == null) 
            return;
        
        HandleEvent(new KeyEvent(target, key, isDown, isRepeat, modifiers));
    }
    
    internal void HandleTextEvent(String text)
    {
        Visual? target = GetKeyboardTarget();
        
        if (target == null) 
            return;

        HandleEvent(new TextEvent(target, text));
    }
    
    internal void HandlePointerButtonEvent(PointF position, PointerButton button, Boolean isDown, ModifierKeys modifiers)
    {
        Visual? target = GetPointerTarget(position);
        
        if (target == null) 
            return;

        HandleEvent(new PointerButtonEvent(target, position, button, isDown, modifiers));
    }
    
    internal void HandlePointerMoveEvent(PointF position, Single deltaX, Single deltaY)
    {
        lastPointerPosition = position;
        
        Visual? target = GetPointerTarget(position);

        if (target != null)
        {
            HandleEvent(new PointerMoveEvent(target, position, deltaX, deltaY));
        }
        
        UpdateHoveredVisual(PointerFocus.GetFocused() ?? target);
    }

    private void UpdateHoveredVisual(Visual? visual)
    {
        if (visual == hoveredVisual) 
            return;
        
        var newHoverRoute = Route.Create(visual);
        Int32 firstDifferentIndex = Route.FindFirstDifferenceFromTop(hoverRoute, newHoverRoute);

        for (Int32 index = firstDifferentIndex; index < hoverRoute.Count; index++)
            hoverRoute.GetFromTop(index).HandlePointerLeave();
        
        for (Int32 index = firstDifferentIndex; index < newHoverRoute.Count; index++)
            newHoverRoute.GetFromTop(index).HandlePointerEnter();
        
        hoverRoute.Dispose();
        hoverRoute = newHoverRoute;
        
        hoveredVisual = visual;
    }

    internal void HandleScrollEvent(PointF position, Single deltaX, Single deltaY)
    {
        Visual? target = GetPointerTarget(position);
        
        if (target == null) 
            return;

        HandleEvent(new ScrollEvent(target, position, deltaX, deltaY));
    }

    #endregion EVENTS

    /// <inheritdoc/>
    public void Dispose()
    {
        hoverRoute.Dispose();
    }
}
