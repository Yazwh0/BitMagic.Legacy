using BitMagic.Common;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;

namespace BitMagic.Emulator.Gl
{
    internal class EmulatorWindow
    {
        private static GL? _gl;
        private static IWindow? _window;
        private static Shader? _shader;

        private static IDisplay? _display;

        private static GlObject[]? _layers;

        private static bool _requireUpdate = false;

        public static void Run(IDisplay display)
        {
            _display = display;
            _window = Window.Create(WindowOptions.Default);

            _window.Size = new Silk.NET.Maths.Vector2D<int> { X = 640, Y = 480 };
            _window.Title = "BitMagic!";
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
            if (_window == null) throw new ArgumentNullException(nameof(_window));
            if (_display == null) throw new ArgumentNullException(nameof(_display));

            _gl = GL.GetApi(_window);

            _layers = new GlObject[_display.Displays.Length];

            for(var i = 0; i < 1; i++)// _display.Displays.Length; i++)
            {
                _layers[i] = new GlObject();
                _layers[i].OnLoad(_gl, _display.Displays[i], i / 10);
            }

            _shader = new Shader(_gl, @"shader\shader.vert", @"shader\shader.frag");
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

            _layers[0].OnRender(_gl, _shader, _requireUpdate);

            //foreach (var i in _layers)
            //{
            //    i.OnRender(_gl, _shader, _requireUpdate);
            //}
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
}
