using FinalDbProject.View;
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
    /// Interaction logic for GameMenu.xaml
    /// </summary>
    public partial class GameMenu : Page
    {

        private string username = ""; // The username
        private string name = ""; // The name of the user
        private static GameMenu instance = null; // Self object
        private GameOptionController opt = GameOptionController.Instance(); // the option instance


        /// <summary>
        /// The constructor.
        /// </summary>
        private GameMenu()
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
        /// Return the self object.
        /// </summary>
        /// <returns></returns>
        public static GameMenu Instance()
        {
            if (instance == null)
            {
                instance = new GameMenu();
            }
            return instance;
        }


        /// <summary>
        /// Sets the data about the user.
        /// </summary>
        /// <param name="username"> UserName </param>
        /// <param name="name"> The name of the user </param>
        public void setData(string username, string name)
        {
            InitializeComponent();

            this.username = username;
            this.name = name;

            LabelHelloName.Content = "Hello " + this.name;
        }


        /// <summary>
        /// Logout: Go to the main page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonLogout_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(MainWindow.Instance());
        }


        /// <summary>
        /// Play the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            DifficultySelection ds = DifficultySelection.Instance(); // Difficulty level selection
            ds.setData(this.username, this.name);
            this.NavigationService.Navigate(ds);
        }


        /// <summary>
        /// Go to statistics page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStatistics_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new StatisticPage());
        }


        /// <summary>
        /// Go to information page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonInformation_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new InformationSelection());
        }


        /// <summary>
        ///  This function will be called when the page is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                bool IsManager = this.opt.IsManager(this.username); //chekc if the username is a manager
                if (IsManager)
                {
                    this.DeletePlayers.Visibility = Visibility.Visible; //if yes make the delete button to be visible
                }else
                {
                    this.DeletePlayers.Visibility = Visibility.Hidden; // if no make the delte button to be hidden
                }
            }
            catch(Exception ex)
            {
                this.NavigationService.Navigate(new ExceptionPage(ex));
            }
        }


        /// <summary>
        /// Delete players page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeletePlayers_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new DeletePage());
        }
    }
}
