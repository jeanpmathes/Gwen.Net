using System;
using System.Linq;
using System.Runtime.Versioning;
using Collections.Generic;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Controls.Templates;
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

    private readonly ResourceRegistry registry;
    private readonly IGwenGui gui;

    private readonly CircularBuffer<Double> renderFrameTimes;
    private readonly CircularBuffer<Double> updateFrameTimes;

    private UnitTestHarness? harness;
    
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

        registry = new ResourceRegistry();

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
        registry.Dispose();

        base.Dispose(disposing);
    }

    protected override void OnLoad()
    {
        GL.ClearColor(Color4.White);

        gui.Load();

        harness = new UnitTestHarness();

        gui.Root?.Child = new ContentControl<UnitTestHarness>
        {
            Content = {Value = harness},
            ContentTemplate = {Value = ContentTemplate.Create<UnitTestHarness>(UnitTestHarnessView.Create)}
        };
        
        gui.Root?.SetDebugOutlines(false);
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
            harness?.UpdateFps.SetValue(frames);
        }
        else
        {
            harness?.UpdateFps.SetValue(UpdateFrequency);
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
            harness?.RenderFps.SetValue(frames);
        }
        else
        {
            harness?.RenderFps.SetValue(UpdateFrequency);
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
