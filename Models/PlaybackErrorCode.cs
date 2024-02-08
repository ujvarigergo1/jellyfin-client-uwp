using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public enum PlaybackErrorCode
    {
        NotAllowed = 0,
        NoCompatibleStream = 1,
        RateLimitExceeded = 2
    }
}
