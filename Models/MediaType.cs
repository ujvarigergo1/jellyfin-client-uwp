using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public enum MediaType
    {
        /// <summary>
        /// The audio.
        /// </summary>
        Audio = 0,

        /// <summary>
        /// The photo.
        /// </summary>
        Photo = 1,

        /// <summary>
        /// The video.
        /// </summary>
        Video = 2
    }
}
