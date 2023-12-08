using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AI_for_digital_games
{
    internal class FiniteStateMachineBrain : IBehaviourSystem
    {
        public void Update(AgentHandler handler, Agent body, GameTime gameTime)
        {
            body.Move(gameTime);
            body.State.Handle(handler, body);
            body.State.Update(handler, body, gameTime);
        }
    }
}
