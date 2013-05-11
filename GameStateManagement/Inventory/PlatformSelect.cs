using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BulletSharp;

namespace GameStateManagement
{
    class PlatformSelect : InventoryObject
    {

        public PlatformSelect(Game game)
            : base(game)
        {
            meshName = "squareplatform"; //PLACEHOLDER
            boundingType = BoundingType.BOX;
        }
        public override void Initialize()
        {
            fMass = 0.0f;
            base.Initialize();
            m_fScale = 1.0f;
            friction = 1.0f;
            generateBoundingShape(ref boundingSphere);
        }
        /*
        public void ApplyForce(Vector3 Force, Vector3 ForcePosition)
        {
            body.ApplyForce(Force, ForcePosition);
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            base.Update(gameTime);
        }

        public void Despawn()
        {
            Game.Components.Remove(this);
        }
         */
    }
}
