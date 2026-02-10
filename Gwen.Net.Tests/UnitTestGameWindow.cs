using System;
using System.Linq;
using System.Runtime.Versioning;
using Collections.Generic;
using Gwen.Net.New.Resources;
using Gwen.Net.New.Themes;
using Gwen.Net.OpenTk.New;
using Gwen.Net.Tests.Components.New;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Boolean = System.Boolean;

[assembly: SupportedOSPlatform("windows")]

namespace Gwen.Net.Tests;

public class UnitTestGameWindow : GameWindow
{
    private const Int32 MaxFrameSampleSize = 10000;

    private readonly IGwenGui gui;

    private readonly CircularBuffer<Double> renderFrameTimes;
    private readonly CircularBuffer<Double> updateFrameTimes;

    private UnitTestHarnessControls? unitTestControls;
    
    public enum Theme
    {
        Default,
        Light,
        Dark,
    }

    private UnitTestGameWindow(Theme theme, GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        UpdateFrequency = 30;

        ResourceRegistry registry = new();

        switch (theme)
        {
            case Theme.Light:
                registry.AddBundle<ClassicLight>();
                break;
            
            case Theme.Dark:
                registry.AddBundle<ClassicDark>();
                break;
        }
        
        gui = GwenGuiFactory.CreateFromGame(this, registry);

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

        unitTestControls = new UnitTestHarnessControls();
        gui.Root?.Child = unitTestControls;
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

            if (updateFrameTimes.Sum(t => t) < 1) return;

            Int32 frames = updateFrameTimes.Count();
            updateFrameTimes.Clear();
            unitTestControls?.UpdateFps = frames;
        }
        else
        {
            unitTestControls?.UpdateFps = UpdateFrequency;
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

            if (renderFrameTimes.Sum(t => t) < 1) return;

            Int32 frames = renderFrameTimes.Count();
            renderFrameTimes.Clear();
            unitTestControls?.RenderFps = frames;
        }
        else
        {
            unitTestControls?.RenderFps = UpdateFrequency;
        }
    }

    [STAThread]
    public static void Main(String[] args)
    {
        var gameWindowSettings = GameWindowSettings.Default;
        var nativeWindowSettings = NativeWindowSettings.Default;
        
        nativeWindowSettings.Profile = ContextProfile.Core;
        
        Vector2i position = new(x: 0, y: 0);
        
        if (args.Contains("-p1"))
            position = new Vector2i(x: 0, y: 0);
        else if (args.Contains("-p2"))
            position = new Vector2i(x: 960, y: 0);
        else if (args.Contains("-p3"))
            position = new Vector2i(x: 0, y: 540);
        else if (args.Contains("-p4"))
            position = new Vector2i(x: 960, y: 540);
        
        nativeWindowSettings.Location = position;
        nativeWindowSettings.ClientSize = new Vector2i(960 - 0, 540 - 32);
        
        var theme = Theme.Default;
        
        if (args.Contains("--light"))
            theme = Theme.Light;
        else if (args.Contains("--dark"))
            theme = Theme.Dark;

        using UnitTestGameWindow window = new(theme, gameWindowSettings, nativeWindowSettings);

        window.Title = $"Gwen.net OpenTK Unit Test [{String.Join(" ", args)}]";
        window.VSync = VSyncMode.Off;

        window.Run();
    }
}
