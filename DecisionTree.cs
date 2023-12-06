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
        //public DecisionTree()
        //{
        //    this.root = new Node();
        //}

        public DecisionTree()
        {
            this.root = new Node
            {
                condition = (handler, subject) => handler.ThreatsNearby(subject),
                @true = new Node
                {
                    action = (handler, subject) => handler.FleeDanger(subject)
                },
                @false = new Node
                {
                    condition = (handler, subject) => handler.PreysNearby(subject),
                    @true = new Node
                    {
                        action = (handler, subject) => handler.ChaseFood(subject)
                    },
                    @false = new Node
                    {
                        action = (handler, subject) => subject.Patrol()
                    }
                }
            };
        }

        internal class Node
        {
            public Func<AgentHandler, Agent, bool> condition;
            public Node @true;
            public Node @false;
            public Action<AgentHandler, Agent> action;
        }

        public void ParseTree(Node node, AgentHandler agentHandler, Agent subject)
        {
            bool nodeIsLeaf = node.@true == null && node.@false == null;

            if(nodeIsLeaf)
            {
                node.action(agentHandler, subject);
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
    }
}
