﻿

using Ninject;
using Prism.Commands;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace Sunctum.ViewModels
{
    public class InformationPaneViewModel : PaneViewModelBase, IInformationPaneViewModel
    {
        private List<TagViewModel> _TagListBoxSelectedItems;

        [Inject]
        public IMainWindowViewModel MainWindowViewModel { get; set; }

        [Inject]
        public ITagManager TagManager { get; set; }

        public override string Title
        {
            get { return "Information"; }
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
                MainWindowViewModel.DisplayInformationPane = false;
            });
            DeleteTagEntryCommand = new DelegateCommand(() =>
            {
                var items = TagListBoxSelectedItems;
                foreach (var item in items)
                {
                    MainWindowViewModel.LibraryVM.TagMng.RemoveImageTag(item.Name);
                }
            });
            DropTagCommand = new DelegateCommand<IDataObject>(data =>
            {
                var imageTagCount = (TagCountViewModel)data.GetData(typeof(TagCountViewModel));
                try
                {
                    MainWindowViewModel.LibraryVM.TagMng.AddImageTagToSelectedObject(imageTagCount.Tag.Name);
                }
                catch (ArgumentException)
                {
                    SystemSounds.Beep.Play();
                }
            });
            TagPlusCommand = new DelegateCommand<string>(async text =>
            {
                await MainWindowViewModel.LibraryVM.TagMng.AddImageTagToSelectedObject(text);
            });
            TagMinusCommand = new DelegateCommand(() =>
            {
                var items = TagListBoxSelectedItems;
                foreach (var item in items)
                {
                    MainWindowViewModel.LibraryVM.TagMng.RemoveImageTag(item.Name);
                }
            });
        }
    }
}
