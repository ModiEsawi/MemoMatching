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
    /// Interaction logic for Record_Table.xaml
    /// </summary>
    public partial class Record_Table : Page
    {
        private string player_username, player_score;
        private int My_Time = 0; //this is the current player time in second
        private string [] PlayerTime;
        private char difficulty;
        private UserRecordsController RecordInterface;
        public Record_Table(string username, string score, char diff)
        {
            InitializeComponent();

            ImageBrush myBrush = new ImageBrush();
            Image image = new Image();
            image.Source = new BitmapImage(
                new Uri(Directory.GetCurrentDirectory() + "\\Resources\\BackgroundImages\\RecordsBG.jpg"));
            myBrush.ImageSource = image.Source;
            window.Background = myBrush;

            // get the username of the player and his score
            this.player_username = username;
            this.player_score = "";
            string [] split_score = score.Split(':');
            this.PlayerTime = new string[3];
            this.PlayerTime[0] = split_score[2]; //sec
            this.My_Time += int.Parse(this.PlayerTime[0]);
            this.PlayerTime[1] = split_score[1]; //min
            this.My_Time += int.Parse(this.PlayerTime[1]) * 60;
            this.PlayerTime[2] = split_score[0]; //hour
            this.My_Time += int.Parse(this.PlayerTime[2]) * 3600;
            this.difficulty = diff;  // save the difficulty of the game

            this.RecordInterface = new UserRecordsController();
        }
        // This function is responsible to build the record table, from a given names and times array.
        private void create_record_table(string[] names, string[] times)
        {
            int length = names.Length;
            TableRowGroup teg = new TableRowGroup();  // creating a new tablerowgroup object that will save all the new rows
            for (int i = 0; i < length; i++)
            {
                TableRow row = new TableRow();  // for each row creating a new row object
                // create a paragraph for each column
                Paragraph place_p = new Paragraph();
                Paragraph name_p = new Paragraph();
                Paragraph time_p = new Paragraph();

                // insert the paragraph for each of them
                if (i == 0 || i == 1 || i == 2)  // if its the first,second or third place bring a picture.
                {
                    // define the image
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    var path = Directory.GetCurrentDirectory() + "\\Resources";  // get the working directory relative path
                    bi.UriSource = new Uri(path + "/" + (i + 1).ToString() + "th.png");  // take the correct pic
                    bi.EndInit();
                    Image img = new Image();
                    img.Source = bi;
                    // set the width and height of the picture.
                    img.Width = 25;
                    img.Height = 25;
                    place_p.Inlines.Add(img);
                }
                else // just write a number
                {
                    place_p.Inlines.Add((i + 1).ToString() + ")");
                }
                if (names[i] == this.player_username)  // if this is the row of our layer, so color it!
                {
                    row.Background = Brushes.Yellow;
                }
                name_p.Inlines.Add(names[i]);
                time_p.Inlines.Add(times[i]);

                // insert each paragraph to the correct cell
                TableCell place_cell = new TableCell(place_p);
                TableCell name_cell = new TableCell(name_p);
                TableCell time_cell = new TableCell(time_p);

                // insert each cell to the row in a specific order
                row.Cells.Add(place_cell);
                row.Cells.Add(name_cell);
                row.Cells.Add(time_cell);

                // insert the row to the table row group object
                teg.Rows.Add(row);
            }
            //insert the table row group to the table
            my_table_record.RowGroups.Add(teg);  // after inserting all the rows
        }


        //this function changed the size of the table according to the window
        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.FlowDoc.ColumnWidth = e.NewSize.Width;
        }

        //this function builds the player score from the time
        private void PlayerScore()
        {
            for(int i = 2; i >= 0; i--)
            {
                if (this.PlayerTime[i].Length < 2)
                {
                    this.PlayerTime[i] = "0" + this.PlayerTime[i];
                }
                this.player_score += this.PlayerTime[i];
            }
        }
        
        
        //this function build the time according to the format 'hh:mm:ss'
        private string BuildTime(string score)
        {
            int length = score.Length;
            string time = "";
            for (int i = 5; i >= 0; i--)
            {
                if (i < length)
                {
                    time += score[(length - 1) - i];
                }
                else
                {
                    time += "0";
                }
                if (i == 4 || i == 2)
                {
                    time += ":";
                }
            }
            return time;
        }

        // this function will take records from mysql table, and pass them forward.
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // when the window loaded we want to go to the sql and get the values from there.
            // after that we want to create a table with the values we got from the sql.
            this.PlayerScore(); // build the player score
            try
            {
                int old_rec = this.RecordInterface.CheckIfUserNameAndDiffExist(this.player_username, this.difficulty);
                if (old_rec == -1) // there was no such record for that user in this difficulty
                {
                    //insert the player, score that achived and the difficulty.
                    this.RecordInterface.InsertValue(this.player_username, this.My_Time, this.difficulty);
                } else
                {
                    // check if the player achived a better record, if yes update the table.
                    if (this.My_Time < old_rec)
                    {
                        this.RecordInterface.UpdateOrigin(this.player_username, this.difficulty, this.My_Time);
                    }
                }
                //at the end, get sorted values from the db.
                var result = this.RecordInterface.GetValues(this.difficulty);
                string[] names = result.Item1;
                string[] times = result.Item2;
                create_record_table(names, times);
                score_p.Inlines.Add(this.BuildTime(this.player_score));
            } catch(Exception ex)
            {
                this.NavigationService.Navigate(new ExceptionPage(ex)); // navigate to exception page
            }
        }

        // this function is will activate the button to go back to the home screen.
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            //nevigate to the home window
            this.NavigationService.Navigate(GameMenu.Instance());
        }
    }
}
