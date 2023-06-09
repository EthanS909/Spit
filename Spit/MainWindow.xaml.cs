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

        private Database database = new Database();

        Game spit = new Game();

        public MainWindow()
        {
            InitializeComponent();
            AI_Difficulty.Text = "Reaction Time: " + reactionTime[difficultyCount] + "ms\r\nLooks Ahead " + movesAhead[difficultyCount] + " Moves";
            AI.Content = difficulty[difficultyCount];
            DataContext = spit;
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
            cardButton.Visibility = Visibility.Visible;
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

        private void OnClick5(object sender, RoutedEventArgs e)
        {

        }
    }
}
