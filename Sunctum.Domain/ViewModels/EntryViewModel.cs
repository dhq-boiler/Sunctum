

using Homura.Core;
using Sunctum.Infrastructure.Core;
using System;
using System.Diagnostics;
using System.Web;

namespace Sunctum.Domain.ViewModels
{
    public class EntryViewModel : PkIdEntityViewModel
    {
        private string _Title;
        private bool _IsLoaded;
        private int? _StarLevel;

        protected EntryViewModel()
        { }

        protected EntryViewModel(Guid id, string title)
            : this()
        {
            ID = id;
            Title = title;
        }

        public string Title
        {
            [DebuggerStepThrough]
            get
            { return _Title; }
            set
            {
                SetProperty(ref _Title, value);
                OnPropertyChanged(PropertyNameUtility.GetPropertyName(() => UnescapedTitle));
            }
        }

        public int? StarLevel
        {
            [DebuggerStepThrough]
            get
            { return _StarLevel; }
            set { SetProperty(ref _StarLevel, value); }
        }

        public string UnescapedTitle
        {
            get { return Title != null ? HttpUtility.HtmlDecode(Title) : null; }
            set { Title = HttpUtility.HtmlEncode(value); }
        }

        public bool IsLoaded
        {
            [DebuggerStepThrough]
            get
            { return _IsLoaded; }
            set { SetProperty(ref _IsLoaded, value); }
        }
    }
}