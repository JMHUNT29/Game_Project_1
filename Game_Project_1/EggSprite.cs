using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Game_Project_1.Collisions;

namespace Game_Project_1
{
    public class EggSprite
    {
        private double directionTimer;

        private Vector2 position;

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
        public EggSprite(Vector2 position)
        {
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
            // Update the direction timer
            directionTimer += gameTime.ElapsedGameTime.TotalSeconds;

            //Switch directions every 2 seconds
            if (directionTimer > 2.0)
            {
                directionTimer -= 2.0;
            }

            //Update position of the egg
            position += new Vector2(0, 1) * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            /// Update the bounds
            this.bounds = new BoundingCircle(position + new Vector2(16, 16), 16);
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
            spriteBatch.Draw(texture, position, source, Color.White, 0, new Vector2(32, 32), 1, SpriteEffects.None, 0);
        }
    }
}
