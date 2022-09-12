using BitMagic.Common;
using BitMagic.X16Emulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X16E;

public class X16EImage
{
    private readonly Emulator _emulator;
    private readonly int _offset;
    public Span<PixelRgba> Pixels => _emulator.Display.Slice(_offset * 800 * 525, 800 * 525); // not *4 as its a struct

    public X16EImage(Emulator emulator, int offset)
    {
        _emulator = emulator;
        _offset = offset;
    }
}
