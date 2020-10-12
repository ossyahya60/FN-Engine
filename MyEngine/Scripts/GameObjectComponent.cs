﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine
{
    public class GameObjectComponent
    {
        public GameObject gameObject;  //Every component belongs to a certain GameObject
        public bool Enabled = true;

        public GameObjectComponent()
        {

        }

        public virtual void Start() //Called once per component
        {
            
        }

        public virtual void Update(GameTime gameTime)  //Called every frame
        {
            
        }

        public virtual void Draw(SpriteBatch spriteBatch)  //Called every frame after "Update()" execution
        {

        }

        public virtual GameObjectComponent DeepCopy()
        {
            return this.MemberwiseClone() as GameObjectComponent;
        }
    }
}
