
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public class SessionInfo
    {
        private const long ProgressIncrement = 10000000;


        private readonly object _progressLock = new object();

        private bool _disposed = false;


        /// <summary>
        /// Gets or sets the remote end point.
        /// </summary>
        /// <value>The remote end point.</value>
        public string RemoteEndPoint { get; set; }

        /// <summary>
        /// Gets the playable media types.
        /// </summary>
        /// <value>The playable media types.</value>
        public IReadOnlyList<MediaType> PlayableMediaTypes
        { set; get;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>The user id.</value>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the type of the client.
        /// </summary>
        /// <value>The type of the client.</value>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets the last activity date.
        /// </summary>
        /// <value>The last activity date.</value>
        public DateTime LastActivityDate { get; set; }

        /// <summary>
        /// Gets or sets the last playback check in.
        /// </summary>
        /// <value>The last playback check in.</value>
        public DateTime LastPlaybackCheckIn { get; set; }

        /// <summary>
        /// Gets or sets the last paused date.
        /// </summary>
        /// <value>The last paused date.</value>
        public DateTime? LastPausedDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the device.
        /// </summary>
        /// <value>The name of the device.</value>
        public string DeviceName { get; set; }

        /// <summary>
        /// Gets or sets the type of the device.
        /// </summary>
        /// <value>The type of the device.</value>
        public string DeviceType { get; set; }


        /// <summary>
        /// Gets or sets the device id.
        /// </summary>
        /// <value>The device id.</value>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>The application version.</value>
        public string ApplicationVersion { get; set; }


        public bool HasCustomDeviceName { get; set; }

        public string PlaylistItemId { get; set; }

        public string ServerId { get; set; }

        public string UserPrimaryImageTag { get; set; }

    }
}
