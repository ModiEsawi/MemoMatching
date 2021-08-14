using FinalDbProject.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

namespace FinalDbProject.View
{
    /// <summary>
    /// Interaction logic for CoronaInfo.xaml
    /// </summary>
    public partial class CoronaInfo : Page
    {
        private string header1, header2;  // the titles of the table
        private bool firstDate, secondDate;  // this flags will help us to know if the user select a date or not
        private CoronaController coronaController;  // the controller of this info
        private string column;  // save the column we want to take from the db
        private string[][] dates;
        private bool ClearFlag; //this flag will indicate if the selection function called because of clear or not

        public CoronaInfo(string header1, string header2, string column)
        {
            InitializeComponent();

            ImageBrush myBrush = new ImageBrush();
            Image image = new Image();
            image.Source = new BitmapImage(
                new Uri(Directory.GetCurrentDirectory() + "\\Resources\\BackgroundImages\\CoronaBG.jpg"));
            myBrush.ImageSource = image.Source;
            window.Background = myBrush;


            this.header1 = header1;//the first title
            this.header2 = header2;//the second title
            //the falgs initialize to be false
            this.firstDate = false;
            this.secondDate = false;
            this.ClearFlag = false;
            this.coronaController = CoronaController.GetInstance(); //get an instance of the controller
            this.coronaController.SetColumn(column);
            this.column = column;
        }

        //in case of exception we will move to exception page
        private void MoveToException(Exception e)
        {
            this.NavigationService.Navigate(new ExceptionPage(e));
        }


        // here we load all the countries we have in the corona table
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //change the title of the table
            this.title1.Inlines.Add(this.header1);
            this.title2.Inlines.Add(this.header2);

            //set the values of the flags
            this.firstDate = false;
            this.secondDate = false;

            try
            {
                string[] countriesName = this.coronaController.GetAllCountries(); // get the names of the countries
                foreach(string name in countriesName)
                {
                    this.country.Items.Add(name); //add it to the combobox 
                }
            }catch(Exception ex)
            {
                this.MoveToException(ex);
            }
        }


        //this function builds the combo box for the dates
        private void split_dates(ComboBox combo, int HalfOne,int startPos = 0)
        {
            this.ClearFlag = true;
            //remove all items from the date combobox
            combo.Items.Clear();
            this.ClearFlag = false;

            for(int i = startPos; i < HalfOne; i++)
            {
                combo.Items.Add(dates[i][0].Split(' ')[0]);
            }
        }


        //this function changed the size of the table according to the window
        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.FlowDoc.ColumnWidth = e.NewSize.Width;
        }

        //if the user select a country
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //get all the dates of the selected country from the covid19 table
                this.dates = this.coronaController.GetDate(this.country.SelectedItem.ToString());
                if (this.second_date.Items.Count > 0) //check if we need to clear the combox of the second date
                {
                    this.ClearFlag = true;
                    this.second_date.Items.Clear();
                    this.ClearFlag = false;
                }
                this.firstDate = false;
                this.secondDate = false;
                this.second_date.Items.Add("Please Select First Date First");
                this.split_dates(this.first_date, this.dates.Length - 1); // set the date in first date
            }catch(Exception ex)
            {
                this.MoveToException(ex);
            }
        }


        //this function builds the table
        private void buildTable(int length, string[] column1, string[] column2)
        {
            //first check if we need to earse the current rowgroup
            if (this.my_information_table.RowGroups.Count > 1)
            {
                this.my_information_table.RowGroups.RemoveAt(1);
            }
            TableRowGroup teg = new TableRowGroup();  // creating a new tablerowgroup object that will save all the new rows
            for (int i = 0; i < length; i++)
            {
                TableRow row = new TableRow();  // for each row creating a new row object
                // create a paragraph for each column
                Paragraph firstColumn_p = new Paragraph();
                Paragraph secondColumn_p = new Paragraph();

                firstColumn_p.Inlines.Add(column2[i]);
                secondColumn_p.Inlines.Add(column1[i].Split(' ')[0]);

                // insert each paragraph to the correct cell
                TableCell firstColumn_c = new TableCell(firstColumn_p);
                TableCell secondColumn_c = new TableCell(secondColumn_p);

                // insert each cell to the row in a specific order
                row.Cells.Add(firstColumn_c);
                row.Cells.Add(secondColumn_c);

                // insert the row to the table row group object
                teg.Rows.Add(row);
            }
            //insert the table row group to the table
            this.my_information_table.RowGroups.Add(teg);  // after inserting all the rows
        }

        private Tuple<string[], string[]> split_array(string[][] data, int length)
        {
            string[] firstElem = new string[length];
            string[] secondElem = new string[length];
            for (int i = 0; i < length; i++)
            {
                firstElem[i] = data[i][0];
                secondElem[i] = data[i][1];
            }
            return Tuple.Create(firstElem, secondElem);
        }


        //this function will give us the information
        private void Show_Info(object sender, RoutedEventArgs e)
        {
            this.Message_P.Inlines.Clear(); //erase the past message if exist
            try
            {
                if (this.firstDate && this.secondDate) // enter here only if the user chose all fields
                {
                    string[][] data = this.coronaController.GetData(this.country.SelectedItem.ToString(), this.first_date.SelectedItem.ToString(), this.second_date.SelectedItem.ToString());
                    int length = data.Length;
                    Tuple<string[], string[]> arrays = this.split_array(data, length);
                    this.buildTable(length, arrays.Item1, arrays.Item2);
                    string sum = this.coronaController.GetSum(this.country.SelectedItem.ToString(), this.first_date.SelectedItem.ToString(), this.second_date.SelectedItem.ToString());
                    this.Message_P.Inlines.Add(this.column + ":" + sum);
                }
                else
                {
                    this.Message_P.Inlines.Add("Please Select All Fields");
                }
            } catch(Exception ex)
            {
                this.MoveToException(ex);
            }
        }

        //set the first date flag to be true
        private void first_date_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //enter just if the clear didnt activate it
            if (!this.ClearFlag)
            {
                if (!this.first_date.SelectedItem.ToString().Contains("Please Select Country First"))
                {
                    this.firstDate = true;
                    this.secondDate = false;
                    this.split_dates(this.second_date, this.dates.Length, this.first_date.SelectedIndex + 1); // build the second date
                }
            }
        }

        //set the second date flag to be true
        private void second_date_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //enter just if the clear didnt activate it
            if (!this.ClearFlag)
            {
                if (!this.second_date.SelectedItem.ToString().Contains("Please Select First Date First"))
                {
                    this.secondDate = true;
                }
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();  // return to the previouse page
        }
    }
}
