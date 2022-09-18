using Homura.ORM;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Encrypt;
using Sunctum.Domain.Models;
using Sunctum.Domain.Util;
using Sunctum.Domain.ViewModels;
using System;
using System.IO;

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
                throw new FileNotFoundException(Target.AbsoluteMasterPath);
            }
            sequence.Add(() =>
            {
                thumbnail = new ThumbnailViewModel();
                thumbnail.ID = Target.ID;
                thumbnail.ImageID = Target.ID;
                var encryptImage = EncryptImageFacade.FindBy(Target.ID);
                if (encryptImage != null && !string.IsNullOrWhiteSpace(Configuration.ApplicationConfiguration.Password))
                {
                    Encryptor.Decrypt(encryptImage.EncryptFilePath, Configuration.ApplicationConfiguration.Password, true);
                    thumbnail.RelativeMasterPath = $"{Path.GetDirectoryName(Target.RelativeMasterPath)}\\{Target.ID.ToString("N")}{Path.GetExtension(Target.RelativeMasterPath)}";
                }
                else
                {
                    try
                    {
                        thumbnail.RelativeMasterPath = ThumbnailGenerator.SaveThumbnail(Target.AbsoluteMasterPath, Target.ID.ToString("N") + System.IO.Path.GetExtension(Target.AbsoluteMasterPath));
                        s_logger.Debug($"Generate thumbnail ImageID={Target.ID}");
                    }
                    catch (Exception e)
                    {
                        s_logger.Warn(e);
                        throw;
                    }
                }
                RecordThumbnail(thumbnail);
                Target.Thumbnail = thumbnail;
            });
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish ThumbnailGenerating"));
        }

        private static void RecordThumbnail(ThumbnailViewModel thumbnail, DataOperationUnit dataOpUnit = null)
        {
            try
            {
                ThumbnailFacade.InsertOrReplace(thumbnail, dataOpUnit);
                s_logger.Debug($"Recorded Thumbnail into database. {thumbnail.ToString()}");
            }
            catch (Exception e)
            {
                s_logger.Error(e);
            }
        }
    }
}
