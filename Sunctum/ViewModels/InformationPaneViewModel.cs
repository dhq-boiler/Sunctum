using Prism.Commands;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Input;
using Unity;

namespace Sunctum.ViewModels
{
    public class InformationPaneViewModel : PaneViewModelBase, IInformationPaneViewModel
    {
        private List<TagViewModel> _TagListBoxSelectedItems;

        [Dependency]
        public Lazy<IMainWindowViewModel> MainWindowViewModel { get; set; }

        [Dependency]
        public ITagManager TagManager { get; set; }

        public override string Title
        {
            get { return "Information"; }
            set { }
        }

        public override string ContentId
        {
            get { return "information"; }
        }

        public override bool CanClose => true;

        public List<TagViewModel> TagListBoxSelectedItems
        {
            get { return _TagListBoxSelectedItems; }
            set { SetProperty(ref _TagListBoxSelectedItems, value); }
        }

        public ICommand CloseCommand { get; set; }

        public ICommand DeleteTagEntryCommand { get; set; }

        public ICommand DropTagCommand { get; set; }

        public ICommand TagPlusCommand { get; set; }

        public ICommand TagMinusCommand { get; set; }

        public InformationPaneViewModel()
        {
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            CloseCommand = new DelegateCommand(() =>
            {
                MainWindowViewModel.Value.DisplayInformationPane = false;
            });
            DeleteTagEntryCommand = new DelegateCommand(() =>
            {
                var items = TagListBoxSelectedItems;
                foreach (var item in items)
                {
                    TagManager.RemoveImageTag(item.Name);
                }
            });
            DropTagCommand = new DelegateCommand<DragEventArgs>(data =>
            {
                var d = data.Data;
                var imageTagCount = (TagCountViewModel)d.GetData(typeof(TagCountViewModel));
                try
                {
                    TagManager.AddImageTagToSelectedObject(imageTagCount.Tag.Name);
                }
                catch (ArgumentException)
                {
                    SystemSounds.Beep.Play();
                }
            });
            TagPlusCommand = new DelegateCommand<string>(async text =>
            {
                await TagManager.AddImageTagToSelectedObject(text);
            });
            TagMinusCommand = new DelegateCommand(() =>
            {
                var items = TagListBoxSelectedItems;
                foreach (var item in items)
                {
                    TagManager.RemoveImageTag(item.Name);
                }
            });
        }
    }
}
