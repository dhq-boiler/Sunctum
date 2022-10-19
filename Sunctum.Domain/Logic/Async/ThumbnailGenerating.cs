using Homura.ORM;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Encrypt;
using Sunctum.Domain.Models;
using Sunctum.Domain.Util;
using Sunctum.Domain.ViewModels;
using System;
using System.IO;
using System.Windows;

namespace Sunctum.Domain.Logic.Async
{
    public class ThumbnailGenerating : AsyncTaskMakerBase, IThumbnailGenerating
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public ImageViewModel Target { get; set; }
        private ThumbnailViewModel thumbnail { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Begin ThumbnailGenerating"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            //暗号化しておらず、ファイルが存在しない場合
            if (!Target.IsEncrypted && !File.Exists(Target.AbsoluteMasterPath))
            {
                //Do nothing
            }
            else
            {
                sequence.Add(async () =>
                {
                    thumbnail = new ThumbnailViewModel();
                    thumbnail.ID = Target.ID;
                    thumbnail.ImageID = Target.ID;
                    var encryptImage = await EncryptImageFacade.FindByAsync(Target.ID);
                    if (encryptImage != null && !string.IsNullOrWhiteSpace(Configuration.ApplicationConfiguration.Password))
                    {
                        await Encryptor.Decrypt(Configuration.ApplicationConfiguration.WorkingDirectory + encryptImage.EncryptFilePath, Configuration.ApplicationConfiguration.Password, true).ConfigureAwait(false);
                        thumbnail.RelativeMasterPath = $"{Path.GetDirectoryName(Target.RelativeMasterPath)}\\{Target.ID.ToString("N")}{Path.GetExtension(Target.RelativeMasterPath)}";
                    }
                    else
                    {
                        try
                        {
                            thumbnail.RelativeMasterPath = await ThumbnailGenerator.SaveThumbnail(Target.AbsoluteMasterPath, Target.ID.ToString("N") + System.IO.Path.GetExtension(Target.AbsoluteMasterPath)).ConfigureAwait(false);
                            s_logger.Debug($"Generate thumbnail ImageID={Target.ID}");
                        }
                        catch (Exception e)
                        {
                            s_logger.Warn(e);
                            throw;
                        }
                    }
                    Target.Thumbnail = thumbnail;
                    await Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        var tr = new Async.ThumbnailRecording();
                        tr.Target = thumbnail;
                        await (Application.Current.MainWindow.DataContext as IMainWindowViewModel).LibraryVM.TaskManager.Enqueue(tr.GetTaskSequence()).ConfigureAwait(false);
                    });
                });
            }
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish ThumbnailGenerating"));
        }
    }
}
