#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        public  AudioEngine audio;
        public  WaveBank wavebank;
        public  SoundBank soundbank;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Super Ball")
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
            MenuEntry levelSelectMenuEntry = new MenuEntry("Load Saved Level");
            MenuEntry levelEditorMenuEntry = new MenuEntry("Level Editor");
            MenuEntry highScores = new MenuEntry("High Scores");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            audio = new AudioEngine("Content/Sounds.xgs"); 
            wavebank = new WaveBank(audio, "Content/XNAsteroids Waves.xwb");
            soundbank = new SoundBank(audio, "Content/XNAsteroids Cues.xsb");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            levelSelectMenuEntry.Selected += LevelSelectMenuEntrySelected;
            levelEditorMenuEntry.Selected += LevelEditorMenuEntrySelected;
            highScores.Selected += HighScoresMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(levelSelectMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(levelEditorMenuEntry);
            MenuEntries.Add(highScores);
            MenuEntries.Add(exitMenuEntry);

            soundbank.PlayCue("Ambient");
        }


        #endregion

        #region Handle Input

        void LevelSelectMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new LoadLevelScreen());
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
        }

        void LevelEditorMenuEntrySelected(object sender, EventArgs e)
        {
            GameplayScreen.spawnManagerStartingLevelName = "LEVEL_EDITOR";
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen());
        }

        void HighScoresMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new HighScoresScreen());
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel()
        {
            const string message = "Are you sure you want to leave? :(";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
