

using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PickoutCover.Domain.Logic.CoverSegment
{
    public class CoverSegmentExtractor
    {
        public static WriteableBitmap LoadBitmap(string filename)
        {
            using (Mat mat = new Mat(filename, ImreadModes.Unchanged))
            using (Mat bgra = new Mat())
            {
                Cv2.CvtColor(mat, bgra, ColorConversionCodes.BGR2BGRA);
                return WriteableBitmapConverter.ToWriteableBitmap(bgra);
            }
        }

        public static IEnumerable<int> ExtractCoverVerticalSegmentIndexes(string filename)
        {
            using (Mat mat = new Mat(filename, ImreadModes.Unchanged))
            using (Mat gray = new Mat())
            using (Mat sobel = new Mat())
            using (Mat thsd = new Mat())
            using (Mat hsv = new Mat())
            {
                Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);
                Cv2.Sobel(gray, sobel, MatType.CV_8UC1, 1, 0, 3, 1, 0, BorderTypes.Default);
                Cv2.Threshold(sobel, thsd, 32, 255, ThresholdTypes.Binary);

                Cv2.CvtColor(mat, hsv, ColorConversionCodes.BGR2HSV);
                var list = X(thsd);
                var l = list.Where(a => a.Count > thsd.Height * 0.15 && a.BreakCount < thsd.Height * 0.035).Select(a => a.Index);
                var lmerged = Merge(l);

                using (Mat h = hsv.ExtractChannel(0))
                using (Mat s = hsv.ExtractChannel(1))
                {
                    var lh = Y(h, lmerged, 0.05, 2);
                    var ls = Y(s, lmerged, 0.15, 2);
                    return And(lh, ls);
                }
            }
        }

        public unsafe static void Line(WriteableBitmap bitmap, IEnumerable<int> xOffsets, Color color)
        {
            byte* p = (byte*)bitmap.BackBuffer.ToPointer();

            int height = bitmap.PixelHeight;
            long step = bitmap.BackBufferStride;

            foreach (int x in xOffsets)
            {
                for (int y = 0; y < height; ++y)
                {
                    *(p + y * step + x * 4 + 0) = color.B;
                    *(p + y * step + x * 4 + 1) = color.G;
                    *(p + y * step + x * 4 + 2) = color.R;
                    *(p + y * step + x * 4 + 3) = color.A;
                }
            }
        }

        public unsafe static void BoldLine(WriteableBitmap bitmap, IEnumerable<int> xOffsets, Color color)
        {
            byte* p = (byte*)bitmap.BackBuffer.ToPointer();

            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            long step = bitmap.BackBufferStride;

            foreach (int x in xOffsets)
            {
                for (int i = -1; i <= 1; ++i)
                {
                    if (x + i >= 0 && x + i < width)
                    {
                        for (int y = 0; y < height; ++y)
                        {
                            *(p + y * step + (x + i) * 4 + 0) = color.B;
                            *(p + y * step + (x + i) * 4 + 1) = color.G;
                            *(p + y * step + (x + i) * 4 + 2) = color.R;
                            *(p + y * step + (x + i) * 4 + 3) = color.A;
                        }
                    }
                }
            }
        }

        public unsafe static void GrayoutLeft(WriteableBitmap bitmap, int xOffset)
        {
            byte* p = (byte*)bitmap.BackBuffer.ToPointer();

            int height = bitmap.PixelHeight;
            long step = bitmap.BackBufferStride;

            for (int x = 0; x < xOffset; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    *(p + y * step + x * 4 + 0) = (byte)(*(p + y * step + x * 4 + 0) / 2);
                    *(p + y * step + x * 4 + 1) = (byte)(*(p + y * step + x * 4 + 1) / 2);
                    *(p + y * step + x * 4 + 2) = (byte)(*(p + y * step + x * 4 + 2) / 2);
                }
            }
        }

        public unsafe static void GrayoutRight(WriteableBitmap bitmap, int xOffset)
        {
            byte* p = (byte*)bitmap.BackBuffer.ToPointer();

            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            long step = bitmap.BackBufferStride;

            for (int x = xOffset + 1; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    *(p + y * step + x * 4 + 0) = (byte)(*(p + y * step + x * 4 + 0) / 2);
                    *(p + y * step + x * 4 + 1) = (byte)(*(p + y * step + x * 4 + 1) / 2);
                    *(p + y * step + x * 4 + 2) = (byte)(*(p + y * step + x * 4 + 2) / 2);
                }
            }
        }

        private static IEnumerable<int> And(IEnumerable<int> left, IEnumerable<int> right)
        {
            return left.Where(a => right.Contains(a));
        }

        private unsafe static IEnumerable<int> Y(Mat mat, IEnumerable<int> lmerged, double diffPixelRate, int offset)
        {
            List<int> ret = new List<int>();
            byte* p = (byte*)mat.Data.ToPointer();

            int width = mat.Width;
            int height = mat.Height;
            long step = mat.Step();
            int channels = mat.Channels();

            foreach (var x in lmerged)
            {
                double totalDistance = 0;
                for (int y = 0; y < height; ++y)
                {
                    for (int c = 0; c < channels; ++c)
                    {
                        byte left = *(p + (y * step) + ((x - offset) * channels) + c);
                        byte right = *(p + (y * step) + ((x + offset) * channels) + c);
                        totalDistance += Math.Abs(left - right);
                    }
                }

                double distancePerPixel = totalDistance / (255.0 * channels) / height;
                if (distancePerPixel >= diffPixelRate)
                {
                    ret.Add(x);
                }
            }

            return ret;
        }

        private static IEnumerable<int> Merge(IEnumerable<int> enumerable)
        {
            List<int> ret = new List<int>();

            List<int> pool = new List<int>();
            var orderby = enumerable.OrderBy(a => a);
            int prevVal = orderby.First();
            foreach (var i in orderby)
            {
                if (i - prevVal > 1)
                {
                    //pool -> ret
                    if (pool.Count == 1 || pool.Count == 2)
                    {
                        ret.Add(pool[0]);
                    }
                    else if (pool.Count >= 3)
                    {
                        ret.Add(pool.ElementAt(pool.Count / 2));
                    }

                    //pool reset
                    pool.Clear();
                }

                pool.Add(i);

                prevVal = i;
            }
            return ret;
        }

        private unsafe static List<SegmentCandidate> X(Mat thsd)
        {
            List<SegmentCandidate> ret = new List<SegmentCandidate>();
            byte* p = (byte*)thsd.Data.ToPointer();
            int width = thsd.Width;
            int height = thsd.Height;
            long step = thsd.Step();

            for (int x = 0; x < width; ++x)
            {
                bool onSegment = false;
                int count = 0;
                int breakCount = 0;
                for (int y = 0; y < height; ++y)
                {
                    byte value = *(p + (y * step) + x);
                    if (value == 255)
                    {
                        ++count;
                        onSegment = true;
                    }
                    else if (onSegment)
                    {
                        ++breakCount;
                        onSegment = false;
                    }
                }
                ret.Add(new SegmentCandidate(x, count, breakCount));
            }
            return ret;
        }

        private class SegmentCandidate
        {
            internal int Index { get; set; }
            internal int Count { get; set; }
            internal int BreakCount { get; set; }

            internal SegmentCandidate(int index, int count, int breakCount)
            {
                Index = index;
                Count = count;
                BreakCount = breakCount;
            }
        }
    }
}
