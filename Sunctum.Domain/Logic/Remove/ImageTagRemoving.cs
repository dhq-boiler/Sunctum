

using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System.Linq;

namespace Sunctum.Domain.Logic.Remove
{
    public class ImageTagRemoving
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static void Remove(ITagManager tagMng, ImageViewModel targetImage, string tagName)
        {
            bool tagExists = TagFacade.Exists(tagName);
            if (tagExists)
            {
                var tag = TagFacade.FindByTagName(tagName);
                ImageTagFacade.DeleteWhereIDIs(targetImage.ID, tag.ID);
                var willRemoves = tagMng.Chains.Where(a => a.ImageID == targetImage.ID && a.Tag.ID == tag.ID);
                if (willRemoves.Count() == 1)
                {
                    var willRemove = willRemoves.Single();
                    tagMng.Chains.Remove(willRemove);
                }
                s_logger.Info($"Removed ImageTag:{tag}");
            }
        }
    }
}
