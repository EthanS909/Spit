using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spit.DataStructures;

namespace Spit
{
    class Pile
    {
        public Pile()
        {

        }

        // Copy Constructor
        public Pile(Pile other)
        {
            Stack stack = new Stack();

            int count = other.pile.stack.Count();
            for (int i = 0; i < count; i++)
            {
                stack.Push(other.pile.stack.GetData(other.pile.stack.Count() - i - 1));
            }

            for (int i = 0; i < count; i++)
            {
                pile.Push(stack.Pop());
            }
        }

        public Stack pile = new Stack();

        public void Unload(Player player)
        {
            int pileLen = pile.Length();
            for (int i = 0; i < pileLen; i++)
            {
                player.hand.pickUpPile.pile.Push(pile.Pop());
            }
        }
    }
}