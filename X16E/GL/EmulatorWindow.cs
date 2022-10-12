using BitMagic.Common;
using BitMagic.X16Emulator;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Diagnostics;

namespace X16E;

internal class EmulatorWindow
{
    private static GL? _gl;
    private static IWindow? _window;
    private static Shader? _shader;
    private static X16EImage[]? _images;


    private static GlObject[]? _layers;
    private static UInt32 _lastCount;
    private static long _lastTicks;
    private static double _speed = 0;
    private static double _fps = 0;
    private static Stopwatch _stopwatch = new Stopwatch();
    private static Emulator _emulator;

    private static bool _requireUpdate = false;

    public static void Run(Emulator emulator)
    {
        _emulator = emulator;
        _window = Window.Create(WindowOptions.Default);

        _images = new X16EImage[6];
        _images[0] = new X16EImage(_emulator, 0);
        _images[1] = new X16EImage(_emulator, 1);
        _images[2] = new X16EImage(_emulator, 2);
        _images[3] = new X16EImage(_emulator, 3);
        _images[4] = new X16EImage(_emulator, 4);
        _images[5] = new X16EImage(_emulator, 5);

        _window.Size = new Silk.NET.Maths.Vector2D<int> { X = 800, Y = 525 };
        _window.Title = "BitMagic! X16E";
        _window.WindowBorder = WindowBorder.Fixed;

        _window.Load += OnLoad;
        _window.Render += OnRender;
        _window.Closing += OnClose;

        _stopwatch.Start();
        _window.Run();
    }

    public static void SetRequireUpdate()
    {
        _requireUpdate = true;
    }

    private static unsafe void OnLoad()
    {
        _gl = GL.GetApi(_window);

        _layers = new GlObject[_images.Length];

        for(var i = 0; i < _images.Length; i++)
        {
            _layers[i] = new GlObject();
            _layers[i].OnLoad(_gl, _images[i], i / 10);
        }

        _shader = new Shader(_gl, @"shader.vert", @"shader.frag");
    }

    private static unsafe void OnRender(double deltaTime)
    {
        if (_gl == null) throw new ArgumentNullException(nameof(_gl));
        if (_shader == null) throw new ArgumentNullException(nameof(_shader));
        if (_layers == null) throw new ArgumentNullException(nameof(_layers));

        //_gl.Enable(EnableCap.DepthTest);
        //_gl.Enable(GLEnum.Blend);
       // _gl.BlendFunc(BlendingFactor.SrcColor, BlendingFactor.SrcColor);
        _gl.Clear(ClearBufferMask.ColorBufferBit);

        //_layers[0].OnRender(_gl, _shader, _requireUpdate);

        foreach (var i in _layers)
        {
            i.OnRender(_gl, _shader, true);
        }
        _requireUpdate = false;
        var thisTicks = _stopwatch.ElapsedMilliseconds;
        if (thisTicks - _lastTicks > 1000)
        {
            var thisCount = _emulator.Vera.Frame_Count;

            if (thisCount == _lastCount) // no frames this second?
            {
                _speed = 0;
                _fps = 0;
            }
            else
            {
                var tickDelta = thisTicks - _lastTicks;
                _fps = (thisCount - _lastCount) / (tickDelta / 1000.0);
                _speed = _fps / 59.523809;
            }
            _lastCount = thisCount;
            _lastTicks = thisTicks;
        }

        _window.Title = $"BitMagic! X16E [{_speed:0.00%} \\ {_fps:0.0} fps \\ {_speed * 8.0:0}Mhz]";
    }

    private static void OnClose()
    {
        _gl?.Dispose();
        _shader?.Dispose();
        if (_layers != null)
        {
            foreach(var i in _layers)
            {
                i.Dispose();
            }
        }
    }
}
