using System.IO;
using Gwen.Net.Control;
using Gwen.Net.Control.Internal;
using Gwen.Net.Renderer;
using Gwen.Net.Skin.Texturing;

namespace Gwen.Net.Skin
{
    #region UI element textures

    public struct SkinTextures
    {
        public Bordered StatusBar;
        public Bordered Selection;
        public Bordered Shadow;
        public Bordered Tooltip;

        public struct _ToolWindow
        {
            public struct _H
            {
                public Bordered DragBar;
                public Bordered Client;
            }

            public struct _V
            {
                public Bordered DragBar;
                public Bordered Client;
            }

            public _H H;
            public _V V;
        }

        public _ToolWindow ToolWindow;

        public struct _Panel
        {
            public Bordered Normal;
            public Bordered Bright;
            public Bordered Dark;
            public Bordered Highlight;
        }

        public struct _Window
        {
            public struct _Normal
            {
                public Bordered TitleBar;
                public Bordered Client;
            }

            public struct _Inactive
            {
                public Bordered TitleBar;
                public Bordered Client;
            }

            public _Normal Normal;
            public _Inactive Inactive;

            public Single Close;
            public Single Close_Hover;
            public Single Close_Down;
            public Single Close_Disabled;
        }

        public struct _CheckBox
        {
            public struct _Active
            {
                public Single Normal;
                public Single Checked;
            }

            public struct _Disabled
            {
                public Single Normal;
                public Single Checked;
            }

            public _Active Active;
            public _Disabled Disabled;
        }

        public struct _RadioButton
        {
            public struct _Active
            {
                public Single Normal;
                public Single Checked;
            }

            public struct _Disabled
            {
                public Single Normal;
                public Single Checked;
            }

            public _Active Active;
            public _Disabled Disabled;
        }

        public struct _TextBox
        {
            public Bordered Normal;
            public Bordered Focus;
            public Bordered Disabled;
        }

        public struct _Tree
        {
            public Bordered Background;
            public Single Minus;
            public Single Plus;
        }

        public struct _ProgressBar
        {
            public Bordered Back;
            public Bordered Front;
        }

        public struct _Scroller
        {
            public Bordered TrackV;
            public Bordered TrackH;
            public Bordered ButtonV_Normal;
            public Bordered ButtonV_Hover;
            public Bordered ButtonV_Down;
            public Bordered ButtonV_Disabled;
            public Bordered ButtonH_Normal;
            public Bordered ButtonH_Hover;
            public Bordered ButtonH_Down;
            public Bordered ButtonH_Disabled;

            public struct _Button
            {
                public Bordered[] Normal;
                public Bordered[] Hover;
                public Bordered[] Down;
                public Bordered[] Disabled;
            }

            public _Button Button;
        }

        public struct _Menu
        {
            public Single RightArrow;
            public Single Check;

            public Bordered Strip;
            public Bordered Background;
            public Bordered BackgroundWithMargin;
            public Bordered Hover;
        }

        public struct _Input
        {
            public struct _Button
            {
                public Bordered Normal;
                public Bordered Hovered;
                public Bordered Disabled;
                public Bordered Pressed;
            }

            public struct _ComboBox
            {
                public Bordered Normal;
                public Bordered Hover;
                public Bordered Down;
                public Bordered Disabled;

                public struct _Button
                {
                    public Single Normal;
                    public Single Hover;
                    public Single Down;
                    public Single Disabled;
                }

                public _Button Button;
            }

            public struct _Slider
            {
                public struct _H
                {
                    public Single Normal;
                    public Single Hover;
                    public Single Down;
                    public Single Disabled;
                }

                public struct _V
                {
                    public Single Normal;
                    public Single Hover;
                    public Single Down;
                    public Single Disabled;
                }

                public _H H;
                public _V V;
            }

            public struct _ListBox
            {
                public Bordered Background;
                public Bordered Hovered;
                public Bordered EvenLine;
                public Bordered OddLine;
                public Bordered EvenLineSelected;
                public Bordered OddLineSelected;
            }

            public struct _UpDown
            {
                public struct _Up
                {
                    public Single Normal;
                    public Single Hover;
                    public Single Down;
                    public Single Disabled;
                }

                public struct _Down
                {
                    public Single Normal;
                    public Single Hover;
                    public Single Down;
                    public Single Disabled;
                }

                public _Up Up;
                public _Down Down;
            }

            public _Button Button;
            public _ComboBox ComboBox;
            public _Slider Slider;
            public _ListBox ListBox;
            public _UpDown UpDown;
        }

        public struct _Tab
        {
            public struct _Bottom
            {
                public Bordered Inactive;
                public Bordered Active;
            }

            public struct _Top
            {
                public Bordered Inactive;
                public Bordered Active;
            }

            public struct _Left
            {
                public Bordered Inactive;
                public Bordered Active;
            }

            public struct _Right
            {
                public Bordered Inactive;
                public Bordered Active;
            }

            public _Bottom Bottom;
            public _Top Top;
            public _Left Left;
            public _Right Right;

            public Bordered Control;
            public Bordered HeaderBar;
        }

        public struct _CategoryList
        {
            public struct _Inner
            {
                public Bordered Header;
                public Bordered Client;
            }

            public Bordered Outer;
            public _Inner Inner;
            public Bordered Header;
        }

        public _Panel Panel;
        public _Window Window;
        public _CheckBox CheckBox;
        public _RadioButton RadioButton;
        public _TextBox TextBox;
        public _Tree Tree;
        public _ProgressBar ProgressBar;
        public _Scroller Scroller;
        public _Menu Menu;
        public _Input Input;
        public _Tab Tab;
        public _CategoryList CategoryList;
    }

    #endregion

    /// <summary>
    ///     Base textured skin.
    /// </summary>
    public class TexturedBase : SkinBase
    {
        private readonly Texture m_Texture;
        protected SkinTextures Textures;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TexturedBase" /> class.
        /// </summary>
        /// <param name="renderer">Renderer to use.</param>
        /// <param name="textureName">Name of the skin texture map.</param>
        public TexturedBase(RendererBase renderer, string textureName)
            : base(renderer)
        {
            m_Texture = new Texture(Renderer);
            m_Texture.Load(textureName);

            InitializeColors();
            InitializeTextures();
        }

        public TexturedBase(RendererBase renderer, Stream textureData)
            : base(renderer)
        {
            m_Texture = new Texture(Renderer);
            m_Texture.LoadStream(textureData);

            InitializeColors();
            InitializeTextures();
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            m_Texture.Dispose();
            base.Dispose();
        }

        #region Initialization

        private void InitializeColors()
        {
            Colors.Window.TitleActive = Renderer.PixelColor(m_Texture, 4 + (8 * 0), y: 508, Color.Red);
            Colors.Window.TitleInactive = Renderer.PixelColor(m_Texture, 4 + (8 * 1), y: 508, Color.Yellow);

            Colors.Button.Normal = Renderer.PixelColor(m_Texture, 4 + (8 * 2), y: 508, Color.Yellow);
            Colors.Button.Hover = Renderer.PixelColor(m_Texture, 4 + (8 * 3), y: 508, Color.Yellow);
            Colors.Button.Down = Renderer.PixelColor(m_Texture, 4 + (8 * 2), y: 500, Color.Yellow);
            Colors.Button.Disabled = Renderer.PixelColor(m_Texture, 4 + (8 * 3), y: 500, Color.Yellow);

            Colors.Tab.Active.Normal = Renderer.PixelColor(m_Texture, 4 + (8 * 4), y: 508, Color.Yellow);
            Colors.Tab.Active.Hover = Renderer.PixelColor(m_Texture, 4 + (8 * 5), y: 508, Color.Yellow);
            Colors.Tab.Active.Down = Renderer.PixelColor(m_Texture, 4 + (8 * 4), y: 500, Color.Yellow);
            Colors.Tab.Active.Disabled = Renderer.PixelColor(m_Texture, 4 + (8 * 5), y: 500, Color.Yellow);
            Colors.Tab.Inactive.Normal = Renderer.PixelColor(m_Texture, 4 + (8 * 6), y: 508, Color.Yellow);
            Colors.Tab.Inactive.Hover = Renderer.PixelColor(m_Texture, 4 + (8 * 7), y: 508, Color.Yellow);
            Colors.Tab.Inactive.Down = Renderer.PixelColor(m_Texture, 4 + (8 * 6), y: 500, Color.Yellow);
            Colors.Tab.Inactive.Disabled = Renderer.PixelColor(m_Texture, 4 + (8 * 7), y: 500, Color.Yellow);

            Colors.Label.Default = Renderer.PixelColor(m_Texture, 4 + (8 * 8), y: 508, Color.Yellow);
            Colors.Label.Bright = Renderer.PixelColor(m_Texture, 4 + (8 * 9), y: 508, Color.Yellow);
            Colors.Label.Dark = Renderer.PixelColor(m_Texture, 4 + (8 * 8), y: 500, Color.Yellow);
            Colors.Label.Highlight = Renderer.PixelColor(m_Texture, 4 + (8 * 9), y: 500, Color.Yellow);

            Colors.TextBox.Text = Renderer.PixelColor(m_Texture, 4 + (8 * 8), y: 508, Color.Yellow);
            Colors.TextBox.Background_Selected = Renderer.PixelColor(m_Texture, 4 + (8 * 10), y: 500, Color.Yellow);
            Colors.TextBox.Caret = Renderer.PixelColor(m_Texture, 4 + (8 * 8), y: 508, Color.Yellow);

            Colors.ListBox.Text_Normal = Renderer.PixelColor(m_Texture, 4 + (8 * 11), y: 508, Color.Yellow);
            Colors.ListBox.Text_Selected = Renderer.PixelColor(m_Texture, 4 + (8 * 11), y: 500, Color.Yellow);

            Colors.Tree.Lines = Renderer.PixelColor(m_Texture, 4 + (8 * 10), y: 508, Color.Yellow);
            Colors.Tree.Normal = Renderer.PixelColor(m_Texture, 4 + (8 * 11), y: 508, Color.Yellow);
            Colors.Tree.Hover = Renderer.PixelColor(m_Texture, 4 + (8 * 10), y: 500, Color.Yellow);
            Colors.Tree.Selected = Renderer.PixelColor(m_Texture, 4 + (8 * 11), y: 500, Color.Yellow);

            Colors.Properties.Line_Normal = Renderer.PixelColor(m_Texture, 4 + (8 * 12), y: 508, Color.Yellow);
            Colors.Properties.Line_Selected = Renderer.PixelColor(m_Texture, 4 + (8 * 13), y: 508, Color.Yellow);
            Colors.Properties.Line_Hover = Renderer.PixelColor(m_Texture, 4 + (8 * 12), y: 500, Color.Yellow);
            Colors.Properties.Title = Renderer.PixelColor(m_Texture, 4 + (8 * 13), y: 500, Color.Yellow);
            Colors.Properties.Column_Normal = Renderer.PixelColor(m_Texture, 4 + (8 * 14), y: 508, Color.Yellow);
            Colors.Properties.Column_Selected = Renderer.PixelColor(m_Texture, 4 + (8 * 15), y: 508, Color.Yellow);
            Colors.Properties.Column_Hover = Renderer.PixelColor(m_Texture, 4 + (8 * 14), y: 500, Color.Yellow);
            Colors.Properties.Border = Renderer.PixelColor(m_Texture, 4 + (8 * 15), y: 500, Color.Yellow);
            Colors.Properties.Label_Normal = Renderer.PixelColor(m_Texture, 4 + (8 * 16), y: 508, Color.Yellow);
            Colors.Properties.Label_Selected = Renderer.PixelColor(m_Texture, 4 + (8 * 17), y: 508, Color.Yellow);
            Colors.Properties.Label_Hover = Renderer.PixelColor(m_Texture, 4 + (8 * 16), y: 500, Color.Yellow);

            Colors.ModalBackground = Renderer.PixelColor(m_Texture, 4 + (8 * 18), y: 508, Color.Yellow);

            Colors.TooltipText = Renderer.PixelColor(m_Texture, 4 + (8 * 19), y: 508, Color.Yellow);

            Colors.Category.Header = Renderer.PixelColor(m_Texture, 4 + (8 * 18), y: 500, Color.Yellow);
            Colors.Category.Header_Closed = Renderer.PixelColor(m_Texture, 4 + (8 * 19), y: 500, Color.Yellow);
            Colors.Category.Line.Text = Renderer.PixelColor(m_Texture, 4 + (8 * 20), y: 508, Color.Yellow);
            Colors.Category.Line.Text_Hover = Renderer.PixelColor(m_Texture, 4 + (8 * 21), y: 508, Color.Yellow);
            Colors.Category.Line.Text_Selected = Renderer.PixelColor(m_Texture, 4 + (8 * 20), y: 500, Color.Yellow);
            Colors.Category.Line.Button = Renderer.PixelColor(m_Texture, 4 + (8 * 21), y: 500, Color.Yellow);
            Colors.Category.Line.Button_Hover = Renderer.PixelColor(m_Texture, 4 + (8 * 22), y: 508, Color.Yellow);
            Colors.Category.Line.Button_Selected = Renderer.PixelColor(m_Texture, 4 + (8 * 23), y: 508, Color.Yellow);
            Colors.Category.LineAlt.Text = Renderer.PixelColor(m_Texture, 4 + (8 * 22), y: 500, Color.Yellow);
            Colors.Category.LineAlt.Text_Hover = Renderer.PixelColor(m_Texture, 4 + (8 * 23), y: 500, Color.Yellow);
            Colors.Category.LineAlt.Text_Selected = Renderer.PixelColor(m_Texture, 4 + (8 * 24), y: 508, Color.Yellow);
            Colors.Category.LineAlt.Button = Renderer.PixelColor(m_Texture, 4 + (8 * 25), y: 508, Color.Yellow);
            Colors.Category.LineAlt.Button_Hover = Renderer.PixelColor(m_Texture, 4 + (8 * 24), y: 500, Color.Yellow);

            Colors.Category.LineAlt.Button_Selected =
                Renderer.PixelColor(m_Texture, 4 + (8 * 25), y: 500, Color.Yellow);
        }

        private void InitializeTextures()
        {
            Textures.Shadow = new Bordered(m_Texture, x: 448, y: 0, w: 31, h: 31, Margin.Eight);
            Textures.Tooltip = new Bordered(m_Texture, x: 128, y: 320, w: 127, h: 31, Margin.Eight);
            Textures.StatusBar = new Bordered(m_Texture, x: 128, y: 288, w: 127, h: 31, Margin.Eight);
            Textures.Selection = new Bordered(m_Texture, x: 384, y: 32, w: 31, h: 31, Margin.Four);

            Textures.Panel.Normal = new Bordered(
                m_Texture,
                x: 256,
                y: 0,
                w: 63,
                h: 63,
                new Margin(left: 16, top: 16, right: 16, bottom: 16));

            Textures.Panel.Bright = new Bordered(
                m_Texture,
                256 + 64,
                y: 0,
                w: 63,
                h: 63,
                new Margin(left: 16, top: 16, right: 16, bottom: 16));

            Textures.Panel.Dark = new Bordered(
                m_Texture,
                x: 256,
                y: 64,
                w: 63,
                h: 63,
                new Margin(left: 16, top: 16, right: 16, bottom: 16));

            Textures.Panel.Highlight = new Bordered(
                m_Texture,
                256 + 64,
                y: 64,
                w: 63,
                h: 63,
                new Margin(left: 16, top: 16, right: 16, bottom: 16));

            Textures.Window.Normal.TitleBar = new Bordered(
                m_Texture,
                x: 0,
                y: 0,
                w: 127,
                h: 24,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            Textures.Window.Normal.Client = new Bordered(
                m_Texture,
                x: 0,
                y: 24,
                w: 127,
                127 - 24,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            Textures.Window.Inactive.TitleBar = new Bordered(
                m_Texture,
                x: 128,
                y: 0,
                w: 127,
                h: 24,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            Textures.Window.Inactive.Client = new Bordered(
                m_Texture,
                x: 128,
                y: 24,
                w: 127,
                127 - 24,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            Textures.ToolWindow.H.DragBar = new Bordered(
                m_Texture,
                x: 384,
                y: 464,
                w: 17,
                h: 31,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            Textures.ToolWindow.H.Client = new Bordered(
                m_Texture,
                384 + 17,
                y: 464,
                127 - 17,
                h: 31,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            Textures.ToolWindow.V.DragBar = new Bordered(
                m_Texture,
                x: 256,
                y: 384,
                w: 31,
                h: 17,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            Textures.ToolWindow.V.Client = new Bordered(
                m_Texture,
                x: 256,
                384 + 17,
                w: 31,
                127 - 17,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            Textures.CheckBox.Active.Checked = new Single(m_Texture, x: 448, y: 32, w: 15, h: 15);
            Textures.CheckBox.Active.Normal = new Single(m_Texture, x: 464, y: 32, w: 15, h: 15);
            Textures.CheckBox.Disabled.Normal = new Single(m_Texture, x: 448, y: 48, w: 15, h: 15);
            Textures.CheckBox.Disabled.Normal = new Single(m_Texture, x: 464, y: 48, w: 15, h: 15);

            Textures.RadioButton.Active.Checked = new Single(m_Texture, x: 448, y: 64, w: 15, h: 15);
            Textures.RadioButton.Active.Normal = new Single(m_Texture, x: 464, y: 64, w: 15, h: 15);
            Textures.RadioButton.Disabled.Normal = new Single(m_Texture, x: 448, y: 80, w: 15, h: 15);
            Textures.RadioButton.Disabled.Normal = new Single(m_Texture, x: 464, y: 80, w: 15, h: 15);

            Textures.TextBox.Normal = new Bordered(m_Texture, x: 0, y: 150, w: 127, h: 21, Margin.Four);
            Textures.TextBox.Focus = new Bordered(m_Texture, x: 0, y: 172, w: 127, h: 21, Margin.Four);
            Textures.TextBox.Disabled = new Bordered(m_Texture, x: 0, y: 193, w: 127, h: 21, Margin.Four);

            Textures.Menu.Strip = new Bordered(m_Texture, x: 0, y: 128, w: 127, h: 21, Margin.One);

            Textures.Menu.BackgroundWithMargin = new Bordered(
                m_Texture,
                x: 128,
                y: 128,
                w: 127,
                h: 63,
                new Margin(left: 24, top: 8, right: 8, bottom: 8));

            Textures.Menu.Background = new Bordered(m_Texture, x: 128, y: 192, w: 127, h: 63, Margin.Eight);
            Textures.Menu.Hover = new Bordered(m_Texture, x: 128, y: 256, w: 127, h: 31, Margin.Eight);
            Textures.Menu.RightArrow = new Single(m_Texture, x: 464, y: 112, w: 15, h: 15);
            Textures.Menu.Check = new Single(m_Texture, x: 448, y: 112, w: 15, h: 15);

            Textures.Tab.Control = new Bordered(m_Texture, x: 0, y: 256, w: 127, h: 127, Margin.Eight);
            Textures.Tab.Bottom.Active = new Bordered(m_Texture, x: 0, y: 416, w: 63, h: 31, Margin.Eight);
            Textures.Tab.Bottom.Inactive = new Bordered(m_Texture, 0 + 128, y: 416, w: 63, h: 31, Margin.Eight);
            Textures.Tab.Top.Active = new Bordered(m_Texture, x: 0, y: 384, w: 63, h: 31, Margin.Eight);
            Textures.Tab.Top.Inactive = new Bordered(m_Texture, 0 + 128, y: 384, w: 63, h: 31, Margin.Eight);
            Textures.Tab.Left.Active = new Bordered(m_Texture, x: 64, y: 384, w: 31, h: 63, Margin.Eight);
            Textures.Tab.Left.Inactive = new Bordered(m_Texture, 64 + 128, y: 384, w: 31, h: 63, Margin.Eight);
            Textures.Tab.Right.Active = new Bordered(m_Texture, x: 96, y: 384, w: 31, h: 63, Margin.Eight);
            Textures.Tab.Right.Inactive = new Bordered(m_Texture, 96 + 128, y: 384, w: 31, h: 63, Margin.Eight);
            Textures.Tab.HeaderBar = new Bordered(m_Texture, x: 128, y: 352, w: 127, h: 31, Margin.Four);

            Textures.Window.Close = new Single(m_Texture, x: 0, y: 224, w: 24, h: 24);
            Textures.Window.Close_Hover = new Single(m_Texture, x: 32, y: 224, w: 24, h: 24);
            Textures.Window.Close_Down = new Single(m_Texture, x: 64, y: 224, w: 24, h: 24);
            Textures.Window.Close_Disabled = new Single(m_Texture, x: 96, y: 224, w: 24, h: 24);

            Textures.Scroller.TrackV = new Bordered(m_Texture, x: 384, y: 208, w: 15, h: 127, Margin.Four);
            Textures.Scroller.ButtonV_Normal = new Bordered(m_Texture, 384 + 16, y: 208, w: 15, h: 127, Margin.Four);
            Textures.Scroller.ButtonV_Hover = new Bordered(m_Texture, 384 + 32, y: 208, w: 15, h: 127, Margin.Four);
            Textures.Scroller.ButtonV_Down = new Bordered(m_Texture, 384 + 48, y: 208, w: 15, h: 127, Margin.Four);
            Textures.Scroller.ButtonV_Disabled = new Bordered(m_Texture, 384 + 64, y: 208, w: 15, h: 127, Margin.Four);
            Textures.Scroller.TrackH = new Bordered(m_Texture, x: 384, y: 128, w: 127, h: 15, Margin.Four);
            Textures.Scroller.ButtonH_Normal = new Bordered(m_Texture, x: 384, 128 + 16, w: 127, h: 15, Margin.Four);
            Textures.Scroller.ButtonH_Hover = new Bordered(m_Texture, x: 384, 128 + 32, w: 127, h: 15, Margin.Four);
            Textures.Scroller.ButtonH_Down = new Bordered(m_Texture, x: 384, 128 + 48, w: 127, h: 15, Margin.Four);
            Textures.Scroller.ButtonH_Disabled = new Bordered(m_Texture, x: 384, 128 + 64, w: 127, h: 15, Margin.Four);

            Textures.Scroller.Button.Normal = new Bordered[4];
            Textures.Scroller.Button.Disabled = new Bordered[4];
            Textures.Scroller.Button.Hover = new Bordered[4];
            Textures.Scroller.Button.Down = new Bordered[4];

            Textures.Tree.Background = new Bordered(
                m_Texture,
                x: 256,
                y: 128,
                w: 127,
                h: 127,
                new Margin(left: 16, top: 16, right: 16, bottom: 16));

            Textures.Tree.Plus = new Single(m_Texture, x: 448, y: 96, w: 15, h: 15);
            Textures.Tree.Minus = new Single(m_Texture, x: 464, y: 96, w: 15, h: 15);

            Textures.Input.Button.Normal = new Bordered(m_Texture, x: 480, y: 0, w: 31, h: 31, Margin.Eight);
            Textures.Input.Button.Hovered = new Bordered(m_Texture, x: 480, y: 32, w: 31, h: 31, Margin.Eight);
            Textures.Input.Button.Disabled = new Bordered(m_Texture, x: 480, y: 64, w: 31, h: 31, Margin.Eight);
            Textures.Input.Button.Pressed = new Bordered(m_Texture, x: 480, y: 96, w: 31, h: 31, Margin.Eight);

            for (int i = 0; i < 4; i++)
            {
                Textures.Scroller.Button.Normal[i] = new Bordered(
                    m_Texture,
                    464 + 0,
                    208 + (i * 16),
                    w: 15,
                    h: 15,
                    Margin.Two);

                Textures.Scroller.Button.Hover[i] = new Bordered(
                    m_Texture,
                    x: 480,
                    208 + (i * 16),
                    w: 15,
                    h: 15,
                    Margin.Two);

                Textures.Scroller.Button.Down[i] = new Bordered(
                    m_Texture,
                    x: 464,
                    272 + (i * 16),
                    w: 15,
                    h: 15,
                    Margin.Two);

                Textures.Scroller.Button.Disabled[i] = new Bordered(
                    m_Texture,
                    480 + 48,
                    272 + (i * 16),
                    w: 15,
                    h: 15,
                    Margin.Two);
            }

            Textures.Input.ListBox.Background = new Bordered(m_Texture, x: 256, y: 256, w: 63, h: 127, Margin.Eight);
            Textures.Input.ListBox.Hovered = new Bordered(m_Texture, x: 320, y: 320, w: 31, h: 31, Margin.Eight);
            Textures.Input.ListBox.EvenLine = new Bordered(m_Texture, x: 352, y: 256, w: 31, h: 31, Margin.Eight);
            Textures.Input.ListBox.OddLine = new Bordered(m_Texture, x: 352, y: 288, w: 31, h: 31, Margin.Eight);

            Textures.Input.ListBox.EvenLineSelected = new Bordered(
                m_Texture,
                x: 320,
                y: 270,
                w: 31,
                h: 31,
                Margin.Eight);

            Textures.Input.ListBox.OddLineSelected = new Bordered(
                m_Texture,
                x: 320,
                y: 288,
                w: 31,
                h: 31,
                Margin.Eight);

            Textures.Input.ComboBox.Normal = new Bordered(
                m_Texture,
                x: 384,
                y: 336,
                w: 127,
                h: 31,
                new Margin(left: 8, top: 8, right: 32, bottom: 8));

            Textures.Input.ComboBox.Hover = new Bordered(
                m_Texture,
                x: 384,
                336 + 32,
                w: 127,
                h: 31,
                new Margin(left: 8, top: 8, right: 32, bottom: 8));

            Textures.Input.ComboBox.Down = new Bordered(
                m_Texture,
                x: 384,
                336 + 64,
                w: 127,
                h: 31,
                new Margin(left: 8, top: 8, right: 32, bottom: 8));

            Textures.Input.ComboBox.Disabled = new Bordered(
                m_Texture,
                x: 384,
                336 + 96,
                w: 127,
                h: 31,
                new Margin(left: 8, top: 8, right: 32, bottom: 8));

            Textures.Input.ComboBox.Button.Normal = new Single(m_Texture, x: 496, y: 272, w: 15, h: 15);
            Textures.Input.ComboBox.Button.Hover = new Single(m_Texture, x: 496, 272 + 16, w: 15, h: 15);
            Textures.Input.ComboBox.Button.Down = new Single(m_Texture, x: 496, 272 + 32, w: 15, h: 15);
            Textures.Input.ComboBox.Button.Disabled = new Single(m_Texture, x: 496, 272 + 48, w: 15, h: 15);

            Textures.Input.UpDown.Up.Normal = new Single(m_Texture, x: 384, y: 112, w: 7, h: 7);
            Textures.Input.UpDown.Up.Hover = new Single(m_Texture, 384 + 8, y: 112, w: 7, h: 7);
            Textures.Input.UpDown.Up.Down = new Single(m_Texture, 384 + 16, y: 112, w: 7, h: 7);
            Textures.Input.UpDown.Up.Disabled = new Single(m_Texture, 384 + 24, y: 112, w: 7, h: 7);
            Textures.Input.UpDown.Down.Normal = new Single(m_Texture, x: 384, y: 120, w: 7, h: 7);
            Textures.Input.UpDown.Down.Hover = new Single(m_Texture, 384 + 8, y: 120, w: 7, h: 7);
            Textures.Input.UpDown.Down.Down = new Single(m_Texture, 384 + 16, y: 120, w: 7, h: 7);
            Textures.Input.UpDown.Down.Disabled = new Single(m_Texture, 384 + 24, y: 120, w: 7, h: 7);

            Textures.ProgressBar.Back = new Bordered(m_Texture, x: 384, y: 0, w: 31, h: 31, Margin.Two);
            Textures.ProgressBar.Front = new Bordered(m_Texture, 384 + 32, y: 0, w: 31, h: 31, Margin.Two);

            Textures.Input.Slider.H.Normal = new Single(m_Texture, x: 416, y: 32, w: 15, h: 15);
            Textures.Input.Slider.H.Hover = new Single(m_Texture, x: 416, 32 + 16, w: 15, h: 15);
            Textures.Input.Slider.H.Down = new Single(m_Texture, x: 416, 32 + 32, w: 15, h: 15);
            Textures.Input.Slider.H.Disabled = new Single(m_Texture, x: 416, 32 + 48, w: 15, h: 15);

            Textures.Input.Slider.V.Normal = new Single(m_Texture, 416 + 16, y: 32, w: 15, h: 15);
            Textures.Input.Slider.V.Hover = new Single(m_Texture, 416 + 16, 32 + 16, w: 15, h: 15);
            Textures.Input.Slider.V.Down = new Single(m_Texture, 416 + 16, 32 + 32, w: 15, h: 15);
            Textures.Input.Slider.V.Disabled = new Single(m_Texture, 416 + 16, 32 + 48, w: 15, h: 15);

            Textures.CategoryList.Outer = new Bordered(m_Texture, x: 256, y: 256, w: 63, h: 127, Margin.Eight);

            Textures.CategoryList.Inner.Header = new Bordered(
                m_Texture,
                256 + 64,
                y: 384,
                w: 63,
                h: 20,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            Textures.CategoryList.Inner.Client = new Bordered(
                m_Texture,
                256 + 64,
                384 + 20,
                w: 63,
                63 - 20,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            Textures.CategoryList.Header = new Bordered(m_Texture, x: 320, y: 352, w: 63, h: 31, Margin.Eight);
        }

        #endregion

        #region UI elements

        public override void DrawButton(ControlBase control, bool depressed, bool hovered, bool disabled)
        {
            if (disabled)
            {
                Textures.Input.Button.Disabled.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (depressed)
            {
                Textures.Input.Button.Pressed.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (hovered)
            {
                Textures.Input.Button.Hovered.Draw(Renderer, control.RenderBounds);

                return;
            }

            Textures.Input.Button.Normal.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawMenuRightArrow(ControlBase control)
        {
            Textures.Menu.RightArrow.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawMenuItem(ControlBase control, bool submenuOpen, bool isChecked)
        {
            if (submenuOpen || control.IsHovered)
            {
                Textures.Menu.Hover.Draw(Renderer, control.RenderBounds);
            }

            if (isChecked)
            {
                Textures.Menu.Check.Draw(
                    Renderer,
                    new Rectangle(control.RenderBounds.X + 4, control.RenderBounds.Y + 3, width: 15, height: 15));
            }
        }

        public override void DrawMenuStrip(ControlBase control)
        {
            Textures.Menu.Strip.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawMenu(ControlBase control, bool paddingDisabled)
        {
            if (!paddingDisabled)
            {
                Textures.Menu.BackgroundWithMargin.Draw(Renderer, control.RenderBounds);

                return;
            }

            Textures.Menu.Background.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawShadow(ControlBase control)
        {
            Rectangle r = control.RenderBounds;
            r.X -= 4;
            r.Y -= 4;
            r.Width += 10;
            r.Height += 10;
            Textures.Shadow.Draw(Renderer, r);
        }

        public override void DrawRadioButton(ControlBase control, bool selected, bool depressed)
        {
            if (selected)
            {
                if (control.IsDisabled)
                {
                    Textures.RadioButton.Disabled.Checked.Draw(Renderer, control.RenderBounds);
                }
                else
                {
                    Textures.RadioButton.Active.Checked.Draw(Renderer, control.RenderBounds);
                }
            }
            else
            {
                if (control.IsDisabled)
                {
                    Textures.RadioButton.Disabled.Normal.Draw(Renderer, control.RenderBounds);
                }
                else
                {
                    Textures.RadioButton.Active.Normal.Draw(Renderer, control.RenderBounds);
                }
            }
        }

        public override void DrawCheckBox(ControlBase control, bool selected, bool depressed)
        {
            if (selected)
            {
                if (control.IsDisabled)
                {
                    Textures.CheckBox.Disabled.Checked.Draw(Renderer, control.RenderBounds);
                }
                else
                {
                    Textures.CheckBox.Active.Checked.Draw(Renderer, control.RenderBounds);
                }
            }
            else
            {
                if (control.IsDisabled)
                {
                    Textures.CheckBox.Disabled.Normal.Draw(Renderer, control.RenderBounds);
                }
                else
                {
                    Textures.CheckBox.Active.Normal.Draw(Renderer, control.RenderBounds);
                }
            }
        }

        public override void DrawGroupBox(ControlBase control, int textStart, int textHeight, int textWidth)
        {
            Rectangle rect = control.RenderBounds;

            rect.Y += (int)(textHeight * 0.5f);
            rect.Height -= (int)(textHeight * 0.5f);

            Color m_colDarker = new(a: 50, r: 0, g: 50, b: 60);
            Color m_colLighter = new(a: 150, r: 255, g: 255, b: 255);

            Renderer.DrawColor = m_colLighter;

            Renderer.DrawFilledRect(new Rectangle(rect.X + 1, rect.Y + 1, textStart - 3, height: 1));

            Renderer.DrawFilledRect(
                new Rectangle(
                    rect.X + 1 + textStart + textWidth,
                    rect.Y + 1,
                    rect.Width - textStart + textWidth - 2,
                    height: 1));

            Renderer.DrawFilledRect(
                new Rectangle(rect.X + 1, rect.Y + rect.Height - 1, rect.X + rect.Width - 2, height: 1));

            Renderer.DrawFilledRect(new Rectangle(rect.X + 1, rect.Y + 1, width: 1, rect.Height));
            Renderer.DrawFilledRect(new Rectangle(rect.X + rect.Width - 2, rect.Y + 1, width: 1, rect.Height - 1));

            Renderer.DrawColor = m_colDarker;

            Renderer.DrawFilledRect(new Rectangle(rect.X + 1, rect.Y, textStart - 3, height: 1));

            Renderer.DrawFilledRect(
                new Rectangle(
                    rect.X + 1 + textStart + textWidth,
                    rect.Y,
                    rect.Width - textStart - textWidth - 2,
                    height: 1));

            Renderer.DrawFilledRect(
                new Rectangle(rect.X + 1, rect.Y + rect.Height - 1, rect.X + rect.Width - 2, height: 1));

            Renderer.DrawFilledRect(new Rectangle(rect.X, rect.Y + 1, width: 1, rect.Height - 1));
            Renderer.DrawFilledRect(new Rectangle(rect.X + rect.Width - 1, rect.Y + 1, width: 1, rect.Height - 1));
        }

        public override void DrawTextBox(ControlBase control)
        {
            if (control.IsDisabled)
            {
                Textures.TextBox.Disabled.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (control.HasFocus)
            {
                Textures.TextBox.Focus.Draw(Renderer, control.RenderBounds);
            }
            else
            {
                Textures.TextBox.Normal.Draw(Renderer, control.RenderBounds);
            }
        }

        public override void DrawTabButton(ControlBase control, bool active, Dock dir)
        {
            if (active)
            {
                DrawActiveTabButton(control, dir);

                return;
            }

            if (dir == Dock.Top)
            {
                Textures.Tab.Top.Inactive.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (dir == Dock.Left)
            {
                Textures.Tab.Left.Inactive.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (dir == Dock.Bottom)
            {
                Textures.Tab.Bottom.Inactive.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (dir == Dock.Right)
            {
                Textures.Tab.Right.Inactive.Draw(Renderer, control.RenderBounds);
            }
        }

        private void DrawActiveTabButton(ControlBase control, Dock dir)
        {
            if (dir == Dock.Top)
            {
                Textures.Tab.Top.Active.Draw(
                    Renderer,
                    control.RenderBounds.Add(new Rectangle(x: 0, y: 0, width: 0, height: 8)));

                return;
            }

            if (dir == Dock.Left)
            {
                Textures.Tab.Left.Active.Draw(
                    Renderer,
                    control.RenderBounds.Add(new Rectangle(x: 0, y: 0, width: 8, height: 0)));

                return;
            }

            if (dir == Dock.Bottom)
            {
                Textures.Tab.Bottom.Active.Draw(
                    Renderer,
                    control.RenderBounds.Add(new Rectangle(x: 0, y: -8, width: 0, height: 8)));

                return;
            }

            if (dir == Dock.Right)
            {
                Textures.Tab.Right.Active.Draw(
                    Renderer,
                    control.RenderBounds.Add(new Rectangle(x: -8, y: 0, width: 8, height: 0)));
            }
        }

        public override void DrawTabControl(ControlBase control)
        {
            Textures.Tab.Control.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawTabTitleBar(ControlBase control)
        {
            Textures.Tab.HeaderBar.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawWindow(ControlBase control, int topHeight, bool inFocus)
        {
            Rectangle rect = control.RenderBounds;
            Rectangle titleRect = rect;
            titleRect.Height = topHeight;
            Rectangle clientRect = rect;
            clientRect.Y += topHeight;
            clientRect.Height -= topHeight;

            if (inFocus)
            {
                Textures.Window.Normal.TitleBar.Draw(Renderer, titleRect);
                Textures.Window.Normal.Client.Draw(Renderer, clientRect);
            }
            else
            {
                Textures.Window.Inactive.TitleBar.Draw(Renderer, titleRect);
                Textures.Window.Inactive.Client.Draw(Renderer, clientRect);
            }
        }

        public override void DrawToolWindow(ControlBase control, bool vertical, int dragSize)
        {
            if (vertical)
            {
                Rectangle rect = control.RenderBounds;
                Rectangle dragRect = rect;
                dragRect.Height = dragSize;
                Rectangle clientRect = rect;
                clientRect.Y += dragSize;
                clientRect.Height -= dragSize;

                Textures.ToolWindow.V.DragBar.Draw(Renderer, dragRect);
                Textures.ToolWindow.V.Client.Draw(Renderer, clientRect);
            }
            else
            {
                Rectangle rect = control.RenderBounds;
                Rectangle dragRect = rect;
                dragRect.Width = dragSize;
                Rectangle clientRect = rect;
                clientRect.X += dragSize;
                clientRect.Width -= dragSize;

                Textures.ToolWindow.H.DragBar.Draw(Renderer, dragRect);
                Textures.ToolWindow.H.Client.Draw(Renderer, clientRect);
            }
        }

        public override void DrawHighlight(ControlBase control)
        {
            Rectangle rect = control.RenderBounds;
            Renderer.DrawColor = new Color(a: 255, r: 255, g: 100, b: 255);
            Renderer.DrawFilledRect(rect);
        }

        public override void DrawScrollBar(ControlBase control, bool horizontal, bool depressed)
        {
            if (horizontal)
            {
                Textures.Scroller.TrackH.Draw(Renderer, control.RenderBounds);
            }
            else
            {
                Textures.Scroller.TrackV.Draw(Renderer, control.RenderBounds);
            }
        }

        public override void DrawScrollBarBar(ControlBase control, bool depressed, bool hovered, bool horizontal)
        {
            if (!horizontal)
            {
                if (control.IsDisabled)
                {
                    Textures.Scroller.ButtonV_Disabled.Draw(Renderer, control.RenderBounds);

                    return;
                }

                if (depressed)
                {
                    Textures.Scroller.ButtonV_Down.Draw(Renderer, control.RenderBounds);

                    return;
                }

                if (hovered)
                {
                    Textures.Scroller.ButtonV_Hover.Draw(Renderer, control.RenderBounds);

                    return;
                }

                Textures.Scroller.ButtonV_Normal.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (control.IsDisabled)
            {
                Textures.Scroller.ButtonH_Disabled.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (depressed)
            {
                Textures.Scroller.ButtonH_Down.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (hovered)
            {
                Textures.Scroller.ButtonH_Hover.Draw(Renderer, control.RenderBounds);

                return;
            }

            Textures.Scroller.ButtonH_Normal.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawProgressBar(ControlBase control, bool horizontal, float progress)
        {
            Rectangle rect = control.RenderBounds;

            if (horizontal)
            {
                Textures.ProgressBar.Back.Draw(Renderer, rect);

                if (progress > 0)
                {
                    rect.Width = (int)(rect.Width * progress);

                    if (rect.Width >= 5.0f)
                    {
                        Textures.ProgressBar.Front.Draw(Renderer, rect);
                    }
                }
            }
            else
            {
                Textures.ProgressBar.Back.Draw(Renderer, rect);

                if (progress > 0)
                {
                    rect.Y = (int)(rect.Y + (rect.Height * (1 - progress)));
                    rect.Height = (int)(rect.Height * progress);

                    if (rect.Height >= 5.0f)
                    {
                        Textures.ProgressBar.Front.Draw(Renderer, rect);
                    }
                }
            }
        }

        public override void DrawListBox(ControlBase control)
        {
            Textures.Input.ListBox.Background.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawListBoxLine(ControlBase control, bool selected, bool even)
        {
            if (selected)
            {
                if (even)
                {
                    Textures.Input.ListBox.EvenLineSelected.Draw(Renderer, control.RenderBounds);

                    return;
                }

                Textures.Input.ListBox.OddLineSelected.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (control.IsHovered)
            {
                Textures.Input.ListBox.Hovered.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (even)
            {
                Textures.Input.ListBox.EvenLine.Draw(Renderer, control.RenderBounds);

                return;
            }

            Textures.Input.ListBox.OddLine.Draw(Renderer, control.RenderBounds);
        }

        public void DrawSliderNotchesH(Rectangle rect, int numNotches, float dist)
        {
            if (numNotches == 0)
            {
                return;
            }

            float iSpacing = rect.Width / (float)numNotches;

            for (int i = 0; i < numNotches + 1; i++)
            {
                Renderer.DrawFilledRect(Util.FloatRect(rect.X + (iSpacing * i), rect.Y + dist - 2, w: 1, h: 5));
            }
        }

        public void DrawSliderNotchesV(Rectangle rect, int numNotches, float dist)
        {
            if (numNotches == 0)
            {
                return;
            }

            float iSpacing = rect.Height / (float)numNotches;

            for (int i = 0; i < numNotches + 1; i++)
            {
                Renderer.DrawFilledRect(Util.FloatRect(rect.X + dist - 2, rect.Y + (iSpacing * i), w: 5, h: 1));
            }
        }

        public override void DrawSlider(ControlBase control, bool horizontal, int numNotches, int barSize)
        {
            Rectangle rect = control.RenderBounds;
            Renderer.DrawColor = new Color(a: 100, r: 0, g: 0, b: 0);

            if (horizontal)
            {
                rect.X += (int)(barSize * 0.5);
                rect.Width -= barSize;
                rect.Y += (int)((rect.Height * 0.5) - 1);
                rect.Height = 1;
                DrawSliderNotchesH(rect, numNotches, barSize * 0.5f);
                Renderer.DrawFilledRect(rect);

                return;
            }

            rect.Y += (int)(barSize * 0.5);
            rect.Height -= barSize;
            rect.X += (int)((rect.Width * 0.5) - 1);
            rect.Width = 1;
            DrawSliderNotchesV(rect, numNotches, barSize * 0.4f);
            Renderer.DrawFilledRect(rect);
        }

        public override void DrawComboBox(ControlBase control, bool down, bool open)
        {
            if (control.IsDisabled)
            {
                Textures.Input.ComboBox.Disabled.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (down || open)
            {
                Textures.Input.ComboBox.Down.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (control.IsHovered)
            {
                Textures.Input.ComboBox.Down.Draw(Renderer, control.RenderBounds);

                return;
            }

            Textures.Input.ComboBox.Normal.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawKeyboardHighlight(ControlBase control, Rectangle r, int offset)
        {
            Rectangle rect = r;

            rect.X += offset;
            rect.Y += offset;
            rect.Width -= offset * 2;
            rect.Height -= offset * 2;

            //draw the top and bottom
            bool skip = true;

            for (int i = 0; i < rect.Width * 0.5; i++)
            {
                m_Renderer.DrawColor = Color.Black;

                if (!skip)
                {
                    Renderer.DrawPixel(rect.X + (i * 2), rect.Y);
                    Renderer.DrawPixel(rect.X + (i * 2), rect.Y + rect.Height - 1);
                }
                else
                {
                    skip = false;
                }
            }

            for (int i = 0; i < rect.Height * 0.5; i++)
            {
                Renderer.DrawColor = Color.Black;
                Renderer.DrawPixel(rect.X, rect.Y + (i * 2));
                Renderer.DrawPixel(rect.X + rect.Width - 1, rect.Y + (i * 2));
            }
        }

        public override void DrawToolTip(ControlBase control)
        {
            Textures.Tooltip.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawScrollButton(ControlBase control, ScrollBarButtonDirection direction, bool depressed,
            bool hovered, bool disabled)
        {
            int i = 0;

            if (direction == ScrollBarButtonDirection.Top)
            {
                i = 1;
            }

            if (direction == ScrollBarButtonDirection.Right)
            {
                i = 2;
            }

            if (direction == ScrollBarButtonDirection.Bottom)
            {
                i = 3;
            }

            if (disabled)
            {
                Textures.Scroller.Button.Disabled[i].Draw(Renderer, control.RenderBounds);

                return;
            }

            if (depressed)
            {
                Textures.Scroller.Button.Down[i].Draw(Renderer, control.RenderBounds);

                return;
            }

            if (hovered)
            {
                Textures.Scroller.Button.Hover[i].Draw(Renderer, control.RenderBounds);

                return;
            }

            Textures.Scroller.Button.Normal[i].Draw(Renderer, control.RenderBounds);
        }

        public override void DrawComboBoxArrow(ControlBase control, bool hovered, bool down, bool open, bool disabled)
        {
            if (disabled)
            {
                Textures.Input.ComboBox.Button.Disabled.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (down || open)
            {
                Textures.Input.ComboBox.Button.Down.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (hovered)
            {
                Textures.Input.ComboBox.Button.Hover.Draw(Renderer, control.RenderBounds);

                return;
            }

            Textures.Input.ComboBox.Button.Normal.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawNumericUpDownButton(ControlBase control, bool depressed, bool up)
        {
            if (up)
            {
                if (control.IsDisabled)
                {
                    Textures.Input.UpDown.Up.Disabled.Draw(Renderer, control.RenderBounds);

                    return;
                }

                if (depressed)
                {
                    Textures.Input.UpDown.Up.Down.Draw(Renderer, control.RenderBounds);

                    return;
                }

                if (control.IsHovered)
                {
                    Textures.Input.UpDown.Up.Hover.Draw(Renderer, control.RenderBounds);

                    return;
                }

                Textures.Input.UpDown.Up.Normal.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (control.IsDisabled)
            {
                Textures.Input.UpDown.Down.Disabled.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (depressed)
            {
                Textures.Input.UpDown.Down.Down.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (control.IsHovered)
            {
                Textures.Input.UpDown.Down.Hover.Draw(Renderer, control.RenderBounds);

                return;
            }

            Textures.Input.UpDown.Down.Normal.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawStatusBar(ControlBase control)
        {
            Textures.StatusBar.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawTreeButton(ControlBase control, bool open)
        {
            Rectangle rect = control.RenderBounds;

            if (open)
            {
                Textures.Tree.Minus.Draw(Renderer, rect);
            }
            else
            {
                Textures.Tree.Plus.Draw(Renderer, rect);
            }
        }

        public override void DrawTreeControl(ControlBase control)
        {
            Textures.Tree.Background.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawTreeNode(ControlBase ctrl, bool open, bool selected, int labelHeight, int labelWidth,
            int halfWay, int lastBranch, bool isRoot, int indent)
        {
            if (selected)
            {
                Textures.Selection.Draw(Renderer, new Rectangle(indent, y: 0, labelWidth, labelHeight));
            }

            base.DrawTreeNode(ctrl, open, selected, labelHeight, labelWidth, halfWay, lastBranch, isRoot, indent);
        }

        public override void DrawColorDisplay(ControlBase control, Color color)
        {
            Rectangle rect = control.RenderBounds;

            if (color.A != 255)
            {
                Renderer.DrawColor = new Color(a: 255, r: 255, g: 255, b: 255);
                Renderer.DrawFilledRect(rect);

                Renderer.DrawColor = new Color(a: 128, r: 128, g: 128, b: 128);

                Renderer.DrawFilledRect(Util.FloatRect(x: 0, y: 0, rect.Width * 0.5f, rect.Height * 0.5f));

                Renderer.DrawFilledRect(
                    Util.FloatRect(rect.Width * 0.5f, rect.Height * 0.5f, rect.Width * 0.5f, rect.Height * 0.5f));
            }

            Renderer.DrawColor = color;
            Renderer.DrawFilledRect(rect);

            Renderer.DrawColor = Color.Black;
            Renderer.DrawLinedRect(rect);
        }

        public override void DrawModalControl(ControlBase control, Color? backgroundColor)
        {
            if (!control.ShouldDrawBackground)
            {
                return;
            }

            Rectangle rect = control.RenderBounds;

            if (backgroundColor == null)
            {
                Renderer.DrawColor = Colors.ModalBackground;
            }
            else
            {
                Renderer.DrawColor = (Color)backgroundColor;
            }

            Renderer.DrawFilledRect(rect);
        }

        public override void DrawMenuDivider(ControlBase control)
        {
            Rectangle rect = control.RenderBounds;
            Renderer.DrawColor = new Color(a: 100, r: 0, g: 0, b: 0);
            Renderer.DrawFilledRect(rect);
        }

        public override void DrawWindowCloseButton(ControlBase control, bool depressed, bool hovered, bool disabled)
        {
            if (disabled)
            {
                Textures.Window.Close_Disabled.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (depressed)
            {
                Textures.Window.Close_Down.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (hovered)
            {
                Textures.Window.Close_Hover.Draw(Renderer, control.RenderBounds);

                return;
            }

            Textures.Window.Close.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawSliderButton(ControlBase control, bool depressed, bool horizontal)
        {
            if (!horizontal)
            {
                if (control.IsDisabled)
                {
                    Textures.Input.Slider.V.Disabled.Draw(Renderer, control.RenderBounds);

                    return;
                }

                if (depressed)
                {
                    Textures.Input.Slider.V.Down.Draw(Renderer, control.RenderBounds);

                    return;
                }

                if (control.IsHovered)
                {
                    Textures.Input.Slider.V.Hover.Draw(Renderer, control.RenderBounds);

                    return;
                }

                Textures.Input.Slider.V.Normal.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (control.IsDisabled)
            {
                Textures.Input.Slider.H.Disabled.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (depressed)
            {
                Textures.Input.Slider.H.Down.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (control.IsHovered)
            {
                Textures.Input.Slider.H.Hover.Draw(Renderer, control.RenderBounds);

                return;
            }

            Textures.Input.Slider.H.Normal.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawCategoryHolder(ControlBase control)
        {
            Textures.CategoryList.Outer.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawCategoryInner(ControlBase control, int headerHeight, bool collapsed)
        {
            if (collapsed)
            {
                Textures.CategoryList.Header.Draw(Renderer, control.RenderBounds);
            }
            else
            {
                Rectangle rect = control.RenderBounds;
                Rectangle headerRect = rect;
                headerRect.Height = headerHeight;
                Rectangle clientRect = rect;
                clientRect.Y += headerHeight;
                clientRect.Height -= headerHeight;
                Textures.CategoryList.Inner.Header.Draw(Renderer, headerRect);
                Textures.CategoryList.Inner.Client.Draw(Renderer, clientRect);
            }
        }

        public override void DrawBorder(ControlBase control, BorderType borderType)
        {
            switch (borderType)
            {
                case BorderType.ToolTip:
                    Textures.Tooltip.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.StatusBar:
                    Textures.StatusBar.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.MenuStrip:
                    Textures.Menu.Strip.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.Selection:
                    Textures.Selection.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.PanelNormal:
                    Textures.Panel.Normal.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.PanelBright:
                    Textures.Panel.Bright.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.PanelDark:
                    Textures.Panel.Dark.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.PanelHighlight:
                    Textures.Panel.Highlight.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.ListBox:
                    Textures.Input.ListBox.Background.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.TreeControl:
                    Textures.Tree.Background.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.CategoryList:
                    Textures.CategoryList.Outer.Draw(Renderer, control.RenderBounds);

                    break;
            }
        }

        #endregion
    }
}