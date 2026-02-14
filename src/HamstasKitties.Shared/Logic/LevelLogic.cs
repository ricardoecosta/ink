#if TILED_LIB_ENABLED
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;
using HamstasKitties.Core;

namespace HamstasKitties.Logic
{
    public class LevelLogic
    {
        public LevelLogic(Director director, Map logicMap)
        {
            this.director = director;
            this.tmxLogicMap = logicMap;
            this.tmxLogicMapArea = new Rectangle(0, 0, this.tmxLogicMap.Width * this.tmxLogicMap.TileWidth, this.tmxLogicMap.Height * this.tmxLogicMap.TileHeight);

            // Loads all droppable areas.
            this.landDroppableTileIndexesDictionary = this.LoadTileInfoPerPositionFromLayerToDictionary(LayersNames.Land);
            this.waterDroppableTileIndexesDictionary = this.LoadTileInfoPerPositionFromLayerToDictionary(LayersNames.Water);

            // Loads all splines.
            this.splines = this.LoadAllSplines();
        }

        public Spline GetSplineFromLayer(string layerName)
        {
            return this.splines[layerName].CreateCopy();
        }

        private Dictionary<string, Spline> LoadAllSplines()
        {
            Dictionary<string, Spline> dictionary = new Dictionary<string, Spline>();

            foreach (Layer currentLayer in this.tmxLogicMap.Layers)
            {
                string layerName = currentLayer.Name;

                if (layerName.StartsWith(SplineLayersPrefix))
                {
                    dictionary.Add(layerName, this.LoadSplineFromLayer(layerName));
                }
            }

            return dictionary;
        }

        private Spline LoadSplineFromLayer(String layerName)
        {
            IEnumerable<MapObject> mapObjectsEnumerable = this.tmxLogicMap.FindObjects(new MapObjectFinder((objLayer, obj) =>
            {
                return objLayer.Name.Equals(layerName);
            }));

            List<MapObject> mapObjectsList = new List<MapObject>(mapObjectsEnumerable);
            mapObjectsList.Sort(new Comparison<MapObject>((obj1, obj2) =>
            {
                return float.Parse(obj1.Name).CompareTo(float.Parse(obj2.Name));
            }));

            SplinePoint[] controlPointsList = mapObjectsList.Select(mapObject =>
            {
                SplinePoint.Types type = (mapObject.Type != null && mapObject.Type != string.Empty) ? (SplinePoint.Types)Enum.Parse(typeof(SplinePoint.Types), mapObject.Type, true) : SplinePoint.Types.None;
                return new SplinePoint(new Vector2(mapObject.Bounds.X, mapObject.Bounds.Y), type);
            }).ToArray();

            return new Spline(Spline.InterpolationType.CatmullRom, controlPointsList, DefaultSecondsBetweenSplineControlPoints);
        }

        public TileInfo GetLandDroppableTileAtWorldPosition(Vector2 worldPosition)
        {
            Point tileIndex = new Point((int)(worldPosition.X / this.DefaultTileWidth), (int)(worldPosition.Y / this.DefaultTileHeight));

            try
            {
                return this.landDroppableTileIndexesDictionary[tileIndex];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public TileInfo GetWaterDroppableTileAtWorldPosition(Vector2 worldPosition)
        {
            Point tileIndex = new Point((int)(worldPosition.X / this.DefaultTileWidth), (int)(worldPosition.Y / this.DefaultTileHeight));

            try
            {
                return this.waterDroppableTileIndexesDictionary[tileIndex];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        private Dictionary<Point, TileInfo> LoadTileInfoPerPositionFromLayerToDictionary(LayersNames layerName)
        {
            Dictionary<Point, TileInfo> dictionary = new Dictionary<Point, TileInfo>();

            for (int x = 0; x < this.tmxLogicMap.Width; x++)
            {
                for (int y = 0; y < this.tmxLogicMap.Height; y++)
                {
                    Point tileIndex = new Point(x, y);
                    dictionary[tileIndex] = this.GetTileInfoFromIndexAndLayer(tileIndex, layerName);
                }
            }

            return dictionary;
        }

        private TileInfo GetTileInfoFromIndexAndLayer(Point tileIndex, LayersNames layerName)
        {
            try
            {
                TileLayer layer = this.tmxLogicMap.GetLayer(layerName.ToString()) as TileLayer;
                Tile tile = layer.Tiles[tileIndex.X, tileIndex.Y];

                if (tile != null)
                {
                    return new TileInfo(tile, tileIndex);
                }

                return null;
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            this.tmxLogicMap.Draw(spriteBatch);
        }

        public int Width
        {
            get
            {
                return this.tmxLogicMapArea.Width;
            }
        }

        public int Height
        {
            get
            {
                return this.tmxLogicMapArea.Height;
            }
        }

        public int DefaultTileWidth
        {
            get
            {
                return this.tmxLogicMap.TileWidth;
            }
        }

        public int DefaultTileHeight
        {
            get
            {
                return this.tmxLogicMap.TileHeight;
            }
        }

        private enum LayersNames
        {
            Land,
            Water
        }

        protected Director director;
        private Map tmxLogicMap;
        private Rectangle tmxLogicMapArea;
        private Dictionary<Point, TileInfo> landDroppableTileIndexesDictionary;
        private Dictionary<Point, TileInfo> waterDroppableTileIndexesDictionary;
        private Dictionary<string, Spline> splines;

        private const string SplineLayersPrefix = "Spline";

        public const float DefaultSecondsBetweenSplineControlPoints = 1;
    }
}
#endif
