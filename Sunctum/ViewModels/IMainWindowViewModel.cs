

using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sunctum.ViewModels
{
    public interface IMainWindowViewModel
    {
        Task Initialize(bool starting, bool shiftPressed);

        Task ImportAsync(string[] objects);

        void OpenBook(BookViewModel book);

        void OpenImage(PageViewModel page);

        void CloseBook();

        void CloseImage();

        void GoPreviousImage();

        void GoNextImage();

        void OperateByKeyboard(KeyEventArgs e);

        void OperateByMouseButton(MouseButtonEventArgs e);

        void OperateByMouseWheel(MouseWheelEventArgs e);

        void MovePageBackward(PageViewModel page);

        void MovePageForward(PageViewModel page);

        Task SaveOpenedBookContentsOrder();

        void ResetScrollOffsetPool();

        void UpdateProgressBarLayout();

        void CloseSearchPane();

        void Search();

        void SetProgress(double progress);

        void Terminate();

        void Exit();

        void RestoreScrollOffset(Guid bookid);

        void SaveScrollOffset(Guid bookid);

        void ResetScrollOffset();

        void AddToSelectedEntry(EntryViewModel add);

        void AddToSelectedEntries(IEnumerable<EntryViewModel> add);

        void RemoveFromSelectedEntries(IEnumerable<EntryViewModel> entries);

        ILibraryManager LibraryVM { get; }

        bool DisplayInformationPane { get; set; }

        bool DisplayTagPane { get; set; }

        bool DisplayAuthorPane { get; set; }

        List<EntryViewModel> SelectedEntries { get; }

        string SearchText { get; set; }

        void FocusSearchBoxIf();

        //↓ICommand

        void ShowPreferenceDialog();
    }
}
