using System.Collections.Generic;

namespace MyEngine.Scripts
{
    internal class CollisionHandler
    {
        public void Update()
        {
            List<GameObjectComponent> Colliders = new List<GameObjectComponent>();

            foreach (GameObject GO in SceneManager.ActiveScene.GameObjects)
                foreach (GameObjectComponent GOC in GO.GameObjectComponents)
                    if (GOC is Collider2D)
                        Colliders.Add(GOC);

            foreach(var collider in Colliders)
            {
                foreach (var collider2 in Colliders)
                {
                    if(collider != collider2)
                    {
                        if(((Collider2D)collider).IsTouching((Collider2D)collider2))
                        {
                            //((Collider2D)collider).Resolve(collider2);
                        }
                    }
                }
            }    
        }
    }
}
