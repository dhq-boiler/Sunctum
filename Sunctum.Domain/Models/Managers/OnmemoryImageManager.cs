

using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Sunctum.Domain.Models.Managers
{
    public class OnmemoryImageManager
    {
        private static readonly OnmemoryImageManager s_instance = new OnmemoryImageManager();
        private Dictionary<Guid, MemoryStream> _map = new Dictionary<Guid, MemoryStream>();

        public static OnmemoryImageManager Instance { get { return s_instance; } }

        public void Put(Guid key, MemoryStream stream)
        {
            _map.Add(key, stream);
        }

        public MemoryStream PullAsMemoryStream(Guid key)
        {
            if (_map.ContainsKey(key))
            {
                return _map[key];
            }
            else
            {
                return null;
            }
        }

        public WriteableBitmap PullAsWriteableBitmap(Guid key)
        {
            if (_map.ContainsKey(key))
            {
                var stream = _map[key];
                stream.Seek(0, SeekOrigin.Begin);
                WriteableBitmap bitmap = new WriteableBitmap(BitmapFrame.Create(stream));
                return bitmap;
            }
            else
            {
                return null;
            }
        }

        public bool Exists(Guid key)
        {
            return _map.ContainsKey(key);
        }
    }
}
