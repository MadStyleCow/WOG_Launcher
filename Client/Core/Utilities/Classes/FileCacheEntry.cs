using System;
using System.Collections.Generic;

namespace Client.Core.Utilities.Classes
{
    [Serializable]
    public class FileCacheEntry
    {
        /* Fields */
        public String Path { get; set; }        // Relative path to the file from the base directory
        public String Hash { get; set; }        // MD5 hash of the file
        public String Sha1Hash { get; set; }    // SHA1 hash of the file
        public DateTime LastWrite { get; set; } // Date and time of the last write operation
    }

    [Serializable]
    public class FileCacheEntryList
    {
        /* Fields */
        public List<FileCacheEntry> FileCacheEntries { get; set; }

        /* Constructors */
    }
}
