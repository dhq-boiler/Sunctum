

using Sunctum.Infrastructure.Core;

namespace Sunctum.Domain.Models
{
    public class Rect : BaseObject
    {
        public Rect()
        { }

        public Rect(double x, double y, double width, double height)
        {
            this.X = x;
            this.Y = y;
            Width = width;
            Height = height;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }
    }
}
