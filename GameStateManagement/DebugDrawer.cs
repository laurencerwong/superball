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
    public class DebugDrawer : BulletSharp.DebugDraw
    {
        GraphicsDevice gd;
        public static BasicEffect basiceffect;
        public DebugDrawer(GraphicsDevice gd)
            : base()
        {
            this.gd = gd;
            basiceffect = new BasicEffect(gd);
            basiceffect.View = GameplayScreen.CameraMatrix;
            basiceffect.World = Matrix.Identity;
            basiceffect.Projection = GameplayScreen.ProjectionMatrix;
            basiceffect.VertexColorEnabled = true;
        }


        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        public override BulletSharp.DebugDrawModes DebugMode
        {
            get
            {
                return _DebugMode;
            }
            set
            {
                _DebugMode = value;
            }
        }
        private BulletSharp.DebugDrawModes _DebugMode;

        public override void Draw3dText(ref Vector3 location, string textString)
        {
        }

        public override void DrawAabb(ref Vector3 from, ref Vector3 to, Color color)
        {
        }

        public override void DrawArc(ref Vector3 center, ref Vector3 normal, ref Vector3 axis, float radiusA, float radiusB, float minAngle, float maxAngle, Color color, bool drawSect, float stepDegrees)
        {
        }

        public override void DrawArc(ref Vector3 center, ref Vector3 normal, ref Vector3 axis, float radiusA, float radiusB, float minAngle, float maxAngle, Color color, bool drawSect)
        {
        }

        public override void DrawBox(ref Vector3 bbMin, ref Vector3 bbMax, ref Matrix trans, Color color)
        {
            Vector3 a = new Vector3(bbMax.X, bbMin.Y, bbMax.Z);
            Vector3 b = new Vector3(bbMin.X, bbMin.Y, bbMax.Z);
            Vector3 c = new Vector3(bbMax.X, bbMin.Y, bbMin.Z);
            Vector3 d = new Vector3(bbMax.X, bbMax.Y, bbMin.Z);
            Vector3 e = new Vector3(bbMin.X, bbMax.Y, bbMax.Z);
            Vector3 f = new Vector3(bbMin.X, bbMax.Y, bbMin.Z);
            DrawTriangle(ref bbMax, ref a, ref b, color, 0.1f);
            DrawTriangle(ref bbMax, ref e, ref b, color, 0.1f);
            DrawTriangle(ref bbMax, ref a, ref c, color, 0.1f);
            DrawTriangle(ref bbMax, ref d, ref c, color, 0.1f);
            DrawTriangle(ref d, ref f, ref bbMin, color, 0.1f);
            DrawTriangle(ref d, ref c, ref bbMin, color, 0.1f);
            DrawTriangle(ref c, ref a, ref bbMin, color, 0.1f);
            DrawTriangle(ref b, ref a, ref bbMin, color, 0.1f);
            DrawTriangle(ref b, ref f, ref bbMin, color, 0.1f);
            DrawTriangle(ref b, ref e, ref f, color, 0.1f);
            DrawTriangle(ref bbMax, ref d, ref f, color, 0.1f);
            DrawTriangle(ref bbMax, ref e, ref f, color, 0.1f);
            DrawTriangle(ref c, ref bbMin, ref a, color, 0.1f);
            DrawTriangle(ref a, ref bbMin, ref b, color, 0.1f);
        }

        public override void DrawBox(ref Vector3 bbMin, ref Vector3 bbMax, Color color)
        {
            Vector3 a = new Vector3(bbMax.X, bbMin.Y, bbMax.Z);
            Vector3 b = new Vector3(bbMin.X, bbMin.Y, bbMax.Z);
            Vector3 c = new Vector3(bbMax.X, bbMin.Y, bbMin.Z);
            Vector3 d = new Vector3(bbMax.X, bbMax.Y, bbMin.Z);
            Vector3 e = new Vector3(bbMin.X, bbMax.Y, bbMax.Z);
            Vector3 f = new Vector3(bbMin.X, bbMax.Y, bbMin.Z);
            DrawTriangle(ref bbMax, ref a, ref b, color, 0.1f);
            DrawTriangle(ref bbMax, ref e, ref b, color, 0.1f);
            DrawTriangle(ref bbMax, ref a, ref c, color, 0.1f);
            DrawTriangle(ref bbMax, ref d, ref c, color, 0.1f);
            DrawTriangle(ref d, ref f, ref bbMin, color, 0.1f);
            DrawTriangle(ref d, ref c, ref bbMin, color, 0.1f);
            DrawTriangle(ref c, ref a, ref bbMin, color, 0.1f);
            DrawTriangle(ref b, ref a, ref bbMin, color, 0.1f);
            DrawTriangle(ref b, ref f, ref bbMin, color, 0.1f);
            DrawTriangle(ref b, ref e, ref f, color, 0.1f);
            DrawTriangle(ref bbMax, ref d, ref f, color, 0.1f);
            DrawTriangle(ref bbMax, ref e, ref f, color, 0.1f);
            DrawTriangle(ref c, ref bbMin, ref a, color, 0.1f);
            DrawTriangle(ref a, ref bbMin, ref b, color, 0.1f);
        }

        public override void DrawCapsule(float radius, float halfHeight, int upAxis, ref Matrix transform, Color color)
        {
        }

        public override void DrawCone(float radius, float height, int upAxis, ref Matrix transform, Color color)
        {
            throw new NotImplementedException();
        }

        public override void DrawContactPoint(ref Vector3 pointOnB, ref Vector3 normalOnB, float distance, int lifeTime, Color color)
        {
            throw new NotImplementedException();
        }

        public override void DrawCylinder(float radius, float halfHeight, int upAxis, ref Matrix transform, Color color)
        {
            throw new NotImplementedException();
        }

        public override void DrawLine(ref Vector3 from, ref Vector3 to, Color color)
        {
            short[] lineListIndices = new short[2];
            lineListIndices[0] = 0;
            lineListIndices[1] = 1;
            VertexPositionColor[] vertexList = new VertexPositionColor[2];
            vertexList[0] = new VertexPositionColor(from, color);
            vertexList[1] = new VertexPositionColor(to, color);

            basiceffect.CurrentTechnique.Passes[0].Apply();
            gd.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList,
                vertexList, 0, 2, lineListIndices, 0, 1);

        }

        public override void DrawLine(ref Vector3 from, ref Vector3 to, Color fromColor, Color toColor)
        {
            short[] lineListIndices = new short[2];
            lineListIndices[0] = 0;
            lineListIndices[1] = 1;
            VertexPositionColor[] vertexList = new VertexPositionColor[2];
           // vertexList[0] = new VertexPositionColor(from, Vector3.UnitY, Vector2.One);
           // vertexList[1] = new VertexPositionColor(to, Vector3.UnitY, Vector2.One);
            vertexList[0] = new VertexPositionColor(from, fromColor);
            vertexList[1] = new VertexPositionColor(to, fromColor);
            basiceffect.CurrentTechnique.Passes[0].Apply();
            gd.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList,
                vertexList, 0, 2, lineListIndices, 0, 1);
        }

        public override void DrawPlane(ref Vector3 planeNormal, float planeConst, ref Matrix transform, Color color)
        {
        }

        public override void DrawSphere(ref Vector3 p, float radius, Color color)
        {
            //return;
            Vector3 a = new Vector3(p.X, p.Y + radius, p.Z);
            Vector3 b = new Vector3(p.X, p.Y - radius, p.Z);
            Vector3 c = new Vector3(p.X + radius, p.Y, p.Z);
            Vector3 d = new Vector3(p.X - radius, p.Y, p.Z);
            Vector3 e = new Vector3(p.X, p.Y, p.Z - radius);
            Vector3 f = new Vector3(p.X, p.Y, p.Z + radius);
            DrawTriangle(ref a, ref f, ref c, color, 0.1f);
            DrawTriangle(ref a, ref c, ref e, color, 0.1f);
            DrawTriangle(ref a, ref e, ref d, color, 0.1f);
            DrawTriangle(ref a, ref d, ref f, color, 0.1f);
            DrawTriangle(ref b, ref f, ref c, color, 0.1f);
            DrawTriangle(ref b, ref c, ref e, color, 0.1f);
            DrawTriangle(ref b, ref e, ref d, color, 0.1f);
            DrawTriangle(ref b, ref d, ref f, color, 0.1f);
        }

        public override void DrawSphere(float radius, ref Matrix transform, Color color)
        {
            Vector3 position = Vector3.Zero;
            radius++;
            DrawSphere(ref position, radius, color);
        }

        public override void DrawSpherePatch(ref Vector3 center, ref Vector3 up, ref Vector3 axis, float radius, float minTh, float maxTh, float minPs, float maxPs, Color color, float stepDegrees)
        {
        }

        public override void DrawSpherePatch(ref Vector3 center, ref Vector3 up, ref Vector3 axis, float radius, float minTh, float maxTh, float minPs, float maxPs, Color color)
        {
        }

        public override void DrawTransform(ref Matrix transform, float orthoLen)
        {
            //throw new NotImplementedException();
        }

        public override void DrawTriangle(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, Color color, float __unnamed004)
        {
            DrawLine(ref v0, ref v1, color);
            DrawLine(ref v1, ref v2, color);
            DrawLine(ref v2, ref v0, color);

        }

        public override void DrawTriangle(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, ref Vector3 __unnamed003, ref Vector3 __unnamed004, ref Vector3 __unnamed005, Color color, float alpha)
        {
            DrawTriangle(ref v0, ref v1, ref v2, color, alpha);
        }

        public override void ReportErrorWarning(string warningString)
        {
            //throw new NotImplementedException();
        }
    }
}
