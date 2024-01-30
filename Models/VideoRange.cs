using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public enum VideoRange
    {
        /// <summary>
        /// Unknown video range.
        /// </summary>
        Unknown,

        /// <summary>
        /// SDR video range.
        /// </summary>
        SDR,

        /// <summary>
        /// HDR video range.
        /// </summary>
        HDR
    }
}
