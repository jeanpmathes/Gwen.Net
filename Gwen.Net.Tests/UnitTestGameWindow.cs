using System;
using System.Linq;
using Collections.Generic;
using Gwen.Net.Control;
using Gwen.Net.OpenTk;
using Gwen.Net.OpenTk.Input;
using Gwen.Net.OpenTk.Platform;
using Gwen.Net.Skin;
using Gwen.Net.Tests.Components;
using OpenToolkit.Graphics.OpenGL;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Desktop;

namespace Gwen.Net.Tests
{
    public class UnitTestGameWindow : GameWindow
    {
        private const int MaxFrameSampleSize = 10000;

        private OpenTkInputTranslator input;
        private OpenTKRendererBase renderer;
        private SkinBase skin;
        private Canvas canvas;
        private UnitTestHarnessControls unitTestControls;

        private readonly CircularBuffer<double> updateFrameTimes;
        private readonly CircularBuffer<double> renderFrameTimes;

        private bool m_AltDown = false;

        public UnitTestGameWindow()
            : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            UpdateFrequency = 30;
            KeyDown += Keyboard_KeyDown;
            KeyUp += Keyboard_KeyUp;

            MouseDown += Mouse_ButtonDown;
            MouseUp += Mouse_ButtonUp;
            MouseMove += Mouse_Move;
            MouseWheel += Mouse_Wheel;

            updateFrameTimes = new CircularBuffer<double>(MaxFrameSampleSize);
            renderFrameTimes = new CircularBuffer<double>(MaxFrameSampleSize);
        }

        protected override void Dispose(bool disposing)
        {
            if (canvas != null)
            {
                canvas.Dispose();
                canvas = null;
            }
            if (skin != null)
            {
                skin.Dispose();
                skin = null;
            }
            if (renderer != null)
            {
                renderer.Dispose();
                renderer = null;
            }
            base.Dispose(disposing);
        }

        private void Keyboard_KeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == OpenToolkit.Windowing.Common.Input.Key.Escape)
                Close();
            else if (e.Key == OpenToolkit.Windowing.Common.Input.Key.AltLeft)
                m_AltDown = true;
            else if (m_AltDown && e.Key == OpenToolkit.Windowing.Common.Input.Key.Enter)
                if (WindowState == WindowState.Fullscreen)
                    WindowState = WindowState.Normal;
                else
                    WindowState = WindowState.Fullscreen;

            input.ProcessKeyDown(e);
        }

        private void Keyboard_KeyUp(KeyboardKeyEventArgs e)
        {
            m_AltDown = false;
            input.ProcessKeyUp(e);
        }

        private void Mouse_ButtonDown(MouseButtonEventArgs args)
        {
            input.ProcessMouseDown(args);
        }

        private void Mouse_ButtonUp(MouseButtonEventArgs args)
        {
            input.ProcessMouseDown(args);
        }

        private void Mouse_Move(MouseMoveEventArgs args)
        {
            input.ProcessMouseMove(args);
        }

        private void Mouse_Wheel(MouseWheelEventArgs args)
        {
            input.ProcessMouseWheel(args);
        }

        protected override void OnLoad()
        {
            GL.ClearColor(Color4.MidnightBlue);

            Platform.Platform.Init(new NetCorePlatform());

            //m_Renderer = new OpenTKGL10Renderer();
            //m_Renderer = new OpenTKGL20Renderer();
            renderer = new OpenTKGL40Renderer();

            skin = new Skin.TexturedBase(renderer, "DefaultSkin2.png");

            skin.DefaultFont = new Font(renderer, "Calibri", 11);
            canvas = new Canvas(skin);
            input = new OpenTkInputTranslator(canvas);

            canvas.SetSize(Size.X, Size.Y);
            canvas.ShouldDrawBackground = true;
            canvas.BackgroundColor = skin.Colors.ModalBackground;

            unitTestControls = new UnitTestHarnessControls(canvas);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            renderer.Resize(e.Width, e.Height);
            canvas.SetSize(e.Width, e.Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (UpdateFrequency == 0)
            {
                updateFrameTimes.Put(e.Time);
                if (updateFrameTimes.Sum(t => t) >= 1)
                {
                    int frames = updateFrameTimes.Count();
                    updateFrameTimes.Clear();
                    unitTestControls.UpdateFps = frames;
                }
            }
            else
            {
                unitTestControls.UpdateFps = UpdateFrequency;
            }
        }

        /// <summary>
        /// Add your game rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            canvas.RenderCanvas();
            SwapBuffers();

            if (RenderFrequency == 0)
            {
                renderFrameTimes.Put(e.Time);
                if (renderFrameTimes.Sum(t => t) >= 1)
                {
                    int frames = renderFrameTimes.Count();
                    renderFrameTimes.Clear();
                    unitTestControls.RenderFps = frames;
                }
            }
            else
            {
                unitTestControls.RenderFps = RenderFrequency;
            }
        }

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (UnitTestGameWindow window = new UnitTestGameWindow())
            {
                window.Title = "Gwen.net OpenTK Unit Test";
                window.VSync = VSyncMode.Off; // to measure performance
                window.Run();
            }
        }
    }
}
