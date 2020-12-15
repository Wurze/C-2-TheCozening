using borcanelGame.Engine;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace borcanelGame.Gameplay
{
    class ItemOnFloor : Sprite
    {
        #region Properties

        public EItemType ItemType { get; private set; }

        #endregion

        #region Constructor

        public ItemOnFloor(Texture2D texture, EItemType itemType) : base(texture)
        {
            ItemType = itemType;
        }

        #endregion
    }
}
