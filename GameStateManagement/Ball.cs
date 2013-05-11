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
    public class Ball : Actor
    {
        public Vector3 playerTranslation;
        public Vector3 forward;
        public Vector3 velocity;
        public SphereShape boundingSphere;
        LinkedList<Vector3> velocityMeasurements;
        public Vector3 LastPosition;
        public bool CanJump = true;
        public bool CanImpulse = true;
        public bool OnGround = true;
        public bool OnGroundLastFrame = true;

        public Ball(Game game)
            : base(game)
        {
            meshName = "ball";
            fMass = 5.0f;
            Visible = false;
            forward = Vector3.UnitX;
            velocityMeasurements = new LinkedList<Vector3>();
            velocityMeasurements.AddFirst(Vector3.UnitX);
            boundingType = BoundingType.SPHERE;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            rotationalfriction = 1.0f;
            restitution = 1.0f;
            friction = 1.0f;
            CollisionGroup = CollisionFilterGroups.CharacterFilter;
            CollidesWith = CollisionFilterGroups.SensorTrigger | CollisionFilterGroups.DefaultFilter | CollisionFilterGroups.KinematicFilter;
            generateBoundingShape(ref boundingSphere);
            Visible = true;
            m_fScale = 1.0f;
            body.ActivationState = ActivationState.DisableDeactivation;
            body.CcdMotionThreshold = radius;
            body.CcdSweptSphereRadius = radius;
        }

        public void CheckOnGround(bool hasCollision)
        {
            OnGroundLastFrame = OnGround;
            if (hasCollision)
            {
                OnGround = true;
                ResetJump();
            }
            else
            {
                if ((LastPosition - playerTranslation).LengthSquared() < 0.15f && OnGroundLastFrame == true)
                    OnGround = true;
                else
                {
                    OnGround = false;
                }
            }
            if (OnGroundLastFrame == false && OnGround == true)
            {
                GameplayScreen.soundbank.PlayCue("Marble_Impact");
            }
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

        public void ApplyImpulse(Vector3 impulse, Vector3 relativePosition, float deltaTime)
        {
            body.ApplyImpulse(impulse * (OnGround ? 285.0f : 200.0f) * deltaTime, impulse);
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            LastPosition = playerTranslation;
            playerTranslation = body.WorldTransform.Translation;
            timer.Update(gameTime);
            base.Update(gameTime);
        }

        public void ResetJump()
        {
            CanJump = true;
        }

        public bool Jump(float deltaTime)
        {
            if (CanJump)
            {
                body.ApplyImpulse(Vector3.UnitY * 50000.0f * deltaTime, Vector3.UnitY);
                CanJump = false;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
