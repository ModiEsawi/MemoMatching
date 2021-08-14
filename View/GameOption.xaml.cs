using System;
using System.Collections.Generic;
using System.IO;
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

namespace FinalDbProject
{
    /// <summary>
    /// Interaction logic for GameOption.xaml
    /// Game options codes:
    ///                     0 - Country_FlagPicture
    ///                     1 - Country_City
    ///                     2 - Country_Continent
    ///                     3 - Country_Language
    ///                     4 - Country_Athlete
    ///                     5 - City_Population
    ///                     6 - Language_Native
    ///                     7 - Country_CoronaCases
    ///                     8 - Country_CoronaDeaths
    /// </summary>
    public partial class GameOption : Page
    {
        private string username = ""; // The username
        private string name = ""; // The name of the user
        private char difficulty = '4'; // The game difficulty lebel: 4- Easy, 6- Medium, 8- Hard
        private GameOptionController opt = GameOptionController.Instance(); // The option treatment
        private Game game = new Game(); // The game object


        /// <summary>
        /// The constructor.
        /// </summary>
        public GameOption()
        {
            InitializeComponent();

            // Add the background image
            ImageBrush myBrush = new ImageBrush();
            Image image = new Image();
            image.Source = new BitmapImage(
                new Uri(Directory.GetCurrentDirectory() + "\\Resources\\BackgroundImages\\HomeBG.jpg"));
            myBrush.ImageSource = image.Source;
            MainGrid.Background = myBrush;
        }


        /// <summary>
        /// If there is an error occure so move to the exception page.
        /// </summary>
        /// <param name="e"></param>
        private void MoveToException(Exception e)
        {
            this.NavigationService.Navigate(new ExceptionPage(e));
        }


        /// <summary>
        /// The constructor (get external data [i.e. username, name and difficulty]).
        /// </summary>
        public void setData(string username, string name, char difficulty)
        {
            InitializeComponent();
            this.username = username;
            this.name = name;
            this.difficulty = difficulty;
        }


        /// <summary>
        /// Get the boardSize. The board size is the number oftuples that we need to match in the game.
        /// (x is even number): this.difficulty = 'x' ===> boardSize = (x^2) / 2.
        /// For instance: this.difficulty = '4' (easy level) ===> boardSize = (4^2) / 2 = 8.
        /// </summary>
        /// <returns>
        /// The board size (this.difficulty is even number, so the answer is an integer).
        /// </returns>
        private int GetBoardSize()
        {
            int boardOrder = this.difficulty - '0'; // The order of the board (like the difficulty ranking)
            return (boardOrder * boardOrder / 2);
        }


        /// <summary>
        /// This function gets the gameoption and starts the game.
        /// </summary>
        /// <param name="gameOption"></param>
        private void GetData(char gameOption)
        {
            try
            {
                (string, string)[] data = this.opt.GetData(gameOption, this.GetBoardSize()); // Array of pairs of the choosen data
                bool IsManager = this.opt.IsManager(this.username);
                this.game.SetData(this.username, this.name, this.difficulty, gameOption, data, IsManager);
                this.NavigationService.Navigate(this.game);
            }
            catch (Exception e)
            {
                this.MoveToException(e);
            }
        }


        /// <summary>
        /// Go to the game with the selected difficult level,
        /// and option 0: Country-FlagPicture.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Country_FlagPicture_Click(object sender, RoutedEventArgs e)
        {
            this.GetData('0');
        }


        /// <summary>
        /// Go to the game with the selected difficult level,
        /// and option 1: Country-City.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Country_City_Click(object sender, RoutedEventArgs e)
        {
            this.GetData('1');
        }


        /// <summary>
        /// Go to the game with the selected difficult level,
        /// and option 2: Country-Continet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Country_Continent_Click(object sender, RoutedEventArgs e)
        {
            this.GetData('2');
        }


        /// <summary>
        /// Go to the game with the selected difficult level,
        /// and option 3: Country-Language.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Country_Language_Click(object sender, RoutedEventArgs e)
        {
            this.GetData('3');
        }


        /// <summary>
        /// Go to the game with the selected difficult level,
        /// and option 4: Country-Athlete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Country_Athlete_Click(object sender, RoutedEventArgs e)
        {
            this.GetData('4');
        }


        /// <summary>
        /// Go to the game with the selected difficult level,
        /// and option 5: City-Population.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_City_Population_Click(object sender, RoutedEventArgs e)
        {
            this.GetData('5');
        }


        /// <summary>
        /// Go to the game with the selected difficult level,
        /// and option 6: Language-Native.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Language_Native_Click(object sender, RoutedEventArgs e)
        {
            this.GetData('6');
        }


        /// <summary>
        /// Go to the game with the selected difficult level,
        /// and option 7: Country-CoronaCases.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Country_CoronaCases_Click(object sender, RoutedEventArgs e)
        {
            this.GetData('7');
        }


        /// <summary>
        /// Go to the game with the selected difficult level,
        /// and option 8: Country-CoronaDeaths.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Country_CoronaDeaths_Click(object sender, RoutedEventArgs e)
        {
            this.GetData('8');
        }


        /// <summary>
        /// Go Back to the previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}