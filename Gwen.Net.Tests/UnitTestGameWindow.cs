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
    /// <summary>
    /// Demonstrates the GameWindow class.
    /// </summary>
    public class UnitTestGameWindow : GameWindow
	{
		const int MaxFrameSampleSize = 10000;

	    private OpenTkInputTranslator m_Input;
		private OpenTKRendererBase m_Renderer;
		private SkinBase m_Skin;
		private Canvas m_Canvas;
		private UnitTestHarnessControls m_UnitTest;

		private readonly CircularBuffer<double> updateFrameTimes;
		private readonly CircularBuffer<double> renderFrameTimes;

		private bool m_AltDown = false;

		public UnitTestGameWindow()
			:base(GameWindowSettings.Default, NativeWindowSettings.Default)
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
			if (m_Canvas != null)
			{
				m_Canvas.Dispose();
				m_Canvas = null;
			}
			if (m_Skin != null)
			{
				m_Skin.Dispose();
				m_Skin = null;
			}
			if (m_Renderer != null)
			{
				m_Renderer.Dispose();
				m_Renderer = null;
			}
			base.Dispose(disposing);
        }

		void Keyboard_KeyDown(KeyboardKeyEventArgs e)
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

			m_Input.ProcessKeyDown(e);
		}

		void Keyboard_KeyUp(KeyboardKeyEventArgs e)
		{
			m_AltDown = false;
			m_Input.ProcessKeyUp(e);
		}

		void Mouse_ButtonDown(MouseButtonEventArgs args)
		{
			m_Input.ProcessMouseDown(args);
		}

		void Mouse_ButtonUp(MouseButtonEventArgs args)
		{
			m_Input.ProcessMouseDown(args);
		}

		void Mouse_Move(MouseMoveEventArgs args)
		{
			m_Input.ProcessMouseMove(args);
		}

		void Mouse_Wheel(MouseWheelEventArgs args)
		{
			m_Input.ProcessMouseWheel(args);
		}

        protected override void OnLoad()
		{
			GL.ClearColor(Color4.MidnightBlue);

			Platform.Platform.Init(new NetCorePlatform());

            //m_Renderer = new OpenTKGL10Renderer();
            //m_Renderer = new OpenTKGL20Renderer();
            m_Renderer = new OpenTKGL40Renderer();

            m_Skin = new Skin.TexturedBase(m_Renderer, "DefaultSkin2.png");
            
			m_Skin.DefaultFont = new Font(m_Renderer, "Calibri", 11);
			m_Canvas = new Canvas(m_Skin);
			m_Input = new OpenTkInputTranslator(m_Canvas);

			m_Canvas.SetSize(Size.X, Size.Y);
			m_Canvas.ShouldDrawBackground = true;
			m_Canvas.BackgroundColor = m_Skin.Colors.ModalBackground;

			m_UnitTest = new UnitTestHarnessControls(m_Canvas);
		}

        protected override void OnResize(ResizeEventArgs e)
        {
			m_Renderer.Resize(e.Width, e.Height);
			m_Canvas.SetSize(e.Width, e.Height);
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			if(UpdateFrequency == 0)
            {
				updateFrameTimes.Put(e.Time);
				if (updateFrameTimes.Sum(t => t) >= 1)
				{
					int frames = updateFrameTimes.Count();
					updateFrameTimes.Clear();
					m_UnitTest.UpdateFps = frames;
				}
			}
            else
            {
				m_UnitTest.UpdateFps = UpdateFrequency;
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
            m_Canvas.RenderCanvas();
            SwapBuffers();

			if (RenderFrequency == 0)
			{
				renderFrameTimes.Put(e.Time);
				if (renderFrameTimes.Sum(t => t) >= 1)
				{
					int frames = renderFrameTimes.Count();
					renderFrameTimes.Clear();
					m_UnitTest.RenderFps = frames;
				}
			}
			else
			{
				m_UnitTest.RenderFps = RenderFrequency;
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
