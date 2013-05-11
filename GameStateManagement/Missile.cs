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
    public class Missile : Actor
    {
        public int force_counter = 0;
        public Missile(Game game)
            : base(game)
        {
            meshName = "Missile";
            // TODO: Construct any child components here
        }
/*
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            timer.AddTimer("Despawn_timer", 5.0f, Despawn, false);
            GameplayScreen.soundbank.PlayCue("Ship_Missile");
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (force_counter < 100)
            {
                vForce = GetWorldFacing() * -100;
                force_counter++;
            }
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public void Despawn()
        {
            Game.Components.Remove(this);
            SpawnManager.Missiles.Remove(this);
        }
 */
    }
}
