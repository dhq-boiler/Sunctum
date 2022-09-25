

using Homura.Extensions;
using Homura.ORM;
using Homura.QueryBuilder.Iso.Dml;
using NLog;
using Sunctum.Domain.Logic.Encrypt;
using Sunctum.Domain.Models;
using Sunctum.Domain.Util;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;

namespace Sunctum.Domain.Data.Dao
{
    internal class ImageDao : SQLiteBaseDao<Image>
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public ImageDao() : base()
        { }

        public ImageDao(Type entityVersionType) : base(entityVersionType)
        { }

        protected override Image ToEntity(IDataRecord reader)
        {
            return new Image()
            {
                ID = reader.SafeGetGuid("ID", Table),
                Title = reader.SafeGetString("Title", Table),
                RelativeMasterPath = reader.SafeGetString("MasterPath", Table),
                IsEncrypted = reader.SafeGetBoolean("IsEncrypted", Table),
                TitleIsEncrypted = CatchThrow(() => reader.SafeGetBoolean("TitleIsEncrypted", Table)),
            };
        }

        internal IEnumerable<Image> GetAllImages(IEnumerable<Guid> bookIds, IEnumerable<Guid> pageIds, IEnumerable<Guid> imageIds, DbConnection conn = null)
        {
            bool isTransaction = conn != null;

            try
            {
                if (!isTransaction)
                {
                    conn = GetConnection();
                }

                using (var command = conn.CreateCommand())
                {
                    using (var query = new Select().Column("i", "ID")
                                                   .Column("i", "Title")
                                                   .Column("i", "MasterPath")
                                                   .Column("i", "IsEncrypted")
                                                   .Column("i", "TitleIsEncrypted")
                                                   .From.Table(new Table<Book>().Name, "b")
                                                   .Inner.Join(new Table<Page>().Name, "p").On.Column("p", "BookID").EqualTo.Column("b", "ID")
                                                   .Inner.Join(new Table<Image>().Name, "i").On.Column("p", "ImageID").EqualTo.Column("i", "ID")
                                                   .Where.Column("b", "ID").In.Array(bookIds.Cast<object>())
                                               .Union
                                                   .Select
                                                   .Column("i", "ID")
                                                   .Column("i", "Title")
                                                   .Column("i", "MasterPath")
                                                   .Column("i", "IsEncrypted")
                                                   .Column("i", "TitleIsEncrypted")
                                                   .From.Table(new Table<Page>().Name, "p")
                                                   .Inner.Join(new Table<Image>().Name, "i").On.Column("p", "ImageID").EqualTo.Column("i", "ID")
                                                   .Where.Column("p", "ID").In.Array(pageIds.Cast<object>())
                                                .Union
                                                    .Select
                                                    .Column("i", "ID")
                                                    .Column("i", "Title")
                                                    .Column("i", "MasterPath")
                                                    .Column("i", "IsEncrypted")
                                                    .Column("i", "TitleIsEncrypted")
                                                    .From.Table(new Table<Image>().Name, "i")
                                                    .Where.Column("ID").In.Array(imageIds.Cast<object>()))
                    {
                        string sql = query.ToSql();
                        command.CommandText = sql;
                        query.SetParameters(command);

                        List<Image> ret = new List<Image>();

                        using (var rdr = command.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                ret.Add(ToEntity(rdr));
                            }
                        }

                        return ret;
                    }
                }
            }
            catch (SQLiteException)
            {
                throw;
            }
            finally
            {
                if (!isTransaction)
                {
                    conn.Dispose();
                }
            }
        }

        internal void GetProperty(ref ImageViewModel image, DbConnection conn = null)
        {
            bool isTransaction = conn != null;

            try
            {
                if (!isTransaction)
                {
                    conn = GetConnection();
                }

                using (var command = conn.CreateCommand())
                {
                    using (var query = new Select()
                                                   .Column("i", "ID").As("iId")
                                                   .Column("i", "Title").As("iTitle")
                                                   .Column("i", "MasterPath").As("iMasterPath")
                                                   .Column("i", "TitleIsEncrypted").As("iTitleIsEncrypted")
                                                   .Column("i", "IsEncrypted").As("iIsEncrypted")
                                                   .Column("t", "ID").As("tId")
                                                   .Column("t", "ImageID").As("tImageId")
                                                   .Column("t", "Path").As("tPath")
                                                   .From.Table(new Table<Image>().Name, "i")
                                                   .Left.Join(new Table<Thumbnail>().Name, "t").On.Column("i", "ID").EqualTo.Column("tImageId")
                                                   .Where.Column("iId").EqualTo.Value(image.ID))
                    {
                        string sql = query.ToSql();
                        command.CommandText = sql;
                        query.SetParameters(command);

                        using (var rdr = command.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                image = new ImageViewModel();
                                image.Configuration = Configuration.ApplicationConfiguration;
                                image.ID = rdr.SafeGetGuid("iId", null);
                                image.Title = rdr.SafeGetString("iTitle", null);
                                image.RelativeMasterPath = rdr.SafeGetString("iMasterPath", null);
                                image.IsEncrypted = rdr.SafeGetBoolean("iIsEncrypted", null);
                                image.TitleIsEncrypted.Value = CatchThrow(() => rdr.SafeGetBoolean("iTitleIsEncrypted", null));
                                if (image.TitleIsEncrypted.Value && !image.TitleIsDecrypted.Value && !string.IsNullOrEmpty(Configuration.ApplicationConfiguration.Password))
                                {
                                    image.Title = Encryptor.DecryptString(image.Title, Configuration.ApplicationConfiguration.Password).Result;
                                    image.TitleIsDecrypted.Value = true;
                                }

                                if (!rdr.IsDBNull("tId") && !rdr.IsDBNull("tImageId") && !rdr.IsDBNull("tPath"))
                                {
                                    var thumbnail = new ThumbnailViewModel();
                                    thumbnail.ID = rdr.SafeGetGuid("tId", null);
                                    thumbnail.ImageID = rdr.SafeGetGuid("tImageId", null);
                                    thumbnail.RelativeMasterPath = rdr.SafeGetString("tPath", null);
                                    image.Thumbnail = thumbnail;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (!isTransaction)
                {
                    conn.Dispose();
                }
            }
        }

        internal long SumTotalFileSize()
        {
            var conn = GetConnection();
            using (var command = conn.CreateCommand())
            {
                string sql = "select sum(ByteSize) from " + Table.Name;
                command.CommandText = sql;

                return (long)command.ExecuteScalar();
            }
        }
    }
}
