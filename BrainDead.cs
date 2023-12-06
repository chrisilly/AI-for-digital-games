using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AI_for_digital_games
{
    internal class BrainDead : IBehaviourSystem
    {
        public void Update(AgentHandler handler, Agent body, GameTime gameTime)
        {
            body.Idle();
        }

        public void Decide(Agent body)
        {
            
        }
    }
}
