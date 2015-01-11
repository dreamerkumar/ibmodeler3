using System.IO;
using System.Xml.Serialization;

namespace Ajubaa.Common  
{
    /// <summary>
    /// serializes or deserializes objects to xml files 
    /// silently eats up any thrown exceptions
    /// </summary>
    public static class XmlSerializerHelper
    {
        public static T Deserialize<T>(string filePath) where T : class
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var deserializer = new XmlSerializer(typeof(T));
                    var tr = new StreamReader(filePath);
                    var deserialized = deserializer.Deserialize(tr) as T;
                    tr.Close();
                    return deserialized;
                }
                catch 
                {
                   
                }
            }
            return null;
        }

        public static void Serialize<T>(T t, string filePath) where T : class
        {
            try
            {
                //try serializing this input to the source folder
                var serializer = new XmlSerializer(typeof(T));
                var tw = new StreamWriter(filePath);
                serializer.Serialize(tw, t);
                tw.Close();
            }
            catch { }
        }
    }
}
