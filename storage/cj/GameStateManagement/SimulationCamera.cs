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

        Vector3 cameraPosition;
        Vector3 cameraTarget;
        Vector3 cameraLeftVector;
        Vector3 cameraUpVector;

        public SimulationCamera(Game game, Ship ship)
            : base(game)
        {
            shipPosition = ship.GetWorldPosition();
            vShipForward = ship.GetWorldFacing();
            vShipUp = ship.GetWorldUp();

            cameraPosition = shipPosition - vShipForward * fHDist + vShipUp * fVDist;
            cameraTarget = shipPosition - cameraPosition;
            Vector3.Normalize(cameraTarget);
            cameraLeftVector = Vector3.Cross(vShipUp, cameraTarget);
            cameraUpVector = Vector3.Cross(cameraTarget, cameraLeftVector);
            //cameraPosition += shipOffset;
            m_CameraMatrix = Matrix.CreateLookAt(cameraPosition, shipPosition, cameraUpVector);

            m_ProjectionMatrix =
                Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(60.0f),
                    game.GraphicsDevice.Viewport.AspectRatio,
                    0.1f,
                    10000.0f
                );
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

        public override void updateCamera(Ship ship)
        {
            shipPosition = ship.GetWorldPosition();
            vShipForward = ship.GetWorldFacing();
            vShipUp = ship.GetWorldUp();

            cameraPosition = shipPosition - vShipForward * fHDist + vShipUp * fVDist;
            cameraTarget = shipPosition - cameraPosition;
            Vector3.Normalize(cameraTarget);
            cameraLeftVector = Vector3.Cross(vShipUp, cameraTarget);
            cameraUpVector = Vector3.Cross(cameraTarget, cameraLeftVector);
            //cameraPosition += shipOffset;
            m_CameraMatrix = Matrix.CreateLookAt(cameraPosition, shipPosition, cameraUpVector);
        }
    }
}
