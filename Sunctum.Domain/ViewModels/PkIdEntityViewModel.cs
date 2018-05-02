

using System;
using System.Diagnostics;

namespace Sunctum.Domain.ViewModels
{
    public abstract class PkIdEntityViewModel : EntityBaseObjectViewModel
    {
        private Guid _ID;

        public Guid ID
        {
            [DebuggerStepThrough]
            get
            { return _ID; }
            set { SetProperty(ref _ID, value); }
        }
    }
}
