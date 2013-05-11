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
    public class PlayerSpawn : Actor
    {
        public SphereShape sphereOfInfluence; //Trigger
        public SphereShape boundingSphere; //RayCasting and collides with other objects but not player
        public float triggerRadius;
        public float Power = 1000.0f;

        public PlayerSpawn(Game game)
            : base(game)
        {
            meshName = "ball";
            boundingType = BoundingType.BOX;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        /// 
        public override void Initialize()
        {
            fMass = 0.0f;
            base.Initialize();
            m_fScale = 1.0f;
            radius = -1;
            simulateDynamics = true;
            CollisionGroup = CollisionFilterGroups.DefaultFilter;
            CollidesWith = CollisionFilterGroups.DefaultFilter;
            generateBoundingShape(ref boundingSphere);
            body.AngularFactor = Vector3.Zero;
            if (!isUserMovable)
                Visible = false;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 


        public void ApplyForce(Vector3 Force, Vector3 ForcePosition)
        {
            body.ApplyForce(Force, ForcePosition);
        }

        public override void Rotate(Vector3 rotationAxis, float rotationAngle)
        {
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (GameplayScreen.cameraType == GameplayScreen.CameraTypes.LevelEditor)
            {
                this.Visible = true;
            }
            else
            {
                this.Visible = false;
            }
            if (body.WantsSleeping)
            {
                body.AngularFactor = Vector3.Zero;
                body.LinearFactor = Vector3.Zero;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        public override void Despawn()
        {
            base.Despawn();
        }
    }
}
