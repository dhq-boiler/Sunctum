

using Sunctum.Domain.Models;
using Sunctum.Infrastructure.Core;
using Sunctum.Infrastructure.Data.Rdbms;
using System;
using System.Diagnostics;
using System.Web;

namespace Sunctum.Domain.ViewModels
{
    public class TagViewModel : PkIdEntityViewModel, IId, IName, ICloneable
    {
        public TagViewModel()
        { }

        public TagViewModel(Guid id, string name)
        {
            ID = id;
            Name = name;
        }

        private string _Name;

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
            get { return Name != null ? HttpUtility.HtmlDecode(Name) : null; }
            set { Name = HttpUtility.HtmlEncode(value); }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj == null || obj.ToString().Equals("{DisconnectedItem}"))
                return false;
            return ID.Equals((obj as TagViewModel).ID);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public object Clone()
        {
            return new TagViewModel()
            {
                ID = this.ID,
                Name = this.Name
            };
        }
    }
}
