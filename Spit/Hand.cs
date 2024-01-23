using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Spit.DataStructures;

namespace Spit
{
    class Hand
    {
        public Pile firstPile = new Pile();
        public Pile secondPile = new Pile();
        public Pile thirdPile = new Pile();
        public Pile fourthPile = new Pile();
        public Pile fifthPile = new Pile();

        public Pile[] piles = new Pile[5];

        public Pile pickUpPile = new Pile();

        public Hand()
        {
            piles[0] = firstPile;
            piles[1] = secondPile;
            piles[2] = thirdPile;
            piles[3] = fourthPile;
            piles[4] = fifthPile;
        }

        // Copy Constructor
        public Hand(Hand original)
        {
            piles[0] = firstPile;
            piles[1] = secondPile;
            piles[2] = thirdPile;
            piles[3] = fourthPile;
            piles[4] = fifthPile;

            Stack pile1 = new Stack();
            Stack pile2 = new Stack();
            Stack pile3 = new Stack();
            Stack pile4 = new Stack();
            Stack pile5 = new Stack();
            Stack[] pileStacks = new Stack[5];
            pileStacks[0] = pile1;
            pileStacks[1] = pile2;
            pileStacks[2] = pile3;
            pileStacks[3] = pile4;
            pileStacks[4] = pile5;

            Stack pickUpStack = new Stack();

            for (int j = 0; j < piles.Length; j++)
            {
                int pileLen = original.piles[j].pile.Length();
                for (int i = 0; i < pileLen; i++)
                {
                    pileStacks[j].Push(original.piles[j].pile.stack.GetData(original.piles[j].pile.stack.Count() - i - 1));
                }
            }

            for (int j = 0; j < piles.Length; j++)
            {
                int pileLen = pileStacks[j].Length();
                for (int i = 0; i < pileLen; i++)
                {
                    piles[j].pile.Push(pileStacks[j].Pop());
                }
            }


            int pickUpPileLen = original.pickUpPile.pile.Length();
            for (int i = 0; i < pickUpPileLen; i++)
            {
                pickUpStack.Push(original.pickUpPile.pile.stack.GetData(original.pickUpPile.pile.stack.Count() - i - 1));
            }

            int pickUpStackLen = pickUpStack.Length();
            for (int i = 0; i < pickUpStackLen; i++)
            {
                pickUpPile.pile.Push(pickUpStack.Pop());
            }
        }

        public int GetNumOfCards()
        {
            int numofCards = 0;

            for (int x = 0; x < piles.Length; x++)
            {
                int length = piles[x].pile.Length();
                for (int i = 0; i < length; i++)
                {
                    numofCards += 1;
                }
            }

            return numofCards;
        }
    }
}
