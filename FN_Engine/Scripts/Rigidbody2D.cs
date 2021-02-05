using Microsoft.Xna.Framework;

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
        public Transform Transform;
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
            Transform = gameObject.Transform;
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

                        Transform.Position += Velocity * DeltaTime;
                    }
                    break;
                case BodyType.Kinematic: //not affected by forces
                    {
                        float DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if(KinematicToKinematicCollisionDetection)
                        {
                            //Check collision here =>Detection only, no response => Learn Signals please :D
                        }

                        Transform.Position += Velocity * DeltaTime;
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
            clone.Transform = Clone.Transform;

            return clone;
        }
    }

    //public class Rigidbody2D: GameObjectComponent
    //{
    //    public bool AffectedByGravity = true;
    //    public bool IsKinematic = false;
    //    public float GravityConstant = 10f;
    //    public float GravityScale = 1f;
    //    public float Mass = 1f;  //Will be used in collision response and collision reaction to other rigidbodies
    //    public bool AffectedByLinearDrag = true;
    //    public float LinearDrag = 1f;  //Affects drag as a whole, but is cuts Low speeds faster.
    //    public float QuadraticDrag = 1f;  //Affects drag as a whole, but is cuts High speeds faster.
    //    public bool ContinousDetection = false; //Turn this on if you want more precise collision detection, but it requires more computational power.
    //    public Vector2 Velocity{
    //        set
    //        {
    //            velocity = value;
    //            VelocityJustAssigned = true;
    //        }
    //        get
    //        {
    //            return velocity;
    //        }
    //    }

    //    private Transform Transform;
    //    private Vector2 NormalForcesApplied;
    //    private Vector2 ImpulseForcesApplied;
    //    private Vector2 LastPosition;
    //    private Vector2 velocity;
    //    private bool VelocityJustAssigned = false;
    //    private Vector2 HandyVector, HandyVector2; //avoid alot of memory allocation in short time

    //    public Rigidbody2D()
    //    {
            
    //    }

    //    public void ResetHorizVelocity()
    //    {
    //        velocity.X = 0;
    //    }

    //    public void ResetVerticalVelocity()
    //    {
    //        velocity.Y = 0;
    //    }

    //    public override void Start()
    //    {
    //        Transform = gameObject.Transform;

    //        NormalForcesApplied = Vector2.Zero;
    //        ImpulseForcesApplied = Vector2.Zero;
    //        if(velocity == null)
    //            velocity = Vector2.Zero;
    //        LastPosition = Vector2.Zero;
    //    }

    //    public override void Update(GameTime gameTime)
    //    {
    //        float GameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
    //        LastPosition = Transform.Position;

    //        if (!IsKinematic)
    //        {
    //            if (AffectedByGravity) //here
    //            {
    //                HandyVector.X = 0;
    //                HandyVector.Y = GravityConstant * GravityScale * 100;

    //                AddForce(HandyVector, ForceMode2D.Force);
    //            }

    //            if (AffectedByLinearDrag && velocity.X != 0) //This drag equation should be modified as it's not working properly and should affect all kinds of movements!
    //            {
    //                HandyVector.X = -LinearDrag;
    //                HandyVector.Y = 0;

    //                HandyVector2.X = -QuadraticDrag;
    //                HandyVector2.Y = 0;

    //                AddForce(HandyVector * velocity.X + HandyVector2 * (velocity.X * velocity.X) * (velocity.X / MathCompanion.Abs(velocity.X)), ForceMode2D.Force);
    //            }

    //            ///////////////////////////HorizontalForces////////////////////
    //            if (!VelocityJustAssigned)  //Assigning velocity from outside overrides any type of force(even drag)
    //            {
    //                if (NormalForcesApplied.X != 0)
    //                {
    //                    HandyVector.X = NormalForcesApplied.X * GameTime;
    //                    HandyVector.Y = 0;

    //                    //Transform.Move(NormalForcesApplied.X * GameTime * GameTime, 0);
    //                    velocity += HandyVector;
    //                }

    //                if (ImpulseForcesApplied.X != 0)
    //                {
    //                    HandyVector.X = ImpulseForcesApplied.X;
    //                    HandyVector.Y = 0;

    //                    //Transform.Move(ImpulseForcesApplied.X * GameTime, 0);
    //                    velocity += HandyVector;
    //                }

    //                /////////////VerticalForces//////////////////
    //                if (NormalForcesApplied.Y != 0)
    //                {
    //                    HandyVector.X = 0;
    //                    HandyVector.Y = NormalForcesApplied.Y * GameTime;

    //                    //Transform.Move(0, NormalForcesApplied.Y * GameTime * GameTime);
    //                    velocity += HandyVector;
    //                }

    //                if (ImpulseForcesApplied.Y != 0)
    //                {
    //                    HandyVector.X = 0;
    //                    HandyVector.Y = ImpulseForcesApplied.Y;

    //                    //Transform.Move(0, ImpulseForcesApplied.Y * GameTime);
    //                    velocity += HandyVector;
    //                }
    //            }

    //            if(velocity != Vector2.Zero)
    //                Move(Velocity * GameTime * 100);

    //            if(GameTime != 0)
    //                velocity = (Transform.Position - LastPosition) / GameTime;


    //            //Forces are applied once per frame
    //            NormalForcesApplied = Vector2.Zero;
    //            ImpulseForcesApplied = Vector2.Zero;
    //            VelocityJustAssigned = false;
    //        }
    //    }

    //    public void AddForce(Vector2 Force, ForceMode2D forceMode2D)
    //    {
    //        if (!IsKinematic)
    //        {
    //            if (forceMode2D == ForceMode2D.Force)
    //                NormalForcesApplied += Force;
    //            else if (forceMode2D == ForceMode2D.Impulse)
    //                ImpulseForcesApplied += Force;
    //        }
    //    }

    //    public void Move(Vector2 movement)  //This function is physics related, Collision is checked when used(Not yet implemented -> collision)
    //    {
    //        Transform.Move(movement);
    //    }

    //    public void Move(float x, float y)  //This function is physics related, Collision is checked when used(Not yet implemented -> collision)
    //    {
    //        Transform.Move(x, y);
    //    }
    //}
}
