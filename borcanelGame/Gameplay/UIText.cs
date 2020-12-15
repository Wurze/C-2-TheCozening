using borcanelGame.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace borcanelGame.Gameplay
{
    // class use to display text on the screen
    class UIText : IActor
    {
        #region Fields

        SpriteFont mFont;

        #endregion

        #region Properties

        public Color TextColor { get; set; } = Color.White;

        public string Text { get; set; }

        public Vector2 Size
        {
            get
            {
                return mFont.MeasureString(Text);
            }
        }

        #endregion

        #region IActor Properties

        public Vector2 Position { get; set; }

        // BoundingBox can be usefull if you want to make more complex UI 
        public Rectangle BoundingBox { get; private set; }

        public bool ToRemove { get; set; }
        public bool IsActive { get; set; }

        #endregion

        #region Constructors

        public UIText(SpriteFont font)
        {
            mFont = font;

            IsActive = true;
        }

        #endregion

        #region Methods

        public void CenterOnScreen(int screenWidth,int screenHeight)
        {
            // Measure text size to place the text in the center of the screen
            Position = new Vector2(screenWidth / 2 - Size.X/2, screenHeight / 2 - Size.Y/2);
        }

        #endregion

        #region IActor Methods

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(mFont, Text, Position, TextColor);
        }

        public void Update(GameTime gameTime)
        {
            
        }

        #endregion
    }
}
