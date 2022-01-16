using Microsoft.Data.SqlClient;
using System;


namespace _ExercisesADO.NET
{
   public class StartUp
    {
        const string sqlConnectionString = @"Server=.\SQLEXPRESS;Database=MinionsDB;Trusted_Connection=True";
        //const string sqlConnectionString = @"Server=.\SQLEXPRESS;Database=MinionsDB;Trusted_Connection=True";
        public static void Main(string[] args)
        {
            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
            {
                sqlConnection.Open();

                // Console.WriteLine("Connected!!!");

                //// Create DATABASE
                ///

                // string createDatabase = "CREATE DATABASE MinionsDB";

                //using (SqlCommand command = new SqlCommand(createDatabase, sqlConnection))
                //{
                //    command.ExecuteNonQuery();
                //}


                //Create TABLES
                //var createTables = CreateTablesStatements();

                //foreach (var query in createTables)
                //{
                //    ExecuteNoneQery(sqlConnection, query);
                //}


                // InsertInto TABLE
                //var insertStatements = InsertDataStatement();

                //foreach (var query in insertStatements)
                //{
                //    ExecuteNoneQery(sqlConnection, query);
                //}

                // --------2.	Villain Names

                //  GetVillianNames(sqlConnection);

                // --------3.	Minion Names

                //GetMinionsNames(sqlConnection);

                // --------4.	Add Minion

                string[] minionInput = Console.ReadLine().Split(" ");

                string minionName = minionInput[1];
                int age = int.Parse(minionInput[2]);
                string town = minionInput[3];

                string[] villainInfo = Console.ReadLine().Split(" ");

                int? townId = GetTownId(sqlConnection, town);

                if (townId == null)
                {
                    string crateTownQuery = "INSERT INTO Towns (Name) VALUES (@townName)";
                    using var sqlCommandCrateTown = new SqlCommand(crateTownQuery, sqlConnection);
                    sqlCommandCrateTown.Parameters.AddWithValue("@townName", town);
                    sqlCommandCrateTown.ExecuteNonQuery();
                    townId = GetTownId(sqlConnection, town);
                    Console.WriteLine($"Town {town} was added to the database.");
                }

                string villainName = villainInfo[1];
                int? villainId = GetVilliantId(sqlConnection, villainName);

                if (villainId == null)
                {
                    string createVillain = @"INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";
                    using var sqlCommandCrateVillain = new SqlCommand(createVillain, sqlConnection);
                    sqlCommandCrateVillain.Parameters.AddWithValue("@villainName", villainName);
                    sqlCommandCrateVillain.ExecuteNonQuery();
                    villainId = GetVilliantId(sqlConnection, villainName);
                    Console.WriteLine($"Villain {villainName} was added to the database.");
                }

                CreateMinion(sqlConnection, minionName, age, townId);

                var minionId = GetMinionID(sqlConnection, minionName);

                InsertMinionVillian(sqlConnection, villainId, minionId);
                Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");

            }
        }

        private static void InsertMinionVillian(SqlConnection sqlConnection, int? villianId, int? minionId)
        {
            var insertIntoMinViln = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@villainId, @minionId)";
            var sqlCommandMinionVilian = new SqlCommand(insertIntoMinViln, sqlConnection);
            sqlCommandMinionVilian.Parameters.AddWithValue("@minionId", minionId);
            sqlCommandMinionVilian.Parameters.AddWithValue("@villainId", villianId);
            sqlCommandMinionVilian.ExecuteNonQuery();
  
        }

        private static int? GetMinionID(SqlConnection sqlConnection, string minionName)
        {
            var moinionIdQuery = "SELECT Id FROM Minions WHERE Name = @Name";
            var sqlCommandMinionId = new SqlCommand(moinionIdQuery, sqlConnection);
            sqlCommandMinionId.Parameters.AddWithValue("@Name", minionName);
            var minionId = sqlCommandMinionId.ExecuteScalar();
            return (int?) minionId;
        }

        private static void  CreateMinion(SqlConnection sqlConnection, string minionName, int age, int? townId)
        {
            string createMinion = @"INSERT INTO Minions (Name, Age, TownId) VALUES (@nam, @age, @townId)";
            var sqlCommandCrateMinion = new SqlCommand(createMinion, sqlConnection);
            sqlCommandCrateMinion.Parameters.AddWithValue("@nam", minionName);
            sqlCommandCrateMinion.Parameters.AddWithValue("@age", age);
            sqlCommandCrateMinion.Parameters.AddWithValue("@townId", townId);
            sqlCommandCrateMinion.ExecuteNonQuery();
        }

        private static int? GetVilliantId(SqlConnection sqlConnection, string villianName)
        {
            string villianIdQuery = "SELECT Id FROM Villains WHERE Name = @Name";
            using var sqlCommand = new SqlCommand(villianIdQuery, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Name", villianName);
            var villianId = (int?)sqlCommand.ExecuteScalar();
            return villianId;
        }

        private static int? GetTownId(SqlConnection sqlConnection , string town)
        {
            string townIdQuery = "SELECT Id FROM Towns WHERE Name = @townName";
            using var sqlCommand = new SqlCommand(townIdQuery, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@townName", town);
            var townId = (int?)sqlCommand.ExecuteScalar();
            return townId;
        }

        private static void GetMinionsNames(SqlConnection sqlConnection)
        {
            int id = int.Parse(Console.ReadLine());
            string villianNameQuery = @"SELECT Name FROM Villains WHERE Id = @Id";
            using var command = new SqlCommand(villianNameQuery, sqlConnection);
            command.Parameters.AddWithValue("@Id", id);
            var result = command.ExecuteScalar();

            string minionsQuery = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                               m.Name, 
                                               m.Age
                                          FROM MinionsVillains AS mv
                                          JOIN Minions As m ON mv.MinionId = m.Id
                                         WHERE mv.VillainId = @Id
                                      ORDER BY m.Name";

            if (result == null)
            {
                Console.WriteLine($"No villain with ID {id} exists in the database.");
            }
            else
            {
                Console.WriteLine($"Villain: {result}");

                using (var minionCommand = new SqlCommand(minionsQuery, sqlConnection))
                {
                    minionCommand.Parameters.AddWithValue("@Id", id);
                    using (var reader = minionCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader[0]}. {reader[1]} {reader[2]}");
                        }
                    }
                }
            }
        }

        private static object ExecuteScalar(SqlConnection connection, string query)
        {
            using var command = new SqlCommand(query, connection);
            var result = command.ExecuteScalar();
            return result;     
        }

        private static void GetVillianNames(SqlConnection connection)
        {
            string query =   @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                                FROM Villains AS v 
                                JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
                            GROUP BY v.Id, v.Name 
                              HAVING COUNT(mv.VillainId) > 3 
                            ORDER BY COUNT(mv.VillainId)";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("(no minions)");
                    }

                    while (reader.Read())
                    {
                        var name = reader[0];
                        var count = reader[1];

                        Console.WriteLine($"{name} - {count}");
                    }
                }
            }
        }

        private static string[] InsertDataStatement()
        {
            var result = new string[]
            {
               "INSERT INTO Countries ([Name]) VALUES('Bulgaria'),('England'),('Cyprus'),('Germany'),('Norway')",

               "INSERT INTO Towns([Name], CountryCode) VALUES('Plovdiv', 1),('Varna', 1),('Burgas', 1),('Sofia', 1),('London', 2),('Southampton', 2),('Bath', 2),('Liverpool', 2),('Berlin', 3),('Frankfurt', 3),('Oslo', 4)",

               "INSERT INTO Minions(Name, Age, TownId) VALUES('Bob', 42, 3),('Kevin', 1, 1),('Bob ', 32, 6),('Simon', 45, 3),('Cathleen', 11, 2),('Carry ', 50, 10),('Becky', 125, 5),('Mars', 21, 1),('Misho', 5, 10),('Zoe', 125, 5),('Json', 21, 1)",

               "INSERT INTO EvilnessFactors(Name) VALUES('Super good'),('Good'),('Bad'), ('Evil'),('Super evil')",

               "INSERT INTO Villains(Name, EvilnessFactorId) VALUES('Gru', 2),('Victor', 1),('Jilly', 3),('Miro', 4),('Rosen', 5),('Dimityr', 1),('Dobromir', 2)",

               "INSERT INTO MinionsVillains(MinionId, VillainId) VALUES(4, 2),(1, 1),(5, 7),(3, 5),(2, 6),(11, 5),(8, 4),(9, 7),(7, 1),(1, 3),(7, 3),(5, 3),(4, 3),(1, 2),(2, 1),(2, 7)"

            };

            return result;
        }


        private static void ExecuteNoneQery(SqlConnection connection, string query)
        {
            using (var command = new SqlCommand(query, connection) )
            {
                command.ExecuteNonQuery();
            }
        }

        private static string[] CreateTablesStatements()
        {
            string[] result = new string[]
            {
                "CREATE TABLE Countries (Id INT PRIMARY KEY IDENTITY,Name VARCHAR(50))",
                "CREATE TABLE Towns(Id INT PRIMARY KEY IDENTITY,Name VARCHAR(50), CountryCode INT FOREIGN KEY REFERENCES Countries(Id))",
                "CREATE TABLE Minions(Id INT PRIMARY KEY IDENTITY,Name VARCHAR(30), Age INT, TownId INT FOREIGN KEY REFERENCES Towns(Id))",
                "CREATE TABLE EvilnessFactors(Id INT PRIMARY KEY IDENTITY, Name VARCHAR(50))",
                "CREATE TABLE Villains (Id INT PRIMARY KEY IDENTITY, Name VARCHAR(50), EvilnessFactorId INT FOREIGN KEY REFERENCES EvilnessFactors(Id))",
                "CREATE TABLE MinionsVillains (MinionId INT FOREIGN KEY REFERENCES Minions(Id),VillainId INT FOREIGN KEY REFERENCES Villains(Id),CONSTRAINT PK_MinionsVillains PRIMARY KEY (MinionId, VillainId))"
            };
            return result;
        }
    }
}
