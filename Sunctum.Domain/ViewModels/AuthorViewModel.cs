

using Homura.Core;
using Homura.ORM;
using Reactive.Bindings;
using Sunctum.Domain.Logic.Encrypt;
using Sunctum.Domain.Models;
using System;
using System.Diagnostics;
using System.Web;

namespace Sunctum.Domain.ViewModels
{
    public class AuthorViewModel : PkIdEntityViewModel, IId, IName, ICloneable
    {
        private string _Name;

        public AuthorViewModel()
        { }

        public AuthorViewModel(string name)
        {
            Name = name;
        }

        public AuthorViewModel(Guid guid, string name)
        {
            ID = guid;
            Name = name;
        }

        public string Name
        {
            [DebuggerStepThrough]
            get
            { return _Name; }
            set
            {
                SetProperty(ref _Name, value);
                OnPropertyChanged(PropertyNameUtility.GetPropertyName(() => UnescapedName));
            }
        }

        public string UnescapedName
        {
            get
            {
                return DecodeOrNull(Name);
            }
            set
            {
                if (NameIsEncrypted.Value && !string.IsNullOrEmpty(Configuration.ApplicationConfiguration.Password))
                {
                    if (NameIsDecrypted.Value)
                    {
                        Name = HttpUtility.HtmlEncode(value);
                        return;
                    }
                    Name = HttpUtility.HtmlEncode(Encryptor.DecryptString(Name, Configuration.ApplicationConfiguration.Password).Result);
                    return;
                }
                Name = HttpUtility.HtmlEncode(value);
            }
        }

        private string DecodeOrNull(string str)
        {
            return str != null ? HttpUtility.HtmlDecode(str) : null;
        }

        public ReactivePropertySlim<bool> NameIsEncrypted { get; } = new ReactivePropertySlim<bool>();

        public ReactivePropertySlim<bool> NameIsDecrypted { get; } = new ReactivePropertySlim<bool>();

        public object Clone()
        {
            return new AuthorViewModel()
            {
                ID = ID,
                Name = Name
            };
        }

        public override string ToString()
        {
            return typeof(AuthorViewModel).Name + " [Name = " + Name + ", " + base.ToString() + "]";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AuthorViewModel))
            {
                return false;
            }

            var author = obj as AuthorViewModel;
            return author.ID.Equals(ID)
                && author.Name.Equals(Name);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode()
                ^ Name.GetHashCode();
        }
    }
}
