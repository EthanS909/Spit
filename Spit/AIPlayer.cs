using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spit
{
    internal class AIPlayer : Player
    {
        private int delay;
        private int lookAhead;

        public AIPlayer(int difficulty)
        {
            if(difficulty == 0 )
            {
                delay = 350;
                lookAhead = 1;
            }
            if (difficulty == 1)
            {
                delay = 250;
                lookAhead = 1;
            }
            if (difficulty == 2)
            {
                delay = 150;
                lookAhead = 3;
            }
        }
    }
}
