using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinalDbProject
{
    class RegisterController
    {
        private string username, password, name;
        private DbConnection dbc;
        public RegisterController()
        {
            this.dbc = DbConnection.Instance(); // get an instance of the DbConnection
        }
        //this function set the username of the player.
        public void SetUserName(string username)
        {
            this.username = username;
        }

        //this function set the Password of the player.
        public void SetPassword(string password)
        {
            this.password = password;
        }
        //this function set the name of the player.
        public void SetName(string name)
        {
            this.name = name;
        }
        //this function check if the given username exist
        public bool IsUsernameExist()
        {
            try
            {
                // send the query
                this.dbc.SendQuery("SELECT username FROM users WHERE username = \'" + this.username + "\';");
                return this.dbc.ReadOneLine() != null;  // return the result
            } catch(Exception e)
            {
                throw e;
            }
        }

        //this function insert to the db a new player.
        public void Register()
        {
            string query = "INSERT INTO users (username, name, password) " +
                "VALUES('" + this.username + "','" + this.name + "','" + this.password  + "');"; // The INSERT query (insert new user)
            try
            {
                this.dbc.SendQuery(query, 1); // send the query and close the connection
            }catch(Exception e)
            {
                throw e;
            }
        }
    }
}
