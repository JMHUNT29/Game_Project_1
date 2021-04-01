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
        private BalloonSprite[] balloons;
        private System.Random rand = new System.Random();
        private BirdSprite bird;

        private Texture2D heart;
        private int lives = 3;
        private bool lose = false;

        private Texture2D _foreground;
        private Texture2D _midground;
        private Texture2D _background;

        public SoundEffect eggPickup;
        public Song backgroundMusic;

        private int choice = 0;

        private double time;

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
                new EggSprite(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width + 60, 250), true),
                new EggSprite(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width + 60, 400), false),
                new EggSprite(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width + 60, 550), false),
                new EggSprite(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width + 60, 700), false)
            };

            balloons = new BalloonSprite[]
            {
                new BalloonSprite(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width + 64, 250), false),
                new BalloonSprite(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width + 64, 400), true),
                new BalloonSprite(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width + 64, 550), true),
                new BalloonSprite(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width + 64, 700), true)
            };

            bird = new BirdSprite();

            _foreground = _content.Load<Texture2D>("Rough_Game_Foreground");
            _midground = _content.Load<Texture2D>("Rough_Game_Hills");
            _background = _content.Load<Texture2D>("Rough_Game_Sky");

            MediaPlayer.Volume = ScreenManager.Volume;
            SoundEffect.MasterVolume = ScreenManager.Volume;

            SpriteBatch = ScreenManager.SpriteBatch;

            foreach (var egg in eggs) egg.LoadContent(_content);
            foreach (var balloon in balloons) balloon.LoadContent(_content);
            bird.LoadContent(_content);
            _font = _content.Load<SpriteFont>("bangers");
            eggPickup = _content.Load<SoundEffect>("birdchirping071414");
            backgroundMusic = _content.Load<Song>("Komiku - Tale on the Late - 13 The Wind");
            MediaPlayer.Play(backgroundMusic);

            heart = _content.Load<Texture2D>("New Piskel");

            //Pause to allow player chance to read instructions
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
                        if (egg.Lives == true)
                        {
                            if (lives < 5) lives++;
                            egg.Lives = false;
                        }
                        egg.Position = new Vector2(-64, 0);
                        eggPickup.Play(volume: 0.5f, pitch: 0.0f, pan: 0.0f);
                    }

                }

                foreach (var balloon in balloons)
                {
                    balloon.Update(gameTime);
                    if (!balloon.Hit && balloon.Bounds.CollidesWith(bird.Bounds))
                    {
                        lives--;
                        balloon.Hit = true;
                    }
                }


                if ((eggs[choice].Position.X < -64) || eggs[choice].Collected)
                {
                    if (eggs[choice].Lives) eggs[choice].Lives = false;
                    eggs[choice].Collected = false;

                    System.Random rand = new System.Random();
                    if (rand.NextDouble() > 0.90) eggs[choice].Lives = true;

                    eggs[choice].EggReset = false;
                    foreach (var b in balloons)
                    {
                        b.BalloonReset = false;
                        b.Hit = false;
                    }
                        

                    choice = rand.Next(0, 4);
                    eggs[choice].EggReset = true;

                    for (int i = 0; i <= balloons.Length - 1; i++)
                    {
                        if(i != choice)
                        {
                            balloons[i].BalloonReset = true;
                            balloons[i].Ready = true;

                        }
                    }

                    eggs[choice].Ready = true;

                }

                bird.Update(gameTime);

                if (bird.position.Y > ScreenManager.GraphicsDevice.Viewport.Height) bird.position.Y = ScreenManager.GraphicsDevice.Viewport.Height;
                else if (bird.position.Y < 64) bird.position.Y = 64;

                if (lose == false)
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

                // Otherwise move the player position.
            var movement = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.Up))
                movement.Y--;

            if (keyboardState.IsKeyDown(Keys.Down))
                movement.Y++;

            if (movement.Length() > 1)
                movement.Normalize();

        }


        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, new Color(131, 180, 199), 0, 0);

            // Our player and enemy are both actually just text strings.
            var spriteBatch = ScreenManager.SpriteBatch;

            // Calculate our offset vector
            float playerX = MathHelper.Clamp(bird.position.X, 300, 13600);
            float offsetX = 300 - playerX;

            Matrix transform;

            // Background
            transform = Matrix.CreateTranslation(offsetX * 0.333f, 0, 0);
            spriteBatch.Begin(transformMatrix: transform);
            spriteBatch.Draw(_background, new Vector2(0,0), Color.White);
            spriteBatch.End();

            // Midground
            transform = Matrix.CreateTranslation(offsetX * 0.666f, 0, 0);
            spriteBatch.Begin(transformMatrix: transform);
            spriteBatch.Draw(_midground, new Vector2(0, 275), Color.White);
            spriteBatch.End();

            //Foreground
            transform = Matrix.CreateTranslation(offsetX, 0, 0);
            spriteBatch.Begin(transformMatrix: transform);
            spriteBatch.Draw(_foreground, new Vector2(0, 915), Color.White);
            spriteBatch.End();

            spriteBatch.Begin();

            if (lose == false) spriteBatch.DrawString(_font, $"Time: {time:0.##} secs", new Vector2(400, 910), new Color(244, 255, 255));
            if (ScreenManager.gameCounter > 0) spriteBatch.DrawString(_font, $"Best Time: {ScreenManager.bestTime:0.##} secs", new Vector2(350, 955), new Color(244, 255, 255));

            
            foreach (var egg in eggs)
            {
                if (lose == false) egg.Draw(gameTime, spriteBatch);
                if ((egg.Bounds.Center.X - egg.Bounds.Radius) < -64 || egg.EggReset == false) egg.Position = new Vector2(-64, 0);
                if (egg.EggReset && egg.Ready)
                {
                    egg.Position = egg.Start;
                    egg.Ready = false;
                }
            }

            foreach (var balloon in balloons)
            {
                if (lose == false) balloon.Draw(gameTime, spriteBatch);
                if ((balloon.Bounds.Center.X - balloon.Bounds.Radius) < -64 || balloon.BalloonReset == false) balloon.Position = new Vector2(-64, 0);
                if (balloon.BalloonReset && balloon.Ready)
                {
                    balloon.Position = balloon.Start;
                    balloon.Ready = false;
                }
            }

            bird.Draw(gameTime, spriteBatch);

            if (lives <= 0)
            {
                lose = true;
                bird.Color = Color.Red;
                spriteBatch.DrawString(_font, $"You Failed!", new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 2) - 64, (ScreenManager.GraphicsDevice.Viewport.Height / 2) - 32), new Color(244, 255, 255));
                MediaPlayer.Stop();
                if (ScreenManager.gameCounter == 0) ScreenManager.bestTime = time;
                ScreenManager.gameCounter++;
                if (time > ScreenManager.bestTime) ScreenManager.bestTime = time;
                end = true;
            }

            for(int i = 0; i <= lives - 1; i++)
            {
                spriteBatch.Draw(heart, new Vector2(5 + (32*i), 5), Color.White);
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
