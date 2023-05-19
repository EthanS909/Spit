using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Spit
{
    internal class Game
    {
        public Deck deck = new Deck();

        public Player[] player = new Player[2];

        public string TopCard
        {
            get { return "CardImages/" + deck.GetTopCard().GetNumber() + "_of_" + deck.GetTopCard().GetSuit() + "s.png"; }
        }

        public Game(int difficulty)
        {
            Start(difficulty);
        }

        public bool Place(Card card)
        {
            bool placed = false;
            if(deck.GetTopCard().GetNumber() == card.GetNumber())
            {
                deck.GetCards().Push(card);
                placed = true;
            }

            return placed;
        }

        public void Start(int difficulty)
        {
            player[0] = new HumanPlayer();
            player[1] = new AIPlayer(difficulty);

            deck.CreateDeck();
            deck.Shuffle();
        }

        public void Load()
        {

        }
    }
}
