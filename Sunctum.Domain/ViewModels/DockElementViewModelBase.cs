

using Prism.Mvvm;

namespace Sunctum.Domain.ViewModels
{
    public abstract class DockElementViewModelBase : BindableBase
    {
        public abstract string Title { get; }

        public abstract string ContentId { get; }

        public bool IsVisible { get; set; }

        public bool IsSelected { get; set; }

        public abstract bool CanClose { get; }
    }
}
