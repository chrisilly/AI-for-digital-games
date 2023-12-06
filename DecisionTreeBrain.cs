using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_for_digital_games
{
    internal class DecisionTreeBrain : IBehaviourSystem
    {
        DecisionTree decisionTree;

        public DecisionTreeBrain()
        {
            decisionTree = new DecisionTree();
        }

        public void Update(AgentHandler handler, Agent body, GameTime gameTime)
        {
            body.Move(gameTime);

            decisionTree.ParseTree(handler, body);
        }
    }
}
