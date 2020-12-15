using borcanelGame.Engine;
using borcanelGame.Gameplay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace borcanelGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // all entity displayed on the map will be add to this list ( cleaner to manage than haveing a variable for all entities)
        List<IActor> listEntities;

        Sprite npc;
        Player player;
        LevelMap map;

        List<UIItem> uiItems;

        // SpriteFont use for dialogue
        SpriteFont mFont;

        // Game menu
        Menu menu;

        // dialogue text displayed when you interact with npc
        UIText dialogueUIText;

        // all dialogue availabe
        const string NO_DIALOGUE = "...";
        const string FIND_GOLD_KEY = "Thanks for my parchment ! Go find the gold key !";
        const string FIND_GRAY_KEY = "OOOOH ! You have the gold key ! Go find the gray one !";
        const string END_GAME_TEXT = "Good job ! You won !";

        // when you set this variable to false all entities will not be updated but still draw
        bool bUpdateActors = true;

        // indicate that the new item was generated
        bool bGeneratedItem = false;
        // indicate the item to find
        EItemType currentItemType = EItemType.NONE;

        // Text displayed when the player win the game
        UIText victoryText;

        bool bWin;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            listEntities = new List<IActor>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            mFont = Content.Load<SpriteFont>("Fonts/mainFont");

            menu = new Menu(mFont, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            map = new LevelMap(Content, "Maps/map");

            player = new Player(map);
            map.Player = player;

            Dictionary<string, Animation> dicAnim = new Dictionary<string, Animation>
            {
                { "Walking_Down", new Animation(Content.Load<Texture2D>("Images/WalkingDown"),3) },
                { "Walking_Left", new Animation(Content.Load<Texture2D>("Images/WalkingLeft"),3) },
                { "Walking_Right", new Animation(Content.Load<Texture2D>("Images/WalkingRight"),3) },
                { "Walking_Up", new Animation(Content.Load<Texture2D>("Images/WalkingUp"),3) }
            };

            player.Init(dicAnim);

            uiItems = new List<UIItem>();

            UIItem parchmentUI = new UIItem(Content.Load<Texture2D>("Images/Parchment"), EItemType.PARCHMENT);
            // deactivate parchementUI it will be activated when the parchment will be picked
            parchmentUI.IsActive = false;

            UIItem key1UI = new UIItem(Content.Load<Texture2D>("Images/Key1"), EItemType.KEY_1);
            // deactivate key1UI it will be activated when the key_1 will be picked
            key1UI.IsActive = false;
            key1UI.Position = new Vector2(0, parchmentUI.Bounds.Bottom + 20);

            UIItem key2UI = new UIItem(Content.Load<Texture2D>("Images/Key2"), EItemType.KEY_2);
            // deactivate key2UI it will be activated when the object_2 will be picked
            key2UI.IsActive = false;
            key2UI.Position = new Vector2(0, key1UI.Bounds.Bottom + 20);

            uiItems.Add(parchmentUI);
            uiItems.Add(key1UI);
            uiItems.Add(key2UI);

            dialogueUIText = new UIText(mFont)
            {
                Position = new Vector2(0, graphics.PreferredBackBufferHeight - 50),
                IsActive = false,
                Text = "..."
            };

            victoryText = new UIText(mFont)
            {
                IsActive = false,
                Text = "VICTORY",
                TextColor = Color.Red
            };

            victoryText.CenterOnScreen(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            Camera.Instance = new Camera(GraphicsDevice.Viewport, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
            Camera.Init(Vector2.Zero, map.Bounds);

            InitGame();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private ItemOnFloor GenerateItem(EItemType itemType)
        {
            ItemOnFloor item = null;

            switch (itemType)
            {
                case EItemType.PARCHMENT:
                    item = new ItemOnFloor(Content.Load<Texture2D>("Images/Parchment"), itemType);
                    break;
                case EItemType.KEY_1:
                    item = new ItemOnFloor(Content.Load<Texture2D>("Images/Key1"), itemType);
                    break;
                case EItemType.KEY_2:
                    item = new ItemOnFloor(Content.Load<Texture2D>("Images/Key2"), itemType);
                    break;
                default:
                    break;
            }

            if(item != null)
            {
                item.Position = map.GetRandomFreePosition(0, map.Width - 1, 0, map.Height - 1);
                item.IsActive = false;
                return item;
            }

            return null;
        }

        void InitGame()
        {
            if(listEntities != null)
            {
                listEntities.Clear();
            }

            if(uiItems != null)
            {
                foreach (var item in uiItems)
                {
                    item.IsActive = false;
                }
            }

            currentItemType = EItemType.NONE;

            // init the player position with the one in the map
            player.Position = map.PlayerPosition;
            player.AnimState = Player.EAnimState.WALK_DOWN;

            npc = new Sprite(Content.Load<Texture2D>("Images/Npc"));
            npc.Position = map.NpcPosition;
            npc.IsActive = false;

            ItemOnFloor parchment = GenerateItem(EItemType.PARCHMENT);
            listEntities.Add(parchment);
            parchment.IsActive = false;

            listEntities.Add(npc);
            listEntities.Add(player);

            victoryText.IsActive = false;
            bWin = false;
            map.EnableDarkMode = true;
        }

        void DrawGame(GameTime gameTime)
        {
            // use sampler that clamp at pixel
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.Instance.ViewMatrix);

            map.DrawFloor(spriteBatch);
            map.DrawWall(spriteBatch);

            // draw all entities that need to be draw
            foreach (var item in listEntities)
            {
                if (item.IsActive)
                {
                    item.Draw(spriteBatch);
                }
            }

            map.DrawOnWall(spriteBatch);
            map.DrawLight(spriteBatch);

            spriteBatch.End();
        }

        void UpdateGame(GameTime gameTime)
        {
            // when this code is #if DEBUG it will be execute only in debug
            #if DEBUG

            // enabl or disable map darkmode for the easy mode
            if (Input.GetKeyDown(Keys.LeftControl))
            {
                map.EnableDarkMode = !map.EnableDarkMode;
            }

            #endif

            if (bUpdateActors)
            {
                Vector2 oldPlayerPos = player.Position;

                map.Update(gameTime);

                // update all entities
                foreach (var entity in listEntities)
                {
                    // draw entity only if this entity is active
                    if (entity.IsActive)
                    {
                        entity.Update(gameTime);

                        // manage player collision with npc
                        if (entity == player)
                        {
                            // if we have collision we set the player position to the old position
                            // I implement a different way to do it with tile because you can use it for all type of games
                            // ( 2D top view & platform for example )
                            if (player.BoundingBox.Intersects(npc.BoundingBox))
                            {
                                player.Position = oldPlayerPos;
                            }
                        }

                        // if this entity is an Item on floor
                        if (entity is ItemOnFloor item)
                        {
                            // test if the player is on the item to pick up
                            if (player.InteractionBox.Intersects(item.BoundingBox))
                            {
                                // tell the game that this item need to be removed to not draw it & update it
                                item.ToRemove = true;

                                // Find the ui item that match the item
                                UIItem uiItem = uiItems.Find(ui => ui.ItemType == item.ItemType);

                                if (uiItem != null)
                                {
                                    // Set UI item to true to tell the player that he picked the item
                                    uiItem.IsActive = true;
                                }

                                switch (item.ItemType)
                                {
                                    case EItemType.PARCHMENT:
                                        dialogueUIText.Text = FIND_GOLD_KEY;
                                        currentItemType = EItemType.KEY_1;
                                        bGeneratedItem = false;
                                        break;
                                    case EItemType.KEY_1:
                                        dialogueUIText.Text = FIND_GRAY_KEY;
                                        currentItemType = EItemType.KEY_2;
                                        bGeneratedItem = false;
                                        break;
                                    case EItemType.KEY_2:
                                        dialogueUIText.Text = END_GAME_TEXT;
                                        currentItemType = EItemType.NONE; // None will means that the game is finished
                                        bGeneratedItem = false;
                                        break;
                                }
                            }
                        }
                    }

                    // if entity is not the player
                    if (entity != player)
                    {
                        if (map.EnableDarkMode)
                        {

                            // test if the entity can be see by the player

                            Point lightCenter = map.LightTrickSprite.Bounds.Center;
                            int lightRadius = map.LightTrickSprite.Bounds.Width / 2;

                            // test if the distance between the entity and the center of the light if inferior to light radius

                            entity.IsActive = Vector2.Distance(lightCenter.ToVector2(), entity.BoundingBox.Center.ToVector2()) <= lightRadius;
                        }
                        else
                        {
                            entity.IsActive = true;
                        }
                    }
                }
            }

            // Interact with npc only if the game is not finished
            if (Input.GetKeyDown(Keys.E) && !bWin)
            {
                if (player.InteractionBox.Intersects(npc.BoundingBox))
                {
                    // Pressing E will activate dialogue or deactivate it
                    dialogueUIText.IsActive = !dialogueUIText.IsActive;

                    if (!dialogueUIText.IsActive)
                    {
                        if (currentItemType == EItemType.NONE)
                        {
                            if (dialogueUIText.Text == END_GAME_TEXT)
                            {
                                bWin = true;
                                victoryText.IsActive = true;
                            }
                        }
                        // This happen only if the new item wasn't generate
                        else if (!bGeneratedItem)
                        {
                            listEntities.Add(GenerateItem(currentItemType));
                            bGeneratedItem = true;
                        }
                    }
                }
            }
            else if (Input.GetKeyDown(Keys.Escape))
            {
                menu.Reset();
            }

            // deactivate all entities if the player is in dialogue & if the game is finished to avoid something from moving
            bUpdateActors = !(dialogueUIText.IsActive || bWin);

            Camera.MoveToTarget(player, map.Bounds, gameTime);

            // we remove all actors that need to be removed ( Always do that at the end of update to make sure you don't need to access to a removed entities)
            listEntities.RemoveAll(actor => actor.ToRemove);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Input.GetState();

            if(menu.IsActive)
            {
                menu.Update(gameTime);
                menu.IsActive = menu.ValidateChoice != Menu.EChoice.START_GAME;
                InitGame();
            }
            else
            {
                UpdateGame(gameTime);
            }

            base.Update(gameTime);

            Input.ReinitializeState();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if(!menu.IsActive)
            {
                DrawGame(gameTime);
            }

            // spriteBatch for UI what you draw here will not be affected by the camera
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if(!menu.IsActive)
            {
                foreach (var item in uiItems)
                {
                    if (item.IsActive)
                    {
                        item.Draw(spriteBatch);
                    }
                }

                if (dialogueUIText.IsActive)
                {
                    dialogueUIText.Draw(spriteBatch);
                }
                else if (victoryText.IsActive)
                {
                    victoryText.Draw(spriteBatch);
                }
            }
            else
            {
                menu.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
