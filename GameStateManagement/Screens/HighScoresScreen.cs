using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace GameStateManagement
{
    class HighScoresScreen : MenuScreen
    {
        public HighScoresScreen() : base("High Scores"){
            MenuEntry backMenuEntry = new MenuEntry("Back");
            backMenuEntry.Selected += OnCancel;
            MenuEntries.Add(backMenuEntry);

            DirectoryInfo dir = new DirectoryInfo(".");
            bool present = false;
            foreach (FileInfo file in dir.GetFiles("HighScores.xml"))
            {
                present = true;
                XDocument doc = XDocument.Load(file.Name);
                XElement XnaObject = doc.Element("XnaContent");
                IEnumerable<XElement> level = XnaObject.Elements("Level");
                List<XElement> levels = level.ToList<XElement>();
                foreach (XElement lvl in levels)
                {
                    MenuEntry levelName = new MenuEntry(lvl.Attribute("name").Value.ToString());
                    //newMenuEntry.Selected += LevelSelected;
                    MenuEntries.Add(levelName);
                    IEnumerable<XElement> score = lvl.Elements("Score");
                    List<XElement> scores = score.ToList<XElement>();
                    foreach (XElement scr in scores)
                    {
                        MenuEntry scoreValue = new MenuEntry(scr.Value.ToString());
                        MenuEntries.Add(scoreValue);
                    }
                }
            }
            if (!present)
            {
                MenuEntry none = new MenuEntry("No High Scores Found.");
                MenuEntries.Add(none);
            }
        }
    }
}
