using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public class QueueItem
    {
        public Guid Id { get; set; }

        public string PlaylistItemId { get; set; }
    }
}
