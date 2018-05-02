

using Sunctum.Domain.ViewModels;

namespace Sunctum.Domain.Logic.Async
{
    public interface IPageOrderUpdating : IAsyncTaskMaker
    {
        BookViewModel Target { get; set; }
    }
}
