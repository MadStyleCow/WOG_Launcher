using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using log4net;

namespace Updater.Utilities
{
    public static class XMLSerializer
    {
        /* Loggers */
        private static readonly ILog Logger = LogManager.GetLogger(typeof(XMLSerializer));

        public static string XmlSerializeToString(this object pObjectInstance)
        {
            try
            {
                var serializer = new XmlSerializer(pObjectInstance.GetType());
                var sb = new StringBuilder();

                using (TextWriter writer = new StringWriter(sb))
                {
                    serializer.Serialize(writer, pObjectInstance);
                }

                return sb.ToString();
            }
            catch(Exception ex)
            {
                Logger.Fatal("An error was encountered while trying to serialize.", ex);
                throw;
            }
        }

        public static T XmlDeserializeFromString<T>(string pObjectData)
        {
            try
            {
                return (T)XmlDeserializeFromString(pObjectData, typeof(T));
            }
            catch(Exception ex)
            {
                Logger.Fatal("An error was encountered while trying to deserialize from string.", ex);
                throw;
            }
        }

        public static object XmlDeserializeFromString(string pObjectData, Type pType)
        {
            try
            {
                var serializer = new XmlSerializer(pType);
                object result;

                using (TextReader reader = new StringReader(pObjectData))
                {
                    result = serializer.Deserialize(reader);
                }

                return result;
            }
            catch(Exception ex)
            {
                Logger.Fatal("An error was encountered while trying to deserialize from string.", ex);
                throw;
            }
        }

        public static object XmlDeserializeFromFile(string pFile, Type pType)
        {
            try
            {
                if (!File.Exists(pFile))
                {
                    throw new FileNotFoundException();
                }

                var serializer = new XmlSerializer(pType);
                object result;

                using (var reader = new StreamReader(pFile))
                {
                    result = serializer.Deserialize(reader);
                }

                return result;
            }
            catch(Exception ex)
            {
                Logger.Fatal("An error was encountered while trying to deserialize from file.", ex);
                throw;
            }
        }

        public static object XmlDeserializeFromStream(Stream pStream, Type pType)
        {
            try
            {
                var serializer = new XmlSerializer(pType);
                object result;

                using (pStream)
                {
                    result = serializer.Deserialize(pStream);
                }

                return result;
            }
            catch(Exception ex)
            {
                Logger.Fatal("An error was encountered while trying to deserialize from stream.", ex);
                throw;
            }
        }
    }
}
