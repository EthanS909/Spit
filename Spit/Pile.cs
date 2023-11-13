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