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
        public static LinkedList<String> collisionShapeMeshNames;

        public BroadphaseInterface broadphase;
        public DefaultCollisionConfiguration collisionConfiguration;
        public CollisionDispatcher dispatcher;
        public ConstraintSolver csolver;
        public AlignedObjectArray<CollisionShape> collisionShapes;

        public bool enabled;

        public DynamicWorld(Game game)
            : base(game)
        {
            SetupPhysicsWorld();
            collisionShapeMeshNames = new LinkedList<String>();
        }

        public void Dispose(BulletSharp.IDisposable idisposable)
        {
            if(idisposable != null)
                idisposable.Dispose();
        }


        public void DestroyDynamicWorld()
        {
            Dispose(dynamicsWorld);
            Dispose(dynamicsWorld.CollisionWorld);
            Dispose(csolver);
            Dispose(broadphase);
            Dispose(dispatcher);
            enabled = false;
        }


        public void SetupPhysicsWorld()
        {
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            broadphase = new DbvtBroadphase();
            csolver = new SequentialImpulseConstraintSolver();
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, broadphase, csolver, collisionConfiguration);
            dynamicsWorld.Gravity = new Vector3(0, -300, 0);
            dynamicsWorld.DispatchInfo.UseContinuous = true; //use continuous collision detection
        }


        public bool hasCollision(Actor actor1)
        {
            bool returnValue = false;
            int numManifolds = dispatcher.NumManifolds;
            for (int i = 0; i < numManifolds; i++)
            {
                PersistentManifold contactManifold = dispatcher.GetManifoldByIndexInternal(i);
                CollisionObject objecta = (CollisionObject)contactManifold.Body0;
                CollisionObject objectb = (CollisionObject)contactManifold.Body1;
                returnValue = (objecta.Equals(actor1.body) || objectb.Equals(actor1.body));
            }
            return returnValue;

        }
        public bool areColliding(RigidBody actor1, RigidBody actor2)
        {
            bool returnValue = false;
            int numManifolds = dispatcher.NumManifolds;
            for (int i = 0; i < numManifolds; i++)
            {
                PersistentManifold contactManifold = dispatcher.GetManifoldByIndexInternal(i);
                CollisionObject objecta = (CollisionObject)contactManifold.Body0;
                CollisionObject objectb = (CollisionObject)contactManifold.Body1;
                RigidBody bodya = RigidBody.Upcast(objecta);
                RigidBody bodyb = RigidBody.Upcast(objectb);
                returnValue = (bodya == (actor1) && bodyb == (actor2)
                    || bodyb == (actor1) && bodya == (actor2) && contactManifold.NumContacts > 0);
            }
            return returnValue;
            
        }
        /// <summary>
        /// Returns point of contact of actor1 on actor 2
        /// </summary>
        public bool collisionPoint(RigidBody actor1, RigidBody actor2, ref Vector3 point)
        {
            int numManifolds = dispatcher.NumManifolds;
            ManifoldPoint pt;
            for (int i = 0; i < numManifolds; i++)
            {
                PersistentManifold contactManifold = dispatcher.GetManifoldByIndexInternal(i);
                CollisionObject objecta = (CollisionObject)contactManifold.Body0;
                CollisionObject objectb = (CollisionObject)contactManifold.Body1;
                RigidBody bodya = RigidBody.Upcast(objecta);
                RigidBody bodyb = RigidBody.Upcast(objectb);
                if (bodya == (actor1) && bodyb == (actor2)
                    && contactManifold.NumContacts > 0)
                {
                    pt = contactManifold.GetContactPoint(0);
                    if (pt.Distance < 0f)
                    {
                        point = pt.PositionWorldOnB;
                        return true;
                    }
                }
                if (bodyb == (actor1) && bodya == (actor2)
    && contactManifold.NumContacts > 0)
                {
                    pt  = contactManifold.GetContactPoint(0);
                    if (pt.Distance < 0f)
                    {
                        point = pt.PositionWorldOnA;
                        return true;
                    }
                }
            }
            return false;

        }
        /// <summary>
        /// Cast ray for actors with the specified filter mask
        /// </summary>
        public void castRay(Vector3 start, Vector3 end, CollisionFilterGroups filter, ref Actor selected)
        {
            CollisionWorld.ClosestRayResultCallback RayCallback = new CollisionWorld.ClosestRayResultCallback(start, end);
            RayCallback.CollisionFilterGroup = CollisionFilterGroups.DefaultFilter;
            RayCallback.CollisionFilterMask = filter;
            dynamicsWorld.RayTest(start, end, RayCallback);
            if (RayCallback.HasHit)
            {
                CollisionObject co = RayCallback.CollisionObject;
                RigidBody body = RigidBody.Upcast(co);
                foreach (GameComponent gc in Game.Components)
                {
                    if (gc is Actor)
                    {
                        Actor a = (Actor)gc;
                        if (a.body.Equals(body) && a.isUserMovable)
                        {
                            a.selected = true;
                            selected = a;
                            return;
                        }
                    }
                }
            }
            if(selected != null)
                selected.selected = false;
            selected = null;
        }
        /// <summary>
        /// Cast ray for all actors that are not the character or a ghost body
        /// </summary>
        public void castRay(Vector3 start, Vector3 end, ref Actor selected)
        {
            CollisionWorld.ClosestRayResultCallback RayCallback = new CollisionWorld.ClosestRayResultCallback(start, end);
            RayCallback.CollisionFilterGroup = CollisionFilterGroups.DefaultFilter;
            RayCallback.CollisionFilterMask = CollisionFilterGroups.AllFilter ^ CollisionFilterGroups.CharacterFilter ^ CollisionFilterGroups.SensorTrigger;
            dynamicsWorld.RayTest(start, end, RayCallback);
            if (RayCallback.HasHit)
            {
                CollisionObject co = RayCallback.CollisionObject;
                RigidBody body = RigidBody.Upcast(co);
                foreach (GameComponent gc in Game.Components)
                {
                    if (gc is Actor)
                    {
                        Actor a = (Actor)gc;
                        if (a.body.Equals(body) && a.isUserMovable)
                        {
                            if (!a.Equals(selected))
                            {
                                a.select();
                                selected = a;
                            }
                            else
                            {
                                selected.deselect();
                                selected = null;
                            }
                            return;
                        }
                    }
                }
            }
            else if(selected != null)
            {
                selected.deselect();
                selected = null;
            }
            
        }
        public LinkedList<Actor> castRay(Vector3 start, Vector3 end)
        {
            Vector3 endMinusStart = end - start;
            LinkedList<Actor> blockingCamera = new LinkedList<Actor>();
            CollisionWorld.AllHitsRayResultCallback RayCallback = new CollisionWorld.AllHitsRayResultCallback(start, end);
            dynamicsWorld.RayTest(start, end, RayCallback);
            if (RayCallback.HasHit)
            {
                for (int i = 0; i < RayCallback.CollisionObjects.Count; i++)
                {
                    CollisionObject co = RayCallback.CollisionObjects.ElementAt(i);
                    RigidBody body = RigidBody.Upcast(co);
                    foreach (GameComponent gc in Game.Components)
                    {
                        if (gc is Actor)
                        {

                            if (((Actor)gc).body.Equals(body))
                            {

                                float distanceFromPlayer = endMinusStart.LengthSquared();
                                float distanceFromActor = (((Actor)gc).body.CenterOfMassPosition - start).LengthSquared();
                                if (distanceFromActor
                                    < distanceFromPlayer)
                                {
                                    blockingCamera.AddFirst((Actor)gc);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return blockingCamera;
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
