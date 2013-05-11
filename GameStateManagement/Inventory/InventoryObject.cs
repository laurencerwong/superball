using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BulletSharp;

namespace GameStateManagement
{
    class InventoryObject : Actor
    {
        /*
        private int numRemaining = 0;
        public int NumRemaining
        {
            get
            {
                return numRemaining;
            }
            set
            {
                numRemaining = value;
            }
        }
         */
        public SphereShape boundingSphere;
        public InventoryObject(Game game) : base(game)
        {
            //isUserMovable = true;
            CollidesWith = BulletSharp.CollisionFilterGroups.DefaultFilter;
            CollisionGroup = BulletSharp.CollisionFilterGroups.DebrisFilter;
            radius = 70.0f;
        }

        public override void Initialize()
        {
            base.Initialize();
            isUserMovable = true;
        }

        /*
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }*/
        
        public override void Update(GameTime gameTime)
        {
            /*
            if (GameplayScreen.cameraType == GameplayScreen.CameraTypes.Simulation)
                Visible = false;
            else
                Visible = true;
             */
            if ( GameplayScreen.cameraType == GameplayScreen.CameraTypes.LevelEditor)
            {
                Visible = true;
            }
            else
            {
                Visible = false;
            }
            base.Update(gameTime);
        }
    }
}
