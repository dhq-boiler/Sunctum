

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sunctum.UI.Core
{
    internal class StringCollectionReader
    {
        private List<string> _lines;
        private int _index = 0;

        public int CurrentIndex { get { return _index; } }

        public StringCollectionReader(string str)
        {
            _lines = new List<string>(str.Split('\r', '\n'));
            _lines.RemoveAll(l => string.IsNullOrEmpty(l));
        }

        public string ReadLine()
        {
            if (_index == _lines.Count)
            {
                return null;
            }
            return _lines[_index++];
        }

        public string Peek()
        {
            return _lines[_index];
        }

        public bool CanRead
        {
            get { return _lines.Count() > _index; }
        }

        public void MoveToNextLine()
        {
            _index++;
        }

        public void Seek(int offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    if (offset < 0)
                    {
                        throw new ArgumentOutOfRangeException("offset < 0");
                    }
                    _index = offset;
                    break;
                case SeekOrigin.Current:
                    if (_index + offset < 0)
                    {
                        throw new ArgumentOutOfRangeException("index + offset < 0");
                    }
                    _index += offset;
                    break;
                case SeekOrigin.End:
                    if (_lines.Count - 1 + offset < 0)
                    {
                        throw new ArgumentOutOfRangeException("lines.Count - 1 + offset < 0");
                    }
                    _index = _lines.Count - 1 + offset;
                    break;
            }
        }
    }
}
