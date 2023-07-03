using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
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

        const int maxPlayers = 2;

        public Deck deck = new Deck();

        public Player[] players = new Player[maxPlayers];

        // Draw cards
        public Stack<Card> playerPickUp = new Stack<Card>();
        public Stack<Card> AIPickUp = new Stack<Card>();

        // Place card
        public Stack pile1 = new Stack();
        public Stack pile2 = new Stack();

        public int selectedPile = -1;
        public Stack[] playerCardPiles = new Stack[5];
        public Stack[] AICardPiles = new Stack[5];
        public Stack[] placePiles = new Stack[2];

        public string[] pileTops = new string[5];
        public string[] pileTop = new string[5];

        string playerFirstPileTop;
        string playerSecondPileTop;
        string playerThirdPileTop;
        string playerFourthPileTop;
        string playerFifthPileTop;

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

            placePiles[0] = pile1;
            placePiles[1] = pile2;
        }

        public bool Place(int pileNum)
        {
            bool placed = false;

            if (selectedPile != -1)
            {
                int cardNumber = playerCardPiles[selectedPile].Peek().GetNumber();
                int target1 = placePiles[pileNum].Peek().GetNumber();
                int target2 = placePiles[pileNum].Peek().GetNumber();

                if (target1 == 1) { target1 = 13; }
                else { target1 -= 1; }
                if (target2 == 13) { target2 = 1; }
                else { target2 += 1; }

                if (cardNumber == target1 || cardNumber == target2)
                {
                    placePiles[pileNum].Push(playerCardPiles[selectedPile].Pop());

                    placed = true;
                }
            }

            return placed;
        }

        public void Start(int difficulty)
        {
            players[0] = new HumanPlayer();
            players[1] = new AIPlayer(difficulty);

            playerCardPiles[0] = players[0].firstPile;
            playerCardPiles[1] = players[0].secondPile;
            playerCardPiles[2] = players[0].thirdPile;
            playerCardPiles[3] = players[0].fourthPile;
            playerCardPiles[4] = players[0].fifthPile;

            AICardPiles[0] = players[1].firstPile;
            AICardPiles[1] = players[1].secondPile;
            AICardPiles[2] = players[1].thirdPile;
            AICardPiles[3] = players[1].fourthPile;
            AICardPiles[4] = players[1].fifthPile;

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
            players[0].firstPile.Push(playerPickUp.Pop());
            for (int i = 0; i < 5; i++)
            {
                if(i < 2)
                {
                    players[0].secondPile.Push(playerPickUp.Pop());
                }
                if(i < 3)
                {
                    players[0].thirdPile.Push(playerPickUp.Pop());
                }
                if (i < 4)
                {
                    players[0].fourthPile.Push(playerPickUp.Pop());
                }
                if (i < 5)
                {
                    players[0].fifthPile.Push(playerPickUp.Pop());
                }
            }

            players[1].firstPile.Push(AIPickUp.Pop());
            for (int i = 0; i < 5; i++)
            {
                if (i < 2)
                {
                    players[1].secondPile.Push(AIPickUp.Pop());
                }
                if (i < 3)
                {
                    players[1].thirdPile.Push(AIPickUp.Pop());
                }
                if (i < 4)
                {
                    players[1].fourthPile.Push(AIPickUp.Pop());
                }
                if (i < 5)
                {
                    players[1].fifthPile.Push(AIPickUp.Pop());
                }
            }

            pile1.Push(AIPickUp.Pop());
            pile2.Push(playerPickUp.Pop());
        }

        public int GetPlayerCardCount()
        {
            int length = 0;
            if (players[0].firstPile != null) { length++; }
            length += players[0].secondPile.Length();
            length += players[0].thirdPile.Length();
            length += players[0].fourthPile.Length();
            length += players[0].fifthPile.Length();
            return length;
        }

        public int GetAICardCount()
        {
            int length = 0;
            if (players[1].firstPile != null) { length++; }
            length += players[1].secondPile.Length();
            length += players[1].thirdPile.Length();
            length += players[1].fourthPile.Length();
            length += players[1].fifthPile.Length();
            return length;
        }

        public void CanPlay()
        {
            // Create targets
            int target1 = pile1.Peek().GetNumber();
            int target2 = pile1.Peek().GetNumber();
            int target3 = pile2.Peek().GetNumber();
            int target4 = pile2.Peek().GetNumber();

            if (target1 == 1) { target1 = 13; }
            else { target1 -= 1; }
            if (target2 == 13) { target2 = 1; }
            else { target2 += 1; }
            if (target3 == 1) { target3 = 13; }
            else { target3 += 1; }
            if (target4 == 13) { target4 = 1; }
            else { target4 += 1; }

            // Check if human can play
            bool humanCanPlay = false;
            foreach(Stack pile in playerCardPiles)
            {
                if(pile.stack.GetHead() != null)
                {
                    int cardNumber = pile.Peek().GetNumber();

                    if (cardNumber == target1 || cardNumber == target2 || cardNumber == target3 || cardNumber == target4)
                    {
                        humanCanPlay = true;
                    }
                }   
            }

            // Check if AI can play
            bool AICanPlay = false;
            foreach (Stack pile in AICardPiles)
            {
                int cardNumber = pile.Peek().GetNumber();

                if (cardNumber == target1 || cardNumber == target2 || cardNumber == target3 || cardNumber == target4)
                {
                    humanCanPlay = true;
                }
            }

            if(!humanCanPlay && !AICanPlay)
            {

            }
        }

        public void Update()
        {
            if (players[0].firstPile.stack.GetHead() != null) { PlayerFirstPileTop = "CardImages/" + players[0].firstPile.Peek().GetNumber() + "_of_" + players[0].firstPile.Peek().GetSuit() + "s.png"; }
            if (players[0].secondPile.stack.GetHead() != null) { PlayerSecondPileTop = "CardImages/" + players[0].secondPile.Peek().GetNumber() + "_of_" + players[0].secondPile.Peek().GetSuit() + "s.png"; }
            if (players[0].thirdPile.stack.GetHead() != null) { PlayerThirdPileTop = "CardImages/" + players[0].thirdPile.Peek().GetNumber() + "_of_" + players[0].thirdPile.Peek().GetSuit() + "s.png"; }
            if (players[0].fourthPile.stack.GetHead() != null) { PlayerFourthPileTop = "CardImages/" + players[0].fourthPile.Peek().GetNumber() + "_of_" + players[0].fourthPile.Peek().GetSuit() + "s.png"; }
            if (players[0].fifthPile.stack.GetHead() != null) { PlayerFifthPileTop = "CardImages/" + players[0].fifthPile.Peek().GetNumber() + "_of_" + players[0].fifthPile.Peek().GetSuit() + "s.png"; }

            if (players[1].firstPile.stack.GetHead() != null) { AIFirstPileTop = "CardImages/" + players[1].firstPile.Peek().GetNumber() + "_of_" + players[1].firstPile.Peek().GetSuit() + "s.png"; }
            if (players[1].secondPile.stack.GetHead() != null) { AISecondPileTop = "CardImages/" + players[1].secondPile.Peek().GetNumber() + "_of_" + players[1].secondPile.Peek().GetSuit() + "s.png"; }
            if (players[1].thirdPile.stack.GetHead() != null) { AIThirdPileTop = "CardImages/" + players[1].thirdPile.Peek().GetNumber() + "_of_" + players[1].thirdPile.Peek().GetSuit() + "s.png"; }
            if (players[1].fourthPile.stack.GetHead() != null) { AIFourthPileTop = "CardImages/" + players[1].fourthPile.Peek().GetNumber() + "_of_" + players[1].fourthPile.Peek().GetSuit() + "s.png"; }
            if (players[1].fifthPile.stack.GetHead() != null) { AIFifthPileTop = "CardImages/" + players[1].fifthPile.Peek().GetNumber() + "_of_" + players[1].fifthPile.Peek().GetSuit() + "s.png"; }

            Pile1Top = "CardImages/" + pile1.Peek().GetNumber() + "_of_" + pile1.Peek().GetSuit() + "s.png";
            Pile2Top = "CardImages/" + pile2.Peek().GetNumber() + "_of_" + pile2.Peek().GetSuit() + "s.png";

            CanPlay();
        }

        public bool IsPileEmpty(int index)
        {
            return playerCardPiles[index].IsEmpty();
        }
    }
}
