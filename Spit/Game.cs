using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
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

        // Player Hand
        public Stack playerFirstPile = new Stack();
        public Stack playerSecondPile = new Stack();
        public Stack playerThirdPile = new Stack();
        public Stack playerFourthPile = new Stack();
        public Stack playerFifthPile = new Stack();

        // AI Hand
        public Stack AiFirstPile = new Stack();
        public Stack AiSecondPile = new Stack();
        public Stack AiThirdPile = new Stack();
        public Stack AiFourthPile = new Stack();
        public Stack AiFifthPile = new Stack();

        // Draw cards
        public Stack<Card> playerPickUp = new Stack<Card>();
        public Stack<Card> AIPickUp = new Stack<Card>();

        // Place card
        public Stack pile1 = new Stack();
        public Stack pile2 = new Stack();

        public int selectedPile = -1;
        public Stack[] cardPiles = new Stack[5];
        public Stack[] placePiles = new Stack[2];

        public string[] pileTops = new string[5];
        public string[] pileTop = new string[5];

        string playerFirstPileTop = "";
        string playerSecondPileTop = "";
        string playerThirdPileTop = "";
        string playerFourthPileTop = "";
        string playerFifthPileTop = "";

        string AiFirstPileTop;
        string AiSecondPileTop;
        string AiThirdPileTop;
        string AiFourthPileTop;
        string AiFifthPileTop;

        string pile1Top;
        string pile2Top;

        

        public string PlayerFirstPileTop
        {
            get { return playerFirstPileTop; }
            set
            { 
                playerFirstPileTop = value;
                OnPropertyChanged("PlayerFirstPileTop");
            }
        }
        public string PlayerSecondPileTop
        {
            get { return playerSecondPileTop; }
            set
            {
                playerSecondPileTop = value;
                OnPropertyChanged("PlayerSecondPileTop");
            }
        }
        public string PlayerThirdPileTop
        {
            get { return playerThirdPileTop; }
            set
            {
                playerThirdPileTop = value;
                OnPropertyChanged("PlayerThirdPileTop");
            }
        }
        public string PlayerFourthPileTop
        {
            get { return playerFourthPileTop; }
            set
            {
                playerFourthPileTop = value;
                OnPropertyChanged("PlayerFourthPileTop");
            }
        }
        public string PlayerFifthPileTop
        {
            get { return playerFifthPileTop; }
            set
            {
                playerFifthPileTop = value;
                OnPropertyChanged("PlayerFifthPileTop");
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

        public string AIFirstPileTop
        {
            get { return AiFirstPileTop; }
            set
            {
                AiFirstPileTop = value;
                OnPropertyChanged("AIFirstPileTop");
            }
        }
        public string AISecondPileTop
        {
            get { return AiSecondPileTop; }
            set
            {
                AiSecondPileTop = value;
                OnPropertyChanged("AISecondPileTop");
            }
        }
        public string AIThirdPileTop
        {
            get { return AiThirdPileTop; }
            set
            {
                AiThirdPileTop = value;
                OnPropertyChanged("AIThirdPileTop");
            }
        }
        public string AIFourthPileTop
        {
            get { return AiFourthPileTop; }
            set
            {
                AiFourthPileTop = value;
                OnPropertyChanged("AIFourthPileTop");
            }
        }
        public string AIFifthPileTop
        {
            get { return AiFifthPileTop; }
            set
            {
                AiFifthPileTop = value;
                OnPropertyChanged("AIFifthPileTop");
            }
        }


        public Game()
        {
            deck.CreateDeck();
            cardPiles[0] = playerFirstPile;
            cardPiles[1] = playerSecondPile;
            cardPiles[2] = playerThirdPile;
            cardPiles[3] = playerFourthPile;
            cardPiles[4] = playerFifthPile;

            placePiles[0] = pile1;
            placePiles[1] = pile2;
        }

        public bool Place(int pileNum)
        {
            bool placed = false;

            if (selectedPile != -1)
            {
                int cardNumber = cardPiles[selectedPile].Peek().GetNumber();
                int target1 = placePiles[pileNum].Peek().GetNumber();
                int target2 = placePiles[pileNum].Peek().GetNumber();

                if (target1 == 1) { target1 = 13; }
                else { target1 -= 1; }
                if (target2 == 13) { target2 = 1; }
                else { target2 += 1; }

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
            playerFirstPile.Push(playerPickUp.Pop());
            for (int i = 0; i < 5; i++)
            {
                if(i < 2)
                {
                    playerSecondPile.Push(playerPickUp.Pop());
                }
                if(i < 3)
                {
                    playerThirdPile.Push(playerPickUp.Pop());
                }
                if (i < 4)
                {
                    playerFourthPile.Push(playerPickUp.Pop());
                }
                if (i < 5)
                {
                    playerFifthPile.Push(playerPickUp.Pop());
                }
            }

            AiFirstPile.Push(AIPickUp.Pop());
            for (int i = 0; i < 5; i++)
            {
                if (i < 2)
                {
                    AiSecondPile.Push(AIPickUp.Pop());
                }
                if (i < 3)
                {
                    AiThirdPile.Push(AIPickUp.Pop());
                }
                if (i < 4)
                {
                    AiFourthPile.Push(AIPickUp.Pop());
                }
                if (i < 5)
                {
                    AiFifthPile.Push(AIPickUp.Pop());
                }
            }

            pile1.Push(AIPickUp.Pop());
            pile2.Push(playerPickUp.Pop());
        }

        public int GetPlayerCardCount()
        {
            int length = 0;
            if (playerFirstPile != null) { length++; }
            length += playerSecondPile.Length();
            length += playerThirdPile.Length();
            length += playerFourthPile.Length();
            length += playerFifthPile.Length();
            return length;
        }

        public int GetAICardCount()
        {
            int length = 0;
            if (AiFirstPile != null) { length++; }
            length += AiSecondPile.Length();
            length += AiThirdPile.Length();
            length += AiFourthPile.Length();
            length += AiFifthPile.Length();
            return length;
        }

        public void Update()
        {
            if (playerFirstPile.stack.GetHead() != null) { PlayerFirstPileTop = "CardImages/" + playerFirstPile.Peek().GetNumber() + "_of_" + playerFirstPile.Peek().GetSuit() + "s.png"; }
            if (playerSecondPile.stack.GetHead() != null) { PlayerSecondPileTop = "CardImages/" + playerSecondPile.Peek().GetNumber() + "_of_" + playerSecondPile.Peek().GetSuit() + "s.png"; }
            if (playerThirdPile.stack.GetHead() != null) { PlayerThirdPileTop = "CardImages/" + playerThirdPile.Peek().GetNumber() + "_of_" + playerThirdPile.Peek().GetSuit() + "s.png"; }
            if (playerFourthPile.stack.GetHead() != null) { PlayerFourthPileTop = "CardImages/" + playerFourthPile.Peek().GetNumber() + "_of_" + playerFourthPile.Peek().GetSuit() + "s.png"; }
            if (playerFifthPile.stack.GetHead() != null) { PlayerFifthPileTop = "CardImages/" + playerFifthPile.Peek().GetNumber() + "_of_" + playerFifthPile.Peek().GetSuit() + "s.png"; }

            Pile1Top = "CardImages/" + pile1.Peek().GetNumber() + "_of_" + pile1.Peek().GetSuit() + "s.png";
            Pile2Top = "CardImages/" + pile2.Peek().GetNumber() + "_of_" + pile2.Peek().GetSuit() + "s.png";
        }

        public bool IsPileEmpty(int index)
        {
            return cardPiles[index].IsEmpty();
        }
    }
}
