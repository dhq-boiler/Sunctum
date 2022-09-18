using Sunctum.Domain.ViewModels;

namespace Sunctum.Domain.Logic.Async
{
    public interface IThumbnailGenerating
    {
         ImageViewModel Target { get; set; }
    }
}