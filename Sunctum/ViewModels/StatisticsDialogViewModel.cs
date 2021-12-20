using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Models.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace Sunctum.ViewModels
{
    internal class StatisticsDialogViewModel : BindableBase, IDialogAware
    {
        [Dependency]
        public ILibrary LibraryManager { get; set; }

        [Dependency]
        public IDataAccessManager DataAccessManager { get; set; }

        public DelegateCommand LoadedCommand { get; }

        public ReactivePropertySlim<int> NumberOfBoots { get; } = new ReactivePropertySlim<int>();

        public ReactivePropertySlim<int> NumberOfBooks { get; } = new ReactivePropertySlim<int>();

        public ReactivePropertySlim<long> TotalFileSize { get; } = new ReactivePropertySlim<long>();

        public ReactivePropertySlim<long> NumberOfPages { get; } = new ReactivePropertySlim<long>();

        public ReactivePropertySlim<long> NumberOfAuthors { get; } = new ReactivePropertySlim<long>();

        public ReactivePropertySlim<long> NumberOfTags { get; } = new ReactivePropertySlim<long>();

        public ReactivePropertySlim<long> NumberOfBookTags { get; } = new ReactivePropertySlim<long>();

        public ReactivePropertySlim<long> NumberOfImageTags { get; } = new ReactivePropertySlim<long>();

        public ReactivePropertySlim<int> NumberOfBooks5 { get; } = new ReactivePropertySlim<int>();

        public ReactivePropertySlim<int> NumberOfBooks4 { get; } = new ReactivePropertySlim<int>();

        public ReactivePropertySlim<int> NumberOfBooks3 { get; } = new ReactivePropertySlim<int>();

        public ReactivePropertySlim<int> NumberOfBooks2 { get; } = new ReactivePropertySlim<int>();

        public ReactivePropertySlim<int> NumberOfBooks1 { get; } = new ReactivePropertySlim<int>();

        public ReactivePropertySlim<int> NumberOfBooksN { get; } = new ReactivePropertySlim<int>();

        public ReactivePropertySlim<int> NumberOfDuplicateBooks { get; } = new ReactivePropertySlim<int>();

        public ReactivePropertySlim<long> TotalDuplicateBooksSize { get; } = new ReactivePropertySlim<long>();

        public string Title => "統計";

        public StatisticsDialogViewModel()
        {
            LoadedCommand = new DelegateCommand(() => Load());
        }

        public event Action<IDialogResult> RequestClose;

        public void Load()
        {
            var id = Guid.Parse("00000000-0000-0000-0000-000000000000");
            var appDao = DataAccessManager.AppDao.Build<StatisticsDao>();
            var statistics = appDao.FindBy(new Dictionary<string, object>() { { "ID", id } }).First();
            NumberOfBoots.Value = statistics.NumberOfBoots;

            NumberOfBooks.Value = BookFacade.FindAll().Count();

            TotalFileSize.Value = ImageFacade.SumTotalFileSize();

            NumberOfPages.Value = PageFacade.CountAll();

            NumberOfAuthors.Value = AuthorFacade.CountAll();

            NumberOfTags.Value = TagFacade.CountAll();

            NumberOfBookTags.Value = BookTagFacade.CountAll();

            NumberOfImageTags.Value = ImageTagFacade.CountAll();

            NumberOfBooks5.Value = StarFacade.FindBookByStar(5).Count();

            NumberOfBooks4.Value = StarFacade.FindBookByStar(4).Count();

            NumberOfBooks3.Value = StarFacade.FindBookByStar(3).Count();

            NumberOfBooks2.Value = StarFacade.FindBookByStar(2).Count();

            NumberOfBooks1.Value = StarFacade.FindBookByStar(1).Count();

            NumberOfBooksN.Value = StarFacade.FindBookByStar(null).Count();

            NumberOfDuplicateBooks.Value = BookFacade.FindDuplicateFingerPrint().Count();

            TotalDuplicateBooksSize.Value = BookFacade.FindDuplicateFingerPrint().Where(x => x.ByteSize != null).Select(x => x.ByteSize.Value).Sum();
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
