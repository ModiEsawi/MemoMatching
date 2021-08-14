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
    /// Interaction logic for ShowInformation.xaml
    /// </summary>
    public partial class ShowInformation : Page
    {
        private GameOptionController opt = GameOptionController.Instance();
        private string header1, header2;
        private bool Flags = false;
        private char selection;  // this field will tell me if the query exist in option or not.
        private int numberOfElements;
        public ShowInformation(char selection, string header1, string header2, int numberofElements)
        {
            InitializeComponent();

            ImageBrush myBrush = new ImageBrush();
            Image image = new Image();
            image.Source = new BitmapImage(
                new Uri(Directory.GetCurrentDirectory() + "\\Resources\\BackgroundImages\\InformationBG.jpg"));
            myBrush.ImageSource = image.Source;
            window.Background = myBrush;

            this.numberOfElements = numberofElements;
            this.selection = selection;
            this.header1 = header1;
            this.header2 = header2;
        }


        // this function will indicate us if the option that was selected is flags
        public void FlagsComing()
        {
            this.Flags = true;
        }

        private Tuple<string[],string[]> split_array((string,string)[] data, int length)
        {
            string[] firstElem = new string[length];
            string[] secondElem = new string[length];
            for (int i = 0; i < length; i++)
            {
                firstElem[i] = data[i].Item1;
                secondElem[i] = data[i].Item2;
            }
            return Tuple.Create(firstElem, secondElem);
        }


        //this function builds the table
        private void buildTable(int length, string[] column1, object [] column2)
        {
            TableRowGroup teg = new TableRowGroup();  // creating a new tablerowgroup object that will save all the new rows
            for (int i = 0; i < length; i++)
            {
                TableRow row = new TableRow();  // for each row creating a new row object
                // create a paragraph for each column
                Paragraph firstColumn_p = new Paragraph();
                Paragraph secondColumn_p = new Paragraph();

                firstColumn_p.Inlines.Add(column1[i]);

                if (this.Flags)  // check if the value is image or string and do downcasting according
                {
                    secondColumn_p.Inlines.Add((UIElement)column2[i]);
                }
                else
                {
                    secondColumn_p.Inlines.Add((string)column2[i]);
                }

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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //first check if we need to earse the current rowgroup
            if (this.my_information_table.RowGroups.Count > 1)
            {
                this.my_information_table.RowGroups.RemoveAt(1);
            }
            try
            {
                //get the data about the selection that the user wanted
                (string, string)[] data = this.opt.GetData(this.selection, this.numberOfElements);
                int length = data.Length;
                Tuple<string[], string[]> arrays = this.split_array(data, length);
                string[] column1 = arrays.Item1;
                object[] column2;
                if (this.Flags) //check if we need to do get image flags or not.
                {
                    column2 = this.opt.GetFlagsImages(arrays.Item2);
                }
                else
                {
                    column2 = arrays.Item2;
                }
                this.buildTable(length, column1, column2);
            }catch(Exception ex)
            {
                this.NavigationService.Navigate(new ExceptionPage(ex));
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        { 
            //set the titles
            this.title1.Inlines.Add(this.header1);
            this.title2.Inlines.Add(this.header2);
        }

        //this function changed the size of the table according to the window
        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.FlowDoc.ColumnWidth = e.NewSize.Width;
        }


        //go to the prevoiuse page
        private void Go_Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
