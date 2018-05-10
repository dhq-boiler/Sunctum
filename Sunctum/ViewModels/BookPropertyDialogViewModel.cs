

using Ninject;
using Prism.Mvvm;
using Reactive.Bindings;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.Infrastructure.Core;
using Sunctum.Properties;
using Sunctum.UI.Controls;
using Sunctum.UI.Dialogs;
using Sunctum.UI.ViewModel;
using Sunctum.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using static Sunctum.UI.Core.Extensions;

namespace Sunctum.ViewModels
{
    public class BookPropertyDialogViewModel : BindableBase
    {
        private BookViewModel _book;
        private List<AuthorViewModel> _AllAuthors;
        private int _SelectedAuthorIndex;

        [Inject]
        public ILibrary LibraryManager { get; set; }

        public ReactiveCommand SelectNextBookCommand { get; set; } = new ReactiveCommand();

        public ReactiveCommand SelectPreviousBookCommand { get; set; } = new ReactiveCommand();

        public ReactiveCommand<Window> OkCommand { get; set; } = new ReactiveCommand<Window>();

        public ReactiveCommand<Window> CancelCommand { get; set; } = new ReactiveCommand<Window>();

        public ReactiveCommand OpenSaveDirCommand { get; set; } = new ReactiveCommand();

        public ReactiveCommand OpenAuthorManagementDialogCommand { get; set; } = new ReactiveCommand();

        private VirtualizingStackPanel VSP_Contents { get { return Parent.Contents_ListView.GetVisualChild<VirtualizingStackPanel>(); } }

        public BookPropertyDialog Parent { get; set; }

        public BookPropertyDialogViewModel()
        {
            LoadAllAuthors();
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            OkCommand
                .Subscribe(dialog =>
                {
                    UpdateBook();
                    dialog.DialogResult = true;
                });
            CancelCommand
                .Subscribe(dialog =>
                {
                    dialog.DialogResult = false;
                });
            SelectNextBookCommand
                .Subscribe(() => SelectNextBook());
            SelectPreviousBookCommand
                .Subscribe(() => SelectPreviousBook());
            OpenSaveDirCommand
                .Subscribe(() => OpenDir());
            OpenAuthorManagementDialogCommand
                .Subscribe(() =>
                {
                    var dialog = new EntityManagementDialog<AuthorViewModel>();
                    var dialogViewModel = new EntityManagementDialogViewModel<AuthorViewModel>(dialog, LibraryManager, "Authorの管理",
                        new Func<string, AuthorViewModel>((name) =>
                        {
                            var author = new AuthorViewModel();
                            author.ID = Guid.NewGuid();
                            author.UnescapedName = name;
                            AuthorFacade.Create(author);
                            return author;
                        }),
                        new Func<IEnumerable<AuthorViewModel>>(() =>
                        {
                            return AuthorFacade.OrderByNaturalString();
                        }),
                        new Func<Guid, AuthorViewModel>((id) =>
                        {
                            return AuthorFacade.FindBy(id);
                        }),
                        new Action<AuthorViewModel>((target) =>
                        {
                            AuthorFacade.Update(target);
                            var willUpdate = LibraryManager.BookSource.Where(b => b.AuthorID == target.ID);
                            foreach (var x in willUpdate)
                            {
                                x.Author = target.Clone() as AuthorViewModel;
                            }
                        }),
                        new Action<Guid>((id) =>
                        {
                            AuthorFacade.Delete(id);
                            var willUpdate = LibraryManager.BookSource.Where(b => b.AuthorID == id);
                            foreach (var x in willUpdate)
                            {
                                x.Author = null;
                            }
                        }),
                        new Action<AuthorViewModel, AuthorViewModel>((willDiscard, into) =>
                        {
                            AuthorFacade.Delete(willDiscard.ID);
                            var willUpdate = LibraryManager.BookSource.Where(b => b.AuthorID == willDiscard.ID);
                            foreach (var x in willUpdate)
                            {
                                x.Author = into.Clone() as AuthorViewModel;
                                BookFacade.Update(x);
                            }
                        }));
                    dialog.EntityMngVM = dialogViewModel;
                    dialogViewModel.Initialize();
                    dialog.Show();
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
            ContentsLoadTask.FillContentsWithImage(LibraryManager, Book);
        }

        public void LoadAllAuthors()
        {
            AllAuthors = AuthorFacade.OrderByNaturalString().ToList();
        }

        public string DialogTitle
        {
            get
            {
                if (Book == null)
                {
                    return "ロード中...";
                }
                return string.Format(Resources.BookPropertyDialogTitle, Book?.Author?.UnescapedName, Book?.UnescapedTitle);
            }
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
                if (Book == null)
                {
                    return "ロード中...";
                }
                return Path.GetDirectoryName(Book.FirstPage.Image.AbsoluteMasterPath);
            }
        }

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

            if (LibraryManager.IsDirty(Book))
            {
                LibraryManager.UpdateInMemory(Book);
            }
        }

        public void SelectBook(BookViewModel book, bool initialized = false)
        {
            Book = book;
            RefleshBook();
            if (initialized)
            {
                SelectAllIfFocusedTitleTextBox();
                ShowFirstPage();
            }
        }

        public void SelectPreviousBook()
        {
            UpdateBook();
            int index = LibraryManager.OnStage.IndexOf(Book);
            if (index - 1 < 0)
            {
                SelectBook((BookViewModel)LibraryManager.OnStage.Last().Clone(), true);
            }
            else
            {
                SelectBook((BookViewModel)LibraryManager.OnStage[index - 1].Clone(), true);
            }
        }

        public void SelectNextBook()
        {
            UpdateBook();
            int index = LibraryManager.OnStage.IndexOf(Book);
            int newIndex = index + 1;
            if (newIndex > LibraryManager.OnStage.Count() - 1)
            {
                SelectBook((BookViewModel)LibraryManager.OnStage.First().Clone(), true);
            }
            else
            {
                SelectBook((BookViewModel)LibraryManager.OnStage[newIndex].Clone(), true);
            }
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
