using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalDbProject
{
    
    class LoginController
    {
        private DbConnection dbc;
        private string username;
        public LoginController()
        {
            this.dbc = DbConnection.Instance(); // get an instance of DbConnection
        }
        public void SetUserName(string username)
        {
            this.username = username;
        }
        //this function try to connect
        public string[] GetUserNameDetails()
        {
            string query = "SELECT username, name, password FROM users WHERE username = \'" +
               this.username + "\';"; // The query
            try
            {
                this.dbc.SendQuery(query); // The query's answer
                return this.dbc.ReadOneLine(); // return the result
            }catch(Exception e)
            {
                throw e;
            }
        }

    }
}
