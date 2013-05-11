#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>

    class Collision
    {
        public Asteroid a, a1;
        public float time;
        public Collision(Asteroid a, Asteroid a1)
        {
            this.a = a;
            this.a1 = a1;
            time = 0.0f;
        }
    }

    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();

		Utils.Timer m_kTimer = new Utils.Timer();

        public static Matrix CameraMatrix;
        public static Matrix ProjectionMatrix;
        public static Vector3 SpecularColor;
        public static Vector3 AmbientColor;
        public static Vector3 DirectionalColor;
        public static Vector3 DirectionalDirection;
        public static float SpecularIntensity;

        public static AudioEngine audio;
        public static WaveBank wavebank;
        public static SoundBank soundbank;

        public int current_color = 0;
        public int current_direction = 0;
        public Vector3[] colors;
        public Vector3[] directions;

        public Vector3 ShiftedDirectionalDirection = Vector3.Normalize(new Vector3(0.0f, 1.0f, 0.0f));
        public Vector3 OriginalDirectionalDirection = Vector3.Normalize(new Vector3(1.0f, 0.0f, 0.0f));

        private bool OriginalDirection = true;
        private bool DespawnShip = false;

        private float LerpElapsedTime = 0.0f;

        //public List<Collision> collisions = new List<Collision>();
        public Collision[] collisions;
        public int num_collisions = 0;
        SpawnManager spawnManager;
        Ship m_kShip;
		Star m_Star;
        Bloom bloom_screen;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            CameraMatrix = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 2000.0f),
                Vector3.Zero, Vector3.UnitY);
            ProjectionMatrix = Matrix.CreateOrthographic(1024.0f, 768.0f, 0.1f, 10000.0f);
            AmbientColor = Color.DarkBlue.ToVector3() * 0.4f;
            SpecularColor = Color.DarkRed.ToVector3() * 2.0f;
            SpecularIntensity = 1.0f;
            DirectionalColor = Color.DarkRed.ToVector3() * 1.0f;
            
            colors = new Vector3[6];
            colors[0] = Color.Red.ToVector3();
            colors[1] = Color.Orange.ToVector3();
            colors[2] = Color.Yellow.ToVector3();
            colors[3] = Color.Green.ToVector3();
            colors[4] = Color.Blue.ToVector3();
            colors[5] = Color.Purple.ToVector3();
            directions = new Vector3[4];
            directions[0] = Vector3.UnitX;
            directions[1] = Vector3.UnitY;
            directions[2] = -1*Vector3.UnitX;
            directions[3] = -1 * Vector3.UnitY;
            DirectionalDirection = OriginalDirectionalDirection;
            //m_kTimer.AddTimer("Diffuse_Light_Switch", 2.0f, MoveDirectionalLight, true);
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        /// 

        private void MoveDirectionalLight()
        {
            current_color = ++current_color % 6;
            current_direction = ++current_direction % 4;
            OriginalDirection = !OriginalDirection;
            LerpElapsedTime = 0.0f;
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("gamefont");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            // Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.

            audio = new AudioEngine("Content/Sounds.xgs");
            wavebank = new WaveBank(audio, "Content/XNAsteroids Waves.xwb");
            soundbank = new SoundBank(audio, "Content/XNAsteroids Cues.xsb");

            ScreenManager.Game.ResetElapsedTime();

			//m_kTimer.AddTimer("Timer 1", 10.0f, new Utils.TimerDelegate(TimerOneShot), false);
			//m_kTimer.AddTimer("Timer 2", 1.0f, new Utils.TimerDelegate(TimerLoop), true);
			//m_kTimer.AddTimer("Timer 3", 10.0f, new Utils.TimerDelegate(TimerLoop), true);
			//m_kTimer.AddTimer("Timer 4", 5.0f, new Utils.TimerDelegate(TimerOneShot), false);
			//m_kTimer.AddTimer("Timer 5", 22.0f, new Utils.TimerDelegate(TimerLoopRemove), true);
            spawnManager = new SpawnManager(ScreenManager.Game);
			m_Star = new Star(ScreenManager.Game);
            SpawnShip();
            bloom_screen = new Bloom(ScreenManager.Game);
            ScreenManager.Game.Components.Add(spawnManager);
			ScreenManager.Game.Components.Add(m_Star);
            ScreenManager.Game.Components.Add(bloom_screen);
            collisions = new Collision[10000];
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                // Apply some random jitter to make the enemy move around.
                const float randomization = 10;

                enemyPosition.X += (float)(random.NextDouble() - 0.5) * randomization;
                enemyPosition.Y += (float)(random.NextDouble() - 0.5) * randomization;

                // Apply a stabilizing force to stop the enemy moving off the screen.
                Vector2 targetPosition = new Vector2(200, 200);

                enemyPosition = Vector2.Lerp(enemyPosition, targetPosition, 0.05f);



                // TODO: this game isn't very fun! You could probably improve
                // it by inserting something more interesting in this space :-)
                if (LerpElapsedTime <= 2.0f)
                    LerpElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                else
                    MoveDirectionalLight();
                DirectionalDirection = Vector3.Lerp(DirectionalDirection, directions[current_direction], LerpElapsedTime / 12.0f);
                DirectionalColor = Vector3.Lerp(DirectionalDirection, colors[current_color], LerpElapsedTime / 2.0f);
                SpecularColor = Vector3.Lerp(DirectionalDirection, colors[((current_color + 1) % 6)], LerpElapsedTime / 2.0f);

                foreach (GameComponent gc in ScreenManager.Game.Components)
                {
                    Asteroid a;
                    if (gc is Asteroid)
                    {
                        a = (Asteroid)gc;
                        if (m_kShip.WorldBounds.Intersects(a.WorldBounds))
                        {
                            DespawnShip = true;
                            m_kTimer.AddTimer("Ship_Respawn", 5.0f, SpawnShip, false);
                        }

                        foreach (Missile m in SpawnManager.Missiles)
                        {
                            if (m.WorldBounds.Intersects(a.WorldBounds))
                            {
                                spawnManager.DestroyMissile(m);
                                spawnManager.DestroyAsteroid(a);
                                break;
                            }
                        }
                        foreach (GameComponent gc1 in ScreenManager.Game.Components)
                        {
                            Asteroid a1;
                            Vector3 new_velocity;
                            if (gc1 is Asteroid && gc1 != gc)
                            {
                                a1 = (Asteroid)gc1;
                                if (a.WorldBounds.Intersects(a1.WorldBounds))
                                {
                                    if (num_collisions == 0)
                                    {
                                        new_velocity = ((a1.fMass - a.fMass) / (a1.fMass + a.fMass)) * a1.m_vVelocity +
                                            ((2 * a.fMass) / (a1.fMass + a.fMass)) * a.m_vVelocity;
                                        a.m_vVelocity = ((a.fMass - a1.fMass) / (a1.fMass + a.fMass)) * a.m_vVelocity +
                                            ((2 * a1.fMass) / (a1.fMass + a.fMass)) * a1.m_vVelocity;
                                        a1.m_vVelocity = new_velocity;


                                        collisions[0] = new Collision(a, a1);
                                        num_collisions = 1;
                                    }

                                    else
                                    {
                                        bool new_collisions = true;
                                        for (int i = 0; i < num_collisions; i++)
                                        {
                                            Collision c = collisions[i];
                                            if ((c.a.name == a1.name && c.a1.name == a.name)
                                                || (c.a.name == a.name && c.a1.name == a1.name))
                                            {
                                                new_collisions = false;
                                                if(c.time > 1.0f)
                                                {
                                                    c.time = 0.0f;
                                        new_velocity = ((c.a1.fMass - c.a.fMass) / (c.a1.fMass + c.a.fMass)) * c.a1.m_vVelocity +
                                            ((2 * c.a.fMass) / (c.a1.fMass + c.a.fMass)) * c.a.m_vVelocity;
                                        c.a.m_vVelocity = ((c.a.fMass - c.a1.fMass) / (c.a1.fMass + c.a.fMass)) * c.a.m_vVelocity +
                                            ((2 * c.a1.fMass) / (c.a1.fMass + c.a.fMass)) * c.a1.m_vVelocity;
                                        c.a1.m_vVelocity = new_velocity;


                                                }
                                            }
                                        }

                                        if (new_collisions)
                                        {
                                        new_velocity = ((a1.fMass - a.fMass) / (a1.fMass + a.fMass)) * a1.m_vVelocity +
                                            ((2 * a.fMass) / (a1.fMass + a.fMass)) * a.m_vVelocity;
                                        a.m_vVelocity = ((a.fMass - a1.fMass) / (a1.fMass + a.fMass)) * a.m_vVelocity +
                                            ((2 * a1.fMass) / (a1.fMass + a.fMass)) * a1.m_vVelocity;
                                        a1.m_vVelocity = new_velocity;


                                            collisions[num_collisions] = new Collision(a, a1);
                                            num_collisions++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (DespawnShip)
                {
                    DespawnShip = false;
                    m_kShip.Despawn();
                }
                spawnManager.RemoveMissiles();
                spawnManager.RemoveAsteroids();



                for (int i = 0; i < num_collisions; i++)
                {
                    Collision c = collisions[i];
                    c.time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

				m_kTimer.Update(gameTime);
            }
        }

            public void SpawnShip(){
                m_kShip = new Ship(ScreenManager.Game);
                ScreenManager.Game.Components.Add(m_kShip);
            }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
		public override void HandleInput(InputState input, GameTime gameTime)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.PauseGame)
            {
                // If they pressed pause, bring up the pause menu screen.
                ScreenManager.AddScreen(new PauseMenuScreen());
            }
            else
            {
                if (input.ShipMoveForward)
                {
                    m_kShip.MoveForward();
                }
                if (input.ShipMoveBackward) 
                {
                    m_kShip.MoveBackward();
                }
                if (input.ShipTurnLeft)
                {
                    m_kShip.TurnLeft((float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                if (input.ShipTurnRight)
                {
                    m_kShip.TurnRight((float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                if (input.ShipFire)
                {
                    m_kShip.FireMissile();
                }
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // spriteBatch.DrawString(gameFont, "// TODO", playerPosition, Color.Green);

            //spriteBatch.DrawString(gameFont, "Insert Gameplay Here",
            //                       enemyPosition, Color.DarkRed);

			//spriteBatch.DrawString(gameFont, MakeTimerDebugString("Timer 1"), new Vector2(20.0f, 500.0f), Color.Blue);
			//spriteBatch.DrawString(gameFont, MakeTimerDebugString("Timer 2"), new Vector2(20.0f, 550.0f), Color.White);
			//spriteBatch.DrawString(gameFont, MakeTimerDebugString("Timer 3"), new Vector2(20.0f, 600.0f), Color.White);
			//spriteBatch.DrawString(gameFont, MakeTimerDebugString("Timer 4"), new Vector2(20.0f, 650.0f), Color.Blue);
			//spriteBatch.DrawString(gameFont, MakeTimerDebugString("Timer 5"), new Vector2(20.0f, 700.0f), Color.White);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }

        #endregion

		#region Timer Test Functions
		void TimerOneShot()
		{
			Console.WriteLine("TimerOneShot fired!");
		}

		void TimerLoop()
		{
			Console.WriteLine("TimerLoop fired!");
		}

		void TimerLoopRemove()
		{
			Console.WriteLine("TimerLoopRemove fired!");
			m_kTimer.RemoveTimer("Timer 3");
		}

		string MakeTimerDebugString(string sTimerName)
		{
			if (m_kTimer.GetTriggerCount(sTimerName) != -1)
				return sTimerName + " - Time: " + m_kTimer.GetRemainingTime(sTimerName).ToString("f3")
					+ " Count: " + m_kTimer.GetTriggerCount(sTimerName);
			else
				return sTimerName + " not found! ";
		}
		#endregion
        public void AddToGameplayScreen(IGameComponent item)
        {
            ScreenManager.Game.Components.Add(item);
        }
	}
}
