using Gwen.Net.Skin;

namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     Color square.
    /// </summary>
    public class ColorDisplay : ControlBase
    {
        private Color color;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorDisplay" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ColorDisplay(ControlBase parent) : base(parent)
        {
            Size = new Size(BaseUnit * 2);
            color = new Color(a: 255, r: 255, g: 0, b: 0);
        }

        /// <summary>
        ///     Current color.
        /// </summary>
        public Color Color
        {
            get => color;
            set => color = value;
        }
        
        public int R
        {
            get => color.R;
            set => color = new Color(color.A, value, color.G, color.B);
        }

        public int G
        {
            get => color.G;
            set => color = new Color(color.A, color.R, value, color.B);
        }

        public int B
        {
            get => color.B;
            set => color = new Color(color.A, color.R, color.G, value);
        }

        public int A
        {
            get => color.A;
            set => color = new Color(value, color.R, color.G, color.B);
        }

        protected override void AdaptToScaleChange()
        {
            int baseSize = BaseUnit * 2;

            int width = Util.IsIgnore(Size.Width) ? Util.Ignore : baseSize;
            int height = Util.IsIgnore(Size.Height) ? Util.Ignore : baseSize;

            Size = new Size(width, height);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawColorDisplay(this, color);
        }
    }
}
