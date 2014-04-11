using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Client.Core.Utilities
{
    public static class XMLSerializer
    {
        public static string XmlSerializeToString(this object pObjectInstance)
        {
            var serializer = new XmlSerializer(pObjectInstance.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, pObjectInstance);
            }

            return sb.ToString();
        }

        public static T XmlDeserializeFromString<T>(string pObjectData)
        {
            return (T)XmlDeserializeFromString(pObjectData, typeof(T));
        }

        public static object XmlDeserializeFromString(string pObjectData, Type pType)
        {
            var serializer = new XmlSerializer(pType);
            object result;

            using (TextReader reader = new StringReader(pObjectData))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }

        public static object XmlDeserializeFromFile(string pFile, Type pType)
        {
            if (!File.Exists(pFile))
            {
                throw new FileNotFoundException();
            }

            var serializer = new XmlSerializer(pType);
            object result;

            using (StreamReader reader = new StreamReader(pFile))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }

        public static object XmlDeserializeFromStream(Stream pStream, Type pType)
        {
            var serializer = new XmlSerializer(pType);
            object result;

            using (pStream)
            {
                result = serializer.Deserialize(pStream);
            }

            return result;
        }
    }
}
