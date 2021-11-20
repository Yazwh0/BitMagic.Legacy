using BitMagic.Common;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Emulator.Gl
{
    internal class EmulatorWindow
    {
        private static GL? _gl;
        private static IWindow? _window;
        private static Texture? _texture;
        private static Shader? _shader;

        private static BufferObject<float>? _vbo;
        private static BufferObject<uint>? _ebo;
        private static VertexArrayObject<float, uint>? _vao;

        private static IDisplay _display;

        // OpenGL has image origin in the bottom-left corner.
        private static readonly float[] Vertices =
{
            //X    Y      Z     U   V
             1.0f,  1.0f, 0.0f, 1f, 0f,
             1.0f, -1.0f, 0.0f, 1f, 1f,
            -1.0f, -1.0f, 0.0f, 0f, 1f,
            -1.0f,  1.0f, 0.0f, 0f, 0f // z was .5
        };

        private static readonly uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        public void Run(IDisplay display)
        {
            _display = display;
            _window = Window.Create(WindowOptions.Default);

            _window.Size = new Silk.NET.Maths.Vector2D<int> { X = 640 * 2, Y = 480 * 2 };
            _window.Title = "BitMagic!";
            _window.WindowBorder = WindowBorder.Fixed;

            _window.Load += OnLoad;
            _window.Render += OnRender;
            _window.Closing += OnClose;

            _window.Run();
        }

        private static unsafe void OnLoad()
        {
            if (_window == null) throw new ArgumentNullException(nameof(_window));

            _gl = GL.GetApi(_window);
            //_gl.Viewport(_window.Size);

            _ebo = new BufferObject<uint>(_gl, Indices, BufferTargetARB.ElementArrayBuffer);
            _vbo = new BufferObject<float>(_gl, Vertices, BufferTargetARB.ArrayBuffer);
            _vao = new VertexArrayObject<float, uint>(_gl, _vbo, _ebo);

            _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
            _vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);

            _shader = new Shader(_gl, @"shader\shader.vert", @"shader\shader.frag");

            _texture = new Texture(_gl, _display.Displays[0]);
        }

        private static unsafe void OnRender(double deltaTime)
        {
            if (_gl == null) throw new ArgumentNullException(nameof(_gl));
            if (_texture == null) throw new ArgumentNullException(nameof(_texture));
            if (_shader == null) throw new ArgumentNullException(nameof(_shader));
            if (_vao == null) throw new ArgumentNullException(nameof(_vao));

            //_gl.Enable(EnableCap.DepthTest);
            //_gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

            //_gl.ClearColor(0, 0, .1f, 1);
            _gl.Clear(ClearBufferMask.ColorBufferBit);

            _vao.Bind();
            _shader.Use();
            _texture.Bind(TextureUnit.Texture0);
            _shader.SetUniform("uTexture0", 0);

            _gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
        }

        private static void OnClose()
        {
            _gl?.Dispose();
            _shader?.Dispose();
            _texture?.Dispose();
        }
    }
}
