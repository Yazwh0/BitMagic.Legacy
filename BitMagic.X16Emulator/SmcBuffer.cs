using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BitMagic.X16Emulator.Emulator;
using Silk.NET.Input;

namespace BitMagic.X16Emulator
{
    public class SmcBuffer
    {
        private readonly Emulator _emulator;

        public SmcBuffer(Emulator emulator)
        {
            _emulator = emulator;
        }

        public void KeyDown(Key key) => AddKey(true, KeyToPs2ScanCode(key));

        public void KeyUp(Key key) => AddKey(false, KeyToPs2ScanCode(key));

        public void AddKey(bool keyDown, uint scancode)
        {
            if (scancode == 0xff && keyDown)
            {   // sequence from the x16 emulator
                PushByte(0xe1);
                PushByte(0x14);
                PushByte(0x77);
                PushByte(0xe1);
                PushByte(0xf0);
                PushByte(0x14);
                PushByte(0xf0);
                PushByte(0x77);
                return;
            }

            if ((scancode & EXTENDED_FLAG) != 0)
                PushByte(0xe0);
            if (!keyDown)
                PushByte(0xf0);

            PushByte((byte)(scancode & 0xff));
        }

        public void PushByte(byte value)
        {
            var next = (_emulator.Keyboard_WritePosition + 1) & (Emulator.SmcKeyboardBufferSize - 1);
            if (next != _emulator.Keyboard_ReadPosition) {
                _emulator.KeyboardBuffer[(int)_emulator.Keyboard_WritePosition] = value;
                _emulator.Keyboard_WritePosition = next;
                //Console.WriteLine($"Keyboard added ${value:X2}");
            }
            else
            {
                Console.ForegroundColor= ConsoleColor.Red;
                Console.WriteLine($"Warning: keyboard buffer full! Cannot add ${value:X2}");
                Console.ResetColor();
            }
        }

        private const uint EXTENDED_FLAG = 0x100;
        public uint KeyToPs2ScanCode(Key key) => key switch
        {
            // SDL_SCANCODE_CLEAR
            Key.GraveAccent => 0x0e,
            Key.Backspace => 0x66,
            Key.Tab => 0x0d,
            Key.Enter => 0x5a,
            Key.Pause => 0x00,
            Key.Escape => 0xff, // Esc is 0x76, but we send break
            Key.Space => 0x29,
            Key.Apostrophe => 0x52,
            Key.Comma => 0x41,
            Key.Minus => 0x4e,
            Key.Period => 0x49,
            Key.Slash => 0x4a,
            Key.Number0 => 0x45,
            Key.Number1 => 0x16,
            Key.Number2 => 0x1e,
            Key.Number3 => 0x26,
            Key.Number4 => 0x25,
            Key.Number5 => 0x2e,
            Key.Number6 => 0x36,
            Key.Number7 => 0x3d,
            Key.Number8 => 0x3e,
            Key.Number9 => 0x46,
            Key.Semicolon => 0x4c,
            Key.Equal => 0x55,
            Key.LeftBracket => 0x54,
            Key.BackSlash => 0x5d,
            Key.RightBracket => 0x5b,
            Key.A => 0x1c,
            Key.B => 0x32,
            Key.C => 0x21,
            Key.D => 0x23,
            Key.E => 0x24,
            Key.F => 0x2b,
            Key.G => 0x34,
            Key.H => 0x33,
            Key.I => 0x43,
            Key.J => 0x3B,
            Key.K => 0x42,
            Key.L => 0x4B,
            Key.M => 0x3A,
            Key.N => 0x31,
            Key.O => 0x44,
            Key.P => 0x4D,
            Key.Q => 0x15,
            Key.R => 0x2D,
            Key.S => 0x1B,
            Key.T => 0x2C,
            Key.U => 0x3C,
            Key.V => 0x2A,
            Key.W => 0x1D,
            Key.X => 0x22,
            Key.Y => 0x35,
            Key.Z => 0x1A,
            Key.Delete => 0x71 | EXTENDED_FLAG,
            Key.Up => 0x75 | EXTENDED_FLAG,
            Key.Down => 0x72 | EXTENDED_FLAG,
            Key.Right => 0x74 | EXTENDED_FLAG,
            Key.Left => 0x6b | EXTENDED_FLAG,
            Key.Insert => 0x70 | EXTENDED_FLAG,
            Key.Home => 0x6c | EXTENDED_FLAG,
            Key.End => 0x69 | EXTENDED_FLAG,
            Key.PageUp => 0x7d | EXTENDED_FLAG,
            Key.PageDown => 0x7a | EXTENDED_FLAG,
            Key.F1 => 0x05,
            Key.F2 => 0x06,
            Key.F3 => 0x04,
            Key.F4 => 0x0c,
            Key.F5 => 0x03,
            Key.F6 => 0x0b,
            Key.F7 => 0x83,
            Key.F8 => 0x0a,
            Key.F9 => 0x01,
            Key.F10 => 0x09,
            Key.F11 => 0x78,
            Key.F12 => 0x07,
            Key.ShiftRight => 0x59,
            Key.ShiftLeft => 0x12,
            Key.CapsLock => 0x58,
            Key.ControlLeft => 0x14,
            Key.ControlRight => 0x14 | EXTENDED_FLAG,
            Key.AltLeft => 0x11,
            Key.AltRight => 0x11 | EXTENDED_FLAG,
            //SDL_SCANCODE_LGUI // Windows/Command
            //SDL_SCANCODE_RGUI => 0x5b | EXTENDED_FLAG,
            Key.Menu => 0x2f | EXTENDED_FLAG,
            //SDL_SCANCODE_NONUSBACKSLASH => 0x61,
            Key.KeypadEnter => 0x5a | EXTENDED_FLAG,
            Key.Keypad0 => 0x70,
            Key.Keypad1 => 0x69,
            Key.Keypad2 => 0x72,
            Key.Keypad3 => 0x7a,
            Key.Keypad4 => 0x6b,
            Key.Keypad5 => 0x73,
            Key.Keypad6 => 0x74,
            Key.Keypad7 => 0x6c,
            Key.Keypad8 => 0x75,
            Key.Keypad9 => 0x7d,
            Key.KeypadDecimal => 0x71,
            Key.KeypadAdd => 0x79,
            Key.KeypadSubtract => 0x7b,
            Key.KeypadMultiply => 0x7c,
            Key.KeypadDivide => 0x4a | EXTENDED_FLAG,
            _ => 0
        };


        private bool _running = true;

        public unsafe void RunI2cCaptuer(ref CpuState state)
        {
            return;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"SDA\tSCL\tmode");
            uint lastPosition = state.I2cPosition;
            var buffer = new Span<byte>((void*)state.I2cBufferPtr, 1024);
            while (_running)
            {
                var thisPosition = state.I2cPosition;
                if (thisPosition != lastPosition)
                {
                    while(thisPosition != lastPosition)
                    {
                        var val = buffer[(int)lastPosition] & 0x03;
                        sb.Append($"{(val & 0x01) + 1}\t{(val & 0x02) >> 1}\t");
                        //sb.AppendLine($"SDA: {val & 0x01}\tSCL: {(val & 0x02) >> 1}");
                        lastPosition++;
                        if (lastPosition >= 1024) lastPosition = 0;

                        val = buffer[(int)lastPosition];
                        sb.AppendLine($"{val}");
                        //sb.AppendLine($"SDA: {val & 0x01}\tSCL: {(val & 0x02) >> 1}");
                        lastPosition++;
                        if (lastPosition >= 1024) lastPosition = 0;

                    }
                }
                else
                    Thread.Sleep(1);
            }

            File.WriteAllText(@"c:\documents\Source\i2c.txt", sb.ToString());
        }

        public void Stop()
        {
            _running = false;
        }
    }
}
