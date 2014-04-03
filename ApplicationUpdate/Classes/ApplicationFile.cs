using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationUpdate.Enums;

namespace ApplicationUpdate.Classes
{
    [Serializable()]
    public class ApplicationFile
    {
        /* Fields */
        public Guid ID { get; set; }
        public String Path { get; set; }
        public Int32 Version { get; set; }
        public List<String> URL { get; set; }
        public FileType Type { get; set; }

        /* Constructors */
        public ApplicationFile() { }
    }
}
