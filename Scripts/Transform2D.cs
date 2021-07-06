using Microsoft.Xna.Framework;
using System;

namespace FN_Engine.Scripts
{
    public class Transform2D: GameObjectComponent
    {
        public Vector2 Position;
        public float Rotation;
        public Vector2 Scale;
        public GameObject Parent = null;

        internal Matrix ReCalculateMatrix()
        {
            Matrix TransformMat = Matrix.Identity;

            TransformMat = Matrix.CreateTranslation(Position.X, Position.Y, 0) * //Position should be positive, right?
                           Matrix.CreateRotationZ(Rotation) *
                           Matrix.CreateScale(Scale.X, Scale.Y, 1);

            if (Parent != null)
                return TransformMat;
            else
                return TransformMat;
        }

        public void UpdateValues()
        {
            Matrix TransformMat = ReCalculateMatrix();

            //Decomposing Matrix to vectors (This Method is obtained from StackExchange
            //URL: https://math.stackexchange.com/questions/237369/given-this-transformation-matrix-how-do-i-decompose-it-into-translation-rotati/417813

            Position = new Vector2(TransformMat.Translation.X, TransformMat.Translation.Y);
            Scale.X = (float)Math.Sqrt(TransformMat.M11 * TransformMat.M11 + TransformMat.M21 * TransformMat.M21 + TransformMat.M31 * TransformMat.M31);
            Scale.Y = (float)Math.Sqrt(TransformMat.M12 * TransformMat.M12 + TransformMat.M22 * TransformMat.M22 + TransformMat.M32 * TransformMat.M32);

            Matrix RotationMat = Matrix.Identity;
            RotationMat.M14 = 0;
            RotationMat.M24 = 0;
            RotationMat.M34 = 0;
            RotationMat.M41 = 0;
            RotationMat.M42 = 0;
            RotationMat.M43 = 0;
            RotationMat.M44 = 1;

            RotationMat.M11 = TransformMat.M11 / Scale.X;
            RotationMat.M21 = TransformMat.M21 / Scale.X;
            RotationMat.M31 = TransformMat.M31 / Scale.X;

            RotationMat.M12 = TransformMat.M12 / Scale.Y;
            RotationMat.M22 = TransformMat.M22 / Scale.Y;
            RotationMat.M32 = TransformMat.M32 / Scale.Y;

            RotationMat.M13 = TransformMat.M13 / 1; //Z scale should be 1, right?
            RotationMat.M23 = TransformMat.M23 / 1;
            RotationMat.M33 = TransformMat.M33 / 1;

            Quaternion RotQuat = Quaternion.CreateFromRotationMatrix(RotationMat);

            //From WIkipedia (From Quaternion to Euler)
            //URL: https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles

            // yaw (z-axis rotation)
            double siny_cosp = 2 * (RotQuat.W * RotQuat.Z + RotQuat.X * RotQuat.Y);
            double cosy_cosp = 1 - 2 * (RotQuat.Y * RotQuat.Y + RotQuat.Z * RotQuat.Z);

            Rotation = (float)Math.Atan2(siny_cosp, cosy_cosp);

            //Vector2 origin = new Vector2(_srcRect.Width * Origin.X, _srcRect.Height * Origin.Y); //Put this in spriteRenderer
        }
    }
}
