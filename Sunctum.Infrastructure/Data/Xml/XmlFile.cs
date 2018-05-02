

using System;
using System.IO;
using System.Xml.Serialization;

namespace Sunctum.Infrastructure.Data.Xml
{
    public class XmlFile
    {
        public static void Serialize(object obj, string filename)
        {
            try
            {
                XmlSerializer xmls = new XmlSerializer(obj.GetType());

                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    xmls.Serialize(fs, obj);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static object Deserialize(string filename, Type type)
        {
            try
            {
                XmlSerializer xmls = new XmlSerializer(type);

                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    return xmls.Deserialize(fs);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
