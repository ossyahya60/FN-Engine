using Microsoft.Xna.Framework;
using System.IO;

namespace FN_Engine
{
    public enum ForceMode2D
    {
        Force, //Gradual Increasing Force
        Impulse //Burst Decreasing Force
    }

    public enum BodyType
    {
        Static, //As terrain, it doesn't move, but affects other rigidbodies
        Dynamic, //This is as static but it moves, affects other rigidbodies
        Kinematic //This is not affected by physics, but may affect other rgidbodies through collision
    }

    public class Rigidbody2D: GameObjectComponent
    {
        public bool KinematicToKinematicCollisionDetection = false;
        public bool AffectedByGravity;
        public bool Interpolate = false; //Might be supported in the future (Interpolation between frames for smoothness during high speeds
        public bool ConstraintHorizontalMovement = false;
        public bool ConstraintVerticalMovement = false;
        public BodyType BodyType = BodyType.Dynamic;
        public float LinearDragScale = 1; //This ranges from 0 to 1 to indicate how much the rigidbody should be affected by linear drag
        public float AngularDragScale = 1; //This ranges from 0 to 1 to indicate how much the rigidbody should be affected by angular drag
        public float GravityScale = 1; //This ranges from 0 to 1 to indicate how much the rigidbody should be affected by gravity
        public float Mass = 1; //Might be used for collision response and other physics-related stuff
        public Vector2 Velocity = Vector2.Zero; // 1 pixel per sec => This shouldn't be set every frame as it negates all other forces acting

        private float GravityConstant = 10; //Universal gravitational constant (Approximated to 10 for easiness)
        private bool Sleeping = false;
        private double DeltaTime = 1.0f / 60;

        public Rigidbody2D()
        {
            AffectedByGravity = true;
        }

        public override void Start()
        {
        }

        public override void Update(GameTime gameTime)
        {
            DeltaTime = gameTime.ElapsedGameTime.TotalSeconds;

            switch (BodyType)
            {
                case BodyType.Static: //Not affected by velocity or forces
                    {

                    }
                    break;
                case BodyType.Dynamic: //affected by everything :D =>This one checks collision only
                    {
                        float DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                        //Check collision here

                        if (AffectedByGravity)
                            Velocity.Y += GravityConstant * GravityScale * DeltaTime;

                        Velocity -= LinearDragScale * DeltaTime * Velocity;

                        gameObject.Transform.Position += Velocity * DeltaTime;
                    }
                    break;
                case BodyType.Kinematic: //not affected by forces
                    {
                        float DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if(KinematicToKinematicCollisionDetection)
                        {
                            //Check collision here =>Detection only, no response => Learn Signals please :D
                        }

                        gameObject.Transform.Position += Velocity * DeltaTime;
                    }
                    break;
                default:
                    break;
            }
        }

        public void AddForce(Vector2 Value, ForceMode2D Mode)
        {
            if (BodyType == BodyType.Dynamic) //Forces affect dynamic rigidbodies only
            {
                switch (Mode)
                {
                    case ForceMode2D.Force:
                        // Force = Mass * Acceleration, => F = Mass * dV/dt, then dV = F * dt / Mass
                        Velocity += Value * (float)DeltaTime / Mass;
                        break;
                    case ForceMode2D.Impulse:
                        // Adds to the velocity of the rigidbody directly!
                        Velocity += Value / Mass;
                        break;
                    default:
                        break;
                }
            }
        }

        public void AddForce(Vector2 Value)
        {
            if (BodyType == BodyType.Dynamic) //Forces affect dynamic rigidbodies only
            {
                // Force = Mass * Acceleration, => F = Mass * dV/dt, then dV = F * dt / Mass
                Velocity += Value * (float)DeltaTime / Mass;
            }
        }

        public override GameObjectComponent DeepCopy(GameObject Clone)
        {
            Rigidbody2D clone = this.MemberwiseClone() as Rigidbody2D;

            return clone;
        }

        public override void Serialize(StreamWriter SW) //get the transform in deserialization
        {
            SW.WriteLine(ToString());

            base.Serialize(SW);
            SW.Write("KinematicToKinematicCollisionDetection:\t" + KinematicToKinematicCollisionDetection.ToString() + "\n");
            SW.Write("AffectedByGravity:\t" + AffectedByGravity.ToString() + "\n");
            SW.Write("Interpolate:\t" + Interpolate.ToString() + "\n");
            SW.Write("ConstraintHorizontalMovement:\t" + ConstraintHorizontalMovement.ToString() + "\n");
            SW.Write("ConstraintVerticalMovement:\t" + ConstraintVerticalMovement.ToString() + "\n");
            SW.Write("BodyType:\t" + BodyType.ToString() + "\n");
            SW.Write("LinearDragScale:\t" + LinearDragScale.ToString() + "\n");
            SW.Write("AngularDragScale:\t" + AngularDragScale.ToString() + "\n");
            SW.Write("GravityScale:\t" + GravityScale.ToString() + "\n");
            SW.Write("Mass:\t" + Mass.ToString() + "\n");
            SW.Write("Velocity:\t" + Velocity.X.ToString() + "\t" + Velocity.Y.ToString() + "\n");
            SW.Write("Sleeping:\t" + Sleeping.ToString() + "\n");

            SW.WriteLine("End Of " + ToString());
        }
    }
}
