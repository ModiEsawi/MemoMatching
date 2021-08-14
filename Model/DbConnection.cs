using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using MySql.Data;
using MySql.Data.MySqlClient;
//Install-Package MySql.Data -Version 8.0.22
namespace FinalDbProject
{
    public class DbConnection
    {
        private string Server { get; set; }
        private string DatabaseName { get; set; }
        private string UserName { get; set; }
        private string Password { get; set; }

        private MySqlDataReader reader = null;
        private MySqlConnection Connection = null;
        public object Port { get; private set; }

        private static DbConnection instance = null;


        //constructor gets the setting of the database
        private DbConnection(string[] data)
        {
            this.Server = data[0];
            this.Port = data[1];
            this.DatabaseName = data[2];
            this.UserName = data[3];
            this.Password = data[4];
        }


        //make this class a singleton
        public static DbConnection Instance(string[] data)
        {
            if (instance == null)
            {
                instance = new DbConnection(data);
            }
            return instance;
        }

        //return the instance
        public static DbConnection Instance()
        {
            return instance;
        }

        //this function opens a connection with the database
        public bool OpenConnection()
        {
            if (this.Connection == null)
            {
                if (String.IsNullOrEmpty(DatabaseName))
                    return false;
                try  //try to open a connection
                {
                    string connstring = string.Format("Server={0}; Port={1}; database={2}; UID={3}; password={4}", Server, Port, DatabaseName, UserName, Password);
                    this.Connection = new MySqlConnection(connstring);
                    this.Connection.Open();
                }
                catch (Exception e) // take the exception to the upper level
                {
                    throw e;
                }
            }
            return true;
        }

        //this function close the connection with the database
        public void Close()
        {
            try
            {
                this.Connection.Close();
            } catch (Exception e)
            {
                throw e;
            }
            this.Connection = null;
        }

        //this fucntion sends the query to the database.
        //first open a connection and then send the query.
        //if there is an exception we returned it backward.
        //ShouldClose is optional and indicate if we need to close the connection right away.
        public void SendQuery(string MyQuery, int ShouldClose = -1)
        {
            try
            {
                if (this.Connection != null) // check if the connection is open, so first close it.
                {
                    this.Close();
                }
                this.OpenConnection(); // first open the connection
                var cmd = new MySqlCommand(MyQuery, this.Connection);
                this.reader = cmd.ExecuteReader();
                if (ShouldClose != -1)
                {
                    this.Close();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //this function will read one line each time from the data we received from the db.
        public string[] ReadOneLine()
        {
            if (reader.Read())
            {
                int length = reader.FieldCount;
                string[] line = new string[length];
                for (int i = 0; i < length; i++)
                {
                    line[i] = reader[i].ToString();
                }
                return line;
            }
            else
            {
                try
                {
                    this.Close(); // end of data, close the connection.
                } catch (Exception e)
                {
                    throw e;
                }
                return null;
            }
        }


        //this function returns an array of lines with all the data
        public string[][] ReadAllLines()
        {
            List<string[]> lines = new List<string[]>();
            try
            {
                string[] line = this.ReadOneLine(); // read one line
                while (line != null) // read untill we dont have lines any more
                {
                    lines.Add(line);
                    line = this.ReadOneLine(); // read one line
                }
            }catch(Exception e)
            {
                throw e;
            }
            return lines.ToArray();
        }
    }
}
