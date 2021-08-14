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
    /// Interaction logic for DifficultySelection.xaml
    /// </summary>
    public partial class DifficultySelection : Page
    {
        private string username = ""; // The username
        private string name = ""; // The name of the user
        private GameOption go; // The object of GameOption
        private static DifficultySelection instance = null; // The self object


        /// <summary>
        /// The constructor.
        /// </summary>
        private DifficultySelection()
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
        public static DifficultySelection Instance()
        {
            if (instance == null)
            {
                instance = new DifficultySelection();
            }
            return instance;
        }


        /// <summary>
        /// Set the data (get external data [i.e. username and name]).
        /// </summary>
        public void setData(string username, string name)
        {
            InitializeComponent();
            this.go = new GameOption();
            this.username = username;
            this.name = name;
        }


        /// <summary>
        /// Go to select game option (with easy level).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonEasy_Click(object sender, RoutedEventArgs e)
        {
            this.go.setData(this.username, this.name, '4');
            this.NavigationService.Navigate(this.go);
        }


        /// <summary>
        /// Go to select game option (with medium level).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonMedium_Click(object sender, RoutedEventArgs e)
        {
            this.go.setData(this.username, this.name, '6');
            this.NavigationService.Navigate(this.go);
        }


        /// <summary>
        /// Go to select game option (with hard level).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonHard_Click(object sender, RoutedEventArgs e)
        {
            this.go.setData(this.username, this.name, '8');
            this.NavigationService.Navigate(this.go);
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
