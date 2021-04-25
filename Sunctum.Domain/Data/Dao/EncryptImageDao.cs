

using Sunctum.Domain.Models;
using Sunctum.Domain.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunctum.Domain.Data.Dao
{
    internal class EncryptImageDao : SQLiteBaseDao<EncryptImage>
    {
        public EncryptImageDao() : base()
        { }

        public EncryptImageDao(Type entityVersionType) : base(entityVersionType)
        { }

        protected override EncryptImage ToEntity(IDataRecord reader)
        {
            return new EncryptImage()
            {
                ID = reader.SafeGetGuid("ID", Table),
                TargetImageID = reader.SafeGetGuid("TargetImageID", Table),
                EncryptFilePath = reader.SafeGetString("EncryptFilePath", Table),
            };
        }
    }
}
