using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        private bool start = false;

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
                new EggSprite(new Vector2((float)rand.NextDouble() * GraphicsDevice.Viewport.Width, 150))
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

            // TODO: use this.Content to load your game content here
            foreach (var egg in eggs) egg.LoadContent(Content);
            bird.LoadContent(Content);
            spriteFont = Content.Load<SpriteFont>("bangers");
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
                    bird.Color = Color.Gold;
                    egg.Collected = true;
                    eggsGathered++;
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
            GraphicsDevice.Clear(Color.DarkGreen);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            foreach (var egg in eggs)
            {
                egg.Draw(gameTime, spriteBatch);
                if (egg.Bounds.Center.Y > GraphicsDevice.Viewport.Height)
                {
                    lose = true;
                }
            }
            bird.Draw(gameTime, spriteBatch);

            if (eggsGathered == 0 && lose == false)
            {
                spriteBatch.DrawString(spriteFont, $"Catch The Egg Before it hits the ground!", new Vector2(1,1), Color.White);
            }
            else if (eggsGathered == 0 && lose == true)
            {
                bird.Color = Color.Red;
                spriteBatch.DrawString(spriteFont, $"You Lose!", new Vector2(1, 1), Color.Red);
            }
            else 
            {
                spriteBatch.DrawString(spriteFont, $"You Win!", new Vector2(1,1), Color.Gold);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
