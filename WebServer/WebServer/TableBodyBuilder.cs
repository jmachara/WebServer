using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Text;
/// <summary> 
/// Author:    Jack Machara
/// Partner:   None
/// Date:      04/20/20
/// Course:    CS 3500, University of Utah, School of Computing 
/// Copyright: CS 3500 and Jack Machara - This work may not be copied for use in Academic Coursework. 
/// 
/// I, Jack Machara, certify that I wrote this code from scratch and did not copy it in part or whole from  
/// another source.  All references used in the completion of the assignment are cited in my README file. 
/// 
/// File Contents 
/// This builds the table body for the high scores website from the database
///    
/// </summary>
namespace WebServer
{
    class TableBodyBuilder
    {
        public static readonly string connectionString;
        /// <summary>
        /// Constructor for the TableBodyBuilder
        /// </summary>
        static TableBodyBuilder()
        {
            var builder = new ConfigurationBuilder();

            builder.AddUserSecrets<TableBodyBuilder>();
            IConfigurationRoot Configuration = builder.Build();
            var SelectedSecrets = Configuration.GetSection("ServerSecrets");

            connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = SelectedSecrets["ServerName"],
                InitialCatalog = SelectedSecrets["InitialCatalog"],
                UserID = SelectedSecrets["UserID"],
                Password = SelectedSecrets["DBPassword"]
            }.ConnectionString;
        }
        /// <summary>
        /// Buildes the string for the high scores table by reading data from the database
        /// </summary>
        /// <returns>html string</returns>
        public static string HighScoresHttpStringBuilder()
        {
            StringBuilder outputString = new StringBuilder();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    //
                    // Open the SqlConnection.
                    //
                    connection.Open();
                    string SQLCommandString = @"
SELECT t.PlayerName, t.GameTime, t.MaxMass, t.HighestRank, p.TimeToFirst
FROM GameStatisticsTable t
Full Outer Join TimeToFirstTable p on p.GameID = t.GameID
Right Join(Select PlayerName, Max(MaxMass) maxMass from GameStatisticsTable group by PlayerName) q
on q.maxMass = t.MaxMass 
and q.PlayerName = t.PlayerName
order by q.MaxMass desc;";

                    using (SqlCommand command = new SqlCommand(SQLCommandString, connection))
                    {
                        SqlDataReader commandResults = command.ExecuteReader();
                        foreach (dynamic result in commandResults)
                        {
                            outputString.Append("<tr><td><a href='/scores/");
                            outputString.Append(result["PlayerName"]);
                            outputString.AppendLine($"'>{result["PlayerName"]}</a><td>{result["MaxMass"]}</td><td>{result["GameTime"]}</td><td>{result["HighestRank"]}</td><td>{result["TimeToFirst"]}</td></tr>");

                        }
                    }
                }
                return outputString.ToString();
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"Error in SQL connection: {exception.Message}");
                return "";
            }

        }
        /// <summary>
        /// Returns the amount of food objects the player has consumed from the database
        /// </summary>
        /// <param name="pagePlayerName">name of the player</param>
        /// <returns>the number of food objects eaten by the player</returns>
        internal static int GetFoodEaten(string pagePlayerName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    //
                    // Open the SqlConnection.
                    //
                    connection.Open();
                    string SQLCommandString = @$"
select TotalFoodEaten
from TotalFoodEatenByPlayerNameTable 
where PlayerName = '{pagePlayerName}'";

                    using (SqlCommand command = new SqlCommand(SQLCommandString, connection))
                    {
                        SqlDataReader commandResults = command.ExecuteReader();
                        foreach (dynamic result in commandResults)
                        {
                            return result["TotalFoodEaten"];
                        }
                    }
                }
                return 0;
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"Error in SQL connection: {exception.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Returns the string for the body of the player scores
        /// </summary>
        /// <param name="Name">Name of the player</param>
        /// <returns>http table body string</returns>
        public static string PlayerScoresHttpStringBuilder(string Name)
        {
            StringBuilder outputString = new StringBuilder();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    string SQLCommandString = @$"
Select MaxMass, GameTime, HighestRank, p.TimeToFirst
from GameStatisticsTable t
full outer join TimeToFirstTable p on t.GameID = p.GameID 
where PlayerName = '{Name}'
order by MaxMass desc;";

                    using (SqlCommand command = new SqlCommand(SQLCommandString, connection))
                    {
                        SqlDataReader commandResults = command.ExecuteReader();
                        foreach (dynamic result in commandResults)
                        {
                            outputString.AppendLine($"<td>{result["MaxMass"]}</td><td>{result["GameTime"]}</td><td>{result["HighestRank"]}</td><td>{result["TimeToFirst"]}</td></tr>");

                        }
                    }
                }
                return outputString.ToString();
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"Error in SQL connection: {exception.Message}");
            }
            return "";

        }
        /// <summary>
        /// Inserts data into the database
        /// </summary>
        /// <param name="dataBaseInsertionRequest">String array of the values being inserted</param>
        /// <returns></returns>
        internal static string InsertScoresIntoTable(string[] dataBaseInsertionRequest)
        {
            try
            {
                if (verifyData(dataBaseInsertionRequest))
                {
                    
                int rowsAffected = 0;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    //
                    // Open the SqlConnection.
                    //
                    connection.Open();
                    int Time = (int)(long.Parse(dataBaseInsertionRequest[5]) - long.Parse(dataBaseInsertionRequest[4]));
                    string SQLCommandString = @$"insert into GameStatisticsTable values('{dataBaseInsertionRequest[1]}', {dataBaseInsertionRequest[2]},{dataBaseInsertionRequest[3]},{Time});";
                    using (SqlCommand command = new SqlCommand(SQLCommandString, connection))
                    {
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
                return @$"
<h1>Data Succesfully Inserted, {rowsAffected} row affected</h1> 
    <h2> <a href='http://localhost:11000/'> Main Page </a> </h2>";
                }
                else
                {
                    return "<h1>Data Failed To Be Inserted, Invalid Data</h1>";
                }
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"Error in SQL connection: {exception.Message}");
                return "<h1>Data Failed To Be Inserted</h1>";
            }
        }
        /// <summary>
        /// Verifys that the data is correct
        /// </summary>
        /// <param name="dataBaseInsertionRequest"></param>
        /// <returns></returns>
        private static bool verifyData(string[] dataBaseInsertionRequest)
        {
            Int32.TryParse(dataBaseInsertionRequest[2], out int massData);
            Int32.TryParse(dataBaseInsertionRequest[3], out int highRankData);
            Int32.TryParse(dataBaseInsertionRequest[4], out int startTime);
            Int32.TryParse(dataBaseInsertionRequest[5], out int endTime);
            bool massValid = massData > 0;
            bool validRank = highRankData > 0;
            bool validTime = endTime > startTime;
            return massValid && validRank && validTime;

        }
    }
}
