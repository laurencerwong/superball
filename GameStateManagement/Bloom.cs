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
        const float BLOOM_THRESHOLD = 0.5f; //lower = more sensitive

        const int BLOOM_PASSES = 1;

        SpriteBatch spriteBatch;

        Effect bloomEffectStep1;
        Effect bloomEffectStep2_3;
        Effect bloomEffectStep4;
        Effect motionBlurEffect;

        int bufferWidth;
        int bufferHeight;

        float[] bloomStrength;
        int currentBloomStrength = 1;

        int bloomWidth;
        int bloomHeight;

        RenderTarget2D tempSceneTarget;
        RenderTarget2D tempBloomTarget;
        RenderTarget2D finalCompositeTarget;
        RenderTarget2D motionBlur;


        public Bloom(Game game)
            : base(game)
        {
            bufferWidth = GameStateManagementGame.PreferredBackBufferWidth;
            bufferHeight = GameStateManagementGame.PreferredBackBufferHeight;

            bloomWidth = bufferWidth / 2;
            bloomHeight = bufferHeight / 2;

            bloomStrength = new float[4];
            bloomStrength[0] = 0.0f;
            bloomStrength[1] = 1.0f;
            bloomStrength[2] = 3.0f;
            bloomStrength[3] = 10.0f;
            
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
            bloomEffectStep2_3 = Game.Content.Load<Effect>("Shaders/BloomBlurHighlights");
            bloomEffectStep4 = Game.Content.Load<Effect>("Shaders/BloomRecombine");
            motionBlurEffect = Game.Content.Load<Effect>("Shaders/MotionBlurCombine");


            PresentationParameters graphParams = GraphicsDevice.PresentationParameters;

            finalCompositeTarget = new RenderTarget2D(GraphicsDevice, GameStateManagementGame.PreferredBackBufferWidth,
                GameStateManagementGame.PreferredBackBufferHeight, false, graphParams.BackBufferFormat, 
                graphParams.DepthStencilFormat, graphParams.MultiSampleCount, RenderTargetUsage.DiscardContents);

            tempBloomTarget = new RenderTarget2D(GraphicsDevice, bloomWidth,
                bloomHeight, false, graphParams.BackBufferFormat, DepthFormat.None);

            tempSceneTarget = new RenderTarget2D(GraphicsDevice, bloomWidth,
                bloomHeight, false, graphParams.BackBufferFormat, DepthFormat.None);
            motionBlur = new RenderTarget2D(GraphicsDevice, GameStateManagementGame.PreferredBackBufferWidth,
                GameStateManagementGame.PreferredBackBufferHeight, false, graphParams.BackBufferFormat,
                graphParams.DepthStencilFormat, graphParams.MultiSampleCount, RenderTargetUsage.DiscardContents);
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            finalCompositeTarget.Dispose();
            tempSceneTarget.Dispose();
            tempBloomTarget.Dispose();
            base.UnloadContent();
        }

        public void ClearMotionBlurBuffer()
        {
            Texture2D blackRectangle = new Texture2D(GraphicsDevice, 1, 1);

            GraphicsDevice.SetRenderTarget(motionBlur);
            spriteBatch.Begin(0, BlendState.Opaque);
            spriteBatch.Draw(blackRectangle, new Rectangle(0, 0, bufferWidth, bufferHeight), Color.White);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(finalCompositeTarget);
            spriteBatch.Begin(0, BlendState.Opaque);
            spriteBatch.Draw(blackRectangle, new Rectangle(0, 0, bufferWidth, bufferHeight), Color.White);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(tempBloomTarget);
            spriteBatch.Begin(0, BlendState.Opaque);
            spriteBatch.Draw(blackRectangle, new Rectangle(0, 0, bloomWidth, bloomHeight), Color.White);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(tempSceneTarget);
            spriteBatch.Begin(0, BlendState.Opaque);
            spriteBatch.Draw(blackRectangle, new Rectangle(0, 0, bloomWidth, bloomHeight), Color.White);
            spriteBatch.End();
        }

        public void BloomSetInitialRenderTarget()
        {
            if (Visible)
            {
                GraphicsDevice.SetRenderTarget(finalCompositeTarget);
            }
        }

        public void toggleBloom()
        {
            currentBloomStrength++;
            currentBloomStrength %= 4;
            Console.WriteLine("currentBloomStrength = " + currentBloomStrength);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;

            bloomEffectStep1.Parameters["BloomThreshold"].SetValue(BLOOM_THRESHOLD);
            motionBlurEffect.Parameters["motionBlurAmount"].SetValue(0.0f);
            GraphicsDevice.Textures[1] = motionBlur;
            BloomDrawIntoRenderTargetMotionBlur(finalCompositeTarget, motionBlur, bufferWidth, bufferHeight, motionBlurEffect);
            BloomDrawIntoRenderTarget(motionBlur, tempBloomTarget, bloomWidth,
                bloomHeight, bloomEffectStep1);

//            BloomDrawIntoRenderTarget(tempBloomTarget, tempSceneTarget, bloomWidth / 2, bloomHeight / 2, bloomEffectStep1);

            bloomEffectStep2_3.Parameters["BlurStrength"].SetValue(bloomStrength[currentBloomStrength]);
            bloomEffectStep2_3.Parameters["BlurRadius"].SetValue(1.1f);
            bloomEffectStep2_3.Parameters["Width"].SetValue(bloomWidth / 2);
            bloomEffectStep2_3.Parameters["Height"].SetValue(bloomHeight / 2);

            for(int i = 0; i < BLOOM_PASSES; i++)
            {
                bloomEffectStep2_3.Parameters["orientation"].SetValue(false);
            
            BloomDrawIntoRenderTarget(tempBloomTarget, tempSceneTarget, bloomWidth, bloomHeight, bloomEffectStep2_3);

            bloomEffectStep2_3.Parameters["orientation"].SetValue(true);

            BloomDrawIntoRenderTarget(tempSceneTarget, tempBloomTarget, bloomWidth, bloomHeight, bloomEffectStep2_3);

            }

            
            
            BloomDrawIntoRenderTarget(tempBloomTarget, null, bufferWidth, bufferHeight, bloomEffectStep4);
            GraphicsDevice.Textures[1] = null;
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
        public void BloomDrawIntoRenderTargetMotionBlur(Texture2D tex, RenderTarget2D renderTarget, int width, int height,
           Effect effect)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(tex, new Rectangle(0, 0, width, height), new Color(255, 255, 255, 255));
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
