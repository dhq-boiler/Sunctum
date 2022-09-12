

using Reactive.Bindings;
using Sunctum.Domain.Models.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shell;

namespace Sunctum.Domain.ViewModels
{
    public interface IMainWindowViewModel : IDisposable
    {
        Task Initialize(bool starting, bool shiftPressed);

        void Terminate();

        void Close();

        ILibrary LibraryVM { get; }

        bool DisplayInformationPane { get; set; }

        bool DisplayTagPane { get; set; }

        bool DisplayAuthorPane { get; set; }

        double WindowLeft { get; set; }

        double WindowTop { get; set; }

        double WindowWidth { get; set; }

        double WindowHeight { get; set; }

        void ShowPreferenceDialog();

        ObservableCollection<IDocumentViewModelBase> DockingDocumentViewModels { get; }

        IDocumentViewModelBase ActiveDocumentViewModel { get; }

        void NewSearchTab(ObservableCollection<BookViewModel> onStage);

        void CloseTab(IDocumentViewModelBase documentViewModelBase);

        void NewContentTab(BookViewModel bookViewModel);

        void NewContentTab(IEnumerable<BookViewModel> list);

        void SaveLayout();

        List<MenuItem> ExtraBookContextMenu { get; }

        List<MenuItem> ExtraPageContextMenu { get; }

        List<MenuItem> ExtraTagContextMenu { get; }

        List<MenuItem> ExtraAuthorContextMenu { get; }

        ReactivePropertySlim<TaskbarItemInfo> TaskbarItemInfo { get; }
        IHomeDocumentViewModel HomeDocumentViewModel { get; set; }

        void InitializeWindowComponent();
        void ManageAppDB();
        void ManageVcDB();
    }
}
