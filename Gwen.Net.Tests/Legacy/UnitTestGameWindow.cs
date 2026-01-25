using System;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using Collections.Generic;
using Gwen.Net.OpenTk.Legacy;
using Gwen.Net.Tests.Components;
using Gwen.Net.Tests.Components.Legacy;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Boolean = System.Boolean;

[assembly: SupportedOSPlatform("windows")]

namespace Gwen.Net.Tests.Legacy
{
    public class UnitTestGameWindow : GameWindow
    {
        private const Int32 MaxFrameSampleSize = 10000;
        private readonly IGwenGui gui;
        private readonly CircularBuffer<Double> renderFrameTimes;

        private readonly CircularBuffer<Double> updateFrameTimes;

        private UnitTestHarnessControls unitTestControls;

        public UnitTestGameWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            UpdateFrequency = 30;

            gui = GwenGuiFactory.CreateFromGame(
                this,
                GwenGuiSettings.Default.From(
                    settings =>
                    {
                        //Have the skin come from somewhere else.
                        settings.SkinFile = new FileInfo("Legacy/DefaultSkin2.png");
                    }));

            updateFrameTimes = new CircularBuffer<Double>(MaxFrameSampleSize);
            renderFrameTimes = new CircularBuffer<Double>(MaxFrameSampleSize);
        }

        protected override void Dispose(Boolean disposing)
        {
            gui.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnLoad()
        {
            GL.ClearColor(Color4.MidnightBlue);
            gui.Load();
            unitTestControls = new UnitTestHarnessControls(gui.Root);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            gui.Resize(e.Size);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (UpdateFrequency == 0)
            {
                updateFrameTimes.Put(args.Time);

                if (updateFrameTimes.Sum(t => t) >= 1)
                {
                    Int32 frames = updateFrameTimes.Count();
                    updateFrameTimes.Clear();
                    unitTestControls.UpdateFps = frames;
                }
            }
            else
            {
                unitTestControls.UpdateFps = UpdateFrequency;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            gui.Render();
            SwapBuffers();

            if (UpdateFrequency == 0)
            {
                renderFrameTimes.Put(args.Time);

                if (renderFrameTimes.Sum(t => t) >= 1)
                {
                    Int32 frames = renderFrameTimes.Count();
                    renderFrameTimes.Clear();
                    unitTestControls.RenderFps = frames;
                }
            }
            else
            {
                unitTestControls.RenderFps = UpdateFrequency;
            }
        }

        [STAThread]
        public static void Main()
        {
            var gameWindowSettings = GameWindowSettings.Default;
            var nativeWindowSettings = NativeWindowSettings.Default;

            nativeWindowSettings.Profile = ContextProfile.Core;

            using UnitTestGameWindow window = new(gameWindowSettings, nativeWindowSettings);

            window.Title = "Gwen.net OpenTK Unit Test";
            window.VSync = VSyncMode.Off; // to measure performance

            window.Run();
        }
    }
}
