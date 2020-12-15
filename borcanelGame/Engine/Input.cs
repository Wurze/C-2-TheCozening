using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace borcanelGame.Engine
{
    public enum MouseButtons
    {
        Left,
        Right,
        Middle
    }

    /// <summary>
    /// Static class to manage input ( Keyboard, Mouse, Gamepad)
    /// </summary>
    public static class Input
    {
        #region Static

        // Private ---------------------------------------------------

        // The keyboardState of the previous frame
        static KeyboardState mOldKBState;
        // The keyboardState of this frame
        static KeyboardState mNewKBState;
        static MouseState mOldMouseState;
        static MouseState mNewMouseState;

        static List<GamePadState> mListOldGPState;
        static List<GamePadState> mListNewGPState;

        #endregion

        #region Fields

        // Public ----------------------------------------------------

        // Private ---------------------------------------------------

        #endregion

        #region Properties

        static public bool AllowClick { get; set; } = true;

        static public int MouseMoveX
        {
            get
            {
                return mNewMouseState.Position.X - mOldMouseState.Position.X;
            }
        }

        static public int MouseMoveY
        {
            get
            {
                return mNewMouseState.Position.Y - mOldMouseState.Position.Y;
            }
        }

        #endregion

        #region Constructors

        #endregion

        #region Methods

        // Public ----------------------------------------------------

        static public bool IsScrollDown()
        {
            if (mOldMouseState.ScrollWheelValue
                > mNewMouseState.ScrollWheelValue)
            {
                return true;
            }

            return false;
        }

        static public bool IsScrollUp()
        {
            if (mOldMouseState.ScrollWheelValue
                < mNewMouseState.ScrollWheelValue)
            {
                return true;
            }

            return false;
        }

        static public int ScrollWheelValue
        {
            get
            {
                if (mNewMouseState.ScrollWheelValue !=
                    mOldMouseState.ScrollWheelValue)
                {
                    return mNewMouseState.ScrollWheelValue;
                }

                return 0;
            }
        }

        /// <summary>
        /// Called to getState
        /// </summary>
        static public void GetState()
        {
            mNewKBState = Keyboard.GetState();
            mNewMouseState = Mouse.GetState();

            mListNewGPState = new List<GamePadState>();

            for (int i = 0; i < GamePad.MaximumGamePadCount; i++)
            {
                GamePadCapabilities capabilities = GamePad.GetCapabilities(i);

                if (capabilities.IsConnected)
                {
                    mListNewGPState.Add(GamePad.GetState(i, GamePadDeadZone.IndependentAxes));
                }
            }
        }

        /// <summary>
        /// Called to reinitialize State
        /// </summary>
        static public void ReinitializeState()
        {
            mOldKBState = mNewKBState;
            mOldMouseState = mNewMouseState;

            mListOldGPState = mListNewGPState;
        }

        /// <summary>
        /// Happen when the player release the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static public bool GetKeyUp(Keys key)
        {
            if (mNewKBState.IsKeyUp(key) && !mOldKBState.IsKeyUp(key))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Happen when the player press the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static public bool GetKeyDown(Keys key)
        {
            if (mNewKBState.IsKeyDown(key) && !mOldKBState.IsKeyDown(key))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Happen when the player hold the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static public bool GetKey(Keys key)
        {
            if (mNewKBState.IsKeyDown(key) && mOldKBState.IsKeyDown(key))
            {
                return true;
            }
            return false;
        }

        static public Point MousePosition
        {
            get
            {
                Vector2 position = mNewMouseState.Position.ToVector2();

                return position.ToPoint();
            }
        }

        static public bool GetMouseButtonUp(MouseButtons button)
        {
            if (!AllowClick)
                return false;

            switch (button)
            {
                case MouseButtons.Left:
                    if (mNewMouseState.LeftButton == ButtonState.Released &&
                        mOldMouseState.LeftButton != ButtonState.Released)
                    {
                        return true;
                    }
                    break;
                case MouseButtons.Right:
                    if (mNewMouseState.RightButton == ButtonState.Released &&
                        mOldMouseState.RightButton != ButtonState.Released)
                    {
                        return true;
                    }
                    break;
                case MouseButtons.Middle:
                    if (mNewMouseState.MiddleButton == ButtonState.Released &&
                        mOldMouseState.MiddleButton != ButtonState.Released)
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

        static public bool GetMouseButtonDown(MouseButtons button)
        {
            if (!AllowClick)
                return false;

            switch (button)
            {
                case MouseButtons.Left:
                    if (mNewMouseState.LeftButton == ButtonState.Pressed &&
                        mOldMouseState.LeftButton != ButtonState.Pressed)
                    {
                        return true;
                    }
                    break;
                case MouseButtons.Right:
                    if (mNewMouseState.RightButton == ButtonState.Pressed &&
                        mOldMouseState.RightButton != ButtonState.Pressed)
                    {
                        return true;
                    }
                    break;
                case MouseButtons.Middle:
                    if (mNewMouseState.MiddleButton == ButtonState.Pressed &&
                        mOldMouseState.MiddleButton != ButtonState.Pressed)
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

        static public bool GetMouseButton(MouseButtons button)
        {
            if (!AllowClick)
                return false;

            switch (button)
            {
                case MouseButtons.Left:
                    if (mNewMouseState.LeftButton == ButtonState.Pressed &&
                        mOldMouseState.LeftButton == ButtonState.Pressed)
                    {
                        return true;
                    }
                    break;
                case MouseButtons.Right:
                    if (mNewMouseState.RightButton == ButtonState.Pressed &&
                        mOldMouseState.RightButton == ButtonState.Pressed)
                    {
                        return true;
                    }
                    break;
                case MouseButtons.Middle:
                    if (mNewMouseState.MiddleButton == ButtonState.Pressed &&
                        mOldMouseState.MiddleButton == ButtonState.Pressed)
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

        static public bool GetButtonUp(int playerId, Buttons button)
        {
            if (mListOldGPState == null || mListOldGPState.Count == 0 || mListNewGPState.Count == 0)
                return true;

            if (playerId < 0 || playerId > mListNewGPState.Count || playerId > mListOldGPState.Count)
                return true;

            GamePadState newGPState = mListNewGPState[playerId];
            GamePadState oldGPState = mListOldGPState[playerId];

            if (newGPState.IsButtonUp(button) && !oldGPState.IsButtonUp(button))
            {
                return true;
            }
            return false;
        }

        static public bool GetButtonDown(int playerId, Buttons button)
        {
            if (mListOldGPState == null || mListOldGPState.Count == 0 || mListNewGPState.Count == 0)
                return false;

            if (playerId < 0 || playerId > mListNewGPState.Count || playerId > mListOldGPState.Count)
                return false;

            GamePadState newGPState = mListNewGPState[playerId];
            GamePadState oldGPState = mListOldGPState[playerId];

            if (newGPState.IsButtonDown(button) && !oldGPState.IsButtonDown(button))
            {
                return true;
            }
            return false;
        }

        static public bool GetButton(int playerId, Buttons button)
        {
            if (mListOldGPState == null || mListOldGPState.Count == 0 || mListNewGPState.Count == 0)
                return false;

            if (playerId < 0 || playerId > mListNewGPState.Count || playerId > mListOldGPState.Count)
                return false;

            GamePadState newGPState = mListNewGPState[playerId];
            GamePadState oldGPState = mListOldGPState[playerId];

            if (newGPState.IsButtonDown(button) && oldGPState.IsButtonDown(button))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
