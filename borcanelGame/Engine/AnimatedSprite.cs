using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace borcanelGame.Engine
{
    class AnimatedSprite : Sprite
    {
        #region Fields

        Dictionary<string, Animation> mAnimations;

        float mTime = 0;

        #endregion

        #region Properties

        protected Animation CurrentAnimation { get; set; }
        public string CurrentKeyAnim { get; protected set; }

        public bool IsPlayed { get; protected set; }

        #endregion

        #region Constructor & Init

        public AnimatedSprite(Dictionary<string,Animation> animations) : base(animations.ElementAt(0).Value.Texture)
        {
            Init(animations);
        }

        public AnimatedSprite() : base(null)
        {
            mAnimations = new Dictionary<string, Animation>();
        }

        public void Init(Dictionary<string, Animation> animations)
        {
            mAnimations = animations;
        }

        #endregion

        #region Methods

        public virtual void SetAnimation(string name)
        {
            if (!mAnimations.ContainsKey(name)) return;

            CurrentKeyAnim = name;

            CurrentAnimation = mAnimations[CurrentKeyAnim];

            Texture = CurrentAnimation.Texture;

            UpdateBoundingBox();

            IsPlayed = true;
        }

        public void StopAnim()
        {
            IsPlayed = false;
        }

        public void PlayAnim()
        {
            IsPlayed = true;
        }

        #endregion

        #region Sprite Methods

        public override void UpdateBoundingBox()
        {
            int width = 0;
            int height = 0;

            if (CurrentAnimation != null)
            {
                width = CurrentAnimation.FrameWidth;
                height = CurrentAnimation.FrameHeight;
            }

            BoundingBox = new Rectangle(
                (int)(Position.X - (int)Math.Ceiling(Origin.X)),
                (int)(Position.Y - (int)Math.Ceiling(Origin.Y)),
                width, height);
        }

        public override void Update(GameTime gameTime)
        {
            if (CurrentAnimation == null || !IsPlayed)
                return;

            mTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (mTime > CurrentAnimation.FrameSpeed)
            {
                mTime = 0f;

                CurrentAnimation.CurrentFrame++;

                if (CurrentAnimation.CurrentFrame >= CurrentAnimation.FrameCount)
                    CurrentAnimation.CurrentFrame = 0;
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentAnimation == null) return;

            spriteBatch.Draw(CurrentAnimation.Texture, Position,new Rectangle(CurrentAnimation.CurrentFrame * CurrentAnimation.FrameWidth,0,CurrentAnimation.FrameWidth,CurrentAnimation.FrameHeight), FilterColor, 0, Origin, Vector2.One, SpriteEffects.None,1);
        }

        #endregion
    }
}
