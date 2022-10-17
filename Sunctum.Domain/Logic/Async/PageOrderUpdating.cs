

using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.PageSorting;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace Sunctum.Domain.Logic.Async
{
    public class PageOrderUpdating : AsyncTaskMakerBase, IPageOrderUpdating
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private int _index;

        public BookViewModel Target { get; set; }

        [Dependency]
        public Lazy<ILibrary> Library { get; set; }

        [Dependency]
        public Lazy<IMainWindowViewModel> MainWindowViewModel { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start PageOrderUpdating"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            sequence.Add(() => _index = 0);

            for (int i = 0; i < Target.Contents.Count; ++i)
            {
                sequence.Add(async () => await UpdatePageOrderIf(_index++));
            }

            sequence.Add(() =>
            {
                if (Target.Contents.Count > 0)
                {
                    Target.FirstPage.Value = Target.Contents.First();
                }
            });

            sequence.Add(() =>
            {
                if (MainWindowViewModel.Value.ActiveDocumentViewModel is not null)
                {
                    MainWindowViewModel.Value.ActiveDocumentViewModel.BookCabinet.UpdateInMemory(Target);
                }
                Library.Value.UpdateInMemory(Target);
            });
        }

        private async Task UpdatePageOrderIf(int i)
        {
            var page = Target.Contents[i];
            if (page.PageIndex != i + 1)
            {
                page.PageIndex = i + 1;
                await PageFacade.UpdateAsync(page);
            }
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish PageOrderUpdating"));
        }
    }
}
