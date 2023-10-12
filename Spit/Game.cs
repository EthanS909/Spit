﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Spit.DataStructures;

namespace Spit
{
    //MVVM for design
    class Game : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private Database database = new Database();

        MainWindow wnd = (MainWindow)Application.Current.MainWindow;

        public DispatcherTimer countDownTimer = new DispatcherTimer();
        public DispatcherTimer AIwait = new DispatcherTimer();

        public bool pickAPile = false;
        public int chosenPile = -1;

        public int tick = 0;

        const int maxPlayers = 2;
        const int human = 0;
        const int ai = 1;

        public Deck deck = new Deck();

        public Player[] players = new Player[maxPlayers];

        // Place card
        public Pile pile1 = new Pile();
        public Pile pile2 = new Pile();

        public int selectedPile = -1;
        public Stack[] playerCardPiles = new Stack[5];
        public Stack[] AICardPiles = new Stack[5];
        public Pile[] placePiles = new Pile[2];

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

        int countDown;

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

        public int CountDown
        {
            get { return countDown;  }
            set
            {
                countDown = value;
                OnPropertyChanged("CountDown");
            }
        }

        public Game()
        {
            deck.CreateDeck();

            placePiles[0] = pile1;
            placePiles[1] = pile2;



            countDownTimer.Tick += Tick;
            countDownTimer.Interval = TimeSpan.FromSeconds(1);
        }

        private void Tick(object sender, EventArgs e)
        {
            tick++;
        }

        public bool Place(int pileNum)
        {
            bool placed = false;

            if (selectedPile != -1)
            {
                int cardNumber = players[human].hand.piles[selectedPile].pile.Peek().GetNumber();
                int target1 = placePiles[pileNum].pile.Peek().GetNumber();
                int target2 = placePiles[pileNum].pile.Peek().GetNumber();

                if (target1 == 1) { target1 = 13; }
                else { target1 -= 1; }
                if (target2 == 13) { target2 = 1; }
                else { target2 += 1; }

                if (cardNumber == target1 || cardNumber == target2)
                {
                    placePiles[pileNum].pile.Push(players[human].hand.piles[selectedPile].pile.Pop());

                    placed = true;
                }
            }

            wnd.aiTimer.Stop();
            wnd.aiTimer.Start();

            return placed;
        }

        public void Start(int difficulty)
        {
            players[human] = new HumanPlayer();
            players[ai] = new AIPlayer(this, difficulty);

            deck.Shuffle();

            SplitCards();
            PlacePlayingCards();
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
                players[human].hand.pickUpPile.pile.Push(deck.deck.Pop());
            }
            for (int i = 0; i < length / 2; i++)
            {
                players[ai].hand.pickUpPile.pile.Push(deck.deck.Pop());
            }
        }

        public void PlacePlayingCards()
        {
            players[human].hand.firstPile.pile.Push(players[human].hand.pickUpPile.pile.Pop());
            for (int i = 0; i < 5; i++)
            {
                if (i < 2 && players[human].hand.pickUpPile.pile.Length() != 0)
                {
                    players[human].hand.secondPile.pile.Push(players[human].hand.pickUpPile.pile.Pop());
                }
                if (i < 3 && players[human].hand.pickUpPile.pile.Length() != 0)
                {
                    players[human].hand.thirdPile.pile.Push(players[human].hand.pickUpPile.pile.Pop());
                }
                if (i < 4 && players[human].hand.pickUpPile.pile.Length() != 0)
                {
                    players[human].hand.fourthPile.pile.Push(players[human].hand.pickUpPile.pile.Pop());
                }
                if (i < 5 && players[human].hand.pickUpPile.pile.Length() != 0)
                {
                    players[human].hand.fifthPile.pile.Push(players[human].hand.pickUpPile.pile.Pop());
                }
            }

            players[ai].hand.firstPile.pile.Push(players[ai].hand.pickUpPile.pile.Pop());
            for (int i = 0; i < 5; i++)
            {
                if (i < 2 && players[ai].hand.pickUpPile.pile.Length() != 0)
                {
                    players[ai].hand.secondPile.pile.Push(players[ai].hand.pickUpPile.pile.Pop());
                }
                if (i < 3 && players[ai].hand.pickUpPile.pile.Length() != 0)
                {
                    players[ai].hand.thirdPile.pile.Push(players[ai].hand.pickUpPile.pile.Pop());
                }
                if (i < 4 && players[ai].hand.pickUpPile.pile.Length() != 0)
                {
                    players[ai].hand.fourthPile.pile.Push(players[ai].hand.pickUpPile.pile.Pop());
                }
                if (i < 5 && players[ai].hand.pickUpPile.pile.Length() != 0)
                {
                    players[ai].hand.fifthPile.pile.Push(players[ai].hand.pickUpPile.pile.Pop());
                }
            }

            if (players[human].hand.pickUpPile.pile.Length() != 0) pile2.pile.Push(players[human].hand.pickUpPile.pile.Pop());
            if (players[ai].hand.pickUpPile.pile.Length() != 0) pile1.pile.Push(players[ai].hand.pickUpPile.pile.Pop());
        }

        public void CanPlay()
        {
            // Create targets
            int target1 = pile1.pile.Peek().GetNumber();
            int target2 = pile1.pile.Peek().GetNumber();
            int target3 = pile2.pile.Peek().GetNumber();
            int target4 = pile2.pile.Peek().GetNumber();

            if (target1 == 1) { target1 = 13; }
            else { target1 -= 1; }
            if (target2 == 13) { target2 = 1; }
            else { target2 += 1; }
            if (target3 == 1) { target3 = 13; }
            else { target3 -= 1; }
            if (target4 == 13) { target4 = 1; }
            else { target4 += 1; }

            // Check if human can play
            bool humanCanPlay = false;
            int hEmptyPiles = 0;
            foreach(Pile pile in players[human].hand.piles)
            {
                if (!pile.pile.IsEmpty())
                {
                    int hCardNumber = pile.pile.Peek().GetNumber();

                    if (hCardNumber == target1 || hCardNumber == target2 || hCardNumber == target3 || hCardNumber == target4)
                    {
                        humanCanPlay = true;
                    }
                }
                else { hEmptyPiles++; }
            }

            // Check if AI can play
            bool AICanPlay = false;
            int aiEmptyPiles = 0;
            foreach (Pile pile in players[ai].hand.piles)
            {
                if (!pile.pile.IsEmpty())
                {
                    int aiCardNumber = pile.pile.Peek().GetNumber();

                    if (aiCardNumber == target1 || aiCardNumber == target2 || aiCardNumber == target3 || aiCardNumber == target4)
                    {
                        AICanPlay = true;
                    }
                }
                else { aiEmptyPiles++; }
            }

            if(hEmptyPiles == 5 || aiEmptyPiles == 5)
            {
                StartTimer();
                pickAPile = true;
            }
            else if(!humanCanPlay && !AICanPlay)
            {
                if(players[human].hand.pickUpPile.pile.Length() != 0 && players[ai].hand.pickUpPile.pile.Length() != 0)
                {
                    wnd.aiTimer.Stop();
                    wnd.background.Visibility = Visibility.Visible;
                    wnd.CountDown.Visibility = Visibility.Visible;
                    wnd.DrawingText.Visibility = Visibility.Visible;
                    countDownTimer.Start();

                    if (tick == 3)
                    {
                        countDownTimer.Stop();
                        tick = 0;

                        wnd.CountDown.Visibility = Visibility.Hidden;
                        wnd.background.Visibility = Visibility.Hidden;
                        wnd.DrawingText.Visibility = Visibility.Hidden;

                        if (players[ai].hand.pickUpPile.pile.Length() != 0)
                        {
                            pile1.pile.Push(players[ai].hand.pickUpPile.pile.Pop());
                        }
                        else
                        {
                            wnd.aiStack.Visibility = Visibility.Hidden;
                        }
                        if (players[human].hand.pickUpPile.pile.Length() != 0)
                        {
                            pile2.pile.Push(players[human].hand.pickUpPile.pile.Pop());
                        }
                        else
                        {
                            wnd.plStack.Visibility = Visibility.Hidden;
                        }
                        wnd.aiTimer.Start();
                    }
                }
            }
        }

        public void StartTimer()
        {
            AIwait.Interval = TimeSpan.FromSeconds(1);
            AIwait.Tick += AIChoosePile;
            AIwait.Start();
            wnd.aiTimer.Stop();
            wnd.updateTimer.Stop();
        }

        public void AIChoosePile(object sender, EventArgs e)
        {
            ChoosePile();
        }

        public void ChoosePile()
        {
            AIwait.Stop();

            if(chosenPile != -1)
            {
                placePiles[chosenPile].Unload(players[human]);
                placePiles[(chosenPile + 1) % 2].Unload(players[ai]);
            }
            else
            {
                int pile0Len = placePiles[0].pile.Length();
                int pile1Len = placePiles[1].pile.Length();

                if (pile0Len < pile1Len)
                {
                    placePiles[0].Unload(players[ai]);
                    placePiles[1].Unload(players[human]);
                }
                else
                {
                    placePiles[1].Unload(players[ai]);
                    placePiles[0].Unload(players[human]);
                }
            }

            CollectPlayingCards();
            ShuffleCards();
            PlacePlayingCards();
            pickAPile = false;
            wnd.updateTimer.Start();
            wnd.aiTimer.Start();
        }

        public void CollectPlayingCards()
        {
            foreach (Pile pile in players[human].hand.piles)
            {
                while (!pile.pile.IsEmpty())
                {
                    players[human].hand.pickUpPile.pile.Push(pile.pile.Pop());
                }
            }

            foreach (Pile pile in players[ai].hand.piles)
            {
                while (!pile.pile.IsEmpty())
                {
                    players[ai].hand.pickUpPile.pile.Push(pile.pile.Pop());
                }
            }
        }

        public void ShuffleCards()
        {
            ///Player pick up cards shuffling
            //Shuffle cards into array
            if(players[human].hand.pickUpPile.pile.Length() != 0)
            {
                Card[] shuffledDeck = new Card[players[human].hand.pickUpPile.pile.Length()];
                Random rnd = new Random();
                for (int x = 0; x < shuffledDeck.Length; x++)
                {
                    int randomSpace = rnd.Next(0, players[human].hand.pickUpPile.pile.Length() - 1);
                    if (players[human].hand.pickUpPile.pile.Length() != 0)
                    {
                        while (shuffledDeck[randomSpace] != null)
                        {
                            randomSpace = (randomSpace + 1) % shuffledDeck.Length;
                        }
                        shuffledDeck[randomSpace] = players[human].hand.pickUpPile.pile.Pop();
                    }
                }

                //Loads cards from array back into the players pick up pile
                for (int x = 0; x < shuffledDeck.Length; x++)
                {
                    players[human].hand.pickUpPile.pile.Push(shuffledDeck[x]);
                }
            }

            ///AI pick up cards shuffling
            //Shuffle cards into array
            if (players[ai].hand.pickUpPile.pile.Length() != 0)
            {
                Card[] shuffledDeck = new Card[players[ai].hand.pickUpPile.pile.Length()];
                Random rnd = new Random();
                for (int x = 0; x < shuffledDeck.Length; x++)
                {
                    int randomSpace = rnd.Next(0, players[ai].hand.pickUpPile.pile.Length() - 1);
                    if (players[ai].hand.pickUpPile.pile.Length() != 0)
                    {
                        while (shuffledDeck[randomSpace] != null)
                        {
                            randomSpace = (randomSpace + 1) % shuffledDeck.Length;
                        }
                        shuffledDeck[randomSpace] = players[ai].hand.pickUpPile.pile.Pop();
                    }
                }

                //Loads cards from array back into the AIs pick up pile
                for (int x = 0; x < shuffledDeck.Length; x++)
                {
                    players[ai].hand.pickUpPile.pile.Push(shuffledDeck[x]);
                }
            }
        }

        public void Update()
        {
            if (!players[human].hand.firstPile.pile.IsEmpty()) { PlayerFirstPileTop = "CardImages/" + players[human].hand.firstPile.pile.Peek().GetNumber() + "_of_" + players[human].hand.firstPile.pile.Peek().GetSuit() + "s.png"; }
            if (!players[human].hand.secondPile.pile.IsEmpty()) { PlayerSecondPileTop = "CardImages/" + players[human].hand.secondPile.pile.Peek().GetNumber() + "_of_" + players[human].hand.secondPile.pile.Peek().GetSuit() + "s.png"; }
            if (!players[human].hand.thirdPile.pile.IsEmpty()) { PlayerThirdPileTop = "CardImages/" + players[human].hand.thirdPile.pile.Peek().GetNumber() + "_of_" + players[human].hand.thirdPile.pile.Peek().GetSuit() + "s.png"; }
            if (!players[human].hand.fourthPile.pile.IsEmpty()) { PlayerFourthPileTop = "CardImages/" + players[human].hand.fourthPile.pile.Peek().GetNumber() + "_of_" + players[human].hand.fourthPile.pile.Peek().GetSuit() + "s.png"; }
            if (!players[human].hand.fifthPile.pile.IsEmpty()) { PlayerFifthPileTop = "CardImages/" + players[human].hand.fifthPile.pile.Peek().GetNumber() + "_of_" + players[human].hand.fifthPile.pile.Peek().GetSuit() + "s.png"; }

            if (!players[ai].hand.firstPile.pile.IsEmpty()) { AIFirstPileTop = "CardImages/" + players[ai].hand.firstPile.pile.Peek().GetNumber() + "_of_" + players[ai].hand.firstPile.pile.Peek().GetSuit() + "s.png"; }
            if (!players[ai].hand.secondPile.pile.IsEmpty()) { AISecondPileTop = "CardImages/" + players[ai].hand.secondPile.pile.Peek().GetNumber() + "_of_" + players[ai].hand.secondPile.pile.Peek().GetSuit() + "s.png"; }
            if (!players[ai].hand.thirdPile.pile.IsEmpty()) { AIThirdPileTop = "CardImages/" + players[ai].hand.thirdPile.pile.Peek().GetNumber() + "_of_" + players[ai].hand.thirdPile.pile.Peek().GetSuit() + "s.png"; }
            if (!players[ai].hand.fourthPile.pile.IsEmpty()) { AIFourthPileTop = "CardImages/" + players[ai].hand.fourthPile.pile.Peek().GetNumber() + "_of_" + players[ai].hand.fourthPile.pile.Peek().GetSuit() + "s.png"; }
            if (!players[ai].hand.fifthPile.pile.IsEmpty()) { AIFifthPileTop = "CardImages/" + players[ai].hand.fifthPile.pile.Peek().GetNumber() + "_of_" + players[ai].hand.fifthPile.pile.Peek().GetSuit() + "s.png"; }

            Pile1Top = "CardImages/" + pile1.pile.Peek().GetNumber() + "_of_" + pile1.pile.Peek().GetSuit() + "s.png";
            Pile2Top = "CardImages/" + pile2.pile.Peek().GetNumber() + "_of_" + pile2.pile.Peek().GetSuit() + "s.png";

            for (int i = 0; i < AICardPiles.Length; i++)
            {
                if (players[ai].hand.piles[i].pile.IsEmpty())
                {
                    wnd.aiCardPiles[i].Visibility = Visibility.Hidden;
                }
            }

            for (int i = 0; i < playerCardPiles.Length; i++)
            {
                if (players[human].hand.piles[i].pile.IsEmpty())
                {
                    wnd.plCardPiles[i].Visibility = Visibility.Hidden;
                }
            }

            if(pile1.pile.IsEmpty())
            {
                wnd.placePiles[0].Visibility = Visibility.Hidden;
            }
            if (pile2.pile.IsEmpty())
            {
                wnd.placePiles[1].Visibility = Visibility.Hidden;
            }

            for (int i = 0; i < AICardPiles.Length; i++)
            {
                if (!players[ai].hand.piles[i].pile.IsEmpty())
                {
                    wnd.aiCardPiles[i].Visibility = Visibility.Visible;
                }
            }

            for (int i = 0; i < playerCardPiles.Length; i++)
            {
                if (!players[human].hand.piles[i].pile.IsEmpty())
                {
                    wnd.plCardPiles[i].Visibility = Visibility.Visible;
                }
            }

            if (!pile1.pile.IsEmpty())
            {
                wnd.placePiles[0].Visibility = Visibility.Visible;
            }
            if (!pile2.pile.IsEmpty())
            {
                wnd.placePiles[1].Visibility = Visibility.Visible;
            }

            CountDown = 3 - tick;

            if (!pickAPile)
            {
                CanPlay();
            }
        }
    }
}
