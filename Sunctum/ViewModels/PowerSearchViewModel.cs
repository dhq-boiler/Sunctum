

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Sunctum.ViewModels
{
    internal class PowerSearchViewModel : BindableBase, IDisposable, IDialogAware
    {
        private IArrangedBookStorage _bookStorage;
        private bool disposedValue;

        public event Action<IDialogResult> RequestClose;

        public PowerSearchViewModel()
        {
            RegisterCommands();
        }

        public ReactiveProperty<Guid> BookId { get; } = new ReactiveProperty<Guid>();

        public ReactiveProperty<string> Author { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> BookTitle { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<int> ConditionStar { get; } = new ReactiveProperty<int>();

        public ICommand SearchCommand { get; set; }

        public ICommand CloseCommand { get; set; }

        public string Title => "パワーサーチ";

        private void RegisterCommands()
        {
            SearchCommand = new DelegateCommand(() => Search());
            CloseCommand = new DelegateCommand(() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel)));
        }

        private void Search()
        {
            var books = _bookStorage.BookSource;

            IEnumerable<BookViewModel> filtered = from book in books
                                                  select book;

            if (BookId.Value != Guid.Empty)
            {
                filtered = from book in filtered
                           where book.ID == BookId.Value
                           select book;
            }

            if (!string.IsNullOrWhiteSpace(Author.Value))
            {
                filtered = from book in filtered
                           where book.Author != null && book.Author.Name.IndexOf(Author.Value) != -1
                           select book;
            }

            if (!string.IsNullOrWhiteSpace(BookTitle.Value))
            {
                filtered = from book in filtered
                           where book.Title.IndexOf(BookTitle.Value) != -1
                           select book;
            }

            switch (ConditionStar.Value)
            {
                case 0:
                    filtered = from book in filtered
                               select book;
                    break;
                case 1:
                    filtered = from book in filtered
                               where !book.StarLevel.HasValue
                               select book;
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    filtered = from book in filtered
                               where book.StarLevel.HasValue && book.StarLevel.Value == ConditionStar.Value - 1
                               select book;
                    break;
            }


            _bookStorage.SearchedBooks = new ReactiveCollection<BookViewModel>();
            _bookStorage.SearchedBooks.AddRange(filtered.ToList());
            _bookStorage.SearchedBooks = _bookStorage.SearchedBooks;

            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
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
            _bookStorage = parameters.GetValue<IArrangedBookStorage>("Storage");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    BookId.Dispose();
                    Author.Dispose();
                    BookTitle.Dispose();
                    ConditionStar.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
