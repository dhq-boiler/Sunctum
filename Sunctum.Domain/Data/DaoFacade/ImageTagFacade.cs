

using Homura.ORM;
using NLog;
using Sunctum.Domain.Bridge;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Data.Rdbms.SQLite;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunctum.Domain.Data.DaoFacade
{
    public static class ImageTagFacade
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static void Insert(ImageTagViewModel target)
        {
            ImageTagDao dao = new ImageTagDao();
            dao.Insert(target.ToEntity());
            s_logger.Debug($"INSERT ImageTag:{target}");
        }

        public static void BatchInsert(TagViewModel tag, IEnumerable<ImageViewModel> images)
        {
            if (images == null || images.Count() == 0)
            {
                s_logger.Warn("No images set.");
                return;
            }

            using (var dataOpUnit = new DataOperationUnit())
            {
                dataOpUnit.Open(ConnectionManager.DefaultConnection);

                ImageTagDao dao = new ImageTagDao();
                Queue<List<ImageViewModel>> processPool = Split(images);

                foreach (var process in processPool)
                {
                    try
                    {
                        dao.BatchInsert(tag.ToEntity(), process.Select(i => i.ToEntity()), dataOpUnit.CurrentConnection);
                    }
                    catch (Exception e)
                    {
                        s_logger.Error(e);
                    }
                }
            }
        }

        private static Queue<List<ImageViewModel>> Split(IEnumerable<ImageViewModel> images)
        {
            Queue<ImageViewModel> targetPool = new Queue<ImageViewModel>(images);
            Queue<List<ImageViewModel>> processPool = new Queue<List<ImageViewModel>>();

            while (targetPool.Count() > 0)
            {
                if (processPool.Count == 0 || (processPool.Last().Count + 1) * 2 > SpecificationDefaults.SQLITE_MAX_VARIABLE_NUMBER)
                {
                    processPool.Enqueue(new List<ImageViewModel>());
                }

                var current = processPool.LastOrDefault();

                if ((current.Count + 1) * 2 <= SpecificationDefaults.SQLITE_MAX_VARIABLE_NUMBER)
                {
                    current.Add(targetPool.Dequeue());
                }
            }

            return processPool;
        }

        public static long CountAll()
        {
            var dao = new ImageTagDao();
            return dao.CountAll();
        }

        public static async Task DeleteWhereIDIs(Guid imageId, Guid tagId)
        {
            ImageTagDao dao = new ImageTagDao();
            await dao.DeleteAsync(new Dictionary<string, object>() { { "ImageID", imageId }, { "TagID", tagId } });
            s_logger.Debug($"DELETE ImageTag ImageId:{imageId}, TagId:{tagId}");
        }

        internal static void DeleteByTagName(string tagName)
        {
            ImageTagDao dao = new ImageTagDao();
            dao.DeleteByTagName(tagName);
        }

        public static IEnumerable<ImageTagViewModel> FindAll()
        {
            ImageTagDao dao = new ImageTagDao();
            return dao.FindAll().ToViewModel();
        }

        public static async IAsyncEnumerable<ImageTagViewModel> FindAllAsync()
        {
            ImageTagDao dao = new ImageTagDao();
            var items = await dao.FindAllAsync().ToListAsync();
            foreach (var item in items)
            {
                yield return item.ToViewModel();
            }
        }

        public static IEnumerable<ImageTagViewModel> FindByTagId(Guid tagId)
        {
            ImageTagDao dao = new ImageTagDao();
            return dao.FindByTagId(tagId).ToViewModel();
        }

        public static IEnumerable<ImageTagViewModel> FindByImageId(Guid imageId)
        {
            ImageTagDao dao = new ImageTagDao();
            return dao.FindBy(new Dictionary<string, object>() { { "ImageID", imageId } }).ToViewModel();
        }

        public static async IAsyncEnumerable<TagCountViewModel> FindAllAsCount()
        {
            ImageTagDao dao = new ImageTagDao();
            var items = await dao.FindAllAsTagCountAsync().ToListAsync();
            foreach (var item in items)
            {
                yield return item.ToViewModel();
            }
        }

        public static IEnumerable<ImageTagViewModel> FindByImageIds(IEnumerable<Guid> imageIds)
        {
            ImageTagDao dao = new ImageTagDao();
            return dao.FindBy(new Dictionary<string, object>() { { "ImageID", imageIds.ToArray() } }).ToViewModel();
        }

        public static async Task<int> CountByTagId(Guid tagId)
        {
            ImageTagDao dao = new ImageTagDao();
            return await dao.CountByAsync(new Dictionary<string, object>() { { "TagID", tagId } });
        }

        public static async Task DeleteWhereIDIs(Guid imageId)
        {
            ImageTagDao dao = new ImageTagDao();
            await dao.DeleteAsync(new Dictionary<string, object>() { { "ImageID", imageId } });
            s_logger.Debug($"DELETE ImageTag ImageId:{imageId}");
        }
    }
}
