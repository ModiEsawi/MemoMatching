using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FinalDbProject
{
    /* The "Game" class , where the actual card matching is played */

    public partial class Game : Page
    {
        /* Setting up our private variables , which consist of timers (that are used to count a wide varity of things such as time to show
         unmatched cards , time to show card matching celebration , user solving time and much more) , dictionaries, booleans,
         strings and more..*/

        private Random random = new Random();
        private DispatcherTimer wrongGuessTimer = new DispatcherTimer();
        private DispatcherTimer celebTimer = new DispatcherTimer();
        private DispatcherTimer showSolutionTimer = new DispatcherTimer();
        private DispatcherTimer solvingTimer =  new DispatcherTimer();
        private ImageSourceConverter converter = new ImageSourceConverter();
        private Tuple<int, int> firstLocation, secondLocation;
        private Label firstCard, secondCard;
        private Dictionary<string, List<string>> firstValDict = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> secondValDict = new Dictionary<string, List<string>>();
        private List<string> icons = new List<string>();
        private Image firstGuess, secondGuess;
        private int solvingTime = 0;
        private int matches = 0;
        private string userName = "";
        private string playerName = "";
        private bool IsManager = false;
        private char difficulty;
        private char gameOption;
        private int boardSize;
        private int MAX_WIDTH = 135;
        private int MAX_HIEGHT = 130;
        private (string, string)[] CardsValue;
        private bool enterOnce = false;


        /* this function Initializes the game's components*/
        public Game()
        {
            InitializeComponent();

            ImageBrush myBrush = new ImageBrush();
            Image image = new Image();
            image.Source = new BitmapImage(
                new Uri(Directory.GetCurrentDirectory() + "\\Resources\\BackgroundImages\\GameBG.jpg"));
            myBrush.ImageSource = image.Source;
            MainGrid.Background = myBrush;
        }


        /*After a user selects a game mode and a difficulty level , we set up our grid according to the users game choices using
         this function*/
        public void SetData(string UserName, string Name, char Difficulty, char GameOption, (string, string)[] Values,bool IsManager)
        {
            this.IsManager = IsManager; // a flag that indicate if the user is a manager
            this.userName = UserName; // the players user name
            this.playerName = Name; // the players name
            this.difficulty = Difficulty; // the difficulty of the game
            this.boardSize = Difficulty - '0'; // convert string to int
            this.gameOption = GameOption; // if gameOption == 0 , this means the player choose the option the contains flag images
            this.CardsValue = Values; // the actual cards value

            /*we used dictionaries to store each card and its match , but if we take a closer look , we realize that sometimes the same
             card can have more than one value , for example ,if we look at our continents-countries table ASIA can match both Jordan and Isreal ,
             another example is athletes and countries , many athletes can match the same country and Vice Versa , to solve this "issue" we created
             tow dictionaries which have a string as a key and a LIST as a value , doing so , with the correct lookup and updates on these dictionaries
             we managed to solve this "issue" and get exactly the right behaviour we aimed for*/

            for (int i = 0; i < ((this.CardsValue.Length)); i++)
            {
                // filling up the dictionaries
                if (!firstValDict.ContainsKey(this.CardsValue[i].Item1))
                {
                    List<string> list = new List<string>();
                    list.Add(this.CardsValue[i].Item2);
                    firstValDict.Add(this.CardsValue[i].Item1, list);
                }
                else
                {
                    List<string> dictList = firstValDict[this.CardsValue[i].Item1];
                    dictList.Add(this.CardsValue[i].Item2);
                    firstValDict[this.CardsValue[i].Item1] = dictList;
                }
                if (!secondValDict.ContainsKey(this.CardsValue[i].Item2))
                {
                    List<string> list = new List<string>();
                    list.Add(this.CardsValue[i].Item1);
                    secondValDict.Add(this.CardsValue[i].Item2, list);
                }
                else
                {
                    List<string> dictList = secondValDict[this.CardsValue[i].Item2];
                    dictList.Add(this.CardsValue[i].Item1);
                    secondValDict[this.CardsValue[i].Item2] = dictList;
                }

                icons.Add(this.CardsValue[i].Item1);
                if (this.gameOption == '0') // as we explained above ,if  gameOption == 0 , this means the player choose the option the contains flag images
                {
                    icons.Add(Directory.GetCurrentDirectory() + "\\Resources\\FlagsPics\\" + this.CardsValue[i].Item2 + ".png");
                }
                else
                {
                    icons.Add(this.CardsValue[i].Item2);
                }
            }
            // now we are ready to set up our game board and place our icons inside the cards!
            SetUpBoard();
            PlaceIcons();
            StartTimer();
        }


        /* this function sets up the player's solving timer and starts it.*/
        private void StartTimer()
        {
            solvingTimer = new DispatcherTimer();
            solvingTimer.Interval = new TimeSpan(0, 0, 1);
            solvingTimer.Tick += Solving_Time_Tick;
            solvingTimer.Start();
        }

        /*this function updates the solving timer that is shown on the screen*/
        private void Solving_Time_Tick(object sender, EventArgs e)
        {
            
            solvingTime++;
            TimerDisplay.Text = string.Format("00:0{0}:{1}", solvingTime / 60, solvingTime % 60);
        }

        /*This function sets up the game board according to the users game mode and game difficulty*/
        private void SetUpBoard()
        {
            for (int i = 0; i < this.boardSize; i++)
            { // add rows and columns
                GameGrid.RowDefinitions.Add(new RowDefinition());
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 1; i <= this.boardSize; i++)
            {
                for (int j = 0; j < this.boardSize; j++)
                {
                    // create Labels
                    Label label = new Label();
                    label.MouseLeftButtonDown += CardClicked;
                    label.Foreground = Brushes.White;
                    label.MaxWidth = MAX_WIDTH;
                    label.MaxHeight = MAX_HIEGHT;
                    label.HorizontalContentAlignment = HorizontalAlignment.Center;
                    Grid.SetRow(label, i);
                    Grid.SetColumn(label, j);
                    GameGrid.Children.Add(label);

                    // create Images
                    Image img = new Image();
                    img.Source = (ImageSource)new ImageSourceConverter().ConvertFromString(Directory.GetCurrentDirectory() + "\\Resources\\question.jpg"); ;
                    img.Uid = "questionMark";
                    img.MouseLeftButtonDown += CardClicked;
                    Grid.SetRow(img, i);
                    Grid.SetColumn(img, j);
                    GameGrid.Children.Add(img);

                    // Create Borders
                    Border border = new Border();
                    border.BorderBrush = Brushes.Black;
                    border.BorderThickness = new Thickness(1);
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);
                    GameGrid.Children.Add(border);
                }
            }
            // create borders for Time and Compliment TextBlocks
            for (int i = 0; i < this.boardSize; i++)
            {
                Border border = new Border();
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new Thickness(1);
                Grid.SetRow(border, 0);
                Grid.SetColumn(border, i);
                GameGrid.Children.Add(border);
            }
            Grid.SetColumn(Compliment, this.boardSize - 1);

            if (!this.IsManager)  // check if the user is not a manager
            {
               SolveGameButton.Visibility = Visibility.Hidden;  // hide the solve game button
            }
        }

        /* this function places the strings/images inside the cards that are shown on the game board , (the function places the icons in random
         locations in each run using the built in Rnadom function*/
        private void PlaceIcons()
        {
            Label label;
            int randomNumber;
            for (int i = 0; i < GameGrid.Children.Count; i++)
            {
                if (GameGrid.Children[i] is Label)
                {
                    label = (Label)GameGrid.Children[i];
                }
                else
                {
                    continue;
                }
                randomNumber = random.Next(0, icons.Count);
                string currentIcon = icons[randomNumber];
                // checking to see if the icon that should be placed inside the card is an image
                if (currentIcon.Length >= 4)
                {
                    string result = currentIcon.Substring(currentIcon.Length - 4);
                    if (result == ".png")
                    {
                        Image img = new Image();
                        img.Source = (ImageSource)new ImageSourceConverter().ConvertFromString(currentIcon);
                        label.Content = img;
                        icons.RemoveAt(randomNumber);
                        continue;
                    }
                }
                // else it has to be a string
                TextBlock txtBlock = new TextBlock();
                txtBlock.TextWrapping = TextWrapping.Wrap;
                txtBlock.Text = icons[randomNumber];
                label.Content = txtBlock;
                label.Foreground = Brushes.DarkRed;
                label.FontWeight = FontWeights.Bold;
                label.FontSize = 17;
                icons.RemoveAt(randomNumber);
            }
        }

        /* this function takes action when a card is clicked on the game board*/
        private void CardClicked(object sender, MouseEventArgs e)
        {
            /* if both of the cards != null , then we "lock" the board by returning so the player wont click on any more cards untill we
             allow him to*/
            if (firstGuess != null && secondGuess != null)
            {
                return;
            }

            if (sender is Label)
            {
                return;
            }

            Image clickedLabel = sender as Image;
            UIElement element = (UIElement)e.Source;
            if (clickedLabel == null)
            {
                return;
            }
             // the first card is clicked
            if (firstGuess == null)
            {
                firstGuess = clickedLabel;
                firstGuess.Visibility = Visibility.Hidden; // get rid of the question mark image to show the label's content
                int firstGuessRow = Grid.GetRow(element);
                int firstGuessColumn = Grid.GetColumn(element);
                firstLocation = new Tuple<int, int>(firstGuessRow, firstGuessColumn);
                return;
            }
            // second card is clicked
            secondGuess = clickedLabel;
            int secondGuessRow = Grid.GetRow(element);
            int secondGuessColumn = Grid.GetColumn(element);
            secondLocation = new Tuple<int, int>(secondGuessRow, secondGuessColumn);

            // if the same card is clicked twice
            if ((firstLocation.Item1 == secondLocation.Item1) && (firstLocation.Item2 == secondLocation.Item2))
            {
                firstGuess.Visibility = Visibility.Visible;
                firstGuess = null;
                secondGuess = null;
                firstCard = null;
                secondCard = null;
                return;
            }
            secondGuess.Visibility = Visibility.Hidden;
            firstCard = GetLabels(firstLocation);
            secondCard = GetLabels(secondLocation);
            // checif to see if both of the cards match!
            if (CheckForHit(GetContentAsString(firstCard), GetContentAsString(secondCard)) == true)
            {
                CelebrateHit(); // celebrate the hit
                matches++; // increse the hits counter
                firstGuess = null;
                secondGuess = null;
                firstCard = null;
                secondCard = null;
                LookForWinner(); // check to see if we matched all cards
            }
            // if the cards dont match , then we keep them up for 1s so the user can see that and then we flip them back over
            else
            {
                wrongGuessTimer.Tick += new EventHandler(Wrong_Guess_Timer);
                wrongGuessTimer.Interval = new TimeSpan(0, 0, 1);
                wrongGuessTimer.Start();
                enterOnce = false;
            }
        }

        /*a labels content can be neither an image or a TextBlock , so this function get the labels content and returns it 
          using this content we actually look for card matches*/
        private string GetContentAsString(Label label)
        {
            string answer = "";
            int ENDING_LENGTH = 6;
            int FLAG_LEN = 2;
            
            if (label.Content is Image)
            {
                /*a flag name is represented as a country code which is 2 chars long , so the source will always look like : .....//**.png ,
                 so first we will keep only the last 6 chars -> "**.png" and then cut out the ".png" to get the flag's name.*/
                Image imageSource = (Image)label.Content;
                answer = imageSource.Source.ToString();
                answer = answer.Substring(answer.Length - ENDING_LENGTH);
                answer = answer.Substring(0, FLAG_LEN);
            }
            else if (label.Content is TextBlock)
            {
                TextBlock textBlock = (TextBlock)label.Content;
                answer = textBlock.Text;
            }
            return answer;
        }
        /*This function checks if the two clicked cards match*/
        private bool CheckForHit(string FirstClick, string SecondClick)
        {
            // the function checks for a match by using the dictionaries.
            if (firstValDict.ContainsKey(FirstClick))
            {
                return firstValDict[FirstClick].Contains(SecondClick);
            }
            else
            {
                return secondValDict[FirstClick].Contains(SecondClick);
            }
        }

        /*if the player matched tow cards , this function celebrates the match by changing the borders color for 1s and 
         show a random Compliment for the user of the top right corner of thr screen*/
        private void CelebrateHit()
        {
            List<string> Compliments = new List<string>()
            {
                " Well Done!", " Nice!", " Good Job!", " You Rock!", " WOW!", " Keep Going!", " Amazing!"
            };
            int randomIndex = random.Next(0, Compliments.Count);
            Compliment.Text = Compliments[randomIndex];
            celebTimer.Tick += new EventHandler(CelebrationTimer);
            celebTimer.Interval = new TimeSpan(0, 0, 1);
            Border border;
            for (int i = 0; i < GameGrid.Children.Count; i++)
            {
                if (GameGrid.Children[i] is Border)
                {
                    Brush brush = new SolidColorBrush(Color.FromRgb((byte)random.Next(0, 256), (byte)random.Next(0, 256), (byte)random.Next(0, 256)));
                    border = (Border)GameGrid.Children[i];
                    border.BorderBrush = brush;
                }
            }
            celebTimer.Start();
        }

        /*after we are done celebrating the match , this function return everything back to normal */
        private void CelebrationTimer(object source, EventArgs e)
        {
            celebTimer.Stop();
            for (int i = 0; i < GameGrid.Children.Count; i++)
            {
                Border border;
                if (GameGrid.Children[i] is Border)
                {
                    border = (Border)GameGrid.Children[i];
                    border.BorderBrush = Brushes.Black;
                }
            }
            Compliment.Text = "";
        }

        /*a "Solve game" button appears on the screen for admin users only , by clicking this button the game is solved automatically,
         (we keep the cards flipped on the screen for 1s)*/
        private void SolveGame(object sender, RoutedEventArgs e)
        {
            showSolutionTimer.Tick += new EventHandler(SetUpForNextPage);
            showSolutionTimer.Interval = new TimeSpan(0, 0, 1);
            Image image;
            for (int i = 0; i < GameGrid.Children.Count; i++)
            {
                if (GameGrid.Children[i] is Image)
                {
                    image = (Image)GameGrid.Children[i];
                    if (image.Uid == "questionMark")
                    {
                        image.Visibility = Visibility.Hidden;
                    }
                }
            }
            showSolutionTimer.Start();
        }

        /* after we have shown the solution on the screen for the admin user , we set up out variables for the next page by setting the hits counter
         * to be equal to the boards amount of hits needed to end the game*/
        private void SetUpForNextPage(object sender, EventArgs e)
        {
            showSolutionTimer.Stop();
            matches = (int)(Math.Pow(this.boardSize, 2) / 2);
            LookForWinner();
        }

        /*when the player makes a wrong guess , we keep the cards flipped for 1s so the player can see that the cards
          dont match and then flip them back over , this is done by this function*/
        private void Wrong_Guess_Timer(object sender, EventArgs e)
        {
            if (enterOnce == false)
            {
                wrongGuessTimer.Stop();
                firstGuess.Visibility = Visibility.Visible;
                secondGuess.Visibility = Visibility.Visible;
                firstGuess = null;
                secondGuess = null;
                enterOnce = true;
            }
        }
        /*this argument is actually a certain label's location o the game board grid , it hold a row and a column ' 
         this function find this certain label by its given row and column*/
        private Label GetLabels(Tuple<int, int> tuple)
        {
            Label label;

            for (int i = 0; i < GameGrid.Children.Count; i++)
            {
                if (GameGrid.Children[i] is Label)
                {
                    label = (Label)GameGrid.Children[i];
                    if (Grid.GetRow(label) == tuple.Item1 && Grid.GetColumn(label) == tuple.Item2)
                    {
                        return label;
                    }
                }
            }
            return null;
        }

        /* this function checks if all the cards have been match , if they have , then we move on to the next page*/
        private void LookForWinner()
        {
            if (matches == (Math.Pow(this.boardSize, 2) / 2))
            {
                string playerTime = TimerDisplay.Text;
                // create the next page
                PlayerResult playerResult = new PlayerResult(playerTime, this.userName, this.difficulty);
                // move on to the next page
                this.NavigationService.Navigate(playerResult);
            }
        }
    }
}