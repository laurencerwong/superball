using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GameStateManagement
{
    class LoadLevelScreen : MenuScreen
    {
        public LoadLevelScreen()
            : base("Available Levels")
        {
            MenuEntry backMenuEntry = new MenuEntry("Back");
            backMenuEntry.Selected += OnCancel;
            MenuEntries.Add(backMenuEntry);

            //List<MenuEntry> availableFiles = new List<MenuEntry>();

            DirectoryInfo dir = new DirectoryInfo(".");
            foreach (FileInfo file in dir.GetFiles("*.xml"))
            {
                if (file.FullName.Contains("HighScores"))
                    continue;
                MenuEntry newMenuEntry = new MenuEntry(file.Name.Remove(file.Name.Length - 4));
                newMenuEntry.Selected += LevelSelected;
                MenuEntries.Add(newMenuEntry);
            }

        }

        void LevelSelected(object sender, EventArgs e)
        {
            if (sender is MenuEntry)
            {
                MenuEntry selectedEntry = (MenuEntry)sender;
                GameplayScreen.spawnManagerStartingLevelName = selectedEntry.Text + ".xml";
                LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
            }
        }
    }
}
