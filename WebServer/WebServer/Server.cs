using NetworkingNS;
using System;
using System.Net.Sockets;
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
/// This is the code for the web server that shows the high scores from the database
///    
/// </summary>
namespace WebServer
{
    
    class Server
    {
        //int based on what page request the server recieves
        static private int WebPageInt = 0;
        //Name of the player that the player scores page will display
        static private string PagePlayerName;
        //Used to insert data into the database.
        static private string[] DataBaseInsertionRequest;


        /// <summary>
        /// Start the program and await for connections (e.g., from the browser).
        /// Press "Enter" to end, though in a real web server, you wouldn't end.... ;^)
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Web Server Active. Awaiting Clients! Press Enter to Quit.");
            Networking.Server_Create_Connection_Listener(OnClientConnect);
            Console.ReadLine();
        }

        /// <summary>
        /// Basic connect handler - i.e., a browser has connected!
        /// </summary>
        /// <param name="state"> Networking state object created by the Networking Code. Contains the socket.</param>
        private static void OnClientConnect(Preserved_Socket_State state)
        {
            state.on_data_received_handler = RequestFromBrowserHandler;
            Networking.await_more_data(state);
        }

        /// <summary>
        /// Creates the HTTP response header
        /// </summary>
        /// <param name="message">the message being sent to determine the length for the header</param>
        /// <returns>http header string</returns>
        private static string BuildHTTPResponseHeader(string message)
        {
            // modify this to return am HTTP Response header, don't forget the new line!
            return @$"
HTTP/1.1 200 OK
Date:Sat, 18 Apr 2020 12:28:53 GMT
Server:JackServer
Content-Length:{message.Length}
Content-Type:text/html
Connection:Closed
";
        }

        /// <summary>
        ///   Uses a switch statement to determine which page to build and returns the body of the page
        /// </summary>
        /// <returns> A string the represents a web page.</returns>
        private static string BuiltHTTPBody()
        {
            switch (WebPageInt)
            {
                case 0:
                    return BuildHomePage();
                case 1:
                    return BuildHighScoresPage(TableBodyBuilder.HighScoresHttpStringBuilder());
                case 2:
                    return BuildPlayerScorePage(PagePlayerName,TableBodyBuilder.PlayerScoresHttpStringBuilder(PagePlayerName), TableBodyBuilder.GetFoodEaten(PagePlayerName));
                case 3:
                    return TableBodyBuilder.InsertScoresIntoTable(DataBaseInsertionRequest);

            }
            //invalid request returns to the homepage
            return BuildHomePage();
        }

        /// <summary>
        /// Builds the page header for the webservers pages
        /// </summary>
        /// <returns>html string of the webservers header for every page</returns>
        private static string BuildHTTPPageHeader()
        {
            return @"
<html lang = 'en'>
    <head>
        <title> CS 3500 Agario High Scores</title>
        <meta charset = 'utf-8'>
        <meta name='description' content='High Scores'>
        <meta name='author' content='Jack'>
        <style>
        body {
            background-color: beige;
             }

        h1 {
            color: green;
            margin-left: 50px;
            }
        table{
            border: 1px solid black;
            background-color: gray;
            color: black;
            margin-left: 40px;
            }
        td{
            border: 1px solid black;
          }

        </style>
    </head>";
        }
        /// <summary>
        /// Builds the homepage
        /// </summary>
        /// <returns>homepage string</returns>
        private static string BuildHomePage()
        {
            return @"
    <body>
        <h1>UhOhSpagett.io</h1>
        <ol>
            <li><p><a href='http://localhost:11000/'> Reload Page </p></li>
            <li><p><a href='http://localhost:11000/highscores'> High Scores</p></li>
        </ol>
    </body>
</html>";
        }
        /// <summary>
        /// Builds the high scores webpage
        /// </summary>
        /// <param name="tableBody">body of the highscores table</param>
        /// <returns>string of the webpage</returns>
        private static string BuildHighScoresPage(string tableBody)
        {
            return @$"
    <body>
        <h1>UhOhSpagett.io High Scores</h1>
        <h2> <a href='http://localhost:11000/'> Main Page </a> </h2>
        <table>
            <tbody>
                <tr><th>Name</th><th>MaxMass</th><th>LifeTime</th><th>HighestRank</th><th>Time To Reach First</th></tr>
                {tableBody}
            </tbody>
        </table>
    </body>
</html>";
        }
        /// <summary>
        /// Buildes the player scores page
        /// </summary>
        /// <param name="Name">player name</param>
        /// <param name="tableBody">Body of the table</param>
        /// <returns>String of the webpage</returns>
        private static string BuildPlayerScorePage(string Name, string tableBody, int totalFoodEaten)
        {
            return @$"
    <body>
        <h1>{Name} High Scores</h1>
        <p> <a href='http://localhost:11000/'> Main Page </a> </p>
        <p> <a href='http://localhost:11000/highscores'> High Scores </a> </p>
        <p> Total Food Eaten By {Name}: {totalFoodEaten} <p>
        <h2> highscores </h2>
        <table>
            <tbody>
                <tr><th>MaxMass</th><th>LifeTime</th><th>HighestRank</th><th>Time To Reach First</th></tr>
                {tableBody}
            </tbody>
        </table>
    </body>
</html>";
        }




        /// <summary>
        /// Create a response message string to send back to the connecting
        /// program (i.e., the web browser).  The string is of the form:
        /// 
        ///   HTTP Header
        ///   [new line]
        ///   HTTP Body
        ///   
        ///  The body is an HTML string.
        /// </summary>
        /// <returns></returns>
        private static string BuildHTTPResponse()
        {
            string message = BuiltHTTPBody();
            string pageHeader = BuildHTTPPageHeader();
            string header = BuildHTTPResponseHeader(pageHeader + message);

            return header + Environment.NewLine + pageHeader + message;
        }

        /// <summary>
        ///   When a request comes in (from a browser) this method will
        ///   be called by the Networking code.  When a Get message is recieved
        ///   the code will store what type of request it was as an int for future use.
        ///   When the code recieves the end of the statement it will send the http webpage
        /// </summary>
        /// <param name="network_message_state"> provided by the Networking code, contains socket and message</param>
        private static void RequestFromBrowserHandler(Preserved_Socket_State network_message_state)
        {
            //Console.WriteLine($"{++counter,4}: {network_message_state.Message}");
            try
            {
                string networkMessage = network_message_state.Message;
                //Gets the GET line from the request
                if (networkMessage.Contains("GET"))
                {
                    //Gets the path(s) from the get request
                    string RequestString = networkMessage.Split()[1];
                    if(RequestString == "/")
                    {
                        //returns 0 for homepage
                        WebPageInt = 0;
                    }
                    else
                    {
                        string[] requests = RequestString.Substring(1).Split('/');
                        if (requests[0] == "highscores")
                        {
                            //one for high scores
                            WebPageInt = 1;
                        }
                        else if(requests[0] == "scores")
                        {
                            if (requests.Length == 2)
                            {
                                WebPageInt = 2; //individual player scores
                                PagePlayerName = requests[1];
                                if (PagePlayerName.Contains("%20"))
                                    PagePlayerName = PagePlayerName.Replace("%20", " ");
                            }
                            else if (requests.Length == 6)
                            { 
                                WebPageInt = 3; //sending a score to the database
                                DataBaseInsertionRequest = requests;
                            }
                            else
                                WebPageInt = 0; //homepage
                        }

                    }
                    
                }
                // by definition if there is a new line, then the request is done
                if (networkMessage == "\r")
                {
                    Networking.Send(network_message_state.socket, BuildHTTPResponse());

                    
                    // the message response told the browser to disconnect, but
                    // if they didn't we will do it.
                    if (network_message_state.socket.Connected)
                    {
                        network_message_state.socket.Shutdown(SocketShutdown.Both);
                        network_message_state.socket.Close();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Something went wrong... this is a bad error message. {exception}");
            }
        }
    }
}
