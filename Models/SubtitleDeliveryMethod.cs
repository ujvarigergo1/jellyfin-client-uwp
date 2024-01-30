using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public enum SubtitleDeliveryMethod
    {
        /// <summary>
        /// Burn the subtitles in the video track.
        /// </summary>
        Encode = 0,

        /// <summary>
        /// Embed the subtitles in the file or stream.
        /// </summary>
        Embed = 1,

        /// <summary>
        /// Serve the subtitles as an external file.
        /// </summary>
        External = 2,

        /// <summary>
        /// Serve the subtitles as a separate HLS stream.
        /// </summary>
        Hls = 3,

        /// <summary>
        /// Drop the subtitle.
        /// </summary>
        Drop = 4
    }
}
