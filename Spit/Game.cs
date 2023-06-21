using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using Spit.DataStructures;

namespace Spit
{
    class Game : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Deck deck = new Deck();

        public Player[] player = new Player[2];

        //public string topCard = "";

        // Hand
        public Stack firstPile = new Stack();
        public Stack secondPile = new Stack();
        public Stack thirdPile = new Stack();
        public Stack fourthPile = new Stack();
        public Stack fifthPile = new Stack();

        // Draw cards
        public Stack<Card> playerPickUp = new Stack<Card>();
        public Stack<Card> AIPickUp = new Stack<Card>();

        // Place card
        public Stack pile1 = new Stack();
        public Stack pile2 = new Stack();

        public int selectedPile = -1;
        public Stack[] cardPiles = new Stack[5];
        public Stack[] placePiles = new Stack[2];

        string firstPileTop;
        string secondPileTop;
        string thirdPileTop;
        string fourthPileTop;
        string fifthPileTop;

        string pile1Top;
        string pile2Top;

        public string FirstPileTop
        {
            get { return firstPileTop; }
            set
            { 
                firstPileTop = value;
                OnPropertyChanged("FirstPileTop");
            }
        }
        public string SecondPileTop
        {
            get { return secondPileTop; }
            set
            {
                secondPileTop = value;
                OnPropertyChanged("SecondPileTop");
            }
        }

        public string ThirdPileTop
        {
            get { return thirdPileTop; }
            set
            {
                thirdPileTop = value;
                OnPropertyChanged("ThirdPileTop");
            }
        }
        public string FourthPileTop
        {
            get { return fourthPileTop; }
            set
            {
                fourthPileTop = value;
                OnPropertyChanged("FourthPileTop");
            }
        }
        public string FifthPileTop
        {
            get { return fifthPileTop; }
            set
            {
                fifthPileTop = value;
                OnPropertyChanged("FifthPileTop");
            }
        }

        public string Pile1Top
        {
            get { return pile1Top; }
            set
            {
                pile1Top = value;
                OnPropertyChanged("Pile1Top");
            }
        }

        public string Pile2Top
        {
            get { return pile2Top; }
            set
            {
                pile2Top = value;
                OnPropertyChanged("Pile2Top");
            }
        }


        public Game()
        {
            deck.CreateDeck();
            cardPiles[0] = firstPile;
            cardPiles[1] = secondPile;
            cardPiles[2] = thirdPile;
            cardPiles[3] = fourthPile;
            cardPiles[4] = fifthPile;

            placePiles[0] = pile1;
            placePiles[1] = pile2;
        }

        public bool Place(int pileNum)
        {
            bool placed = false;

            int cardNumber = cardPiles[selectedPile].Peek().GetNumber();
            int target1 = placePiles[pileNum].Peek().GetNumber() - 1;
            int target2 = placePiles[pileNum].Peek().GetNumber() + 1;

            if(target1 > 13) { target1 -= 13; }
            if(target1 < 1) { target1 += 13; }
            if(target2 > 13) { target2 -= 13; }
            if(target2 < 1) { target2 += 13; }

            if (selectedPile != -1)
            {
                if (cardNumber == target1 || cardNumber == target2)
                {
                    placePiles[pileNum].Push(cardPiles[selectedPile].Pop());

                    placed = true;
                }
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

            Update();
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

            pile1.Push(AIPickUp.Pop());
            pile2.Push(playerPickUp.Pop());
        }

        public int GetPlayerCardCount()
        {
            int length = 0;
            if (firstPile != null) { length++; }
            length += secondPile.Length();
            length += thirdPile.Length();
            length += fourthPile.Length();
            length += fifthPile.Length();
            return length;
        }

        public int GetAICards()
        {
            return 0;
        }

        public void Update()
        {
            foreach(Stack pile in cardPiles)
            {
                if(pile.stack.head != null)
                {
                    FirstPileTop = "CardImages/" + firstPile.Peek().GetNumber() + "_of_" + firstPile.Peek().GetSuit() + "s.png";
                    SecondPileTop = "CardImages/" + secondPile.Peek().GetNumber() + "_of_" + secondPile.Peek().GetSuit() + "s.png";
                    ThirdPileTop = "CardImages/" + thirdPile.Peek().GetNumber() + "_of_" + thirdPile.Peek().GetSuit() + "s.png";
                    FourthPileTop = "CardImages/" + fourthPile.Peek().GetNumber() + "_of_" + fourthPile.Peek().GetSuit() + "s.png";
                    FifthPileTop = "CardImages/" + fifthPile.Peek().GetNumber() + "_of_" + fifthPile.Peek().GetSuit() + "s.png";
                }
            }

            Pile1Top = "CardImages/" + pile1.Peek().GetNumber() + "_of_" + pile1.Peek().GetSuit() + "s.png";
            Pile2Top = "CardImages/" + pile2.Peek().GetNumber() + "_of_" + pile2.Peek().GetSuit() + "s.png";
        }
    }
}
