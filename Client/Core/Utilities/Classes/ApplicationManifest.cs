using System;
using System.Collections.Generic;

namespace Client.Core.Utilities.Classes
{
    [Serializable]
    public class ApplicationManifest
    {
        /* Fields */
        public Int32 ManifestVersion { get; set; }
        public List<ApplicationFile> ApplicationFileList { get; set; }
    }
}
