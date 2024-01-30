using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public enum AudioSpatialFormat
    {
        /// <summary>
        /// None audio spatial format.
        /// </summary>
        None,

        /// <summary>
        /// Dolby Atmos audio spatial format.
        /// </summary>
        DolbyAtmos,

        /// <summary>
        /// DTS:X audio spatial format.
        /// </summary>
        DTSX,
    }
}
