using BitMagic.Common;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Emulator.Gl
{
    internal class GlObject : IDisposable
    {
        private Texture? _texture;
        private BufferObject<float>? _vbo;
        private BufferObject<uint>? _ebo;
        private VertexArrayObject<float, uint>? _vao;

        // OpenGL has image origin in the bottom-left corner.
        private static readonly float[] Vertices =
{
            //X    Y      Z     U   V
             1.0f,  1.0f, 0.0f, 1f, 0f,
             1.0f, -1.0f, 0.0f, 1f, 1f,
            -1.0f, -1.0f, 0.0f, 0f, 1f,
            -1.0f,  1.0f, 0.0f, 0f, 0f
        };

        private static readonly uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        public void OnLoad(GL gl, BitImage image, float zpos)
        {
            Vertices[2] = zpos;
            Vertices[7] = zpos;
            Vertices[12] = zpos;
            Vertices[17] = zpos;

            _ebo = new BufferObject<uint>(gl, Indices, BufferTargetARB.ElementArrayBuffer);
            _vbo = new BufferObject<float>(gl, Vertices, BufferTargetARB.ArrayBuffer);
            _vao = new VertexArrayObject<float, uint>(gl, _vbo, _ebo);

            _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
            _vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);

            _texture = new Texture(gl, image);
        }

        public unsafe void OnRender(GL gl, Shader shader, bool requireUpdate)
        {
            if (_texture == null) throw new ArgumentNullException(nameof(_texture));
            if (_vao == null) throw new ArgumentNullException(nameof(_vao));

            if (requireUpdate)
                _texture.Update();

            gl.Enable(GLEnum.Blend);
            gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _vao.Bind();
            shader.Use();
            _texture.Bind(TextureUnit.Texture0);
            shader.SetUniform("uTexture0", 0);

            gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
        }

        public void Dispose()
        {
            _texture?.Dispose();
        }
    }
}
