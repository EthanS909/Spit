using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spit.DataStructures;

namespace Spit
{
    abstract class Player
    {
        // Players Hand
        public Hand hand = new Hand();

        public Player()
        {

        }

        public virtual async void Move()
        {

        }

        public virtual int GetDelay()
        {
            return 0;
        }

        public virtual int GetDifficulty()
        {
            return -1;
        }

        public virtual void MinMax()
        {

        }

        public virtual void Reset()
        {

        }
    }
}
