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
    public class EggSprite
    {
        public enum Side
        {
            Right = 0,
            Left = 1,
        }

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
        public bool EggReset;

        /// <summary>
        /// Whether or not egg will give player extra lives
        /// </summary>
        public bool Lives { get; set; } = false;

        /// <summary>
        /// Whether or not egg was collected
        /// </summary>
        public bool Collected { get; set; } = false;

        /// <summary>
        /// The bounding volume of the sprite
        /// </summary>
        public BoundingCircle Bounds => bounds;

        public bool Ready = false;

        private double waitTime = 0;

        private float accelerationChange = (float)0.006;



        /// <summary>
        /// Creates a new egg sprite
        /// </summary>
        /// <param name="position">The position of the sprite in the game</param>
        public EggSprite(Vector2 position, bool reset)
        {

            this.EggReset = reset;

            //Sets position and bounds for egg
            this.Position = position;
            //Sets start position for egg.
            this.Start = position;
            this.bounds = new BoundingCircle(position + new Vector2(16, 16), 16);
            velocity = new Vector2(290, this.Position.Y);
        }

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Egg");
        }

        /// <summary>
        /// Updates the egg sprite to continue to fall
        /// </summary>
        /// <param name="gameTime">The game time</param>
        public void Update(GameTime gameTime)
        {

            if (this.Position.X <= -64 && EggReset) this.Position = Start;
            else if (this.Position.X > -64 && EggReset)
            {

                float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

                Position.X -= velocity.X * t;

            }

            if (Collected) this.bounds = new BoundingCircle(new Vector2(16, 16), 16);
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
            if (Collected) return;

            var source = new Rectangle(0, 0, 32, 32);
            spriteBatch.Draw(texture, Position, source, Color.White, 0, new Vector2(32, 32), 1.5f, SpriteEffects.None, 0);

        }
    }
}
