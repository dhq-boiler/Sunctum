

using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace Sunctum.Domain.Models.Managers
{
    public class OnmemoryImageManager
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private static readonly OnmemoryImageManager s_instance = new OnmemoryImageManager();
        private Dictionary<Tuple<Guid, bool>, MemoryStream> _map = new Dictionary<Tuple<Guid, bool>, MemoryStream>();

        public static OnmemoryImageManager Instance { get { return s_instance; } }

        public void Put(Guid key, bool isThumbnail, MemoryStream stream)
        {
            var tuple = new Tuple<Guid, bool>(key, isThumbnail);
            if (!_map.ContainsKey(tuple))
            {
                _map.Add(tuple, stream);
            }
        }

        public void Clear()
        {
            _map.Clear();
        }

        public MemoryStream PullAsMemoryStream(Guid key, bool isThumbnail)
        {
            var tuple = new Tuple<Guid, bool>(key, isThumbnail);
            if (_map.ContainsKey(tuple))
            {
                return _map[tuple];
            }
            else
            {
                return null;
            }
        }

        public WriteableBitmap PullAsWriteableBitmap(Guid key, bool isThumbnail)
        {
            var tuple = new Tuple<Guid, bool>(key, isThumbnail);
            if (_map.ContainsKey(tuple))
            {
                try
                {
                    var stream = _map[tuple];
                    stream.Seek(0, SeekOrigin.Begin);
                    WriteableBitmap bitmap = new WriteableBitmap(BitmapFrame.Create(stream));
                    return bitmap;
                }
                catch (NotSupportedException e)
                {
                    s_logger.Error($"{e} key:{key}, isThumbnail:{isThumbnail}");
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public bool Exists(Guid key, bool isThumbnail)
        {
            var tuple = new Tuple<Guid, bool>(key, isThumbnail);
            return _map.ContainsKey(tuple);
        }
    }
}
