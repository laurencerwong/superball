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
    /// This is Level Editor Camera that implements Camera.
    /// </summary>
    public class LevelEditorCamera : Camera
    {
        public LevelEditorCamera(Game game)
            : base(game)
        {
            m_CameraMatrix = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 2000.0f),
                Vector3.Zero, Vector3.UnitY);
            m_ProjectionMatrix = Matrix.CreateOrthographic(1024.0f, 768.0f, 0.1f, 10000.0f);
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
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}
