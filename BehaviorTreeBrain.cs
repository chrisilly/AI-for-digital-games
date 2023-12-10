using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AI_for_digital_games
{
    internal class BehaviorTreeBrain : IBehaviourSystem
    {
        BehaviorTree behaviorTree;

        public BehaviorTreeBrain()
        {
            behaviorTree = new BehaviorTree();
        }

        public void Update(AgentHandler handler, Agent body, GameTime gameTime)
        {
            if(!behaviorTree.IsTreeGenerated())
                behaviorTree.GenerateTree(handler, body);

            body.Move(gameTime);

            behaviorTree.Tick();
        }
    }
}
