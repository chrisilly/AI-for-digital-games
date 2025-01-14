using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_for_digital_games
{
    public class AgentHandler
    {
        List<Agent> agents = new List<Agent>();
        IBehaviourSystem brain = new NeuralNetworkBrain();
        int dummySpawnInterval = Game1.random.Next(1000, 5000);
        float dummySpawnTimer = 0;
        int agentCap = 10;

        Agent special;

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

                    if (agent.IsColliding(other))
                    {
                        if (agent.IsBiggerThan(other))
                            agent.Eat(other);
                        else if (other.IsBiggerThan(agent))
                            other.Eat(agent);
                    }
                }

                agent.Update();
                agent.Brain.Update(this, agent, gameTime);
            }

            if (agents.Count < agentCap)
                SpawnDummies(gameTime);
        }

        public void SpawnSpecialAgent(IBehaviourSystem brain)
        {
            if(special != null)
                agents.Remove(special);

            special = new Agent(brain, Color.White, 2f);
            AddAgent(special);
        }

        // Spawns dummy agents on a timer with random intervals
        public void SpawnDummies(GameTime gameTime)
        {
            dummySpawnTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (dummySpawnTimer >= dummySpawnInterval)
            {
                float randomSize = (float)Game1.random.NextDouble();
                AddAgent(new Agent(brain, Color.Red, randomSize));
                Debug.WriteLine("Spawned new dummy.");
                dummySpawnInterval = Game1.random.Next(1000, 5000);
                dummySpawnTimer = 0;
            }
        }

        public void FleeDanger(Agent subject)
        {
            subject.SteerFromDanger(GetNearestThreat(subject));
        }

        public void ChaseFood(Agent subject)
        {
            subject.SteerToTarget(GetNearestFood(subject));
        }

        Agent GetNearestFood(Agent subject)
        {
            Agent nearestAgent = null;
            float nearestDistance = (float)Math.Sqrt(Math.Pow(Game1.ScreenWidth, 2) + Math.Pow(Game1.ScreenHeight, 2));

            foreach (Agent other in agents)
            {
                if (other == subject) continue;

                float distance = Vector2.Distance(subject.Position, other.Position);

                if (distance < nearestDistance && subject.IsBiggerThan(other))
                {
                    nearestAgent = other;
                    nearestDistance = distance;
                }
            }

            return nearestAgent;
        }

        Agent GetNearestThreat(Agent subject)
        {
            Agent nearestAgent = null;
            float nearestDistance = (float)Math.Sqrt(Math.Pow(Game1.ScreenWidth, 2) + Math.Pow(Game1.ScreenHeight, 2));

            foreach (Agent other in agents)
            {
                if (other == subject) continue;

                float distance = Vector2.Distance(subject.Position, other.Position);

                if (distance < nearestDistance && other.IsBiggerThan(subject))
                {
                    nearestAgent = other;
                    nearestDistance = distance;
                }
            }

            return nearestAgent;
        }

        public bool PreysNearby(Agent subject)
        {
            foreach (Agent other in agents)
            {
                if (other == subject) continue;

                if(subject.IsNearPrey(other))
                    return true;
            }

            return false;
        }

        public bool ThreatsNearby(Agent subject)
        {
            foreach (Agent other in agents)
            {
                if (other == subject) continue;

                if(subject.IsNearThreat(other))
                    return true;
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Agent agent in agents)
            {
                agent.Draw(spriteBatch);
            }

            special.DrawDebug(spriteBatch);
        }

        public void AddAgent(Agent agent)
        {
            agents.Add(agent);
        }
    }
}
