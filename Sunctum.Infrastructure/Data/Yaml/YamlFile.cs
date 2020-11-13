

using System;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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
                using (var input = File.OpenText(filename))
                {
                    var deserializerBuilder = new DeserializerBuilder();
                    var deserializer = deserializerBuilder.Build();
                    return deserializer.Deserialize<T>(input);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
