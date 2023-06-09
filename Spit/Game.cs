using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Spit
{
    class Game : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Deck deck = new Deck();

        public Player[] player = new Player[2];

        public string topCard = "";

        public Card firstPile;
        public Stack<Card> secondPile = new Stack<Card>();
        public Stack<Card> thirdPile = new Stack<Card>();
        public Stack<Card> fourthPile = new Stack<Card>();
        public Stack<Card> fifthPile = new Stack<Card>();

        public Stack<Card> playerPickUp = new Stack<Card>();
        public Stack<Card> AIPickUp = new Stack<Card>();

        public Game()
        {
            deck.CreateDeck();
        }

        /*public bool TopCardVisibility
        {
            get { return deck.GetTopCard().IsVisible(); }
            set
            {
                deck.GetTopCard().isVisible = value;
                if(deck.GetTopCard().IsVisible())
                {
                    TopCard = "CardImages/" + deck.GetTopCard().GetNumber() + "_of_" + deck.GetTopCard().GetSuit() + "s.png";
                }
                else
                {
                    TopCard = "CardImages/back.png";
                }
            }
        }*/

        public string TopCard
        {
            get
            {
                return topCard;
            }
            set
            {
                topCard = value;
                OnPropertyChanged("TopCard");
            }
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

            deck.Shuffle(this);

            SplitCards();
            PlacePlayingCards();
        }

        public void Load()
        {

        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void SplitCards()
        {
            int length = deck.deck.Length();

            for(int i = 0; i < length / 2; i++)
            {
                playerPickUp.Push(deck.deck.Pop());
            }
            for (int i = 0; i < length / 2; i++)
            {
                AIPickUp.Push(deck.deck.Pop());
            }
        }

        public void PlacePlayingCards()
        {
            firstPile = playerPickUp.Pop();
            for (int i = 0; i < 5; i++)
            {
                if(i < 2)
                {
                    secondPile.Push(playerPickUp.Pop());
                }
                if(i < 3)
                {
                    thirdPile.Push(playerPickUp.Pop());
                }
                if (i < 4)
                {
                    fourthPile.Push(playerPickUp.Pop());
                }
                if (i < 5)
                {
                    fifthPile.Push(playerPickUp.Pop());
                }
            }
        }

        public int GetPlayerCards()
        {
            int length = 0;
            if (firstPile != null) { length++; }
            length += secondPile.Count();
            length += thirdPile.Count();
            length += fourthPile.Count();
            length += fifthPile.Count();
            return length;
        }

        public int GetAICards()
        {
            return 0;
        }
    }
}
