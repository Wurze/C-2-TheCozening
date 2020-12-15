using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace borcanelGame.Engine
{
    public class Camera
    {
        #region Fields

        // Public ----------------------------------------------------------------------

        public static Camera Instance { get; set; }

        public Matrix ViewMatrix;

        public float TranslationSpeed = 60.0f;

        /// <summary>
        /// Area where the game is draw ( NOT UI ) 
        /// </summary>
        public Rectangle GameField;

        // Protected  ------------------------------------------------------------------

        // Private ---------------------------------------------------------------------

        private Rectangle mZoneGame;
        private Viewport mView;

        #endregion

        #region Properties

        public Vector2 Position { get; private set; }

        public Rectangle CameraBox { get; private set; }

        #endregion

        #region Constructors

        public Camera(Viewport pView, Rectangle gameField)
        {
            mView = pView;

            Position = Vector2.Zero;

            mZoneGame = new Rectangle(0, 0, pView.Width, pView.Height);

            GameField = gameField;
        }

        #endregion

        #region Methods

        public static void Init(Vector2 pPosition, Rectangle pZoneGame)
        {
            Instance.Position = pPosition;

            Instance.mZoneGame = pZoneGame;

            Matrix position = Matrix.CreateTranslation(pPosition.X, pPosition.Y, 0);

            var offset = Matrix.CreateTranslation(
                (float)Math.Round(Instance.GameField.Width / 2.0f),
                (float)Math.Round(Instance.GameField.Height / 2.0f),
                0);

            Instance.ViewMatrix = position * offset;
        }

        public static void Update(GameTime pGameTime)
        {
            Instance.CameraBox = new Rectangle(-(int)Instance.Position.X - Instance.GameField.Width / 2, -(int)Instance.Position.Y - Instance.GameField.Height / 2, Instance.GameField.Width, Instance.GameField.Height);
        }

        public static void MoveToTarget(IActor target, Rectangle pZoneGame, GameTime pGameTime)
        {
            Vector2 centerScreen = Instance.GameField.Center.ToVector2();

            Vector2 translationVector = -target.Position;

            // Center the player on screen
            float minTranslationX = -(pZoneGame.Width - centerScreen.X);
            float maxTranslationX = -(pZoneGame.X + centerScreen.X);

            float minTranslationY = -(pZoneGame.Height - centerScreen.Y);
            float maxTranslationY = -(pZoneGame.Y + centerScreen.Y);

            if (translationVector.X < minTranslationX)
                translationVector.X = minTranslationX;
            else if (translationVector.X > maxTranslationX)
                translationVector.X = maxTranslationX;
            if (translationVector.Y < minTranslationY)
                translationVector.Y = minTranslationY;
            else if (translationVector.Y > maxTranslationY)
                translationVector.Y = maxTranslationY;

            Vector2 newPos = Instance.Position;

            float difX = translationVector.X - Instance.Position.X;
            float difY = translationVector.Y - Instance.Position.Y;

            if (Math.Abs(difX) > 2)
                newPos.X += (difX / 20) * 60 * (float)pGameTime.ElapsedGameTime.TotalSeconds;
            if (Math.Abs(difY) > 2)
                newPos.Y += (difY / 20) * 60 * (float)pGameTime.ElapsedGameTime.TotalSeconds;

            Instance.Position = newPos;

            Matrix position = Matrix.CreateTranslation(
                (int)Math.Round(Instance.Position.X),
                (int)Math.Round(Instance.Position.Y),
                0);

            var offset = Matrix.CreateTranslation(
                (int)Math.Round(Instance.GameField.Width / 2.0f),
                (int)Math.Round(Instance.GameField.Height / 2.0f),
                0);

            Instance.ViewMatrix = position * offset;
        }

        #endregion
    }
}
