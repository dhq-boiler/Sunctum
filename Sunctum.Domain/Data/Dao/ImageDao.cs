

using NLog;
using simpleqb.Iso.Dml;
using Sunctum.Domain.Models;
using Sunctum.Domain.Util;
using Sunctum.Infrastructure.Data.Rdbms;
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
                ID = reader.SafeGetGuid("ID"),
                Title = reader.SafeGetString("Title"),
                RelativeMasterPath = reader.SafeGetString("MasterPath"),
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
                                                   .From.Table(new Table<Book>().Name, "b")
                                                   .Inner.Join(new Table<Page>().Name, "p").On.Column("p", "BookID").EqualTo.Column("b", "ID")
                                                   .Inner.Join(new Table<Image>().Name, "i").On.Column("p", "ImageID").EqualTo.Column("i", "ID")
                                                   .Where.Column("b", "ID").In.Value(bookIds.Cast<object>())
                                               .Union
                                                   .Select
                                                   .Column("i", "ID")
                                                   .Column("i", "Title")
                                                   .Column("i", "MasterPath")
                                                   .From.Table(new Table<Page>().Name, "p")
                                                   .Inner.Join(new Table<Image>().Name, "i").On.Column("p", "ImageID").EqualTo.Column("i", "ID")
                                                   .Where.Column("p", "ID").In.Value(pageIds.Cast<object>())
                                                .Union
                                                    .Select
                                                    .Column("i", "ID")
                                                    .Column("i", "Title")
                                                    .Column("i", "MasterPath")
                                                    .From.Table(new Table<Image>().Name, "i")
                                                    .Where.Column("ID").In.Value(imageIds.Cast<object>()))
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
    }
}
