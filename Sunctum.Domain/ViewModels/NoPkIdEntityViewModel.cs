

using System;

namespace Sunctum.Domain.ViewModels
{
    public class NoPkIdEntityViewModel : EntityBaseObjectViewModel
    {
        private Guid _ID;

        public Guid ID
        {
            get { return _ID; }
            set { SetProperty(ref _ID, value); }
        }
    }
}