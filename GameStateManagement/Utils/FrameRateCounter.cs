using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Utils
{
    public class FrameRateCounter : DrawableGameComponent
    {
        private SpriteBatch m_kSpriteBatch;
        private SpriteFont m_kFont;

        private Vector2 m_vPosition;

		private float m_fCurrentFrameRate;

        private Queue<float> m_fPriorFrameRates;
        private float m_fMaxFrameRate;
        private float m_fMinFrameRate;

        private const int m_iMaxPriorFrameCount = 1000;
        private const int m_iMaxPriorLockedFrameCount = 120;
        bool LockedFrameRate = true;
        private int m_iCurPriorFrameCount;
        
        public FrameRateCounter(Game game, Vector2 vPosition)
            : base(game)
        {
	    DrawOrder = 1000;
            m_vPosition = vPosition;
            m_fPriorFrameRates = new Queue<float>();
        }

        protected override void LoadContent()
        {
            IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)this.Game.Services.GetService(typeof(IGraphicsDeviceService));

            m_kSpriteBatch = new SpriteBatch(graphicsService.GraphicsDevice);
            m_kFont = Game.Content.Load<SpriteFont>("fpsfont");

            base.LoadContent();
        }
        
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }
        
        public override void Update(GameTime gameTime)
        {
            
            m_fPriorFrameRates.Enqueue((float)gameTime.ElapsedGameTime.TotalSeconds);
            m_iCurPriorFrameCount++;

            if (m_iCurPriorFrameCount > (LockedFrameRate ? m_iMaxPriorLockedFrameCount : m_iMaxPriorFrameCount))
            {
                m_fCurrentFrameRate = m_fPriorFrameRates.Dequeue();
                m_fMaxFrameRate = m_fCurrentFrameRate;
                m_fMinFrameRate = m_fCurrentFrameRate;

                foreach (float frameRate in m_fPriorFrameRates)
                {
                    if (frameRate < m_fMaxFrameRate) m_fMaxFrameRate = frameRate;
                    else if (frameRate > m_fMinFrameRate) m_fMinFrameRate = frameRate;
                    m_fCurrentFrameRate += frameRate;
                }

                m_fCurrentFrameRate = (float)(LockedFrameRate ? m_iMaxPriorLockedFrameCount : m_iMaxPriorFrameCount)/ m_fCurrentFrameRate;
                m_fMaxFrameRate = (float)1.0/ m_fMaxFrameRate;
                m_fMinFrameRate = (float)1.0/ m_fMinFrameRate;

                m_fPriorFrameRates.Clear();
                m_iCurPriorFrameCount = 0;
            }


			base.Update(gameTime);
        }
        
        public override void Draw(GameTime gameTime)
        {
            m_kSpriteBatch.Begin();
            
			// Color this based on the framerate
            Color DrawColor = Color.Green;
			if (m_fCurrentFrameRate < 15.0f)
                DrawColor = Color.Red;
			else if (m_fCurrentFrameRate < 30.0f)
                DrawColor = Color.Yellow;
            /*
			m_kSpriteBatch.DrawString(m_kFont, "AFPS: " + m_fCurrentFrameRate.ToString("f3")
                + "\nMFPS: " + m_fMaxFrameRate.ToString("f3") + "\nmFPS: " + m_fMinFrameRate.ToString("f3"), m_vPosition, DrawColor);
             */
            m_kSpriteBatch.End();

            base.Draw(gameTime);
        }

		public void ResetFPSCount()
		{
            LockedFrameRate = !LockedFrameRate;
            m_iCurPriorFrameCount = 0;
            m_fPriorFrameRates.Clear();
		}
    }
}
