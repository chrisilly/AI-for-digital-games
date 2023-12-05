using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AI_for_digital_games
{
    internal class DecisionTree
    {
        Node root;

        /// <summary>
        /// Empty decision tree
        /// </summary>
        public DecisionTree()
        {
            this.root = new Node();
        }

        /// <summary>
        /// Decision tree for agents who act solely on hunger (Outdated)
        /// </summary>
        /// <param name="OnFoodNearby"></param>
        /// <param name="OnFoodOnSubject"></param>
        /// <param name="Default"></param>
        public DecisionTree(Action<AgentHandler> OnFoodNearby, Action<AgentHandler> OnFoodOnSubject, Action<AgentHandler> Default)
        {
            this.root = new Node
            {
                //condition = (agentHandler, subject) => agentHandler.FoodOnSubject(subject),
                @true = new Node
                {
                    action = OnFoodOnSubject
                },
                @false = new Node
                {
                    condition = (agentHandler, subject) => agentHandler.FoodNearby(subject),
                    @true = new Node
                    {
                        action = OnFoodNearby
                    },
                    @false = new Node
                    {
                        action = Default
                    }
                }
            };
        }

        internal class Node
        {
            public Func<AgentHandler, Agent, bool> condition;
            public Node @true;
            public Node @false;
            public Action<AgentHandler> action;
        }

        public void ParseTree(Node node, AgentHandler agentHandler, Agent subject)
        {
            bool nodeIsLeaf = node.@true == null && node.@false == null;

            if(nodeIsLeaf)
            {
                node.action(agentHandler);
                return;
            }

            if(node.condition(agentHandler, subject))
            {
                ParseTree(node.@true, agentHandler, subject);
            }
            else
            {
                ParseTree(node.@false, agentHandler, subject);
            }
        }

        public void ParseTree(AgentHandler agentHandler, Agent subject)
        {
            ParseTree(root, agentHandler, subject);
        }

        #region Condition Variable Test
        //static Random random = new Random();

        //public Node testNode = new Node
        //{
        //    //condition = (body) => random.Next(2) == 1,
        //    condition = (body) => body.FoodOnMe(),
        //};

        //bool TryNodeCondition(Node node)
        //{
        //    return node.condition();
        //}
        #endregion
    }
}
