﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public enum PlayMethod
    {
        Transcode = 0,
        DirectStream = 1,
        DirectPlay = 2
    }
}
