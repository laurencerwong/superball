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
    public class Skybox : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public String meshName;
        private Model ActorModel;
        Matrix[] ActorBones;
        public Matrix transformMatrix;
        public Skybox(Game game)
            : base(game)
        {
            meshName = "skybox";

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            transformMatrix = Matrix.CreateFromQuaternion(Quaternion.Identity)*Matrix.CreateTranslation(Vector3.Zero);
            base.Initialize();
            DrawOrder = 1;
        }

        protected override void LoadContent()
        {
            ActorModel = Game.Content.Load<Model>(meshName);
            ActorBones = new Matrix[ActorModel.Bones.Count];
            base.LoadContent();
        }


        public void updatePosition(Vector3 position)
        {
            transformMatrix = Matrix.CreateTranslation(position);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            ActorModel.CopyAbsoluteBoneTransformsTo(ActorBones);
            foreach (ModelMesh mesh in ActorModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = ActorBones[mesh.ParentBone.Index] * transformMatrix;
                    effect.View = GameplayScreen.CameraMatrix;
                    effect.Projection = GameplayScreen.ProjectionMatrix;
                }
                mesh.Draw();
            }

            
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}
