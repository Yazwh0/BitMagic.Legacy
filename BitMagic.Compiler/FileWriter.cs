using BitMagic.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMagic.Compiler
{
    internal interface IWriter
    {
        void Add(byte toAdd, int address);
        void Add(byte[] toAdd, int address);
        void SetHeader(IEnumerable<byte> toAdd);
        NamedStream Write();
    }

    internal class FileWriter : IWriter
    {
        public string FileName { get; }
        public string SegmentName { get; }

        private byte[] _header;
        private List<byte> _data = new List<byte>(0x10000);
        private int _startAddress;

        public FileWriter(string segmentName, string fileName, int startAddress)
        {
            SegmentName = segmentName;
            FileName = fileName;
            _startAddress = startAddress;
            _header = Array.Empty<byte>();
        }

        public void Add(byte toAdd, int address)
        {
            var index = _startAddress - address;

            if (index < 0)
                throw new IndexOutOfRangeException();

            while (_data.Count < index)
            {
                _data.Add(0x00);
            }

            if (_data[index] != 0)
                throw new Exception("Overwrite detected!");

            _data[index] = toAdd;
        }

        public void Add(byte[] toAdd, int address)
        {
            var index = address - _startAddress;

            if (index < 0)
                throw new IndexOutOfRangeException();

            while (_data.Count < index + toAdd.Length)
            {
                _data.Add(0x00);
            }

            for(var i = 0; i < toAdd.Length; i++)
            {
                if (_data[index] != 0)
                    throw new Exception("Overwrite detected!");

                _data[index++] = toAdd[i];
            }
        }

        public void SetHeader(IEnumerable<byte> toAdd)
        {
            _header = toAdd.ToArray();
        }

        public NamedStream Write() => new (SegmentName, FileName, _header.Concat(_data).ToArray());        
    }
}
