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
        public void Update(Agent body)
        {
            body.Move(GetRandomDirection());
        }

        public void Decide(Agent body)
        {
            
        }

        Vector2 GetRandomDirection()
        {
            Random random = new Random();
            int x = random.Next(-1, 2);
            int y = random.Next(-1, 2);
            return new Vector2(x, y);
        }
    }
}
