using System;
using Gwen.Net.Legacy;
using Gwen.Net.Legacy.Control;

namespace Gwen.Net.Tests.Components.Legacy
{
    [UnitTest(Category = "Non-Interactive", Order = 105)]
    public class ImagePanelTest : GUnit
    {
        public ImagePanelTest(ControlBase parent)
            : base(parent)
        {
            /* Normal */
            {
                ImagePanel img = new(this)
                {
                    Margin = Margin.Five,
                    Dock = Dock.Top,
                    Size = new Size(width: 100, height: 100),
                    ImageName = "gwen.png"
                };
            }


            /* Missing */
            {
                ImagePanel img = new(this)
                {
                    Margin = Margin.Five,
                    Dock = Dock.Top,
                    Size = new Size(width: 100, height: 100),
                    ImageName = "missingimage.png"
                };
            }

            /* Clicked */
            {
                ImagePanel img = new(this)
                {
                    Margin = Margin.Five,
                    Dock = Dock.Top,
                    Size = new Size(width: 100, height: 100),
                    ImageName = "test16.png"
                };

                img.Clicked += Image_Clicked;
            }
        }

        private void Image_Clicked(ControlBase control, EventArgs args)
        {
            UnitPrint("Image: Clicked");
        }
    }
}
