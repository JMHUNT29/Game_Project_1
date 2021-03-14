using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Game_Project_1.StateManagement;

namespace Game_Project_1.Screens
{
    // This screen implements the actual game logic. It is just a
    // placeholder to get the idea across: you'll probably want to
    // put some more interesting gameplay in here!
    public class GameplayScreen : GameScreen
    {
        private ContentManager _content;
        private SpriteFont _font;
        public SpriteBatch SpriteBatch;

        private EggSprite[] eggs;
        private System.Random rand = new System.Random();
        private BirdSprite bird;
        private int eggsGathered;
        private bool lose = false;

        public SoundEffect eggPickup;
        public Song backgroundMusic;

        private double time;
        private double bestTime = 100;

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;

        private bool end;


        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Keys.Back, Keys.Escape }, true);

        }


        // Load graphics content for the game
        public override void Activate()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            end = false;

            eggs = new EggSprite[]
            {
                new EggSprite(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, (float)(rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Height * 0.7)), EggSprite.Side.Right),
                new EggSprite(new Vector2(0, (float)(rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Height * 0.7)), EggSprite.Side.Left),
                new EggSprite(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, (float)(rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Height * 0.7)), EggSprite.Side.Right),
                new EggSprite(new Vector2(0, (float)(rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Height * 0.7)), EggSprite.Side.Left),
                new EggSprite(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, (float)(rand.NextDouble() * ScreenManager.GraphicsDevice.Viewport.Height * 0.7)), EggSprite.Side.Right)
            };

            bird = new BirdSprite();

            MediaPlayer.Volume = ScreenManager.Volume;
            SoundEffect.MasterVolume = ScreenManager.Volume;

            SpriteBatch = ScreenManager.SpriteBatch;
            // TODO: use this.Content to load your game content here
            foreach (var egg in eggs) egg.LoadContent(_content);
            bird.LoadContent(_content);
            _font = _content.Load<SpriteFont>("bangers");
            eggPickup = _content.Load<SoundEffect>("birdchirping071414");
            backgroundMusic = _content.Load<Song>("Komiku - Tale on the Late - 13 The Wind");
            MediaPlayer.Play(backgroundMusic);

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(2500);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Unload()
        {
            _content.Unload();
        }

        // This method checks the GameScreen.IsActive property, so the game will
        // stop updating when the pause menu is active, or if you tab away to a different application.
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            else
                _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
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
                if (bird.position.X > ScreenManager.GraphicsDevice.Viewport.Width) bird.position.X = ScreenManager.GraphicsDevice.Viewport.Width;
                else if (bird.position.X < 64) bird.position.X = 64;
                else if (bird.position.Y > ScreenManager.GraphicsDevice.Viewport.Height) bird.position.Y = ScreenManager.GraphicsDevice.Viewport.Height;
                else if (bird.position.Y < 64) bird.position.Y = 64;

                if (lose == false && eggsGathered != 5)
                {
                    time += gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            if (end)
            {
                Thread.Sleep(2000);

                ScreenManager.RemoveScreen(this);
                ReplayGameScreen r = new ReplayGameScreen();
                ScreenManager.AddScreen(r, ControllingPlayer);
            }

        }

        // Unlike the Update method, this will only be called when the gameplay screen is active.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];

            PlayerIndex player;
            if (_pauseAction.Occurred(input, ControllingPlayer, out player))
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // Otherwise move the player position.
                var movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                if (movement.Length() > 1)
                    movement.Normalize();

            }
        }


        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.SkyBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            if (lose == false) spriteBatch.DrawString(_font, $"Time: {time:0.##} seconds", new Vector2(455, 0), Color.Gray);
            if (bestTime < 100) spriteBatch.DrawString(_font, $"Best Time: {bestTime:0.##} seconds", new Vector2(455, 30), Color.Gray);

            foreach (var egg in eggs)
            {
                if (lose == false) egg.Draw(gameTime, spriteBatch);
                if (egg.Bounds.Center.Y - 32 > ScreenManager.GraphicsDevice.Viewport.Height) lose = true;
            }

            bird.Draw(gameTime, spriteBatch);

            if (eggsGathered < 5 && lose == true)
            {
                bird.Color = Color.Red;
                spriteBatch.DrawString(_font, $"You Lose!", new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 2) - 64, (ScreenManager.GraphicsDevice.Viewport.Height / 2) - 32), Color.Red);
                MediaPlayer.Stop();
                if(time < bestTime) time = bestTime;
                end = true;
            }
            else if (eggsGathered == 5 && lose == false)
            {
                spriteBatch.DrawString(_font, $"You Win!", new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 2) - 64, (ScreenManager.GraphicsDevice.Viewport.Height / 2) - 32), Color.ForestGreen);
                bird.Color = Color.ForestGreen;
                MediaPlayer.Stop();
                end = true;
            }


            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }


        }
    }
}
