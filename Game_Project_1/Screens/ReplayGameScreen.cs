using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Game_Project_1.StateManagement;

namespace Game_Project_1.Screens
{
    class ReplayGameScreen : MenuScreen
    {
        public ReplayGameScreen() : base("Would you like to Play Again?")
        {
            var playGameMenuEntry = new MenuEntry("Play Again");
            var exitMenuEntry = new MenuEntry("Exit");

            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Exit?";
            var confirmExitMessageBox = new ExitMessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
    }
}
