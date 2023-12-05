using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using MonoGame.Extended;

namespace AI_for_digital_games
{
    public class Agent
    {
        const float defaultSpeed = 0.05f;
        const float chaseSpeed = defaultSpeed * 1.5f;

        IBehaviourSystem brain;

        Texture2D texture;
        Vector2 origin;
        Color color;
        float scale;
        Rectangle hitbox;

        Vector2 position;
        public Vector2 Position { get { return new Vector2(hitbox.Center.X, hitbox.Center.Y); } }
        Vector2 direction;
        float speed;
        public Vector2 Velocity { get { return direction * speed; } }
        public bool Alive { get; private set; } = true;

        public Agent(IBehaviourSystem brain)
        {
            this.brain = brain;

            this.texture = Game1.texture;
            this.color = Color.White;
            this.scale = 1f;

            position = Game1.GetRandomPosition();
            //direction = Vector2.Zero;
            direction = Game1.GetRandomDirection();
            speed = defaultSpeed;

            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            hitbox = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public Agent(IBehaviourSystem brain, Color color, float scale = 1f) : this(brain)
        {
            this.color = color;
            this.scale = scale;
            origin.X *= scale / 2;
            origin.Y *= scale / 2;
            //origin = new Vector2(texture.Width * scale / 2, texture.Height * scale / 2);
            hitbox.Width = (int)(texture.Width * scale);
            hitbox.Height = (int)(texture.Height * scale);
            speed = defaultSpeed / scale;
        }

        public void Update(GameTime gameTime)
        {
            //brain.Update(this);
            Move(gameTime);

            if (IsOutOfBounds(500))
                Die();

            //Wander();
        }

        public void Move(GameTime gameTime)
        {
            if (direction == Vector2.Zero)
                return;
            if(IsObstacled())
                SteerAwayFromObstacles();

            direction.Normalize();
            position += direction * speed * gameTime.ElapsedGameTime.Milliseconds;

            //update hitbox
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;

            //Debug.WriteLine(position);
        }

        public void Idle()
        {
            direction = Vector2.Zero;
        }

        public void Wander()
        {
            // get orthogonal vector of the direction
            Vector2 directionNormal = new Vector2(direction.Y, -direction.X);

            // go in circles?
            direction = direction + directionNormal/100;
        }

        public void Chase(Agent target)
        {
            // direction = targetPosition - position + targetVelocity;
            direction = target.position - position + target.Velocity;
        }

        public void Flee(Agent threat)
        {
            // direction = dangerVelocity + dangerVelocityNormal;
            direction = threat.Velocity + new Vector2(threat.Velocity.Y, -threat.Velocity.X) /* * fleeSpeed */;
        }

        public void Eat(Agent victim)
        {
            scale += (victim.scale/10) * (1/scale);
            hitbox.Width = (int)(texture.Width * scale);
            hitbox.Height = (int)(texture.Height * scale);
            victim.Die();
        }

        public bool IsColliding(Agent other)
        {
            return hitbox.Intersects(other.hitbox);
        }

        public bool IsBiggerThan(Agent other)
        {
            return scale > other.scale;
        }

        public bool IsCloseToOutOfBounds()
        {
            int threshHold = 100 /*+ hitbox.Width / 5*/;

            return DistanceToNearestEdge() < threshHold;
        }

        public void Die()
        {
            Alive = false;
        }

        public bool IsOutOfBounds(int offset = 0)
        {
            return Position.X < 0 - offset || Position.X > Game1.ScreenWidth + offset || Position.Y < 0 - offset || Position.Y > Game1.ScreenHeight + offset;
        }

        (float left, float right, float top, float bottom) GetDistanceToEdges()
        {
            float left = Position.X;
            float right = Game1.ScreenWidth - Position.X;
            float top = Position.Y;
            float bottom = Game1.ScreenHeight - Position.Y;

            return (left, right, top, bottom);
        }

        Vector2 GetTurnDirection()
        {
            // Determing which edge is closest
            (float left, float right, float top, float bottom) = GetDistanceToEdges();
            float nearestEdge = Math.Min(Math.Min(left, right), Math.Min(top, bottom));

            Vector2 turnDirection = new Vector2(direction.Y, -direction.X) / DistanceToNearestEdge();

            if (left == nearestEdge)
                turnDirection *= direction.Y >= 0 ? 1 : -1;
            else if (right == nearestEdge)
                turnDirection *= direction.Y >= 0 ? -1 : 1;
            else if (bottom == nearestEdge)
                turnDirection *= direction.X >= 0 ? 1 : -1;
            else if (top == nearestEdge)
                turnDirection *= direction.X >= 0 ? -1 : 1;

            return turnDirection;
        }

        public float DistanceToNearestEdge()
        {
            float distanceToBottom = Game1.ScreenHeight - (Position.Y + hitbox.Height / 2);
            float distanceToTop =   Position.Y - hitbox.Height / 2;
            float distanceToLeft =  Position.X - hitbox.Width / 2;
            float distanceToRight = Game1.ScreenWidth - (Position.X + hitbox.Width / 2);

            return Math.Min(distanceToLeft, Math.Min(distanceToRight, Math.Min(distanceToTop, distanceToBottom)));
        }

        bool IsObstacled()
        {
            if(IsOutOfBounds()) return true;
            if(IsCloseToOutOfBounds()) return true;

            return false;
        }

        public void SteerAwayFromObstacles()
        {
            if (IsOutOfBounds())
                direction = Game1.ScreenCenter - Position;
            else if(IsCloseToOutOfBounds())
                // turn proportionally to the distance to the nearest edge
                direction += GetTurnDirection();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(texture, position, null, Color.Yellow * 0.2f, 0f, origin, scale, SpriteEffects.None, 0);

            // Draw null origin
            //spriteBatch.Draw(texture, position, null, Color.Yellow * 0.3f, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);

            // Draw hitbox
            spriteBatch.Draw(texture, hitbox, color);
            //spriteBatch.Draw(texture, hitbox, null, Color.DarkGreen * 0.3f, 0f, origin, SpriteEffects.None, 0);
        }
    }
}
