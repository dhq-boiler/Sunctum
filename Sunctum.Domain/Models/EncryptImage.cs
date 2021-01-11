

using Homura.ORM.Mapping;
using Sunctum.Domain.Models.Conversion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sunctum.Domain.Models
{
    [DefaultVersion(typeof(VersionOrigin))]
    public class EncryptImage : PkIdEntity
    {
        private Guid _TargetImageID;
        private string _EncryptFilePath;

        [Column("TargetImageID", "NUMERIC", 1)]
        public Guid TargetImageID
        {
            [DebuggerStepThrough]
            get
            { return _TargetImageID; }
            set { SetProperty(ref _TargetImageID, value); }
        }

        [Column("EncryptFilePath", "TEXT", 2)]
        public string EncryptFilePath
        {
            [DebuggerStepThrough]
            get
            { return _EncryptFilePath; }
            set { SetProperty(ref _EncryptFilePath, value); }
        }
    }
}
