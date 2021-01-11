

using Homura.Core;

namespace PickoutCover.Domain.Logic.CoverSegment
{
    public class CoverSideCandidate : BaseObject
    {
        public int Offset { get; set; }
        public bool IsPredicted { get; set; }

        public CoverSideCandidate(int offset, bool isPredicted)
        {
            Offset = offset;
            IsPredicted = isPredicted;
        }

        public override string ToString()
        {
            return $"{Offset}{IsPredictedString()}";
        }

        private string IsPredictedString()
        {
            return IsPredicted ? " (予測)" : "";
        }
    }
}
