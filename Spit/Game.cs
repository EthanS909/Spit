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

        public Database database;

        MainWindow wnd = (MainWindow)Application.Current.MainWindow;

        public DispatcherTimer countDownTimer = new DispatcherTimer();
        public DispatcherTimer AIwait = new DispatcherTimer();

        public bool pickAPile = false;
        public int chosenPile = -1;

        public int tick = 0;

        const int maxPlayers = 2;
        public const int HUMAN = 0;
        public const int AI = 1;

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

        string aiFirstPileTop;
        string aiSecondPileTop;
        string aiThirdPileTop;
        string aiFourthPileTop;
        string aiFifthPileTop;

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
            get { return aiFirstPileTop; }
            set
            {
                aiFirstPileTop = value;
                OnPropertyChanged("AIFirstPileTop");
            }
        }
        public string AISecondPileTop
        {
            get { return aiSecondPileTop; }
            set
            {
                aiSecondPileTop = value;
                OnPropertyChanged("AISecondPileTop");
            }
        }
        public string AIThirdPileTop
        {
            get { return aiThirdPileTop; }
            set
            {
                aiThirdPileTop = value;
                OnPropertyChanged("AIThirdPileTop");
            }
        }
        public string AIFourthPileTop
        {
            get { return aiFourthPileTop; }
            set
            {
                aiFourthPileTop = value;
                OnPropertyChanged("AIFourthPileTop");
            }
        }
        public string AIFifthPileTop
        {
            get { return aiFifthPileTop; }
            set
            {
                aiFifthPileTop = value;
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

            database = new Database(this);
        }

        public void CreatePlayers(int difficulty)
        {
            players[HUMAN] = new HumanPlayer();
            players[AI] = new AIPlayer(this, difficulty);
        }

        public void LoadGame(int gameIndex)
        {
            database.LoadGameState(gameIndex);
        }

        public void SaveGame(int gameIndex)
        {
            database.OverrideSavedGame(gameIndex);
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
                int cardNumber = players[HUMAN].hand.piles[selectedPile].pile.Peek().GetNumber();
                int target1 = placePiles[pileNum].pile.Peek().GetNumber();
                int target2 = placePiles[pileNum].pile.Peek().GetNumber();

                if (target1 == 1) { target1 = 13; }
                else { target1 -= 1; }
                if (target2 == 13) { target2 = 1; }
                else { target2 += 1; }

                if (cardNumber == target1 || cardNumber == target2)
                {
                    placePiles[pileNum].pile.Push(players[HUMAN].hand.piles[selectedPile].pile.Pop());

                    placed = true;
                }
            }

            wnd.aiTimer.Stop();
            wnd.aiTimer.Start();

            return placed;
        }

        public void Start(int difficulty)
        {
            CreatePlayers(difficulty);

            deck.Shuffle();

            SplitCards();
            PlacePlayingCards();
            FlipOverPile();
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
                players[HUMAN].hand.pickUpPile.pile.Push(deck.deck.Pop());
            }
            for (int i = 0; i < length / 2; i++)
            {
                players[AI].hand.pickUpPile.pile.Push(deck.deck.Pop());
            }
        }

        public void PlacePlayingCards()
        {
            players[HUMAN].hand.piles[0].pile.Push(players[HUMAN].hand.pickUpPile.pile.Pop());
            for (int i = 0; i < 5; i++)
            {
                if (i < 2 && players[HUMAN].hand.pickUpPile.pile.Length() != 0)
                {
                    players[HUMAN].hand.piles[1].pile.Push(players[HUMAN].hand.pickUpPile.pile.Pop());
                }
                if (i < 3 && players[HUMAN].hand.pickUpPile.pile.Length() != 0)
                {
                    players[HUMAN].hand.piles[2].pile.Push(players[HUMAN].hand.pickUpPile.pile.Pop());
                }
                if (i < 4 && players[HUMAN].hand.pickUpPile.pile.Length() != 0)
                {
                    players[HUMAN].hand.piles[3].pile.Push(players[HUMAN].hand.pickUpPile.pile.Pop());
                }
                if (i < 5 && players[HUMAN].hand.pickUpPile.pile.Length() != 0)
                {
                    players[HUMAN].hand.piles[4].pile.Push(players[HUMAN].hand.pickUpPile.pile.Pop());
                }
            }

            players[AI].hand.piles[0].pile.Push(players[AI].hand.pickUpPile.pile.Pop());
            for (int i = 0; i < 5; i++)
            {
                if (i < 2 && players[AI].hand.pickUpPile.pile.Length() != 0)
                {
                    players[AI].hand.piles[1].pile.Push(players[AI].hand.pickUpPile.pile.Pop());
                }
                if (i < 3 && players[AI].hand.pickUpPile.pile.Length() != 0)
                {
                    players[AI].hand.piles[2].pile.Push(players[AI].hand.pickUpPile.pile.Pop());
                }
                if (i < 4 && players[AI].hand.pickUpPile.pile.Length() != 0)
                {
                    players[AI].hand.piles[3].pile.Push(players[AI].hand.pickUpPile.pile.Pop());
                }
                if (i < 5 && players[AI].hand.pickUpPile.pile.Length() != 0)
                {
                    players[AI].hand.piles[4].pile.Push(players[AI].hand.pickUpPile.pile.Pop());
                }
            }
        }

        public void CanPlay()
        {
            // Create targets
            int target1 = 0;
            int target2 = 0;
            int target3 = 0;
            int target4 = 0;

            if (!placePiles[0].pile.IsEmpty())
            {
                target1 = placePiles[0].pile.Peek().GetNumber();
                target2 = placePiles[0].pile.Peek().GetNumber();
            }
            if (!placePiles[1].pile.IsEmpty())
            {
                target3 = placePiles[1].pile.Peek().GetNumber();
                target4 = placePiles[1].pile.Peek().GetNumber();
            }

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
            foreach(Pile pile in players[HUMAN].hand.piles)
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
            int AIEmptyPiles = 0;
            foreach (Pile pile in players[AI].hand.piles)
            {
                if (!pile.pile.IsEmpty())
                {
                    int AICardNumber = pile.pile.Peek().GetNumber();

                    if (AICardNumber == target1 || AICardNumber == target2 || AICardNumber == target3 || AICardNumber == target4)
                    {
                        AICanPlay = true;
                    }
                }
                else { AIEmptyPiles++; }
            }

            if(hEmptyPiles == 5 || AIEmptyPiles == 5)
            {
                StartTimer();
                pickAPile = true;
            }
            else if(!humanCanPlay && !AICanPlay)
            {
                if(players[HUMAN].hand.pickUpPile.pile.Length() != 0 || players[AI].hand.pickUpPile.pile.Length() != 0)
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

                        FlipOverPile();
                        wnd.aiTimer.Start();
                    }
                }
                else
                {
                    /*int pile1Length = pile1.pile.Length();
                    int pile2Length = pile2.pile.Length();

                    int numOfCardToRemove = 0;

                    Stack pile1Reversed = new Stack();
                    Stack pile2Reversed = new Stack();

                    Stack pickup = new Stack();

                    if (pile1Length < pile2Length)
                    { 
                        numOfCardToRemove = pile1Length - 1;
                    }
                    else if (pile2Length < pile1Length)
                    {
                        numOfCardToRemove = pile2Length - 1;
                    }
                    else if (pile1Length == pile2Length)
                    {
                        numOfCardToRemove = pile1Length - 1;
                    }

                    for (int i = 0; i < pile1Length; i++)
                    {
                        pile1Reversed.Push(pile1.pile.Pop());
                    }
                    for (int i = 0; i < pile2Length; i++)
                    {
                        pile2Reversed.Push(pile2.pile.Pop());
                    }

                    for (int i = 0; i < numOfCardToRemove; i++)
                    {
                        pickup.Push(pile1Reversed.Pop());
                        pickup.Push(pile2Reversed.Pop());
                    }

                    for (int i = 0; i < pile1Reversed.Length(); i++)
                    {
                        pile1.pile.Push(pile1Reversed.Pop());
                    }
                    for (int i = 0; i < pile2Reversed.Length(); i++)
                    {
                        pile2.pile.Push(pile2Reversed.Pop());
                    }

                    Card[] shuffledPickup = new Card[numOfCardToRemove * 2];
                    Random rnd = new Random();
                    while (!pickup.IsEmpty())
                    {
                        int randomSpace = rnd.Next(0, (numOfCardToRemove * 2) - 1);
                        while (shuffledPickup[randomSpace] != null)
                        {
                            randomSpace = (randomSpace + 1) % shuffledPickup.Length;
                        }
                        shuffledPickup[randomSpace] = pickup.Pop();
                    }

                    for (int x = 0; x < shuffledPickup.Length; x++)
                    {
                        pickup.Push(shuffledPickup[x]);
                    }

                    for (int i = 0; i < numOfCardToRemove * 2; i++)
                    {
                        if(i <= numOfCardToRemove)
                        {
                            players[HUMAN].hand.pickUpPile.pile.Push(pickup.Pop());
                        }
                        else
                        {
                            players[AI].hand.pickUpPile.pile.Push(pickup.Pop());
                        }
                    }*/

                    wnd.Stalemate.Visibility = Visibility.Visible;

                    pickAPile = true;
                    StartTimer();
                }
            }
        }

        public void FlipOverPile()
        {
            if (players[AI].hand.pickUpPile.pile.Length() != 0)
            {
                placePiles[0].pile.Push(players[AI].hand.pickUpPile.pile.Pop());
            }
            else
            {
                wnd.aiStack.Visibility = Visibility.Hidden;
            }
            if (players[HUMAN].hand.pickUpPile.pile.Length() != 0)
            {
                placePiles[1].pile.Push(players[HUMAN].hand.pickUpPile.pile.Pop());
            }
            else
            {
                wnd.plStack.Visibility = Visibility.Hidden;
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
                placePiles[chosenPile].Unload(players[HUMAN]);
                placePiles[(chosenPile + 1) % 2].Unload(players[AI]);
            }
            else
            {
                int pile0Len = placePiles[0].pile.Length();
                int pile1Len = placePiles[1].pile.Length();

                if (pile0Len < pile1Len)
                {
                    placePiles[0].Unload(players[AI]);
                    placePiles[1].Unload(players[HUMAN]);
                }
                else
                {
                    placePiles[1].Unload(players[AI]);
                    placePiles[0].Unload(players[HUMAN]);
                }
            }

            CollectPlayingCards();
            ShuffleCards();
            PlacePlayingCards();
            FlipOverPile();
            wnd.Stalemate.Visibility = Visibility.Hidden;
            pickAPile = false;
            wnd.ResetExtraCardImages();
            wnd.updateTimer.Start();
            wnd.aiTimer.Start();
        }

        public void CollectPlayingCards()
        {
            foreach (Pile pile in players[HUMAN].hand.piles)
            {
                while (!pile.pile.IsEmpty())
                {
                    players[HUMAN].hand.pickUpPile.pile.Push(pile.pile.Pop());
                }
            }

            foreach (Pile pile in players[AI].hand.piles)
            {
                while (!pile.pile.IsEmpty())
                {
                    players[AI].hand.pickUpPile.pile.Push(pile.pile.Pop());
                }
            }
        }

        public void ShuffleCards()
        {
            ///Player pick up cards shuffling
            //Shuffle cards into array
            if(players[HUMAN].hand.pickUpPile.pile.Length() != 0)
            {
                Card[] shuffledDeck = new Card[players[HUMAN].hand.pickUpPile.pile.Length()];
                Random rnd = new Random();
                for (int x = 0; x < shuffledDeck.Length; x++)
                {
                    int randomSpace = rnd.Next(0, players[HUMAN].hand.pickUpPile.pile.Length() - 1);
                    if (players[HUMAN].hand.pickUpPile.pile.Length() != 0)
                    {
                        while (shuffledDeck[randomSpace] != null)
                        {
                            randomSpace = (randomSpace + 1) % shuffledDeck.Length;
                        }
                        shuffledDeck[randomSpace] = players[HUMAN].hand.pickUpPile.pile.Pop();
                    }
                }

                //Loads cards from array back into the players pick up pile
                for (int x = 0; x < shuffledDeck.Length; x++)
                {
                    players[HUMAN].hand.pickUpPile.pile.Push(shuffledDeck[x]);
                }
            }

            ///AI pick up cards shuffling
            //Shuffle cards into array
            if (players[AI].hand.pickUpPile.pile.Length() != 0)
            {
                Card[] shuffledDeck = new Card[players[AI].hand.pickUpPile.pile.Length()];
                Random rnd = new Random();
                for (int x = 0; x < shuffledDeck.Length; x++)
                {
                    int randomSpace = rnd.Next(0, players[AI].hand.pickUpPile.pile.Length() - 1);
                    if (players[AI].hand.pickUpPile.pile.Length() != 0)
                    {
                        while (shuffledDeck[randomSpace] != null)
                        {
                            randomSpace = (randomSpace + 1) % shuffledDeck.Length;
                        }
                        shuffledDeck[randomSpace] = players[AI].hand.pickUpPile.pile.Pop();
                    }
                }

                //Loads cards from array back into the AIs pick up pile
                for (int x = 0; x < shuffledDeck.Length; x++)
                {
                    players[AI].hand.pickUpPile.pile.Push(shuffledDeck[x]);
                }
            }
        }

        public void Update()
        {
            if (!players[HUMAN].hand.piles[0].pile.IsEmpty()) { PlayerFirstPileTop = "CardImages/" + players[HUMAN].hand.piles[0].pile.Peek().GetNumber() + "_of_" + players[HUMAN].hand.piles[0].pile.Peek().GetSuit() + "s.png"; }
            if (!players[HUMAN].hand.piles[1].pile.IsEmpty()) { PlayerSecondPileTop = "CardImages/" + players[HUMAN].hand.piles[1].pile.Peek().GetNumber() + "_of_" + players[HUMAN].hand.piles[1].pile.Peek().GetSuit() + "s.png"; }
            if (!players[HUMAN].hand.piles[2].pile.IsEmpty()) { PlayerThirdPileTop = "CardImages/" + players[HUMAN].hand.piles[2].pile.Peek().GetNumber() + "_of_" + players[HUMAN].hand.piles[2].pile.Peek().GetSuit() + "s.png"; }
            if (!players[HUMAN].hand.piles[3].pile.IsEmpty()) { PlayerFourthPileTop = "CardImages/" + players[HUMAN].hand.piles[3].pile.Peek().GetNumber() + "_of_" + players[HUMAN].hand.piles[3].pile.Peek().GetSuit() + "s.png"; }
            if (!players[HUMAN].hand.piles[4].pile.IsEmpty()) { PlayerFifthPileTop = "CardImages/" + players[HUMAN].hand.piles[4].pile.Peek().GetNumber() + "_of_" + players[HUMAN].hand.piles[4].pile.Peek().GetSuit() + "s.png"; }

            if (!players[AI].hand.piles[0].pile.IsEmpty()) { AIFirstPileTop = "CardImages/" + players[AI].hand.piles[0].pile.Peek().GetNumber() + "_of_" + players[AI].hand.piles[0].pile.Peek().GetSuit() + "s.png"; }
            if (!players[AI].hand.piles[1].pile.IsEmpty()) { AISecondPileTop = "CardImages/" + players[AI].hand.piles[1].pile.Peek().GetNumber() + "_of_" + players[AI].hand.piles[1].pile.Peek().GetSuit() + "s.png"; }
            if (!players[AI].hand.piles[2].pile.IsEmpty()) { AIThirdPileTop = "CardImages/" + players[AI].hand.piles[2].pile.Peek().GetNumber() + "_of_" + players[AI].hand.piles[2].pile.Peek().GetSuit() + "s.png"; }
            if (!players[AI].hand.piles[3].pile.IsEmpty()) { AIFourthPileTop = "CardImages/" + players[AI].hand.piles[3].pile.Peek().GetNumber() + "_of_" + players[AI].hand.piles[3].pile.Peek().GetSuit() + "s.png"; }
            if (!players[AI].hand.piles[4].pile.IsEmpty()) { AIFifthPileTop = "CardImages/" + players[AI].hand.piles[4].pile.Peek().GetNumber() + "_of_" + players[AI].hand.piles[4].pile.Peek().GetSuit() + "s.png"; }

            if (!placePiles[0].pile.IsEmpty()) Pile1Top = "CardImages/" + placePiles[0].pile.Peek().GetNumber() + "_of_" + placePiles[0].pile.Peek().GetSuit() + "s.png";
            if (!placePiles[1].pile.IsEmpty()) Pile2Top = "CardImages/" + placePiles[1].pile.Peek().GetNumber() + "_of_" + placePiles[1].pile.Peek().GetSuit() + "s.png";

            for (int i = 0; i < AICardPiles.Length; i++)
            {
                if (players[AI].hand.piles[i].pile.IsEmpty())
                {
                    wnd.aiCardPiles[i].Visibility = Visibility.Hidden;
                }
            }

            for (int i = 0; i < playerCardPiles.Length; i++)
            {
                if (players[HUMAN].hand.piles[i].pile.IsEmpty())
                {
                    wnd.plCardPiles[i].Visibility = Visibility.Hidden;
                }
            }

            if(placePiles[0].pile.IsEmpty())
            {
                wnd.placePiles[0].Visibility = Visibility.Hidden;
            }
            if (placePiles[1].pile.IsEmpty())
            {
                wnd.placePiles[1].Visibility = Visibility.Hidden;
            }

            for (int i = 0; i < players[AI].hand.piles.Length; i++)
            {
                if (!players[AI].hand.piles[i].pile.IsEmpty())
                {
                    wnd.aiCardPiles[i].Visibility = Visibility.Visible;
                }
            }

            for (int i = 0; i < players[HUMAN].hand.piles.Length; i++)
            {
                if (!players[HUMAN].hand.piles[i].pile.IsEmpty())
                {
                    wnd.plCardPiles[i].Visibility = Visibility.Visible;
                }
            }

            if (!placePiles[0].pile.IsEmpty())
            {
                wnd.placePiles[0].Visibility = Visibility.Visible;
            }
            if (!placePiles[1].pile.IsEmpty())
            {
                wnd.placePiles[1].Visibility = Visibility.Visible;
            }

            if (players[HUMAN].hand.pickUpPile.pile.IsEmpty())
            {
                wnd.plStack.Visibility = Visibility.Hidden;
            }
            else
            {
                wnd.plStack.Visibility = Visibility.Visible;
            }
            if (players[AI].hand.pickUpPile.pile.IsEmpty())
            {
                wnd.aiStack.Visibility = Visibility.Hidden;
            }
            else
            {
                wnd.aiStack.Visibility = Visibility.Visible;
            }

            CountDown = 3 - tick;

            if (!pickAPile)
            {
                CanPlay();
            }
        }
    }
}
