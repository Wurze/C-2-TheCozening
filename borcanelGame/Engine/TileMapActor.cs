using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledLib;
using TiledLib.Layer;

namespace borcanelGame.Engine
{
    /// <summary>
    /// Class use to draw map can be overrided to draw a map with specific properties
    /// </summary>
    class TileMapActor : IActor
    {
        #region Fields

        /// <summary>
        /// Contains the tileMap create with Tiled
        /// </summary>
        protected Map mMapData;

        /// <summary>
        /// Contains all tilesets use in the map
        /// </summary>
        protected Dictionary<ITileset, Texture2D> mTilesets;

        #endregion

        #region IActor Properties

        public Vector2 Position { get; set; }

        public Rectangle BoundingBox { get; private set; }

        public bool ToRemove { get; set; }
        public bool IsActive { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// The Rectangle that countains the map
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, TileWidth * Width, TileHeight * Height);
            }
        }

        public int TileWidth
        {
            get
            {
                if (mMapData != null)
                    return mMapData.CellWidth;

                return 0;
            }
        }
        public int TileHeight
        {
            get
            {
                if (mMapData != null)
                    return mMapData.CellHeight;

                return 0;
            }
        }
        public int Width
        {
            get
            {
                if (mMapData != null)
                    return mMapData.Width;

                return 0;
            }
        }

        public int Height
        {
            get
            {
                if (mMapData != null)
                    return mMapData.Height;

                return 0;
            }
        }

        #endregion

        #region Constructors

        public TileMapActor(ContentManager content,string mapPath)
        {
            mMapData = content.Load<Map>(mapPath);

            // Load Tilesets
            mTilesets = new Dictionary<ITileset, Texture2D>();

            for (int i = 0; i < mMapData.Tilesets.Length; i++)
            {
                ITileset tileset = mMapData.Tilesets[i];

                Texture2D texture = content.Load<Texture2D>(GetRelativImagePath(content,tileset.ImagePath));

                mTilesets.Add(tileset, texture);
            }

            IsActive = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the tile id ( not graphical id) in the map with column & row
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public virtual int GetTileAt(int col, int row)
        {
            return -1;
        }

        /// <summary>
        /// Get the tile id in the map with screen position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual int GetTileAtPosition(float x, float y)
        {
            return -1;
        }

        /// <summary>
        /// Indicate if the tile specitified with the tileId is solid or not
        /// </summary>
        /// <param name="pTileId"></param>
        /// <returns></returns>
        public virtual bool IsTileSolid(int tileId)
        {
            return false;
        }

        /// <summary>
        /// Indicate if the tile specitified with the col & row is solid or not
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public virtual bool IsTileSolid(int col, int row)
        {
            return false;
        }

        /// <summary>
        /// Get the relativ image path of the tileset in map to load it after with contentManager
        /// </summary>
        /// <param name="pPath"></param>
        /// <returns></returns>
        string GetRelativImagePath(ContentManager content,string path)
        {
            // Get relativPath
            string relativPath = path.Replace(content.RootDirectory, "");

            // Remove eventual "../"
            relativPath = relativPath.Replace("../", "");

            // Remove extension
            int extensionIndex = relativPath.IndexOf(".");

            return relativPath.Substring(0, extensionIndex);
        }

        /// <summary>
        /// If you have more than one tileset you use this method to find a tileset according to the graphical id of the tile ( Hint : Open the map with notepad to see what it looks like)
        /// </summary>
        /// <param name="gid"></param>
        /// <returns></returns>
        protected ITileset GetUsedTileset(int gid)
        {
            ITileset usedtileset = null;

            foreach (var item in mTilesets)
            {
                ITileset tileset = item.Key;

                if (gid >= tileset.FirstGid && gid < tileset.FirstGid + tileset.TileCount)
                {
                    usedtileset = tileset;
                    break;
                }
            }

            return usedtileset;
        }

        /// <summary>
        /// Method to draw a tile
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gid"></param>
        /// <param name="layerType"></param>
        /// <param name="layerId"></param>
        /// <param name="tileset"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="depth"></param>
        protected virtual void DrawTile(SpriteBatch spriteBatch, int gid, int layerId, ITileset tileset, int col, int row, float depth = 0)
        {
            float x = col * TileWidth;
            float y = row * TileHeight;

            if (gid != -1)
            {
                int tilesetColumn = gid % tileset.Columns;
                int tilesetLine = (int)Math.Floor((double)gid / (double)tileset.Columns);

                Rectangle tilesetRec = new Rectangle(tileset.TileWidth * tilesetColumn, tileset.TileHeight * tilesetLine, tileset.TileWidth, tileset.TileHeight);

                Vector2 tilePosition = new Vector2(x + (int)Math.Floor(Position.X), y + (int)Math.Floor(Position.Y));

                spriteBatch.Draw(mTilesets[tileset], tilePosition, tilesetRec, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            }
        }

        /// <summary>
        /// Methods use to draw a tile layer
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="spriteBatch"></param>
        protected virtual void DrawTileLayer(int layerId, SpriteBatch spriteBatch)
        {
            if (layerId >= mMapData.Layers.Length || !(mMapData.Layers[layerId] is TileLayer tileLayer))
                return;

            if (!tileLayer.Visible)
                return;

            int col = 0;
            int row = 0;

            for (int i = 0; i < tileLayer.Data.Length; i++)
            {
                int gid = tileLayer.Data[i];

                if (gid != 0)
                {
                    float depth = 0;

                    ITileset usedTileset = GetUsedTileset(gid);

                    int tileFrame = gid - usedTileset.FirstGid;

                    DrawTile(spriteBatch, tileFrame, layerId, usedTileset, col, row, depth);
                }
                col++;
                if (col == Width)
                {
                    col = 0;
                    row++;
                }
            }
        }

        #endregion

        #region IActor Methods

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < mMapData.Layers.Length; i++)
            {
                DrawTileLayer(i, spriteBatch);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            
        }

        #endregion
    }
}
