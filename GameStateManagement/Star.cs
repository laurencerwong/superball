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
	public class Star : Microsoft.Xna.Framework.DrawableGameComponent
	{
		Texture2D myTexture;
		Vector2 vPosition = Vector2.Zero;
		Vector2 vVelocity = Vector2.Zero;
		const float fSpeed = 100.0f;
        Vector2 vRightScreenNormal = new Vector2(-1, 0);
        Vector2 vLeftScreenNormal = new Vector2(1, 0);
        Vector2 vTopScreenNormal = new Vector2(0, -1);
        Vector2 vBottomScreenNormal = new Vector2(0, 1);
		SpriteBatch spriteBatch;
		Random rand = new Random();

		public Star(Game game)
			: base(game)
		{
			// TODO: Construct any child components here
		}

		/// <summary>
		/// Allows the game component to perform any initialization it needs to before starting
		/// to run.  This is where it can query for any required services and load content.
		/// </summary>
		public override void Initialize()
		{
			// TODO: Add your initialization code here

			base.Initialize();
			DrawOrder = 500;

			// Set velocity to random direction
			vVelocity.X = rand.Next(-1000, 1000);
			vVelocity.Y = rand.Next(-1000, 1000);

			// Set to the const speed
			vVelocity.Normalize();
			vVelocity *= fSpeed;
		}


		protected override void LoadContent()
		{
			base.LoadContent();
			myTexture = Game.Content.Load<Texture2D>("supermariostar");
			spriteBatch = new SpriteBatch(GraphicsDevice);
		}

		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			// BEGIN: Don't change this code!
			float fDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
			vPosition += vVelocity * fDeltaTime;
			// END: Don't change this code!

			// TODO: Add your screen boundary detection
            if (vPosition.X > 512)
                vVelocity = Vector2.Reflect(vVelocity, vRightScreenNormal);
            else if (vPosition.X < -512)
                vVelocity = Vector2.Reflect(vVelocity, vLeftScreenNormal);
            if (vPosition.Y > 384)
                vVelocity = Vector2.Reflect(vVelocity, vTopScreenNormal);
            else if (vPosition.Y < -384)
                vVelocity = Vector2.Reflect(vVelocity, vBottomScreenNormal);

			// and reflection here!

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			spriteBatch.Begin();
			Vector2 vDrawPosition = vPosition;
			vDrawPosition.X += 512 - 64;
			vDrawPosition.Y += 384 - 64;
			spriteBatch.Draw(myTexture, vDrawPosition, Color.White);
			spriteBatch.End();
		}
	}
}
