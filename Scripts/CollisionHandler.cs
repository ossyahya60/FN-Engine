using Microsoft.Xna.Framework;
using System;
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
                foreach (var collider2 in Colliders)
                {
                    if (collider != collider2)
                    {
                        var CD1 = collider as Collider2D;
                        var CD2 = collider2 as Collider2D;
                        var CallerRigidBody = collider.gameObject.GetComponent<Rigidbody2D>();

                        if (CallerRigidBody != null && CallerRigidBody.BodyType == BodyType.Dynamic && !CD1.IsTrigger() && !CD2.IsTrigger() && CD1.CollisionDetection(CD2, false)) //We call the resolution two times for more stable resolutions
                            CD1.CollisionResponse(CallerRigidBody, CD2, (float)gameTime.ElapsedGameTime.TotalSeconds, ref Colliders);
                    }
                }
            }
        }
    }
}