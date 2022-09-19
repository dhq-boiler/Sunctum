using Sunctum.Domain.ViewModels;

namespace Sunctum.Domain.Logic.Async
{
    public interface IThumbnailRecording
    {
        ThumbnailViewModel Target { get; set; }
    }
}