using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalDbProject
{
    class UserRecordsController
    {
        private DbConnection myconn;
        public UserRecordsController()
        {
            this.myconn = DbConnection.Instance(); //DbConnection Instance
        }

        //this function checks if a username and a difficulty is already exist in the db.
        //if it exist so return the score, else return a flag.
        public int CheckIfUserNameAndDiffExist(string username, char diff)
        {
            string[] result;
            try
            {
                this.myconn.SendQuery("SELECT score FROM users_records WHERE difficulty = "
                    + diff + " and username = '" + username + "'");
                result = this.myconn.ReadOneLine();
            }
            catch(Exception e)
            {
                throw e;
            }
            if (result == null)
            {
                return -1;
            }
            else
            {
                return int.Parse(result[0]); //convert the string to double
            }
        }

        // this function update a result of username in a certain difficulty.
        public void UpdateOrigin(string username, char diff, int score)
        {
            try
            {
                this.myconn.SendQuery("Update users_records set score = " + score.ToString() + " where username = '"
                    + username + "' and difficulty = '" + diff + "'", 1);
            }catch(Exception e)
            {
                throw e;
            }
        }


        //this function insert a new value to the users_records table
        public void InsertValue(string username, int record, char diff)
        {
            try
            {
                this.myconn.SendQuery("Insert Into users_records (username,difficulty,score) values " +
                    "('" + username + "','" + diff + "'," + record.ToString() + ")", 1);
            }catch(Exception e)
            {
                throw e;
            }
        }

        //this function helps to build the time to be like 'hh:mm:ss'
        private string CheckEnoughCharsInTime(int time)
        {
            if(time < 10)
            {
                return "0" + time;
            }
            return time.ToString();
        }
        //this function builds the time to be like 'hh:mm:ss'
        private string BuildTime(string score)
        {
            int parseScore = int.Parse(score);
            int h_counter = 0,m_counter = 0;
            //count how many hours in the score
            while(parseScore >= 3600)
            {
                parseScore -= 3600;
                h_counter++;
            }

            //count how many minutes in the score
            while(parseScore >= 60)
            {
                parseScore -= 60;
                m_counter++;
            }

            //form it to time presentaion
            return this.CheckEnoughCharsInTime(h_counter) + ":" + this.CheckEnoughCharsInTime(m_counter) + ":" + this.CheckEnoughCharsInTime(parseScore);
        }
       

        //this function return all the scores and usernames of players who played on the given difficulty in the past
        public Tuple<string[], string[]> GetValues(char diff)
        {
            List<string> names = new List<string>();
            List<string> scores = new List<string>();
            try
            {
                this.myconn.SendQuery("Select username,score From users_records where difficulty = " + diff + " order by score");
                string[] line = this.myconn.ReadOneLine();
                while (line != null)
                {
                    //insert the username of the player
                    names.Add(line[0]);
                    //insert the time in format 'hh:mm:ss'
                    scores.Add(BuildTime(line[1]));
                    line = this.myconn.ReadOneLine();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return Tuple.Create(names.ToArray(), scores.ToArray());
        }
    }
}
