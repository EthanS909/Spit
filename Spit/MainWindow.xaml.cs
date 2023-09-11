using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
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
        private string[] difficulty = { "Easy", "Normal", "Hard" };
        private int[] reactionTime = { 1000, 350, 250};
        private int[] movesAhead = { 1, 2, 3};

        List<Button> cardPiles = new List<Button>();
        List<Button> placePiles = new List<Button>();

        private Database database = new Database();

        Game spit = new Game();

        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            AI_Difficulty.Text = "Reaction Time: " + reactionTime[difficultyCount] + "ms\r\nLooks Ahead " + movesAhead[difficultyCount] + " Moves";
            AI.Content = difficulty[difficultyCount];
            SetDataContext();
            cardPiles.Add(plPile1);
            cardPiles.Add(plPile2);
            cardPiles.Add(plPile3);
            cardPiles.Add(plPile4);
            cardPiles.Add(plPile5);

            placePiles.Add(pile1);
            placePiles.Add(pile2);

            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += new EventHandler(Tick);
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
                        DisplaySettingsUI(false);
                        DisplayPauseMenu(true);
                    }
                }
                else if (ExitGame.Visibility == Visibility.Visible)
                {
                    DisplayPauseMenu(false);
                    timer.Start();
                }
                else
                {
                    DisplayPauseMenu(true);
                    timer.Stop();
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

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            //Disable Start Screen UI
            DisplayStartUI(false);
            if(background.Visibility == Visibility.Visible)
            {
                DisplayPauseMenu(false);
                background.Visibility = Visibility.Visible;
            }

            DisplaySettingsUI(true);
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

            timer.Interval = TimeSpan.FromMilliseconds(spit.players[1].GetDelay());
            timer.Start();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            //spit.Load();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if(background.Visibility == Visibility.Visible)
            {
                DisplaySettingsUI(false);
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
                Settings.Visibility = Visibility.Visible;
                Exit.Visibility = Visibility.Visible;
            }
            else
            {
                Play.Visibility = Visibility.Hidden;
                Settings.Visibility = Visibility.Hidden;
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

            for(int i = 0; i < cardPiles.Count; i++)
            {
                if (cardPiles[i] == pile)
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
            foreach(Button b in cardPiles)
            {
                b.BorderThickness = new Thickness(6);
            }
            selected.BorderThickness = new Thickness(0);
            spit.selectedPile = index;
        }

        private void DeselectPile()
        {
            for(int index = 0; index < cardPiles.Count; index++)
            {
                cardPiles[index].BorderThickness = new Thickness(5);

                if (spit.IsPileEmpty(index))
                {
                    cardPiles[index].Visibility = Visibility.Hidden;
                }
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

            bool placed = spit.Place(index);

            if (placed) { DeselectPile(); spit.Update(); }
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExitGame_Click(object sender, RoutedEventArgs e)
        {
            // Hides the Game UI
            DisplayGameUI(false);

            // Displays the Start Screen UI so you can start a new game
            DisplayStartUI(true);

            DisplayPauseMenu(false);
        }

        private void DisplayPauseMenu(bool enabled)
        {
            if(enabled)
            {
                background.Visibility = Visibility.Visible;
                Settings2.Visibility = Visibility.Visible;
                Save.Visibility = Visibility.Visible;
                ExitGame.Visibility = Visibility.Visible;
            }
            else
            {
                background.Visibility = Visibility.Hidden;
                Settings2.Visibility = Visibility.Hidden;
                Save.Visibility = Visibility.Hidden;
                ExitGame.Visibility = Visibility.Hidden;
            }
        }

        public void DisplaySettingsUI(bool enabled)
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

        public void Tick(object sender, EventArgs e)
        {
            //Debug.WriteLine("playing");
            spit.players[1].Move();
        }
    }
}
