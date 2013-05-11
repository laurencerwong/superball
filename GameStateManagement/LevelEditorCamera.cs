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
        public bool followPlayer = false;
        int _zoomLevel = 0;
        int _zoomLevelOld = 0;
        public float horz = 1024.0f;
        public float vert = 768.0f;
        public Vector3 OldTranslation;
        public LevelEditorCamera(Game game)
            : base(game)
        {
            m_CameraMatrix = Matrix.CreateLookAt(Translation,
                Vector3.Zero, Vector3.UnitY);
            m_ProjectionMatrix = Matrix.CreateOrthographic(1024.0f, 768.0f, 0.1f, 10000.0f);
            Translation = new Vector3(0, 0, 100.0f);
            ApplyTranslation();
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

        public void ZoomCamera(int zoomAmount)
        {

            if (zoomAmount > 0)
            {
                vert += 50.0f;
                horz = (vert / 3.0f * 4.0f);
                m_ProjectionMatrix = Matrix.CreateOrthographic(horz, vert, 0.1f, 10000.0f);
                GameplayScreen.ProjectionMatrix = m_ProjectionMatrix;
            }
            else if (zoomAmount < 0 && vert - 50.0f > 0.0f)
            {
                vert -= 50.0f;
                horz = (vert / 3.0f * 4.0f);
                m_ProjectionMatrix = Matrix.CreateOrthographic(horz, vert, 0.1f, 10000.0f);
                GameplayScreen.ProjectionMatrix = m_ProjectionMatrix;
            }
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

        public override void updateCamera(Ball player, GameTime gameTime)
        {
            zoomLevel = Mouse.GetState().ScrollWheelValue;
            if (updateTranslation)
            {
                ApplyTranslation();
                updateTranslation = false;
            }
        }

        public override void ApplyTranslation()
        {
            //Translation = Vector3.Transform(Translation,Matrix.CreateFromQuaternion(m_qRotation));
            m_CameraMatrix = Matrix.CreateLookAt(Translation,
    (Translation * (Vector3.UnitX + Vector3.UnitY)), Vector3.UnitY);
            GameplayScreen.CameraMatrix = m_CameraMatrix;
            base.ApplyTranslation();
        }
    }
}
