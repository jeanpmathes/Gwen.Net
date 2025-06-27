using System;
using Gwen.Net.Control;

namespace Gwen.Net.Input
{
    /// <summary>
    ///     Keyboard state.
    /// </summary>
    public class KeyData
    {
        public Boolean[] KeyState { get; }
        public Single[] NextRepeat { get; }
        public Boolean LeftMouseDown { get; set; }
        public Boolean RightMouseDown { get; set; }
        public ControlBase Target { get; set; }

        public KeyData()
        {
            KeyState = new Boolean[(Int32)GwenMappedKey.Count];
            NextRepeat = new Single[(Int32)GwenMappedKey.Count];
            // everything is initialized to 0 by default
        }
    }
}
