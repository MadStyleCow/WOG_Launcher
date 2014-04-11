using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Utilities.Classes
{
    [Serializable()]
    public class FileCacheEntry
    {
        /* Fields */
        public String Path { get; set; }        // Relative path to the file from the base directory
        public String Hash { get; set; }        // MD5 hash of the file
        public DateTime LastWrite { get; set; } // Date and time of the last write operation

        /* Constructors */
        public FileCacheEntry() { }
    }

    [Serializable()]
    public class FileCacheEntryList
    {
        /* Fields */
        public List<FileCacheEntry> FileCacheEntries { get; set; }

        /* Constructors */
        public FileCacheEntryList() { }
    }
}
