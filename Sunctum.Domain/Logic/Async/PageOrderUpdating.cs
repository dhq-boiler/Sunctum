

using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.ViewModels;

namespace Sunctum.Domain.Logic.Async
{
    public class PageOrderUpdating : AsyncTaskMakerBase, IPageOrderUpdating
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private int _index;

        public BookViewModel Target { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start PageOrderUpdating"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            sequence.Add(() => _index = 0);

            for (int i = 0; i < Target.Contents.Count; ++i)
            {
                sequence.Add(() => UpdatePageOrderIf(_index++));
            }

            sequence.Add(() =>
            {
                if (Target.Contents.Count > 0)
                {
                    Target.FirstPage = Target.Contents[0];
                }
            });
        }

        private void UpdatePageOrderIf(int i)
        {
            var page = Target.Contents[i];
            if (page.PageIndex != i + 1)
            {
                page.PageIndex = i + 1;
                PageFacade.Update(page);
            }
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish PageOrderUpdating"));
        }
    }
}
