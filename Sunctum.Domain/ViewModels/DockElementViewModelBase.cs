

using Prism.Mvvm;

namespace Sunctum.Domain.ViewModels
{
    public abstract class DockElementViewModelBase : BindableBase
    {
        private bool _IsVisible;
        private bool _IsSelected;

        public abstract string Title { get; }

        public abstract string ContentId { get; }

        public bool IsVisible
        {
            get { return _IsVisible; }
            set { SetProperty(ref _IsVisible, value); }
        }

        public bool IsSelected
        {
            get { return _IsSelected; }
            set { SetProperty(ref _IsSelected, value); }
        }

        public abstract bool CanClose { get; }
    }
}
