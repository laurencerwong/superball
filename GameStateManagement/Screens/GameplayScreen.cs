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
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna;
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
        public SpriteFont gameFont;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();

        Utils.Timer m_kTimer = new Utils.Timer();
        public bool updateTimer = false;
        public float elapsedTime = 0.0f;

        public static Matrix prevCameraMatrix;
        public static Matrix prevProjectionMatrix;
        public static Matrix CameraMatrix;
        public static Matrix ProjectionMatrix;
        public static Vector3 SpecularColor;
        public static Vector3 AmbientColor;
        public static Vector3 DirectionalColor;
        public static Vector3 DirectionalDirection;
        public static float SpecularIntensity;

        public List<InventoryObject> inventoryObjects;

        public DynamicWorld dynamicsWorld;
        public static Vector3 playerSpawnCoordinates;
        public Ball player;

        public Actor selectedActor = null;
        public Actor mousedOverActor = null;

        public Texture2D circleTexture;

        public Vector3 LastMousePosition;
        public Vector3 CurrentMousePosition;
        public MouseState mouseState;
        public enum RotateSelectedAxis
        {
            X, Y, Z
        }
        public RotateSelectedAxis CurrentAxis = RotateSelectedAxis.X;
        public Vector3[] Axis;
        public bool FastCamera = false;

        public static AudioEngine audio;
        public static WaveBank wavebank;
        public static SoundBank soundbank;

        public LinkedList<Actor> actorsBlockingCamera;

        public static string spawnManagerStartingLevelName = "Demo.xml";
        public static int spawnManagerStartingLevelNumber = 0;

        Quaternion CameraRotation = Quaternion.Identity;

        public bool physWorldEnabled = true;

        public static String time;

        SpawnManager spawnManager;
        Bloom bloom_screen;
        ShaderHelper shaderHelper;
        Skybox sky;

        // Camera stuff
        Camera camera;
        public enum CameraTypes { LevelEditor, Simulation, None };
        public static CameraTypes cameraType;

        public static bool SavingInProgress = false;
        public static string saveName;

        public Vector3 getPlayerPosition()
        {
            return new Vector3(player.body.WorldTransform.Translation.X, player.body.WorldTransform.Translation.Y, player.body.WorldTransform.Translation.Z);
        }

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {

            CameraMatrix = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 100.0f),
                Vector3.Zero, Vector3.UnitY);

            prevCameraMatrix = CameraMatrix;

            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70.0f), 1.33f, 0.1f, 2000.0f);
            //ProjectionMatrix = Matrix.CreateOrthographic(1024.0f, 768.0f, 0.1f, 10000.0f);
            AmbientColor = Color.DarkGray.ToVector3();// *0.4f;
            SpecularColor = Color.White.ToVector3() * 0.5f;// *2.0f;
            SpecularIntensity = 0.00001f;
            DirectionalColor = Color.LightGray.ToVector3() * 1.0f;
            DirectionalDirection = new Vector3(0, 1, 0);
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            Axis = new Vector3[3];
            Axis[0] = Vector3.UnitX;
            Axis[1] = Vector3.UnitY;
            Axis[2] = Vector3.UnitZ;


            playerSpawnCoordinates = new Vector3(0, 200, 0);
            actorsBlockingCamera = new LinkedList<Actor>();



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

            circleTexture = ScreenManager.Game.Content.Load<Texture2D>("circletexture");

            ScreenManager.Game.ResetElapsedTime();

            dynamicsWorld = new DynamicWorld(ScreenManager.Game);
            DebugDrawer debugDrawer = new DebugDrawer(ScreenManager.Game.GraphicsDevice);
            debugDrawer.DebugMode = DebugDrawModes.DrawWireframe;
            DynamicWorld.dynamicsWorld.DebugDrawer = debugDrawer;
            DynamicWorld.dynamicsWorld.DispatchInfo.DebugDraw = debugDrawer;


            inventoryObjects = new List<InventoryObject>();
            SpawnManager.startingLevelName = spawnManagerStartingLevelName;
            spawnManager = new SpawnManager(ScreenManager.Game);
            //spawnPlayer();
            int count = 0;
            if (spawnManager.gAllowedPlatforms > 0)
                count++;
            if (spawnManager.gAllowedHammers > 0)
                count++;
            if (spawnManager.gAllowedSprings > 0)
                count++;
            if (spawnManager.gAllowedGravityBalls > 0)
                count++;
            if (spawnManager.gAllowedRamps > 0)
                count++;
            int numInv = 0;
            float fract = 1024.0f / (float)count;
            float first = fract / 2.0f;
            if (spawnManager.gAllowedPlatforms > 0)
            {
                PlatformSelect platformSelect = new PlatformSelect(ScreenManager.Game);
                inventoryObjects.Add(platformSelect);
                ScreenManager.Game.Components.Add(platformSelect);
                Vector3 location = new Vector3((first + (fract * numInv)) - 512.0f, -270, 0);
                platformSelect.setCoordinates(location);
                platformSelect.m_qRotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(15));
                numInv++;
            }
            if (spawnManager.gAllowedHammers > 0)
            {
                HammerSelect hammerSelect = new HammerSelect(ScreenManager.Game);
                inventoryObjects.Add(hammerSelect);
                ScreenManager.Game.Components.Add(hammerSelect);
                Vector3 location = new Vector3((first + (fract * numInv)) - 512.0f, -270, 0);
                hammerSelect.setCoordinates(location);
                //hammerSelect.m_qRotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(15));
                numInv++;
            }
            if (spawnManager.gAllowedSprings > 0)
            {
                SpringSelect springSelect = new SpringSelect(ScreenManager.Game);
                inventoryObjects.Add(springSelect);
                ScreenManager.Game.Components.Add(springSelect);
                Vector3 location = new Vector3((first + (fract * numInv)) - 512.0f, -270, 0);
                springSelect.setCoordinates(location);
                springSelect.m_qRotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(15));
                numInv++;
            }
            if (spawnManager.gAllowedGravityBalls > 0)
            {
                GravityBallSelect ballSelect = new GravityBallSelect(ScreenManager.Game);
                inventoryObjects.Add(ballSelect);
                ScreenManager.Game.Components.Add(ballSelect);
                Vector3 location = new Vector3((first + (fract * numInv)) - 512.0f, -270, 0);
                ballSelect.setCoordinates(location);
                //ballSelect.m_qRotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(15));
                numInv++;
            }
            if (spawnManager.gAllowedRamps > 0)
            {
                RampSelect rampSelect = new RampSelect(ScreenManager.Game);
                inventoryObjects.Add(rampSelect);
                ScreenManager.Game.Components.Add(rampSelect);
                Vector3 location = new Vector3((first + (fract * numInv)) - 512.0f, -270, 0);
                rampSelect.setCoordinates(location);
                rampSelect.m_qRotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(15));
                numInv++;
            }

            bloom_screen = new Bloom(ScreenManager.Game);
            //Bloom helper ensures that all the actors are drawn before the gui elements.
            sky = new Skybox(ScreenManager.Game);
            shaderHelper = new ShaderHelper(ScreenManager.Game);
            shaderHelper.bloom = bloom_screen;
            ScreenManager.Game.Components.Add(shaderHelper);
            ScreenManager.Game.Components.Add(dynamicsWorld);
            ScreenManager.Game.Components.Add(sky);
            ScreenManager.Game.Components.Add(bloom_screen);

            bloom_screen.DrawOrder = 10;

            // Music stuff

            Song song = content.Load<Song>("gameplaymusic");
            MediaPlayer.Volume = 1;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(song);

            // Camera stuff
            camera = new LevelEditorCamera(ScreenManager.Game);
            CameraMatrix = camera.m_CameraMatrix;
            ProjectionMatrix = camera.m_ProjectionMatrix;
            cameraType = CameraTypes.LevelEditor;
            soundbank.PlayCue("Start_Game");
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
            MediaPlayer.Stop();
            physWorldEnabled = false;
            foreach (GameComponent gc in ScreenManager.Game.Components)
            {
                if (gc is Actor)
                    ((Actor)gc).Visible = false;
            }
            //Had to manually remove everything :(
            for (int i = 0; i < inventoryObjects.Count; i++)
            {
                inventoryObjects[i].Dispose();
            }
            inventoryObjects = null;
            sky.Dispose();
            sky = null;
            dynamicsWorld.DestroyDynamicWorld();
        }


        #endregion

        public void spawnPlayer()
        {
            player = new Ball(ScreenManager.Game);
            ScreenManager.Game.Components.Add(player);
            player.Visible = true;
            player.setCoordinates(playerSpawnCoordinates);
        }

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

                camera.updateCamera(player, gameTime);
                sky.updatePosition(player == null ? camera.Translation : player.body.CenterOfMassPosition);
                prevCameraMatrix = CameraMatrix;
                CameraMatrix = camera.m_CameraMatrix;

                mouseState = Mouse.GetState();
                LastMousePosition = CurrentMousePosition;
                CurrentMousePosition = ScreenManager.Game.GraphicsDevice.Viewport.Unproject(
                        new Vector3(mouseState.X, mouseState.Y, CameraMatrix.Translation.Z),
                        ProjectionMatrix, CameraMatrix, Matrix.Identity);
                if (physWorldEnabled)
                {
                    if (updateTimer)
                    {
                        elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    }
                    dynamicsWorld.StepSimulation(gameTime);
                    if (player != null)
                    {
                        player.CheckOnGround(dynamicsWorld.hasCollision(player));
                        if (dynamicsWorld.areColliding(player.body, spawnManager.getTargetDoor.body))
                        {
                            ScreenManager.AddScreen(new FinishedLevelScreen(spawnManager));
                        }
                        foreach (Spring s in spawnManager.getSprings)
                        {
                            if (dynamicsWorld.areColliding(player.body, s.triggerBody))
                            {
                                player.body.ApplyImpulse(s.body.WorldTransform.Up * 1000, -Vector3.UnitY);
                            }
                            if (s.body.WorldTransform.Translation.Y < -1000)
                                s.Despawn();
                        }
                        foreach (GravityBall gra in spawnManager.getGravityBalls)
                        {
                            if (dynamicsWorld.areColliding(player.body, gra.triggerBody))
                            {
                                Vector3 dir = gra.triggerBody.CenterOfMassPosition - player.body.CenterOfMassPosition;
                                //Vector3 GravBallInfluence = (gra.Power * Vector3.Normalize(dir) * (float)(1.0+1.0/dir.LengthSquared()));
                                Vector3 GravBallInfluence = 1200.0f * Vector3.UnitY;
                                player.body.ApplyCentralForce(GravBallInfluence);
                            }
                        }
                        /*foreach (Ramp r in spawnManager.getRamps)
                        {
                            Vector3 pointOfContact = new Vector3();
                            if (dynamicsWorld.collisionPoint(player.body, r.body, ref pointOfContact))
                            {
                                player.body.ApplyCentralImpulse((pointOfContact - player.body.WorldTransform.Translation));
                            }
                        }*/
                        if (player.body.WorldTransform.Translation.Y < -10000)
                        {
                            ScreenManager.AddScreen(new LoseScreen());
                        }

                    }
                    if (selectedActor == null)
                    {
                        dynamicsWorld.castRay(
                                new Vector3(CurrentMousePosition.X, CurrentMousePosition.Y, CurrentMousePosition.Z),
                                new Vector3(CurrentMousePosition.X, CurrentMousePosition.Y, -2000),
                                CollisionFilterGroups.DebrisFilter,
                                ref mousedOverActor
                                );
                    }
                    else
                    {
                        mousedOverActor = null;
                    }
                    if (cameraType == CameraTypes.Simulation)
                    {
                        actorsBlockingCamera = dynamicsWorld.castRay(Camera.cameraPosition, player.body.WorldTransform.Translation);
                        foreach (Actor a in actorsBlockingCamera)
                        {
                            a.alpha = 0.1f;
                        }
                        actorsBlockingCamera.Clear();


                    }
                    if (cameraType == CameraTypes.LevelEditor)
                    {
                        foreach (InventoryObject inv in inventoryObjects)
                        {
                            inv.setCoordinates(new Vector3(inv.GetWorldPosition().X - (CameraMatrix.Translation.X - prevCameraMatrix.Translation.X),
                                inv.GetWorldPosition().Y - (CameraMatrix.Translation.Y - prevCameraMatrix.Translation.Y),
                                inv.GetWorldPosition().Z - (CameraMatrix.Translation.Z - prevCameraMatrix.Translation.Z)));
                        }
                        prevCameraMatrix = CameraMatrix;
                    }
                }
            }

        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        /// 

        public override void HandleInput(InputState input, GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.PauseGame)
            {
                // If they pressed pause, bring up the pause menu screen.
                ScreenManager.AddScreen(new PauseMenuScreen());
            }
            if ((input.SaveLevel && cameraType == CameraTypes.Simulation )|| SavingInProgress)
            {
                if (SavingInProgress == false)
                {
                    physWorldEnabled = false;
                    SavingInProgress = true;
                    return;
                }
                if (input.IsNewKeyPress(Keys.Enter))
                {
                    physWorldEnabled = true;
                    spawnManager.saveLevel(saveName);
                    SavingInProgress = false;
                }
                else
                {
                    // README!!!! - CJ
                    // if you set SavingInProgress here, then the 'v' appears as part of the saveName
                    // simple fix is just to press 'backspace' when you're saving to get rid of the 'v'
                    //SavingInProgress = true;
                    if (input.KeyPressed != Keys.None)
                    {
                        if (SavingInProgress)
                        {
                            if (input.KeyPressed == Keys.Back)
                            {
                                if (saveName.Length > 0)
                                {
                                    saveName = saveName.Remove(saveName.Length - 1, 1);
                                }
                            }
                            else if (input.KeyPressed.ToString().Length == 2) //Assuming it's a number
                            {
                                saveName += input.KeyPressed.ToString().Substring(1, 1);
                            }
                            else
                            {
                                if (input.ShiftHeld)
                                {
                                    saveName += input.KeyPressed.ToString();
                                }
                                else
                                {
                                    saveName += input.KeyPressed.ToString().ToLower();
                                }
                            }
                        }
                    }
                    // if you set SavingInProgress here, the above issue is fixed
                    // BUT, SavingInProgress doesn't seem to be set back to false
                    // and thus the draw() function keeps drawing the saveName with
                    // the cursor '|'
                }
            }
            if (SavingInProgress)
            {
                return;
            }
            else
            {
                if (input.LeftClick)
                {
                    /*Actor trialSelected = new Actor(this.ScreenManager.Game);
                    if (selectedActor != null)
                    {
                        dynamicsWorld.castRay(
                            new Vector3(CurrentMousePosition.X, CurrentMousePosition.Y, 1000),
                            new Vector3(CurrentMousePosition.X, CurrentMousePosition.Y, -2000),
                            ref trialSelected
                            );
                    }
                    else
                    {
                        dynamicsWorld.castRay(
                            new Vector3(CurrentMousePosition.X, CurrentMousePosition.Y, 1000),
                            new Vector3(CurrentMousePosition.X, CurrentMousePosition.Y, -2000),
                            ref selectedActor
                            );
                    }
                    if (trialSelected.Equals(selectedActor))
                    {
                     */
                    dynamicsWorld.castRay(
                        new Vector3(CurrentMousePosition.X, CurrentMousePosition.Y, 1000),
                        new Vector3(CurrentMousePosition.X, CurrentMousePosition.Y, -2000),
                        ref selectedActor
                        );
                }
                if (input.RightReleased && selectedActor != null) //copies and pastes the current object
                {
                    if (selectedActor is Platform)
                    {
                        spawnManager.placePlatform(selectedActor.body.WorldTransform.Translation.X,
                            selectedActor.body.WorldTransform.Translation.Y,
                            selectedActor.body.WorldTransform.Translation.Z);
                        spawnManager.getPlatforms[spawnManager.getPlatforms.Count - 1].m_qRotation = selectedActor.m_qRotation;
                    }
                    if (selectedActor is Spring)
                    {
                        spawnManager.placeSpring(selectedActor.body.WorldTransform.Translation.X,
                            selectedActor.body.WorldTransform.Translation.Y,
                            selectedActor.body.WorldTransform.Translation.Z);
                        spawnManager.getSprings[spawnManager.getSprings.Count - 1].m_qRotation = selectedActor.m_qRotation;
                    }
                    if (selectedActor is Hammer)
                    {
                        spawnManager.placeHammer(selectedActor.body.WorldTransform.Translation.X,
                            selectedActor.body.WorldTransform.Translation.Y,
                            selectedActor.body.WorldTransform.Translation.Z);
                        spawnManager.getHammers[spawnManager.getHammers.Count - 1].m_qRotation = selectedActor.m_qRotation;
                    }
                    if (selectedActor is Ramp)
                    {
                        spawnManager.placeRamp(selectedActor.body.WorldTransform.Translation.X,
                            selectedActor.body.WorldTransform.Translation.Y,
                            selectedActor.body.WorldTransform.Translation.Z);
                        spawnManager.getRamps[spawnManager.getRamps.Count - 1].m_qRotation = selectedActor.m_qRotation;
                    }
                    if (selectedActor is GravityBall)
                    {
                        spawnManager.placeGravityBall(selectedActor.body.WorldTransform.Translation.X,
                            selectedActor.body.WorldTransform.Translation.Y,
                            selectedActor.body.WorldTransform.Translation.Z);
                        spawnManager.getGravityBalls[spawnManager.getGravityBalls.Count - 1].triggerRadius = ((GravityBall)selectedActor).triggerRadius;
                    }
                }
                //}
                if (input.ChangeCamera)
                {
                    if (cameraType == CameraTypes.LevelEditor && selectedActor != null)
                    {
                        selectedActor.select();
                        selectedActor = null;
                    }
                    changeCamera();
                }
                if (input.ToggleBloom)
                {
                    bloom_screen.toggleBloom();
                }
                switch (cameraType)
                {
                    case CameraTypes.LevelEditor:
                        if (input.PlaceHammer)
                        {
                            spawnManager.placeHammer(CurrentMousePosition.X, CurrentMousePosition.Y - 51.8461f, 0);
                        }
                        if (input.PlacePlatform)
                        {
                            spawnManager.placePlatform(CurrentMousePosition.X, CurrentMousePosition.Y, 0);
                        }
                        if (input.PlaceSpring)
                        {
                            spawnManager.placeSpring(CurrentMousePosition.X, CurrentMousePosition.Y, 0);
                        }
                        if (input.PlaceGravityBall)
                        {
                            spawnManager.placeGravityBall(CurrentMousePosition.X, CurrentMousePosition.Y, 0);
                        }
                        if (input.PlaceRamp)
                        {
                            spawnManager.placeRamp(CurrentMousePosition.X, CurrentMousePosition.Y, 0);
                        }
                        if (selectedActor != null && input.MiddleReleased)
                        {
                            CurrentAxis = (RotateSelectedAxis)(((int)CurrentAxis + 1) % 3);
                        }
                        if (mousedOverActor != null)
                        {
                            if (mousedOverActor is InventoryObject)
                            {
                                mousedOverActor.m_qRotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(3));
                            }
                        }
                        if (selectedActor != null)
                        {
                            //Console.WriteLine("FOUND ACTOR");
                            if (selectedActor is InventoryObject)
                            {
                                if (selectedActor is PlatformSelect)
                                {
                                    selectedActor = spawnManager.placePlatform(CurrentMousePosition.X, CurrentMousePosition.Y, 0);
                                }
                                if (selectedActor is HammerSelect)
                                {
                                    selectedActor = spawnManager.placeHammer(CurrentMousePosition.X, CurrentMousePosition.Y - 51.8461f, 0);
                                }
                                if (selectedActor is SpringSelect)
                                {
                                    selectedActor = spawnManager.placeSpring(CurrentMousePosition.X, CurrentMousePosition.Y, 0);
                                }
                                if (selectedActor is RampSelect)
                                {
                                    selectedActor = spawnManager.placeRamp(CurrentMousePosition.X, CurrentMousePosition.Y, 0);
                                }
                                if (selectedActor is GravityBallSelect)
                                {
                                    selectedActor = spawnManager.placeGravityBall(CurrentMousePosition.X, CurrentMousePosition.Y, 0);
                                }
                                if (selectedActor != null)
                                {
                                    selectedActor.select();
                                }
                            }
                            else
                            {
                                selectedActor.TranslateBody(new Vector3(CurrentMousePosition.X, CurrentMousePosition.Y, 0.0f), input.ShiftHeld);
                                if (selectedActor is PlayerSpawn)
                                {
                                    GameplayScreen.playerSpawnCoordinates = selectedActor.body.WorldTransform.Translation;
                                }
                            }
                        }
                        if (selectedActor != null && input.RotateActor)
                        {
                            if (input.ShiftHeld)
                                selectedActor.Rotate(Axis[(int)CurrentAxis], -MathHelper.Pi/8.0f);
                            else
                                selectedActor.Rotate(Axis[(int)CurrentAxis], MathHelper.Pi/8.0f);
                        }
                        if (selectedActor == null && input.RotateActor)
                        {
                            camera.m_qRotation *= Quaternion.CreateFromAxisAngle(Axis[(int)CurrentAxis], MathHelper.PiOver4);
                        }
                        if (selectedActor != null && input.DeleteObject)
                        {
                            if(!(selectedActor is PlayerSpawn))
                                selectedActor.Despawn();
                            if (selectedActor is Hammer)
                            {
                                spawnManager.gAllowedHammers++;
                            }
                            if (selectedActor is Platform)
                            {
                                spawnManager.gAllowedPlatforms++;
                            }
                            if (selectedActor is Spring)
                            {
                                spawnManager.gAllowedSprings++;
                            }
                            if (selectedActor is GravityBall)
                            {
                                spawnManager.gAllowedGravityBalls++;
                            }
                            if (selectedActor is Ramp)
                            {
                                spawnManager.gAllowedRamps++;
                            }
                        }
                        FastCamera = false;
                        if (input.ShiftHeld)
                        {
                            FastCamera = true;
                        }
                        if (input.ShipMoveForward)
                        {
                            Vector3 tempCameraTranslation = camera.Translation;
                            tempCameraTranslation.Y += (FastCamera ? 500.0f : 300.0f) * deltaTime;
                            camera.Translation = tempCameraTranslation;
                        }
                        if (input.ShipMoveBackward)
                        {
                            Vector3 tempCameraTranslation = camera.Translation;
                            tempCameraTranslation.Y -= (FastCamera ? 500.0f : 300.0f) * deltaTime;
                            camera.Translation = tempCameraTranslation;
                        }
                        if (input.ShipTurnLeft)
                        {
                            Vector3 tempCameraTranslation = camera.Translation;
                            tempCameraTranslation.X -= (FastCamera ? 500.0f : 300.0f) * deltaTime;
                            camera.Translation = tempCameraTranslation;
                        }
                        if (input.ShipTurnRight)
                        {
                            Vector3 tempCameraTranslation = camera.Translation;
                            tempCameraTranslation.X += (FastCamera ? 500.0f : 300.0f) * deltaTime;
                            camera.Translation = tempCameraTranslation;
                        }
                        if (input.ShipFire)
                        {
                        }
                        break;


                    case CameraTypes.Simulation:
                        if (input.ShipTurnLeft)
                        {
                            player.ApplyImpulse(Vector3.Cross(Vector3.UnitY, GetPlayerCameraForward(player.body.CenterOfMassPosition, Camera.cameraPosition)), CameraMatrix.Left, deltaTime);
                        }
                        if (input.ShipTurnRight)
                        {
                            player.ApplyImpulse(Vector3.Cross(GetPlayerCameraForward(player.body.CenterOfMassPosition, Camera.cameraPosition), Vector3.UnitY), CameraMatrix.Right, deltaTime);
                        }

                        if (input.ShipMoveForward)
                        {
                            player.ApplyImpulse(GetPlayerCameraForward(player.body.CenterOfMassPosition, Camera.cameraPosition), CameraMatrix.Forward, deltaTime);
                        }
                        if (input.ShipMoveBackward)
                        {
                            player.ApplyImpulse(-GetPlayerCameraForward(player.body.CenterOfMassPosition, Camera.cameraPosition), -CameraMatrix.Forward, deltaTime);
                        }
                        if (input.ShipFire)
                        {
                            if (player.Jump(deltaTime))
                            {
                                soundbank.PlayCue("Jump");
                            }
                        }
                        if (input.SaveLevel)
                        {
                            spawnManager.saveLevel("");
                        }

                        break;
                }
            }
        }

        public Vector3 GetPlayerCameraForward(Vector3 playerPos, Vector3 camPos)
        {
            Vector3 forwardVector = playerPos - camPos;
            forwardVector.Y = 0;//+= ( camPos.Y - playerPos.Y);
            forwardVector.Normalize();
            return forwardVector;
        }



        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            //bloom_screen.BloomSetInitialRenderTarget();
            /*ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);*/

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 mousePos = new Vector2(mouseState.X - 10, mouseState.Y - 25);
            spriteBatch.Begin();
            if (SavingInProgress)
            {
                Vector2 saveTextPos = new Vector2(600.0f, 0.0f);
                spriteBatch.DrawString(gameFont, "FileName: " + saveName + "|", saveTextPos, Color.Green);
            }
            spriteBatch.DrawString(gameFont, "Level: " + (spawnManager.gCurrentLevel + 1).ToString(), new Vector2(20, 5), Color.Green);
            if (cameraType == CameraTypes.LevelEditor)
            {
                spriteBatch.DrawString(gameFont, CurrentAxis.ToString() /*+ " " + CurrentMousePosition.X.ToString() + " " + CurrentMousePosition.Y.ToString()*/, mousePos, Color.Green);
                foreach (InventoryObject inv in inventoryObjects)
                {
                    Vector2 objText = new Vector2(inv.body.WorldTransform.Translation.X, inv.body.WorldTransform.Translation.Y);
                    Vector3 objloc = ScreenManager.Game.GraphicsDevice.Viewport.Project(
                        new Vector3(objText.X, objText.Y, CameraMatrix.Translation.Z),
                        ProjectionMatrix, CameraMatrix, Matrix.Identity);
                    if (inv is PlatformSelect)
                    {
                        //Vector2 platformText = new Vector2((inv.body.WorldTransform.Translation.X + 512) - 10, (384 - inv.body.WorldTransform.Translation.Y) + 20);
                        Vector2 platloc2 = new Vector2(objloc.X - 10, objloc.Y + 60);
                        spriteBatch.DrawString(gameFont, spawnManager.gAllowedPlatforms.ToString(), platloc2, Color.Green);
                    }
                    if (inv is HammerSelect)
                    {
                        Vector2 hamloc2 = new Vector2(objloc.X - 10, objloc.Y + 60);
                        spriteBatch.DrawString(gameFont, spawnManager.gAllowedHammers.ToString(), hamloc2, Color.Green);
                    }
                    if (inv is SpringSelect)
                    {
                        Vector2 sprloc2 = new Vector2(objloc.X - 10, objloc.Y + 60);
                        spriteBatch.DrawString(gameFont, spawnManager.gAllowedSprings.ToString(), sprloc2, Color.Green);
                    }
                    if (inv is GravityBallSelect)
                    {
                        Vector2 gbloc2 = new Vector2(objloc.X - 10, objloc.Y + 60);
                        spriteBatch.DrawString(gameFont, spawnManager.gAllowedGravityBalls.ToString(), gbloc2, Color.Green);
                    }
                    if (inv is RampSelect)
                    {
                        Vector2 rmploc2 = new Vector2(objloc.X - 10, objloc.Y + 60);

                        spriteBatch.DrawString(gameFont, spawnManager.gAllowedRamps.ToString(), rmploc2, Color.Green);
                    }
                }
                if (selectedActor != null)
                    if (selectedActor is GravityBall)
                    {
                        spriteBatch.Draw(circleTexture, new Rectangle((int)(mousePos.X + 10 - ((GravityBall)selectedActor).triggerRadius), (int)(mousePos.Y + 25 - ((GravityBall)selectedActor).triggerRadius),
                            (int)((GravityBall)selectedActor).triggerRadius * 2, (int)((GravityBall)selectedActor).triggerRadius * 2), Color.White);
                    }
            }
            else
            {
                Vector2 counterPosition = new Vector2(870, 700);
                int minutes = (int)(elapsedTime / 1000.0f / 60.0f);
                int seconds = ((int)elapsedTime / 1000) % 60;
                int milliseconds = ((int)elapsedTime % 100);
                String minutesString = minutes.ToString().PadLeft(2, '0');
                String secondsString = seconds.ToString().PadLeft(2, '0');
                time = minutesString + ":" + secondsString + "." + milliseconds;
                spriteBatch.DrawString(gameFont, time, counterPosition, Color.White);
            }
            //Vector2 hammerText = new Vector2(20, 720);
            //spriteBatch.DrawString(gameFont, "{H}ammers: " + spawnManager.gAllowedHammers.ToString(), hammerText, Color.Green);
            //Vector2 springText = new Vector2(hammerText.X + 270, hammerText.Y);
            //spriteBatch.DrawString(gameFont, "sprin{G}s: " + spawnManager.gAllowedSprings.ToString(), springText, Color.Green);
            //Vector2 platformText = new Vector2(springText.X + 250, springText.Y);
            //spriteBatch.DrawString(gameFont, "platfor{M}s: " + spawnManager.gAllowedPlatforms.ToString(), platformText, Color.Green);
            //Vector2 saveText = new Vector2(platformText.X + 270, platformText.Y);
            //spriteBatch.DrawString(gameFont, "Sa{V}e", saveText, Color.Green);
            spriteBatch.End();
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

        private void changeCamera()
        {
            if (cameraType == CameraTypes.LevelEditor)
            {
                updateTimer = true;
                spawnPlayer();
                ScreenManager.Game.IsMouseVisible = false;
                //Console.WriteLine("switching to SIMULATION CAMERA");
                camera = new SimulationCamera(ScreenManager.Game, player);
                CameraMatrix = camera.m_CameraMatrix;
                ProjectionMatrix = camera.m_ProjectionMatrix;
                cameraType = CameraTypes.Simulation;
                foreach (InventoryObject inv in inventoryObjects)
                {
                    inv.Visible = false;
                }
            }
            else if (cameraType == CameraTypes.Simulation)
            {
                updateTimer = false;
                elapsedTime = 0.0f;
                ScreenManager.Game.IsMouseVisible = false;
                //Console.WriteLine("switching to LEVEL EDITOR CAMERA");
                camera = new LevelEditorCamera(ScreenManager.Game);
                CameraMatrix = camera.m_CameraMatrix;
                ProjectionMatrix = camera.m_ProjectionMatrix;
                cameraType = CameraTypes.LevelEditor;
                player.Despawn();
                player = null;
                foreach (InventoryObject inv in inventoryObjects)
                {
                    inv.Visible = true;
                }
            }
        }
    }
}
