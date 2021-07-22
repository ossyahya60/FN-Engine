using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace FN_Engine
{
    public class Transform : GameObjectComponent
    {
        public Vector2 LastPosition { private set; get; }
        public float LastRotation { private set; get; }
        public Vector2 LastScale { private set; get; }
        public Vector2 Position;
        public float Rotation = 0;
        public Vector2 Scale = Vector2.One;

        internal bool JustParented = false;

        internal Transform FakeTransform
        {
            set
            {
                fakeTransform = value;
            }
            get
            {
                if (gameObject.Parent == null)
                    return this;

                if (fakeTransform == null)
                    fakeTransform = DeepCopy(null) as Transform;

                return fakeTransform;
            }
        }

        private Transform fakeTransform = null;

        public Transform()
        {
            LastPosition = Vector2.Zero;
        }

        public override void Start()
        {
            gameObject.Transform = this;
        }

        public static Transform Identity { get { return _Identity; } }

        private static readonly Transform _Identity = new Transform();

        internal void CloneRelevantData(ref Transform ToBeSet)
        {
            ToBeSet.Position = Position;
            ToBeSet.Rotation = Rotation;
            ToBeSet.Scale = Scale;
        }

        internal override void UpdateUI(GameTime gameTime)
        {
            LastPosition = Position;
            LastRotation = Rotation;
            LastScale = Scale;
        }

        public override void Update(GameTime gameTime)
        {
            LastPosition = Position;
            LastRotation = Rotation;
            LastScale = Scale;
        }

        // This Appoach is based on this example code from here:
        //URL: http://www.catalinzima.com/2011/06/2d-skeletal-animations/
        public static Transform Compose(Transform a, Transform b) //Eats memory?
        {
            Transform Result = new Transform();
            Vector2 transformedPosition = a.TransformVector(b.Position);
            Result.Position = transformedPosition;
            Result.Rotation = a.Rotation + b.Rotation;
            Result.Scale = a.Scale * b.Scale;
            return Result;
        }

        public static void Lerp(ref Transform key1, ref Transform key2, float amount, ref Transform result)
        {
            result.Position = Vector2.Lerp(key1.Position, key2.Position, amount);
            result.Scale = Vector2.Lerp(key1.Scale, key2.Scale, amount);
            result.Rotation = MathHelper.Lerp(key1.Rotation, key2.Rotation, amount);
        }

        //Call this function if you found that transform doesn't respond to change by parent transformations
        public Transform AdjustTransformation()
        {
            Transform T = gameObject.Transform.FakeTransform;
            if (gameObject.Parent != null)
            {
                if (gameObject.Transform.JustParented)
                {
                    Position -= gameObject.Parent.Transform.Position;
                    Rotation -= gameObject.Parent.Transform.Rotation;
                    Scale /= gameObject.Parent.Transform.Scale;
                    JustParented = false;
                }

                if (Position - LastPosition != Vector2.Zero)
                    Console.WriteLine("WOOW");

                T.Position += Position - LastPosition;
                T.Rotation += Rotation - LastRotation;
                T.Scale += gameObject.Transform.Scale - LastScale;

                T = Compose(gameObject.Parent.Transform, T);

                T.CloneRelevantData(ref gameObject.Transform);
            }

            return T;
        }

        public Vector2 TransformVector(Vector2 point)
        {
            Vector2 result = Vector2.Transform(point, Matrix.CreateRotationZ(Rotation));
            result *= Scale;
            result += Position;
            return result;
        }

        public void MoveX(float x) //Move a gameobject a certain distance in x
        {
            Position += Vector2.UnitX * x;
        }

        public void MoveY(float y) //Move a gameobject a certain distance in y
        {
            Position += Vector2.UnitY * y;
        }

        public void Move(float x, float y) //Move a gameobject a certain distance in x and y axis
        {
            Position += Vector2.UnitX * x + Vector2.UnitY * y;
        }

        public void Move(Vector2 Movement) //Move a gameobject a certain distance in x and y axis
        {
            Position += Movement;
        }

        public void ScaleX(float x) //Move a gameobject a certain distance in x
        {
            Scale += Vector2.UnitX * x;
        }

        public void ScaleY(float y) //Move a gameobject a certain distance in y
        {
            Scale += Vector2.UnitY * y;
        }

        public void ScaleBoth(float x, float y) //Move a gameobject a certain distance in x and y axis
        {
            Scale += Vector2.UnitX * x + Vector2.UnitY * y;
        }

        public void ScaleBoth(Vector2 scale) //Move a gameobject a certain distance in x and y axis
        {
            Scale += scale;
        }

        public override void Destroy()
        {
            gameObject.Transform = null;
        }

        public override GameObjectComponent DeepCopy(GameObject clone)
        {
            Transform Clone = this.MemberwiseClone() as Transform;

            if(clone != null)
                clone.Transform = Clone;

            return Clone;
        }

        public override void Serialize(StreamWriter SW) //Pass transform to gameObject in deserialization
        {
            SW.WriteLine(ToString());

            base.Serialize(SW);
            SW.Write("LastPosition:\t" + LastPosition.X.ToString() + "\t" + LastPosition.Y.ToString() + "\n");
            //SW.Write("scale:\t" + scale.X.ToString() + "\t" + scale.Y.ToString() + "\n");
            //SW.Write("position:\t" + position.X.ToString() + "\t" + position.Y.ToString() + "\n");
            //SW.Write("rotation:\t" + rotation.ToString() + "\n");
            //SW.Write("localScale:\t" + localScale.X.ToString() + "\t" + localScale.Y.ToString() + "\n");

            SW.WriteLine("End Of " + ToString());
        }

        public override void Deserialize(StreamReader SR)
        {
            ////SR.ReadLine(); //Already done

            //base.Deserialize(SR);

            //gameObject.Transform = this;
            //string[] LP = SR.ReadLine().Split('\t');
            //LastPosition = new Vector2(float.Parse(LP[1]), float.Parse(LP[2]));
            //string[] SC = SR.ReadLine().Split('\t');
            //scale = new Vector2(float.Parse(SC[1]), float.Parse(SC[2]));
            //string[] Pos = SR.ReadLine().Split('\t');
            //position = new Vector2(float.Parse(Pos[1]), float.Parse(Pos[2]));
            //rotation = float.Parse(SR.ReadLine().Split('\t')[1]);
            //string[] LSC = SR.ReadLine().Split('\t');
            //localScale = new Vector2(float.Parse(LSC[1]), float.Parse(LSC[2]));

            //SR.ReadLine();
        }
    }
}