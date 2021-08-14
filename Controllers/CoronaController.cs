using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalDbProject.Controllers
{
    //this class is singleton
    class CoronaController
    {
        private Dictionary<string,string> countriesCode; // save the codes for all the countries
        private DbConnection dbc; //instance of the model
        private string column; //the column we want to take from the covid19 table
        private static CoronaController Instance = null;

        //constructor is private because this class is singleton.
        private CoronaController()
        {
            //initialize
            this.dbc = DbConnection.Instance();
            this.countriesCode = new Dictionary<string, string>();
        }

        //this function return the sum of the values in the given column, between the given dates in the given country
        public string GetSum(string countryName, string firstDate, string secondDate)
        {
            //make the date to be like the format in the sql
            firstDate = firstDate.Replace("/", "-");
            firstDate = this.buildDate(firstDate);
            secondDate = secondDate.Replace("/", "-");
            secondDate = this.buildDate(secondDate);

            //build the query to send to the db
            string query = "select sum(" + this.column + ") from covid19 where countryCode =" +
                " '"+ this.countriesCode[countryName] + "' and date between '" + firstDate + "'" +
                " and '" + secondDate + "' order by date;";
            try
            {
                this.dbc.SendQuery(query);  // send the query
                string[] line = this.dbc.ReadOneLine(); // get the result
                return line[0]; // return the sum
            } catch(Exception e)
            {
                throw e;
            }
        }


        //set the column from which we will take the data
        public void SetColumn(string column)
        {
            this.column = column;
        }


        //get an instance of this class
        public static CoronaController GetInstance()
        {
            if (Instance == null)
            {
                Instance = new CoronaController();
            }
            return Instance;
        }

        //this function bring us all the countries names, and store them with their code in a local dictionary
        public string[] GetAllCountries()
        {
            if (this.countriesCode.Count == 0)  // in order to save time, this class is singleton so we dont need to call the query every time we enter to this class.
            {
                //build the query to send to the db
                string query = "select distinct countries.country_name, countries.country_code" +
                    " from countries,covid19" +
                    " where countries.country_code = covid19.countryCode;";
                try
                {
                    this.dbc.SendQuery(query);
                    string[][] countries_codes = this.dbc.ReadAllLines(); // take all the countries and thier codes.
                    int length = countries_codes.Length;
                    for (int i = 0; i < length; i++)
                    {
                        countriesCode.Add(countries_codes[i][0], countries_codes[i][1]); // insert all the keys and values
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return this.countriesCode.Keys.ToArray();
        }

        //this function build the date according to the format of the date in mysql
        private string buildDate(string date)
        {
            string[] split_date = date.Split('-');
            return split_date[2] + "-" + split_date[1] + "-" + split_date[0];
        }


        /// <summary>
        /// the function gets a country name and dates
        /// </summary>
        /// <param name="countryName"></param>
        /// <param name="firstDate"></param>
        /// <param name="secondDate"></param>
        /// <returns>a string array with all the relevent data of the given input</returns>
        public string[][] GetData(string countryName,string firstDate, string secondDate)
        {
            //replace the '/' char with '-', and change the position of the year and day
            firstDate = firstDate.Replace("/", "-");
            firstDate = this.buildDate(firstDate);
            secondDate = secondDate.Replace("/", "-");
            secondDate = this.buildDate(secondDate);

            //build the query to send to the db
            string query = "select date," + this.column + " " +
                "from covid19 where countryCode = '" + this.countriesCode[countryName] + "'" +
                " and date between '" + firstDate + "' and '" + secondDate + "' order by date";
            try
            {
                this.dbc.SendQuery(query);
                return this.dbc.ReadAllLines();
            }catch(Exception e)
            {
                throw e;
            }
        }

        //this function return all the dates of a certain country
        public string[][] GetDate(string countryName)
        {
            //build the query to send to the db.
            string query = "select date from covid19 where countryCode = '" + this.countriesCode[countryName] + "' order by date";
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
