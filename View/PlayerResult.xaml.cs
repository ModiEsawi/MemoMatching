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
    /*This page shows the player the time it took him to tolve the game , and displays a button for the player to see the
     leader boards table*/
    public partial class PlayerResult : Page
    {
        private String playerSolvingTime;
        private string userName;
        private char difficulty;
        // setting up the class's variables in this constructor
        public PlayerResult(String playerTime, string username, char difficulty)
        {
            InitializeComponent();

            ImageBrush myBrush = new ImageBrush();
            Image image = new Image();
            image.Source = new BitmapImage(
                new Uri(Directory.GetCurrentDirectory() + "\\Resources\\BackgroundImages\\GameOverBG.jpg"));
            myBrush.ImageSource = image.Source;
            MainGrid.Background = myBrush;

            this.playerSolvingTime = playerTime;
            this.userName = username;
            this.difficulty = difficulty;
            // showing the player his solving time.
            SolvingTime.Text = this.playerSolvingTime.ToString();
        }

        /*when the "Leader Board" button is clicked , this function activates and takes us to the leader board table*/
        private void LeaderBoard_Button(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Record_Table(this.userName, this.playerSolvingTime, this.difficulty));
        }

    }
}
