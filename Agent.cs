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
        const float defaultSpeed = 0.15f;
        const float chaseSpeed = defaultSpeed * 1.5f;
        const float fleeSpeed = defaultSpeed * 1.5f;

        int threatDetectionRange = 60;
        int foodDetectionRange = 40;

        float DefaultSpeed { get { return defaultSpeed / (float)Math.Sqrt(scale); } }

        public IBehaviourSystem Brain { get; private set; }

        public IState State { get; set; }

        Texture2D texture;
        Vector2 origin;
        Color color;
        float scale;
        Rectangle hitbox;

        Vector2 position;
        public Vector2 Position { get { return new Vector2(hitbox.Center.X, hitbox.Center.Y); } }
        Vector2 TopCenter { get { return new Vector2(hitbox.Center.X, hitbox.Top); } }
        Vector2 BottomCenter { get { return new Vector2(hitbox.Center.X, hitbox.Bottom); } }
        Vector2 LeftCenter { get { return new Vector2(hitbox.Left, hitbox.Center.Y); } }
        Vector2 RightCenter { get { return new Vector2(hitbox.Right, hitbox.Center.Y); } }

        Vector2 direction;
        float speed;
        public Vector2 Velocity { get { return direction * speed; } }

        public bool Alive { get; private set; } = true;

        public Agent(IBehaviourSystem brain)
        {
            this.Brain = brain;
            this.State = new PatrollingState();

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
            speed = DefaultSpeed;
        }

        public void Update()
        {
            SteerFromWalls();
        }

        public void Move(GameTime gameTime)
        {
            if (direction == Vector2.Zero)
                return;
            else if (direction.Length() > 1 /*|| !IsObstacled()*/)
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

        public void Patrol()
        {
            speed = DefaultSpeed;
        }

        #region Steering
        public void SteerToTarget(Agent target)
        {
            speed = chaseSpeed;
            direction += target.Position - Position + (target.Velocity * 2f);
        }

        public void SteerFromDanger(Agent threat)
        {
            speed = fleeSpeed;
            //direction = threat.Velocity + new Vector2(threat.Velocity.Y, -threat.Velocity.X);
            direction +=  (Position - threat.Position) / ((float)Math.Pow(Vector2.Distance(Position, threat.Position), 1.5f) + 1) ;
        }

        public void SteerFromWalls()
        {
            if (IsOutOfBounds())
                direction += Game1.ScreenCenter - Position;
            else if(IsCloseToOutOfBounds())
                direction += OpposingForce();
        }
        #endregion

        public void Eat(Agent victim)
        {
            scale += (victim.scale/10) * (1/(scale + 1));
            hitbox.Width = (int)(texture.Width * scale);
            hitbox.Height = (int)(texture.Height * scale);
            victim.Die();
        }

        public void Die()
        {
            Alive = false;
        }

        public bool IsNear(Agent other, float proximityThreshold)
        {
            if (ShortestDistanceTo(other) < proximityThreshold)
            {
                return true;
            }

            return false;
        }

        public bool IsNearThreat(Agent other)
        {
            return IsNear(other, threatDetectionRange) && other.IsBiggerThan(this);
        }

        public bool IsNearPrey(Agent other)
        {
            return IsNear(other, foodDetectionRange) && IsBiggerThan(other);
        }

        public float ShortestDistanceTo(Agent other)
        {
            return Vector2.Distance(other.GetNearestPoint(this), GetNearestPoint(other));
        }

        // Clamp a value between a minimum and maximum value
        Vector2 GetNearestPoint(Agent other)
        {
            Vector2 otherNearestPoint = Vector2.Zero;

            if (other.Position.X < hitbox.Left)
                otherNearestPoint.X = hitbox.Left;
            else if (other.Position.X > hitbox.Right)
                otherNearestPoint.X = hitbox.Right;
            else
                otherNearestPoint.X = other.Position.X;

            if (other.Position.Y < hitbox.Top)
                otherNearestPoint.Y = hitbox.Top;
            else if (other.Position.Y > hitbox.Bottom)
                otherNearestPoint.Y = hitbox.Bottom;
            else
                otherNearestPoint.Y = other.Position.Y;

            return otherNearestPoint;
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
            int threshHold = 100;

            return DistanceToNearestEdge() < threshHold;
        }

        public bool IsOutOfBounds(int offset = 0)
        {
            return LeftCenter.X < 0 - offset || RightCenter.X > Game1.ScreenWidth + offset || TopCenter.Y < 0 - offset || BottomCenter.Y > Game1.ScreenHeight + offset;
        }

        (float left, float right, float top, float bottom) GetDistanceToEdges()
        {
            float left = LeftCenter.X;
            float right = Game1.ScreenWidth - RightCenter.X;
            float top = TopCenter.Y;
            float bottom = Game1.ScreenHeight - BottomCenter.Y;

            return (left, right, top, bottom);
        }

        /// <returns>The opposing force of the nearest wall</returns>
        Vector2 OpposingForce()
        {
            // Determing which edge is closest
            (float left, float right, float top, float bottom) = GetDistanceToEdges();
            float nearestEdge = Math.Min(Math.Min(left, right), Math.Min(top, bottom));

            //Vector2 turnDirection = new Vector2(direction.Y, -direction.X) / DistanceToNearestEdge();
            Vector2 opposingForce = Vector2.Zero;

            if (left <= nearestEdge)
            {
                opposingForce = new Vector2(1, 0);
            }
            else if (right <= nearestEdge)
            {
                opposingForce = new Vector2(-1, 0);
            }
            else if (bottom <= nearestEdge)
            {
                opposingForce = new Vector2(0, -1);
            }
            else if (top <= nearestEdge)
            {
                opposingForce = new Vector2(0, 1);
            }

            return opposingForce / (DistanceToNearestEdge() + 1);
        }

        public float DistanceToNearestEdge()
        {
            float distanceToBottom = Game1.ScreenHeight - BottomCenter.Y;
            float distanceToTop =   TopCenter.Y;
            float distanceToLeft =  LeftCenter.X;
            float distanceToRight = Game1.ScreenWidth - RightCenter.X;

            return Math.Min(distanceToLeft, Math.Min(distanceToRight, Math.Min(distanceToTop, distanceToBottom)));
        }

        public bool IsObstacled()
        {
            if(IsOutOfBounds()) return true;
            if(IsCloseToOutOfBounds()) return true;

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(texture, position, null, Color.Yellow * 0.2f, 0f, origin, scale, SpriteEffects.None, 0);

            // Draw without origin
            //spriteBatch.Draw(texture, position, null, Color.Yellow * 0.3f, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);

            // Draw hitbox
            spriteBatch.Draw(texture, hitbox, color);
        }

        public void DrawDebug(SpriteBatch spriteBatch)
        {
            // Draw direction
            spriteBatch.DrawLine(Position, Position + (direction * 100f), Color.Blue * 0.5f);

            // Draw distance to nearest edge
            spriteBatch.DrawLine(Position, Position + (OpposingForce() * 100f), Color.Yellow * 0.2f);

            // Draw threat detection range
            //spriteBatch.DrawCircle(Position, threatDetectionRange, 20, Color.Red * 0.2f);
            spriteBatch.DrawRectangle(hitbox.X - threatDetectionRange, hitbox.Y - threatDetectionRange, hitbox.Width + threatDetectionRange * 2, hitbox.Height + threatDetectionRange * 2, Color.Red * 0.2f);

            // Draw food detection range
            //spriteBatch.DrawCircle(Position, foodDetectionRange, 20, Color.Green * 0.2f);
            spriteBatch.DrawRectangle(hitbox.X - foodDetectionRange, hitbox.Y - foodDetectionRange, hitbox.Width + foodDetectionRange * 2, hitbox.Height + foodDetectionRange * 2, Color.Green * 0.2f);
        }
    }
}
