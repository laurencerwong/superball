using System;
using System.IO;
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
    public class Actor : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public static ushort DrawOrderNum = 0;
        // Variables
        private Model ActorModel;
        public Matrix transformMatrix;
        public CollisionFilterGroups CollisionGroup = CollisionFilterGroups.DefaultFilter;
        public CollisionFilterGroups CollidesWith = CollisionFilterGroups.AllFilter ^ CollisionFilterGroups.SensorTrigger ^ CollisionFilterGroups.DebrisFilter ^ CollisionFilterGroups.KinematicFilter;// ^ CollisionFilterGroups.DefaultFilter;
        public string meshName;
        Matrix[] ActorBones;
        public Utils.Timer timer;
        public float fMass;
        public ushort MyDrawOrderNum;
        public short[] indexData;
        public Vector3[] vertexData;
        public RigidBody body;
        public TriangleIndexVertexArray indexedVertexArray;
        public bool useModelForCollision = false;
        private bool userMovable = false;
        public bool isUserMovable
        {
            get
            {
                return userMovable;
            }
            set
            {
                userMovable = value;
            }
        }
        public enum BoundingType
        {
            SPHERE, BOX, TRIMESH
        }
        public Vector3 RigidBodyOffset = Vector3.Zero;
        public BoundingType boundingType;
        public bool simulateDynamics = true;
        public bool selected = false;

        public float alpha = 1.0f;
        public bool drawSecondPass = false;

        public const int GRIDSIZE = 5;

        public float radius = -1;
        public float restitution = -1;
        public float friction = -1;
        public float rotationalfriction = -1;

        public float m_fScale
        {
            get
            {
                return _fScale;
            }
            set
            {
                if (_fScale != value)
                {
                    _fScale = value;
                    ApplyMatrixTransforms();
                }
            }
        }
        private float _fScale = 1.0f;

        public Vector3 m_vWorldPosition
        {
            get
            {
                return _vWorldPosition;
            }
            set
            {
                if (_vWorldPosition != value)
                {
                    _vWorldPosition = value;
                    ApplyMatrixTransforms();
                    _vWorldPosition = Vector3.Zero;
                }
            }
        }
        private Vector3 _vWorldPosition;

        public Quaternion m_qRotation
        {
            get
            {
                return _qRotation;
            }
            set
            {
                if (_qRotation != value)
                {
                    _qRotation = value;
                    ApplyMatrixTransforms();
                }
            }
        }
        private Quaternion _qRotation;

        // Functions
        public Actor(Game game)
            : base(game)
        {
            timer = new Utils.Timer();
            transformMatrix = Matrix.Identity;
            fMass = 1.0f;
            m_vWorldPosition = Vector3.Zero;
            m_qRotation = Quaternion.Identity;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
            DrawOrder = 1;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float f_TimeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer.Update(gameTime);
            DrawOrder = (alpha < 1.0f) ? 3 : 2;
            base.Update(gameTime);
        }

        public virtual void Rotate(Vector3 rotationAxis, float rotationAngle)
        {
            m_qRotation = Quaternion.CreateFromAxisAngle(rotationAxis, rotationAngle) * m_qRotation;
        }

        public void generateBoundingShape(ref SphereShape s)
        {
            if (radius == -1)
            {
                foreach (ModelMesh mesh in ActorModel.Meshes)
                {
                    radius = mesh.BoundingSphere.Radius;
                }
            }
            if (checkForRedundantCollisionObject(ref s, radius))
                return;
            s = new SphereShape(radius);
            addBodyToDynamicsWorld(restitution, friction, s);
        }

        public void generateBoundingShape(ref BoxShape b)
        {
            Vector3 halfExtents = new Vector3(0, 0, 0);
            foreach (Vector3 v in vertexData)
            {
                if (halfExtents.X < v.X)
                    halfExtents.X = v.X;
                if (halfExtents.Y < v.Y)
                    halfExtents.Y = v.Y;
                if (halfExtents.Z < v.Z)
                    halfExtents.Z = v.Z;
            }
            if (checkForRedundantCollisionObject(ref b))
                return;
            b = new BoxShape(halfExtents.X, halfExtents.Y, halfExtents.Z);
            addBodyToDynamicsWorld(restitution, friction, b);
        }

        public void generateBoundingShape(ref BvhTriangleMeshShape tms)
        {
            tms = new BvhTriangleMeshShape(indexedVertexArray, true, new Vector3(-10000, -10000, -10000),
                new Vector3(10000, 10000, 10000));
            addBodyToDynamicsWorld(restitution, friction, tms);
        }

        public void generateBoundingShape(ref ConvexTriangleMeshShape tms)
        {
            tms = new ConvexTriangleMeshShape(indexedVertexArray, true);
            addBodyToDynamicsWorld(restitution, friction, tms);
        }

        public void generateBoundingShape(ref ConvexHullShape tms)
        {

            tms = new ConvexHullShape(vertexData);
            addBodyToDynamicsWorld(restitution, friction, tms);
        }

        public bool checkForRedundantCollisionObject(ref BoxShape b)
        {
            RigidBody rb;
            foreach (CollisionObject co in DynamicWorld.dynamicsWorld.CollisionObjectArray)
            {
                rb = RigidBody.Upcast(co);
                foreach (GameComponent gc in Game.Components)
                {
                    if (gc is Actor)
                    {
                        Actor a = (Actor)gc;
                        if (a.meshName.Equals(this.meshName) && !a.Equals(this) && !(a is InventoryObject))
                        {
                            b = (BoxShape)a.body.CollisionShape;
                            addBodyToDynamicsWorld(restitution, friction, b);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool checkForRedundantCollisionObject(ref SphereShape b, float radius)
        {
            RigidBody rb;
            foreach (CollisionObject co in DynamicWorld.dynamicsWorld.CollisionObjectArray)
            {
                rb = RigidBody.Upcast(co);
                if (co.CollisionShape is SphereShape)
                {
                    SphereShape co_b = (SphereShape)co.CollisionShape;
                    if (co_b.Radius == radius)
                    {
                        b = co_b;
                        addBodyToDynamicsWorld(restitution, friction, b);
                        return true;
                    }
                }
            }
            return false;
        }


        public void addBodyToDynamicsWorld(float restitution, float friction, CollisionShape c)
        {
            /*
             * To disable contact response and use rigid body as trigger, use body.CollisionFlags |= CollisionFlags.NoContactRe
            */

            //Higher restitution = higher bounce
            if (restitution == -1)
                restitution = 0.7f;
            if (friction == -1)
                friction = 1.5f;
            if (rotationalfriction == -1)
                rotationalfriction = friction;
            Vector3 inertia;
            c.CalculateLocalInertia(fMass, out inertia);
            DefaultMotionState motionState = new DefaultMotionState(Matrix.Identity);
            RigidBodyConstructionInfo rigidBodyInfo = new RigidBodyConstructionInfo(fMass, motionState, c, inertia);
            rigidBodyInfo.Restitution = restitution;
            rigidBodyInfo.Friction = friction;
            rigidBodyInfo.RollingFriction = rotationalfriction;
            body = new RigidBody(rigidBodyInfo);
            if (simulateDynamics)
            {
                DynamicWorld.dynamicsWorld.AddRigidBody(body, CollisionGroup, CollidesWith);
            }
            else
            {
                body.CollisionFlags |= CollisionFlags.NoContactResponse;
                DynamicWorld.dynamicsWorld.AddRigidBody(body, CollisionGroup, CollidesWith);
            }
        }


        protected override void LoadContent()
        {
            ActorModel = Game.Content.Load<Model>(meshName);
            ActorBones = new Matrix[ActorModel.Bones.Count];
            if (boundingType != BoundingType.SPHERE)
            {

                foreach (ModelMesh mesh in ActorModel.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        int vertStride = part.VertexBuffer.VertexDeclaration.VertexStride;
                        int vertBufferSize = part.NumVertices;
                        VertexElement[] vertexElements = part.VertexBuffer.VertexDeclaration.GetVertexElements();
                        VertexElement vertexPosition = new VertexElement();
                        foreach (VertexElement vert in vertexElements)
                        {
                            if (vert.VertexElementUsage == VertexElementUsage.Position &&
                                vert.VertexElementFormat == VertexElementFormat.Vector3)
                            {
                                vertexPosition = vert;
                                break;
                            }

                        }
                        vertexData = new Vector3[vertBufferSize];
                        part.VertexBuffer.GetData<Vector3>(
                            part.VertexOffset * vertStride + vertexPosition.Offset,
                            vertexData,
                            0,
                            vertBufferSize,
                            vertStride);
                        IndexElementSize indexSize = part.IndexBuffer.IndexElementSize;
                        if (indexSize == IndexElementSize.SixteenBits)
                        {
                            indexData = new short[part.PrimitiveCount * 3];
                            part.IndexBuffer.GetData<short>(part.StartIndex * 2,
                                indexData,
                                0,
                                part.PrimitiveCount * 3);
                            int[] intIndexData = new int[part.PrimitiveCount * 3];
                            for (int i = 0; i < part.PrimitiveCount * 3; i++)
                            {
                                intIndexData[i] = (int)indexData[i];
                            }
                            indexData = null;
                            indexedVertexArray = new TriangleIndexVertexArray(intIndexData, vertexData);

                        }
                    }

                }


            }
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected void DrawDebugObject(GameTime gameTime, RigidBody body)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            ActorModel.CopyAbsoluteBoneTransformsTo(ActorBones);
            foreach (ModelMesh mesh in ActorModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = ActorBones[mesh.ParentBone.Index] * body.WorldTransform;
                    effect.View = GameplayScreen.CameraMatrix;
                    effect.Projection = GameplayScreen.ProjectionMatrix;
                    effect.PreferPerPixelLighting = true;
                    effect.AmbientLightColor = selected ? Color.White.ToVector3() : GameplayScreen.AmbientColor;
                    effect.SpecularColor = selected ? Color.White.ToVector3() : GameplayScreen.SpecularColor;
                    effect.SpecularPower = GameplayScreen.SpecularIntensity;
                    effect.DirectionalLight0.DiffuseColor = selected ? Color.White.ToVector3() : GameplayScreen.DirectionalColor;
                    effect.DirectionalLight0.Direction = GameplayScreen.DirectionalDirection;
                    effect.Alpha = alpha;
                    DebugDrawer.basiceffect.World = effect.World;
                    DebugDrawer.basiceffect.View = effect.View;
                    DebugDrawer.basiceffect.Projection = effect.Projection;
                }
                mesh.Draw();
                alpha = 1.0f;
                //DynamicWorld.dynamicsWorld.DebugDrawObject(DebugDrawer.basiceffect.World, body.CollisionShape, Color.White);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //if (alpha < 1.0f && !drawSecondPass)
            base.Draw(gameTime);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            ActorModel.CopyAbsoluteBoneTransformsTo(ActorBones);
            foreach (ModelMesh mesh in ActorModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = ActorBones[mesh.ParentBone.Index] * body.WorldTransform;
                    effect.View = GameplayScreen.CameraMatrix;
                    effect.Projection = GameplayScreen.ProjectionMatrix;
                    effect.PreferPerPixelLighting = true;
                    effect.AmbientLightColor = selected ? Color.White.ToVector3() : GameplayScreen.AmbientColor;
                    effect.SpecularColor = selected ? Color.White.ToVector3() : GameplayScreen.SpecularColor;
                    effect.SpecularPower = GameplayScreen.SpecularIntensity;
                    effect.DirectionalLight0.DiffuseColor = selected ? Color.White.ToVector3() : GameplayScreen.DirectionalColor;
                    effect.DirectionalLight0.Direction = GameplayScreen.DirectionalDirection;
                    effect.Alpha = alpha;
                    DebugDrawer.basiceffect.World = effect.World;
                    DebugDrawer.basiceffect.View = effect.View;
                    DebugDrawer.basiceffect.Projection = effect.Projection;
                }
                mesh.Draw();
                //DynamicWorld.dynamicsWorld.DebugDrawObject(DebugDrawer.basiceffect.World, body.CollisionShape, Color.White);
            }
           
            alpha = 1.0f;
        }

        public virtual void TranslateBody(Vector3 position, bool round)
        {
            Quaternion rotation;
            Vector3 newPosition;
            Vector3 scale;
            body.WorldTransform.Decompose(out scale, out rotation, out newPosition);
            newPosition = position;
            if (round)
            {
                newPosition.X = (int)Math.Round((double)newPosition.X / (double)GRIDSIZE);
                newPosition.Y = (int)Math.Round((double)newPosition.Y / (double)GRIDSIZE);
                newPosition.X *= GRIDSIZE;
                newPosition.Y *= GRIDSIZE;
            }
            body.WorldTransform = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(m_qRotation) * Matrix.CreateTranslation(newPosition);
        }

        public virtual void select()
        {
            selected = true;
        }

        public virtual void deselect()
        {
            selected = false;
            //Vector3 coordinates = body.WorldTransform.Translation;
            //coordinates.Z = 0;
            //setCoordinates(coordinates);
        }

        public Vector3 GetWorldFacing()
        {
            return body.WorldTransform.Forward;
        }

        public Vector3 GetWorldPosition()
        {
            return body.WorldTransform.Translation;
        }

        public Vector3 GetWorldUp()
        {
            return body.WorldTransform.Up;
        }

        public virtual void Despawn()
        {
            DynamicWorld.dynamicsWorld.RemoveRigidBody(body);
            Game.Components.Remove(this);
        }

        public virtual void setCoordinates(Vector3 Position)
        {
            //m_vWorldPosition = Position;
            if (body != null)
            {
                Matrix Translation, Scale, Rotation;
                //WorldBounds.Center = m_vWorldPosition;
                //WorldBounds.Radius = ModelBounds.Radius * m_fScale;
                Translation = Matrix.CreateTranslation(Position);
                // WorldBounds.Center = Translation.Translation;
                Scale = Matrix.CreateScale(m_fScale);
                Rotation = Matrix.CreateFromQuaternion(m_qRotation);
                transformMatrix = Scale * Rotation * Translation;
                body.WorldTransform = Scale * Rotation * Translation;
            }
        }

        private void ApplyMatrixTransforms()
        {
            if (body != null)
            {
                Matrix Translation, Scale, Rotation;
                //WorldBounds.Center = m_vWorldPosition;
                //WorldBounds.Radius = ModelBounds.Radius * m_fScale;
                Translation = Matrix.CreateTranslation(body.WorldTransform.Translation + m_vWorldPosition);
                // WorldBounds.Center = Translation.Translation;
                Scale = Matrix.CreateScale(m_fScale);
                Rotation = Matrix.CreateFromQuaternion(m_qRotation);
                transformMatrix = Scale * Rotation * Translation;
                body.WorldTransform = Scale * Rotation * Translation;
            }

        }

    }
}
