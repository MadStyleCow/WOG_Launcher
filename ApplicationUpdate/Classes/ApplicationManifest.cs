using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationUpdate.Classes
{
    [Serializable()]
    public class ApplicationManifest
    {
        /* Fields */
        public List<ApplicationFile> ApplicationFileList { get; set; }

        /* Constructors */
        public ApplicationManifest() { }
    }
}
