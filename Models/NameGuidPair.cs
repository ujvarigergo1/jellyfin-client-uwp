using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public class NameGuidPair
    {
        public string Name { get; set; }

        public Guid Id { get; set; }
    }
}
