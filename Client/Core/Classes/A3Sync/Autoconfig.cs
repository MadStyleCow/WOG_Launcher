using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Classes.A3Sync
{
    [Serializable]
    public class Autoconfig
    {
        // Public properties
        public string repositoryName { get; set; }
        public Protocol protocol { get; set; }
        public List<object> favoriteServers { get; set; }

        // Public constructor
        public Autoconfig() { }
    }
}