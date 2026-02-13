#if TILED_LIB_ENABLED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledLib;
using Microsoft.Xna.Framework;

namespace GameLibrary.Logic
{
    /// <summary>
    /// Struct to save extended information about the tile.
    /// </summary>
    public class TileInfo
    {
        public TileInfo(Tile tile, Point tileIndex)
        {
            Tile = tile;
            TileIndex = tileIndex;
        }

        public Tile Tile { get; set; }
        public Point TileIndex { get; set; }
    }
}
#endif