

using System;
using System.Collections.Generic;
using System.Linq;
using Homura.Core;
using OpenCvSharp;
using Sunctum.Domain.Models;
using Sunctum.Infrastructure.Core;

namespace Sunctum.Domain.Logic.BookSorting
{
    public class CoverComparator
    {
        public enum Color
        {
            Blue = 0,
            Green = 1,
            Red = 2
        }

        internal static CoverSegmentCollection ComputeColorInfo(string filename, Color targetColor)
        {
            using (Mat mat = new Mat(filename, ImreadModes.Unchanged))
            using (Mat normalized = new Mat())
            {
                Cv2.Normalize(mat, normalized, 0, 255, NormTypes.MinMax);

                CoverSegmentCollection ret = new CoverSegmentCollection();
                for (int y = 0; y < Specifications.VERTICAL_SEGMENT_COUNT; ++y)
                {
                    for (int x = 0; x < Specifications.HORIZONTAL_SEGMENT_COUNT; ++x)
                    {
                        ret.Add(Median(normalized, x, y, Specifications.HORIZONTAL_SEGMENT_COUNT, Specifications.VERTICAL_SEGMENT_COUNT, (int)targetColor));
                    }
                }
                return ret;
            }
        }

        private unsafe static CoverSegmentColorInfo Median(Mat mat, int xOffset, int yOffset, int s_HORIZONTAL_SEGMENT_COUNT, int s_VERTICAL_SEGMENT_COUNT, int channelOffset)
        {
            byte* p = (byte*)mat.Data.ToPointer();
            int width = mat.Width;
            int height = mat.Height;
            int channels = mat.Channels();
            long step = mat.Step();
            int horizontalPixelCount = width / s_HORIZONTAL_SEGMENT_COUNT;
            int verticalPixelCount = height / s_VERTICAL_SEGMENT_COUNT;
            int totalPixelCount = horizontalPixelCount * verticalPixelCount;

            List<int> rList = new List<int>();

            int yInclusiveBegin = yOffset * verticalPixelCount;
            int yExclusiveEnd = (yOffset + 1) * verticalPixelCount;

            for (int y = yInclusiveBegin; y < yExclusiveEnd; ++y)
            {
                int xInclusiveBegin = xOffset * horizontalPixelCount;
                int xExclusiveEnd = (xOffset + 1) * horizontalPixelCount;
                long ystep = y * step;
                for (int x = xInclusiveBegin; x < xExclusiveEnd; ++x)
                {
                    rList.Add(*(p + ystep + x * channels + channelOffset));
                }
            }

            return new CoverSegmentColorInfo(rList.OrderBy(s => s).Skip(totalPixelCount / 2).Take(1).First());
        }

        public class CoverSegmentCollection : List<CoverSegmentColorInfo>, IComparable<CoverSegmentCollection>
        {
            public int CompareTo(CoverSegmentCollection other)
            {
                var xMedianAvg = this.Average(a => a.Median);
                var yMedianAvg = other.Average(a => a.Median);

                int ret = xMedianAvg.CompareTo(yMedianAvg);
                if (ret != 0) return ret;

                for (int i = 0; i < Count; ++i)
                {
                    var xMedian = this[i].Median;
                    var yMedian = other[i].Median;

                    ret = xMedian.CompareTo(yMedian);
                    if (ret != 0) return ret;
                }

                return 0;
            }

            public override string ToString()
            {
                string s = "{";
                Queue<CoverSegmentColorInfo> q = new Queue<CoverSegmentColorInfo>(this);
                while (q.Count() > 0)
                {
                    var csci = q.Dequeue();
                    s += csci.ToString();
                    if (q.Count() > 0)
                    {
                        s += ", ";
                    }
                }
                return s;
            }
        }

        public class CoverSegmentColorInfo : BaseObject, IComparer<CoverSegmentColorInfo>
        {
            internal CoverSegmentColorInfo(int median)
            {
                Median = median;
            }

            /// <summary>
            /// 区分領域の中央値
            /// </summary>
            public int Median { get; set; }

            public int Compare(CoverSegmentColorInfo x, CoverSegmentColorInfo y)
            {
                return x.Median.CompareTo(y.Median);
            }
        }
    }
}
