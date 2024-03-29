﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Spit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int difficultyCount = 0;
        private string[] difficulty = { "Beginner", "Intermediate", "Advanced" };
        private int[] reactionTime = { 1500, 1200, 900};
        private int[] movesAhead = { 1, 3, 5};

        public List<Button> plCardPiles = new List<Button>();
        public List<Image> aiCardPiles = new List<Image>();
        public List<Button> placePiles = new List<Button>();

        public List<Button> emptyPiles = new List<Button>();

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
                if (SaveScreen.Visibility != Visibility.Visible)
                {
                    if (Back.Visibility == Visibility.Visible)
                    {
                        if (background.Visibility == Visibility.Hidden)
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
                        if(WinningText.Visibility == Visibility.Hidden)
                        {
                            if(!spit.humanCanPlay && !spit.AICanPlay)
                            {
                                DrawingText.Visibility = Visibility.Visible;
                            }
                            aiTimer.Start();
                            updateTimer.Start();
                            if (spit.tick > 0)
                            {
                                spit.countDownTimer.Start();
                            }
                        }
                        else
                        {
                            ExitGame.Visibility = Visibility.Visible;
                        }
                    } 
                    else
                    {
                        if(WinningText.Visibility == Visibility.Hidden)
                        {
                            DisplayPauseMenu(true);
                            DrawingText.Visibility = Visibility.Hidden;
                            aiTimer.Stop();
                            updateTimer.Stop();
                            spit.countDownTimer.Stop();
                        }
                    }
                }
                else
                {
                    SaveScreen.Visibility = Visibility.Hidden;
                    DisplayGameUI(true);
                    DisplayPauseMenu(true);

                    foreach (Image i in placePile1)
                    {
                        i.Visibility = Visibility.Visible;
                    }
                    foreach (Image i in placePile2)
                    {
                        i.Visibility = Visibility.Visible;
                    }

                    ShowEmptyPiles();
                }
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            DisplaySavedGameState();
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
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
            {
                App.Current.Windows[intCounter].Close();
            }
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

        private void DisplayLoadUI(object sender, RoutedEventArgs e)
        {
            DisplayPlayUI(false);
            LoadScreen.Visibility = Visibility.Visible;
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            pre.Visibility = Visibility.Hidden;

            Button button = (Button)sender;
            int gameIndex = Convert.ToInt32(button.Name[4]) - 48;

            spit = new Game();
            SetDataContext();
            spit.LoadGame(gameIndex);

            if(!spit.database.nothingSavedToShow)
            {
                DisplayPlayUI(false);
                DisplayGameUI(true);
                LoadScreen.Visibility = Visibility.Hidden;

                updateTimer.Start();

                aiTimer.Interval = TimeSpan.FromMilliseconds(spit.players[Game.AI].GetDelay());
                aiTimer.Start();
            }
            spit.database.nothingSavedToShow = false;
        }

        private void DisplaySaveUI(object sender, RoutedEventArgs e)
        {
            DisplayGameUI(false);
            DisplayPauseMenu(false);
            CountDown.Visibility = Visibility.Hidden;
            SaveScreen.Visibility = Visibility.Visible;

            foreach(Image i in placePile1)
            {
                i.Visibility = Visibility.Hidden;
            }
            foreach (Image i in placePile2)
            {
                i.Visibility = Visibility.Hidden;
            }

            HideEmptyPiles();
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int gameIndex = Convert.ToInt32(button.Name[4]) - 48;

            SaveGamePreview(gameIndex);

            spit.SaveGame(gameIndex);
            DisplaySavedGameState();
        }

        private void LoadScreenExit_Click(object sender, RoutedEventArgs e)
        {
            DisplayPlayUI(true);
            LoadScreen.Visibility = Visibility.Hidden;
            LoadScreenExit.Visibility = Visibility.Hidden;
        }

        private void SaveScreenExit_Click(object sender, RoutedEventArgs e)
        {
            DisplayPauseMenu(true);
            DisplayGameUI(true);
            SaveScreen.Visibility = Visibility.Hidden;

            foreach (Image i in placePile1)
            {
                i.Visibility = Visibility.Visible;
            }
            foreach (Image i in placePile2)
            {
                i.Visibility = Visibility.Visible;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            DisplayRulesUI(false);
            if (background.Visibility == Visibility.Visible)
            {
                DisplayPauseMenu(true);
            } else
            {
                // Display Start Screen UI
                DisplayStartUI(true);

                // Hide AI Difficulty Levels
                DisplayPlayUI(false);
            }
        }

        private void EmptyPile_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (spit.pickAPile)
            {
                if (button.Name == "emptyPile1")
                {
                    spit.chosenPile = 0;
                }
                else if (button.Name == "emptyPile2")
                {
                    spit.chosenPile = 1;
                }
                spit.ChoosePile();
            }
        }

        public void WinCheck()
        {
            if (spit.winningPlayer != null)
            {
                string winningPlayer = spit.winningPlayer == 0 ? "Human" : "AI";
                WinningText.Text = String.Format("{0} has won the game!", winningPlayer);
                WinningText.Visibility = Visibility.Visible;
                DisplayGameUI(false);
                ExitGame.Visibility = Visibility.Visible;

                foreach (Image i in placePile1)
                {
                    i.Visibility = Visibility.Hidden;
                }
                foreach (Image i in placePile2)
                {
                    i.Visibility = Visibility.Hidden;
                }
                emptyPile1.Visibility = Visibility.Hidden;
                emptyPile2.Visibility = Visibility.Hidden;
                HideEmptyPiles();
            }
        }

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            if (AI.Content != difficulty[0])
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
            if (enabled)
            {
                title.Visibility = Visibility.Visible;
                Play.Visibility = Visibility.Visible;
                Rules.Visibility = Visibility.Visible;
                Exit.Visibility = Visibility.Visible;
            }
            else
            {
                title.Visibility = Visibility.Hidden;
                Play.Visibility = Visibility.Hidden;
                Rules.Visibility = Visibility.Hidden;
                Exit.Visibility = Visibility.Hidden;
            }
        }

        private void DisplayPlayUI(bool enabled)
        {
            if (enabled)
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
            pre.Source = null;

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

            for (int i = 0; i < plCardPiles.Count; i++)
            {
                if (plCardPiles[i] == pile)
                {
                    index = i;
                }
            }

            if (spit.selectedPile != -1 && spit.selectedPile != index)
            {
                if (spit.players[0].hand.piles[index].pile.Peek().GetNumber() == spit.players[0].hand.piles[spit.selectedPile].pile.Peek().GetNumber())
                {
                    spit.players[0].hand.piles[index].pile.Push(spit.players[0].hand.piles[spit.selectedPile].pile.Pop());
                    PlayingCardsEmptyPiles(spit.selectedPile);
                    DeselectPile();
                    return;
                }
            }

            SelectPile(pile, index);
        }

        private void SelectPile(Button selected, int index)
        {
            if(spit.selectedPile != index)
            {
                foreach (Button b in plCardPiles)
                {
                    b.BorderThickness = new Thickness(6);
                }
                selected.BorderThickness = new Thickness(0);
                spit.selectedPile = index;
            }
            else
            {
                DeselectPile();
            }
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

            if (spit.pickAPile)
            {
                spit.chosenPile = index;
                spit.AIwait.Stop();
                spit.ChoosePile();
                DeselectPile();
            }
            else
            {
                bool placed = spit.Place(index);

                if (placed) { PlayingCardsEmptyPiles(spit.selectedPile); DeselectPile(); }
            }
        }

        private void ExitGame_Click(object sender, RoutedEventArgs e)
        {
            // Hides the Game UI
            DisplayGameUI(false);
            WinningText.Visibility = Visibility.Hidden;
            CountDown.Visibility = Visibility.Hidden;
            DrawingText.Visibility = Visibility.Hidden;

            // Displays the Start Screen UI so you can start a new game
            DisplayStartUI(true);

            DisplayPauseMenu(false);

            ResetExtraCardImages();
            
            RemoveEmptyPiles();
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
                RulesText.Visibility = Visibility.Visible;
            }
            else
            {
                Back.Visibility = Visibility.Hidden;
                RulesText.Visibility = Visibility.Hidden;
            }
        }

        private void SetDataContext()
        {
            DataContext = spit;
        }

        public void AiPlay(object sender, EventArgs e)
        {
            spit.players[Game.AI].Move();
        }

        public void Update(object sender, EventArgs e)
        {
            spit.Update();
            GenerateExtraCardImages();
        }

        int pile1ZIndex = 51;
        int pile2ZIndex = 51;
        int[] pile1NextInts = { 10, 10, 200, 170 };
        Thickness pile1Next = new Thickness(10, 10, 200, 170);
        int[] pile2NextInts = { 0, 7, -5, -7 };
        Thickness pile2Next = new Thickness(0, 7, -5, -7);
        int numOfCardsInPile1 = 0;
        int numOfCardsInPile2 = 0;
        public void GenerateExtraCardImages()
        {
            int pile1Length = spit.placePiles[0].pile.Length();
            int pile2Length = spit.placePiles[1].pile.Length();
            int i = 0;
            for (i = 0; i < (pile1Length / 3) - numOfCardsInPile1; i++)
            {
                Image newCardImage = new Image();
                newCardImage.Margin = pile1Next;
                newCardImage.Source = plStack.Source;
                Grid.SetColumn(newCardImage, 2);
                Grid.SetRow(newCardImage, 2);
                Grid.SetRowSpan(newCardImage, 3);
                Grid.SetColumnSpan(newCardImage, 2);
                Grid.SetZIndex(newCardImage, pile1ZIndex);
                pile1ZIndex -= 1;
                screen.Children.Add(newCardImage);

                placePile1.Add(newCardImage);

                pile1NextInts[0] += 5;
                pile1NextInts[1] += 5;
                pile1NextInts[2] -= 7;
                pile1NextInts[3] -= 5;
                pile1Next = new Thickness(pile1NextInts[0], pile1NextInts[1], pile1NextInts[2], pile1NextInts[3]);
            }
            numOfCardsInPile1 += i;

            for (i = 0; i < (pile2Length / 3) - numOfCardsInPile2; i++)
            {
                Image newCardImage = new Image();
                newCardImage.Margin = pile2Next;
                newCardImage.Source = plStack.Source;
                Grid.SetColumn(newCardImage, 4);
                Grid.SetRow(newCardImage, 2);
                Grid.SetRowSpan(newCardImage, 2);
                Grid.SetZIndex(newCardImage, pile2ZIndex);
                pile2ZIndex -= 1;
                screen.Children.Add(newCardImage);

                placePile2.Add(newCardImage);

                pile2NextInts[0] += 10;
                pile2NextInts[1] += 8;
                pile2NextInts[2] -= 5;
                pile2NextInts[3] -= 8;
                pile2Next = new Thickness(pile2NextInts[0], pile2NextInts[1], pile2NextInts[2], pile2NextInts[3]);
            }
            numOfCardsInPile2 += i;
        }

        public void ResetExtraCardImages()
        {
            for (int i = 0; i < placePile1.Count; i++)
            {
                screen.Children.Remove(placePile1[i]);
            }
            placePile1 = new List<Image>();

            pile1ZIndex = 51;
            pile1NextInts[0] = 10;
            pile1NextInts[1] = 10;
            pile1NextInts[2] = 200;
            pile1NextInts[3] = 170;
            pile1Next = new Thickness(10, 10, 200, 170);
            numOfCardsInPile1 = 0;

            for (int i = 0; i < placePile2.Count; i++)
            {
                screen.Children.Remove(placePile2[i]);
            }
            placePile2 = new List<Image>();

            pile2ZIndex = 51;
            pile2NextInts[0] = 0;
            pile2NextInts[1] = 7;
            pile2NextInts[2] = -5;
            pile2NextInts[3] = -7;
            pile2Next = new Thickness(0, 7, -5, -7);
            numOfCardsInPile2 = 0;
        }

        public void DisplaySavedGameState()
        {
            Database database = new Database(null);

            string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string[] game = { "gameSave1", "gameSave2", "gameSave3", "gameSave4" };
            string destination1 = new string(path + @"\\" + game[0] + ".png");
            string destination2 = new string(path + @"\\" + game[1] + ".png");
            string destination3 = new string(path + @"\\" + game[2] + ".png");
            string destination4 = new string(path + @"\\" + game[3] + ".png");

            if (database.CheckSavedGame(1) == 1)
            {
                Load1Image.Source = new BitmapImage(new Uri("full_file.png", UriKind.Relative));
                Save1Image.Source = new BitmapImage(new Uri("full_file.png", UriKind.Relative));
            }
            else
            {
                Load1Image.Source = new BitmapImage(new Uri("empty_file.png", UriKind.Relative));
                Save1Image.Source = new BitmapImage(new Uri("empty_file.png", UriKind.Relative));
                if(System.IO.File.Exists(destination1))
                {
                    System.IO.File.Delete(destination1);
                }
            }
            if (database.CheckSavedGame(2) == 1)
            {
                Load2Image.Source = new BitmapImage(new Uri("full_file.png", UriKind.Relative));
                Save2Image.Source = new BitmapImage(new Uri("full_file.png", UriKind.Relative));
            }
            else
            {
                Load2Image.Source = new BitmapImage(new Uri("empty_file.png", UriKind.Relative));
                Save2Image.Source = new BitmapImage(new Uri("empty_file.png", UriKind.Relative));
                if (System.IO.File.Exists(destination2))
                {
                    System.IO.File.Delete(destination2);
                }
            }
            if (database.CheckSavedGame(3) == 1)
            {
                Load3Image.Source = new BitmapImage(new Uri("full_file.png", UriKind.Relative));
                Save3Image.Source = new BitmapImage(new Uri("full_file.png", UriKind.Relative));
            }
            else
            {
                Load3Image.Source = new BitmapImage(new Uri("empty_file.png", UriKind.Relative));
                Save3Image.Source = new BitmapImage(new Uri("empty_file.png", UriKind.Relative));
                if (System.IO.File.Exists(destination3))
                {
                    System.IO.File.Delete(destination3);
                }
            }
            if (database.CheckSavedGame(4) == 1)
            {
                Load4Image.Source = new BitmapImage(new Uri("full_file.png", UriKind.Relative));
                Save4Image.Source = new BitmapImage(new Uri("full_file.png", UriKind.Relative));
            }
            else
            {
                Load4Image.Source = new BitmapImage(new Uri("empty_file.png", UriKind.Relative));
                Save4Image.Source = new BitmapImage(new Uri("empty_file.png", UriKind.Relative));
                if (System.IO.File.Exists(destination4))
                {
                    System.IO.File.Delete(destination4);
                }
            }
        }

        public void SaveGamePreview(int gameIndex)
        {
            DisplayGameUI(true);
            SaveScreen.Visibility = Visibility.Hidden;

            UIElement gameScreen = screen as UIElement;
            string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string[] game = { "gameSave1", "gameSave2", "gameSave3", "gameSave4" };
            Uri destination = new Uri(path + @"\\" + game[gameIndex - 1] + ".png");

            try
            {
                double screenHeight = gameScreen.RenderSize.Height;
                double screenWidth = gameScreen.RenderSize.Width;

                RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)screenWidth, (int)screenHeight, 96, 96, PixelFormats.Pbgra32);
                VisualBrush visualBrush = new VisualBrush(gameScreen);

                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(0, 0), new Point(screenWidth, screenHeight)));
                }
                renderTarget.Render(drawingVisual);

                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));
                using (FileStream stream = new FileStream(destination.LocalPath, FileMode.Create, FileAccess.Write))
                {
                    encoder.Save(stream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            DisplayGameUI(false);
            SaveScreen.Visibility = Visibility.Visible;
        }

        public void ShowGamePreview(object sender, RoutedEventArgs e)
        {
            try
            {
                int gameIndex = 0;

                Button button = (Button)sender;
                string temp = button.Name;
                string[] parts = temp.Split("Load");
                gameIndex = Convert.ToInt32(parts[1]) - 1;

                string[] game = { "gameSave1", "gameSave2", "gameSave3", "gameSave4" };
                string path = String.Format(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\\" + game[gameIndex] + ".png");

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path);
                bitmap.EndInit();

                pre.Source = bitmap;
                pre.Visibility = Visibility.Visible;
            } catch (Exception ex)
            {
                Debug.WriteLine("Game not saved in this slot!");
            }
        }
        public void HideGamePreview(object sender, RoutedEventArgs e)
        {
            pre.Visibility = Visibility.Hidden;
        }

        public Dictionary<Button, int> emptyPilesDict = new Dictionary<Button, int>();
        public void PlayingCardsEmptyPiles(int index)
        {
            if (spit.players[0].hand.piles[index].pile.IsEmpty())
            {
                Button emptyPile = new Button();
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("CardImages/card_outline.png", UriKind.Relative));
                emptyPile.Margin = plCardPiles[index].Margin;
                emptyPile.Content = image;
                emptyPile.Background = Brushes.Transparent;
                emptyPile.BorderBrush = Brushes.Transparent;
                emptyPile.Click += PlayingEmptyPile_Click;
                Grid.SetColumn(emptyPile, index + 1);
                Grid.SetRow(emptyPile, 4);
                Grid.SetRowSpan(emptyPile, 2);
                emptyPilesDict.Add(emptyPile, index);
                emptyPiles.Add(emptyPile);
                screen.Children.Add(emptyPile);

                DeselectPile();
            }
        }

        public void PlayingEmptyPile_Click(object sender, RoutedEventArgs e)
        {
            if (spit.selectedPile == -1)
            {
                return;
            }

            Button button = (Button)sender;

            int index = 0;

            for (int i = 0; i < emptyPiles.Count; i++)
            {
                if (emptyPiles[i] == button)
                {
                    index = i;
                }
            }

            if (spit.players[Game.HUMAN].hand.piles[emptyPilesDict[emptyPiles[index]]].pile.IsEmpty())
            {
                spit.players[Game.HUMAN].hand.piles[emptyPilesDict[emptyPiles[index]]].pile.Push(spit.players[0].hand.piles[spit.selectedPile].pile.Pop());

                plCardPiles[emptyPilesDict[emptyPiles[index]]].Visibility = Visibility.Visible;
                screen.Children.Remove(emptyPiles[index]);
                emptyPiles.Remove(emptyPiles[index]);

                PlayingCardsEmptyPiles(spit.selectedPile);
                DeselectPile();
            }
        }

        public void RemoveEmptyPiles()
        {
            for (int i = 0; i < emptyPiles.Count; i++)
            {
                screen.Children.Remove(emptyPiles[i]);
            }
            emptyPiles = new List<Button>();
            emptyPilesDict = new Dictionary<Button, int>();
        }

        public void HideEmptyPiles()
        {
            for (int i = 0; i < emptyPiles.Count; i++)
            {
                emptyPiles[i].Visibility = Visibility.Hidden;
            }
        }
        public void ShowEmptyPiles()
        {
            for (int i = 0; i < emptyPiles.Count; i++)
            {
                emptyPiles[i].Visibility = Visibility.Visible;
            }
        }
    }
}
