using System;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace _4._AddMinion_dd
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] minionInput = Console.ReadLine().Split(" ").Skip(1).ToArray();

            string minionName = minionInput[0];
            int minionAge = int.Parse(minionInput[1]);
            string minionTown = minionInput[2];

            string[] villainInput = Console.ReadLine().Split(" ").Skip(1).ToArray();

            string villainName = villainInput[0];

            const string connectionString = @"Server=.\SQLEXPRESS;Database=MinionsDB;Trusted_Connection=True";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand($"SELECT Name FROM Towns WHERE Name = '{minionTown}'", connection))
                {

                    if (command.ExecuteScalar() is null)
                    {
                        string insertTownQuery = $@"INSERT INTO Towns(Name) VALUES('{minionTown}')";

                        using(var insertTown= new SqlCommand(insertTownQuery, connection))
                        {
                            insertTown.ExecuteNonQuery();
                        }

                        Console.WriteLine($"Town {minionTown} was added to the database.");
                    }
                }

                using (var command = new SqlCommand($"SELECT Name FROM Villains WHERE Name = '{villainName}'", connection))
                {
                    if (command.ExecuteScalar() is null)
                    {
                        string insertVillain = $@"INSERT INTO Villains VALUES ('{villainName}', 4)";

                        using(var insertVillainCommand = new SqlCommand(insertVillain, connection))
                        {
                            insertVillainCommand.ExecuteNonQuery();
                        }

                        Console.WriteLine($"Villain {villainName} was added to the database.");
                    }
                }

                int townId;

                using (var getTownId = new SqlCommand($"SELECT Id FROM Towns WHERE Name = '{minionTown}'", connection))
                {
                    townId =(int)getTownId.ExecuteScalar();
                }

                using (var addMinionComand = new SqlCommand(@$"INSERT INTO Minions VALUES ('{minionName}', '{minionAge}', {townId})", connection))
                {
                    addMinionComand.ExecuteNonQuery();
                }


                int villianId;
                int minionId;

                using (var getVillianId = new SqlCommand($"SELECT Id FROM Villains WHERE Name = '{villainName}'", connection))
                {
                    villianId = (int)getVillianId.ExecuteScalar();
                }

                using (var getMinionId = new SqlCommand($"SELECT Id FROM Minions WHERE Name = '{minionName}'", connection))
                {
                    minionId = (int)getMinionId.ExecuteScalar();
                }

                using (var addMinionServant =  new SqlCommand($"INSERT INTO MinionsVillains VALUES ({minionId}, {villianId})", connection))
                {
                    addMinionServant.ExecuteNonQuery();

                    Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
                }
            }
        }
    }
}
