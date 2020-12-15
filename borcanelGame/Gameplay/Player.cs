using borcanelGame.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace borcanelGame.Gameplay
{
    class Player : AnimatedSprite
    {
        #region Fields

        float mSpeed = 2f;

        TileMapActor mMap;

        public enum ECollisionSide { LEFT, RIGHT, TOP, BOTTOM }

        #endregion

        /// <summary>
        /// I create a animState enum to manage animation without string
        /// </summary>
        public enum EAnimState { WALK_LEFT, WALK_UP, WALK_RIGHT, WALK_DOWN, NONE }

        #region Properties

        public int TileVisibility { get; private set; } = 2;

        // This rectangle is use to detect collision with object on floor & npc
        public Rectangle InteractionBox
        {
            get
            {
                // interactionBox need to be bigger than bounding box
                Rectangle interactionBox = BoundingBox;

                interactionBox.X -= 4;
                interactionBox.Y -= 4;
                interactionBox.Width += 8;
                interactionBox.Height += 8;

                return interactionBox;
            }
        }

        public EAnimState AnimState
        {
            get
            {
                switch (CurrentKeyAnim)
                {
                    case "Walking_Left":
                        return EAnimState.WALK_LEFT;
                    case "Walking_Up":
                        return EAnimState.WALK_UP;
                    case "Walking_Right":
                        return EAnimState.WALK_RIGHT;
                    case "Walking_Down":
                        return EAnimState.WALK_DOWN;
                    default:
                        return EAnimState.NONE;
                }
            }
            set
            {
                switch (value)
                {
                    case EAnimState.WALK_LEFT:
                        SetAnimation("Walking_Left");
                        break;
                    case EAnimState.WALK_UP:
                        SetAnimation("Walking_Up");
                        break;
                    case EAnimState.WALK_RIGHT:
                        SetAnimation("Walking_Right");
                        break;
                    case EAnimState.WALK_DOWN:
                        SetAnimation("Walking_Down");
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Constructor

        public Player(TileMapActor map) : base()
        {
            mMap = map;
        }

        #endregion

        #region Collision Methods

        /* The right part of the player collide with the left part of the world */
        private bool CollideRight()
        {
            int id1 = mMap.GetTileAtPosition(BoundingBox.Right, BoundingBox.Y + 3);
            int id2 = mMap.GetTileAtPosition(BoundingBox.Right, BoundingBox.Bottom - 2);

            return mMap.IsTileSolid(id1) || mMap.IsTileSolid(id2);
        }

        /* The left part of the player collide with the right part of the world */
        private bool CollideLeft()
        {
            int id1 = mMap.GetTileAtPosition(BoundingBox.X - 1, BoundingBox.Y + 3);
            int id2 = mMap.GetTileAtPosition(BoundingBox.X - 1, BoundingBox.Bottom - 2);

            return mMap.IsTileSolid(id1) || mMap.IsTileSolid(id2);
        }

        /* The top part of the player collide with the bottom part of the world */
        private bool CollideAbove()
        {
            int id1 = mMap.GetTileAtPosition(BoundingBox.X + 1, BoundingBox.Y - 2);
            int id2 = mMap.GetTileAtPosition(BoundingBox.Right - 2, BoundingBox.Y - 2);

            return mMap.IsTileSolid(id1) || mMap.IsTileSolid(id2);
        }

        /* The bottom part of the player collide with the top part of the world */
        private bool CollideBellow()
        {
            int id1 = mMap.GetTileAtPosition(BoundingBox.X + 3, BoundingBox.Bottom);
            int id2 = mMap.GetTileAtPosition(BoundingBox.Right - 3, BoundingBox.Bottom);

            return mMap.IsTileSolid(id1) || mMap.IsTileSolid(id2);
        }

        public void OnCollisionWithBox(Rectangle collidingBox, ECollisionSide side)
        {
            Vector2 position = Position;

            switch (side)
            {
                case ECollisionSide.LEFT:
                    position.X = collidingBox.Right;
                    position.X += ((int)Position.X - BoundingBox.Left);
                    break;
                case ECollisionSide.RIGHT:
                    position.X = collidingBox.Left;
                    position.X -= (BoundingBox.Right - (int)Math.Round(Position.X));
                    break;
                case ECollisionSide.TOP:
                    position.Y = collidingBox.Bottom;
                    position.Y += ((int)Position.Y - BoundingBox.Top);
                    break;
                case ECollisionSide.BOTTOM:
                    position.Y = collidingBox.Top;
                    position.Y -= (BoundingBox.Bottom - (int)Math.Round(Position.Y));
                    break;
                default:
                    break;
            }

            Position = position;
        }

        private void ManageCollision()
        {
            bool bCollide = false;

            bool bCollideRight = false;
            bool bCollideLeft = false;
            bool bCollideAbove = false;
            bool bCollideBellow = false;

            bCollideRight = CollideRight();
            bCollideLeft = CollideLeft();

            if (Math.Abs(vx) < 0.3f)
            {
                bCollide = bCollideRight || bCollideLeft;
            }
            else if (vx > 0)
            {
                bCollide = bCollideRight;
                bCollideLeft = false;
            }
            else if (vx < 0)
            {
                bCollide = bCollideLeft;
                bCollideRight = false;
            }

            if (bCollide)
            {
                int col = 0;

                if (bCollideLeft)
                {
                    col = (int)Math.Floor((BoundingBox.X + mMap.TileWidth / 2.0f) / mMap.TileWidth) - 1;
                }
                else if (bCollideRight)
                {
                    col = (int)Math.Floor((BoundingBox.Right + mMap.TileWidth / 2.0f) / mMap.TileWidth);
                }

                if (col > mMap.Width)
                    col = mMap.Width;
                else if (col < -1)
                    col = -1;

                Rectangle collidingTile = new Rectangle(col * mMap.TileWidth, (int)Position.Y, mMap.TileWidth, mMap.TileHeight);

                if (bCollideLeft)
                {
                    OnCollisionWithBox(collidingTile, ECollisionSide.LEFT);
                }
                else if (bCollideRight)
                {
                    OnCollisionWithBox(collidingTile, ECollisionSide.RIGHT);
                }

                vx = 0;
            }

            bCollide = false;

            bCollideAbove = CollideAbove();
            bCollideBellow = CollideBellow();

            if (Math.Abs(vy) < 0.3f)
            {
                bCollide = bCollideAbove || bCollideBellow;
            }
            else if (vy > 0)
            {
                bCollide = bCollideBellow;
                bCollideAbove = false;
            }
            else if (vy < 0)
            {
                bCollide = bCollideAbove;
                bCollideBellow = false;
            }

            if (bCollide)
            {
                int row = 0;

                if (bCollideAbove)
                {
                    row = (int)Math.Floor((BoundingBox.Y + mMap.TileHeight / 2.0f) / mMap.TileHeight) - 1;
                }
                else if (bCollideBellow)
                {
                    row = (int)Math.Floor((BoundingBox.Bottom + mMap.TileHeight / 2.0f) / mMap.TileHeight);
                }

                if (row > mMap.Height)
                    row = mMap.Height;
                else if (row < -1)
                    row = -1;

                Rectangle collidingTile = new Rectangle((int)Position.X, row * mMap.TileHeight, mMap.TileWidth, mMap.TileHeight);

                if (bCollideAbove)
                {
                    OnCollisionWithBox(collidingTile, ECollisionSide.TOP);
                }
                else if (bCollideBellow)
                {
                    OnCollisionWithBox(collidingTile, ECollisionSide.BOTTOM);
                }

                vy = 0;
            }
        }

        #endregion

        #region AnimatedSprite Methods

        public override void UpdateBoundingBox()
        {
            // The collision is at the feet of the player
            BoundingBox = new Rectangle(
            (int)(Position.X + 5),
            (int)(Position.Y + 21),
            10,
            10
            );
        }
        public override void Update(GameTime gameTime)
        {
            vx = 0;
            vy = 0;

            float velocityValue = mSpeed * 60 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Input.GetKey(Keys.W))
            {
                vy -= velocityValue;
                AnimState = EAnimState.WALK_UP;
            }
            else if (Input.GetKey(Keys.S))
            {
                vy += velocityValue;
                AnimState = EAnimState.WALK_DOWN;
            }
            
            if (Input.GetKey(Keys.A))
            {
                vx -= velocityValue;
                AnimState = EAnimState.WALK_LEFT;
            }
            else if (Input.GetKey(Keys.D))
            {
                vx += velocityValue;
                AnimState = EAnimState.WALK_RIGHT;
            }

            if(vx == 0 && vy == 0)
            {
                StopAnim();
                CurrentAnimation.CurrentFrame = 0;
            }

            ManageCollision();

            base.Update(gameTime);
        }

        #endregion
    }
}
