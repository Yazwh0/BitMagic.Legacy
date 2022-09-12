using BitMagic.Common;
using Silk.NET.OpenGL;
using System;
using System.Runtime.InteropServices;

namespace X16E;

public class Texture : IDisposable
{
    private uint _handle;
    private GL? _gl;
    private readonly X16EImage _image;
    private const uint imageWidth = 800;
    private const uint imageHeight = 525;

    public unsafe Texture(GL gl, X16EImage image)
    {
        _image = image;
        _gl = gl;

        /*            Span<Rgba32> pixelRowSpan = _image.GetPixelRowSpan(0);
                    for (int x = 0; x < _image.Width; x++)
                    {
                        pixelRowSpan[x] = new Rgba32(255, 255, 255, 255);
                    }

                    for(var i = 0; i < _image.Height; i++)
                    {
                        pixelRowSpan = _image.GetPixelRowSpan(i);
                        pixelRowSpan[0] = new Rgba32(255, 255, 255, 255);
                        pixelRowSpan[_image.Width - 1] = new Rgba32(255, 255, 255, 255);
                    }

                    pixelRowSpan = _image.GetPixelRowSpan(_image.Height - 1);
                    for (int x = 0; x < _image.Width; x++)
                    {
                        pixelRowSpan[x] = new Rgba32(255, 255, 255, 255);
                    }*/


        fixed (void* data = &MemoryMarshal.GetReference(_image.Pixels))
        {
            Load(gl, data, imageWidth, imageHeight);
        }
    }

    private unsafe void Load(GL gl, void* data, uint width, uint height)
    {
        _gl = gl;

        _handle = _gl.GenTexture();
        Bind();

        _gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);
        _gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    public unsafe void Update()
    {
        if (_gl == null) throw new ArgumentNullException(nameof(_gl));

        fixed (void* data = &MemoryMarshal.GetReference(_image.Pixels))
        {
            _gl.DeleteTexture(_handle);
            Load(_gl, data, imageWidth, imageHeight);
        }
    }

    public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
    {
        if (_gl == null) throw new ArgumentNullException(nameof(_gl));

        _gl.ActiveTexture(textureSlot);
        _gl.BindTexture(TextureTarget.Texture2D, _handle);
    }

    public void Dispose()
    {
        _gl?.DeleteTexture(_handle);
    }
}