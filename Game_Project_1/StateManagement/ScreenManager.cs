﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Game_Project_1;


namespace Game_Project_1.StateManagement
{
    /// <summary>
    /// The ScreenManager is a component which manages one or more GameScreen instance.
    /// It maintains a stack of screens, calls their Update and Draw methods when 
    /// appropriate, and automatically routes input to the topmost screen.
    /// </summary>
    public class ScreenManager : DrawableGameComponent, IParticleEmitter
    {
        private readonly List<GameScreen> _screens = new List<GameScreen>();
        private readonly List<GameScreen> _tmpScreensList = new List<GameScreen>();

        private readonly ContentManager _content;
        private readonly InputState _input = new InputState();

        private bool _isInitialized;

        public RainParticleSystem _rain;
        public FireworkParticleSystem _fireworks;
        public PopParticleSystem _pop;


        public Vector2 Position { get; set; }

        public Vector2 Velocity { get; set; }

        /// <summary>
        /// A SpriteBatch shared by all GameScreens
        /// </summary>
        public SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// A SpriteFont shared by all GameScreens
        /// </summary>
        public SpriteFont Font { get; private set; }

        /// <summary>
        /// A blank texture that can be used by the screens.
        /// </summary>
        public Texture2D BlankTexture { get; private set; }

        /// <summary>
        /// Volume for the game's music.
        /// </summary>
        public float Volume = (float)0.6;

        /// <summary>
        /// Keeps track of games played
        /// </summary>
        public int gameCounter = 0;

        /// <summary>
        /// Keeps track of best time
        /// </summary>
        public double bestTime = 0;

        /// <summary>
        /// Constructs a new ScreenManager
        /// </summary>
        /// <param name="game">The game this ScreenManager belongs to</param>
        public ScreenManager(Game game) : base(game)
        {
            _content = new ContentManager(game.Services, "Content");
        }

        /// <summary>
        /// Initializes the ScreenManager
        /// </summary>
        public override void Initialize()
        {

            // TODO: Add your initialization logic here
            _rain = new RainParticleSystem(this.Game, new Rectangle(0, -50, 650, 827));
            this.Game.Components.Add(_rain);

            _fireworks = new FireworkParticleSystem(this.Game, 20);
            this.Game.Components.Add(_fireworks);

            _pop = new PopParticleSystem(this.Game, 15);
            this.Game.Components.Add(_pop);

            base.Initialize();
            _isInitialized = true;

        }

        /// <summary>
        /// Loads content for the ScreenManager and its screens
        /// </summary>
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Font = _content.Load<SpriteFont>("bangers");
            BlankTexture = _content.Load<Texture2D>("blank");
            // Tell each of the screens to load thier content 
            foreach (var screen in _screens)
            {
                screen.Activate();
            }
        }

        /// <summary>
        /// Unloads content for the ScreenManager's screens
        /// </summary>
        protected override void UnloadContent()
        {
            foreach (var screen in _screens)
            {
                screen.Unload();
            }
        }

        /// <summary>
        /// Updates all screens managed by the ScreenManager
        /// </summary>
        /// <param name="gameTime">An object representing time in the game</param>
        public override void Update(GameTime gameTime)
        {
            // Read in the keyboard and gamepad
            _input.Update();

            // Make a copy of the screen list, to avoid confusion if 
            // the process of updating a screen adds or removes others
            _tmpScreensList.Clear();
            _tmpScreensList.AddRange(_screens);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            while (_tmpScreensList.Count > 0)
            {
                // Pop the topmost screen 
                var screen = _tmpScreensList[_tmpScreensList.Count - 1];
                _tmpScreensList.RemoveAt(_tmpScreensList.Count - 1);

                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active)
                {
                    // if this is the first active screen, let it handle input 
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(gameTime, _input);
                        otherScreenHasFocus = true;
                    }

                    // if this is an active non-popup, all subsequent 
                    // screens are covered 
                    if (!screen.IsPopup) coveredByOtherScreen = true;
                }
            }
        }

        /// <summary>
        /// Draws the appropriate screens managed by the SceneManager
        /// </summary>
        /// <param name="gameTime">An object representing time in the game</param>
        public override void Draw(GameTime gameTime)
        {
            foreach (var screen in _screens)
            {
                if (screen.ScreenState == ScreenState.Hidden) continue;

                screen.Draw(gameTime);
            }
        }

        /// <summary>
        /// Adds a screen to the ScreenManager
        /// </summary>
        /// <param name="screen">The screen to add</param>
        public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
        {
            screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content
            if (_isInitialized) screen.Activate();

            _screens.Add(screen);
        }

        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload its content 
            if (_isInitialized) screen.Unload();

            _screens.Remove(screen);
            _tmpScreensList.Remove(screen);
        }

        /// <summary>
        /// Exposes an array holding all the screens managed by the ScreenManager
        /// </summary>
        /// <returns>An array containing references to all current screens</returns>
        public GameScreen[] GetScreens()
        {
            return _screens.ToArray();
        }

        // Helper draws a translucent black fullscreen sprite, used for fading
        // screens in and out, and for darkening the background behind popups.
        public void FadeBackBufferToBlack(float alpha)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(BlankTexture, GraphicsDevice.Viewport.Bounds, Color.Black * alpha);
            SpriteBatch.End();
        }

        public void Deactivate()
        {
        }

        public bool Activate()
        {
            return false;
        }
    }
}
