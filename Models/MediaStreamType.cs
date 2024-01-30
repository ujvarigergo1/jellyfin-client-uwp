using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public enum MediaStreamType
    {
        /// <summary>
        /// The audio.
        /// </summary>
        Audio,

        /// <summary>
        /// The video.
        /// </summary>
        Video,

        /// <summary>
        /// The subtitle.
        /// </summary>
        Subtitle,

        /// <summary>
        /// The embedded image.
        /// </summary>
        EmbeddedImage,

        /// <summary>
        /// The data.
        /// </summary>
        Data
    }
}
