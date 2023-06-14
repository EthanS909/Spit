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

        // Hand
        public Stack<Card> firstPile = new Stack<Card>();
        public Stack<Card> secondPile = new Stack<Card>();
        public Stack<Card> thirdPile = new Stack<Card>();
        public Stack<Card> fourthPile = new Stack<Card>();
        public Stack<Card> fifthPile = new Stack<Card>();

        // Draw cards
        public Stack<Card> playerPickUp = new Stack<Card>();
        public Stack<Card> AIPickUp = new Stack<Card>();

        // Place card
        public Stack<Card> pile1 = new Stack<Card>();
        public Stack<Card> pile2 = new Stack<Card>();

        public int selectedPile;
        public Stack<Card>[] piles = new Stack<Card>[5];

        public Game()
        {
            deck.CreateDeck();
            piles[0] = firstPile;
            piles[1] = secondPile;
            piles[2] = thirdPile;
            piles[3] = fourthPile;
            piles[4] = fifthPile;

        }

        public bool TopCardVisibility
        {
            get { return deck.GetTopCard().IsVisible(); }
            set
            {
                Card card = deck.GetTopCard();
                card.isVisible = value;
                if(card.IsVisible())
                {
                    TopCard = "CardImages/" + card.GetNumber() + "_of_" + card.GetSuit() + "s.png";
                }
                else { TopCard = "CardImages/back.png"; }
            }
        }

        public string TopCard
        {
            get
            { 
                if (deck.GetTopCard().IsVisible() || topCard == "CardImages/back.png") { return topCard; }
                else
                {
                    TopCard = "CardImages/back.png";
                    return topCard;
                }
            }
            set
            {
                topCard = value;
                OnPropertyChanged("TopCard");
            }
        }

        public bool Place(Card card, Stack<Card> pile, int selectedPile)
        {
            bool placed = false;
            if (piles[selectedPile].Peek().GetNumber() <= pile.Peek().GetNumber() && piles[selectedPile].Peek().GetNumber() <= pile.Peek().GetNumber() - 1)
            {
                pile.Push(card);
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
            firstPile.Push(playerPickUp.Pop());
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

        public int GetPlayerCardCount()
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
