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
    public class DynamicWorld : Microsoft.Xna.Framework.GameComponent
    {
        public static BulletSharp.DiscreteDynamicsWorld dynamicsWorld;

        public BroadphaseInterface broadphase;
        public DefaultCollisionConfiguration collisionConfiguration;
        public CollisionDispatcher dispatcher;
        public ConstraintSolver csolver;
        public AlignedObjectArray<CollisionShape> collisionShapes;

        public DynamicWorld(Game game)
            : base(game)
        {
            SetupPhysicsWorld();
        }


        public void SetupPhysicsWorld()
        {
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            broadphase = new DbvtBroadphase();
            csolver = new SequentialImpulseConstraintSolver();
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, broadphase, csolver, collisionConfiguration);
            dynamicsWorld.Gravity = new Vector3(0, -100, 0);
            dynamicsWorld.DispatchInfo.UseContinuous = true; //use continuous collision detection
        }

        public bool areColliding(Actor actor1, Actor actor2)
        {
            int numManifolds = dispatcher.NumManifolds;
            bool match = false;
            for (int i = 0; i < numManifolds; i++)
            {
                PersistentManifold contactManifold = dispatcher.GetManifoldByIndexInternal(i);
                CollisionObject objecta = (CollisionObject)contactManifold.Body0;
                CollisionObject objectb = (CollisionObject)contactManifold.Body1;
                if (objecta.Equals(actor1.body) && objectb.Equals(actor2.body)
                    || objectb.Equals(actor2.body) && objecta.Equals(actor1.body))
                    match = true;
                if (match && contactManifold.NumContacts > 0)
                    return true;
            }
            return false;
            
        }
        public bool castRay(Vector3 start, Vector3 end)
        {
            CollisionWorld.ClosestRayResultCallback RayCallback = new CollisionWorld.ClosestRayResultCallback(start, end);
            dynamicsWorld.RayTest(start, end, RayCallback);
            if (RayCallback.HasHit)
            {
                end = RayCallback.HitPointWorld;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void StepSimulation(GameTime gameTime)
        {
            dynamicsWorld.StepSimulation((float)gameTime.ElapsedGameTime.TotalSeconds, 0);
        }
    }
}
