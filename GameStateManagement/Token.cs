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
using BulletSharp;


namespace GameStateManagement
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Token : Actor
    {
        public BvhTriangleMeshShape triMesh;
        public Token(Game game)
            : base(game)
        {
            meshName = "token";

            boundingType = BoundingType.TRIMESH;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            fMass = 0.0f;
            base.Initialize();
            m_fScale = 1.0f;
            friction = 2.0f;
            restitution = 0.0f;
            generateBoundingShape(ref triMesh);
            isUserMovable = false;
            body.CcdSweptSphereRadius = 53.311f;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        public void ApplyForce(Vector3 Force, Vector3 ForcePosition)
        {
            body.ApplyForce(Force, ForcePosition);
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            base.Update(gameTime);
        }
    }
}
