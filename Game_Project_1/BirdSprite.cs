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

        private Vector2 position = new Vector2(400, 400);

        private bool flipped;

        private BoundingRectangle bounds = new BoundingRectangle(new Vector2(200 - 32, 200 - 32), 32, 32);

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

            // Apply keyboard movement
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W)) position += new Vector2(0, -1);
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S)) position += new Vector2(0, 1);
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                position += new Vector2(-3, 0);
                flipped = true;
            }
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                position += new Vector2(3, 0);
                flipped = false;
            }

            /// Update the bounds
            bounds.X = position.X - 16;
            bounds.Y = position.Y - 16;
        }

        /// <summary>
        /// Draws the sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffects = (flipped) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(atlas, position, new Rectangle(0, 0, 32, 32), Color, 0, new Vector2(32, 32), 2f, spriteEffects, 0);
        }
    }
}
