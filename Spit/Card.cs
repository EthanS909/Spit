using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spit
{
    class Card
    {
        private int number;
        private string suit;

        public Card(string suit, int number)
        {
            this.suit = suit;
            this.number = number;
        }

        public int GetNumber() { return number; }
        public string GetSuit() { return suit; }
    }
}
