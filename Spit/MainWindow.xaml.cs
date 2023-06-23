using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Spit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int difficultyCount = 0;
        private string[] difficulty = { "Easy", "Normal", "Hard" };
        private int[] reactionTime = { 350, 250, 150};
        private int[] movesAhead = { 1, 2, 3};

        List<Button> cardPiles = new List<Button>();
        List<Button> placePiles = new List<Button>();

        private Database database = new Database();

        Game spit = new Game();

        public MainWindow()
        {
            InitializeComponent();
            AI_Difficulty.Text = "Reaction Time: " + reactionTime[difficultyCount] + "ms\r\nLooks Ahead " + movesAhead[difficultyCount] + " Moves";
            AI.Content = difficulty[difficultyCount];
            DataContext = spit;
            cardPiles.Add(plPile1);
            cardPiles.Add(plPile2);
            cardPiles.Add(plPile3);
            cardPiles.Add(plPile4);
            cardPiles.Add(plPile5);

            placePiles.Add(pile1);
            placePiles.Add(pile2);
        }

        public void Update()
        {

        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            //Hide Start Screen UI
            DisplayStartUI(false);

            //Display AI Difficulty Levels
            DisplayPlayUI(true);


            //Start Game
            // Spit game = new Spit();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            //Disable Start Screen UI
            DisplayStartUI(false);

            Back.Visibility = Visibility.Visible;


        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AI_Click(object sender, RoutedEventArgs e)
        {
            DisplayPlayUI(false);
            spit.Start(difficultyCount);
            DisplayGameUI();

        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            //spit.Load();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            //Display Start Screen UI
            DisplayStartUI(true);

            //Hide AI Difficulty Levels
            DisplayPlayUI(false);
        }

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

        private void DisplayGameUI()
        {
            plPile1.Visibility = Visibility.Visible;
            plPile2.Visibility = Visibility.Visible;
            plPile3.Visibility = Visibility.Visible;
            plPile4.Visibility = Visibility.Visible;
            plPile5.Visibility = Visibility.Visible;

            plStack.Visibility = Visibility.Visible;
            aiStack.Visibility = Visibility.Visible;

            pile1.Visibility = Visibility.Visible;
            pile2.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
            foreach (Button b in cardPiles)
            {
                b.BorderThickness = new Thickness(5);
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
    }
}
