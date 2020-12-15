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
    class UIItem : Sprite
    {
        #region Properties

        public EItemType ItemType { get; private set; }

        #endregion

        #region Constructor

        public UIItem(Texture2D texture, EItemType itemType) : base(texture)
        {
            ItemType = itemType;
            Scale = 2;
        }

        #endregion
    }
}
