

using Homura.ORM;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Logic.Encrypt;
using Sunctum.Domain.Models;
using Sunctum.Domain.Util;
using Sunctum.Domain.ViewModels;
using System;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Threading;
using System.Threading.Tasks;

namespace Sunctum.Domain.Logic.Generate
{
    public static class ThumbnailGenerating
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static void GenerateThumbnail(ImageViewModel target, DataOperationUnit dataOpUnit = null)
        {
            try
            {
                while (!InternalGenerateThumbnail(target, dataOpUnit))
                {
                    Thread.Sleep(500);
                }
            }
            catch (FileNotFoundException)
            {
                s_logger.Warn($"Abort thumbnail generating. File not found:{target.AbsoluteMasterPath}");
            }
        }

        private static bool InternalGenerateThumbnail(ImageViewModel target, DataOperationUnit dataOpUnit)
        {
            lock (target)
            {
                //暗号化しておらず、ファイルが存在しない場合
                if (!target.IsEncrypted && !File.Exists(target.AbsoluteMasterPath))
                {
                    throw new FileNotFoundException(target.AbsoluteMasterPath);
                }

                var thumbnail = new ThumbnailViewModel();
                thumbnail.ID = target.ID;
                thumbnail.ImageID = target.ID;

                var encryptImage = EncryptImageFacade.FindBy(target.ID);
                if (encryptImage != null && !string.IsNullOrWhiteSpace(Configuration.ApplicationConfiguration.Password))
                {
                    Encryptor.Decrypt(encryptImage.EncryptFilePath, Configuration.ApplicationConfiguration.Password, true);
                    thumbnail.RelativeMasterPath = $"{Path.GetDirectoryName(target.RelativeMasterPath)}\\{target.ID.ToString("N")}{Path.GetExtension(target.RelativeMasterPath)}";
                }
                else
                {
                    try
                    {
                        thumbnail.RelativeMasterPath = ThumbnailGenerator.SaveThumbnail(target.AbsoluteMasterPath, target.ID.ToString("N") + System.IO.Path.GetExtension(target.AbsoluteMasterPath));
                    }
                    catch (Exception e)
                    {
                        s_logger.Warn(e);
                        return false;
                    }

                    s_logger.Debug($"Generate thumbnail ImageID={target.ID}");
                }

                Task.Factory.StartNew(() => RecordThumbnail(thumbnail));

                //Apply thumbnail
                target.Thumbnail = thumbnail;
                return true;
            }
        }

        private static void RecordThumbnail(ThumbnailViewModel thumbnail)
        {
            try
            {
                ThumbnailFacade.InsertOrReplace(thumbnail);
                s_logger.Debug($"Recorded Thumbnail into database. {thumbnail.ToString()}");
            }
            catch (Exception e)
            {
                s_logger.Error(e);
            }
        }
    }
}
