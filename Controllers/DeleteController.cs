using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalDbProject.Controllers
{
    class DeleteController
    {
        private DbConnection dbc; //the model
        public DeleteController()
        {
            this.dbc = DbConnection.Instance();  //get an instance of the model
        }


        //this function gets a username and delete the player with that username from the users table and the record table
        public void DeletePlayer(string username)
        {
            string delteFromUsersquery = "DELETE from users where username = '" + username + "';"; //query to delete from the users table
            string delteFromRecordsquery = "DELETE from users_records where username = '" + username + "';"; //query to delete from record table
            try
            {
                this.dbc.SendQuery(delteFromUsersquery, 1);  // first delete from the users table, and close the connection
                this.dbc.SendQuery(delteFromRecordsquery, 1);  //second delete from users record, and close the connection
            }catch(Exception ex)
            {
                throw ex;
            }
        }


        //this function returns to us all the information about all the players in our data-base
        public string[][] GetAllPlayers()
        {
            // take only the players that are not managers
            string query = "select username,password,name from users where not EXISTS(select username from admins where admins.username = users.username)";
            try
            {
                this.dbc.SendQuery(query);
                return this.dbc.ReadAllLines();
            }catch(Exception e)
            {
                throw e;
            }
        }
    }
}
