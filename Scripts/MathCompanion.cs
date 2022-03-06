using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FN_Engine
{
    public static class MathCompanion
    {
        public static float Abs(float number)
        {
            if (number >= 0)
                return number;
            else
                return -number;
        }

        public static Vector2 Abs(Vector2 number)
        {
            return new Vector2(Math.Abs(number.X), Math.Abs(number.Y));
        }

        public static System.Numerics.Vector2 Abs(System.Numerics.Vector2 number)
        {
            return new System.Numerics.Vector2(Math.Abs(number.X), Math.Abs(number.Y));
        }

        public static Point Abs(Point number)
        {
            return new Point(Math.Abs(number.X), Math.Abs(number.Y));
        }

        public static bool IsPositive(float number)
        {
            if (number >= 0)
                return true;
            else
                return false;
        }

        public static float Clamp(float Value, float Min, float Max)
        {
            if (Value < Min)
                return Min;
            else if (Value > Max)
                return Max;
            else
                return Value;
        }

        public static Vector2 Clamp(Vector2 Value, Vector2 Min, Vector2 Max)
        {
            Vector2 Output = Value;
            if (Value.X <= Min.X)
                Output.X = Min.X;
            else if (Value.X > Max.X)
                Output.X = Max.X;

            if (Value.Y <= Min.Y)
                Output.Y = Min.Y;
            else if (Value.Y > Max.Y)
                Output.Y = Max.Y;

            return Output;
        }

        public static int Sign(float number)
        {
            if (number >= 0)
                return 1;
            else
                return -1;
        }

        public static Vector2 Sign(Vector2 vector2)
        {
            return new Vector2(Math.Sign(vector2.X), Math.Sign(vector2.Y));
        }

        public static double Atan(float y, float x)
        {
            return Math.Atan2(y, x);
        }

        public static float GetAngle(Vector2 V1, Vector2 V2)
        {
            return MathHelper.ToDegrees((float)Atan(V2.Y - V1.Y, V2.X - V1.X));
        }

        public static float GetAngle_Rad(Vector2 V1, Vector2 V2)
        {
            return (float)Atan(V2.Y - V1.Y, V2.X - V1.X);
        }

        public static int GetNumerOfValuesBetween(int A1, int A2, float ScaleDownFactor)
        {
            float sign = 0;
            if (A2 != A1)
                sign = Sign(A2 - A1);

            return (int)((A2 - A1) * ScaleDownFactor - sign);
        }

        public static float CrossProduct(Vector2 V1, Vector2 V2)
        {
            return (V1.X* V2.Y) - (V1.Y* V2.X);
        }

        public static Vector3 GetCircleEnclosingPoints(List<Vector2> points)
        {
            Vector3 circle = Vector3.Zero; //radius, X, Y

            foreach(Vector2 point1 in points)
            {
                foreach (Vector2 point2 in points)
                {
                    float lengthSq = (point1 - point2).LengthSquared();
                    if (lengthSq > circle.X)
                    {
                        circle.X = lengthSq;
                        circle.Y = (point1.X + point2.X) / 2.0f;
                        circle.Z = (point1.Y + point2.Y) / 2.0f;
                    }
                }
            }

            circle.X = (float)Math.Sqrt(circle.X) / 2;

            return circle;
        }

        public static List<Vector2> GetEquTriangleEnclosingPoints(List<Vector2> points)
        {
            List<Vector2> triangle = new List<Vector2>(); //radius, X, Y

            float offset = 20.0f;

            Vector3 circle = GetCircleEnclosingPoints(points);

            float sideLength = circle.X * (6 / (float)Math.Sqrt(3));
            float height = sideLength * ((float)Math.Sqrt(3) / 2);

            triangle.Add(new Vector2(circle.Y - sideLength * 0.5f - offset, circle.Z + circle.X + offset));
            triangle.Add(new Vector2(circle.Y + sideLength * 0.5f + offset, circle.Z + circle.X + offset));
            triangle.Add(new Vector2(circle.Y, circle.Z + circle.X - height - offset));

            return triangle;
        }

        public static object DelanuayTriangulation(List<Vector2> points)
        {
            var motherTriangle = GetEquTriangleEnclosingPoints(points);
            var triangles = new List<List<Vector2>>();
            triangles.Add(motherTriangle);

            List<Vector2> points2 = new List<Vector2>();
            points2.AddRange(motherTriangle);

            //loop on all points
            foreach (Vector2 point in points)
            {
                points2.Add(point);
                //get the circumcircle of every triangle
                foreach (List<Vector2> tri in triangles)
                {
                    float p = tri[0].X;
                    float t = tri[0].Y;
                    float q = tri[1].X;
                    float u = tri[1].Y;
                    float s = tri[2].X;
                    float z = tri[2].Y;
                    float z2 = (float)Math.Pow(z, 2);
                    float s2 = (float)Math.Pow(s, 2);
                    float u2 = (float)Math.Pow(u, 2);
                    float q2 = (float)Math.Pow(q, 2);
                    float t2 = (float)Math.Pow(t, 2);
                    float p2 = (float)Math.Pow(p, 2);

                    float A = ((u - t) * z2 + (-u2 + t2 - q2 + p2) * z + t * u2 + (-t2 + s2 - p2) * u + (q2 - s2) * t) / ((q - p) * z + (p - s) * u + (s - q) * t);
                    float B = -((q - p) * z2 + (p - s) * u2 + (s - q) * t2 + (q - p) * s2 + (p2 - q2) * s + p * q2 - p2 * q) / ((q - p) * z + (p - s) * u + (s - q) * t);
                    float C = -((p * u - q * t) * z2 + (-p * u2 + q * t2 - p * q2 + p2 * q) * z + s * t * u2 + (-s * t2 + p * s2 - p2 * s) * u + (q2 * s - q * s2) * t) / ((q - p) * z + (p - s) * u + (s - q) * t);

                    foreach (Vector2 point2 in points2)
                    {
                        if (point2 == point)
                            continue;

                        float finalVal = (point2.X * point2.X) + (point2.Y * point2.Y) + A * point2.X + B * point2.Y + C;
                    }
                }
            }

            return null;
        }
    }
}
