using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Game_Project_1.Collisions;

namespace Game_Project_1
{
    class BalloonSprite
    {

        private double waitTimer;

        /// <summary>
        /// Position of egg.
        /// </summary>
        public Vector2 Position;

        public Vector2 velocity;

        private Texture2D texture;

        private BoundingCircle bounds;

        public Vector2 Start;

        /// <summary>
        /// Tells the egg to go back to starting position.
        /// </summary>
        public bool BalloonReset;

        /// <summary>
        /// Whether or not egg was collected
        /// </summary>
        public bool Hit { get; set; } = false;

        /// <summary>
        /// The bounding volume of the sprite
        /// </summary>
        public BoundingCircle Bounds => bounds;

        private double waitTime = 0;

        public bool Ready = true;

        public float accelerationChange = (float)0.005;



        /// <summary>
        /// Creates a new egg sprite
        /// </summary>
        /// <param name="position">The position of the sprite in the game</param>
        public BalloonSprite(Vector2 position, bool reset)
        {

            this.BalloonReset = reset;

            //Sets position and bounds for egg
            this.Position = position;
            //Sets start position for egg.
            this.Start = position;
            this.bounds = new BoundingCircle(position + new Vector2(32, 32), 32);
            velocity = new Vector2(290, this.Position.Y);
        }

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Rough_Game_Balloon");
        }

        /// <summary>
        /// Updates the egg sprite to continue to fall
        /// </summary>
        /// <param name="gameTime">The game time</param>
        public void Update(GameTime gameTime)
        {
            waitTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if (this.Position.X <= -64 && BalloonReset) this.Position = Start;
            else if (this.Position.X > -64 && BalloonReset)
            {

                float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

                Position.X -= velocity.X * t;

            }

            if (Hit) this.bounds = new BoundingCircle(new Vector2(32, 32), 32);
            /// Update the bounds
            else bounds.Center = new Vector2(Position.X, Position.Y);

        }

        /// <summary>
        /// Draws the animated sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Hit) return;

            var source = new Rectangle(0, 0, 64, 73);
            spriteBatch.Draw(texture, Position, source, Color.White, 0, new Vector2(64, 73), 0.85f, SpriteEffects.None, 0);

        }
    }
}
