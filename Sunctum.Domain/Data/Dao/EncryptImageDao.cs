

using Homura.Extensions;
using Sunctum.Domain.Models;
using System;
using System.Data;

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
