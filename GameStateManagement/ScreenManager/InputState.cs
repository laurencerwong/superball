#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// Helper for reading input from keyboard and gamepad. This class tracks both
    /// the current and previous state of both input devices, and implements query
    /// properties for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        #region Fields

        public const int MaxInputs = 4;

        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;
        public MouseState CurrentMouseState;


        public readonly KeyboardState[] LastKeyboardStates;
        public readonly GamePadState[] LastGamePadStates;
        public MouseState LastMouseState;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            CurrentGamePadStates = new GamePadState[MaxInputs];
            CurrentMouseState = new MouseState();


            LastKeyboardStates = new KeyboardState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];
            LastMouseState = new MouseState();
        }


        #endregion

        #region Properties

        public Keys KeyPressed
        {
            get
            {
                if (IsNewKeyPress(Keys.A))
                {
                    return Keys.A;
                }
                if (IsNewKeyPress(Keys.B))
                {
                    return Keys.B;
                }
                if (IsNewKeyPress(Keys.C))
                {
                    return Keys.C;
                }
                if (IsNewKeyPress(Keys.D))
                {
                    return Keys.D;
                }
                if (IsNewKeyPress(Keys.E))
                {
                    return Keys.E;
                }
                if (IsNewKeyPress(Keys.F))
                {
                    return Keys.F;
                }
                if (IsNewKeyPress(Keys.G))
                {
                    return Keys.G;
                }
                if (IsNewKeyPress(Keys.H))
                {
                    return Keys.H;
                }
                if (IsNewKeyPress(Keys.I))
                {
                    return Keys.I;
                }
                if (IsNewKeyPress(Keys.J))
                {
                    return Keys.J;
                }
                if (IsNewKeyPress(Keys.K))
                {
                    return Keys.K;
                }
                if (IsNewKeyPress(Keys.L))
                {
                    return Keys.L;
                }
                if (IsNewKeyPress(Keys.M))
                {
                    return Keys.M;
                }
                if (IsNewKeyPress(Keys.N))
                {
                    return Keys.N;
                }
                if (IsNewKeyPress(Keys.O))
                {
                    return Keys.O;
                }
                if (IsNewKeyPress(Keys.P))
                {
                    return Keys.P;
                }
                if (IsNewKeyPress(Keys.Q))
                {
                    return Keys.Q;
                }
                if (IsNewKeyPress(Keys.R))
                {
                    return Keys.R;
                }
                if (IsNewKeyPress(Keys.S))
                {
                    return Keys.S;
                }
                if (IsNewKeyPress(Keys.T))
                {
                    return Keys.T;
                }
                if (IsNewKeyPress(Keys.U))
                {
                    return Keys.U;
                }
                if (IsNewKeyPress(Keys.V))
                {
                    return Keys.V;
                }
                if (IsNewKeyPress(Keys.W))
                {
                    return Keys.W;
                }
                if (IsNewKeyPress(Keys.X))
                {
                    return Keys.X;
                }
                if (IsNewKeyPress(Keys.Y))
                {
                    return Keys.Y;
                }
                if (IsNewKeyPress(Keys.Z))
                {
                    return Keys.Z;
                }
                if (IsNewKeyPress(Keys.D0))
                {
                    return Keys.D0;
                }
                if (IsNewKeyPress(Keys.D1))
                {
                    return Keys.D1;
                }
                if (IsNewKeyPress(Keys.D2))
                {
                    return Keys.D2;
                }
                if (IsNewKeyPress(Keys.D3))
                {
                    return Keys.D3;
                }
                if (IsNewKeyPress(Keys.D4))
                {
                    return Keys.D4;
                }
                if (IsNewKeyPress(Keys.D5))
                {
                    return Keys.D5;
                }
                if (IsNewKeyPress(Keys.D6))
                {
                    return Keys.D6;
                }
                if (IsNewKeyPress(Keys.D7))
                {
                    return Keys.D7;
                }
                if (IsNewKeyPress(Keys.D8))
                {
                    return Keys.D8;
                }
                if (IsNewKeyPress(Keys.D9))
                {
                    return Keys.D9;
                }
                if (IsNewKeyPress(Keys.Back))
                {
                    return Keys.Back;
                }
                return Keys.None;
                //return null;
            }
        }

        /// <summary>
        /// Checks for a "menu up" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuUp
        {
            get
            {
                return IsNewKeyPress(Keys.Up) ||
                       IsNewButtonPress(Buttons.DPadUp) ||
                       IsNewButtonPress(Buttons.LeftThumbstickUp);
            }
        }


        /// <summary>
        /// Checks for a "menu down" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuDown
        {
            get
            {
                return IsNewKeyPress(Keys.Down) ||
                       IsNewButtonPress(Buttons.DPadDown) ||
                       IsNewButtonPress(Buttons.LeftThumbstickDown);
            }
        }


        /// <summary>
        /// Checks for a "menu select" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuSelect
        {
            get
            {
                return IsNewKeyPress(Keys.Space) ||
                       IsNewKeyPress(Keys.Enter) ||
                       IsNewButtonPress(Buttons.A) ||
                       IsNewButtonPress(Buttons.Start);
            }
        }


        /// <summary>
        /// Checks for a "menu cancel" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuCancel
        {
            get
            {
                return IsNewKeyPress(Keys.Escape) ||
                       IsNewButtonPress(Buttons.B) ||
                       IsNewButtonPress(Buttons.Back);
            }
        }


        /// <summary>
        /// Checks for a "pause the game" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool PauseGame
        {
            get
            {
                return IsNewKeyPress(Keys.Escape) ||
                       IsNewButtonPress(Buttons.Back) ||
                       IsNewButtonPress(Buttons.Start);
            }
        }

        public bool ShipFire
        {
            get
            {
                return IsNewKeyPress(Keys.Space);
            }
        }

        public bool ShipTurnLeft
        {
            get
            {
                return IsKeyHeld(Keys.A);
            }
        }

        public bool ShipTurnRight
        {
            get
            {
                return IsKeyHeld(Keys.D);
            }
        }

        public bool ShipMoveForward
        {
            get
            {
                return IsKeyHeld(Keys.W);
            }
        }

        public bool ShipMoveBackward
        {
            get
            {
                return IsKeyHeld(Keys.S);
            }
        }
        public bool PlaceHammer
        {
            get
            {
                return IsNewKeyPress(Keys.H);
            }
        }
        public bool PlacePlatform
        {
            get
            {
                return IsNewKeyPress(Keys.M);
            }
        }
        public bool PlaceSpring
        {
            get
            {
                return IsNewKeyPress(Keys.G);
            }
        }
        public bool PlaceGravityBall
        {
            get
            {
                return IsNewKeyPress(Keys.L);
            }
        }
        public bool PlaceRamp
        {
            get
            {
                return IsNewKeyPress(Keys.U);
            }
        }
        public bool SaveLevel
        {
            get
            {
                return IsNewKeyPress(Keys.V);
            }
        }
        public bool ChangeCamera
        {
            get
            {
                return IsNewKeyPress(Keys.C);
            }
        }
        public bool RotateActor
        {
            get
            {
                return IsNewKeyPress(Keys.R);
            }
        }
        public bool HoldRotateActor
        {
            get
            {
                return IsKeyHeld(Keys.R);
            }
        }
        public bool ReleasedRotateActor
        {
            get
            {
                return IsNewKeyReleased(Keys.R);
            }
        }
        public bool ReleasedMoveHammer
        {
            get
            {
                return IsNewKeyReleased(Keys.LeftShift);
            }
        }
        public bool NewMoveHammer
        {
            get
            {
                return IsNewKeyPress(Keys.LeftShift);
            }
        }


        public bool LeftClick
        {
            get
            {
                return IsLeftMouseClicked();
            }
        }

        public bool RightReleased
        {
            get
            {
                return IsRightMouseReleased();
            }
        }

        public bool LeftHeld
        {
            get
            {
                return IsLeftMouseHeld();
            }
        }
        public bool LeftReleased
        {
            get
            {
                return IsLeftMouseReleased();
            }
        }

        public byte MouseWheel
        {
            get
            {
                return MouseWheelValue();
            }
        }
        public bool ShiftHeld
        {
            get
            {
                return IsKeyHeld(Keys.LeftShift);
            }
        }
        public bool ShiftReleased
        {
            get
            {
                return IsNewKeyPress(Keys.LeftShift);
            }
        }
        public bool MiddleReleased
        {
            get
            {
                return IsMiddleMouseReleased();
            }
        }
        public bool SetPlayerSpawn
        {
            get
            {
                return IsNewKeyPress(Keys.P);
            }
        }
        public bool DeleteObject
        {
            get
            {
                return IsNewKeyPress(Keys.Delete);
            }
        }
        public bool ToggleBloom
        {
            get
            {
                return IsNewKeyPress(Keys.B);
            }
        }


        #endregion

        #region Methods


        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            LastMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
            for (int i = 0; i < MaxInputs; i++)
            {
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                LastGamePadStates[i] = CurrentGamePadStates[i];


                CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

            }
        }


        /// <summary>
        /// Helper for checking if a key was newly pressed during this update,
        /// by any player.
        /// </summary>
        public bool IsNewKeyPress(Keys key)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsNewKeyPress(key, (PlayerIndex)i))
                    return true;
            }

            return false;
        }

        public bool IsNewKeyReleased(Keys key)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsNewKeyReleased(key, (PlayerIndex)i))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Helper for checking if a key was newly pressed during this update,
        /// by the specified player.
        /// </summary>
        public bool IsNewKeyPress(Keys key, PlayerIndex playerIndex)
        {
            return (CurrentKeyboardStates[(int)playerIndex].IsKeyDown(key) &&
                    LastKeyboardStates[(int)playerIndex].IsKeyUp(key));
        }

        public bool IsNewKeyReleased(Keys key, PlayerIndex playerIndex)
        {
            return (CurrentKeyboardStates[(int)playerIndex].IsKeyUp(key) &&
                    LastKeyboardStates[(int)playerIndex].IsKeyDown(key));
        }


        /// <summary>
        /// Helper for checking if a button was newly pressed during this update,
        /// by any player.
        /// </summary>
        public bool IsNewButtonPress(Buttons button)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsNewButtonPress(button, (PlayerIndex)i))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Helper for checking if a button was newly pressed during this update,
        /// by the specified player.
        /// </summary>
        public bool IsNewButtonPress(Buttons button, PlayerIndex playerIndex)
        {
            return (CurrentGamePadStates[(int)playerIndex].IsButtonDown(button) &&
                    LastGamePadStates[(int)playerIndex].IsButtonUp(button));
        }


        /// <summary>
        /// Helper for checking if a key was newly pressed during this update,
        /// by any player.
        /// </summary>
        public bool IsKeyHeld(Keys key)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsKeyHeld(key, (PlayerIndex)i))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Helper for checking if a key was newly pressed during this update,
        /// by the specified player.
        /// </summary>
        public bool IsKeyHeld(Keys key, PlayerIndex playerIndex)
        {
            return (CurrentKeyboardStates[(int)playerIndex].IsKeyDown(key) &&
                    LastKeyboardStates[(int)playerIndex].IsKeyDown(key));
        }


        public bool IsLeftMouseClicked()
        {
            return (CurrentMouseState.LeftButton == ButtonState.Pressed
                && LastMouseState.LeftButton == ButtonState.Released);
        }

        public bool IsRightMouseClicked()
        {
            return (CurrentMouseState.RightButton == ButtonState.Pressed
                && LastMouseState.RightButton == ButtonState.Released);
        }

        public bool IsLeftMouseHeld()
        {
            return (CurrentMouseState.LeftButton == ButtonState.Pressed
                && LastMouseState.LeftButton == ButtonState.Pressed);
        }

        public bool IsRightMouseHeld()
        {
            return (CurrentMouseState.RightButton == ButtonState.Pressed
                && LastMouseState.RightButton == ButtonState.Pressed);
        }

        public bool IsLeftMouseReleased()
        {
            return (CurrentMouseState.LeftButton == ButtonState.Released
                && LastMouseState.LeftButton == ButtonState.Pressed);
        }

        public bool IsRightMouseReleased()
        {
            return (CurrentMouseState.RightButton == ButtonState.Released
                && LastMouseState.RightButton == ButtonState.Pressed);
        }

        public byte MouseWheelValue()
        {
            if (LastMouseState.ScrollWheelValue - CurrentMouseState.ScrollWheelValue > 0)
                return 2;
            else if (LastMouseState.ScrollWheelValue - CurrentMouseState.ScrollWheelValue < 0)
                return 0;
            else
                return 1;
        }

        public bool IsMiddleMouseReleased()
        {
            return (LastMouseState.MiddleButton == ButtonState.Pressed &&
                CurrentMouseState.MiddleButton == ButtonState.Released);
        }


        /// <summary>
        /// Checks for a "menu select" input action from the specified player.
        /// </summary>
        public bool IsMenuSelect(PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Space, playerIndex) ||
                   IsNewKeyPress(Keys.Enter, playerIndex) ||
                   IsNewButtonPress(Buttons.A, playerIndex) ||
                   IsNewButtonPress(Buttons.Start, playerIndex);
        }


        /// <summary>
        /// Checks for a "menu cancel" input action from the specified player.
        /// </summary>
        public bool IsMenuCancel(PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Escape, playerIndex) ||
                   IsNewButtonPress(Buttons.B, playerIndex) ||
                   IsNewButtonPress(Buttons.Back, playerIndex);
        }


        #endregion
    }
}
