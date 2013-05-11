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
    public class GravityBall : Actor
    {
        public SphereShape sphereOfInfluence; //Trigger
        public SphereShape boundingSphere; //RayCasting and collides with other objects but not player
        public RigidBody triggerBody;
        public float triggerRadius
        {
            get
            {
                return _triggerRadius;
            }
            set
            {
                _triggerRadius = value;
                updateTriggerBody();
            }
        }

        private float _triggerRadius;
        public float Power = 1000.0f;

        public GravityBall(Game game)
            : base(game)
        {
            meshName = "gravityball";
            boundingType = BoundingType.BOX;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            fMass = 0.0f;
            base.Initialize();
            m_fScale = 1.0f;
            restitution = 0.0f;
            friction = 100.0f;
            radius = 100.0f;
            triggerRadius = radius;
            simulateDynamics = false;
            CollisionGroup = CollisionFilterGroups.SensorTrigger;
            CollidesWith = CollisionFilterGroups.CharacterFilter;
            generateBoundingShape(ref sphereOfInfluence);
            triggerBody = body;
            radius = -1;
            simulateDynamics = true;
            CollisionGroup = CollisionFilterGroups.DefaultFilter;
            CollidesWith = CollisionFilterGroups.DefaultFilter | CollisionFilterGroups.CharacterFilter;
            generateBoundingShape(ref boundingSphere);
            body.AngularFactor = Vector3.Zero;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        public void updateTriggerBody()
        {
            if (body == null || triggerBody == null)
                return;
            Matrix oldWorldTransform = body.WorldTransform;
            DynamicWorld.dynamicsWorld.RemoveRigidBody(body);
            DynamicWorld.dynamicsWorld.RemoveRigidBody(triggerBody);
            simulateDynamics = false;
            radius = triggerRadius;
            CollisionGroup = CollisionFilterGroups.SensorTrigger;
            CollidesWith = CollisionFilterGroups.CharacterFilter;
            generateBoundingShape(ref sphereOfInfluence);
            triggerBody = body;
            triggerBody.WorldTransform = oldWorldTransform;
            radius = -1;
            simulateDynamics = true;
            CollisionGroup = CollisionFilterGroups.DefaultFilter;
            CollidesWith = CollisionFilterGroups.DefaultFilter | CollisionFilterGroups.CharacterFilter;
            generateBoundingShape(ref boundingSphere);
            body.AngularFactor = Vector3.Zero;
            body.WorldTransform = oldWorldTransform;
        }

        public void ApplyForce(Vector3 Force, Vector3 ForcePosition)
        {
            body.ApplyForce(Force, ForcePosition);
        }

        public override void Rotate(Vector3 rotationAxis, float rotationAngle)
        {
            triggerRadius = triggerRadius + (rotationAngle > 0 ? 10.0f : ((triggerRadius - 10.0 > 0) ? -10.0f : 0));
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
