﻿

using NLog;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Exceptions;
using Sunctum.Domain.Logic.Encrypt;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sunctum.Converters
{
    internal class PathToWriteableBitmap : IValueConverter
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return DependencyProperty.UnsetValue;
            }

            var image = value as ImageViewModel;
            var thumbnail = image?.Thumbnail;

            if (parameter is not null && parameter.Equals("RAW"))
            {
                return LoadBitmap(image);
            }

            if (thumbnail is not null)
            {
                if (thumbnail.RelativeMasterPath is null)
                {
                    var th = ThumbnailFacade.FindByImageID(image.ID).Result;
                    if (th is not null)
                    {
                        image.Thumbnail = thumbnail = th;
                        return LoadBitmap(thumbnail.AbsoluteMasterPath, image.IsEncrypted);
                    }
                }
                if (image is not null && image.IsEncrypted)
                {
                    try
                    {
                        Task.Run(async () => await image.DecryptImage(true).ConfigureAwait(false)).GetAwaiter().GetResult();
                    }
                    catch (ArgumentException)
                    {
                        return LoadBitmap($"{Configuration.ApplicationConfiguration.ExecutingDirectory}\\{Specifications.LOCK_ICON_FILE}", image.IsEncrypted);
                    }
                    catch (CryptographicException)
                    {
                        return DependencyProperty.UnsetValue;
                    }
                }
                if (!image.IsEncrypted && !File.Exists(thumbnail.AbsoluteMasterPath))
                {
                    return Application.Current.Dispatcher.Invoke(() =>
                    {
                        var tg = new Domain.Logic.Async.ThumbnailGenerating();
                        tg.Target = image;
                        (Application.Current.MainWindow.DataContext as IMainWindowViewModel).LibraryVM.TaskManager.RunSync(tg.GetTaskSequence());
                        return LoadBitmap(image.Thumbnail.AbsoluteMasterPath, image.IsEncrypted);
                    });
                }
                if (Guid.TryParse(Path.GetFileNameWithoutExtension(thumbnail.AbsoluteMasterPath), out var guid))
                {
                    var bitmap = OnmemoryImageManager.Instance.PullAsWriteableBitmap(guid, true);
                    if (bitmap is null)
                    {
                        return LoadBitmap(thumbnail.AbsoluteMasterPath, image.IsEncrypted);
                    }
                    return bitmap;
                }
                return DependencyProperty.UnsetValue;
            }
            else if (image is not null)
            {
                var th = ThumbnailFacade.FindByImageID(image.ID).Result;
                if (th is not null && !image.IsEncrypted)
                {
                    image.Thumbnail = thumbnail = th;
                    return LoadBitmap(thumbnail.AbsoluteMasterPath, image.IsEncrypted);
                }
                else
                {
                    return Application.Current.Dispatcher.Invoke(() =>
                    {
                        var tg = new Domain.Logic.Async.ThumbnailGenerating();
                        tg.Target = image;
                        (Application.Current.MainWindow.DataContext as IMainWindowViewModel).LibraryVM.TaskManager.RunSync(tg.GetTaskSequence());
                        if (image.IsEncrypted)
                        {
                            var beginDateTime = DateTime.Now;
                            var currentDateTime = DateTime.Now;
                            while (true)
                            {
                                try
                                {
                                    Task.Run(async () => await image.DecryptImage(true).ConfigureAwait(false)).GetAwaiter().GetResult();
                                }
                                catch (ArgumentException)
                                {
                                    if ((currentDateTime - beginDateTime).TotalSeconds >= 60) //60秒経過
                                    {
                                        return LoadBitmap($"{Configuration.ApplicationConfiguration.ExecutingDirectory}\\{Specifications.LOCK_ICON_FILE}", image.IsEncrypted);
                                    }
                                    continue;
                                }
                                var bitmap = OnmemoryImageManager.Instance.PullAsWriteableBitmap(image.ID, true);
                                if (bitmap is not null)
                                {
                                    return bitmap;
                                }
                                else
                                {
                                    return DependencyProperty.UnsetValue;
                                }
                            }
                        }
                        else
                        {
                            return LoadBitmap(image.Thumbnail.AbsoluteMasterPath, image.IsEncrypted);
                        }
                    });
                }
            }
            else if (!Configuration.ApplicationConfiguration.LibraryIsEncrypted && File.Exists(value as string))
            {
                return LoadBitmap(value as string, false);
            }
            else
            {
                var filePathPart = value as string;
                var file = Path.Combine(Configuration.ApplicationConfiguration.WorkingDirectory, filePathPart.Substring(filePathPart.IndexOf("\\") + 1));
                Task.Run(async () => await Encryptor.Decrypt(file, Configuration.ApplicationConfiguration.Password, false)).GetAwaiter().GetResult();
                var guid = Guid.Parse(Path.GetFileNameWithoutExtension(value as string));
                var bitmap = OnmemoryImageManager.Instance.PullAsWriteableBitmap(guid, false);
                if (bitmap is null)
                {
                    return DependencyProperty.UnsetValue;
                }
                return bitmap;
            }
        }

        private object LoadBitmap(ImageViewModel image)
        {
            if (image.IsEncrypted)
            {
                var bitmap = OnmemoryImageManager.Instance.PullAsWriteableBitmap(image.ID, false);
                if (bitmap is null)
                {
                    Task.Run(async () => await image.DecryptImage(false)).GetAwaiter().GetResult();
                    return OnmemoryImageManager.Instance.PullAsWriteableBitmap(image.ID, false);
                }
                return bitmap;
            }
            else
            {
                if (!File.Exists(image.AbsoluteMasterPath))
                {
                    s_logger.Debug($"Do not convert because file does not exist.");
                    return DependencyProperty.UnsetValue;
                }
                s_logger.Debug($"Load bitmap:{image.AbsoluteMasterPath}");
                return LoadBitmap(image.AbsoluteMasterPath, image.IsEncrypted);
            }
        }

        private object LoadBitmap(string path, bool IsEncrypted)
        {
            try
            {
                using (Mat mat = new Mat(path, ImreadModes.Unchanged))
                {
                    if (mat.Rows == 0 || mat.Cols == 0)
                    {
                        var fileInfo = new FileInfo(path);
                        if (fileInfo.Length == 0)
                        {
                            s_logger.Error($"File is broken:{path}");
                            return DependencyProperty.UnsetValue;
                        }
                        if (Path.GetExtension(path) == ".gif")
                        {
                            return DependencyProperty.UnsetValue;
                        }
                        Thread.Sleep(100);
                        s_logger.Error($"Retry to load bitmap:{path}");
                        return LoadBitmap(path, IsEncrypted);
                    }
                    return WriteableBitmapConverter.ToWriteableBitmap(mat);
                }
            }
            catch (OutOfMemoryException e)
            {
                s_logger.Error(e);
                GC.WaitForPendingFinalizers();
                GC.Collect();
                return DependencyProperty.UnsetValue;
            }
            catch (COMException e)
            {
                s_logger.Error(e);
                GC.WaitForPendingFinalizers();
                GC.Collect();
                return DependencyProperty.UnsetValue;
            }
            catch (OpenCVException e)
            {
                s_logger.Error(e);
                GC.WaitForPendingFinalizers();
                GC.Collect();
                return DependencyProperty.UnsetValue;
            }
            catch (FileNotFoundException e)
            {
                s_logger.Error(e);
                GC.Collect();
                if (IsEncrypted)
                {
                    return LoadBitmap($"{Configuration.ApplicationConfiguration.ExecutingDirectory}\\{Specifications.LOCK_ICON_FILE}", false);
                }
                else
                {
                    return DependencyProperty.UnsetValue;
                }
            }
        }

        [Obsolete]
        private WriteableBitmap ToWriteableBitmap(Mat mat)
        {
            WriteableBitmap bitmap = new WriteableBitmap(mat.Cols, mat.Rows, 92, 92, SelectChannels(mat), null);

            unsafe
            {
                try
                {
                    bitmap.Lock();

                    byte* p_src = (byte*)mat.Data.ToPointer();
                    byte* p_dst = (byte*)bitmap.BackBuffer.ToPointer();
                    int step_dst = bitmap.BackBufferStride;
                    int channels = mat.Channels();
                    long step_src = mat.Step();

                    for (int y = 0; y < bitmap.PixelHeight; ++y)
                    {
                        for (int x = 0; x < bitmap.PixelWidth; ++x)
                        {
                            for (int c = 0; c < channels; ++c)
                            {
                                *(p_dst + y * step_dst + x * channels + c) = *(p_src + y * step_src + x * channels + c);
                            }
                        }
                    }

                    bitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
                }
                finally
                {
                    bitmap.Unlock();
                }
            }

            return bitmap;
        }


        [Obsolete]
        private byte PickPixel(Mat mat, int y, int x, int c)
        {
            switch (mat.Channels())
            {
                case 1:
                    return mat.At<Vec3b>(y, x)[c];
                case 3:
                    return mat.At<Vec3b>(y, x)[c];
                case 4:
                    return mat.At<Vec4b>(y, x)[c];
                default:
                    throw new NotSupportedException();
            }
        }

        private PixelFormat SelectChannels(Mat mat)
        {
            switch (mat.Channels())
            {
                case 1:
                    return System.Windows.Media.PixelFormats.Gray8;
                case 3:
                    return System.Windows.Media.PixelFormats.Bgr24;
                case 4:
                    return System.Windows.Media.PixelFormats.Bgra32;
                default:
                    throw new NotSupportedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
