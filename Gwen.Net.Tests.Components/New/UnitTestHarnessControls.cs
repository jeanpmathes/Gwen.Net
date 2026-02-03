using System;
using System.Drawing;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.Tests.Components.New
{
    public class UnitTestHarnessControls : Control<UnitTestHarnessControls>
    {
        // todo: split this class into view model and data template, it should not be a control itself
        
        public Double RenderFps { get; set; }
        public Double UpdateFps { get; set; }

        protected override ControlTemplate<UnitTestHarnessControls> CreateDefaultTemplate()
        {
            return ControlTemplate.Create<UnitTestHarnessControls>(static _ => new Border(
                new Border
                {
                    MinimumSize = new SizeF(width: 500f, height: 250f)
                })
            {
                MinimumSize = new SizeF(width: 500f, height: 500f)
            });
        }
    }
}
