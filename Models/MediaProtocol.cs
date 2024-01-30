using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public enum MediaProtocol
    {
        File = 0,
        Http = 1,
        Rtmp = 2,
        Rtsp = 3,
        Udp = 4,
        Rtp = 5,
        Ftp = 6
    }
}
