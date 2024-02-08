using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public class PlaybackInfoResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaybackInfoResponse" /> class.
        /// </summary>
        public PlaybackInfoResponse()
        {
            MediaSources = Array.Empty<MediaSourceInfo>();
        }

        /// <summary>
        /// Gets or sets the media sources.
        /// </summary>
        /// <value>The media sources.</value>
        public IReadOnlyList<MediaSourceInfo> MediaSources { get; set; }

        /// <summary>
        /// Gets or sets the play session identifier.
        /// </summary>
        /// <value>The play session identifier.</value>
        public string PlaySessionId { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>The error code.</value>
        public PlaybackErrorCode? ErrorCode { get; set; }
    }
}
