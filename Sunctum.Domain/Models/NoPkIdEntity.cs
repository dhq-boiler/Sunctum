

using Homura.ORM;
using Homura.ORM.Mapping;
using System;

namespace Sunctum.Domain.Models
{
    public abstract class NoPkIdEntity : EntityBaseObject, IId
    {
        private Guid _ID;

        [Column("ID", "NUMERIC", 0)]
        [Since(typeof(VersionOrigin))]
        public Guid ID
        {
            get { return _ID; }
            set { SetProperty(ref _ID, value); }
        }
    }
}
