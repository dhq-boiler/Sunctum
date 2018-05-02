

using System;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Sunctum.Infrastructure.Data.Yaml
{
    public static class YamlConverter
    {
        public static string ToYaml(object obj)
        {
            try
            {
                var serializer = new Serializer();
                return serializer.Serialize(obj);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string ExceptionToYaml<T>(T ex) where T : Exception
        {
            try
            {
                var serializer = new SerializerBuilder()
                    .WithAttributeOverride<T>(e => e.Message, new YamlMemberAttribute() { ScalarStyle = ScalarStyle.Literal })
                    .WithAttributeOverride<T>(e => e.StackTrace, new YamlMemberAttribute() { ScalarStyle = ScalarStyle.Literal })
                    .WithAttributeOverride<T>(e => e.TargetSite, new YamlIgnoreAttribute())
                    .Build();
                return serializer.Serialize(ex);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static T ToObjectAS<T>(string yaml)
        {
            try
            {
                var deserializer = new Deserializer();
                return deserializer.Deserialize<T>(yaml);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
