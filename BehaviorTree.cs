using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_for_digital_games
{
    internal class BehaviorTree
    {
        Selector root;

        /// <summary>
        /// Manual pre-defined behavior tree
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="subject">The agent who the brain should control</param>
        public BehaviorTree()
        {
            
        }

        abstract class Task
        {
            public enum Status { Running, Success, Failure };
            public Status taskStatus { get; }
            public abstract Status Tick();
        }

        class Action : Task
        {
            Action<AgentHandler, Agent> action;
            AgentHandler handler;
            Agent subject;

            public Action(Action<AgentHandler, Agent> action, AgentHandler handler, Agent subject)
            {
                this.action = action;
                this.handler = handler;
                this.subject = subject;
            }

            public override Status Tick()
            {
                bool succeeded = true;
                bool failed = false;
                bool HandleStubb()
                {
                    action(handler, subject); 
                    return true;
                }

                if (HandleStubb() == succeeded)
                    return Status.Success;
                else if(HandleStubb() == failed)
                    return Status.Failure;
                else
                    return Status.Running;
            }
        }

        class Condition : Task
        {
            AgentHandler handler;
            Agent subject;

            public Condition(AgentHandler handler, Agent subject)
            {
                this.handler = handler;
                this.subject = subject;
            }

            public Func<AgentHandler, Agent, bool> condition;
            public override Status Tick()
            {
                if (condition(handler, subject))
                    return Status.Success;
                else
                    return Status.Failure;
            }
        }

        class Sequence : Task
        {
            public Task[] children;
            public override Status Tick()
            {
                foreach (Task child in children)
                {
                    if (child.Tick() == Status.Failure)
                        return Status.Failure;
                }
                return Status.Success;
            }
        }

        class Selector : Task
        {
            public Task[] children;
            public override Status Tick()
            {
                foreach (Task child in children)
                {
                    if (child.Tick() != Status.Failure)
                        return child.taskStatus;
                }

                return Status.Failure;
            }
        }

        public void GenerateTree(AgentHandler handler, Agent subject)
        {
            #region tree node definitions
            Sequence chasePrey = new Sequence
            {
                children = new Task[]
                {
                    new Condition(handler, subject)
                    {
                        condition = (handler, subject) => !handler.ThreatsNearby(subject)
                    },
                    new Condition(handler, subject)
                    {
                        condition = (handler, subject) => handler.PreysNearby(subject)
                    },
                    new Action((handler, subject) => handler.ChaseFood(subject), handler, subject)
                }
            };

            Sequence fleeDanger = new Sequence
            {
                children = new Task[]
                {
                    new Condition(handler, subject)
                    {
                        condition = (handler, subject) => handler.ThreatsNearby(subject)
                    },
                    new Action((handler, subject) => handler.FleeDanger(subject), handler, subject)
                }
            };

            Sequence patrol = new Sequence
            {
                children = new Task[]
                {
                    new Condition(handler, subject)
                    {
                        condition = (handler, subject) => !handler.ThreatsNearby(subject)
                    },
                    new Condition(handler, subject)
                    {
                        condition = (handler, subject) => !handler.PreysNearby(subject)
                    },
                    new Action((handler, subject) => subject.Patrol(), handler, subject)
                }
            };

            Action idle = new Action((handler, subject) => subject.Idle(), handler, subject);
            #endregion

            root = new Selector
            {
                children = new Task[]
                {
                    chasePrey,
                    fleeDanger,
                    patrol,
                    idle
                }
            };
        }

        public bool IsTreeGenerated()
        {
            return root != null;
        }

        public void Tick()
        {
            root.Tick();
        }
    }
}
