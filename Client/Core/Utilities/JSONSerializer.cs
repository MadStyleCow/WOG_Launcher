using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client.Core.Utilities
{
    public static class JSONSerializer
    {
        /* Static methods */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFile"></param>
        /// <param name="pType"></param>
        /// <returns></returns>
        public static object DeserializeFromFile (String pFile, Type pType)
        {
            // Check to see if file exists
            if (!File.Exists(pFile))
            {
                // Throw an exception if it does not
                throw new FileNotFoundException();
            }

            // Read the file to a string
            string _jsonContent = File.ReadAllText(pFile);

            // Try to convert a file
            return JsonConvert.DeserializeObject(_jsonContent, pType);
        }
    }
}
