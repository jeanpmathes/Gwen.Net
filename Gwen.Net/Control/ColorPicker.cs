using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Control.Layout;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     RGBA color picker.
    /// </summary>
    public class ColorPicker : ControlBase, IColorPicker
    {
        private Color m_Color;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorPicker" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ColorPicker(ControlBase parent)
            : base(parent)
        {
            MouseInputEnabled = true;

            CreateControls();
            SelectedColor = new Color(a: 255, r: 50, g: 60, b: 70);
        }

        /// <summary>
        ///     Red value of the selected color.
        /// </summary>
        public int R
        {
            get => m_Color.R;
            set => m_Color = new Color(m_Color.A, value, m_Color.G, m_Color.B);
        }

        /// <summary>
        ///     Green value of the selected color.
        /// </summary>
        public int G
        {
            get => m_Color.G;
            set => m_Color = new Color(m_Color.A, m_Color.R, value, m_Color.B);
        }

        /// <summary>
        ///     Blue value of the selected color.
        /// </summary>
        public int B
        {
            get => m_Color.B;
            set => m_Color = new Color(m_Color.A, m_Color.R, m_Color.G, value);
        }

        /// <summary>
        ///     Alpha value of the selected color.
        /// </summary>
        public int A
        {
            get => m_Color.A;
            set => m_Color = new Color(value, m_Color.R, m_Color.G, m_Color.B);
        }

        /// <summary>
        ///     Determines whether the Alpha control is visible.
        /// </summary>
        public bool AlphaVisible
        {
            get
            {
                GroupBox gb = FindChildByName("Alphagroupbox", recursive: true) as GroupBox;

                return !gb.IsHidden;
            }
            set
            {
                GroupBox gb = FindChildByName("Alphagroupbox", recursive: true) as GroupBox;
                gb.IsHidden = !value;
                //Invalidate();
            }
        }

        /// <summary>
        ///     Selected color.
        /// </summary>
        public Color SelectedColor
        {
            get => m_Color;
            set
            {
                m_Color = value;
                UpdateControls();
            }
        }

        /// <summary>
        ///     Invoked when the selected color has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ColorChanged;

        private void CreateControls()
        {
            VerticalLayout colorControlLayout = new(this);
            colorControlLayout.Dock = Dock.Fill;

            CreateColorControl(colorControlLayout, "Red");
            CreateColorControl(colorControlLayout, "Green");
            CreateColorControl(colorControlLayout, "Blue");
            CreateColorControl(colorControlLayout, "Alpha");

            GroupBox finalGroup = new(this);
            finalGroup.Dock = Dock.Right;
            finalGroup.Text = "Result";
            finalGroup.Name = "ResultGroupBox";

            DockLayout finalLayout = new(finalGroup);

            ColorDisplay disp = new(finalLayout);
            disp.Dock = Dock.Fill;
            disp.Name = "Result";
            disp.Width = Util.Ignore;
            disp.Height = Util.Ignore;
        }

        private void CreateColorControl(ControlBase parent, string name)
        {
            GroupBox colorGroup = new(parent);
            colorGroup.Text = name;
            colorGroup.Name = name + "groupbox";

            DockLayout layout = new(colorGroup);

            ColorDisplay disp = new(layout);
            disp.Height = Util.Ignore;
            disp.Dock = Dock.Left;
            disp.Name = name;

            TextBoxNumeric numeric = new(layout);
            numeric.Dock = Dock.Right;
            numeric.FitToText = "000";
            numeric.Name = name + "Box";
            numeric.SelectAllOnFocus = true;
            numeric.TextChanged += NumericTyped;

            HorizontalSlider slider = new(layout);
            slider.Dock = Dock.Fill;
            slider.VerticalAlignment = VerticalAlignment.Center;
            slider.SetRange(min: 0, max: 255);
            slider.Name = name + "Slider";
            slider.ValueChanged += SlidersMoved;
        }

        private void NumericTyped(ControlBase control, EventArgs args)
        {
            TextBoxNumeric box = control as TextBoxNumeric;

            if (null == box)
            {
                return;
            }

            if (box.Text == string.Empty)
            {
                return;
            }

            int textValue = (int)box.Value;

            if (textValue < 0)
            {
                textValue = 0;
            }

            if (textValue > 255)
            {
                textValue = 255;
            }

            if (box.Name.Contains("Red"))
            {
                R = textValue;
            }

            if (box.Name.Contains("Green"))
            {
                G = textValue;
            }

            if (box.Name.Contains("Blue"))
            {
                B = textValue;
            }

            if (box.Name.Contains("Alpha"))
            {
                A = textValue;
            }

            UpdateControls();
        }

        private void UpdateColorControls(string name, Color col, int sliderVal)
        {
            ColorDisplay disp = FindChildByName(name, recursive: true) as ColorDisplay;
            disp.Color = col;

            HorizontalSlider slider = FindChildByName(name + "Slider", recursive: true) as HorizontalSlider;
            slider.Value = sliderVal;

            TextBoxNumeric box = FindChildByName(name + "Box", recursive: true) as TextBoxNumeric;
            box.Value = sliderVal;
        }

        private void UpdateControls()
        {
            //This is a little weird, but whatever for now
            UpdateColorControls("Red", new Color(a: 255, SelectedColor.R, g: 0, b: 0), SelectedColor.R);
            UpdateColorControls("Green", new Color(a: 255, r: 0, SelectedColor.G, b: 0), SelectedColor.G);
            UpdateColorControls("Blue", new Color(a: 255, r: 0, g: 0, SelectedColor.B), SelectedColor.B);
            UpdateColorControls("Alpha", new Color(SelectedColor.A, r: 255, g: 255, b: 255), SelectedColor.A);

            ColorDisplay disp = FindChildByName("Result", recursive: true) as ColorDisplay;
            disp.Color = SelectedColor;

            if (ColorChanged != null)
            {
                ColorChanged.Invoke(this, EventArgs.Empty);
            }
        }

        private void SlidersMoved(ControlBase control, EventArgs args)
        {
            /*
            HorizontalSlider* redSlider		= gwen_cast<HorizontalSlider>(	FindChildByName( "RedSlider",   true ) );
            HorizontalSlider* greenSlider	= gwen_cast<HorizontalSlider>(	FindChildByName( "GreenSlider", true ) );
            HorizontalSlider* blueSlider	= gwen_cast<HorizontalSlider>(	FindChildByName( "BlueSlider",  true ) );
            HorizontalSlider* alphaSlider	= gwen_cast<HorizontalSlider>(	FindChildByName( "AlphaSlider", true ) );
            */

            HorizontalSlider slider = control as HorizontalSlider;

            if (slider != null)
            {
                SetColorByName(GetColorFromName(slider.Name), (int)slider.Value);
            }

            UpdateControls();
            //SetColor( Gwen::Color( redSlider->GetValue(), greenSlider->GetValue(), blueSlider->GetValue(), alphaSlider->GetValue() ) );
        }

        private int GetColorByName(string colorName)
        {
            if (colorName == "Red")
            {
                return SelectedColor.R;
            }

            if (colorName == "Green")
            {
                return SelectedColor.G;
            }

            if (colorName == "Blue")
            {
                return SelectedColor.B;
            }

            if (colorName == "Alpha")
            {
                return SelectedColor.A;
            }

            return 0;
        }

        private static string GetColorFromName(string name)
        {
            if (name.Contains("Red"))
            {
                return "Red";
            }

            if (name.Contains("Green"))
            {
                return "Green";
            }

            if (name.Contains("Blue"))
            {
                return "Blue";
            }

            if (name.Contains("Alpha"))
            {
                return "Alpha";
            }

            return String.Empty;
        }

        private void SetColorByName(string colorName, int colorValue)
        {
            if (colorName == "Red")
            {
                R = colorValue;
            }
            else if (colorName == "Green")
            {
                G = colorValue;
            }
            else if (colorName == "Blue")
            {
                B = colorValue;
            }
            else if (colorName == "Alpha")
            {
                A = colorValue;
            }
        }
    }
}