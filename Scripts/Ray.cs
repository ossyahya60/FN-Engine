using Microsoft.Xna.Framework;
using System;

namespace FN_Engine
{
    public class Ray
    {
        public Vector2 Origin = Vector2.Zero;
        public Vector2 Direction = Vector2.One;
        public float t = 1;
        public bool HitSomething = false; //Need to be reset for reuse

        public Vector2 GetAPointAlongRay(float Units)
        {
            return Origin + Units * Direction;
        }

        // return the distance of ray origin to intersection point
        public float GetRayToLineSegmentIntersection(Vector2 P1, Vector2 P2)
        {
            //Direction.Normalize();
            Vector2 v1 = Origin - P1;
            Vector2 v2 = P2 - P1;
            Vector2 v3 = new Vector2(-Direction.Y, Direction.X);

            float dot = Vector2.Dot(v2, v3);
            if (Math.Abs(dot) < 0.000001f)
                return -1.0f;

            float t1 = MathCompanion.CrossProduct(v2, v1) / dot;
            float t2 = Vector2.Dot(v1, v3) / dot;

            if (t1 >= 0.0f && t2 >= 0.0f && t2 <= 1.0f)
            {
                t = t1;
                return t1;
            }

            return -1.0f;
        }

        public float GetRayToLineSegmentIntersection(LineOccluder lineOccluder)
        {
            //Direction.Normalize();
            Vector2 v1 = Origin - lineOccluder.StartPoint;
            Vector2 v2 = lineOccluder.EndPoint - lineOccluder.StartPoint;
            Vector2 v3 = new Vector2(-Direction.Y, Direction.X);

            float dot = Vector2.Dot(v2, v3);
            if (Math.Abs(dot) < 0.000001f)
                return -1.0f;

            float t1 = MathCompanion.CrossProduct(v2, v1) / dot;
            float t2 = Vector2.Dot(v1, v3) / dot;

            if (t1 >= 0.0f && t2 >= 0.0f && t2 <= 1.0f)
            {
                t = t1;
                HitSomething = true;
                return t1;
            }

            return -1.0f;
        }

        //Make One ray that rotates one full rotation and determines the shadowy parts
    }
}
