﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Spit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int difficultyCount = 0;
        private string[] difficulty = { "Beginer", "Intermediate", "Advanced" };
        private int[] reactionTime = { 1500, 1200, 900};
        private int[] movesAhead = { 1, 2, 3};

        public List<Button> plCardPiles = new List<Button>();
        public List<Image> aiCardPiles = new List<Image>();
        public List<Button> placePiles = new List<Button>();

        List<Image> placePile1 = new List<Image>();
        List<Image> placePile2 = new List<Image>();

        Game spit;

        public DispatcherTimer aiTimer = new DispatcherTimer();
        public DispatcherTimer updateTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            AI_Difficulty.Text = "Reaction Time: " + reactionTime[difficultyCount] + "ms\r\nLooks Ahead " + movesAhead[difficultyCount] + " Moves";
            AI.Content = difficulty[difficultyCount];
            SetDataContext();
            plCardPiles.Add(plPile1);
            plCardPiles.Add(plPile2);
            plCardPiles.Add(plPile3);
            plCardPiles.Add(plPile4);
            plCardPiles.Add(plPile5);

            aiCardPiles.Add(aiPile1);
            aiCardPiles.Add(aiPile2);
            aiCardPiles.Add(aiPile3);
            aiCardPiles.Add(aiPile4);
            aiCardPiles.Add(aiPile5);

            placePiles.Add(pile1);
            placePiles.Add(pile2);

            aiTimer.Tick += AiPlay;

            updateTimer.Tick += Update;
            updateTimer.Interval = TimeSpan.FromMilliseconds(10);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!e.Handled && e.Key == Key.Escape && Keyboard.Modifiers == ModifierKeys.None && Play.Visibility != Visibility.Visible)
            {
                if (Back.Visibility == Visibility.Visible)
                {
                    if(background.Visibility == Visibility.Hidden)
                    {
                        DisplayPlayUI(false);
                        DisplayStartUI(true);
                        DisplayPauseMenu(false);
                    }
                    else
                    {
                        DisplayRulesUI(false);
                        DisplayPauseMenu(true);
                    }
                }
                else if (ExitGame.Visibility == Visibility.Visible)
                {
                    DisplayPauseMenu(false);
                    DrawingText.Visibility = Visibility.Visible;
                    aiTimer.Start();
                    updateTimer.Start();
                    if(spit.tick > 0)
                    {
                        spit.countDownTimer.Start();
                    }
                }
                else
                {
                    DisplayPauseMenu(true);
                    DrawingText.Visibility = Visibility.Hidden;
                    aiTimer.Stop();
                    updateTimer.Stop();
                    spit.countDownTimer.Stop();
                }
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            //Hide Start Screen UI
            DisplayStartUI(false);

            //Display AI Difficulty Levels
            DisplayPlayUI(true);
        }

        private void Rules_Click(object sender, RoutedEventArgs e)
        {
            //Disable Start Screen UI
            DisplayStartUI(false);
            if(background.Visibility == Visibility.Visible)
            {
                DisplayPauseMenu(false);
                background.Visibility = Visibility.Visible;
            }

            DisplayRulesUI(true);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AI_Click(object sender, RoutedEventArgs e)
        {
            DisplayPlayUI(false);
            spit = new Game();
            SetDataContext();
            spit.Start(difficultyCount);
            DisplayGameUI(true);

            aiTimer.Interval = TimeSpan.FromMilliseconds(spit.players[Game.AI].GetDelay());
            aiTimer.Start();
            updateTimer.Start();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            spit = new Game();
            SetDataContext();
            spit.LoadGame();

            spit.Start(difficultyCount);
            DisplayGameUI(true);

            aiTimer.Interval = TimeSpan.FromMilliseconds(spit.players[Game.AI].GetDelay());
            aiTimer.Start();
            updateTimer.Start();
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            spit.SaveGame();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if(background.Visibility == Visibility.Visible)
            {
                DisplayRulesUI(false);
                DisplayPauseMenu(true);
            } else
            {
                // Display Start Screen UI
                DisplayStartUI(true);

                // Hide AI Difficulty Levels
                DisplayPlayUI(false);
            }
        }

        /*private void Back2_Click(object sender, RoutedEventArgs e)
        /{
            DisplayPauseMenu(true);
        }*/

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            if(AI.Content != difficulty[0])
            {
                difficultyCount--;
                AI.Content = difficulty[difficultyCount];
                AI_Difficulty.Text = "Reaction Time: " + reactionTime[difficultyCount] + "ms\r\nLooks Ahead " + movesAhead[difficultyCount] + " Moves";
            }
            
        }
        private void Right_Click(object sender, RoutedEventArgs e)
        {
            if (AI.Content != difficulty[2])
            {
                difficultyCount++;
                AI.Content = difficulty[difficultyCount];
                AI_Difficulty.Text = "Reaction Time: " + reactionTime[difficultyCount] + "ms\r\nLooks Ahead " + movesAhead[difficultyCount] + " Moves";
            }
        }

        private void DisplayStartUI(bool enabled)
        {
            if(enabled)
            {
                Play.Visibility = Visibility.Visible;
                Rules.Visibility = Visibility.Visible;
                Exit.Visibility = Visibility.Visible;
            }
            else
            {
                Play.Visibility = Visibility.Hidden;
                Rules.Visibility = Visibility.Hidden;
                Exit.Visibility = Visibility.Hidden;
            }
        }

        private void DisplayPlayUI(bool enabled)
        {
            if(enabled)
            {
                AI.Visibility = Visibility.Visible;
                Load.Visibility = Visibility.Visible;
                Back.Visibility = Visibility.Visible;
                Left.Visibility = Visibility.Visible;
                Right.Visibility = Visibility.Visible;
                AI_Difficulty.Visibility = Visibility.Visible;
            }
            else
            {
                AI.Visibility = Visibility.Hidden;
                Load.Visibility = Visibility.Hidden;
                Back.Visibility = Visibility.Hidden;
                Left.Visibility = Visibility.Hidden;
                Right.Visibility = Visibility.Hidden;
                AI_Difficulty.Visibility = Visibility.Hidden;
            } 
        }

        private void DisplayGameUI(bool enabled)
        {
            if (enabled)
            {
                plPile1.Visibility = Visibility.Visible;
                plPile2.Visibility = Visibility.Visible;
                plPile3.Visibility = Visibility.Visible;
                plPile4.Visibility = Visibility.Visible;
                plPile5.Visibility = Visibility.Visible;

                aiPile1.Visibility = Visibility.Visible;
                aiPile2.Visibility = Visibility.Visible;
                aiPile3.Visibility = Visibility.Visible;
                aiPile4.Visibility = Visibility.Visible;
                aiPile5.Visibility = Visibility.Visible;

                plStack.Visibility = Visibility.Visible;
                aiStack.Visibility = Visibility.Visible;

                pile1.Visibility = Visibility.Visible;
                pile2.Visibility = Visibility.Visible;
            }
            else
            {
                plPile1.Visibility = Visibility.Hidden;
                plPile2.Visibility = Visibility.Hidden;
                plPile3.Visibility = Visibility.Hidden;
                plPile4.Visibility = Visibility.Hidden;
                plPile5.Visibility = Visibility.Hidden;

                aiPile1.Visibility = Visibility.Hidden;
                aiPile2.Visibility = Visibility.Hidden;
                aiPile3.Visibility = Visibility.Hidden;
                aiPile4.Visibility = Visibility.Hidden;
                aiPile5.Visibility = Visibility.Hidden;

                plStack.Visibility = Visibility.Hidden;
                aiStack.Visibility = Visibility.Hidden;

                pile1.Visibility = Visibility.Hidden;
                pile2.Visibility = Visibility.Hidden;
            }
        }

        private void Card_Click(object sender, RoutedEventArgs e)
        {
            Button pile = (Button)sender;

            int index = 0;

            for(int i = 0; i < plCardPiles.Count; i++)
            {
                if (plCardPiles[i] == pile)
                {
                    index = i;
                }
            }

            if(pile.BorderThickness != new Thickness(0))
            {
                SelectPile(pile, index);
            }
            else
            {
                DeselectPile();
            }
        }

        private void SelectPile(Button selected, int index)
        {
            foreach(Button b in plCardPiles)
            {
                b.BorderThickness = new Thickness(6);
            }
            selected.BorderThickness = new Thickness(0);
            spit.selectedPile = index;
        }

        private void DeselectPile()
        {
            for(int index = 0; index < plCardPiles.Count; index++)
            {
                plCardPiles[index].BorderThickness = new Thickness(5);
            }
            spit.selectedPile = -1;
        }

        private void PlaceCard(object sender, RoutedEventArgs e)
        {
            Button pile = (Button)sender;

            int index = 0;

            for (int i = 0; i < placePiles.Count; i++)
            {
                if (placePiles[i] == pile)
                {
                    index = i;
                }
            }

            if (spit.pickAPile == true)
            {
                spit.chosenPile = index;
                spit.AIwait.Stop();
                spit.ChoosePile();
                DeselectPile();
            }
            else
            {
                bool placed = spit.Place(index);

                if (placed) { DeselectPile(); }
            }
        }

        private void ExitGame_Click(object sender, RoutedEventArgs e)
        {
            // Hides the Game UI
            DisplayGameUI(false);

            // Displays the Start Screen UI so you can start a new game
            DisplayStartUI(true);

            DisplayPauseMenu(false);

            ResetExtraCardImages();
        }

        private void DisplayPauseMenu(bool enabled)
        {
            if(enabled)
            {
                background.Visibility = Visibility.Visible;
                Rules2.Visibility = Visibility.Visible;
                Save.Visibility = Visibility.Visible;
                ExitGame.Visibility = Visibility.Visible;
            }
            else
            {
                background.Visibility = Visibility.Hidden;
                Rules2.Visibility = Visibility.Hidden;
                Save.Visibility = Visibility.Hidden;
                ExitGame.Visibility = Visibility.Hidden;
            }
        }

        public void DisplayRulesUI(bool enabled)
        {
            if (enabled)
            {
                Back.Visibility = Visibility.Visible;
            }
            else
            {
                Back.Visibility = Visibility.Hidden;
            }
        }

        private void SetDataContext()
        {
            DataContext = spit;
        }

        public void AiPlay(object sender, EventArgs e)
        {
            spit.players[1].Move();
        }

        public void Update(object sender, EventArgs e)
        {
            spit.Update();
            GenerateExtraCardImages(0, -1);
        }

        int ZIndex = 51;
        int[] nextInts = { 10, 10, 200, 170 };
        Thickness next = new Thickness(10, 10, 200, 170);
        int numberOfCards = 0;
        public void GenerateExtraCardImages(int pileIndex, int player)
        {
            //10,10,190,170
            //20,20,175,160
            int length = spit.pile1.pile.Length();
            int i = 0;

            if (player == -1)
            {
                for (i = 0; i < (length / 3) - numberOfCards; i++)
                {
                    Image newCardImage = new Image();
                    newCardImage.Margin = next;
                    newCardImage.Source = plStack.Source;
                    Grid.SetColumn(newCardImage, 2);
                    Grid.SetRow(newCardImage, 2);
                    Grid.SetRowSpan(newCardImage, 3);
                    Grid.SetColumnSpan(newCardImage, 2);
                    Grid.SetZIndex(newCardImage, ZIndex);
                    ZIndex -= 1;
                    screen.Children.Add(newCardImage);

                    placePile1.Add(newCardImage);

                    nextInts[0] += 5;
                    nextInts[1] += 5;
                    nextInts[2] -= 7;
                    nextInts[3] -= 5;
                    next = new Thickness(nextInts[0], nextInts[1], nextInts[2], nextInts[3]);
                }
                numberOfCards += i;
            }
            else if (player == Game.HUMAN)
            {

            } else if (player == Game.AI)
            {

            }
        }

        public void ResetExtraCardImages()
        {
            for (int i = 0; i < placePile1.Count; i++)
            {
                screen.Children.Remove(placePile1[i]);
            }
            placePile1 = new List<Image>();

            ZIndex = 51;
            nextInts[0] = 10;
            nextInts[1] = 10;
            nextInts[2] = 200;
            nextInts[3] = 170;
            next = new Thickness(10, 10, 200, 170);
            numberOfCards = 0;
        }
    }
}
