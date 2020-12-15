using borcanelGame.Engine;
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
using TiledLib.Objects;

namespace borcanelGame.Gameplay
{
    class LevelMap : TileMapActor
    {
        #region Fields

        /// <summary>
        /// enum used to identify a layer ( ONWALL for deco on the wall, entities for player pos & npcs)
        /// </summary>
        public enum ELayer { FLOOR = 0, WALL = 1, ONWALL = 2, ENTITIES = 3} 

        /// <summary>
        /// Entity type that match the entity type in the tilemap
        /// </summary>
        public enum EEntityType { PLAYER = 0, NPC = 1 }

        public Player Player;

        private List<Point> mVisibleTiles;

        #endregion

        #region Properties

        public bool EnableDarkMode { get; set; }

        public Sprite LightTrickSprite { get; private set; }

        public Vector2 NpcPosition { get; private set; }

        public Vector2 PlayerPosition { get; private set; }

        #endregion

        #region Constructor

        public LevelMap(ContentManager content, string mapPath) : base(content, mapPath)
        {
            mVisibleTiles = new List<Point>();

            LightTrickSprite = new Sprite(content.Load<Texture2D>("Images/LightTrickSprite"));

            InitEntitiesPosition();

            EnableDarkMode = true;
        }

        #endregion

        #region TileMapActor Methods

        public override int GetTileAt(int col, int row)
        {
            if (row < 0 || row > Height || col < 0 || col > Width)
                return -1;

            int tileId = row * Width + col;

            return tileId;
        }

        public override int GetTileAtPosition(float x, float y)
        {
            int col = (int)x / TileWidth;
            int row = (int)y / TileHeight;

            if (x < 0)
                col = -1;
            if (y < 0)
                row = -1;

            return GetTileAt(col, row);
        }

        public override bool IsTileSolid(int tileId)
        {
            if (tileId < 0 || tileId >= Width * Height)
            {
                return true;
            }

            if(mMapData.Layers[(int)ELayer.WALL] is TileLayer layer)
            {
                if(layer.Data[tileId] > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool IsTileSolid(int col, int row)
        {
            return IsTileSolid(GetTileAt(col, row));
        }

        protected override void DrawTileLayer(int layerId, SpriteBatch spriteBatch)
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

                    // before drawing tile we check if the tile is visible
                    if(mVisibleTiles.Contains(new Point(col,row)) || !EnableDarkMode)
                    {
                        DrawTile(spriteBatch, tileFrame, layerId, usedTileset, col, row, depth);
                    }
                }
                col++;
                if (col == Width)
                {
                    col = 0;
                    row++;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // manage visibility only if dark mode is enabled
            if (EnableDarkMode)
            {
                // Mange visisibility

                // Create a rectangle that represent player visibility
                Rectangle visibilityRange;
                visibilityRange.X = (Player.Bounds.Center.X - (int)Math.Round(Position.X)) / TileWidth - Player.TileVisibility * 2;
                visibilityRange.Y = (Player.Bounds.Center.Y - (int)Math.Round(Position.Y)) / TileHeight  - Player.TileVisibility * 2;
                visibilityRange.Width = Player.TileVisibility * 4;
                visibilityRange.Height = Player.TileVisibility * 4;

                // update lightTrickSprite position to have a circle light feeling
                LightTrickSprite.Position = new Vector2(visibilityRange.X * TileHeight, visibilityRange.Y * TileHeight);

                mVisibleTiles.Clear();

                // Update display halls

                for (int j = visibilityRange.Y; j < visibilityRange.Bottom; j++)
                {
                    for (int i = visibilityRange.X; i < visibilityRange.Right; i++)
                    {
                        mVisibleTiles.Add(new Point(i, j));
                    }
                }
            }
            else
            {
                mVisibleTiles.Clear();
            }
        }

        #endregion

        #region Methods

        private void InitEntitiesPosition()
        {
            // test if a entity layer exists
            if (mMapData.Layers.Length <= (int)ELayer.ENTITIES)
                return;

            // test if the layer is an object layer ( it's the case on the tilemap)
            if (!(mMapData.Layers[(int)ELayer.ENTITIES] is ObjectLayer layer))
                return;

            foreach (var item in layer.Objects)
            {
                // verify is the item as an entityType property & object is a tileObject ( means the object is from an image)
                if(item.Properties.ContainsKey("EntityType") && item is TileObject tileObject)
                {
                    //get entityType ( need parsing because in the file properties are string)
                    EEntityType entityType = (EEntityType)int.Parse(tileObject.Properties["EntityType"]);

                    // set entityPos ( need to substract tileObject.Height because the object is draw by the bottom side)
                    Vector2 entityPos = new Vector2((int)tileObject.X, (int)(tileObject.Y - tileObject.Height));

                    switch (entityType)
                    {
                        case EEntityType.PLAYER:
                            PlayerPosition = entityPos;
                            break;
                        case EEntityType.NPC:
                            NpcPosition = entityPos;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Get a random position on floor
        /// </summary>
        /// <param name="minCol"></param>
        /// <param name="maxCol"></param>
        /// <param name="minRow"></param>
        /// <param name="maxRow"></param>
        /// <returns></returns>
        public Vector2 GetRandomFreePosition(int minCol,int maxCol,int minRow,int maxRow)
        {
            bool bIsTileSolid = false;

            Random rand = new Random();

            do
            {
                int col = rand.Next(minCol, maxCol);
                int row = rand.Next(minRow, maxRow);

                bIsTileSolid = IsTileSolid(col, row);

                if (!bIsTileSolid)
                {
                    return new Vector2(col * TileWidth, row * TileHeight);
                }

            } while (bIsTileSolid);

            return Vector2.Zero;
        }

        public void DrawFloor(SpriteBatch spritebatch)
        {
            DrawTileLayer((int)ELayer.FLOOR, spritebatch);
        }

        public void DrawWall(SpriteBatch spritebatch)
        {
            DrawTileLayer((int)ELayer.WALL, spritebatch);
        }

        public void DrawOnWall(SpriteBatch spritebatch)
        {
            DrawTileLayer((int)ELayer.ONWALL, spritebatch);
        }

        public void DrawLight(SpriteBatch spritebatch)
        {
            if(EnableDarkMode)
                LightTrickSprite.Draw(spritebatch);
        }

        #endregion
    }
}
