using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Core.Classes.A3Sync
{
    [Serializable]
    public class Protocol
    {
        // Properties
        public string login { get; set; }
        public string password { get; set; }
        public string url { get; set; }
        public string port { get; set; }
        public string protocolType { get; set; }
        public string connectionTimeOut { get; set; }
        public string readTimeOut { get; set; }
        public string hostname { get; set; }
        public string remotePath { get; set; }

        // Public constructor
        public Protocol() { }
    }
}
