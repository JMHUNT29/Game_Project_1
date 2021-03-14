using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using Game_Project_1.Screens;
using Game_Project_1.StateManagement;

namespace Game_Project_1
{
    public class CatchTheEgg : Game
    {
        private GraphicsDeviceManager graphics;
        private readonly ScreenManager _screenManager;
        private SpriteBatch spriteBatch;

        private EggSprite[] eggs;
        private BirdSprite bird;
        private SpriteFont spriteFont;
        private int eggsGathered;
        private bool lose = false;

        private SoundEffect eggPickup;
        private Song backgroundMusic;

        private double time;

        /// <summary>
        /// A game where a bird tries to catch her egg before it falls off screen
        /// </summary>
        public CatchTheEgg()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Initializes the game 
        /// </summary>
        protected override void Initialize()
        {


            base.Initialize();
        }

        /// <summary>
        /// Loads content for the game
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
            foreach (var egg in eggs) egg.LoadContent(Content);
            bird.LoadContent(Content);
            spriteFont = Content.Load<SpriteFont>("bangers");
            eggPickup = Content.Load<SoundEffect>("birdchirping071414");
            backgroundMusic = Content.Load<Song>("Komiku - Tale on the Late - 13 The Wind");
            MediaPlayer.Play(backgroundMusic);
        }

        /// <summary>
        /// Updates the game world
        /// </summary>
        /// <param name="gameTime">The game time</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            // Detect and process collisions with egg and bird
            foreach (var egg in eggs)
            {
                egg.Update(gameTime);
                if (!egg.Collected && egg.Bounds.CollidesWith(bird.Bounds))
                {
                    egg.Collected = true;
                    eggsGathered++;
                    eggPickup.Play(volume: 0.5f, pitch: 0.0f, pan: 0.0f);
                }
            }

            bird.Update(gameTime);
            if (bird.position.X > GraphicsDevice.Viewport.Width) bird.position.X = GraphicsDevice.Viewport.Width;
            else if (bird.position.X < 64) bird.position.X = 64;
            else if (bird.position.Y > GraphicsDevice.Viewport.Height) bird.position.Y = GraphicsDevice.Viewport.Height;
            else if (bird.position.Y < 64) bird.position.Y = 64;

            if (lose == false && eggsGathered != 5)
            {
                time += gameTime.ElapsedGameTime.TotalSeconds;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the game
        /// </summary>
        /// <param name="gameTime">The game time</param>
        protected override void Draw(GameTime gameTime)
        {

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            GraphicsDevice.Clear(Color.SkyBlue);

            if (lose == false) spriteBatch.DrawString(spriteFont, $"Time: {time:0.##} seconds", new Vector2(455, 0), Color.FloralWhite);

            foreach (var egg in eggs)
            {
                if (lose == false) egg.Draw(gameTime, spriteBatch);
                if (egg.Bounds.Center.Y - 32 > GraphicsDevice.Viewport.Height) lose = true;
            }

            bird.Draw(gameTime, spriteBatch);

            if (eggsGathered < 5 && lose == true)
            {
                bird.Color = Color.Red;
                spriteBatch.DrawString(spriteFont, $"You Lose!", new Vector2((GraphicsDevice.Viewport.Width/2) - 64, (GraphicsDevice.Viewport.Height/2) - 32), Color.Red);
                MediaPlayer.Stop();
            }
            else if (eggsGathered == 5 && lose == false)
            {
                spriteBatch.DrawString(spriteFont, $"You Win!", new Vector2((GraphicsDevice.Viewport.Width / 2) - 64, (GraphicsDevice.Viewport.Height / 2)  - 32), Color.Gold);
                bird.Color = Color.Gold;
                MediaPlayer.Stop();
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
