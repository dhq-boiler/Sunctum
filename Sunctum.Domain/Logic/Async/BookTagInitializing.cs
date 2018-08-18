

using NLog;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.ViewModels;
using System.Linq;

namespace Sunctum.Domain.Logic.Async
{
    public class BookTagInitializing : AsyncTaskMakerBase, IBookTagInitializing
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start BookTagInitializing"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            var bookTags = new IntermediateTableDao().FindAll().ToList();
            foreach (var chain in bookTags)
            {
                sequence.Add(() =>
                {
                    var newEntity = new BookTagViewModel(chain.BookID, chain.TagID);
                    if (!BookTagFacade.Exists(newEntity))
                    {
                        BookTagFacade.Insert(newEntity);
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
