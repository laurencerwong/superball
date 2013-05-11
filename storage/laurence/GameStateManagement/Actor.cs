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
        // Variables
        private Model ActorModel;
        public Matrix transformMatrix;
        public string meshName;
        Matrix[] ActorBones;
        public Utils.Timer timer;
        public float fMass;
        public short[] indexData;
        public Vector3[] vertexData;
        public RigidBody body;
        public bool useModelForCollision = false;
        public bool simulateDynamics = true;

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
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float f_TimeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer.Update(gameTime);
            base.Update(gameTime);
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
            b = new BoxShape(halfExtents.X, halfExtents.Y, halfExtents.Z);
            addBodyToDynamicsWorld(restitution, friction, b);
        }

        public void addBodyToDynamicsWorld(float restitution, float friction, CollisionShape c)
        {
            if (simulateDynamics)
            {
                //Higher restitution = higher bounce
                if (restitution == -1)
                    restitution = 0.7f;
                if (friction == -1)
                    friction = 1.5f;
                Vector3 inertia;
                c.CalculateLocalInertia(fMass, out inertia);
                DefaultMotionState motionState = new DefaultMotionState(transformMatrix);
                RigidBodyConstructionInfo rigidBodyInfo = new RigidBodyConstructionInfo(fMass, motionState, c, inertia);
                rigidBodyInfo.Restitution = restitution;
                rigidBodyInfo.Friction = friction;
                rigidBodyInfo.RollingFriction = friction;
                body = new RigidBody(rigidBodyInfo);
                DynamicWorld.dynamicsWorld.AddRigidBody(body);
            }
            else
            {
                CollisionObject co = new CollisionObject();
                co.CollisionShape = c;
                co.CollisionFlags = co.CollisionFlags | CollisionFlags.NoContactResponse;
                DynamicWorld.dynamicsWorld.AddCollisionObject(co);
            }
        }


        protected override void LoadContent()
        {
            ActorModel = Game.Content.Load<Model>(meshName);
            ActorBones = new Matrix[ActorModel.Bones.Count];
            TriangleMesh triMesh = new TriangleMesh();
            if (useModelForCollision)
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
                            for (int i = 0; i < part.PrimitiveCount; i++)
                            {
                                triMesh.AddTriangle(vertexData[indexData[i * 3]],
                                    vertexData[indexData[i * 3 + 1]],
                                    vertexData[indexData[i * 3 + 2]]);
                            }

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

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
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
                    effect.AmbientLightColor = GameplayScreen.AmbientColor;
                    effect.SpecularColor = GameplayScreen.SpecularColor;
                    effect.SpecularPower = GameplayScreen.SpecularIntensity;
                    effect.DirectionalLight0.DiffuseColor = GameplayScreen.DirectionalColor;
                    effect.DirectionalLight0.Direction = Vector3.UnitY;
                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }

        public Vector3 GetWorldFacing()
        {
            return body.WorldTransform.Forward;
        }

        public Vector3 GetWorldPosition()
        {
            return body.WorldTransform.Translation;
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


        private void ApplyRotation()
        {
            if (body != null)
            {
                Matrix transform = body.WorldTransform;
                Matrix Rotation = Matrix.CreateFromQuaternion(m_qRotation);
                body.WorldTransform = Rotation * transform;
                m_qRotation = Quaternion.Identity;
            }

        }

    }
}
