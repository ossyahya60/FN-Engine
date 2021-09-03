﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FN_Engine
{
    internal static class CollisionHandler
    {
        static public void Update(GameTime gameTime)
        {
            List<GameObjectComponent> Colliders = new List<GameObjectComponent>();

            foreach (GameObject GO in SceneManager.ActiveScene.GameObjects)
            {
                if (!GO.Active)
                    continue;

                foreach (GameObjectComponent GOC in GO.GameObjectComponents)
                {
                    if (!GOC.Enabled)
                        continue;

                    if (GOC is Collider2D)
                        Colliders.Add(GOC);
                }
            }

            foreach (var collider in Colliders)
            {
                var CallerRigidBody = collider.gameObject.GetComponent<Rigidbody2D>();

                if (CallerRigidBody == null || !CallerRigidBody.Enabled) //Condition for any event related to collision to happen
                    continue;

                HashSet<Collider2D> LastFrameCollisions = CallerRigidBody.CollidedWithLastFrameList();

                foreach (var collider2 in Colliders)
                {
                    if (collider.gameObject.Name.Equals(collider2.gameObject.Name)) //To avoid collision within the same gameobject
                        continue;

                    var CD1 = collider as Collider2D;
                    var CD2 = collider2 as Collider2D;

                    bool Collided = CD1.CollisionDetection(CD2, false);
                    bool CD1Trigger = CD1.IsTrigger();
                    bool CD2Trigger = CD2.IsTrigger();

                    if (CallerRigidBody.BodyType == BodyType.Dynamic && !CD1Trigger && !CD2Trigger && Collided) //We call the resolution two times for more stable resolutions
                    {
                        //This approach suits moving vs non moving object, otherwise it will break!
                        Vector2 CollisionPos = CallerRigidBody.gameObject.Transform.Position;
                        CallerRigidBody.gameObject.Transform.Position -= CallerRigidBody.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        CD1.CollisionResponse(CallerRigidBody, CD2, (float)gameTime.ElapsedGameTime.TotalSeconds, ref Colliders, CollisionPos, true);
                    }

                    if (Collided)
                    {
                        bool JustAdded = false;
                        if (CD1Trigger)
                        {
                            if (LastFrameCollisions.Contains(CD2)) //OnTriggerStay CD1
                                collider.OnTriggerStay2D(CD2);
                            else
                            {
                                collider.OnTriggerEnter2D(CD2); //OnTriggerEnter CD1
                                LastFrameCollisions.Add(CD2);
                                JustAdded = true;
                            }
                        }
                        else if(!CD2Trigger) //Collidable
                        {
                            if (LastFrameCollisions.Contains(CD2)) //OnCollisionStay CD1
                                collider.OnCollisionStay2D(CD2);
                            else
                            {
                                collider.OnCollisionEnter2D(CD2); //OnCollisionEnter CD1
                                LastFrameCollisions.Add(CD2);

                                //(collider as BoxCollider2D).Entered++;
                                JustAdded = true;
                            }
                        }

                        if(CD2Trigger)
                        {
                            if(LastFrameCollisions.Contains(CD2) && !JustAdded) //OnTriggerStay CD2
                                collider2.OnTriggerStay2D(CD1);
                            else
                            {
                                collider2.OnTriggerEnter2D(CD1); //OnTriggerEnter CD2

                                if (!JustAdded)
                                    LastFrameCollisions.Add(CD2);
                            }
                        }
                        else if (!CD1Trigger) //Collidable
                        {
                            if (LastFrameCollisions.Contains(CD2) && !JustAdded) //OnCollisionStay CD2
                                collider2.OnCollisionStay2D(CD1);
                            else
                            {
                                collider2.OnCollisionEnter2D(CD1); //OnCollisionEnter CD2

                                if (!JustAdded)
                                    LastFrameCollisions.Add(CD2);
                            }
                        }
                    }
                    else
                    {
                        bool JustRemoved = false;
                        if (CD1Trigger) //OnTriggerExit CD1
                        {
                            if (LastFrameCollisions.Contains(CD2))
                            {
                                collider.OnTriggerExit2D(CD2);
                                LastFrameCollisions.Remove(CD2);
                                JustRemoved = true;
                            }
                        }
                        else if(!CD2Trigger && LastFrameCollisions.Contains(CD2)) //OnCollisionExit CD1
                        {
                            collider.OnCollisionExit2D(CD2);

                            var CollidedAgaistRB = collider2.gameObject.GetComponent<Rigidbody2D>();

                            LastFrameCollisions.Remove(CD2);
                            JustRemoved = true;
                            //(collider as BoxCollider2D).Exited++;
                        }

                        if (CD2Trigger) //OnTriggerExit CD2
                        {
                            if (JustRemoved || LastFrameCollisions.Contains(CD2))
                            {
                                collider2.OnTriggerExit2D(CD1);

                                if (!JustRemoved)
                                    LastFrameCollisions.Remove(CD2);
                            }
                        }
                        else if (!CD1Trigger && (JustRemoved || LastFrameCollisions.Contains(CD2))) //OnCollisionExit CD2
                        {
                            collider2.OnCollisionExit2D(CD1);

                            if (!JustRemoved)
                                LastFrameCollisions.Remove(CD2);
                        }
                    }
                }
            }
        }
    }
}