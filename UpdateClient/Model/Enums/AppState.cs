﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateClient.Model.Enums
{
    public enum AppState
    {
        INIT,
        CHECK,
        CANCELCHECK,
        UPDATE,
        CANCELUPDATE,
        PLAY,
        CLOSE
    }
}