using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Game_Project_1.Collisions;

namespace Game_Project_1
{
    /// <summary>
    /// A class representing a bird
    /// </summary>
    public class BirdSprite
    {
        private KeyboardState keyboardState;

        private Texture2D atlas;

        public Vector2 position = new Vector2(150, 500);

        private BoundingRectangle bounds = new BoundingRectangle(new Vector2(150 - 16, 500 - 16), 32, 32);

        private double animationTimer;

        private short animationFrame = 1;

        /// <summary>
        /// The bounding volume of the sprite
        /// </summary>
        public BoundingRectangle Bounds => bounds;

        /// <summary>
        /// The color to blend with the bird
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            atlas = content.Load<Texture2D>("birds.32x32");
        }

        /// <summary>
        /// Updates the sprite's position based on user input
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        public void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            if (position.Y > 750) position = new Vector2(150, 750);
            else if (position.Y < 250) position = new Vector2(150, 250);

            // Apply keyboard movement
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                position += new Vector2(0, -2)  * 125 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                position += new Vector2(0, 2) * 125 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            /// Update the bounds
            bounds.Y = position.Y - 16;
        }

        /// <summary>
        /// Draws the sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

            //Update animation frame
            if (animationTimer > 0.3)
            {
                animationFrame++;
                if (animationFrame > 3) animationFrame = 0;
                animationTimer -= 0.3;
            }

            //Draws the sprite
            var source = new Rectangle(animationFrame * 32, 0, 32, 32);
            spriteBatch.Draw(atlas, position, source, Color, 0, new Vector2(32, 32), 2.5f, SpriteEffects.FlipHorizontally, 0);
        }
    }
}
