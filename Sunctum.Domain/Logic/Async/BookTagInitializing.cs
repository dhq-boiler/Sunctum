

using NLog;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Sunctum.Domain.Logic.Async
{
    public class BookTagInitializing : AsyncTaskMakerBase, IBookTagInitializing
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private IEnumerable<ImageTagViewModel> _imageTags;

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start BookTagInitializing"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            var bookImageChains = new IntermediateTableDao().FindAll().ToList();
            foreach (var chain in bookImageChains)
            {
                sequence.Add(() =>
                {
                    _imageTags = ImageTagFacade.FindByImageId(chain.ImageId).ToList();
                });

                sequence.Add(() =>
                {
                    foreach (var imageTag in _imageTags)
                    {
                        var newEntity = new BookTagViewModel(chain.BookId, imageTag.TagID);
                        if (!BookTagFacade.Exists(newEntity))
                        {
                            BookTagFacade.Insert(newEntity);
                        }
                    }
                });
            }
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish BookTagInitializing"));
        }
    }
}
