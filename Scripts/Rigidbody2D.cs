using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
        public bool AffectedByGravity = true;
        public bool ConstraintHorizontalMovement = false;
        public bool ConstraintVerticalMovement = false;
        public BodyType BodyType = BodyType.Dynamic;
        public float Restitution = 1;
        public float LinearDragScale = 1; //This ranges from 0 to 1 to indicate how much the rigidbody should be affected by linear drag
        public float GravityScale = 1; //This ranges from 0 to 1 to indicate how much the rigidbody should be affected by gravity
        public float Mass = 1; //Might be used for collision response and other physics-related stuff
        public Vector2 Velocity; // 1 pixel per sec => This shouldn't be set every frame as it negates all other forces acting

        internal Dictionary<Collider2D, List<Collider2D>> LastFrameCollisionList = new Dictionary<Collider2D, List<Collider2D>>();

        private readonly float GravityConstant = 10; //Universal gravitational constant (Approximated to 10 for easiness)
        private float DeltaTime = 1.0f / 60;

        public override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (BodyType)
            {
                case BodyType.Static: //Not affected by velocity or forces
                    {

                    }
                    break;
                case BodyType.Dynamic: //affected by everything :D =>This one checks collision only
                    {
                        if (AffectedByGravity)
                            Velocity.Y += GravityConstant * GravityScale;
                    }
                    break;
                case BodyType.Kinematic: //not affected by forces
                    {

                    }
                    break;
                default:
                    break;
            }

            if (BodyType != BodyType.Static)
            {
                Velocity -= LinearDragScale * DeltaTime * Velocity;

                if (ConstraintHorizontalMovement)
                    Velocity = new Vector2(0, Velocity.Y);
                if (ConstraintVerticalMovement)
                    Velocity = new Vector2(Velocity.X, 0);

                gameObject.Transform.Position += Velocity * DeltaTime;
            }
        }

        public void AddForce(Vector2 Value, ForceMode2D Mode = ForceMode2D.Force)
        {
            if (BodyType == BodyType.Dynamic) //Forces affect dynamic rigidbodies only
            {
                switch (Mode)
                {
                    case ForceMode2D.Force:
                        // Force = Mass * Acceleration, => F = Mass * dV/dt, then dV = F * dt / Mass
                        Velocity += Value * DeltaTime / Mass;
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

        public override GameObjectComponent DeepCopy(GameObject Clone)
        {
            Rigidbody2D clone = this.MemberwiseClone() as Rigidbody2D;
            clone.gameObject = Clone;

            return clone;
        }
    }
}
