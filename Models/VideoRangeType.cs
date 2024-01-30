﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public enum VideoRangeType
    {
        /// <summary>
        /// Unknown video range type.
        /// </summary>
        Unknown,

        /// <summary>
        /// SDR video range type (8bit).
        /// </summary>
        SDR,

        /// <summary>
        /// HDR10 video range type (10bit).
        /// </summary>
        HDR10,

        /// <summary>
        /// HLG video range type (10bit).
        /// </summary>
        HLG,

        /// <summary>
        /// Dolby Vision video range type (12bit).
        /// </summary>
        DOVI,

        /// <summary>
        /// HDR10+ video range type (10bit to 16bit).
        /// </summary>
        HDR10Plus
    }
}
