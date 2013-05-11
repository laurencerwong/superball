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
using BulletSharp;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>



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

        public DynamicWorld dynamicsWorld;
        public Ball player;
        public Spring s;
        public Platform p1;

        public static AudioEngine audio;
        public static WaveBank wavebank;
        public static SoundBank soundbank;

        Quaternion CameraRotation = Quaternion.Identity;

        public int current_color = 0;
        public int current_direction = 0;
        public Vector3[] colors;
        public Vector3[] directions;

        public Vector3 ShiftedDirectionalDirection = Vector3.Normalize(new Vector3(0.0f, 1.0f, 0.0f));
        public Vector3 OriginalDirectionalDirection = Vector3.Normalize(new Vector3(1.0f, 0.0f, 0.0f));

        private bool OriginalDirection = true;
        private bool DespawnShip = false;

        private float LerpElapsedTime = 0.0f;

        SpawnManager spawnManager;
        Bloom bloom_screen;
        Door d;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            
            CameraMatrix = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 2000.0f),
                Vector3.Zero, Vector3.UnitY);
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70.0f), 1.33f, 0.1f, 2000.0f);
            AmbientColor = Color.DarkGray.ToVector3();// *0.4f;
            SpecularColor = Color.LightGray.ToVector3();// *2.0f;
            SpecularIntensity = 0.00001f;
            DirectionalColor = Color.White.ToVector3() * 1.0f;
            DirectionalDirection = OriginalDirectionalDirection;
            //m_kTimer.AddTimer("Diffuse_Light_Switch", 2.0f, MoveDirectionalLight, true);
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

        }



        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        /// 

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

            dynamicsWorld = new DynamicWorld(ScreenManager.Game);
            spawnManager = new SpawnManager(ScreenManager.Game);
            bloom_screen = new Bloom(ScreenManager.Game);
            Platform p = new Platform(ScreenManager.Game);
            p1 = new Platform(ScreenManager.Game);
            d = new Door(ScreenManager.Game);
            //Hammer h = new Hammer(ScreenManager.Game);
            player = new Ball(ScreenManager.Game);
            s = new Spring(ScreenManager.Game);

            ScreenManager.Game.Components.Add(dynamicsWorld);
            ScreenManager.Game.Components.Add(bloom_screen);
            ScreenManager.Game.Components.Add(p);
            ScreenManager.Game.Components.Add(p1);
            ScreenManager.Game.Components.Add(player);
            ScreenManager.Game.Components.Add(d);
            //ScreenManager.Game.Components.Add(h);
            ScreenManager.Game.Components.Add(s);
            player.setCoordinates(new Vector3(0, 200, 1900));
            p.setCoordinates(new Vector3(0, 0, 1900));
            p1.setCoordinates(new Vector3(100, 0, 1900));
            d.setCoordinates(new Vector3(150, 0, 1900));
           // p1.m_qRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(25.0f));
            //h.setCoordinates(new Vector3(150, 200, 1900));
            s.setCoordinates(new Vector3(100, 200, 1900));
            bloom_screen.DrawOrder = 1;
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
            var mouseState = Mouse.GetState();
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            dynamicsWorld.StepSimulation(gameTime);
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                //this dont work yet
                if (dynamicsWorld.castRay(
                    new Vector3(mouseState.X, mouseState.Y, CameraMatrix.Translation.Z),
                    new Vector3(mouseState.X, mouseState.Y, -1000)
                    ))
                    Console.WriteLine("HITSOMETHING");
            }
            if (dynamicsWorld.areColliding(player, d))
                Console.WriteLine("DONE");
            if (dynamicsWorld.areColliding(player, s))
            {
                Console.WriteLine("JUMP");
                player.body.ApplyImpulse(Vector3.UnitY * 1000, -Vector3.UnitY);
            }
            if (dynamicsWorld.areColliding(s, p1))
            {
                Console.WriteLine("H");
                s.body.Gravity = Vector3.Zero;
            }
            if (IsActive)
            {

                bloom_screen.Visible = true;
            }
            else
            {
                bloom_screen.Visible = false;
            }
            Vector3 vTranslate = CameraMatrix.Translation;
            vTranslate.X = -player.body.WorldTransform.Translation.X;
            Matrix mTranslate = Matrix.CreateTranslation(vTranslate);
            CameraMatrix = Matrix.CreateFromQuaternion(CameraRotation) * mTranslate;
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
                if (input.ShipTurnLeft)
                {
                    player.ApplyForce(-Vector3.UnitX * 150.0f, Vector3.UnitX);
                }
                if (input.ShipTurnRight)
                {
                    player.ApplyForce(Vector3.UnitX * 150.0f, -Vector3.UnitX);
                }
                if (input.ShipMoveForward)
                {
                    Matrix Translation = Matrix.CreateTranslation(CameraMatrix.Translation + new Vector3(0.0f, 0.0f, -1000.0f * (float)gameTime.ElapsedGameTime.TotalSeconds));
                    CameraMatrix = Matrix.CreateFromQuaternion(CameraRotation) * Translation;
                }
                if (input.ShipMoveBackward)
                {
                    Matrix Translation = Matrix.CreateTranslation(CameraMatrix.Translation + new Vector3(0.0f, 0.0f, 1000.0f * (float)gameTime.ElapsedGameTime.TotalSeconds));
                    CameraMatrix = Matrix.CreateFromQuaternion(CameraRotation) * Translation;
                }
                if (input.ShipFire)
                {
                    player.body.ApplyImpulse(Vector3.UnitY * 150.0f, -Vector3.UnitY);
                }
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!

            bloom_screen.BloomSetInitialRenderTarget();
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);
            Matrix translate = player.body.CenterOfMassTransform;
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
