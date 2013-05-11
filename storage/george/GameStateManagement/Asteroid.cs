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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Asteroid : Actor
    {

        Random rand = new Random();
        public int name;

        public Asteroid(Game game)
            : base(game)
        {
            meshName = "Asteroid";
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            m_qRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2);
            while (vForce == Vector3.Zero)
            {
                vForce.X = (float)rand.Next(-5, 5);
                vForce.Y = (float)rand.Next(-5, 5);
            }
            vForce.X *= 1000f;
            vForce.Y *= 1000f;
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (m_vVelocity.Length() != 0.0f)
                m_qRotation *= Quaternion.Normalize(Quaternion.CreateFromAxisAngle(Vector3.Normalize(m_vVelocity), MathHelper.ToRadians(25.0f) * (float)gameTime.ElapsedGameTime.TotalSeconds));
            base.Update(gameTime);
        }

        public void Despawn()
        {
            Game.Components.Remove(this);
        }
    }
}
