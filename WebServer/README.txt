Author:     Jack Machara
Partner:    None
Date:       4/18/20
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  jmachara
Repo:https://github.com/uofu-cs3500-spring20/assignment-9-web-server-and-sql-imjackandillbeyourserver
Commit #:   6034148805ed4346fbfcdc5654eeb594d06fc850
Assignment: Assignment #9 SQL and a Web Server              
Copyright:  CS 3500 and Jack Machara - This work may not be copied for use in Academic Coursework.

1. Comments to Evaluators:
    Database Table Summary:
        I decided to have 5 different tables, One holding the statistics that every game will provide,
            one with the number of players eaten by GameID, One for the time spent in first place by GameID,
            a table for how long it took a player to reach first by GameID, and the total amount of fodd eaten
            by player name. In the Game statistics I decided to store the total game time instead of the start
            and end game times seperately to make it easier to extract that information. 
    Extent of work:
        My webpage is able to do everything that professor De St. Germain asked for, but I didn't get to implementing
            the client to log information to the database. My home webpage has a link to the highscores page and a refresh 
            link. 
        On the highscores page it has a table with the names of all the different players, their highest mass, their 
            time to get to first, their gametime and their highest rank.
        If you click the link on the players name on the table it takes you to that players scores which shows their name and 
            how much food that player has eaten overall. Under that it has a table with their scores, their gametime, their highest 
            ranks and the time it took them to get to first if they did.
        If you type in the url with all the information for a highscore as described in the assignment document it will take it, make sure 
            the data is valid and then put it into the database if it is. 
    PartnerShip: N/A
    Branching: N/A
    Testing:
        Its hard to test these kinds of applications, so I just would run the server and make sure it was displaying the correct data, all 
            the links worked properly and that you could insert data through the browser. 
    Versioning:
        I had 2 versions to my program. The first one was when i got it mostly working and the second version is the webserver that has colors,
            borders to tables and extra information such as the total amount of food eaten by player name on the player scores page. 
            
    

2. Assignment Specific Topics
                Hours Estimated/Worked         Assignment                                   Note
                             15/15           Assignment #9 SQL and a Web Server              
                     
              
3. Consulted Peers:

N/A

4. References:
1. SQL Selecting multiple columns based on max value in one column - https://stackoverflow.com/questions/6860746/sql-selecting-multiple-columns-based-on-max-value-in-one-column