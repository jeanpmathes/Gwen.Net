using System;

namespace Gwen.Net.New;

/// <summary>
/// The visibility of a control.
/// </summary>
public enum Visibility
{
    /// <summary>
    /// The control is visible. Only visible controls can receive input and focus.
    /// </summary>
    Visible = 0,

    /// <summary>
    /// The control is hidden, but still takes up space in the layout.
    /// </summary>
    Hidden = 1,

    /// <summary>
    /// The control is collapsed and does not take up any space in the layout.
    /// </summary>
    Collapsed = 2
}

/// <summary>
/// Tools for working with visibility values.
/// </summary>
public static class Visibilities
{
    extension(Visibility visibility) 
    {
        /// <summary>
        /// Whether the element is visible.
        /// </summary>
        public Boolean IsVisible => visibility == Visibility.Visible;
        
        /// <summary>
        /// Whether the element participates in layout. Hidden and visible elements do, collapsed elements don't.
        /// </summary>
        public Boolean IsLayouted => visibility != Visibility.Collapsed;
    }
    
    /// <summary>
    /// Get a visibility value from a boolean. Visible if <c>true</c>, collapsed if <c>false</c>.
    /// </summary>
    /// <param name="visible">Whether the control should be visible.</param>
    /// <returns>A visibility value corresponding to the given boolean.</returns>
    public static Visibility FromBoolean(Boolean visible)
    {
        return visible ? Visibility.Visible : Visibility.Collapsed;
    }
    
    /// <summary>
    /// Get the lower of two visibility values. The lower visibility is the one that is less visible.
    /// </summary>
    public static Visibility Lower(Visibility a, Visibility b)
    {
        return (Visibility) Math.Max((Int32) a, (Int32) b);
    }
}
