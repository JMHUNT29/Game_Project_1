﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game_Project_1.StateManagement;

namespace Game_Project_1.Screens
{
    // A popup message box screen, used to display "are you sure?" confirmation messages.
    public class MessageBoxScreen : GameScreen
    {
        private readonly string _message;
        private Texture2D _gradientTexture;
        private readonly InputAction _menuSelect;
        private readonly InputAction _menuCancel;

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        // Constructor lets the caller specify whether to include the standard
        // "A=ok, B=cancel" usage text prompt.
        public MessageBoxScreen(string message, bool includeUsageText = true)
        {
            const string usageText = "\n\nPress Space or Enter to Quit" +
                                     "\n\nPress Backspace to Return to Pause Menu";

            if (includeUsageText)
                _message = message + usageText;
            else
                _message = message;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);

            _menuSelect = new InputAction(
                new[] { Keys.Enter, Keys.Space }, true);
            _menuCancel = new InputAction(
                new[] { Keys.Back }, true);
        }

        // Loads graphics content for this screen. This uses the shared ContentManager
        // provided by the Game class, so the content will remain loaded forever.
        // Whenever a subsequent MessageBoxScreen tries to load this same content,
        // it will just get back another reference to the already loaded data.
        public override void Activate()
        {
            var content = ScreenManager.Game.Content;
                _gradientTexture = content.Load<Texture2D>("blank");
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.
            if (_menuSelect.Occurred(input, ControllingPlayer, out playerIndex))
            {
                Accepted?.Invoke(this, new PlayerIndexEventArgs(playerIndex));
                ExitScreen();
            }
            else if (_menuCancel.Occurred(input, ControllingPlayer, out playerIndex))
            {
                Cancelled?.Invoke(this, new PlayerIndexEventArgs(playerIndex));
                ExitScreen();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;
            var font = ScreenManager.Font;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.
            var viewport = ScreenManager.GraphicsDevice.Viewport;
            var viewportSize = new Vector2(viewport.Width, viewport.Height);
            var textSize = font.MeasureString(_message);
            var textPosition = (viewportSize - textSize) / 2;

            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 16;

            var backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                (int)textPosition.Y - vPad, (int)textSize.X + hPad * 2, (int)textSize.Y + vPad * 2);

            var color = Color.White * TransitionAlpha;    // Fade the popup alpha during transitions

            spriteBatch.Begin();

            spriteBatch.Draw(_gradientTexture, backgroundRectangle, new Color(85, 149, 111) * TransitionAlpha);
            spriteBatch.DrawString(font, _message, textPosition, new Color(244, 255, 255) * TransitionAlpha);

            spriteBatch.End();
        }
    }
}
