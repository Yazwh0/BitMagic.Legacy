using BitMagic.Common;
using BitMagic.X16Emulator;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;

namespace X16E;

internal class EmulatorWindow
{
    private static GL? _gl;
    private static IWindow? _window;
    private static Shader? _shader;
    private static X16EImage[]? _images;


    private static GlObject[]? _layers;

    private static bool _requireUpdate = false;

    public static void Run(Emulator emulator)
    {
        var _emulator = emulator;
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
