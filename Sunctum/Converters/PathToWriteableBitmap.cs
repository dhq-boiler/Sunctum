

using NLog;
using OpenCvSharp;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Data;
using System.Windows.Media.Imaging;

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
                    return ToWriteableBitmap(mat);
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

        private WriteableBitmap ToWriteableBitmap(Mat mat)
        {
            WriteableBitmap bitmap = new WriteableBitmap(mat.Cols, mat.Rows, 92, 92, System.Windows.Media.PixelFormats.Bgr24, null);

            unsafe
            {
                try
                {
                    bitmap.Lock();

                    byte* p_dst = (byte*)bitmap.BackBuffer.ToPointer();
                    byte* p_src = (byte*)mat.Data.ToPointer();
                    int step = bitmap.BackBufferStride;
                    int channels = 3;

                    for (int y = 0; y < bitmap.PixelHeight; ++y)
                    {
                        for (int x = 0; x < bitmap.PixelWidth; ++x)
                        {
                            for (int c = 0; c < channels; ++c)
                            {
                                *(p_dst + y * step + x * channels + c) = *(p_src + y * step + x * channels + c);
                            }
                        }
                    }
                }
                finally
                {
                    bitmap.Unlock();
                }
            }

            return bitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
