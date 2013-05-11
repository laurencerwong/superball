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
    /// This is the Simulation Camera that implements Camera.
    /// </summary>
    public class SimulationCamera : Camera
    {
        Vector3 shipPosition;
        Vector3 vShipForward;
        Vector3 vShipUp;
        Vector3 shipOffset = new Vector3(0, 0, 30);
        float fHDist = 150.0f;
        float fVDist = 80.0f;
        float currentRotation = 0.0f;
        //float FOV = 60.0f;

        // Spring camera
        float fSpringConstant;
        float fDampConstant;
        Vector3 vDisplacement;
        Vector3 vSpringAccel;
        Vector3 vCameraVelocity;
        Vector3 cameraTarget;
        Vector3 cameraLeftVector;
        Vector3 cameraUpVector;

        // Mouse
        float xRot = 0.0f;//MathHelper.PiOver2;
        float yRot = 0.0f;//-MathHelper.Pi / 10.0f;
        const float rotationSpeed = 5.0f;

        // Keeping mouse centered
        int centerX;
        int centerY;

        int zoomLevel
        {
            get
            {
                return _zoomLevel;
            }
            set
            {
                _zoomLevelOld = _zoomLevel;
                _zoomLevel = value;
                ZoomCamera(_zoomLevelOld - _zoomLevel);
            }
        }
        int _zoomLevel = 0;
        int _zoomLevelOld = 0;


        Quaternion q_cameraRotation = Quaternion.Identity;

        public SimulationCamera(Game game, Ball player)
            : base(game)
        {
            shipPosition = player.GetWorldPosition();
            vShipForward = player.GetWorldFacing();
            vShipUp = player.GetWorldUp();

            cameraPosition = shipPosition - vShipForward * fHDist + Vector3.UnitY * fVDist;
            cameraTarget = shipPosition - cameraPosition;
            Vector3.Normalize(cameraTarget);
            cameraLeftVector = Vector3.Cross(vShipUp, cameraTarget);
            cameraUpVector = Vector3.Cross(cameraTarget, cameraLeftVector);
            m_CameraMatrix = Matrix.CreateLookAt(cameraPosition, shipPosition, Vector3.UnitY);

            // Keeping mouse centered - find center of game window
            centerX = game.Window.ClientBounds.Width / 2;
            centerY = game.Window.ClientBounds.Height / 2;

            // Spring camera
            fSpringConstant = 10.0f;
            fDampConstant = (float)(2.0f * Math.Sqrt(fSpringConstant));

            m_ProjectionMatrix =
                Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60.0f),
                    game.GraphicsDevice.Viewport.AspectRatio,
                    0.1f,
                    10000.0f
                );
        }

        public void ZoomCamera(int zoomAmount)
        {

            if (zoomAmount > 0)
            {
                    fVDist += 5.0f;
                    fHDist += 5.0f;
            }
            else if (zoomAmount < 0)
            {
                if (fVDist > 5.0f && fHDist > 5.0f)
                {
                    fVDist -= 5.0f;
                    fHDist -= 5.0f;
                }
            }
        }

        /// <summary>
        /// Allows the component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
        }

        /// <summary>
        /// Allows the component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void updateCamera(Ball player, GameTime gameTime)
        {
            float fDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            zoomLevel = Mouse.GetState().ScrollWheelValue;
            shipPosition = player.GetWorldPosition();

            // Mouse/Orbit camera
            MouseState currentMouseState = Mouse.GetState();
            float xDiff = currentMouseState.X - centerX;
            float yDiff = currentMouseState.Y - centerY;
            xRot -= rotationSpeed * xDiff * fDelta;
            yRot -= rotationSpeed * yDiff * fDelta;
            // Keep mouse centered
            Mouse.SetPosition(centerX, centerY);
            Quaternion rot1 = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(yRot));
            Quaternion rot2 = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(xRot));
            Quaternion mouseRotation = Quaternion.Concatenate(rot1, rot2);

            // Store the camera position as an [X, Y, Z] offset from the target object
            Vector3 offset = -Vector3.UnitX * fHDist + Vector3.UnitY * fVDist;
            // Rotate this camera offset using standard rotation methods (quaternion)
            offset = Vector3.Transform(offset, mouseRotation);
            Vector3 cameraIdealPosition = shipPosition + offset;
            
            // Spring camera
            vDisplacement = cameraPosition - cameraIdealPosition;
            vSpringAccel = (-fSpringConstant * vDisplacement) - (fDampConstant * vCameraVelocity);
            vCameraVelocity += vSpringAccel * fDelta;
            cameraPosition += vCameraVelocity * fDelta;

            cameraTarget = shipPosition;

            m_CameraMatrix = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.UnitY);

        }
    }
}
