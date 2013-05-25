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
    public class Spring : Actor
    {
        public SphereShape boundingSphere; //Trigger
        public BoxShape boundingBox; //RayCasting and collides with other objects but not player
        public RigidBody triggerBody;
        public float Power = 500.0f;

        public Spring(Game game)
            : base(game)
        {
            meshName = "spring";
            boundingType = BoundingType.BOX;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            fMass = 100.0f;
            base.Initialize();
            m_fScale = 1.0f;
            restitution = 0.0f;
            friction = 100.0f;
            radius = 6.0f;
            simulateDynamics = false;
            CollisionGroup = CollisionFilterGroups.SensorTrigger;
            CollidesWith = CollisionFilterGroups.AllFilter ^ CollisionFilterGroups.KinematicFilter;
            generateBoundingShape(ref boundingSphere);
            body.ActivationState = ActivationState.DisableDeactivation;
            triggerBody = body;
            simulateDynamics = true;
            CollisionGroup = CollisionFilterGroups.DefaultFilter;
            CollidesWith = CollisionFilterGroups.AllFilter ^ CollisionFilterGroups.CharacterFilter;
            generateBoundingShape(ref boundingBox);
            body.AngularFactor = Vector3.Zero;
            body.ActivationState = ActivationState.DisableDeactivation;
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

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (body.WantsSleeping)
            {
                body.AngularFactor = Vector3.Zero;
                body.LinearFactor = Vector3.Zero;
            }
            base.Update(gameTime);
            triggerBody.WorldTransform = body.WorldTransform;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            DrawDebugObject(gameTime, triggerBody);
        }
        public override void Despawn()
        {
            DynamicWorld.dynamicsWorld.RemoveRigidBody(triggerBody);
            base.Despawn();
        }
    }
}
