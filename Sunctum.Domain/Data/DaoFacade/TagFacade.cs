

using NLog;
using Sunctum.Domain.Bridge;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Util;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sunctum.Domain.Data.DaoFacade
{
    public static class TagFacade
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static void Insert(TagViewModel target)
        {
            TagDao dao = new TagDao();
            dao.Insert(target.ToEntity());
            s_logger.Debug($"INSERT Tag:{target}");
        }

        public static bool Exists(string tagName)
        {
            TagDao dao = new TagDao();
            return dao.CountBy(new Dictionary<string, object>() { { "Name", tagName } }) > 0;
        }

        public static IEnumerable<TagViewModel> FindAll()
        {
            TagDao dao = new TagDao();
            return dao.FindAll().ToViewModel();
        }

        public static TagViewModel FindByTagName(string tagName)
        {
            TagDao dao = new TagDao();
            return dao.FindBy(new Dictionary<string, object>() { { "Name", tagName } }).SingleOrDefault().ToViewModel();
        }

        internal static void DeleteByTagName(string tagName)
        {
            TagDao dao = new TagDao();
            dao.Delete(new Dictionary<string, object>() { { "Name", tagName } });
        }

        public static IEnumerable<TagViewModel> OrderByNaturalString()
        {
            TagDao dao = new TagDao();
            return dao.FindAll().OrderBy(t => t.Name, new NaturalStringComparer()).ToViewModel();
        }

        public static TagViewModel FindBy(Guid id)
        {
            TagDao dao = new TagDao();
            return dao.FindBy(new Dictionary<string, object>() { { "ID", id } }).SingleOrDefault().ToViewModel();
        }

        public static void Update(TagViewModel target)
        {
            TagDao dao = new TagDao();
            dao.Update(target.ToEntity());
        }

        public static void Delete(Guid id)
        {
            TagDao dao = new TagDao();
            dao.DeleteWhereIDIs(id);
        }
    }
}
