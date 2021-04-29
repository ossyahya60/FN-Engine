using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace MyEngine
{
    public class Transform : GameObjectComponent
    {
        public Vector2 LastPosition { private set; get; }

        private Vector2 scale = Vector2.One;
        private Vector2 position = Vector2.Zero;
        private float rotation = 0;
        private Vector2 localScale;

        public Transform()
        {
            rotation = 0;
            scale = Vector2.One;
            position = Vector2.Zero;
            LastPosition = position;
            localScale = Vector2.One;
        }

        public override void Start()
        {
            gameObject.Transform = this;
        }

        //public static int PixelsPerUnit = 100; //100 pixel = 1 unit = 1 meter
        public Vector2 Position
        {
            set
            {
                LastPosition = position;
                position = value;

                GameObject[] GOs = gameObject.GetChildren();

                if(GOs != null)
                    foreach (GameObject GO in GOs)
                        GO.Transform.Position += (position - LastPosition);
            }
            get
            {
                return position;
            }
        }

        public float Rotation //Gets rotation in radians
        {
            set
            {
                float LastRotation = rotation;
                rotation = value; //Check boundaries

                GameObject[] GOs = gameObject.GetChildren();

                if (GOs != null)
                    foreach (GameObject GO in GOs)
                        GO.Transform.Rotation += rotation - LastRotation;
            }
            get
            {
                return rotation;
            }
        }

        public Vector2 Scale //Scale of a gameobject in x-y coordinate system
        {
            set
            {
                Vector2 LastScale = scale;
                scale = value;
                scale.X = (scale.X >= 0) ? scale.X : 0;
                scale.Y = (scale.Y >= 0) ? scale.Y : 0;

                GameObject[] GOs = gameObject.GetChildren();

                if (GOs != null)
                    foreach (GameObject GO in GOs)
                        GO.Transform.Scale += (scale - LastScale) * GO.Transform.LocalScale;
            }
            get
            {
                return scale;
            }
        }

        public Vector2 LocalPosition
        {
            set
            {
                if (gameObject.Parent == null)
                    Position = value;
                else
                    Position = value + gameObject.Parent.Transform.Position;
            }
            get
            {
                if (gameObject.Parent == null)
                    return Position;
                else
                    return Position - gameObject.Parent.Transform.Position;
            }
        }

        public float LocalRotation
        {
            set
            {
                if (gameObject.Parent == null)
                    Rotation = value;
                else
                    Rotation = value + gameObject.Parent.Transform.Rotation;
            }
            get
            {
                if (gameObject.Parent == null)
                    return Rotation;
                else
                    return Rotation - gameObject.Parent.Transform.Rotation;
            }
        }

        //Experimental
        //public Matrix GetTransformationMatrix()
        //{
        //    return Matrix.CreateTranslation(Position.X, Position.Y, 1) * Matrix.CreateRotationZ(Rotation) * Matrix.CreateScale(Scale.X, Scale.Y, 1);
        //}

        public Vector2 LocalScale
        {
            set
            {
                Vector2 PrevLocalScale = localScale;
                if (gameObject.Parent == null)
                    Scale = value;
                else
                {
                    localScale = value;
                    localScale.X = (localScale.X >= 0) ? localScale.X : 0;
                    localScale.Y = (localScale.Y >= 0) ? localScale.Y : 0;

                    Scale *= localScale/PrevLocalScale;
                }
            }
            get
            {
                if (gameObject.Parent == null)
                    return Scale;
                else
                {
                    return localScale;
                }
            }
        }

        public static Vector2 Left
        {
            get
            {
                return new Vector2(-1, 0);
            }
        }
        public static Vector2 Right
        {
            get
            {
                return new Vector2(1, 0);
            }
        }
        public static Vector2 Up
        {
            get
            {
                return new Vector2(0, -1);
            }
        }
        public static Vector2 Down
        {
            get
            {
                return new Vector2(0, 1);
            }
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
            LocalScale += Vector2.UnitX * x;
        }

        public void ScaleY(float y) //Move a gameobject a certain distance in y
        {
            LocalScale += Vector2.UnitY * y;
        }

        public void ScaleBoth(float x, float y) //Move a gameobject a certain distance in x and y axis
        {
            LocalScale += Vector2.UnitX * x + Vector2.UnitY * y;
        }

        public void ScaleBoth(Vector2 scale) //Move a gameobject a certain distance in x and y axis
        {
            LocalScale += scale;
        }

        public override GameObjectComponent DeepCopy(GameObject clone)
        {
            Transform Clone = this.MemberwiseClone() as Transform;

            clone.Transform = Clone;
            Clone.position = new Vector2(position.X, position.Y);
            Clone.scale = new Vector2(scale.X, scale.Y);
            Clone.rotation = rotation;

            return Clone;
        }

        public override void Serialize(StreamWriter SW) //Pass transform to gameObject in deserialization
        {
            SW.WriteLine(ToString());

            base.Serialize(SW);
            SW.Write("LastPosition:\t" + LastPosition.X.ToString() + "\t" + LastPosition.Y.ToString() + "\n");
            SW.Write("scale:\t" + scale.X.ToString() + "\t" + scale.Y.ToString() + "\n");
            SW.Write("position:\t" + position.X.ToString() + "\t" + position.Y.ToString() + "\n");
            SW.Write("rotation:\t" + rotation.ToString() + "\n");
            SW.Write("localScale:\t" + localScale.X.ToString() + "\t" + localScale.Y.ToString() + "\n");

            SW.WriteLine("End Of " + ToString());
        }

        public override void Deserialize(StreamReader SR)
        {
            //SR.ReadLine(); //Already done

            base.Deserialize(SR);

            gameObject.Transform = this;
            string[] LP = SR.ReadLine().Split('\t');
            LastPosition = new Vector2(float.Parse(LP[1]), float.Parse(LP[2]));
            string[] SC = SR.ReadLine().Split('\t');
            scale = new Vector2(float.Parse(SC[1]), float.Parse(SC[2]));
            string[] Pos = SR.ReadLine().Split('\t');
            position = new Vector2(float.Parse(Pos[1]), float.Parse(Pos[2]));
            rotation = float.Parse(SR.ReadLine().Split('\t')[1]);
            string[] LSC = SR.ReadLine().Split('\t');
            localScale = new Vector2(float.Parse(LSC[1]), float.Parse(LSC[2]));

            SR.ReadLine();
        }
    }
}