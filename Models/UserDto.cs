using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Usb;

namespace Jellyfin.Models
{
    public class UserDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDto"/> class.
        /// </summary>
        public UserDto()
        {
            Policy = new UserPolicy();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the server identifier.
        /// </summary>
        /// <value>The server identifier.</value>
        public string ServerId { get; set; }

        /// <summary>
        /// Gets or sets the name of the server.
        /// This is not used by the server and is for client-side usage only.
        /// </summary>
        /// <value>The name of the server.</value>
        public string ServerName { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the primary image tag.
        /// </summary>
        /// <value>The primary image tag.</value>
        public string PrimaryImageTag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has password.
        /// </summary>
        /// <value><c>true</c> if this instance has password; otherwise, <c>false</c>.</value>
        public bool HasPassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has configured password.
        /// </summary>
        /// <value><c>true</c> if this instance has configured password; otherwise, <c>false</c>.</value>
        public bool HasConfiguredPassword { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has configured easy password.
        /// </summary>
        /// <value><c>true</c> if this instance has configured easy password; otherwise, <c>false</c>.</value>
        [Obsolete("Easy Password has been replaced with Quick Connect")]
        public bool HasConfiguredEasyPassword { get; set; }

        /// <summary>
        /// Gets or sets whether async login is enabled or not.
        /// </summary>
        public bool? EnableAutoLogin { get; set; }

        /// <summary>
        /// Gets or sets the last login date.
        /// </summary>
        /// <value>The last login date.</value>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// Gets or sets the last activity date.
        /// </summary>
        /// <value>The last activity date.</value>
        public DateTime? LastActivityDate { get; set; }


        /// <summary>
        /// Gets or sets the policy.
        /// </summary>
        /// <value>The policy.</value>
        public UserPolicy Policy { get; set; }

        /// <summary>
        /// Gets or sets the primary image aspect ratio.
        /// </summary>
        /// <value>The primary image aspect ratio.</value>
        public double? PrimaryImageAspectRatio { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name ?? base.ToString();
        }
    }
}
