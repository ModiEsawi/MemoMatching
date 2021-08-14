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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Page
    {
        private static MainWindow instance = null; // The self object


        /// <summary>
        /// The constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Add the background image
            ImageBrush myBrush = new ImageBrush();
            Image image = new Image();
            image.Source = new BitmapImage(
                new Uri(Directory.GetCurrentDirectory() + "\\Resources\\BackgroundImages\\HomeBG.jpg"));
            myBrush.ImageSource = image.Source;
            MainGrid.Background = myBrush;

            // Connection MySql headers
            string[] connectionArr = { "localhost", "3305", "team12", "root", "database2021!" };
            GameOptionController opt = GameOptionController.Instance();          
            opt.SetInfo(connectionArr);
        }


        /// <summary>
        /// Return the self object.
        /// </summary>
        /// <returns></returns>
        public static MainWindow Instance()
        {
            if (instance == null)
            {
                instance = new MainWindow();
            }
            return instance;
        }


        /// <summary>
        /// Redirect to the Login page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(Login.Instance());
        }


        /// <summary>
        /// Redirect to the Register page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(Register.Instance());
        }
    }
}

