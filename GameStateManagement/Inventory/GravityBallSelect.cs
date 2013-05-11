using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BulletSharp;

namespace GameStateManagement
{
    class GravityBallSelect : InventoryObject
    {
        public GravityBallSelect(Game game)
            : base(game)
        {
            meshName = "gravityball"; //PLACEHOLDER
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
    }
}

