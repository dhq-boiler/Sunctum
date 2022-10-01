using Homura.ORM;
using Homura.ORM.Mapping;
using System.Diagnostics;

namespace Sunctum.Domain.Models
{
    [DefaultVersion(typeof(VersionOrigin))]
    public class KeyValue : EntityBaseObject
    {
        private string _Key;
        private string _Value;

        public KeyValue()
        { }

        [Column("Key", "TEXT", 0), NotNull]
        public string Key
        {
            [DebuggerStepThrough]
            get
            { return _Key; }
            set
            {
                SetProperty(ref _Key, value);
            }
        }

        [Column("Value", "TEXT", 1)]
        public string Value
        {
            [DebuggerStepThrough]
            get
            { return _Value; }
            set
            {
                SetProperty(ref _Value, value);
            }
        }
    }
}
