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
        public Stack firstPile = new Stack();
        public Stack secondPile = new Stack();
        public Stack thirdPile = new Stack();
        public Stack fourthPile = new Stack();
        public Stack fifthPile = new Stack();

        public virtual async void Move()
        {

        }

        public virtual int GetDelay()
        {
            return 0;
        }

        public virtual Task CalcBestMove()
        {
            return Task.CompletedTask;
        }
    }
}
