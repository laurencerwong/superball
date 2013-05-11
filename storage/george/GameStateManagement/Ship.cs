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
    public class Ship : Actor
    {
        private bool CanFireMissile = true;
        public Ship(Game game)
            : base(game)
        {
            meshName = "Ship";
            fMass = 0.7f;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            GameplayScreen.soundbank.PlayCue("Ship_Spawn");
            m_qRotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2);
            base.Initialize();
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

        private void ResetMissileTimer()
        {
            CanFireMissile = true;
        }

        public void MoveForward()
        {
            vForce = GetWorldFacing() * 50.0f;
        }
        public void MoveBackward()
        {
            vForce = GetWorldFacing() * -50.0f;
        }
        public void TurnLeft(float gameTimeSeconds)
        {
            m_qRotation *= Quaternion.CreateFromAxisAngle(
                       Vector3.UnitY, MathHelper.ToRadians(25.0f) * gameTimeSeconds * 10.0f);
        }
        public void TurnRight(float gameTimeSeconds)
        {
            m_qRotation *= Quaternion.CreateFromAxisAngle(
                      Vector3.UnitY, MathHelper.ToRadians(-25.0f) * gameTimeSeconds * 10.0f);
        }
        public void FireMissile()
        {
            if(CanFireMissile)
            {
                CanFireMissile = false;
                timer.AddTimer("missile_timer", 1.0f, ResetMissileTimer, false);
                Missile FiredMissile = new Missile(Game);
                Game.Components.Add(FiredMissile);
                FiredMissile.m_vWorldPosition = GetWorldPosition();
                FiredMissile.m_qRotation = m_qRotation * Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi);
                FiredMissile.fMass = 0.5f;  
                FiredMissile.vForce = FiredMissile.GetWorldFacing() * -5000.0f;
                SpawnManager.AddMissile(FiredMissile);
            }
        }
        public void Despawn()
        {
            Game.Components.Remove(this);
        }
    }
}
