using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public enum MetadataField
    {
        /// <summary>
        /// The cast.
        /// </summary>
        Cast,

        /// <summary>
        /// The genres.
        /// </summary>
        Genres,

        /// <summary>
        /// The production locations.
        /// </summary>
        ProductionLocations,

        /// <summary>
        /// The studios.
        /// </summary>
        Studios,

        /// <summary>
        /// The tags.
        /// </summary>
        Tags,

        /// <summary>
        /// The name.
        /// </summary>
        Name,

        /// <summary>
        /// The overview.
        /// </summary>
        Overview,

        /// <summary>
        /// The runtime.
        /// </summary>
        Runtime,

        /// <summary>
        /// The official rating.
        /// </summary>
        OfficialRating
    }
}
