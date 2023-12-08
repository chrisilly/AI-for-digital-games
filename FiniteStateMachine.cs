using Microsoft.Xna.Framework;

namespace AI_for_digital_games
{
    public interface IState
    {
        public void Handle(AgentHandler handler, Agent subject);
        public void Update(AgentHandler handler, Agent subject, GameTime gameTime);
    }

    //public class Context
    //{
    //    IState state;

    //    public Context(IState state)
    //    {
    //        this.state = state;
    //    }

    //    public void SetState(IState state)
    //    {
    //        this.state = state;
    //    }
    //}

    class FleeingState : IState
    {
        public void Handle(AgentHandler handler, Agent subject)
        {
            if (!handler.ThreatsNearby(subject))
            {
                subject.Patrol();
                subject.State = new PatrollingState();
            }
        }

        public void Update(AgentHandler handler, Agent subject, GameTime gameTime)
        {
            //subject.Move(gameTime);
            handler.FleeDanger(subject);
        }
    }

    class PatrollingState : IState
    {
        public void Handle(AgentHandler handler, Agent subject)
        {
            if (handler.ThreatsNearby(subject))
            {
                subject.State = new FleeingState();
            }
            else if (handler.PreysNearby(subject))
            {
                subject.State = new HuntingState();
            }
        }

        public void Update(AgentHandler handler, Agent subject, GameTime gameTime)
        {
            //subject.Move(gameTime);
        }
    }

    class HuntingState : IState
    {
        public void Handle(AgentHandler handler, Agent subject)
        {
            if (!handler.PreysNearby(subject))
            {
                subject.Patrol();
                subject.State = new PatrollingState();
            }
        }

        public void Update(AgentHandler handler, Agent subject, GameTime gameTime)
        {
            //subject.Move(gameTime);
            handler.ChaseFood(subject);
        }
    }
}
