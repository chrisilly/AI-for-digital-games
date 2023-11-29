using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace AI_for_digital_games
{
    public class Agent
    {
        IBehaviourSystem brain;
        Texture2D texture;
        Rectangle hitbox;

        Vector2 position;
        Vector2 direction;
        float speed;

        public Agent(IBehaviourSystem brain, Texture2D texture)
        {
            this.brain = brain;
            this.texture = texture;
            hitbox = new Rectangle(0, 0, texture.Width, texture.Height);

            position = Vector2.Zero;
            direction = new Vector2(0, 1);
            speed = 5f;
        }

        public void Update(GameTime gameTime)
        {
            brain.Update(this);
        }

        public void Move(Vector2 direction)
        {
            position += direction * speed /** gameTime.ElapsedGameTime.Milliseconds*/;
            //Debug.WriteLine(position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
            
            // Draw hitbox
            spriteBatch.Draw(texture, hitbox, Color.Green * 0.3f);
        }
    }
}
