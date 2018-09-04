

using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Reactive.Bindings;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Sunctum.ViewModels
{
    internal class PowerSearchViewModel : IInteractionRequestAware
    {
        public Action FinishInteraction { get; set; }

        private INotification _notification;
        private IArrangedBookStorage _bookStorage;

        public PowerSearchViewModel()
        {
            RegisterCommands();
        }

        public INotification Notification
        {
            get
            {
                return _notification;
            }

            set
            {
                _notification = value;
                _bookStorage = value.Content as IArrangedBookStorage;
            }
        }

        public ReactiveProperty<Guid> BookId { get; } = new ReactiveProperty<Guid>();

        public ReactiveProperty<string> Author { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> Title { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<int> ConditionStar { get; } = new ReactiveProperty<int>();

        public ICommand SearchCommand { get; set; }

        public ICommand CloseCommand { get; set; }

        private void RegisterCommands()
        {
            SearchCommand = new DelegateCommand(() => Search());
            CloseCommand = new DelegateCommand(() => FinishInteraction());
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

            if (!string.IsNullOrWhiteSpace(Title.Value))
            {
                filtered = from book in filtered
                           where book.Title.IndexOf(Title.Value) != -1
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


            _bookStorage.SearchedBooks = new ObservableCollection<BookViewModel>(filtered.ToList());
        }
    }
}
