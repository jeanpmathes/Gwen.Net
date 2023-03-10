﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using Collections.Generic;
using Gwen.Net.OpenTk;
using Gwen.Net.Tests.Components;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

[assembly: SupportedOSPlatform("windows")]

namespace Gwen.Net.Tests
{
    public class UnitTestGameWindow : GameWindow
    {
        private const int MaxFrameSampleSize = 10000;
        private readonly IGwenGui gui;
        private readonly CircularBuffer<double> renderFrameTimes;

        private readonly CircularBuffer<double> updateFrameTimes;

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
                        settings.SkinFile = new FileInfo("DefaultSkin2.png");
                    }));

            updateFrameTimes = new CircularBuffer<double>(MaxFrameSampleSize);
            renderFrameTimes = new CircularBuffer<double>(MaxFrameSampleSize);
        }

        protected override void Dispose(bool disposing)
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

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            gui.Render();
            SwapBuffers();

            if (RenderFrequency == 0)
            {
                renderFrameTimes.Put(args.Time);

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

        [STAThread]
        public static void Main()
        {
            var gameWindowSettings = GameWindowSettings.Default;
            var nativeWindowSettings = NativeWindowSettings.Default;

            nativeWindowSettings.Profile = ContextProfile.Core;

            using UnitTestGameWindow window = new(gameWindowSettings, nativeWindowSettings)
            {
                Title = "Gwen.net OpenTK Unit Test", VSync = VSyncMode.Off // to measure performance
            };

            window.Run();
        }
    }
}
