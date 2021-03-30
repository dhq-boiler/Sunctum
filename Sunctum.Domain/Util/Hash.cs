

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
    }
}
