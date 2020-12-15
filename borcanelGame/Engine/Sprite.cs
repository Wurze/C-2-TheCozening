using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace borcanelGame.Engine
{
    /// <summary>
    /// A sprite that use Texture2D to be draw
    /// And contains movement methods
    /// </summary>
    public class Sprite : IActor
    {
        #region Fields

        public float vx;
        public float vy;

        private Vector2 mPosition;

        #endregion

        #region IActor Properties

        public Vector2 Position
        {
            get
            {
                return mPosition;
            }
            set
            {
                mPosition = value;
                UpdateBoundingBox();
            }
        }

        public Rectangle BoundingBox { get; protected set; }

        public bool ToRemove { get; set; }
        public bool IsActive { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Indicate the origin of the sprite( by default upper corner )
        /// </summary>
        public Vector2 Origin { get; set; }

        /// <summary>
        /// Filter Color used in draw ( ex : if you draw a white texture with a red Filter Color a red texture will be draw)
        /// </summary>
        public Color FilterColor { get; set; }

        /// <summary>
        /// Sprite opacity to draw sprite with transparency or not
        /// </summary>
        public float Alpha { get; set; }

        /// <summary>
        /// Scale sprite to have the sprite bigger or smaller than the original culture
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// Sprite texture
        /// </summary>
        public Texture2D Texture { get; protected set; }

        /// <summary>
        /// Rectangle occupy by the sprite
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    (int)(Position.X - (int)Math.Ceiling(Origin.X)),
                    (int)(Position.Y - (int)Math.Ceiling(Origin.Y)),
                    (int)Math.Ceiling((double)Texture.Width),
                    (int)Math.Ceiling((double)Texture.Height)
                    );
            }
        }

        #endregion

        #region Constructors

        public Sprite(Texture2D texture) : base()
        {
            FilterColor = Color.White;
            IsActive = true; // don't forget to set IsActive to true
            Alpha = 1;
            Scale = 1;

            Origin = Vector2.Zero;

            Texture = texture;

            FilterColor = Color.White;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update Bounding Box according to Position, Origin and Texture size
        /// </summary>
        public virtual void UpdateBoundingBox()
        {
            BoundingBox = new Rectangle(
                (int)(Position.X - (int)Math.Ceiling(Origin.X)),
                (int)(Position.Y - (int)Math.Ceiling(Origin.Y)),
                (int)Math.Ceiling((double)Texture.Width),
                (int)Math.Ceiling((double)Texture.Height)
                );
        }

        /// <summary>
        /// Center origin in the center of the texture
        /// </summary>
        public void CenterOrigin()
        {
            Origin = new Vector2(Texture.Width / 2.0f, Texture.Height / 2.0f);

            UpdateBoundingBox();
        }

        public void Move(float x, float y)
        {
            Position = new Vector2(Position.X + x, Position.Y + y);
        }

        #endregion

        #region IActor Methods

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                Vector2 drawPosition = new Vector2((int)Math.Round(Position.X), (int)Math.Round(Position.Y));

                spriteBatch.Draw(Texture, drawPosition, origin: Origin,scale : new Vector2(Scale), color: FilterColor * Alpha);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if (IsActive)
                Move(vx, vy);
        }

        #endregion
    }
}