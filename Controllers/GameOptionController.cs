using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FinalDbProject
{
    /// <summary>
    /// The controller for the game options.
    /// </summary>
    class GameOptionController
    {
        private DbConnection dbc; // Instance of databaseConnection
        private static GameOptionController instance = null; // The self object


        /// <summary>
        /// The constructor.
        /// </summary>
        private GameOptionController()
        {
            //the constructor is private because we want this class to be singleton.
        }


        /// <summary>
        /// This function checks if the given username is a manager.
        /// </summary>
        /// <param name="username"> The username </param>
        /// <returns></returns>
        public bool IsManager(string username)
        {
            string query = "SELECT username FROM admins WHERE username = \'" +
               username + "\';"; // The query
            try
            {
                this.dbc.SendQuery(query);
                // check if the username is in the admin table
                if (this.dbc.ReadOneLine() != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// Return the self object.
        /// </summary>
        /// <returns></returns>
        public static GameOptionController Instance()
        {
            if (instance == null)
            {
                instance = new GameOptionController();
            }
            return instance;
        }


        /// <summary>
        /// Set the info at the model.
        /// </summary>
        /// <param name="data"></param>
        public void SetInfo(string[] data)
        {
            this.dbc = DbConnection.Instance(data);
        }


        /// <summary>
        /// Return the rest of the queries that we have grilled.
        /// </summary>
        /// <param name="randomList"> The list of the random rows numbers  </param>
        /// <returns></returns>
        private string getTheRestOfTheGrillQuery(List<int> randomList)
        {
            string answer = "";
            int randomListLength = randomList.Count();

            for (int i = 0; i < randomListLength - 1; i++)
            {
                answer = answer + "filteredTable.RowNum = " + randomList[i].ToString() + " or ";
            }

            answer = answer + "filteredTable.RowNum = " + randomList[randomListLength - 1].ToString();

            return answer;
        }


        /// <summary>
        /// The method returns an array of images of flags.
        /// </summary>
        /// <param name="flagsNames"> a string array with all the name of the flags </param>
        /// <returns> An array of Image object of flags. </returns>
        public Image[] GetFlagsImages(string[] flagsNames)
        {
            int length = flagsNames.Length;
            Image[] flagsArry = new Image[length];

            for (int i = 0; i < length; i++)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                var path = Directory.GetCurrentDirectory();  // get the working directory relative path
                bi.UriSource = new Uri(path + "/Resources/FlagsPics/" + flagsNames[i] + ".png");  // take the correct pic
                bi.EndInit();
                Image img = new Image();
                img.Source = bi;
                // set the width and height of the picture.
                img.Width = 25;
                img.Height = 25;
                flagsArry[i] = img;
            }
            return flagsArry;
        }


        /// <summary>
        /// The method returns the relevant pairs for this game option.
        /// </summary>
        /// <param name="firstTableName"> The name of the table that we grill its rows. </param>
        /// <param name="takeRowQuery"> The query that grills elements from the firatTableName. </param>
        /// <param name="boardSize"> (boardOrder^2) / 2 </param>
        /// <param name="flag"> True - If this is languages query (and False - If not). </param>
        /// <returns> An array of the pairs that we need for the game cards. </returns>
        private (string, string)[] GetRelevantData(string firstTableName, string takeRowQuery, int boardSize, bool flag = false)
        {
            try
            {
                List<int> randomList = this.GetRandomList(firstTableName, boardSize); // List of the rows that we will take

                (string, string)[] answer = new (string, string)[boardSize]; // The answer array
                if (!flag)
                {
                    this.dbc.SendQuery(takeRowQuery + this.getTheRestOfTheGrillQuery(randomList) + ");"); // query + rowNumber
                    string[][] entries = this.dbc.ReadAllLines(); // The given entries: [[e1, e2], [e_1, e_2]]

                    for (int i = 0; i < entries.GetLength(0); i++)
                    {
                        string first = entries[i][0]; // The first data
                        string secondReachElement = entries[i][1]; // The second element (Sometimes it will be the id of this element)


                        answer[i] = (first, secondReachElement);
                    }
                }
                else
                {
                    string[] split_query = takeRowQuery.Split('@');
                    takeRowQuery = split_query[0] + this.getTheRestOfTheGrillQuery(randomList) + split_query[1];
                    this.dbc.SendQuery(takeRowQuery);
                    string[][] entries1 = this.dbc.ReadAllLines(); // The given entries: [[e1, e2], [e_1, e_2]]
                    Dictionary<string, string> mappingLanguages = new Dictionary<string, string>();
                    for (int i = 0; i < entries1.GetLength(0); i++)
                    {
                        if (!mappingLanguages.ContainsKey(entries1[i][0]))
                        {
                            mappingLanguages.Add(entries1[i][0], "");
                        }
                        mappingLanguages[entries1[i][0]] += entries1[i][1] + ", ";
                    }
                    int index = 0;

                    foreach(string key in mappingLanguages.Keys)
                    {
                        answer[index] = (key, mappingLanguages[key].Substring(0, mappingLanguages[key].Length - 2));
                        index++;
                    }
                }
                return answer;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        ///  The method returns the relevant pairs for this game option.
        /// </summary>
        /// <param name="boardSize"> (boardOrder^2) / 2 </param>
        /// <param name="gameOption"> The option code of the game (i.e. 2: Country_City). </param>
        /// <returns> An array of the pairs that we need for the game cards. </returns>
        public (string, string)[] GetData(char gameOption, int boardSize)
        {
            string grillQuery = ""; // Query that gives entry in the given row.

            bool flag = false;
           
            string firstTableName = ""; // The name of the table that we take its 'random' rows

            switch (gameOption)
            {
                case '0': // Country_FlagPicture
                    firstTableName = "countries";
                    grillQuery = "SELECT filteredTable.country_name, filteredTable.country_code FROM(SELECT *, ROW_NUMBER() " +
                                 "OVER(ORDER BY (SELECT NULL)) AS RowNum FROM " + firstTableName + ") " +
                                 "AS filteredTable WHERE (";
                    break;
                case '1': // Country_City
                    firstTableName = "world_cities";
                    grillQuery = "SELECT countries.country_name, filteredTable.city FROM(SELECT *, ROW_NUMBER() " +
                                 "OVER(ORDER BY (SELECT NULL)) AS RowNum FROM " + firstTableName + ") " +
                                 "AS filteredTable, countries WHERE countries.country_code = filteredTable.country_code " +
                                 "and (";
                    break;
                case '2': // Countery_Continent
                    firstTableName = "countries";
                    grillQuery = "SELECT filteredTable.country_name, continents.continentname FROM(SELECT *, ROW_NUMBER() " +
                                 "OVER(ORDER BY (SELECT NULL)) AS RowNum FROM " + firstTableName + ") " +
                                 "AS filteredTable, continents WHERE continents.continentcode = filteredTable.continent_code " +
                                 "and (";
                    break;
                case '3': // Country_Langauge
                    firstTableName = "(SELECT DISTINCT countryCode FROM countries_and_languages) as ConnectionTbl";
                    flag = true;
                    grillQuery = "SELECT countries.country_name, languages.name FROM " +
                        "countries, languages, countries_and_languages as ConnectionTable, " +
                        "(SELECT filteredTable.countryCode FROM(SELECT*, ROW_NUMBER() " +
                        "OVER(ORDER BY (SELECT NULL)) AS RowNum FROM(SELECT DISTINCT countryCode FROM " +
                        "countries_and_languages) AS connectionTBL) AS filteredTable WHERE @) AS " +
                        "codeCountry WHERE countries.country_code = codeCountry.countryCode and " +
                        "languages.code = ConnectionTable.languages and " +
                        "countries.country_code = ConnectionTable.countryCode;";
                    break;
                case '4': // Country_Athlete
                    firstTableName = "athletes";
                    grillQuery = "SELECT countries.country_name, filteredTable.athleteName FROM(SELECT *, ROW_NUMBER() " +
                                 "OVER(ORDER BY (SELECT NULL)) AS RowNum FROM " + firstTableName + ") " +
                                 "AS filteredTable, countries WHERE countries.country_code = filteredTable.countryCode " +
                                 "and (";
                    break;
                case '5': // City_Population
                    firstTableName = "world_cities";
                    grillQuery = "SELECT filteredTable.city, filteredTable.population FROM(SELECT *, ROW_NUMBER() " +
                                 "OVER(ORDER BY (SELECT NULL)) AS RowNum FROM " + firstTableName + ") " +
                                 "AS filteredTable WHERE (";
                    break;
                case '6': // Language_Native
                    firstTableName = "languages";
                    grillQuery = "SELECT filteredTable.name, filteredTable.native FROM(SELECT *, ROW_NUMBER() " +
                                 "OVER(ORDER BY (SELECT NULL)) AS RowNum FROM " + firstTableName + ") " +
                                 "AS filteredTable WHERE (";
                    break;
                case '7': // Country_CoronaCases
                    firstTableName = "(SELECT countryCode FROM covid19 GROUP BY(countryCode)) AS coronaCases";
                    grillQuery = "SELECT countries.country_name, filteredTable.SumCases " +
                        "FROM(SELECT Sum(cases) AS SumCases,countryCode, ROW_NUMBER() OVER(ORDER BY (SELECT NULL))" +
                        " AS RowNum FROM covid19 GROUP BY(countryCode)) AS filteredTable, " +
                        "countries WHERE countries.country_code = filteredTable.countryCode and (";
                    break;
                case '8': // Country_CoronaDeaths
                    firstTableName = "(SELECT countryCode FROM covid19 GROUP BY(countryCode)) AS coronaDeaths";
                    grillQuery = "SELECT countries.country_name, filteredTable.Sumdeaths " +
                        "FROM(SELECT SUM(deaths) AS Sumdeaths, countryCode, ROW_NUMBER() " +
                        "OVER(ORDER BY (SELECT NULL))" +
                        " AS RowNum FROM covid19 GROUP BY(countryCode)) AS filteredTable, " +
                        "countries WHERE countries.country_code = filteredTable.countryCode and (";
                    break;
                default: // country and one language
                    firstTableName = "countries_and_languages";
                    grillQuery = "select countries.country_name, languages.name FROM(SELECT *, ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) " +
                        "AS RowNum FROM countries_and_languages) AS filteredTable, countries, languages WHERE " +
                        "countries.country_code = filteredTable.countryCode and languages.code = filteredTable.languages and (";
                    break;
            }
            try
            {
                return this.GetRelevantData(firstTableName, grillQuery, boardSize,flag);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        ///  The method returns the number of entries in the given table.
        /// </summary>
        /// <param name="tableName"> The name of the table. </param>
        /// <returns> The number of entries in the table 'tableName'. </returns>
        private int GetNumberOfElements(string tableName)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM " + tableName + ";"; // The query that counts rows
                this.dbc.SendQuery(query);
                return Int32.Parse(this.dbc.ReadOneLine()[0]);
            } catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        ///  Give the data for the game by the query and boardSize (Go to the database).
        ///  We grill random entries from the database.
        /// </summary>
        /// <param name="selectTable"> The table that we grill its entries. </param>
        /// <param name="boardSize"> The size of the board that we need: (boardOrder^2) / 2. </param>
        /// <returns>
        ///         A list of integers: The number of rows that we need to take.
        /// </returns>
        private List<int> GetRandomList(string selectTable, int boardSize)
        {
            try
            {
                Random rnd = new Random(); // The random object
                List<int> randomList = new List<int>(); // A list of the number we have already grilled
                int randomListSize = 0; // The number of elements in randomList
                int numberOfEntries = this.GetNumberOfElements(selectTable); // The number of entries in the selectedTable
                int entryNumber = rnd.Next(1, numberOfEntries); // The entry number that we grill

                // Grill 'boardSize' different numbers of rows
                while (randomListSize < boardSize)
                {
                    // If the number did not grill before
                    if (!randomList.Contains(entryNumber))
                    {
                        randomList.Add(entryNumber);
                        randomListSize++;
                    }

                    entryNumber = rnd.Next(1, numberOfEntries); // Grill the next number
                }

                return randomList;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}