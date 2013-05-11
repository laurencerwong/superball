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
    /// This is the Camera component that implements Actor.
    /// </summary>
    public abstract class Camera : Actor
    {
        // Variables

        protected bool updateTranslation = false;

        public Matrix m_CameraMatrix
        {
            get
            {
                return _CameraMatrix;
            }
            set
            {
                _CameraMatrix = value;
            }
        }
        private Matrix _CameraMatrix;

        public Matrix m_ProjectionMatrix
        {
            get
            {
                return _ProjectionMatrix;
            }
            set
            {
                _ProjectionMatrix = value;
                GameplayScreen.prevProjectionMatrix = GameplayScreen.ProjectionMatrix;
                GameplayScreen.ProjectionMatrix = _ProjectionMatrix;
            }
        }
        private Matrix _ProjectionMatrix;

        public Quaternion m_qRotation
        {
            get
            {
                return _m_qRotation;
            }
            set
            {
                _m_qRotation = value;
                //ApplyTranslation();
                updateTranslation = true;
            }
        }
        private Quaternion _m_qRotation;

        public Vector3 Translation
        {
            get
            {
                return _Translation;
            }
            set
            {
                _Translation = value;
                //ApplyTranslation();
                updateTranslation = true;
            }
        }
        private Vector3 _Translation;
        public static Vector3 cameraPosition;
        // Functions
        public Camera(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            m_qRotation = Quaternion.Identity;
            ApplyTranslation();
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

        public virtual void updateCamera(Ball player, GameTime gameTime)
        {

        }

        public virtual void FollowPlayer()
        {
        }

        public virtual void ApplyTranslation()
        {
        }
    }
}
