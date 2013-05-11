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
    /// 
    /// 4 Steps to Bloom:
    /// 
    /// 1. Copy image into buffer while only preserving the highlights (via BLOOM_THRESHOLD)
    /// 
    /// 2. Copy highlights into another buffer while applying horizontal blur
    /// 
    /// 3. Copy highlights into first buffer while applying vertical blur
    /// 
    /// 4. draw last buffer and image back into main backbuffer with combination shader
    /// </summary>
    public class Bloom : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const float BLOOM_THRESHOLD = 0.5f;
        const int BLOOM_SAMPLES = 9;

        SpriteBatch spriteBatch;

        Effect bloomEffectStep1;
        Effect bloomEffectStep2_3;
        Effect bloomEffectStep4;

        RenderTarget2D tempSceneTarget;
        RenderTarget2D tempBloomTarget;
        RenderTarget2D finalCompositeTarget;

        public Bloom(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            bloomEffectStep1 = Game.Content.Load<Effect>("Shaders/BloomExtractHighlights");
            //bloomEffectStep2_3 = Game.Content.Load<Effect>("Shaders/BloomBlur");
            //bloomEffectStep4 = Game.Content.Load<Effect>("Shaders/BloomRecombine");


            PresentationParameters graphParams = GraphicsDevice.PresentationParameters;

            finalCompositeTarget = new RenderTarget2D(GraphicsDevice, GameStateManagementGame.PreferredBackBufferWidth,
                GameStateManagementGame.PreferredBackBufferHeight, false, graphParams.BackBufferFormat, 
                graphParams.DepthStencilFormat, graphParams.MultiSampleCount, RenderTargetUsage.DiscardContents);

            tempBloomTarget = new RenderTarget2D(GraphicsDevice, GameStateManagementGame.PreferredBackBufferWidth / 2,
                GameStateManagementGame.PreferredBackBufferHeight / 2, false, graphParams.BackBufferFormat, DepthFormat.None);

            tempSceneTarget = new RenderTarget2D(GraphicsDevice, GameStateManagementGame.PreferredBackBufferWidth / 2,
                GameStateManagementGame.PreferredBackBufferHeight / 2, false, graphParams.BackBufferFormat, DepthFormat.None);

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            finalCompositeTarget.Dispose();
            tempSceneTarget.Dispose();
            tempBloomTarget.Dispose();
            base.UnloadContent();
        }

        public void BloomSetInitialRenderTarget()
        {
            if (Visible)
                GraphicsDevice.SetRenderTarget(finalCompositeTarget);
            else
                Console.WriteLine("Bloom unable to set render target!!!");
        }

        public override void Draw(GameTime gameTime)
        {
            /*GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;

            bloomEffectStep1.Parameters["BloomThreshold"].SetValue(BLOOM_THRESHOLD);

//            BloomDrawIntoRenderTarget(finalCompositeTarget, tempBloomTarget, bloomEffectStep1);
            BloomDrawIntoRenderTarget(finalCompositeTarget, tempBloomTarget, GameStateManagementGame.PreferredBackBufferHeight,
                GameStateManagementGame.PreferredBackBufferWidth, bloomEffectStep1);
            GraphicsDevice.SetRenderTarget(null);*/
            base.Draw(gameTime);
        }

        public void BloomDrawIntoRenderTarget(Texture2D tex, RenderTarget2D renderTarget, int width, int height,
            Effect effect)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);

            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(tex, new Rectangle(0, 0, width,
                height), Color.White);
            spriteBatch.End();
            
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
