using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml.Linq;


namespace GameStateManagement
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SpawnManager : Microsoft.Xna.Framework.GameComponent
    {
        private const int MaxAsteroids = 10;

        public static List<Missile> Missiles = new List<Missile>();

        public List<Asteroid> AsteroidsToRemove = new List<Asteroid>();
        public List<Missile> MissilesToRemove = new List<Missile>();

        private List<XElement> levels = new List<XElement>();
        private int currentLevel = 0;
        public int gCurrentLevel
        {
            get
            {
                return currentLevel;
            }
        }
        private int allowedHammers = 0;
        public int gAllowedHammers
        {
            get
            {
                return allowedHammers;
            }
        }
        private int allowedSprings = 0;
        public int gAllowedSprings
        {
            get
            {
                return allowedSprings;
            }
        }
        private int allowedPlatforms = 0;
        public int gAllowedPlatforms
        {
            get
            {
                return allowedPlatforms;
            }
        }

        private List<Hammer> hammers = new List<Hammer>();
        private Door targetDoor;

        //public List<Asteroid> Asteroids = new List<Asteroid>();

        private const float AsteroidFrequency = 3.0f;

        private Asteroid m_TempAsteroid;

        private Game game;

        Random random = new Random();

        Utils.Timer timer = new Utils.Timer();

        class coordinateXYZ
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
            public coordinateXYZ(float x, float y, float z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }

        public SpawnManager(Game game)
            : base(game)
        {
            this.game = game;
            XDocument doc = XDocument.Load("Level1.xml");
            XElement XnaObject = doc.Element("XnaContent");
            IEnumerable<XElement> level = XnaObject.Elements("Level");
            levels = level.ToList<XElement>();
            XElement firstLevel = levels.ElementAt(currentLevel);
            parseLevel(firstLevel);
        }

        public bool nextLevel()
        {
            currentLevel++;
            if (currentLevel >= levels.Count)
            {
                return false;
            }
            game.Components.Remove(targetDoor);
            foreach (Hammer ham in hammers)
            {
                game.Components.Remove(ham);
            }
            hammers = new List<Hammer>();
            XElement level = levels.ElementAt(currentLevel);
            parseLevel(level);
            return true;
        }

        private void parseLevel(XElement level)
        {
            XElement levelObject = level.Element("LevelObject");
            List<coordinateXYZ> levelObjects = new List<coordinateXYZ>();
            foreach (XElement el in levelObject.Elements("Floor"))
            {
                levelObjects.Add(new coordinateXYZ(float.Parse(el.Attribute("X").Value), float.Parse(el.Attribute("Y").Value), float.Parse(el.Attribute("Z").Value)));
            }
            foreach (coordinateXYZ co in levelObjects)
            {
                game.Components.Add(new Hammer(game, co.X, co.Y, co.Z));
                //also make global private list of whatever type of object this ends up being (not hammer), add to it here, remove from it at start of new level
                Console.Write(co.X + " ");
                Console.Write(co.Y + " ");
                Console.WriteLine(co.Z);
            }
            XElement door = levelObject.Element("Door");
            coordinateXYZ doorLocation = new coordinateXYZ(float.Parse(door.Attribute("X").Value), float.Parse(door.Attribute("Y").Value), float.Parse(door.Attribute("Z").Value));
            targetDoor = new Door(game, doorLocation.X, doorLocation.Y, doorLocation.Z);
            game.Components.Add(targetDoor);
            XElement interactiveObject = level.Element("InteractiveObject");
            XElement hammer = interactiveObject.Element("Hammer");
            allowedHammers = int.Parse(hammer.Value);
            Console.WriteLine(allowedHammers);
            XElement spring = interactiveObject.Element("Spring");
            allowedSprings = int.Parse(spring.Value);
            Console.WriteLine(allowedSprings);
            XElement platform = interactiveObject.Element("Platform");
            allowedPlatforms = int.Parse(platform.Value);
            Console.WriteLine(allowedPlatforms);
        }

        public void placeHammer(float xPos, float yPos, float zPos)
        {
            if (allowedHammers > 0)
            {
                allowedHammers--;
                Hammer newHammer = new Hammer(game, xPos, yPos, zPos);
                hammers.Add(newHammer);
                game.Components.Add(newHammer);
            }
        }
        public void placeSpring(float xPos, float yPos, float zPos)
        {
            if (allowedSprings > 0)
            {
                allowedSprings--;

            }
        }
        public void placePlatform(float xPos, float yPos, float zPos)
        {
            if (allowedPlatforms-- > 0)
            {
                allowedPlatforms--;

            }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {

            timer.AddTimer("Asteroid Timer", AsteroidFrequency, new Utils.TimerDelegate(CreateAsteroid),
                true);

            base.Initialize();
        }

        private void CreateAsteroid()
        {
            m_TempAsteroid = new Asteroid(this.Game);
            Matrix m_mTempMatrix = Matrix.Identity;
            Matrix.CreateTranslation((float)random.Next(-512, 512), (float)random.Next(-384, 384),
                (float)random.Next(-1, 1), out m_mTempMatrix);
            Game.Components.Add(m_TempAsteroid);
            m_TempAsteroid.fMass = ((float)random.Next(50, 100)) / 100.0f;
            m_TempAsteroid.name = timer.GetTriggerCount("Asteroid Timer");
            m_TempAsteroid.m_vWorldPosition = m_mTempMatrix.Translation;

        }

        public static void AddMissile(Missile m)
        {
            Missiles.Add(m);
        }

        public void DestroyMissile(Missile m)
        {
            if (MissilesToRemove.Contains(m))
                return;
            MissilesToRemove.Add(m);
        }

        public void DestroyAsteroid(Asteroid a)
        {
            if (AsteroidsToRemove.Contains(a))
                return;
            AsteroidsToRemove.Add(a);
        }

        
        public void RemoveMissiles()
        {
            foreach (Missile m in MissilesToRemove)
            {
                m.Despawn();
            }
            MissilesToRemove.Clear();
        }
        

        public void RemoveAsteroids()
        {
            foreach (Asteroid a in AsteroidsToRemove)
            {
                a.Despawn();
            }
            AsteroidsToRemove.Clear();
        }
        
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            timer.Update(gameTime);
            base.Update(gameTime);
        }
    }
}
