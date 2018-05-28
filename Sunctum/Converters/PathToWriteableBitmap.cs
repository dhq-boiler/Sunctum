

using NLog;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Data;

namespace Sunctum.Converters
{
    internal class PathToWriteableBitmap : IValueConverter
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value as string;
            s_logger.Debug($"Load bitmap:{path}");
            return LoadBitmap(path);
        }

        private object LoadBitmap(string path)
        {
            try
            {
                using (Mat mat = new Mat(path, ImreadModes.Unchanged))
                {
                    if (mat.Rows == 0 || mat.Cols == 0)
                    {
                        var fileInfo = new FileInfo(path);
                        if (fileInfo.Length == 0)
                        {
                            s_logger.Error($"File is broken:{path}");
                            return null;
                        }
                        if (Path.GetExtension(path) == ".gif")
                        {
                            return null;
                        }
                        Thread.Sleep(100);
                        s_logger.Error($"Retry to load bitmap:{path}");
                        return LoadBitmap(path);
                    }
                    return mat.ToWriteableBitmap();
                }
            }
            catch (OutOfMemoryException e)
            {
                s_logger.Error(e);
                GC.WaitForPendingFinalizers();
                GC.Collect();
                return null;
            }
            catch (COMException e)
            {
                s_logger.Error(e);
                GC.WaitForPendingFinalizers();
                GC.Collect();
                return null;
            }
            catch (OpenCVException e)
            {
                s_logger.Error(e);
                GC.WaitForPendingFinalizers();
                GC.Collect();
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
