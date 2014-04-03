using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateClient.Model.Classes
{
    [Serializable()]
    public class ApplicationManifest
    {
        /* Fields */
        public Int32 ManifestVersion { get; set; }
        public List<ApplicationFile> ApplicationFileList { get; set; }

        /* Constructors */
        public ApplicationManifest() { }
    }
}
