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
    /// Interaction logic for InformationSelection.xaml
    /// </summary>
    public partial class InformationSelection : Page
    {
        private ShowInformation showInfo; // Show information object
        private string header1, header2; // The two headers, thease are the titles of the table in the next page
        public InformationSelection()
        {
            InitializeComponent();

            // Add the background image
            ImageBrush myBrush = new ImageBrush();
            Image image = new Image();
            image.Source = new BitmapImage(
                new Uri(Directory.GetCurrentDirectory() + "\\Resources\\BackgroundImages\\InformationBG.jpg"));
            myBrush.ImageSource = image.Source;
            MainGrid.Background = myBrush;
        }

        private void ButtonAthletes_Click(object sender, RoutedEventArgs e)
        {
            this.header1 = "Country";
            this.header2 = "Athletes";
            this.GoToTableWithOutQuery(false, 10, '4');
        }

        private void ButtonCoronaDeaths_Click(object sender, RoutedEventArgs e)
        {
            this.header1 = "deaths";
            this.header2 = "Date";
            this.GoToCoronaTable(this.header1);
        }

        private void ButtonCities_Click(object sender, RoutedEventArgs e)
        {
            this.header1 = "Country";
            this.header2 = "City";
            this.GoToTableWithOutQuery(false, 10, '1');
        }

        private void ButtonCoronaCases_Click(object sender, RoutedEventArgs e)
        {
            this.header1 = "cases";
            this.header2 = "Date";
            this.GoToCoronaTable(this.header1);
        }

        private void ButtonFlags_Click(object sender, RoutedEventArgs e)
        {
            this.header1 = "Country";
            this.header2 = "Flag Imge";
            this.GoToTableWithOutQuery(true, 10, '0');
        }

        private void ButtonLanguages_Click(object sender, RoutedEventArgs e)
        {
            this.header1 = "Language";
            this.header2 = "Native";
            this.GoToTableWithOutQuery(false, 10, '6');
        }

        private void Go_Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(GameMenu.Instance());
        }


        //here we will go to the table that knows to show the corona info with the dates. the title will be date and the parameter column
        private void GoToCoronaTable(string column)
        {
            this.NavigationService.Navigate(new View.CoronaInfo(this.header1, this.header2, column));
        }

        private void ButtonCountryLanguage_Click(object sender, RoutedEventArgs e)
        {
            this.header1 = "Country";
            this.header2 = "Language";
            this.GoToTableWithOutQuery(false, 10, '9');
        }


        //here we already have the query in option so we call it from there
        private void GoToTableWithOutQuery(bool isFlag, int numberofelem, char selection)
        {
            this.showInfo = new ShowInformation(selection, this.header1, this.header2, numberofelem);
            if (isFlag)  // check if we want the flags or not
            {
                this.showInfo.FlagsComing();
            }
            this.NavigationService.Navigate(this.showInfo);
        }
    }
}
