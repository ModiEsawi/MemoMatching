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
    /// Interaction logic for ExceptionPage.xaml
    /// </summary>
    public partial class ExceptionPage : Page
    {
        private Exception my_Ex;
        public ExceptionPage(Exception e)
        {
            InitializeComponent();

            ImageBrush myBrush = new ImageBrush();
            Image image = new Image();
            image.Source = new BitmapImage(
                new Uri(Directory.GetCurrentDirectory() + "\\Resources\\BackgroundImages\\ErrorBG.jpg"));
            myBrush.ImageSource = image.Source;
            MainGrid.Background = myBrush;

            this.my_Ex = e;
        }

        //here we return to the main window page
        private void HomeReturn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(MainWindow.Instance());
        }

        //when the page will load, show the exception
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.showException.Text = this.my_Ex.Message;
        }
    }
}
