﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateClient.Model.Classes
{
    [Serializable()]
    public class Manifest
    {
        /* Fields */
        public List<Server> ServerList;

        /* Constructors */
        public Manifest() { }
    }
}
