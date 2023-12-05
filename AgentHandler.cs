using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_for_digital_games
{
    internal class AgentHandler
    {
        List<Agent> agents = new List<Agent>();
        int dummySpawnInterval = Game1.random.Next(1000, 5000);
        float timer = 0;
        int agentCap = 10;

        public void Update(GameTime gameTime)
        {
            foreach (Agent agent in agents.ToList())
            {
                // Stop updating dead agents
                if(!agent.Alive)
                {
                    agents.Remove(agent);
                    continue;
                }

                // Handle collisions
                foreach (Agent other in agents.ToList())
                {
                    if (agent == other) continue;

                    if(agent.IsColliding(other))
                    {
                        if (agent.IsBiggerThan(other))
                            agent.Eat(other);
                        else if(other.IsBiggerThan(agent))
                            other.Eat(agent);
                    }
                }

                agent.Update(gameTime);
            }

            if (agents.Count < agentCap)
                SpawnDummies(gameTime);
        }

        // Spawns dummy agents on a timer with random intervals
        public void SpawnDummies(GameTime gameTime)
        {           
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (timer >= dummySpawnInterval)
            {
                AddAgent(new Agent(new BrainDead(), Color.Red, 0.3f));
                Debug.WriteLine("Spawned new dummy.");
                dummySpawnInterval = Game1.random.Next(1000, 5000);
                timer = 0;
            }
        }

        public void MoveToNearestFood(Agent subject)
        {
            Agent nearestAgent = GetNearestFood(subject);

            if (nearestAgent != null)
            {
                //subject.MoveTo(nearestAgent.Position);
            }
        }

        private Agent GetNearestFood(Agent subject)
        {
            Agent nearestAgent = null;
            float nearestDistance = (float)Math.Sqrt(Math.Pow(Game1.ScreenWidth, 2) + Math.Pow(Game1.ScreenHeight, 2));

            foreach (Agent agent in agents)
            {
                if (agent == subject) continue;

                float distance = Vector2.Distance(agent.Position, subject.Position);

                if (distance < nearestDistance)
                {
                    nearestAgent = agent;
                    nearestDistance = distance;
                }
            }

            return nearestAgent;
        }

        public bool FoodNearby(Agent subject)
        {
            foreach (Agent agent in agents)
            {
                if (agent == subject) continue;

                if (Vector2.Distance(agent.Position, subject.Position) < 500)
                {
                    //subject.MoveTo(agent.Position);
                    return true;
                }
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Agent agent in agents)
            {
                agent.Draw(spriteBatch);
            }
        }

        public void AddAgent(Agent agent)
        {
            agents.Add(agent);
        }
    }
}
