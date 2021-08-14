using Org.BouncyCastle.Math.EC.Endo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalDbProject.Controllers
{
    class StatisticController
    {
        private DbConnection dbc;
        public StatisticController()
        {
            this.dbc = DbConnection.Instance();
        }


        //this function return the number of players that play on the given difficulty
        public int GetNumberOfPlayers(char diff)
        {
            string query = "SELECT COUNT(*) FROM users_records WHERE difficulty = '" + diff + "';";  // get the score in asc way
            try
            {
                this.dbc.SendQuery(query);  // send the query
                return int.Parse(this.dbc.ReadOneLine()[0]); // return the number of players who play in that difficulty
            }catch(Exception e)
            {
                throw e;
            }
        }

        //this function helps to builds the time to be like 'hh:mm:ss'
        private string CheckEnoughCharsInTime(int time)
        {
            if (time < 10)
            {
                return "0" + time;
            }
            return time.ToString();
        }
        //this function builds the time to be like 'hh:mm:ss'
        private string BuildTime(string score)
        {
            int parseScore = int.Parse(score);
            int h_counter = 0, m_counter = 0;
            //count how many hours in the score
            while (parseScore >= 3600)
            {
                parseScore -= 3600;
                h_counter++;
            }

            //count how many minutes in the score
            while (parseScore >= 60)
            {
                parseScore -= 60;
                m_counter++;
            }

            //form it to time presentaion
            return this.CheckEnoughCharsInTime(h_counter) + ":" + this.CheckEnoughCharsInTime(m_counter) + ":" + this.CheckEnoughCharsInTime(parseScore);
        }

        //this function return the avg time of a given difficulty
        private double GetAvg(char diff)
        {
            //build the query
            string query = "SELECT AVG(score) from users_records where difficulty = '" + diff + "';";
            try
            {
                this.dbc.SendQuery(query);
                string[] avg_result = this.dbc.ReadOneLine();
                //check if the return data is not null and contains a number
                if (avg_result != null)
                {
                    if (avg_result[0] != "")
                    {
                        return double.Parse(avg_result[0]);
                    }
                }
                return 0;
            }catch(Exception exception)
            {
                throw exception;
            }
        }


        //this function calculate the avg of all the difficulties
        public double[] Avg()
        {
            double[] avg = new double[3];
            char diff = '4';
            try
            {
                //go in a loop for every difficulty and calculate the avg of that difficulty
                for (int i = 0; i < 3; i++)
                {
                    avg[i] = GetAvg(diff);
                    //increment diff twice to get to the next difficulty
                    diff++;
                    diff++;
                }
            }
            catch(Exception exception)
            {
                throw exception;
            }
            return avg; // return the avg
        }


        //this function returns the best score from the given diff
        public string GetBestScore(char diff)
        {
            //build the query
            string query = "SELECT score from users_records where difficulty = '"+ diff +"' order by score;";
            try
            {
                this.dbc.SendQuery(query);
                string[] result_score = this.dbc.ReadOneLine();
                if (result_score == null)
                {
                    return "Doesn't Exist";
                }else
                {
                    return BuildTime(result_score[0]);
                }
            }catch(Exception exception)
            {
                throw exception;
            }
        }
    }
}
