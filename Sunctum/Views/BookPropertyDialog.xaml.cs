

using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using Sunctum.UI.Dialogs;
using Sunctum.UI.ViewModel;
using Sunctum.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Sunctum.Views
{
    /// <summary>
    /// Interaction logic for BookPropertyDialog.xaml
    /// </summary>
    public partial class BookPropertyDialog : Window
    {
        public BookPropertyDialogViewModel DialogVM { get; set; }

        public BookViewModel DirtyBook { get { return DialogVM.Book; } }

        private ILibraryManager _libVM;

        public BookPropertyDialog(BookViewModel book, ILibraryManager libVM)
        {
            InitializeComponent();
            _libVM = libVM;
            DataContext = DialogVM = new BookPropertyDialogViewModel(this, book, libVM);
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            DialogVM.OpenDir();
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogVM.UpdateBook();
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OpenAuthorManagementDialog_Click(object sender, RoutedEventArgs e)
        {
            EntityManagementDialog<AuthorViewModel> dialog = new EntityManagementDialog<AuthorViewModel>();
            EntityManagementDialogViewModel<AuthorViewModel> dialogViewModel = new EntityManagementDialogViewModel<AuthorViewModel>(dialog, _libVM, "Authorの管理",
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
                    var willUpdate = _libVM.LoadedBooks.Where(b => b.AuthorID == target.ID);
                    foreach (var x in willUpdate)
                    {
                        x.Author = target.Clone() as AuthorViewModel;
                    }
                }),
                new Action<Guid>((id) =>
                {
                    AuthorFacade.Delete(id);
                    var willUpdate = _libVM.LoadedBooks.Where(b => b.AuthorID == id);
                    foreach (var x in willUpdate)
                    {
                        x.Author = null;
                    }
                }),
                new Action<AuthorViewModel, AuthorViewModel>((willDiscard, into) =>
                {
                    AuthorFacade.Delete(willDiscard.ID);
                    var willUpdate = _libVM.LoadedBooks.Where(b => b.AuthorID == willDiscard.ID);
                    foreach (var x in willUpdate)
                    {
                        x.Author = into.Clone() as AuthorViewModel;
                        BookFacade.Update(x);
                    }
                }));
            dialog.EntityMngVM = dialogViewModel;
            dialogViewModel.Initialize();
            dialog.Show();
        }

        private void ComboBox_DropDownOpened(object sender, System.EventArgs e)
        {
            DialogVM.LoadAllAuthors();
        }

        private void Title_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    DialogVM.UpdateBook();
                    DialogResult = true;
                    break;
                case Key.Escape:
                    DialogResult = false;
                    Close();
                    break;
                default:
                    break;
            }
        }
    }
}
