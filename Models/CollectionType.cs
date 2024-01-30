using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public enum CollectionType
    {
        /// <summary>
        /// Unknown collection.
        /// </summary>
        unknown = 0,

        /// <summary>
        /// Movies collection.
        /// </summary>
        movies = 1,

        /// <summary>
        /// Tv shows collection.
        /// </summary>
        tvshows = 2,

        /// <summary>
        /// Music collection.
        /// </summary>
        music = 3,

        /// <summary>
        /// Music videos collection.
        /// </summary>
        musicvideos = 4,

        /// <summary>
        /// Trailers collection.
        /// </summary>
        trailers = 5,

        /// <summary>
        /// Home videos collection.
        /// </summary>
        homevideos = 6,

        /// <summary>
        /// Box sets collection.
        /// </summary>
        boxsets = 7,

        /// <summary>
        /// Books collection.
        /// </summary>
        books = 8,

        /// <summary>
        /// Photos collection.
        /// </summary>
        photos = 9,

        /// <summary>
        /// Live tv collection.
        /// </summary>
        livetv = 10,

        /// <summary>
        /// Playlists collection.
        /// </summary>
        playlists = 11,

        /// <summary>
        /// Folders collection.
        /// </summary>
        folders = 12,

        /// <summary>
        /// Tv show series collection.
        /// </summary>
        
        tvshowseries = 101,

        /// <summary>
        /// Tv genres collection.
        /// </summary>
        
        tvgenres = 102,

        /// <summary>
        /// Tv genre collection.
        /// </summary>
        
        tvgenre = 103,

        /// <summary>
        /// Tv latest collection.
        /// </summary>
        
        tvlatest = 104,

        /// <summary>
        /// Tv next up collection.
        /// </summary>
        
        tvnextup = 105,

        /// <summary>
        /// Tv resume collection.
        /// </summary>
        
        tvresume = 106,

        /// <summary>
        /// Tv favorite series collection.
        /// </summary>
        
        tvfavoriteseries = 107,

        /// <summary>
        /// Tv favorite episodes collection.
        /// </summary>
        
        tvfavoriteepisodes = 108,

        /// <summary>
        /// Latest movies collection.
        /// </summary>
        
        movielatest = 109,

        /// <summary>
        /// Movies to resume collection.
        /// </summary>
        
        movieresume = 110,

        /// <summary>
        /// Movie movie collection.
        /// </summary>
        
        moviemovies = 111,

        /// <summary>
        /// Movie collections collection.
        /// </summary>
        moviecollection = 112,

        /// <summary>
        /// Movie favorites collection.
        /// </summary>
        moviefavorites = 113,

        /// <summary>
        /// Movie genres collection.
        /// </summary>
        moviegenres = 114,

        /// <summary>
        /// Movie genre collection.
        /// </summary>
        moviegenre = 115
    }

}
