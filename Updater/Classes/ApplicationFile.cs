using System;
using System.Collections.Generic;
using Updater.Enums;

namespace Updater.Classes
{
    [Serializable]
    public class ApplicationFile
    {
        /* Fields */
        public Guid Id { get; set; }
        public String Path { get; set; }
        public Int32 Version { get; set; }
        public List<String> Url { get; set; }
        public FileType Type { get; set; }
    }
}
