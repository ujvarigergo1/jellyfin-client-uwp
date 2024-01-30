﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public class TrickplayInfo
    {
        /// <summary>
        /// Gets or sets the id of the associated item.
        /// </summary>
        /// <remarks>
        /// Required.
        /// </remarks>
        [JsonIgnore]
        public Guid ItemId { get; set; }

        /// <summary>
        /// Gets or sets width of an individual thumbnail.
        /// </summary>
        /// <remarks>
        /// Required.
        /// </remarks>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets height of an individual thumbnail.
        /// </summary>
        /// <remarks>
        /// Required.
        /// </remarks>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets amount of thumbnails per row.
        /// </summary>
        /// <remarks>
        /// Required.
        /// </remarks>
        public int TileWidth { get; set; }

        /// <summary>
        /// Gets or sets amount of thumbnails per column.
        /// </summary>
        /// <remarks>
        /// Required.
        /// </remarks>
        public int TileHeight { get; set; }

        /// <summary>
        /// Gets or sets total amount of non-black thumbnails.
        /// </summary>
        /// <remarks>
        /// Required.
        /// </remarks>
        public int ThumbnailCount { get; set; }

        /// <summary>
        /// Gets or sets interval in milliseconds between each trickplay thumbnail.
        /// </summary>
        /// <remarks>
        /// Required.
        /// </remarks>
        public int Interval { get; set; }

        /// <summary>
        /// Gets or sets peak bandwith usage in bits per second.
        /// </summary>
        /// <remarks>
        /// Required.
        /// </remarks>
        public int Bandwidth { get; set; }
    }
    }
