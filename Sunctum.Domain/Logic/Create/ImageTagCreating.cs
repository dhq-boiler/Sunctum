

using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Data.SQLite;

namespace Sunctum.Domain.Logic.Create
{
    public class ImageTagCreating
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static void Create(ITagManager tagMng, ImageViewModel targetImage, string tagName)
        {
            bool tagExists = TagFacade.Exists(tagName);
            if (tagExists)
            {
                var tag = TagFacade.FindByTagName(tagName);
                CreateImageTag(tagMng, targetImage, tag);
            }
            else
            {
                var newTag = CreateTag(tagMng, tagName);
                CreateImageTag(tagMng, targetImage, newTag);
            }
        }

        private static TagViewModel CreateTag(ITagManager tagMng, string tagName)
        {
            var newTag = new TagViewModel(Guid.NewGuid(), tagName);
            TagFacade.Insert(newTag);
            tagMng.Tags.Add(newTag);
            return newTag;
        }

        private static void CreateImageTag(ITagManager tagMng, ImageViewModel targetImage, TagViewModel tag)
        {
            var newImageTag = new ImageTagViewModel(targetImage.ID, tag);
            try
            {
                ImageTagFacade.Insert(newImageTag);
            }
            catch (SQLiteException e)
            {
                s_logger.Error(e);
            }
            if (!tagMng.Chains.Contains(newImageTag))
            {
                tagMng.Chains.Add(newImageTag);
            }
        }
    }
}
