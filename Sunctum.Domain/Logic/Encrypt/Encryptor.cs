

using ChinhDo.Transactions;
using Homura.ORM;
using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.Util;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage.Streams;

namespace Sunctum.Domain.Logic.Encrypt
{
    public static class Encryptor
    {
        public static void Encrypt(ImageViewModel image, string OutFilePath, string password, DataOperationUnit dataOpUnit, TxFileManager txFileManager)
        {
            int len;
            byte[] buffer = new byte[4096];

            string originalImagePath = image.AbsoluteMasterPath;

            txFileManager.Snapshot(OutFilePath);

            using (FileStream outfs = new FileStream(OutFilePath, FileMode.Create, FileAccess.Write))
            {
                using (AesManaged aes = new AesManaged())
                {
                    aes.BlockSize = 128;
                    aes.KeySize = 128;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, 16);
                    byte[] salt = new byte[16];
                    salt = deriveBytes.Salt;

                    byte[] bufferKey = deriveBytes.GetBytes(16);
                    aes.Key = bufferKey;
                    aes.GenerateIV();

                    using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (CryptoStream cse = new CryptoStream(outfs, encryptor, CryptoStreamMode.Write))
                    {
                        outfs.Write(salt, 0, 16);
                        outfs.Write(aes.IV, 0, 16);

                        using (FileStream fs = new FileStream(originalImagePath, FileMode.Open, FileAccess.Read))
                        {
                            while ((len = fs.Read(buffer, 0, 4096)) > 0)
                            {
                                cse.Write(buffer, 0, len);
                            }
                        }
                    }
                }
            }

            EncryptImage encryptImage = new EncryptImage();
            encryptImage.ID = Guid.NewGuid();
            encryptImage.TargetImageID = image.ID;
            encryptImage.EncryptFilePath = OutFilePath;

            EncryptImageDao dao = new EncryptImageDao();
            dao.InsertOrReplace(encryptImage, dataOpUnit.CurrentConnection);
        }

        public static async Task<string> EncryptString(string plainText, string password)
        { 
            // 入力文字列をバイト型配列に変換
            byte[] src = Encoding.Unicode.GetBytes(plainText);

            // Encryptor（暗号化器）を用意する
            using (var aes = Aes.Create())
            {
                aes.BlockSize = 128;
                aes.KeySize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, 16);
                byte[] salt = new byte[16];
                salt = deriveBytes.Salt;

                byte[] bufferKey = deriveBytes.GetBytes(16);
                aes.Key = bufferKey;
                aes.GenerateIV();

                // 出力ストリームを用意する
                using (var encrypter = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var mem = new MemoryStream())
                {
                    await mem.WriteAsync(salt, 0, 16);
                    await mem.WriteAsync(aes.IV, 0, 16);
                    // 暗号化して書き出す
                    using (var cs = new CryptoStream(mem, encrypter, CryptoStreamMode.Write))
                    {
                        await cs.WriteAsync(src, 0, src.Length);
                    }
                    byte[] result = mem.ToArray();
                    // Base64文字列に変換して返す
                    return Convert.ToBase64String(result);
                }
            }
        }

        public static async Task<string> DecryptString(string base64Text, string password)
        {
            // Base64文字列をバイト型配列に変換
            byte[] src = Convert.FromBase64String(base64Text);

            // Decryptor（復号器）を用意する
            using (var aes = Aes.Create())
            {
                aes.BlockSize = 128;
                aes.KeySize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] salt = new byte[16];
                Array.Copy(src, 0, salt, 0, 16);

                Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, salt);
                byte[] bufferKey = deriveBytes.GetBytes(16);
                aes.Key = bufferKey;

                byte[] iv = new byte[16];
                Array.Copy(src, 16, iv, 0, 16);
                aes.IV = iv;
                //aes.GenerateIV();

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                // 入力ストリームを開く
                using (var inStream = new MemoryStream(src, false))
                // 出力ストリームを用意する
                using (var outStream = new MemoryStream())
                {
                    inStream.Seek(32, SeekOrigin.Begin);
                    // 復号して一定量ずつ読み出し、それを出力ストリームに書き出す
                    using (var cs = new CryptoStream(inStream, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] buffer = new byte[4096]; // バッファーサイズはBlockSizeの倍数にする
                        int len = 0;
                        while ((len = await cs.ReadAsync(buffer, 0, 4096)) > 0)
                            await outStream.WriteAsync(buffer, 0, len);
                    }
                    // 出力がファイルなら、以上で完了

                    // 文字列に変換して返す
                    byte[] result = outStream.ToArray();
                    return Encoding.Unicode.GetString(result);
                }
            }
        }

        public static void DeleteOriginal(PageViewModel page, TxFileManager fileMgr)
        {
            if (page.Image == null)
            {
                ContentsLoadTask.SetImageToPage(page);
            }
            fileMgr.Delete(page.Image.AbsoluteMasterPath);
        }

        public static void Decrypt(string encryptedFilePath, string password, bool isThumbnail)
        {
            if (password is null || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException($"password is not specified.");
            }
            int len;
            byte[] buffer = new byte[4096];

            MemoryStream outstream = new MemoryStream();

            using (var fs = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var aes = new AesManaged())
                {
                    aes.BlockSize = 128;
                    aes.KeySize = 128;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    byte[] salt = new byte[16];
                    fs.Read(salt, 0, 16);

                    byte[] iv = new byte[16];
                    fs.Read(iv, 0, 16);
                    aes.IV = iv;

                    Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, salt);
                    byte[] bufferKey = deriveBytes.GetBytes(16);
                    aes.Key = bufferKey;

                    fs.Seek(32, SeekOrigin.Begin);

                    using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (CryptoStream cse = new CryptoStream(fs, decryptor, CryptoStreamMode.Read))
                    {
                        while ((len = cse.Read(buffer, 0, 4096)) > 0)
                        {
                            outstream.Write(buffer, 0, len);
                        }
                    }
                }
            }

            if (!isThumbnail)
            {
                OnmemoryImageManager.Instance.Put(Guid.Parse(Path.GetFileNameWithoutExtension(encryptedFilePath)), false, outstream);
            }
            else
            {
                var filename = Guid.Parse(Path.GetFileNameWithoutExtension(encryptedFilePath)).ToString("N") + System.IO.Path.GetExtension(encryptedFilePath);
                var stream = ThumbnailGenerator.ScaleDownAndSaveAndToMemoryStream(outstream, filename);
                OnmemoryImageManager.Instance.Put(Guid.Parse(Path.GetFileNameWithoutExtension(encryptedFilePath)), true, stream);
            }
        }

        public static void Decrypt(string encryptedFilePath, string outputFilePath, string password, TxFileManager txFileManager)
        {
            int len;
            byte[] buffer = new byte[4096];

            if (!Directory.Exists(outputFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));
            }

            txFileManager.Snapshot(outputFilePath);

            using (var outstream = new FileStream(outputFilePath, FileMode.Create))
            {
                using (var fs = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var aes = new AesManaged())
                    {
                        aes.BlockSize = 128;
                        aes.KeySize = 128;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;

                        byte[] salt = new byte[16];
                        fs.Read(salt, 0, 16);

                        byte[] iv = new byte[16];
                        fs.Read(iv, 0, 16);
                        aes.IV = iv;

                        Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, salt);
                        byte[] bufferKey = deriveBytes.GetBytes(16);
                        aes.Key = bufferKey;

                        fs.Seek(32, SeekOrigin.Begin);

                        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                        using (CryptoStream cse = new CryptoStream(fs, decryptor, CryptoStreamMode.Read))
                        {
                            while ((len = cse.Read(buffer, 0, 4096)) > 0)
                            {
                                outstream.Write(buffer, 0, len);
                            }
                        }
                    }
                }
            }
        }
    }
}
