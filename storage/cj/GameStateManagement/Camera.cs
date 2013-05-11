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
            }
        }
        private Matrix _ProjectionMatrix;

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

        public virtual void updateCamera(Ship m_kShip)
        {

        }
    }
}
