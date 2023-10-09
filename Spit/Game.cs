using System;
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

        MainWindow wnd = (MainWindow)Application.Current.MainWindow;

        public DispatcherTimer countDownTimer = new DispatcherTimer();
        public DispatcherTimer AIwait = new DispatcherTimer();

        public bool pickAPile = false;
        public int chosenPile = -1;

        public int tick = 0;

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

            wnd.aiTimer.Start();

            return placed;
        }

        public void Start(int difficulty)
        {
            players[0] = new HumanPlayer();
            players[1] = new AIPlayer(this, difficulty);

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

            deck.Shuffle();

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
            players[0].firstPile.Push(playerPickUp.Pop());
            for (int i = 0; i < 5; i++)
            {
                if (i < 2 && playerPickUp.Count != 0)
                {
                    players[0].secondPile.Push(playerPickUp.Pop());
                }
                if (i < 3 && playerPickUp.Count != 0)
                {
                    players[0].thirdPile.Push(playerPickUp.Pop());
                }
                if (i < 4 && playerPickUp.Count != 0)
                {
                    players[0].fourthPile.Push(playerPickUp.Pop());
                }
                if (i < 5 && playerPickUp.Count != 0)
                {
                    players[0].fifthPile.Push(playerPickUp.Pop());
                }
            }

            players[1].firstPile.Push(AIPickUp.Pop());
            for (int i = 0; i < 5; i++)
            {
                if (i < 2 && AIPickUp.Count != 0)
                {
                    players[1].secondPile.Push(AIPickUp.Pop());
                }
                if (i < 3 && AIPickUp.Count != 0)
                {
                    players[1].thirdPile.Push(AIPickUp.Pop());
                }
                if (i < 4 && AIPickUp.Count != 0)
                {
                    players[1].fourthPile.Push(AIPickUp.Pop());
                }
                if (i < 5 && AIPickUp.Count != 0)
                {
                    players[1].fifthPile.Push(AIPickUp.Pop());
                }
            }

            if (playerPickUp.Count != 0) pile2.Push(playerPickUp.Pop());
            if (AIPickUp.Count != 0) pile1.Push(AIPickUp.Pop());
        }

        public int GetPlayerCardCount()
        {
            int length = 0;
            length += players[0].firstPile.Length();
            length += players[0].secondPile.Length();
            length += players[0].thirdPile.Length();
            length += players[0].fourthPile.Length();
            length += players[0].fifthPile.Length();
            return length;
        }

        public int GetAICardCount()
        {
            int length = 0;
            length += players[1].firstPile.Length();
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
            else { target3 -= 1; }
            if (target4 == 13) { target4 = 1; }
            else { target4 += 1; }

            // Check if human can play
            bool humanCanPlay = false;
            int hEmptyPiles = 0;
            foreach(Stack pile in playerCardPiles)
            {
                if (!pile.IsEmpty())
                {
                    int hCardNumber = pile.Peek().GetNumber();

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
            foreach (Stack pile in AICardPiles)
            {
                if (!pile.IsEmpty())
                {
                    int aiCardNumber = pile.Peek().GetNumber();

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
                if(AIPickUp.Count != 0 && playerPickUp.Count != 0)
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

                        if (AIPickUp.Count != 0)
                        {
                            pile1.Push(AIPickUp.Pop());
                        }
                        else
                        {
                            wnd.aiStack.Visibility = Visibility.Hidden;
                        }
                        if (playerPickUp.Count != 0)
                        {
                            pile2.Push(playerPickUp.Pop());
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
        }

        public void AIChoosePile(object sender, EventArgs e)
        {
            ChoosePile();
            CollectPlayingCards();
            Update();
            ShuffleCards();
            PlacePlayingCards();
            pickAPile = false;
        }

        public void ChoosePile()
        {
            AIwait.Stop();

            if(chosenPile != -1)
            {
                int pileLen = placePiles[chosenPile].Length();
                for (int i = 0; i < pileLen + 1; i++)
                {
                    playerPickUp.Push(placePiles[chosenPile].Pop());
                    AIPickUp.Push(placePiles[(chosenPile + 1) % 2].Pop());
                }
            }
            else
            {
                int pile0Len = placePiles[0].Length();
                int pile1Len = placePiles[1].Length();

                if (pile0Len < pile1Len)
                {
                    for (int i = 0; i < pile0Len + 1; i++)
                    {
                        AIPickUp.Push(placePiles[0].Pop());
                        playerPickUp.Push(placePiles[1].Pop());
                    }
                }
                else
                {
                    for (int i = 0; i < pile1Len + 1; i++)
                    {
                        AIPickUp.Push(placePiles[1].Pop());
                        playerPickUp.Push(placePiles[0].Pop());
                    }
                }
            }
        }

        public void CollectPlayingCards()
        {
            foreach (Stack pile in playerCardPiles)
            {
                while (!pile.IsEmpty())
                {
                    playerPickUp.Push(pile.Pop());
                }
            }

            foreach (Stack pile in AICardPiles)
            {
                while (!pile.IsEmpty())
                {
                    AIPickUp.Push(pile.Pop());
                }
            }
        }

        public void ShuffleCards()
        {

        }

        public void Update()
        {
            if (!players[0].firstPile.IsEmpty()) { PlayerFirstPileTop = "CardImages/" + players[0].firstPile.Peek().GetNumber() + "_of_" + players[0].firstPile.Peek().GetSuit() + "s.png"; }
            if (!players[0].secondPile.IsEmpty()) { PlayerSecondPileTop = "CardImages/" + players[0].secondPile.Peek().GetNumber() + "_of_" + players[0].secondPile.Peek().GetSuit() + "s.png"; }
            if (!players[0].thirdPile.IsEmpty()) { PlayerThirdPileTop = "CardImages/" + players[0].thirdPile.Peek().GetNumber() + "_of_" + players[0].thirdPile.Peek().GetSuit() + "s.png"; }
            if (!players[0].fourthPile.IsEmpty()) { PlayerFourthPileTop = "CardImages/" + players[0].fourthPile.Peek().GetNumber() + "_of_" + players[0].fourthPile.Peek().GetSuit() + "s.png"; }
            if (!players[0].fifthPile.IsEmpty()) { PlayerFifthPileTop = "CardImages/" + players[0].fifthPile.Peek().GetNumber() + "_of_" + players[0].fifthPile.Peek().GetSuit() + "s.png"; }

            if (!players[1].firstPile.IsEmpty()) { AIFirstPileTop = "CardImages/" + players[1].firstPile.Peek().GetNumber() + "_of_" + players[1].firstPile.Peek().GetSuit() + "s.png"; }
            if (!players[1].secondPile.IsEmpty()) { AISecondPileTop = "CardImages/" + players[1].firstPile.Peek().GetNumber() + "_of_" + players[1].secondPile.Peek().GetSuit() + "s.png"; }
            if (!players[1].thirdPile.IsEmpty()) { AIThirdPileTop = "CardImages/" + players[1].thirdPile.Peek().GetNumber() + "_of_" + players[1].thirdPile.Peek().GetSuit() + "s.png"; }
            if (!players[1].fourthPile.IsEmpty()) { AIFourthPileTop = "CardImages/" + players[1].fourthPile.Peek().GetNumber() + "_of_" + players[1].fourthPile.Peek().GetSuit() + "s.png"; }
            if (!players[1].fifthPile.IsEmpty()) { AIFifthPileTop = "CardImages/" + players[1].fifthPile.Peek().GetNumber() + "_of_" + players[1].fifthPile.Peek().GetSuit() + "s.png"; }

            Pile1Top = "CardImages/" + pile1.Peek().GetNumber() + "_of_" + pile1.Peek().GetSuit() + "s.png";
            Pile2Top = "CardImages/" + pile2.Peek().GetNumber() + "_of_" + pile2.Peek().GetSuit() + "s.png";

            for (int i = 0; i < AICardPiles.Length; i++)
            {
                if (AICardPiles[i].IsEmpty())
                {
                    wnd.aiCardPiles[i].Visibility = Visibility.Hidden;
                }
            }

            for (int i = 0; i < playerCardPiles.Length; i++)
            {
                if (playerCardPiles[i].IsEmpty())
                {
                    wnd.plCardPiles[i].Visibility = Visibility.Hidden;
                }
            }

            if(pile1.IsEmpty())
            {
                wnd.placePiles[0].Visibility = Visibility.Hidden;
            }
            if (pile2.IsEmpty())
            {
                wnd.placePiles[1].Visibility = Visibility.Hidden;
            }

            CountDown = 3 - tick;

            if (!pickAPile)
            {
                CanPlay();
            }
        }
    }
}
