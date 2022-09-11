using Homura.Extensions;
using Sunctum.Domain.Models;
using System;
using System.Data;

namespace Sunctum.Domain.Data.Dao
{
    public class GitHubReleasesLatestDao : SQLiteBaseDao<GitHubReleasesLatest>
    {
        public GitHubReleasesLatestDao() : base()
        { }

        public GitHubReleasesLatestDao(Type entityVersionType) : base(entityVersionType)
        { }

        protected override GitHubReleasesLatest ToEntity(IDataRecord reader)
        {
            return new GitHubReleasesLatest()
            {
                URL = reader.SafeGetString("URL", Table),
            };
        }
    }
}
