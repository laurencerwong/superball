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
using BulletSharp;


namespace GameStateManagement
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Hammer : Actor
    {
        public BoxShape boundingBox;
        HingeConstraint hc;
        public Hammer(Game game)
            : base(game)
        {
            meshName = "wholehammer";
            useModelForCollision = true;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            fMass = 1000.0f;
            base.Initialize();
            m_fScale = 1.0f;
            generateBoundingShape(ref boundingBox);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        public void setCoordinates(Vector3 Position)
        {
            m_vWorldPosition = Position;
            //HingeConstraint hc = new HingeConstraint(body, Matrix.CreateTranslation(Position-Vector3.UnitX * 170.0f));
            //GameplayScreen.dynamicsWorld.AddConstraint(hc);
        }

        public void ApplyForce(Vector3 Force, Vector3 ForcePosition)
        {
            body.ApplyForce(Force, ForcePosition);
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            base.Update(gameTime);
        }

        public void Despawn()
        {
            Game.Components.Remove(this);
        }
    }
}