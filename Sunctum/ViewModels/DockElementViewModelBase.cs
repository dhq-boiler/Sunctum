﻿

using Prism.Mvvm;

namespace Sunctum.ViewModels
{
    public abstract class DockElementViewModelBase : BindableBase
    {
        public abstract string Title { get; }

        public abstract string ContentId { get; }
    }
}
