using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;

namespace DataBaseReader
{
    class TableBodyMaker
    {
        public readonly string connectionString;
        public TableBodyMaker()
        {
            var builder = new ConfigurationBuilder();

            builder.AddUserSecrets<TableBodyMaker>();
            IConfigurationRoot Configuration = builder.Build();
            var SelectedSecrets = Configuration.GetSection("DataBaseReaderSecrets");

            connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = SelectedSecrets["ServerName"],
                InitialCatalog = SelectedSecrets["InitialCatalog"],
                UserID = SelectedSecrets["UserID"],
                Password = SelectedSecrets["DBPassword"]
            }.ConnectionString;
        }

        public string HighScoresHttpStringBuilder()
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
SELECT t.PlayerName, t.GameTime, t.MaxMass
FROM GameStatisticsTable t
Right Join(Select PlayerName, Max(MaxMass) maxMass from GameStatisticsTable group by PlayerName) q
on q.maxMass = t.MaxMass 
and q.PlayerName = t.PlayerName
order by q.MaxMass desc; ";

                    using (SqlCommand command = new SqlCommand(SQLCommandString, connection))
                    {
                        SqlDataReader commandResults = command.ExecuteReader();
                        foreach(dynamic result in commandResults)
                        {
                            outputString.Append("<tr><td><a href='http://localhost:11000/");
                            outputString.Append(result["PlayerName"]);
                            outputString.AppendLine($"' {result["PlayerName"]} </a></td><td>{result["MaxMass"]}</td><td>{result["GameTime"]}</td></tr>");
                            
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
        
    }
}
