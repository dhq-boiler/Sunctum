using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PickoutCover.Domain.Logic
{
    class _WriteableBitmapConverter
    {
        private PixelFormat SelectChannels(Mat mat)
        {
            switch (mat.Channels())
            {
                case 1:
                    return System.Windows.Media.PixelFormats.Gray8;
                case 3:
                    return System.Windows.Media.PixelFormats.Bgr24;
                case 4:
                    return System.Windows.Media.PixelFormats.Bgra32;
                default:
                    throw new NotSupportedException();
            }
        }

        private byte PickPixel(Mat mat, int y, int x, int c)
        {
            switch (mat.Channels())
            {
                case 1:
                    return mat.At<Vec3b>(y, x)[c];
                case 3:
                    return mat.At<Vec3b>(y, x)[c];
                case 4:
                    return mat.At<Vec4b>(y, x)[c];
                default:
                    throw new NotSupportedException();
            }
        }

        public WriteableBitmap ToWriteableBitmap(Mat mat)
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
                            for (int c = 0; c < channels; ++c)
                            {
                                var bytes = PickPixel(mat, y, x, c);
                                *(p_dst + y * step_dst + x * channels + c) = bytes;
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
    }
}
