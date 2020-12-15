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
    /// Represent something that can be draw on scene ( Sprites, Maps, etc...)
    /// It's made to be draw by a scene object
    /// </summary>
    public interface IActor
    {
        Vector2 Position { get; set; }

        /// <summary>
        /// Indicate the rectangle box to use in collision
        /// </summary>
        Rectangle BoundingBox { get; }

        /// <summary>
        /// Indicate that this actor need to be remove
        /// </summary>
        bool ToRemove { get; set; }

        /// <summary>
        /// If IsActive is equal to false this actor will not be updated and draw
        /// </summary>
        bool IsActive { get; set; }

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);
    }
}
