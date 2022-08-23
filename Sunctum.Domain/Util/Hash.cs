

using Sunctum.Domain.Logic.Import;
using Sunctum.Domain.Models;
using Sunctum.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sunctum.Domain.Util
{
    internal class Hash
    {
        public static string Generate(BookViewModel book)
        {
            var hashProvider = new MD5CryptoServiceProvider();
            byte[] hash = null;
            var contents = book.Contents.Where(x => x.Image != null).ToList();
            foreach (var page in contents)
            {
                using (var fs = new FileStream(page.Image.AbsoluteMasterPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var bs = hashProvider.ComputeHash(fs);
                    if (hash == null)
                    {
                        hash = bs;
                    }
                    else
                    {
                        var list = new List<byte>();
                        for (int i = 0; i < bs.Length; i++)
                        {
                            list.Add((byte)(bs[i] ^ hash[i]));
                        }
                        hash = list.ToArray();
                    }
                }
            }
            if (hash is null)
                return null;
            return BitConverter.ToString(hash).ToLower().Replace("-", "");
        }

        public static string Generate(PageViewModel page)
        {
            var hashProvider = new MD5CryptoServiceProvider();
            byte[] hash = null;
            using (var fs = new FileStream(page.Image.AbsoluteMasterPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var bs = hashProvider.ComputeHash(fs);
                if (hash == null)
                {
                    hash = bs;
                }
                else
                {
                    var list = new List<byte>();
                    for (int i = 0; i < bs.Length; i++)
                    {
                        list.Add((byte)(bs[i] ^ hash[i]));
                    }
                    hash = list.ToArray();
                }
            }
            if (hash is null)
                return null;
            return BitConverter.ToString(hash).ToLower().Replace("-", "");
        }

        public static string Generate(List<Importer> children)
        {
            byte[] hash = null;
            var fingerPrints = children.Select(x => x.FingerPrint);
            foreach (var fingerPrint in fingerPrints)
            {
                var bs = StringToBytes(fingerPrint);
                if (hash == null)
                {
                    hash = bs;
                }
                else
                {
                    var list = new List<byte>();
                    for (int i = 0; i < bs.Length; i++)
                    {
                        list.Add((byte)(bs[i] ^ hash[i]));
                    }
                    hash = list.ToArray();
                }
            }
        
            if (hash is null)
                return null;
            return BitConverter.ToString(hash).ToLower().Replace("-", "");
        }
        
        // 16進数文字列 => Byte配列
        public static byte[] StringToBytes(string str)
        {
            var bs = new List<byte>();
            for (int i = 0; i < str.Length / 2; i++)
            {
                bs.Add(Convert.ToByte(str.Substring(i * 2, 2), 16));
            }
            // "01-AB-EF" こういう"-"区切りを想定する場合は以下のようにする
            // var bs = str.Split('-').Select(hex => Convert.ToByte(hex, 16));
            return bs.ToArray();
        }
    }
}
