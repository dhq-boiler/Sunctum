﻿using NLog;
using Sunctum.Domain.Logic.Async;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Plugin;
using System;
using System.Windows;
using Unity;

namespace FolderFilesImporter
{
    public class FolderFilesImporter : IDropPlugin
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public string AcceptableDataFormat => DataFormats.FileDrop;

        [Dependency]
        public IBookImporting BookImportingService { get; set; }

        [Dependency]
        public ITaskManager TaskManager { get; set; }

        public async void Execute(IDataObject dataObject)
        {
            if (dataObject.GetData(DataFormats.FileDrop) is string[] objects && objects.Length > 0)
            {
                try
                {
                    BookImportingService.MasterDirectory = Specifications.MASTER_DIRECTORY;
                    BookImportingService.ObjectPaths = objects;
                    await TaskManager.Enqueue(BookImportingService.GetTaskSequence());
                }
                catch (NullReferenceException e)
                {
                    s_logger.Error(e, "Failed to import.");
                }
                catch (Exception e)
                {
                    s_logger.Error(e, "Failed to import.");
                }
            }
        }
    }
}
