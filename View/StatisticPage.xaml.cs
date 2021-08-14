using FinalDbProject.Controllers;
using MySqlX.XDevAPI.Relational;
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

namespace FinalDbProject.View
{
    /// <summary>
    /// Interaction logic for StatisticPage.xaml
    /// </summary>
    public partial class StatisticPage : Page
    {
        private StatisticController statisticController;
        public StatisticPage()
        {
            InitializeComponent();

            // Add the background image
            ImageBrush myBrush = new ImageBrush();
            Image image = new Image();
            image.Source = new BitmapImage(
                new Uri(Directory.GetCurrentDirectory() + "\\Resources\\BackgroundImages\\StatisticsBG.jpg"));
            myBrush.ImageSource = image.Source;
            window.Background = myBrush;

            this.statisticController = new StatisticController();
        }

        //this function changed the size of the table according to the window
        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.FlowDoc.ColumnWidth = e.NewSize.Width;
        }

        //this function return us to the prevoius page
        private void Go_Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        //in case of exception we will move to exception page
        private void MoveToException(Exception e)
        {
            this.NavigationService.Navigate(new ExceptionPage(e));
        }

        //this function gets an information to insert to a line, and the function returns a tablerow object with that info.
        private TableRow AddLineToTable(string data, string[] str_line = null,int[] int_line = null,double [] double_line = null)
        {
            Paragraph data_P = new Paragraph();
            Paragraph Easy_P = new Paragraph();
            Paragraph Medium_P = new Paragraph();
            Paragraph Hard_P = new Paragraph();


            TableRow row = new TableRow();
            
            //insert to the paragraphs
            data_P.Inlines.Add(data);

            if (str_line != null)
            {
                Easy_P.Inlines.Add(str_line[0]);
                Medium_P.Inlines.Add(str_line[1]);
                Hard_P.Inlines.Add(str_line[2]);
            } else if (int_line != null)
            {
                Easy_P.Inlines.Add(int_line[0].ToString());
                Medium_P.Inlines.Add(int_line[1].ToString());
                Hard_P.Inlines.Add(int_line[2].ToString());
            }
            else
            {
                Easy_P.Inlines.Add(double_line[0].ToString());
                Medium_P.Inlines.Add(double_line[1].ToString());
                Hard_P.Inlines.Add(double_line[2].ToString());
            }

            //insert to cells
            TableCell data_C = new TableCell(data_P);
            TableCell Easy_C = new TableCell(Easy_P);
            Easy_C.FlowDirection = FlowDirection.RightToLeft;
            TableCell Medium_C = new TableCell(Medium_P);
            Medium_C.FlowDirection = FlowDirection.RightToLeft;
            TableCell Hard_C = new TableCell(Hard_P);
            Hard_C.FlowDirection = FlowDirection.RightToLeft;

            //insert cells to row
            row.Cells.Add(data_C);
            row.Cells.Add(Easy_C);
            row.Cells.Add(Medium_C);
            row.Cells.Add(Hard_C);

            return row;
        }


        //this function build the table
        private void BuildTable(string[]HigheScore,int[]numPlayers, double[] avg)
        {
            TableRowGroup teg = new TableRowGroup();  // creating a new tablerowgroup object that will save all the new rows
            //insert all rows
            teg.Rows.Add(this.AddLineToTable("Players num",null,numPlayers));
            teg.Rows.Add(this.AddLineToTable("Players avg (sec)", null,null,avg));
            teg.Rows.Add(this.AddLineToTable("HigheScore", HigheScore));

            //insert the rowgourp to the table
            this.my_statistic_table.RowGroups.Add(teg);
        }

        //this function will create the statistic table
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            int[] numPlayerLine = new int[3];
            double[] avgLine = new double[3];
            string[] highScoreLine = new string[3];
            try
            {
                // take the number of players from each difficulty
                numPlayerLine[0] = this.statisticController.GetNumberOfPlayers('4');
                numPlayerLine[1] = this.statisticController.GetNumberOfPlayers('6');
                numPlayerLine[2] = this.statisticController.GetNumberOfPlayers('8');

                // calculate the avg
                avgLine = this.statisticController.Avg();


                //take the best score from each difficulty
                highScoreLine[0] = this.statisticController.GetBestScore('4');
                highScoreLine[1] = this.statisticController.GetBestScore('6');
                highScoreLine[2] = this.statisticController.GetBestScore('8');

                this.BuildTable(highScoreLine, numPlayerLine, avgLine);  //build the table
            }
            catch(Exception ex)
            {
                this.MoveToException(ex);
            }
        }
    }
}
