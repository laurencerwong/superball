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
    protected const float fTerminalVelocity = 500.0f;
    public Vector3 vForce;
    protected Vector3 vAcceleration;
    bool bPhysicsDriven;
    public BoundingSphere ModelBounds;
    public BoundingSphere WorldBounds;
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
            }
        }
    }
    private Vector3 _vWorldPosition;

    public Quaternion m_qRotation
    {
        get
        {
            return  _qRotation;
        }
        set
        {
            if(_qRotation != value)
            {
                _qRotation = value;
                ApplyMatrixTransforms();
            }
        }
    }
    private Quaternion _qRotation;

    public Vector3 m_vVelocity
    {
        get
        {
            return _vVelocity;
        }
        set
        {
            if (_vVelocity != value)
            {
                if (value.Length() > fTerminalVelocity)
                {
                    value.Normalize();
                    _vVelocity = value;
                    _vVelocity *= fTerminalVelocity;
                }
                else
                    _vVelocity = value;
            }
        }
    }
    private Vector3 _vVelocity;
	
	// Functions
        public Actor(Game game)
            : base(game)
        {
            timer = new Utils.Timer();
	        transformMatrix = Matrix.Identity;

            fMass = 1.0f;
            vForce = Vector3.Zero;
            vAcceleration = Vector3.Zero;
            bPhysicsDriven = true;
            m_vWorldPosition = Vector3.Zero;
            m_qRotation = Quaternion.Identity;
            m_vVelocity = Vector3.Zero;
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
            if (bPhysicsDriven)
            {
                m_vVelocity += vAcceleration * f_TimeDelta / 2.0f;
                m_vWorldPosition += m_vVelocity * f_TimeDelta;
                vAcceleration = vForce / fMass;
                m_vVelocity += vAcceleration * f_TimeDelta / 2.0f;
            }
            else
            {
                m_vWorldPosition += m_vVelocity * f_TimeDelta;
            }
                if (m_vWorldPosition.X > 512)
                {
                    Vector3 m_vTempWorldPosition = m_vWorldPosition;
                    m_vTempWorldPosition.X = -511;
                    m_vTempWorldPosition.X++;
                    m_vWorldPosition = m_vTempWorldPosition;
                }

                if (m_vWorldPosition.X < -512)
                {
                    Vector3 m_vTempWorldPosition = m_vWorldPosition;
                    m_vTempWorldPosition.X = 511;
                    m_vTempWorldPosition.X--;
                    m_vWorldPosition = m_vTempWorldPosition;
                }

                if (m_vWorldPosition.Y > 384 )
                {
                    Vector3 m_vTempWorldPosition = m_vWorldPosition;
                    m_vTempWorldPosition.Y = -383;
                    m_vTempWorldPosition.Y++;
                    m_vWorldPosition = m_vTempWorldPosition;
                }
                if (m_vWorldPosition.Y < -384)
                {
                    Vector3 m_vTempWorldPosition = m_vWorldPosition;
                    m_vTempWorldPosition.Y = 383;
                    m_vTempWorldPosition.Y--;
                    m_vWorldPosition = m_vTempWorldPosition;
                }
                vForce = Vector3.Zero;
            timer.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            ActorModel = Game.Content.Load<Model>(meshName);
            ActorBones = new Matrix[ActorModel.Bones.Count];
            foreach (ModelMesh mesh in ActorModel.Meshes)
            {
                ModelBounds = BoundingSphere.CreateMerged(ModelBounds, mesh.BoundingSphere);
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
                    effect.World = ActorBones[mesh.ParentBone.Index]*transformMatrix;
                    effect.View = GameplayScreen.CameraMatrix;
                    effect.Projection = GameplayScreen.ProjectionMatrix;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.AmbientLightColor = GameplayScreen.AmbientColor;
                    effect.SpecularColor = GameplayScreen.SpecularColor;
                    effect.SpecularPower = GameplayScreen.SpecularIntensity;
                    effect.DirectionalLight0.DiffuseColor = GameplayScreen.DirectionalColor;
                    effect.DirectionalLight0.Direction = GameplayScreen.DirectionalDirection;
                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }

        public Vector3 GetWorldFacing()
        {
            return transformMatrix.Forward;
        }

        public Vector3 GetWorldPosition()
        {
            return transformMatrix.Translation;
        }

        private void ApplyMatrixTransforms()
        {
            Matrix Translation, Scale, Rotation;
            WorldBounds.Center = m_vWorldPosition;
            WorldBounds.Radius = ModelBounds.Radius * m_fScale;
            Translation = Matrix.CreateTranslation(m_vWorldPosition);
            Scale = Matrix.CreateScale(m_fScale);
            Rotation = Matrix.CreateFromQuaternion(m_qRotation);

            transformMatrix = Scale * Rotation * Translation;
        }


    }
}
