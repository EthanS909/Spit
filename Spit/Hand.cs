using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

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
