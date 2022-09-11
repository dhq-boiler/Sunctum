using Homura.ORM;
using Homura.ORM.Mapping;
using System;

namespace boilersUpdater.Models
{
    public class VersionControl : EntityBaseObject
    {
        private string _FullVersion;
        private int _Major;
        private int _Minor;
        private int _Build;
        private int _Revision;
        private bool _IsValid;
        private DateTime? _InstalledDate;
        private DateTime? _RetiredDate;

        [Column("FullVersion", "Text", 0), PrimaryKey, Index]
        public string FullVersion
        {
            get { return _FullVersion; }
            set { SetProperty(ref _FullVersion, value); }
        }

        [Column("Major", "INTEGER", 1), NotNull]
        public int Major
        {
            get { return _Major; }
            set { SetProperty(ref _Major, value); }
        }

        [Column("Minor", "INTEGER", 2), NotNull]
        public int Minor
        {
            get { return _Minor; }
            set { SetProperty(ref _Minor, value); }
        }

        [Column("Build", "INTEGER", 3), NotNull]
        public int Build
        {
            get { return _Build; }
            set { SetProperty(ref _Build, value); }
        }

        [Column("Revision", "INTEGER", 4), NotNull]
        public int Revision
        {
            get { return _Revision; }
            set { SetProperty(ref _Revision, value); }
        }

        [Column("IsValid", "INTEGER", 5), NotNull]
        public bool IsValid
        {
            get { return _IsValid; }
            set { SetProperty(ref _IsValid, value); }
        }

        [Column("InstalledDate", "NUMERIC", 6)]
        public DateTime? InstalledDate
        {
            get { return _InstalledDate; }
            set { SetProperty(ref _InstalledDate, value); }
        }

        [Column("RetiredDate", "NUMERIC", 7)]
        public DateTime? RetiredDate
        {
            get { return _RetiredDate; }
            set { SetProperty(ref _RetiredDate, value); }
        }
    }
}
