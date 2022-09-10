using Homura.Extensions;
using NLog;
using Sunctum.Domain.Models;
using System;
using System.Data;

namespace Sunctum.Domain.Data.Dao
{
    public class VersionControlDao : SQLiteBaseDao<VersionControl>
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public VersionControlDao() : base()
        { }

        public VersionControlDao(Type entityVersionType) : base(entityVersionType)
        { }

        protected override VersionControl ToEntity(IDataRecord reader)
        {
            return new VersionControl()
            {
                FullVersion = reader.SafeGetString("FullVersion", Table),
                Major = reader.SafeGetInt("Major", Table),
                Minor = reader.SafeGetInt("Minor", Table),
                Build = reader.SafeGetInt("Build", Table),
                Revision = reader.SafeGetInt("Revision", Table),
                IsValid = reader.SafeGetBoolean("IsValid", Table),
                InstalledDate = reader.SafeGetNullableDateTime("InstalledDate", Table),
                RetiredDate = reader.SafeGetNullableDateTime("RetiredDate", Table),
            };
        }
    }
}
