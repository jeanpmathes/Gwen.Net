using System;
using System.Collections.Generic;
using System.Drawing;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Input;

/// <summary>
/// Handles input events and translates them into GWEN events.
/// </summary>
public sealed class InputHandler
{
    private readonly Visual root;

    /// <summary>
    /// The keyboard focus.
    /// </summary>
    public Focus KeyboardFocus { get; } = new();
    
    /// <summary>
    /// The pointer (mouse) focus.
    /// </summary>
    public Focus PointerFocus { get; } = new();

    /// <summary>
    /// Creates a new <seealso cref="InputHandler"/> with the specified root visual.
    /// </summary>
    /// <param name="root">The root visual of the visual tree. Input events will be hit-tested against this visual and its descendants.</param>
    public InputHandler(Visual root)
    {
        this.root = root;
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

    private static List<Visual> GetEventRoute(Visual target)
    {
        var route = new List<Visual>(); // todo: store and reuse this list to avoid allocations
        
        Visual? current = target;
        
        while (current != null)
        {
            route.Add(current);
            current = current.Parent;
        }
        
        return route;
    }

    private static void HandleEvent(InputEvent inputEvent)
    {
        List<Visual> route = GetEventRoute(inputEvent.Target);

        for (Int32 index = route.Count - 1; index >= 0; index--)
        {
            Visual visual = route[index];
            
            inputEvent.SetTarget(visual);
            visual.HandleInputPreview(inputEvent);
            
            if (inputEvent.Handled) return;
        }
        
        for (Int32 index = 0; index < route.Count; index++)
        {
            Visual visual = route[index];
            
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
        Visual? target = GetPointerTarget(position);
        
        if (target == null) 
            return;

        HandleEvent(new PointerMoveEvent(target, position, deltaX, deltaY));
    }
    
    internal void HandleScrollEvent(PointF position, Single deltaX, Single deltaY)
    {
        Visual? target = GetPointerTarget(position);
        
        if (target == null) 
            return;

        HandleEvent(new ScrollEvent(target, position, deltaX, deltaY));
    }

    #endregion EVENTS
}
