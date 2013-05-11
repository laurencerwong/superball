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
        //public List<Asteroid> Asteroids = new List<Asteroid>();

        private const float AsteroidFrequency = 3.0f;

        private Asteroid m_TempAsteroid;

        Random random = new Random();

        Utils.Timer timer = new Utils.Timer();

        public SpawnManager(Game game)
            : base(game)
        {

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
