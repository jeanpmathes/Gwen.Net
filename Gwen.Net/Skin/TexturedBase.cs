using System;
using System.IO;
using Gwen.Net.Control;
using Gwen.Net.Control.Internal;
using Gwen.Net.Renderer;
using Gwen.Net.Skin.Texturing;
using Single = Gwen.Net.Skin.Texturing.Single;

namespace Gwen.Net.Skin
{
    #region UI element textures

    internal struct SkinTextures
    {
        public Bordered statusBar;
        public Bordered selection;
        public Bordered shadow;
        public Bordered tooltip;

        public struct ToolWindow
        {
            public struct H
            {
                public Bordered dragBar;
                public Bordered client;
            }

            public struct V
            {
                public Bordered dragBar;
                public Bordered client;
            }

            public H h;
            public V v;
        }

        public ToolWindow toolWindow;

        public struct Panel
        {
            public Bordered normal;
            public Bordered bright;
            public Bordered dark;
            public Bordered highlight;
        }

        public struct Window
        {
            public struct Normal
            {
                public Bordered titleBar;
                public Bordered client;
            }

            public struct Inactive
            {
                public Bordered titleBar;
                public Bordered client;
            }

            public Normal normal;
            public Inactive inactive;

            public Single close;
            public Single closeHover;
            public Single closeDown;
            public Single closeDisabled;
        }

        public struct CheckBox
        {
            public struct Active
            {
                public Single normal;
                public Single @checked;
            }

            public struct Disabled
            {
                public Single normal;
                public Single @checked;
            }

            public Active active;
            public Disabled disabled;
        }

        public struct RadioButton
        {
            public struct Active
            {
                public Single normal;
                public Single @checked;
            }

            public struct Disabled
            {
                public Single normal;
                public Single @checked;
            }

            public Active active;
            public Disabled disabled;
        }

        public struct TextBox
        {
            public Bordered normal;
            public Bordered focus;
            public Bordered disabled;
        }

        public struct Tree
        {
            public Bordered background;
            public Single minus;
            public Single plus;
        }

        public struct ProgressBar
        {
            public Bordered back;
            public Bordered front;
        }

        public struct Scroller
        {
            public Bordered trackV;
            public Bordered trackH;
            public Bordered buttonVNormal;
            public Bordered buttonVHover;
            public Bordered buttonVDown;
            public Bordered buttonVDisabled;
            public Bordered buttonHNormal;
            public Bordered buttonHHover;
            public Bordered buttonHDown;
            public Bordered buttonHDisabled;

            public struct Button
            {
                public Bordered[] normal;
                public Bordered[] hover;
                public Bordered[] down;
                public Bordered[] disabled;
            }

            public Button button;
        }

        public struct Menu
        {
            public Single rightArrow;
            public Single check;

            public Bordered strip;
            public Bordered background;
            public Bordered backgroundWithMargin;
            public Bordered hover;
        }

        public struct Input
        {
            public struct Button
            {
                public Bordered normal;
                public Bordered hovered;
                public Bordered disabled;
                public Bordered pressed;
            }

            public struct ComboBox
            {
                public Bordered normal;
                public Bordered hover;
                public Bordered down;
                public Bordered disabled;

                public struct ButtonColors
                {
                    public Single normal;
                    public Single hover;
                    public Single down;
                    public Single disabled;
                }

                public ButtonColors buttonColors;
            }

            public struct Slider
            {
                public struct H
                {
                    public Single normal;
                    public Single hover;
                    public Single down;
                    public Single disabled;
                }

                public struct V
                {
                    public Single normal;
                    public Single hover;
                    public Single down;
                    public Single disabled;
                }

                public H h;
                public V v;
            }

            public struct ListBox
            {
                public Bordered background;
                public Bordered hovered;
                public Bordered evenLine;
                public Bordered oddLine;
                public Bordered evenLineSelected;
                public Bordered oddLineSelected;
            }

            public struct UpDown
            {
                public struct Up
                {
                    public Single normal;
                    public Single hover;
                    public Single down;
                    public Single disabled;
                }

                public struct Down
                {
                    public Single normal;
                    public Single hover;
                    public Single down;
                    public Single disabled;
                }

                public Up up;
                public Down down;
            }

            public Button button;
            public ComboBox comboBox;
            public Slider slider;
            public ListBox listBox;
            public UpDown upDown;
        }

        public struct Tab
        {
            public struct Bottom
            {
                public Bordered inactive;
                public Bordered active;
            }

            public struct Top
            {
                public Bordered inactive;
                public Bordered active;
            }

            public struct Left
            {
                public Bordered inactive;
                public Bordered active;
            }

            public struct Right
            {
                public Bordered inactive;
                public Bordered active;
            }

            public Bottom bottom;
            public Top top;
            public Left left;
            public Right right;

            public Bordered control;
            public Bordered headerBar;
        }

        public struct CategoryList
        {
            public struct Inner
            {
                public Bordered header;
                public Bordered client;
            }

            public Bordered outer;
            public Inner inner;
            public Bordered header;
        }

        public Panel panel;
        public Window window;
        public CheckBox checkBox;
        public RadioButton radioButton;
        public TextBox textBox;
        public Tree tree;
        public ProgressBar progressBar;
        public Scroller scroller;
        public Menu menu;
        public Input input;
        public Tab tab;
        public CategoryList categoryList;
    }

    #endregion

    /// <summary>
    ///     Base textured skin.
    /// </summary>
    public class TexturedBase : SkinBase
    {
        private readonly Texture texture;
        private SkinTextures textures;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TexturedBase" /> class.
        /// </summary>
        /// <param name="renderer">Renderer to use.</param>
        /// <param name="texturePath">Path of the skin texture map.</param>
        /// <param name="errorCallback">Callback to invoke when a skin-loading error occurs.</param>
        public TexturedBase(RendererBase renderer, FileInfo texturePath, Action<Exception> errorCallback)
            : base(renderer)
        {
            texture = new Texture(Renderer);
            texture.Load(texturePath.FullName, errorCallback);

            InitializeColors();
            InitializeTextures();
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            texture.Dispose();
            base.Dispose();
        }

        #region Initialization

        private void InitializeColors()
        {
            colors.windowColors.titleActive = Renderer.PixelColor(texture, 4 + (8 * 0), y: 508, Color.Red);
            colors.windowColors.titleInactive = Renderer.PixelColor(texture, 4 + (8 * 1), y: 508, Color.Yellow);

            colors.buttonColors.normal = Renderer.PixelColor(texture, 4 + (8 * 2), y: 508, Color.Yellow);
            colors.buttonColors.hover = Renderer.PixelColor(texture, 4 + (8 * 3), y: 508, Color.Yellow);
            colors.buttonColors.down = Renderer.PixelColor(texture, 4 + (8 * 2), y: 500, Color.Yellow);
            colors.buttonColors.disabled = Renderer.PixelColor(texture, 4 + (8 * 3), y: 500, Color.Yellow);

            colors.tabColors.activeColors.normal = Renderer.PixelColor(texture, 4 + (8 * 4), y: 508, Color.Yellow);
            colors.tabColors.activeColors.hover = Renderer.PixelColor(texture, 4 + (8 * 5), y: 508, Color.Yellow);
            colors.tabColors.activeColors.down = Renderer.PixelColor(texture, 4 + (8 * 4), y: 500, Color.Yellow);
            colors.tabColors.activeColors.disabled = Renderer.PixelColor(texture, 4 + (8 * 5), y: 500, Color.Yellow);
            colors.tabColors.inactiveColors.normal = Renderer.PixelColor(texture, 4 + (8 * 6), y: 508, Color.Yellow);
            colors.tabColors.inactiveColors.hover = Renderer.PixelColor(texture, 4 + (8 * 7), y: 508, Color.Yellow);
            colors.tabColors.inactiveColors.down = Renderer.PixelColor(texture, 4 + (8 * 6), y: 500, Color.Yellow);
            colors.tabColors.inactiveColors.disabled = Renderer.PixelColor(texture, 4 + (8 * 7), y: 500, Color.Yellow);

            colors.labelColors.@default = Renderer.PixelColor(texture, 4 + (8 * 8), y: 508, Color.Yellow);
            colors.labelColors.bright = Renderer.PixelColor(texture, 4 + (8 * 9), y: 508, Color.Yellow);
            colors.labelColors.dark = Renderer.PixelColor(texture, 4 + (8 * 8), y: 500, Color.Yellow);
            colors.labelColors.highlight = Renderer.PixelColor(texture, 4 + (8 * 9), y: 500, Color.Yellow);

            colors.textBoxColors.text = Renderer.PixelColor(texture, 4 + (8 * 8), y: 508, Color.Yellow);
            colors.textBoxColors.backgroundSelected = Renderer.PixelColor(texture, 4 + (8 * 10), y: 500, Color.Yellow);
            colors.textBoxColors.caret = Renderer.PixelColor(texture, 4 + (8 * 8), y: 508, Color.Yellow);

            colors.listBoxColors.textNormal = Renderer.PixelColor(texture, 4 + (8 * 11), y: 508, Color.Yellow);
            colors.listBoxColors.textSelected = Renderer.PixelColor(texture, 4 + (8 * 11), y: 500, Color.Yellow);

            colors.treeColors.lines = Renderer.PixelColor(texture, 4 + (8 * 10), y: 508, Color.Yellow);
            colors.treeColors.normal = Renderer.PixelColor(texture, 4 + (8 * 11), y: 508, Color.Yellow);
            colors.treeColors.hover = Renderer.PixelColor(texture, 4 + (8 * 10), y: 500, Color.Yellow);
            colors.treeColors.selected = Renderer.PixelColor(texture, 4 + (8 * 11), y: 500, Color.Yellow);

            colors.propertiesColors.lineNormal = Renderer.PixelColor(texture, 4 + (8 * 12), y: 508, Color.Yellow);
            colors.propertiesColors.lineSelected = Renderer.PixelColor(texture, 4 + (8 * 13), y: 508, Color.Yellow);
            colors.propertiesColors.lineHover = Renderer.PixelColor(texture, 4 + (8 * 12), y: 500, Color.Yellow);
            colors.propertiesColors.title = Renderer.PixelColor(texture, 4 + (8 * 13), y: 500, Color.Yellow);
            colors.propertiesColors.columnNormal = Renderer.PixelColor(texture, 4 + (8 * 14), y: 508, Color.Yellow);
            colors.propertiesColors.columnSelected = Renderer.PixelColor(texture, 4 + (8 * 15), y: 508, Color.Yellow);
            colors.propertiesColors.columnHover = Renderer.PixelColor(texture, 4 + (8 * 14), y: 500, Color.Yellow);
            colors.propertiesColors.border = Renderer.PixelColor(texture, 4 + (8 * 15), y: 500, Color.Yellow);
            colors.propertiesColors.labelNormal = Renderer.PixelColor(texture, 4 + (8 * 16), y: 508, Color.Yellow);
            colors.propertiesColors.labelSelected = Renderer.PixelColor(texture, 4 + (8 * 17), y: 508, Color.Yellow);
            colors.propertiesColors.labelHover = Renderer.PixelColor(texture, 4 + (8 * 16), y: 500, Color.Yellow);

            colors.modalBackground = Renderer.PixelColor(texture, 4 + (8 * 18), y: 508, Color.Yellow);

            colors.tooltipText = Renderer.PixelColor(texture, 4 + (8 * 19), y: 508, Color.Yellow);

            colors.categoryColors.header = Renderer.PixelColor(texture, 4 + (8 * 18), y: 500, Color.Yellow);
            colors.categoryColors.headerClosed = Renderer.PixelColor(texture, 4 + (8 * 19), y: 500, Color.Yellow);
            colors.categoryColors.lineColors.text = Renderer.PixelColor(texture, 4 + (8 * 20), y: 508, Color.Yellow);
            colors.categoryColors.lineColors.textHover = Renderer.PixelColor(texture, 4 + (8 * 21), y: 508, Color.Yellow);
            colors.categoryColors.lineColors.textSelected = Renderer.PixelColor(texture, 4 + (8 * 20), y: 500, Color.Yellow);
            colors.categoryColors.lineColors.button = Renderer.PixelColor(texture, 4 + (8 * 21), y: 500, Color.Yellow);
            colors.categoryColors.lineColors.buttonHover = Renderer.PixelColor(texture, 4 + (8 * 22), y: 508, Color.Yellow);
            colors.categoryColors.lineColors.buttonSelected = Renderer.PixelColor(texture, 4 + (8 * 23), y: 508, Color.Yellow);
            colors.categoryColors.lineAltColors.text = Renderer.PixelColor(texture, 4 + (8 * 22), y: 500, Color.Yellow);
            colors.categoryColors.lineAltColors.textHover = Renderer.PixelColor(texture, 4 + (8 * 23), y: 500, Color.Yellow);
            colors.categoryColors.lineAltColors.textSelected = Renderer.PixelColor(texture, 4 + (8 * 24), y: 508, Color.Yellow);
            colors.categoryColors.lineAltColors.button = Renderer.PixelColor(texture, 4 + (8 * 25), y: 508, Color.Yellow);
            colors.categoryColors.lineAltColors.buttonHover = Renderer.PixelColor(texture, 4 + (8 * 24), y: 500, Color.Yellow);

            colors.categoryColors.lineAltColors.buttonSelected =
                Renderer.PixelColor(texture, 4 + (8 * 25), y: 500, Color.Yellow);
        }

        private void InitializeTextures()
        {
            textures.shadow = new Bordered(texture, x: 448, y: 0, w: 31, h: 31, Margin.Eight);
            textures.tooltip = new Bordered(texture, x: 128, y: 320, w: 127, h: 31, Margin.Eight);
            textures.statusBar = new Bordered(texture, x: 128, y: 288, w: 127, h: 31, Margin.Eight);
            textures.selection = new Bordered(texture, x: 384, y: 32, w: 31, h: 31, Margin.Four);

            textures.panel.normal = new Bordered(
                texture,
                x: 256,
                y: 0,
                w: 63,
                h: 63,
                new Margin(left: 16, top: 16, right: 16, bottom: 16));

            textures.panel.bright = new Bordered(
                texture,
                256 + 64,
                y: 0,
                w: 63,
                h: 63,
                new Margin(left: 16, top: 16, right: 16, bottom: 16));

            textures.panel.dark = new Bordered(
                texture,
                x: 256,
                y: 64,
                w: 63,
                h: 63,
                new Margin(left: 16, top: 16, right: 16, bottom: 16));

            textures.panel.highlight = new Bordered(
                texture,
                256 + 64,
                y: 64,
                w: 63,
                h: 63,
                new Margin(left: 16, top: 16, right: 16, bottom: 16));

            textures.window.normal.titleBar = new Bordered(
                texture,
                x: 0,
                y: 0,
                w: 127,
                h: 24,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            textures.window.normal.client = new Bordered(
                texture,
                x: 0,
                y: 24,
                w: 127,
                127 - 24,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            textures.window.inactive.titleBar = new Bordered(
                texture,
                x: 128,
                y: 0,
                w: 127,
                h: 24,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            textures.window.inactive.client = new Bordered(
                texture,
                x: 128,
                y: 24,
                w: 127,
                127 - 24,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            textures.toolWindow.h.dragBar = new Bordered(
                texture,
                x: 384,
                y: 464,
                w: 17,
                h: 31,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            textures.toolWindow.h.client = new Bordered(
                texture,
                384 + 17,
                y: 464,
                127 - 17,
                h: 31,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            textures.toolWindow.v.dragBar = new Bordered(
                texture,
                x: 256,
                y: 384,
                w: 31,
                h: 17,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            textures.toolWindow.v.client = new Bordered(
                texture,
                x: 256,
                384 + 17,
                w: 31,
                127 - 17,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            textures.checkBox.active.@checked = new Single(texture, x: 448, y: 32, w: 15, h: 15);
            textures.checkBox.active.normal = new Single(texture, x: 464, y: 32, w: 15, h: 15);
            textures.checkBox.disabled.normal = new Single(texture, x: 448, y: 48, w: 15, h: 15);
            textures.checkBox.disabled.@checked = new Single(texture, x: 464, y: 48, w: 15, h: 15);

            textures.radioButton.active.@checked = new Single(texture, x: 448, y: 64, w: 15, h: 15);
            textures.radioButton.active.normal = new Single(texture, x: 464, y: 64, w: 15, h: 15);
            textures.radioButton.disabled.normal = new Single(texture, x: 448, y: 80, w: 15, h: 15);
            textures.radioButton.disabled.@checked = new Single(texture, x: 464, y: 80, w: 15, h: 15);

            textures.textBox.normal = new Bordered(texture, x: 0, y: 150, w: 127, h: 21, Margin.Four);
            textures.textBox.focus = new Bordered(texture, x: 0, y: 172, w: 127, h: 21, Margin.Four);
            textures.textBox.disabled = new Bordered(texture, x: 0, y: 193, w: 127, h: 21, Margin.Four);

            textures.menu.strip = new Bordered(texture, x: 0, y: 128, w: 127, h: 21, Margin.One);

            textures.menu.backgroundWithMargin = new Bordered(
                texture,
                x: 128,
                y: 128,
                w: 127,
                h: 63,
                new Margin(left: 24, top: 8, right: 8, bottom: 8));

            textures.menu.background = new Bordered(texture, x: 128, y: 192, w: 127, h: 63, Margin.Eight);
            textures.menu.hover = new Bordered(texture, x: 128, y: 256, w: 127, h: 31, Margin.Eight);
            textures.menu.rightArrow = new Single(texture, x: 464, y: 112, w: 15, h: 15);
            textures.menu.check = new Single(texture, x: 448, y: 112, w: 15, h: 15);

            textures.tab.control = new Bordered(texture, x: 0, y: 256, w: 127, h: 127, Margin.Eight);
            textures.tab.bottom.active = new Bordered(texture, x: 0, y: 416, w: 63, h: 31, Margin.Eight);
            textures.tab.bottom.inactive = new Bordered(texture, 0 + 128, y: 416, w: 63, h: 31, Margin.Eight);
            textures.tab.top.active = new Bordered(texture, x: 0, y: 384, w: 63, h: 31, Margin.Eight);
            textures.tab.top.inactive = new Bordered(texture, 0 + 128, y: 384, w: 63, h: 31, Margin.Eight);
            textures.tab.left.active = new Bordered(texture, x: 64, y: 384, w: 31, h: 63, Margin.Eight);
            textures.tab.left.inactive = new Bordered(texture, 64 + 128, y: 384, w: 31, h: 63, Margin.Eight);
            textures.tab.right.active = new Bordered(texture, x: 96, y: 384, w: 31, h: 63, Margin.Eight);
            textures.tab.right.inactive = new Bordered(texture, 96 + 128, y: 384, w: 31, h: 63, Margin.Eight);
            textures.tab.headerBar = new Bordered(texture, x: 128, y: 352, w: 127, h: 31, Margin.Four);

            textures.window.close = new Single(texture, x: 0, y: 224, w: 24, h: 24);
            textures.window.closeHover = new Single(texture, x: 32, y: 224, w: 24, h: 24);
            textures.window.closeDown = new Single(texture, x: 64, y: 224, w: 24, h: 24);
            textures.window.closeDisabled = new Single(texture, x: 96, y: 224, w: 24, h: 24);

            textures.scroller.trackV = new Bordered(texture, x: 384, y: 208, w: 15, h: 127, Margin.Four);
            textures.scroller.buttonVNormal = new Bordered(texture, 384 + 16, y: 208, w: 15, h: 127, Margin.Four);
            textures.scroller.buttonVHover = new Bordered(texture, 384 + 32, y: 208, w: 15, h: 127, Margin.Four);
            textures.scroller.buttonVDown = new Bordered(texture, 384 + 48, y: 208, w: 15, h: 127, Margin.Four);
            textures.scroller.buttonVDisabled = new Bordered(texture, 384 + 64, y: 208, w: 15, h: 127, Margin.Four);
            textures.scroller.trackH = new Bordered(texture, x: 384, y: 128, w: 127, h: 15, Margin.Four);
            textures.scroller.buttonHNormal = new Bordered(texture, x: 384, 128 + 16, w: 127, h: 15, Margin.Four);
            textures.scroller.buttonHHover = new Bordered(texture, x: 384, 128 + 32, w: 127, h: 15, Margin.Four);
            textures.scroller.buttonHDown = new Bordered(texture, x: 384, 128 + 48, w: 127, h: 15, Margin.Four);
            textures.scroller.buttonHDisabled = new Bordered(texture, x: 384, 128 + 64, w: 127, h: 15, Margin.Four);

            textures.scroller.button.normal = new Bordered[4];
            textures.scroller.button.disabled = new Bordered[4];
            textures.scroller.button.hover = new Bordered[4];
            textures.scroller.button.down = new Bordered[4];

            textures.tree.background = new Bordered(
                texture,
                x: 256,
                y: 128,
                w: 127,
                h: 127,
                new Margin(left: 16, top: 16, right: 16, bottom: 16));

            textures.tree.plus = new Single(texture, x: 448, y: 96, w: 15, h: 15);
            textures.tree.minus = new Single(texture, x: 464, y: 96, w: 15, h: 15);

            textures.input.button.normal = new Bordered(texture, x: 480, y: 0, w: 31, h: 31, Margin.Eight);
            textures.input.button.hovered = new Bordered(texture, x: 480, y: 32, w: 31, h: 31, Margin.Eight);
            textures.input.button.disabled = new Bordered(texture, x: 480, y: 64, w: 31, h: 31, Margin.Eight);
            textures.input.button.pressed = new Bordered(texture, x: 480, y: 96, w: 31, h: 31, Margin.Eight);

            for (var i = 0; i < 4; i++)
            {
                textures.scroller.button.normal[i] = new Bordered(
                    texture,
                    464 + 0,
                    208 + (i * 16),
                    w: 15,
                    h: 15,
                    Margin.Two);

                textures.scroller.button.hover[i] = new Bordered(
                    texture,
                    x: 480,
                    208 + (i * 16),
                    w: 15,
                    h: 15,
                    Margin.Two);

                textures.scroller.button.down[i] = new Bordered(
                    texture,
                    x: 464,
                    272 + (i * 16),
                    w: 15,
                    h: 15,
                    Margin.Two);

                textures.scroller.button.disabled[i] = new Bordered(
                    texture,
                    480 + 48,
                    272 + (i * 16),
                    w: 15,
                    h: 15,
                    Margin.Two);
            }

            textures.input.listBox.background = new Bordered(texture, x: 256, y: 256, w: 63, h: 127, Margin.Eight);
            textures.input.listBox.hovered = new Bordered(texture, x: 320, y: 320, w: 31, h: 31, Margin.Eight);
            textures.input.listBox.evenLine = new Bordered(texture, x: 352, y: 256, w: 31, h: 31, Margin.Eight);
            textures.input.listBox.oddLine = new Bordered(texture, x: 352, y: 288, w: 31, h: 31, Margin.Eight);

            textures.input.listBox.evenLineSelected = new Bordered(
                texture,
                x: 320,
                y: 270,
                w: 31,
                h: 31,
                Margin.Eight);

            textures.input.listBox.oddLineSelected = new Bordered(
                texture,
                x: 320,
                y: 288,
                w: 31,
                h: 31,
                Margin.Eight);

            textures.input.comboBox.normal = new Bordered(
                texture,
                x: 384,
                y: 336,
                w: 127,
                h: 31,
                new Margin(left: 8, top: 8, right: 32, bottom: 8));

            textures.input.comboBox.hover = new Bordered(
                texture,
                x: 384,
                336 + 32,
                w: 127,
                h: 31,
                new Margin(left: 8, top: 8, right: 32, bottom: 8));

            textures.input.comboBox.down = new Bordered(
                texture,
                x: 384,
                336 + 64,
                w: 127,
                h: 31,
                new Margin(left: 8, top: 8, right: 32, bottom: 8));

            textures.input.comboBox.disabled = new Bordered(
                texture,
                x: 384,
                336 + 96,
                w: 127,
                h: 31,
                new Margin(left: 8, top: 8, right: 32, bottom: 8));

            textures.input.comboBox.buttonColors.normal = new Single(texture, x: 496, y: 272, w: 15, h: 15);
            textures.input.comboBox.buttonColors.hover = new Single(texture, x: 496, 272 + 16, w: 15, h: 15);
            textures.input.comboBox.buttonColors.down = new Single(texture, x: 496, 272 + 32, w: 15, h: 15);
            textures.input.comboBox.buttonColors.disabled = new Single(texture, x: 496, 272 + 48, w: 15, h: 15);

            textures.input.upDown.up.normal = new Single(texture, x: 384, y: 112, w: 7, h: 7);
            textures.input.upDown.up.hover = new Single(texture, 384 + 8, y: 112, w: 7, h: 7);
            textures.input.upDown.up.down = new Single(texture, 384 + 16, y: 112, w: 7, h: 7);
            textures.input.upDown.up.disabled = new Single(texture, 384 + 24, y: 112, w: 7, h: 7);
            textures.input.upDown.down.normal = new Single(texture, x: 384, y: 120, w: 7, h: 7);
            textures.input.upDown.down.hover = new Single(texture, 384 + 8, y: 120, w: 7, h: 7);
            textures.input.upDown.down.down = new Single(texture, 384 + 16, y: 120, w: 7, h: 7);
            textures.input.upDown.down.disabled = new Single(texture, 384 + 24, y: 120, w: 7, h: 7);

            textures.progressBar.back = new Bordered(texture, x: 384, y: 0, w: 31, h: 31, Margin.Two);
            textures.progressBar.front = new Bordered(texture, 384 + 32, y: 0, w: 31, h: 31, Margin.Two);

            textures.input.slider.h.normal = new Single(texture, x: 416, y: 32, w: 15, h: 15);
            textures.input.slider.h.hover = new Single(texture, x: 416, 32 + 16, w: 15, h: 15);
            textures.input.slider.h.down = new Single(texture, x: 416, 32 + 32, w: 15, h: 15);
            textures.input.slider.h.disabled = new Single(texture, x: 416, 32 + 48, w: 15, h: 15);

            textures.input.slider.v.normal = new Single(texture, 416 + 16, y: 32, w: 15, h: 15);
            textures.input.slider.v.hover = new Single(texture, 416 + 16, 32 + 16, w: 15, h: 15);
            textures.input.slider.v.down = new Single(texture, 416 + 16, 32 + 32, w: 15, h: 15);
            textures.input.slider.v.disabled = new Single(texture, 416 + 16, 32 + 48, w: 15, h: 15);

            textures.categoryList.outer = new Bordered(texture, x: 256, y: 256, w: 63, h: 127, Margin.Eight);

            textures.categoryList.inner.header = new Bordered(
                texture,
                256 + 64,
                y: 384,
                w: 63,
                h: 20,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            textures.categoryList.inner.client = new Bordered(
                texture,
                256 + 64,
                384 + 20,
                w: 63,
                63 - 20,
                new Margin(left: 8, top: 8, right: 8, bottom: 8));

            textures.categoryList.header = new Bordered(texture, x: 320, y: 352, w: 63, h: 31, Margin.Eight);
        }

        #endregion

        #region UI elements

        public override void DrawButton(ControlBase control, bool depressed, bool hovered, bool disabled)
        {
            if (disabled)
            {
                textures.input.button.disabled.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (depressed)
            {
                textures.input.button.pressed.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (hovered)
            {
                textures.input.button.hovered.Draw(Renderer, control.RenderBounds);

                return;
            }

            textures.input.button.normal.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawMenuRightArrow(ControlBase control)
        {
            textures.menu.rightArrow.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawMenuItem(ControlBase control, bool submenuOpen, bool isChecked)
        {
            if (submenuOpen || control.IsHovered)
            {
                textures.menu.hover.Draw(Renderer, control.RenderBounds);
            }

            if (isChecked)
            {
                textures.menu.check.Draw(
                    Renderer,
                    new Rectangle(control.RenderBounds.X + 4, control.RenderBounds.Y + 3, width: 15, height: 15));
            }
        }

        public override void DrawMenuStrip(ControlBase control)
        {
            textures.menu.strip.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawMenu(ControlBase control, bool paddingDisabled)
        {
            if (!paddingDisabled)
            {
                textures.menu.backgroundWithMargin.Draw(Renderer, control.RenderBounds);

                return;
            }

            textures.menu.background.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawShadow(ControlBase control)
        {
            Rectangle r = control.RenderBounds;
            r.X -= 4;
            r.Y -= 4;
            r.Width += 10;
            r.Height += 10;
            textures.shadow.Draw(Renderer, r);
        }

        public override void DrawRadioButton(ControlBase control, bool selected, bool depressed)
        {
            if (selected)
            {
                if (control.IsDisabled)
                {
                    textures.radioButton.disabled.@checked.Draw(Renderer, control.RenderBounds);
                }
                else
                {
                    textures.radioButton.active.@checked.Draw(Renderer, control.RenderBounds);
                }
            }
            else
            {
                if (control.IsDisabled)
                {
                    textures.radioButton.disabled.normal.Draw(Renderer, control.RenderBounds);
                }
                else
                {
                    textures.radioButton.active.normal.Draw(Renderer, control.RenderBounds);
                }
            }
        }

        public override void DrawCheckBox(ControlBase control, bool selected, bool depressed)
        {
            if (selected)
            {
                if (control.IsDisabled)
                {
                    textures.checkBox.disabled.@checked.Draw(Renderer, control.RenderBounds);
                }
                else
                {
                    textures.checkBox.active.@checked.Draw(Renderer, control.RenderBounds);
                }
            }
            else
            {
                if (control.IsDisabled)
                {
                    textures.checkBox.disabled.normal.Draw(Renderer, control.RenderBounds);
                }
                else
                {
                    textures.checkBox.active.normal.Draw(Renderer, control.RenderBounds);
                }
            }
        }

        public override void DrawGroupBox(ControlBase control, int textStart, int textHeight, int textWidth)
        {
            Rectangle rect = control.RenderBounds;

            rect.Y += (int) (textHeight * 0.5f);
            rect.Height -= (int) (textHeight * 0.5f);

            Color colDarker = new(a: 50, r: 0, g: 50, b: 60);
            Color colLighter = new(a: 150, r: 255, g: 255, b: 255);

            Renderer.DrawColor = colLighter;

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

            Renderer.DrawColor = colDarker;

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
                textures.textBox.disabled.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (control.HasFocus)
            {
                textures.textBox.focus.Draw(Renderer, control.RenderBounds);
            }
            else
            {
                textures.textBox.normal.Draw(Renderer, control.RenderBounds);
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
                textures.tab.top.inactive.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (dir == Dock.Left)
            {
                textures.tab.left.inactive.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (dir == Dock.Bottom)
            {
                textures.tab.bottom.inactive.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (dir == Dock.Right)
            {
                textures.tab.right.inactive.Draw(Renderer, control.RenderBounds);
            }
        }

        private void DrawActiveTabButton(ControlBase control, Dock dir)
        {
            if (dir == Dock.Top)
            {
                textures.tab.top.active.Draw(
                    Renderer,
                    control.RenderBounds.Add(new Rectangle(x: 0, y: 0, width: 0, height: 8)));

                return;
            }

            if (dir == Dock.Left)
            {
                textures.tab.left.active.Draw(
                    Renderer,
                    control.RenderBounds.Add(new Rectangle(x: 0, y: 0, width: 8, height: 0)));

                return;
            }

            if (dir == Dock.Bottom)
            {
                textures.tab.bottom.active.Draw(
                    Renderer,
                    control.RenderBounds.Add(new Rectangle(x: 0, y: -8, width: 0, height: 8)));

                return;
            }

            if (dir == Dock.Right)
            {
                textures.tab.right.active.Draw(
                    Renderer,
                    control.RenderBounds.Add(new Rectangle(x: -8, y: 0, width: 8, height: 0)));
            }
        }

        public override void DrawTabControl(ControlBase control)
        {
            textures.tab.control.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawTabTitleBar(ControlBase control)
        {
            textures.tab.headerBar.Draw(Renderer, control.RenderBounds);
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
                textures.window.normal.titleBar.Draw(Renderer, titleRect);
                textures.window.normal.client.Draw(Renderer, clientRect);
            }
            else
            {
                textures.window.inactive.titleBar.Draw(Renderer, titleRect);
                textures.window.inactive.client.Draw(Renderer, clientRect);
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

                textures.toolWindow.v.dragBar.Draw(Renderer, dragRect);
                textures.toolWindow.v.client.Draw(Renderer, clientRect);
            }
            else
            {
                Rectangle rect = control.RenderBounds;
                Rectangle dragRect = rect;
                dragRect.Width = dragSize;
                Rectangle clientRect = rect;
                clientRect.X += dragSize;
                clientRect.Width -= dragSize;

                textures.toolWindow.h.dragBar.Draw(Renderer, dragRect);
                textures.toolWindow.h.client.Draw(Renderer, clientRect);
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
                textures.scroller.trackH.Draw(Renderer, control.RenderBounds);
            }
            else
            {
                textures.scroller.trackV.Draw(Renderer, control.RenderBounds);
            }
        }

        public override void DrawScrollBarBar(ControlBase control, bool depressed, bool hovered, bool horizontal)
        {
            if (!horizontal)
            {
                if (control.IsDisabled)
                {
                    textures.scroller.buttonVDisabled.Draw(Renderer, control.RenderBounds);

                    return;
                }

                if (depressed)
                {
                    textures.scroller.buttonVDown.Draw(Renderer, control.RenderBounds);

                    return;
                }

                if (hovered)
                {
                    textures.scroller.buttonVHover.Draw(Renderer, control.RenderBounds);

                    return;
                }

                textures.scroller.buttonVNormal.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (control.IsDisabled)
            {
                textures.scroller.buttonHDisabled.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (depressed)
            {
                textures.scroller.buttonHDown.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (hovered)
            {
                textures.scroller.buttonHHover.Draw(Renderer, control.RenderBounds);

                return;
            }

            textures.scroller.buttonHNormal.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawProgressBar(ControlBase control, bool horizontal, float progress)
        {
            Rectangle rect = control.RenderBounds;

            if (horizontal)
            {
                textures.progressBar.back.Draw(Renderer, rect);

                if (progress > 0)
                {
                    rect.Width = (int) (rect.Width * progress);

                    if (rect.Width >= 5.0f)
                    {
                        textures.progressBar.front.Draw(Renderer, rect);
                    }
                }
            }
            else
            {
                textures.progressBar.back.Draw(Renderer, rect);

                if (progress > 0)
                {
                    rect.Y = (int) (rect.Y + (rect.Height * (1 - progress)));
                    rect.Height = (int) (rect.Height * progress);

                    if (rect.Height >= 5.0f)
                    {
                        textures.progressBar.front.Draw(Renderer, rect);
                    }
                }
            }
        }

        public override void DrawListBox(ControlBase control)
        {
            textures.input.listBox.background.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawListBoxLine(ControlBase control, bool selected, bool even)
        {
            if (selected)
            {
                if (even)
                {
                    textures.input.listBox.evenLineSelected.Draw(Renderer, control.RenderBounds);

                    return;
                }

                textures.input.listBox.oddLineSelected.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (control.IsHovered)
            {
                textures.input.listBox.hovered.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (even)
            {
                textures.input.listBox.evenLine.Draw(Renderer, control.RenderBounds);

                return;
            }

            textures.input.listBox.oddLine.Draw(Renderer, control.RenderBounds);
        }

        public void DrawSliderNotchesH(Rectangle rect, int numNotches, float dist)
        {
            if (numNotches == 0)
            {
                return;
            }

            float iSpacing = rect.Width / (float) numNotches;

            for (var i = 0; i < numNotches + 1; i++)
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

            float iSpacing = rect.Height / (float) numNotches;

            for (var i = 0; i < numNotches + 1; i++)
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
                rect.X += (int) (barSize * 0.5);
                rect.Width -= barSize;
                rect.Y += (int) ((rect.Height * 0.5) - 1);
                rect.Height = 1;
                DrawSliderNotchesH(rect, numNotches, barSize * 0.5f);
                Renderer.DrawFilledRect(rect);

                return;
            }

            rect.Y += (int) (barSize * 0.5);
            rect.Height -= barSize;
            rect.X += (int) ((rect.Width * 0.5) - 1);
            rect.Width = 1;
            DrawSliderNotchesV(rect, numNotches, barSize * 0.4f);
            Renderer.DrawFilledRect(rect);
        }

        public override void DrawComboBox(ControlBase control, bool down, bool open)
        {
            if (control.IsDisabled)
            {
                textures.input.comboBox.disabled.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (down || open)
            {
                textures.input.comboBox.down.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (control.IsHovered)
            {
                textures.input.comboBox.down.Draw(Renderer, control.RenderBounds);

                return;
            }

            textures.input.comboBox.normal.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawKeyboardHighlight(ControlBase control, Rectangle rect, int offset)
        {
            rect.X += offset;
            rect.Y += offset;
            rect.Width -= offset * 2;
            rect.Height -= offset * 2;

            //draw the top and bottom
            var skip = true;

            for (var i = 0; i < rect.Width * 0.5; i++)
            {
                renderer.DrawColor = Color.Black;

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

            for (var i = 0; i < rect.Height * 0.5; i++)
            {
                Renderer.DrawColor = Color.Black;
                Renderer.DrawPixel(rect.X, rect.Y + (i * 2));
                Renderer.DrawPixel(rect.X + rect.Width - 1, rect.Y + (i * 2));
            }
        }

        public override void DrawToolTip(ControlBase control)
        {
            textures.tooltip.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawScrollButton(ControlBase control, ScrollBarButtonDirection direction, bool depressed,
            bool hovered, bool disabled)
        {
            var i = 0;

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
                textures.scroller.button.disabled[i].Draw(Renderer, control.RenderBounds);

                return;
            }

            if (depressed)
            {
                textures.scroller.button.down[i].Draw(Renderer, control.RenderBounds);

                return;
            }

            if (hovered)
            {
                textures.scroller.button.hover[i].Draw(Renderer, control.RenderBounds);

                return;
            }

            textures.scroller.button.normal[i].Draw(Renderer, control.RenderBounds);
        }

        public override void DrawComboBoxArrow(ControlBase control, bool hovered, bool down, bool open, bool disabled)
        {
            if (disabled)
            {
                textures.input.comboBox.buttonColors.disabled.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (down || open)
            {
                textures.input.comboBox.buttonColors.down.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (hovered)
            {
                textures.input.comboBox.buttonColors.hover.Draw(Renderer, control.RenderBounds);

                return;
            }

            textures.input.comboBox.buttonColors.normal.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawNumericUpDownButton(ControlBase control, bool depressed, bool up)
        {
            if (up)
            {
                if (control.IsDisabled)
                {
                    textures.input.upDown.up.disabled.Draw(Renderer, control.RenderBounds);

                    return;
                }

                if (depressed)
                {
                    textures.input.upDown.up.down.Draw(Renderer, control.RenderBounds);

                    return;
                }

                if (control.IsHovered)
                {
                    textures.input.upDown.up.hover.Draw(Renderer, control.RenderBounds);

                    return;
                }

                textures.input.upDown.up.normal.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (control.IsDisabled)
            {
                textures.input.upDown.down.disabled.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (depressed)
            {
                textures.input.upDown.down.down.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (control.IsHovered)
            {
                textures.input.upDown.down.hover.Draw(Renderer, control.RenderBounds);

                return;
            }

            textures.input.upDown.down.normal.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawStatusBar(ControlBase control)
        {
            textures.statusBar.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawTreeButton(ControlBase control, bool open)
        {
            Rectangle rect = control.RenderBounds;

            if (open)
            {
                textures.tree.minus.Draw(Renderer, rect);
            }
            else
            {
                textures.tree.plus.Draw(Renderer, rect);
            }
        }

        public override void DrawTreeControl(ControlBase control)
        {
            textures.tree.background.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawTreeNode(ControlBase ctrl, bool open, bool selected, int labelHeight, int labelWidth,
            int halfWay, int lastBranch, bool isRoot, int indent)
        {
            if (selected)
            {
                textures.selection.Draw(Renderer, new Rectangle(indent, y: 0, labelWidth, labelHeight));
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
                Renderer.DrawColor = colors.modalBackground;
            }
            else
            {
                Renderer.DrawColor = (Color) backgroundColor;
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
                textures.window.closeDisabled.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (depressed)
            {
                textures.window.closeDown.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (hovered)
            {
                textures.window.closeHover.Draw(Renderer, control.RenderBounds);

                return;
            }

            textures.window.close.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawSliderButton(ControlBase control, bool depressed, bool horizontal)
        {
            if (!horizontal)
            {
                if (control.IsDisabled)
                {
                    textures.input.slider.v.disabled.Draw(Renderer, control.RenderBounds);

                    return;
                }

                if (depressed)
                {
                    textures.input.slider.v.down.Draw(Renderer, control.RenderBounds);

                    return;
                }

                if (control.IsHovered)
                {
                    textures.input.slider.v.hover.Draw(Renderer, control.RenderBounds);

                    return;
                }

                textures.input.slider.v.normal.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (control.IsDisabled)
            {
                textures.input.slider.h.disabled.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (depressed)
            {
                textures.input.slider.h.down.Draw(Renderer, control.RenderBounds);

                return;
            }

            if (control.IsHovered)
            {
                textures.input.slider.h.hover.Draw(Renderer, control.RenderBounds);

                return;
            }

            textures.input.slider.h.normal.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawCategoryHolder(ControlBase control)
        {
            textures.categoryList.outer.Draw(Renderer, control.RenderBounds);
        }

        public override void DrawCategoryInner(ControlBase control, int headerHeight, bool collapsed)
        {
            if (collapsed)
            {
                textures.categoryList.header.Draw(Renderer, control.RenderBounds);
            }
            else
            {
                Rectangle rect = control.RenderBounds;
                Rectangle headerRect = rect;
                headerRect.Height = headerHeight;
                Rectangle clientRect = rect;
                clientRect.Y += headerHeight;
                clientRect.Height -= headerHeight;
                textures.categoryList.inner.header.Draw(Renderer, headerRect);
                textures.categoryList.inner.client.Draw(Renderer, clientRect);
            }
        }

        public override void DrawBorder(ControlBase control, BorderType border)
        {
            switch (border)
            {
                case BorderType.ToolTip:
                    textures.tooltip.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.StatusBar:
                    textures.statusBar.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.MenuStrip:
                    textures.menu.strip.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.Selection:
                    textures.selection.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.PanelNormal:
                    textures.panel.normal.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.PanelBright:
                    textures.panel.bright.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.PanelDark:
                    textures.panel.dark.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.PanelHighlight:
                    textures.panel.highlight.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.ListBox:
                    textures.input.listBox.background.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.TreeControl:
                    textures.tree.background.Draw(Renderer, control.RenderBounds);

                    break;
                case BorderType.CategoryList:
                    textures.categoryList.outer.Draw(Renderer, control.RenderBounds);

                    break;
            }
        }

        #endregion
    }
}
