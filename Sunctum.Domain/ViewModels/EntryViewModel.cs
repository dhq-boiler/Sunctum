

using Homura.Core;
using Reactive.Bindings;
using Sunctum.Domain.Logic.Encrypt;
using Sunctum.Domain.Models;
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
        private string _FingerPrint;

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

        public ReactivePropertySlim<bool> TitleIsEncrypted { get; } = new ReactivePropertySlim<bool>();

        public ReactivePropertySlim<bool> TitleIsDecrypted { get; } = new ReactivePropertySlim<bool>();

        public int? StarLevel
        {
            [DebuggerStepThrough]
            get
            { return _StarLevel; }
            set { SetProperty(ref _StarLevel, value); }
        }

        public string UnescapedTitle
        {
            get
            {
                if (TitleIsEncrypted.Value && !string.IsNullOrEmpty(Configuration.ApplicationConfiguration.Password))
                {
                    if (TitleIsDecrypted.Value)
                    {
                        return DecodeOrNull(Title);
                    }
                    return DecodeOrNull(Encryptor.DecryptString(Title, Configuration.ApplicationConfiguration.Password).Result);
                }
                return DecodeOrNull(Title);
            }
            set
            {
                if (TitleIsEncrypted.Value && !string.IsNullOrEmpty(Configuration.ApplicationConfiguration.Password))
                {
                    if (TitleIsDecrypted.Value)
                    {
                        Title = HttpUtility.HtmlEncode(value);
                        return;
                    }
                    Title = HttpUtility.HtmlEncode(Encryptor.DecryptString(Title, Configuration.ApplicationConfiguration.Password).Result);
                    return;
                }
                Title = HttpUtility.HtmlEncode(value);
            }
        }

        private string DecodeOrNull(string str)
        {
            return str != null ? HttpUtility.HtmlDecode(str) : null;
        }

        public string FingerPrint
        {
            [DebuggerStepThrough]
            get
            { return _FingerPrint; }
            set { SetProperty(ref _FingerPrint, value); }
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