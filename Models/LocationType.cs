using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public enum LocationType
    {
        /// <summary>
        /// The file system.
        /// </summary>
        FileSystem = 0,

        /// <summary>
        /// The remote.
        /// </summary>
        Remote = 1,

        /// <summary>
        /// The virtual.
        /// </summary>
        Virtual = 2,

        /// <summary>
        /// The offline.
        /// </summary>
        Offline = 3
    }
}
