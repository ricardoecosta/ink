#if TILED_LIB_ENABLED
using System;
using Microsoft.Xna.Framework;
using TiledLib;

namespace HamstasKitties.Logic
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
