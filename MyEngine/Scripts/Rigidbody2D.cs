using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MyEngine
{
    public enum ForceMode2D
    {
        Force, //Gradual Increasing Force
        Impulse //Burst Decreasing Force
    }

    public class Rigidbody2D: GameObjectComponent
    {
        public bool AffectedByGravity = true;
        public bool IsKinematic = false;
        public float GravityConstant = 10f;
        public float GravityScale = 1f;
        public float Mass = 1f;  //Will be used in collision response and collision reaction to other rigidbodies
        public bool AffectedByLinearDrag = true;
        public float LinearDrag = 1f;  //Affects drag as a whole, but is cuts Low speeds faster.
        public float QuadraticDrag = 1f;  //Affects drag as a whole, but is cuts High speeds faster.
        public bool ContinousDetection = false; //Turn this on if you want more precise collision detection, but it requires more computational power.
        public Vector2 Velocity{
            set
            {
                velocity = value;
                VelocityJustAssigned = true;
            }
            get
            {
                return velocity;
            }
        }

        private Transform Transform;
        private Vector2 NormalForcesApplied;
        private Vector2 ImpulseForcesApplied;
        private Vector2 LastPosition;
        private Vector2 velocity;
        private bool VelocityJustAssigned = false;

        public Rigidbody2D()
        {
            
        }

        public void ResetHorizVelocity()
        {
            velocity.X = 0;
        }

        public void ResetVerticalVelocity()
        {
            velocity.Y = 0;
        }

        public override void Start()
        {
            Transform = gameObject.Transform;

            NormalForcesApplied = Vector2.Zero;
            ImpulseForcesApplied = Vector2.Zero;
            if(velocity == null)
                velocity = Vector2.Zero;
            LastPosition = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            float GameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            LastPosition = Transform.Position;

            if (!IsKinematic)
            {
                if (AffectedByGravity) //here
                    AddForce(new Vector2(0, GravityConstant * GravityScale), ForceMode2D.Force);

                if (AffectedByLinearDrag && velocity.X != 0) //This drag equation should be modified as it's not working properly and should affect all kinds of movements!
                    AddForce(new Vector2(-LinearDrag, 0) * velocity.X + new Vector2(-QuadraticDrag, 0) * (velocity.X * velocity.X) * (velocity.X / MathCompanion.Abs(velocity.X)), ForceMode2D.Force);

                ///////////////////////////HorizontalForces////////////////////
                if (!VelocityJustAssigned)  //Assigning velocity from outside overrides any type of force(even drag)
                {
                    if (NormalForcesApplied.X != 0)
                    {
                        //Transform.Move(NormalForcesApplied.X * GameTime * GameTime, 0);
                        velocity += new Vector2(NormalForcesApplied.X * GameTime, 0);
                    }

                    if (ImpulseForcesApplied.X != 0)
                    {
                        //Transform.Move(ImpulseForcesApplied.X * GameTime, 0);
                        velocity += new Vector2(ImpulseForcesApplied.X, 0);
                    }

                    /////////////VerticalForces//////////////////
                    if (NormalForcesApplied.Y != 0)
                    {
                        //Transform.Move(0, NormalForcesApplied.Y * GameTime * GameTime);
                        velocity += new Vector2(0, NormalForcesApplied.Y * GameTime);
                    }

                    if (ImpulseForcesApplied.Y != 0)
                    {
                        //Transform.Move(0, ImpulseForcesApplied.Y * GameTime);
                        velocity += new Vector2(0, ImpulseForcesApplied.Y);
                    }
                }

                if(velocity != Vector2.Zero)
                    Move(Velocity * GameTime);

                if(GameTime != 0)
                    velocity = (Transform.Position - LastPosition) / GameTime;


                //Forces are applied once per frame
                NormalForcesApplied = Vector2.Zero;
                ImpulseForcesApplied = Vector2.Zero;
                VelocityJustAssigned = false;
            }
        }

        public void AddForce(Vector2 Force, ForceMode2D forceMode2D)
        {
            if (!IsKinematic)
            {
                if (forceMode2D == ForceMode2D.Force)
                    NormalForcesApplied += Force;
                else if (forceMode2D == ForceMode2D.Impulse)
                    ImpulseForcesApplied += Force;
            }
        }

        public void Move(Vector2 movement)  //This function is physics related, Collision is checked when used(Not yet implemented -> collision)
        {
            Transform.Move(movement);
        }

        public void Move(float x, float y)  //This function is physics related, Collision is checked when used(Not yet implemented -> collision)
        {
            Transform.Move(x, y);
        }
    }
}
