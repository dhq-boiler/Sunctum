

using NLog;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Sunctum.Domain.Models;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Sunctum.Domain.Util
{
    public class ThumbnailGenerator
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static string SaveThumbnail(string srcFilename, string destFilename)
        {
            try
            {
                using (Bitmap small = ScaleDown(srcFilename))
                {
                    var ext = Path.GetExtension(destFilename).ToLower();
                    var quality = Configuration.ApplicationConfiguration.ThumbnailQuality;

                    switch (ext)
                    {
                        case ".jpeg":
                        case ".jpg":
                            return SaveImageAsJPEG(destFilename, small, quality);
                        case ".png":
                            return SaveImageAsPNG(destFilename, small, quality);
                        case ".gif":
                            return SaveImageAsGIF(destFilename, small, quality);
                        default:
                            throw new NotSupportedException("Support in JPG, PNG and GIF");
                    }
                }
            }
            catch (Exception e)
            {
                s_logger.Error(e);
                throw;
            }
        }

        public static Bitmap ScaleDown(string filename)
        {
            try
            {
                using (var src = new Mat(filename, ImreadModes.Unchanged))
                {
                    int width, height;

                    if (src.Width > src.Height)
                    {
                        width = 300;
                        height = (int)(300.0 / src.Width * src.Height);
                    }
                    else
                    {
                        height = 300;
                        width = (int)(300.0 / src.Height * src.Width);
                    }
                    src.Resize(new OpenCvSharp.Size(width, height), 0, 0, OpenCvSharp.InterpolationFlags.Lanczos4);
                    return BitmapConverter.ToBitmap(src);
                }
            }
            catch (IOException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        public static MemoryStream ScaleDownAndSaveAndToMemoryStream(Stream stream, string filename)
        {
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var src = Mat.FromStream(stream, ImreadModes.Unchanged))
                {
                    int width, height;

                    if (src.Width > src.Height)
                    {
                        width = 300;
                        height = (int)(300.0 / src.Width * src.Height);
                    }
                    else
                    {
                        height = 300;
                        width = (int)(300.0 / src.Height * src.Width);
                    }
                    src.Resize(new OpenCvSharp.Size(width, height), 0, 0, OpenCvSharp.InterpolationFlags.Lanczos4);
                    return src.ToMemoryStream(ext: Path.GetExtension(filename));
                }
            }
            catch (IOException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }

            return null;
        }

        private static string SaveImageAsJPEG(string destFilename, Bitmap bmp, int quality)
        {
            try
            {
                using (EncoderParameters eps = new EncoderParameters(1))
                {
                    using (EncoderParameter ep = new EncoderParameter(Encoder.Quality, (long)quality))
                    {
                        eps.Param[0] = ep;

                        ImageCodecInfo ici = GetEncoderInfo("image/jpeg");

                        string newFileName = GetAbsoluteCacheFilePath(destFilename);
                        CreateCacheDirectoryIfDoesntExist();

                        SaveBitmap(bmp, eps, ici, newFileName);

                        return GetRelativeCacheFilePath(destFilename);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void SaveBitmap(Bitmap bmp, EncoderParameters eps, ImageCodecInfo ici, string newFileName)
        {
            Bitmap s;
            using (Bitmap t = new Bitmap(bmp))
            {
                s = (Bitmap)t.Clone();
            }
            using (s)
            {
                s.Save(newFileName, ici, eps);
            }
        }

        private static void CreateCacheDirectoryIfDoesntExist()
        {
            if (!Directory.Exists(AbsoluteCacheDirectory))
            {
                Directory.CreateDirectory(AbsoluteCacheDirectory);
                s_logger.Debug($"Create directory:{AbsoluteCacheDirectory}");
            }
        }

        private static string SaveImageAsPNG(string destFilename, Bitmap bmp, int quality)
        {
            try
            {
                using (EncoderParameters eps = new EncoderParameters(1))
                {
                    using (EncoderParameter ep = new EncoderParameter(Encoder.Quality, (long)quality))
                    {
                        eps.Param[0] = ep;

                        ImageCodecInfo ici = GetEncoderInfo("image/png");

                        string newFileName = GetAbsoluteCacheFilePath(destFilename);
                        CreateCacheDirectoryIfDoesntExist();
                        SaveBitmap(bmp, eps, ici, newFileName);

                        return GetRelativeCacheFilePath(destFilename);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string SaveImageAsGIF(string destFilename, Bitmap bmp, int quality)
        {
            try
            {
                using (EncoderParameters eps = new EncoderParameters(1))
                {
                    using (EncoderParameter ep = new EncoderParameter(Encoder.Quality, (long)quality))
                    {
                        eps.Param[0] = ep;

                        ImageCodecInfo ici = GetEncoderInfo("image/gif");

                        string newFileName = GetAbsoluteCacheFilePath(destFilename);
                        CreateCacheDirectoryIfDoesntExist();
                        SaveBitmap(bmp, eps, ici, newFileName);

                        return GetRelativeCacheFilePath(destFilename);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static ImageCodecInfo GetEncoderInfo(string mineType)
        {
            try
            {
                ImageCodecInfo[] encs = ImageCodecInfo.GetImageEncoders();
                foreach (ImageCodecInfo enc in encs)
                {
                    if (enc.MimeType == mineType)
                    {
                        return enc;
                    }
                }
                throw new NotSupportedException("No corresponding codec.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal static string GetAbsoluteCacheFilePath(string path)
        {
            return $"{AbsoluteCacheDirectory}\\{Path.GetFileName(path)}";
        }

        internal static string AbsoluteCacheDirectory
        {
            get { return $"{Configuration.ApplicationConfiguration.WorkingDirectory}\\{Specifications.CACHE_DIRECTORY}"; }
        }

        internal static string GetRelativeCacheFilePath(string path)
        {
            return $"{Specifications.CACHE_DIRECTORY}\\{Path.GetFileName(path)}";
        }
    }
}
