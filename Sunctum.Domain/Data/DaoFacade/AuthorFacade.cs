﻿

using Homura.ORM;
using NLog;
using Sunctum.Domain.Bridge;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Models;
using Sunctum.Domain.Util;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Sunctum.Domain.Data.DaoFacade
{
    public static class AuthorFacade
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static void Create(AuthorViewModel target)
        {
            AuthorDao dao = new AuthorDao();
            dao.Insert(target.ToEntity());
            s_logger.Debug($"INSERT Author:{target}");
        }

        public static AuthorViewModel InsertIfNotExists(AuthorViewModel target, DataOperationUnit dataOpUnit = null)
        {
            Contract.Requires(target != null);

            AuthorDao dao = new AuthorDao();

            if (target == null)
            {
                return null;
            }
            else if (Exists(target.Name, dataOpUnit))
            {
                return FindBy(target.Name, dataOpUnit).Single().ToViewModel();
            }
            else
            {
                dao.Insert(target.ToEntity(), dataOpUnit?.CurrentConnection);
                s_logger.Debug($"INSERT Author:{target}");
                return target;
            }
        }

        public static IEnumerable<AuthorViewModel> FindAll()
        {
            AuthorDao dao = new AuthorDao();
            return dao.FindAll().ToViewModel();
        }

        public static IEnumerable<AuthorCountViewModel> FindAllAsCountOrderByNameAsc()
        {
            AuthorDao dao = new AuthorDao();
            return dao.FindAllAsCountOrderByNameAsc().ToViewModel();
        }

        public static IEnumerable<AuthorCountViewModel> FindAllAsCountOrderByNameDesc()
        {
            AuthorDao dao = new AuthorDao();
            return dao.FindAllAsCountOrderByNameDesc().ToViewModel();
        }

        public static IEnumerable<AuthorCountViewModel> FindAllAsCountOrderByCountAsc()
        {
            AuthorDao dao = new AuthorDao();
            return dao.FindAllAsCountOrderByCountAsc().ToViewModel();
        }

        public static IEnumerable<AuthorCountViewModel> FindAllAsCountOrderByCountDesc()
        {
            AuthorDao dao = new AuthorDao();
            return dao.FindAllAsCountOrderByCountDesc().ToViewModel();
        }

        public static void Delete(Guid id)
        {
            AuthorDao dao = new AuthorDao();
            dao.DeleteWhereIDIs(id);
            s_logger.Debug($"DELETE Author:{id}");
        }

        public static IEnumerable<AuthorViewModel> OrderByNaturalString()
        {
            AuthorDao dao = new AuthorDao();
            return dao.FindAll().OrderBy(a => a.Name, new NaturalStringComparer()).ToViewModel();
        }

        public static AuthorViewModel FindBy(Guid id, DataOperationUnit dataOpUnit = null)
        {
            AuthorDao dao = new AuthorDao();
            return dao.FindBy(new Dictionary<string, object>() { { "ID", id } }, dataOpUnit?.CurrentConnection).SingleOrDefault().ToViewModel();
        }

        public static IEnumerable<Author> FindBy(string name, DataOperationUnit dataOpUnit = null)
        {
            AuthorDao dao = new AuthorDao();
            return dao.FindBy(new Dictionary<string, object>() { { "Name", name } }, dataOpUnit?.CurrentConnection);
        }

        public static bool Exists(Guid authorID, DataOperationUnit dataOpUnit = null)
        {
            AuthorDao dao = new AuthorDao();
            return dao.CountBy(new Dictionary<string, object>() { { "ID", authorID } }, dataOpUnit?.CurrentConnection) > 0;
        }

        public static bool Exists(string name, DataOperationUnit dataOpUnit = null)
        {
            AuthorDao dao = new AuthorDao();
            return dao.CountBy(new Dictionary<string, object>() { { "Name", name } }, dataOpUnit?.CurrentConnection) > 0;
        }

        public static void Update(AuthorViewModel target)
        {
            AuthorDao dao = new AuthorDao();
            dao.Update(target.ToEntity());
            s_logger.Debug($"UPDATE Author:{target}");
        }
    }
}
