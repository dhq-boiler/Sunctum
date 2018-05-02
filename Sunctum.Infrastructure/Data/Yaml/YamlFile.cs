

using System;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace Sunctum.Infrastructure.Data.Yaml
{
    public static class YamlFile
    {
        public static void Serialize(object obj, string filename)
        {
            try
            {
                var serializer = new Serializer();

                using (FileStream fs = new FileStream(filename, FileMode.Create))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    serializer.Serialize(sw, obj);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static T Deserialize<T>(string filename)
        {
            try
            {
                var deserializer = new Deserializer();

                using (FileStream fs = new FileStream(filename, FileMode.Open))
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    return deserializer.Deserialize<T>(sr);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
