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

        private Side side;

        private double directionTimer;

        private double waitTime;

        private Vector2 position;

        private Vector2 velocity;

        private Texture2D texture;

        private BoundingCircle bounds;

        public bool Collected { get; set; } = false;

        /// <summary>
        /// The bounding volume of the sprite
        /// </summary>
        public BoundingCircle Bounds => bounds;



        /// <summary>
        /// Creates a new egg sprite
        /// </summary>
        /// <param name="position">The position of the sprite in the game</param>
        public EggSprite(Vector2 position, Side side)
        {
            //Update waitTime which is how long egg goes up
            System.Random rand = new System.Random();
            this.waitTime = rand.NextDouble() + 1;

            //Updates which side egg will come out on
            this.side = side;

            //Sets position and bounds for egg
            this.position = position;
            this.bounds = new BoundingCircle(position + new Vector2(16, 16), 16);
        }

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("egg");
        }

        /// <summary>
        /// Updates the egg sprite to continue to fall
        /// </summary>
        /// <param name="gameTime">The game time</param>
        public void Update(GameTime gameTime)
        {
            System.Random rand = new System.Random();

            directionTimer += gameTime.ElapsedGameTime.TotalSeconds;

            // Creates different velocities based on side and waitTime
            if (directionTimer < waitTime && side == Side.Right) velocity = new Vector2((float)(rand.NextDouble() * -100), (float)(rand.NextDouble() * -100));
            else if (directionTimer < waitTime && side == Side.Left) velocity = new Vector2((float)(rand.NextDouble() * 100), (float)(rand.NextDouble() * -100));
            else if (directionTimer > waitTime && side == Side.Right) velocity = new Vector2((float)(rand.NextDouble() * -100), (float)(rand.NextDouble() * 100));
            else velocity = new Vector2((float)(rand.NextDouble() * 100), (float)(rand.NextDouble() * 100));

            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 acceleration = new Vector2(1100, 1100);

            velocity += acceleration * t;
            position += velocity * t;

            if (Collected) this.bounds = new BoundingCircle(new Vector2(16, 16), 16);
            /// Update the bounds
            else this.bounds = new BoundingCircle(position + new Vector2(16, 16), 16);
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
            spriteBatch.Draw(texture, position, source, Color.ForestGreen, 0, new Vector2(32, 32), 1, SpriteEffects.None, 0);
        }
    }
}
