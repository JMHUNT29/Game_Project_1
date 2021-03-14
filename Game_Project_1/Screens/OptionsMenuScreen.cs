using Game_Project_1.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Game_Project_1.Screens
{
    // The options screen is brought up over the top of the main menu
    // screen, and gives the user a chance to configure the game
    // in various hopefully useful ways.
    public class OptionsMenuScreen : MenuScreen
    {

        private readonly MenuEntry _backgroundVolumeMenuEntry;

        private static int _currentBackgroundVolume = 3;

        public OptionsMenuScreen() : base("Options")
        {
            _backgroundVolumeMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            var back = new MenuEntry("Back");

            _backgroundVolumeMenuEntry.Selected += BackgroundVolumeMenuEntrySelected;
            back.Selected += OnCancel;

            MenuEntries.Add(_backgroundVolumeMenuEntry);
            MenuEntries.Add(back);
        }

        // Fills in the latest values for the options screen menu text.
        private void SetMenuEntryText()
        {
            _backgroundVolumeMenuEntry.Text = $"Volume: {_currentBackgroundVolume}";
        }

        private void BackgroundVolumeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _currentBackgroundVolume++;
            ScreenManager.Volume += (float)0.2;


            if (_currentBackgroundVolume > 5)
            {
                ScreenManager.Volume = 0;
                _currentBackgroundVolume = 0;
            }

            SetMenuEntryText();
        }

    }
}
