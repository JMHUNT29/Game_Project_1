using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Game_Project_1
{
    public class CatchTheEgg : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private EggSprite[] eggs;
        private BirdSprite bird;
        private SpriteFont spriteFont;
        private int eggsGathered;
        private bool lose = false;
        private Texture2D background;

        private SoundEffect eggPickup;
        private Song backgroundMusic;

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
            System.Random rand = new System.Random();
            eggs = new EggSprite[]
            {
                new EggSprite(new Vector2(GraphicsDevice.Viewport.Width, (float)(rand.NextDouble() * GraphicsDevice.Viewport.Height * 0.5)), EggSprite.Side.Right),
                new EggSprite(new Vector2(0, (float)(rand.NextDouble() * GraphicsDevice.Viewport.Height * 0.5)), EggSprite.Side.Left),
                new EggSprite(new Vector2(GraphicsDevice.Viewport.Width, (float)(rand.NextDouble() * GraphicsDevice.Viewport.Height * 0.5)), EggSprite.Side.Right),
                new EggSprite(new Vector2(0, (float)(rand.NextDouble() * GraphicsDevice.Viewport.Height * 0.5)), EggSprite.Side.Left),
                new EggSprite(new Vector2(GraphicsDevice.Viewport.Width, (float)(rand.NextDouble() * GraphicsDevice.Viewport.Height * 0.5)), EggSprite.Side.Right)
            };
            eggsGathered = 0;
            bird = new BirdSprite();

            base.Initialize();
        }

        /// <summary>
        /// Loads content for the game
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("sunny-background-large");
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
                    eggPickup.Play();
                }
            }

            bird.Update(gameTime);

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
            spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            foreach (var egg in eggs)
            {
                egg.Draw(gameTime, spriteBatch);
                if (egg.Bounds.Center.Y > GraphicsDevice.Viewport.Height)
                {
                    lose = true;
                }
            }
            bird.Draw(gameTime, spriteBatch);

            if (eggsGathered == 0)
            {
                spriteBatch.DrawString(spriteFont, $"Catch The Eggs Before they reach the bottom!", new Vector2(1, GraphicsDevice.Viewport.Height - 100), Color.Black);
            }
            else if (eggsGathered < 5 && lose == true)
            {
                bird.Color = Color.Red;
                spriteBatch.DrawString(spriteFont, $"You Lose!", new Vector2(1, 1), Color.Red);
                MediaPlayer.Stop();
            }
            else if (eggsGathered == 5 && lose == false)
            {
                spriteBatch.DrawString(spriteFont, $"You Win!", new Vector2(1,1), Color.Gold);
                bird.Color = Color.Gold;
                MediaPlayer.Stop();
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
