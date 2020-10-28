

using NLog;
using OpenCvSharp;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Converters;
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
            WriteableBitmap bitmap = new WriteableBitmap(mat.Cols, mat.Rows, 92, 92, SelectChannels(mat), null);

            unsafe
            {
                try
                {
                    bitmap.Lock();

                    byte* p_dst = (byte*)bitmap.BackBuffer.ToPointer();
                    int step_dst = bitmap.BackBufferStride;
                    int channels = mat.Channels();
                    long step_src = mat.Step();

                    for (int y = 0; y < bitmap.PixelHeight; ++y)
                    {
                        for (int x = 0; x < bitmap.PixelWidth; ++x)
                        {
                            var vector = PickPixel(mat, y, x);
                            for (int c = 0; c < channels; ++c)
                            {
                                *(p_dst + y * step_dst + x * channels + c) = vector[c];
                            }
                        }
                    }

                    bitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
                }
                finally
                {
                    bitmap.Unlock();
                }
            }

            return bitmap;
        }

        private IVec<byte> PickPixel(Mat mat, int y, int x)
        {
            switch (mat.Channels())
            {
                case 3:
                    return mat.At<Vec3b>(y, x);
                case 4:
                    return mat.At<Vec4b>(y, x);
                default:
                    throw new NotSupportedException();
            }
        }

        private PixelFormat SelectChannels(Mat mat)
        {
            switch (mat.Channels())
            {
                case 3:
                    return System.Windows.Media.PixelFormats.Bgr24;
                case 4:
                    return System.Windows.Media.PixelFormats.Bgra32;
                default:
                    throw new NotSupportedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
