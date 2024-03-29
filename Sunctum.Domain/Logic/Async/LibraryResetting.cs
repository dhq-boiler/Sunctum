﻿using NLog;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Linq;
using Unity;

namespace Sunctum.Domain.Logic.Async
{
    public class LibraryResetting : AsyncTaskMakerBase, IAsyncTaskMaker, ILibraryResetting
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        [Dependency]
        public Lazy<ILibrary> LibraryManager { get; set; }

        [Dependency]
        public Lazy<IMainWindowViewModel> mainWindowViewModel { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start LibraryResetting"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            sequence.Add(() => CleanUp());
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish LibraryResetting"));
        }

        private void CleanUp()
        {
            if (LibraryManager.Value.BookSource != null)
            {
                foreach (var book in LibraryManager.Value.BookSource)
                {
                    book.Dispose();
                }
                LibraryManager.Value.BookSource.Clear();
            }

            if (mainWindowViewModel.Value.DockingDocumentViewModels is not null)
            {
                mainWindowViewModel.Value.DockingDocumentViewModels.ToList().ForEach(x => x.SelectedEntries.Clear());
            }
        }
    }
}
