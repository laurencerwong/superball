#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Xml;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class FinishedLevelScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public FinishedLevelScreen(SpawnManager mgr)
            : base("Level Complete")
        {
            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;

            // Create our menu entries.
            MenuEntry nextLevelGameMenuEntry = new MenuEntry("Next Level");
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");

            // Hook up menu event handlers.
            nextLevelGameMenuEntry.Selected += nextLevelGameMenuEntrySelected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(nextLevelGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);

            DirectoryInfo dir = new DirectoryInfo(".");
            bool present = false;
            int currlev = mgr.gCurrentLevel;
            currlev++;
            String currentLevel = "Level" + currlev.ToString();
            foreach (FileInfo file in dir.GetFiles("HighScores.xml"))
            {
                present = true;
                XDocument doc = XDocument.Load(file.Name);
                XElement XnaObject = doc.Element("XnaContent");
                IEnumerable<XElement> level = XnaObject.Elements("Level");
                /* List<XElement> levels = level.ToList<XElement>(); */
                bool found = false;
                foreach (XElement lvl in level)
                {
                    if(lvl.Attribute("name").Value.ToString().Equals(currentLevel)){
                        IEnumerable<XElement> allScores = lvl.Elements("Score");
                        XElement newScore = new XElement("Score");
                        newScore.SetValue(GameplayScreen.time);
                        bool last = true;
                        int place = 0;
                        foreach (XElement scr in allScores)
                        {
                            if (strToSec(newScore.Value.ToString()) < strToSec(scr.Value.ToString()))
                            {
                                scr.AddBeforeSelf(newScore);
                                if (place == 0)
                                {
                                    MenuEntry newHighScore = new MenuEntry("NEW HIGH SCORE!!");
                                    MenuEntries.Add(newHighScore);
                                }
                                last = false;
                                break;
                            }
                            place++;
                        }
                        if (last)
                        {
                            lvl.Add(newScore);
                        }
                        //lvl.Add(newScore);
                        found = true;
                    }
                }
                if (!found)
                {
                    XElement newLevel = new XElement("Level");
                    XAttribute newLevelName = new XAttribute("name", currentLevel);
                    newLevel.Add(newLevelName);
                    XElement newScore = new XElement("Score");
                    newScore.SetValue(GameplayScreen.time);
                    newLevel.Add(newScore);
                    XnaObject.Add(newLevel);
                    MenuEntry newHighScore = new MenuEntry("NEW HIGH SCORE!!");
                    MenuEntries.Add(newHighScore);
                }
                XmlWriter writer = XmlWriter.Create("HighScores.xml");
                doc.WriteTo(writer);
                writer.Close();
            }
            if (!present)
            {
                XDocument newDoc = new XDocument();
                XElement XNAContent = new XElement("XnaContent");
                XElement Level = new XElement("Level");
                XAttribute newLevelName = new XAttribute("name", currentLevel);
                Level.Add(newLevelName);
                XElement newScore = new XElement("Score");
                newScore.SetValue(GameplayScreen.time);
                Level.Add(newScore);
                XNAContent.Add(Level);
                newDoc.Add(XNAContent);
                XmlWriter writer = XmlWriter.Create("HighScores.xml");
                newDoc.WriteTo(writer);
                writer.Close();
                MenuEntry newHighScore = new MenuEntry("NEW HIGH SCORE!!");
                MenuEntries.Add(newHighScore);
            }
        }

        double strToSec(String time)
        {
            String[] minSecMilli = new String[3];
            String[] delimiters = new String[2];
            delimiters[0] = ":"; delimiters[1] = ".";
            minSecMilli = time.Split(delimiters, 3, new StringSplitOptions());
            int[] nums = new int[3];
            nums[0] = int.Parse(minSecMilli[0]);
            nums[1] = int.Parse(minSecMilli[1]);
            nums[2] = int.Parse(minSecMilli[2]);
            double total = nums[0] * 60 + nums[1] + (double)nums[2] / 100.0f;
            return total;
        }

        #endregion

        #region Handle Input


        void nextLevelGameMenuEntrySelected(object sender, EventArgs e)
        {
            // right now just loads GameplayScreen again
            GameplayScreen.spawnManagerStartingLevelNumber++;
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
            
        }
        
        
        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, EventArgs e)
        {
            const string message = "Are you sure you want to leave? :(";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, new BackgroundScreen(),
                                                     new MainMenuScreen());
        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            base.Draw(gameTime);
        }


        #endregion
    }
}
