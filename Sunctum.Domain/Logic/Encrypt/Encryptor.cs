

using Sunctum.Domain.Data.Dao;
using Sunctum.Domain.Logic.Load;
using Sunctum.Domain.Models;
using Sunctum.Domain.Models.Managers;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sunctum.Domain.Logic.Encrypt
{
    internal static class Encryptor
    {
        public static void Encrypt(ImageViewModel image, string OutFilePath, string password)
        {
            int len;
            byte[] buffer = new byte[4096];

            string originalImagePath = image.AbsoluteMasterPath;

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

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

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
            dao.InsertOrReplace(encryptImage);
        }

        public static void DeleteOriginal(PageViewModel page)
        {
            if (page.Image == null)
            {
                ContentsLoadTask.SetImageToPage(page);
            }
            File.Delete(page.Image.AbsoluteMasterPath);
        }

        public static void Decrypt(string encryptedFilePath, string password)
        {
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

            OnmemoryImageManager.Instance.put(Guid.Parse(Path.GetFileNameWithoutExtension(encryptedFilePath)), outstream);
        }
    }
}
