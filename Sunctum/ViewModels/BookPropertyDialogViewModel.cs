

using Prism.Commands;
using Prism.Mvvm;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.Infrastructure.Core;
using Sunctum.Properties;
using Sunctum.UI.Controls;
using Sunctum.Views;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using static Sunctum.UI.Core.Extensions;

namespace Sunctum.ViewModels
{
    public class BookPropertyDialogViewModel : BindableBase
    {
        private BookViewModel _book;
        private ILibraryManager _libVM;
        private List<AuthorViewModel> _AllAuthors;
        private int _SelectedAuthorIndex;

        public ICommand SelectNextBookCommand { get; set; }

        public ICommand SelectPreviousBookCommand { get; set; }

        private VirtualizingStackPanel VSP_Contents { get { return Parent.Contents_ListView.GetVisualChild<VirtualizingStackPanel>(); } }

        public BookPropertyDialogViewModel(BookPropertyDialog dialog, BookViewModel book, ILibraryManager libVM)
        {
            Parent = dialog;
            Book = (BookViewModel)book.Clone();
            _libVM = libVM;
            LoadAllAuthors();
            RefleshBook();
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            SelectNextBookCommand = new DelegateCommand(() =>
            {
                SelectNextBook();
            });
            SelectPreviousBookCommand = new DelegateCommand(() =>
            {
                SelectPreviousBook();
            });
        }

        private void RefleshBook()
        {
            PrepareBookContents();
            AuthorIndex = _AllAuthors.FindIndex(a => a.ID.Equals(Book.AuthorID));
        }

        private void PrepareBookContents()
        {
            BookLoading.Load(Book);
            ContentsLoadTask.FillContentsWithImage(_libVM, Book);
        }

        public void LoadAllAuthors()
        {
            AllAuthors = AuthorFacade.OrderByNaturalString().ToList();
        }

        public string DialogTitle
        {
            get { return string.Format(Resources.BookPropertyDialogTitle, Book.Author?.UnescapedName, Book.UnescapedTitle); }
        }

        public BookViewModel Book
        {
            [DebuggerStepThrough]
            get
            { return _book; }
            set
            {
                SetProperty(ref _book, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => AuthorIndex));
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => DialogTitle));
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => SaveDir));
            }
        }

        public List<AuthorViewModel> AllAuthors
        {
            [DebuggerStepThrough]
            get
            { return _AllAuthors; }
            set
            {
                SetProperty(ref _AllAuthors, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => DialogTitle));
            }
        }

        public int AuthorIndex
        {
            [DebuggerStepThrough]
            get
            { return _SelectedAuthorIndex; }
            set
            {
                SetProperty(ref _SelectedAuthorIndex, value);
                RaisePropertyChanged(PropertyNameUtility.GetPropertyName(() => DialogTitle));
            }
        }

        public string SaveDir
        {
            get
            {
                return Path.GetDirectoryName(Book.FirstPage.Image.AbsoluteMasterPath);
            }
        }

        public BookPropertyDialog Parent { get; private set; }

        internal void OpenDir()
        {
            Process.Start(SaveDir);
        }

        internal void UpdateBook()
        {
            if (AuthorIndex >= 0 && AuthorIndex < AllAuthors.Count)
            {
                Book.Author = AllAuthors[AuthorIndex];
            }
            else
            {
                Book.Author = null;
            }

            if (_libVM.IsDirty(Book))
            {
                _libVM.UpdateInMemory(Book);
            }
        }

        public void SelectPreviousBook()
        {
            UpdateBook();
            int index = _libVM.OnStage.IndexOf(Book);
            if (index - 1 < 0)
            {
                Book = (BookViewModel)_libVM.OnStage.Last().Clone();
            }
            else
            {
                Book = (BookViewModel)_libVM.OnStage[index - 1].Clone();
            }
            RefleshBook();
            SelectAllIfFocusedTitleTextBox();
            ShowFirstPage();
        }

        public void SelectNextBook()
        {
            UpdateBook();
            int index = _libVM.OnStage.IndexOf(Book);
            int newIndex = index + 1;
            if (newIndex > _libVM.OnStage.Count() - 1)
            {
                Book = (BookViewModel)_libVM.OnStage.First().Clone();
            }
            else
            {
                Book = (BookViewModel)_libVM.OnStage[newIndex].Clone();
            }
            RefleshBook();
            SelectAllIfFocusedTitleTextBox();
            ShowFirstPage();
        }

        private void ShowFirstPage()
        {
            VSP_Contents.SetOffset(new System.Windows.Point(0, 0));
        }

        private void SelectAllIfFocusedTitleTextBox()
        {
            var titleTextBox = Parent.Title_TextBox;
            if (titleTextBox.IsFocused)
            {
                titleTextBox.SelectAll();
            }
        }
    }
}
