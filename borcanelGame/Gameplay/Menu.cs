using borcanelGame.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace borcanelGame.Gameplay
{
    class Menu : IActor
    {
        #region Fields

        public enum EChoice { START_GAME, HOW_TO_PLAY, NONE }

        // Text displayed in the menu
        List<UIText> mChoices;

        UIText mHowToPlayInfoText;

        int mCurrentChoiceId;

        const string START_GAME_TEXT = "Start Game";
        const string HOW_TO_PLAY_TEXT = "How to play";
        const string HOW_TO_PLAY_INFO_TEXT = "Use W,A,S,D to move the player\nE to interact with npc\nSpace to validate a choice";

        #endregion

        #region Properties

        // the choice that the player validate
        public EChoice ValidateChoice { get; private set; }

        #endregion

        #region IActor Properties

        public Vector2 Position { get; set; }

        public Rectangle BoundingBox { get; private set; }

        public bool ToRemove { get; set; }
        public bool IsActive { get; set; }

        #endregion

        #region Constructor

        public Menu(SpriteFont font,int screenWidth,int screenHeight)
        {
            IsActive = true;

            UIText startGameText = new UIText(font);
            startGameText.Text = START_GAME_TEXT;
            startGameText.TextColor = Color.Red;
            startGameText.CenterOnScreen(screenWidth, screenHeight);
            
            Vector2 pos = startGameText.Position;
            pos.Y -= startGameText.Size.Y;
            startGameText.Position = pos;

            UIText howToPlayText = new UIText(font);
            howToPlayText.Text = HOW_TO_PLAY_TEXT;
            howToPlayText.CenterOnScreen(screenWidth, screenHeight);

            pos = howToPlayText.Position;
            pos.Y += howToPlayText.Size.Y;

            howToPlayText.Position = pos;

            // Set the current choice of the menu
            mCurrentChoiceId = 0;

            mChoices = new List<UIText>();
            mChoices.Add(startGameText);
            mChoices.Add(howToPlayText);

            ValidateChoice = EChoice.NONE;

            mHowToPlayInfoText = new UIText(font);
            mHowToPlayInfoText.Text = HOW_TO_PLAY_INFO_TEXT;
            mHowToPlayInfoText.CenterOnScreen(screenWidth, screenHeight);
        }

        #endregion

        #region Methods

        public void Reset()
        {
            IsActive = true;

            ValidateChoice = EChoice.NONE;

            foreach (var choice in mChoices)
            {
                choice.TextColor = Color.White;
            }

            mCurrentChoiceId = 0;

            mChoices[mCurrentChoiceId].TextColor = Color.Red;
        }

        #endregion

        #region IActor Methods

        public void Draw(SpriteBatch spriteBatch)
        {
            // if the choice how to play is selected we don't draw the menu
            if (ValidateChoice != EChoice.HOW_TO_PLAY)
            {
                foreach (var choice in mChoices)
                {
                    if (choice.IsActive)
                    {
                        choice.Draw(spriteBatch);
                    }
                }
            }
            else
            {
                mHowToPlayInfoText.Draw(spriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {
            // if the choice how to play is selected we will not update the menu
            if(ValidateChoice != EChoice.HOW_TO_PLAY)
            {
                UIText oldChoice = mChoices[mCurrentChoiceId];

                // move down in menu
                if (Input.GetKeyDown(Keys.Down))
                {
                    // Increment current choice, it will loop !
                    mCurrentChoiceId = (mCurrentChoiceId + 1) % mChoices.Count;
                }
                // move up in menu
                else if (Input.GetKeyDown(Keys.Up))
                {
                    // Decrement current choice, it will loop !
                    mCurrentChoiceId = Math.Abs(mCurrentChoiceId - 1) % mChoices.Count;
                }

                UIText newChoice = mChoices[mCurrentChoiceId];

                // Make sure to set oldchoice to the normal state & set the new choice to the new state
                if (oldChoice != newChoice)
                {
                    oldChoice.TextColor = Color.White;
                    newChoice.TextColor = Color.Red;
                }

                // validate choice
                if (Input.GetKeyDown(Keys.Space))
                {
                    if (newChoice.Text == START_GAME_TEXT)
                    {
                        ValidateChoice = EChoice.START_GAME;
                    }
                    else if (newChoice.Text == HOW_TO_PLAY_TEXT)
                    {
                        ValidateChoice = EChoice.HOW_TO_PLAY;
                    }
                }
            }
            // Go back to menu
            else if(Input.GetKeyDown(Keys.Space))
            {
                ValidateChoice = EChoice.NONE;
            }
        }

        #endregion
    }
}
