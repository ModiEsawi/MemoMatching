using FinalDbProject.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
    /// Interaction logic for DeletePage.xaml
    /// </summary>
    public partial class DeletePage : Page
    {
        private DeleteController deleteController; // the controller that connect us to the model

        public DeletePage()
        {
            InitializeComponent();
            this.deleteController = new DeleteController();
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


        //this function delete the specific username from the db
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Hyperlink ClickedRow = (Hyperlink)sender; //convert the sender to be hyper link
                foreach(TableRow row in this.my_delete_table.RowGroups[1].Rows)
                {
                    if (row.Cells[0].Blocks.FirstBlock == ClickedRow.Parent) // check if the paragpraph of the cell is the same as the parent of the hyperlink
                    {
                        // take the username from the second column of the table.
                        this.deleteController.DeletePlayer(new TextRange(row.Cells[1].Blocks.FirstBlock.ContentStart, row.Cells[1].Blocks.FirstBlock.ContentEnd).Text);
                        this.my_delete_table.RowGroups[1].Rows.Remove(row); //remove the current row from the rowgroup of the delete table
                        break;
                    }
                }
            }catch(Exception exception)
            {
                this.MoveToException(exception);
            }
        }

        //this function build one row from the table
        private TableRow AddLineToTable(Hyperlink deleteB, string[] str_line)
        {
            Paragraph deleteB_P = new Paragraph();
            Paragraph Username_P = new Paragraph();
            Paragraph Password_P = new Paragraph();
            Paragraph Name_P = new Paragraph();


            TableRow row = new TableRow();

            //insert to the paragraphs
            deleteB_P.Inlines.Add(deleteB);

            Username_P.Inlines.Add(str_line[0]);
            Password_P.Inlines.Add(str_line[1]);
            Name_P.Inlines.Add(str_line[2]);

            //insert to cells
            TableCell deleteB_C = new TableCell(deleteB_P);
            TableCell Username_C = new TableCell(Username_P);
            Username_C.FlowDirection = FlowDirection.RightToLeft;
            TableCell Password_C = new TableCell(Password_P);
            Password_C.FlowDirection = FlowDirection.RightToLeft;
            TableCell Name_C = new TableCell(Name_P);
            Name_C.FlowDirection = FlowDirection.RightToLeft;

            //insert cells to row
            row.Cells.Add(deleteB_C);
            row.Cells.Add(Username_C);
            row.Cells.Add(Password_C);
            row.Cells.Add(Name_C);

            return row;
        }


        //this function builds the players table
        private void BuildTable(string [][] players)
        {
            TableRowGroup teg = new TableRowGroup();  // creating a new tablerowgroup object that will save all the new rows
            foreach (string[] PlayerInfo in players)
            {
                Hyperlink DeleteLink = new Hyperlink();  // create a hyper link
                DeleteLink.Click += DeleteClick; //add click event handler to the hyper link
                DeleteLink.Inlines.Add("Delete");
                DeleteLink.TextDecorations = null;
                teg.Rows.Add(this.AddLineToTable(DeleteLink, PlayerInfo)); //add the row to the table row group
            }

            this.my_delete_table.RowGroups.Add(teg); // add the table tow group obj to the table
        }

        //this function will be called when the page is loaded
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.BuildTable(this.deleteController.GetAllPlayers()); // get all the players that exist in our database
            }catch(Exception exception)
            {
                this.MoveToException(exception);
            }
        }
    }
}
