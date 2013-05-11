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
        public BvhTriangleMeshShape triMesh;
        public ConvexHullShape conHullShape;
        public BoxShape boundingBox;
        public HingeConstraint hc;
        public float rotationalSpeed = -5.0f;
        public Hammer(Game game)
            : base(game)
        {
            meshName = "wholehammer";
            boundingType = BoundingType.TRIMESH;
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
            restitution = 1.0f;
            CollisionGroup = CollisionFilterGroups.KinematicFilter;
            generateBoundingShape(ref conHullShape);
            body.ActivationState = ActivationState.DisableDeactivation;
            
        }

        public override void setCoordinates(Vector3 Position)
        {
            base.setCoordinates(Position);
        }

        public override void Rotate(Vector3 rotationAxis, float rotationAngle)
        {
            base.Rotate(Vector3.UnitZ, rotationAngle);
             
        }

        public override void deselect()
        {
            base.deselect();
            SetHinge();
            body.SetMassProps(1000.0f, body.CollisionShape.CalculateLocalInertia(1000.0f));
            //hc.EnableAngularMotor(true, rotationalSpeed, 1000000.0f);
        }

        public override void select()
        {
            base.select();
            body.SetMassProps(0.0f, Vector3.One);
            if (hc != null)
            {
                DynamicWorld.dynamicsWorld.RemoveConstraint(hc);
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        public void SetHinge()
        {
            Vector3 hingePosition = new Vector3(0, 51.8461f, 0);
            if (hc != null)
            {
                DynamicWorld.dynamicsWorld.RemoveConstraint(hc);
                hc = null;
            }
            hc = new HingeConstraint(body, hingePosition, Vector3.UnitZ, true);
            DynamicWorld.dynamicsWorld.AddConstraint(hc);
        }

        public void SetMass(float mass)
        {
            body.SetMassProps(mass, body.CollisionShape.CalculateLocalInertia(mass));
        }

        public void ApplyForce(Vector3 Force, Vector3 ForcePosition)
        {
            body.ApplyForce(Force, ForcePosition);
        }

        

        public void speedUp()
        {
            body.ApplyImpulse(body.AngularVelocity * 100.0f, body.CenterOfMassPosition);
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (selected)
            {
                body.LinearFactor = Vector3.Zero;
            }
            else
            {
                body.LinearFactor = Vector3.One;
            }
            base.Update(gameTime);
        }

        public override void Despawn()
        {
            //DynamicWorld.dynamicsWorld.RemoveConstraint(hc);
            base.Despawn();
        }
    }
}
