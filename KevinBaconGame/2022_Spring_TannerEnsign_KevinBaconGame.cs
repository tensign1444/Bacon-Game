using GraphProject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace KevinBaconGame
{
    class Program
    {

        private static string[] FileNames; //Holds the names in the file along with the movies
        private static MathGraph<string> Actors; //Graph with all the actors.
        private static int SeperationDegree; //Seperation Degree count
        private static string funFactMovie; //holder for a movie name


        static void Main(string[] args)
        {
            try
            {
                Driver($"DataBase.txt");
            }
            catch (IndexOutOfRangeException ee)
            {
                Console.WriteLine($"ERROR! {ee.Message}");
                Environment.Exit(0);
            }



        }



        /// <summary>
        /// This is the driver method which drives the whole program, writes to the console, calls the other methods and uses the if statements along with timing
        /// </summary>
        /// <param name="fileText"></param>
        static void Driver(string fileText)
        {
            FileInfo file = new FileInfo(fileText);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Actors = new MathGraph<string>("Actors");
            funFactMovie = "";

            ReadFile(fileText);
            AddActorsToGraph();
            timer.Stop();


            Console.WriteLine($"Loaded {fileText} ({file.Length} bytes) in {timer.ElapsedMilliseconds} ms");



            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Please choose two actors > ");
                SeperationDegree = 0;

                string read = Console.ReadLine();

                if (read.ToUpper().Equals("EXIT") || read.ToUpper().Equals("QUIT") || read.ToUpper().Equals("Q") || read.ToUpper().Equals("E"))
                    Environment.Exit(0);

                if (read.ToUpper().Contains(" AND "))
                {
                    string[] str = read.Split(" and ");
                    timer.Reset();
                    timer.Start();

                    if (!Actors.ContainsVertex(str[0]))
                    {
                        timer.Stop();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"The actor {str[0]} is not in the graph.");

                    }
                    if (!Actors.ContainsVertex(str[1]))
                    {
                        timer.Stop();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"The actor {str[1]} is not in the graph.");

                    }
                    else
                    {
                        if (!CheckIfConnected(str[0], str[1]))
                        {
                            timer.Stop();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("The two actors do not have a connection.");

                        }
                        else
                        {
                            Console.WriteLine($"This database has {str[0]} in {Actors.CountAdjacent(str[0])} movie(s).");
                            Console.WriteLine($"This database has {str[1]} in {Actors.CountAdjacent(str[1])} movie(s).");
                            Console.WriteLine($"There are {Actors.CountComponents()} different sets of actors in this database.");
                            Console.WriteLine($"They are in a set with {Actors.CountConnectedTo(str[0])} other movies and actors.");
                            Console.WriteLine("_________CONNECTION BELOW____________");
                            GetShortestPathToActor(str[0], str[1]);
                            timer.Stop();
                            Console.Write($"There are {SeperationDegree} degrees of separation between '");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write($"{str[0]}");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write($"' and '");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write($"{str[1]}");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write($"'.\n");
                            FunFact(str[0]);
                        }

                    }




                }
                else
                {
                    timer.Stop();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Input must be in format 'ACTOR and ACTOR'.");
                }

                Console.WriteLine($"Time Elapsed To Search over {Actors.CountVertices()} vertices's and {Actors.CountEdges()} edges : {timer.ElapsedMilliseconds} ms");
            }

        }

        static void FunFact(string actor)
        {          
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"'{actor}'");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" has been in {Actors.CountAdjacent(actor)} movies in this data base and there were {Actors.CountAdjacent(funFactMovie)} different actors in '");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"'{funFactMovie}'");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" in this data base.\n");
        }

        /// <summary>
        /// Reads the file then adds the names and strings from the file to a string array
        /// </summary>
        /// <param name="fileText"></param>
        static void ReadFile(string fileText)
        {
            FileNames = File.ReadAllText(fileText).Split('\n');
        }

        /// <summary>
        /// Checks if the two actors do have a connection
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns>true or false</returns>
        static bool CheckIfConnected(string str1, string str2)
        {
            return Actors.TestConnectedTo(str1, str2);
        }

        /// <summary>
        /// This method adds everything from the text file to the graph
        /// </summary>
        static void AddActorsToGraph()
        {
            string[] temp = default;
            foreach (string str in FileNames)
            {
                temp = str.Split('|');

                if (str.Equals("") || str.Equals(null))
                    break;
                if (Actors.ContainsVertex(temp[0]) && Actors.ContainsVertex(temp[1]))
                    Actors.AddEdge(temp[0], temp[1]);
                else if (Actors.ContainsVertex(temp[0]) && !Actors.ContainsVertex(temp[1]))
                {
                    Actors.AddVertex(temp[1]);
                    Actors.AddEdge(temp[0], temp[1]);
                }
                else if (!Actors.ContainsVertex(temp[0]) && Actors.ContainsVertex(temp[1]))
                {
                    Actors.AddVertex(temp[0]);
                    Actors.AddEdge(temp[0], temp[1]);
                }
                else
                {
                    Actors.AddVertex(temp[0]);
                    Actors.AddVertex(temp[1]);
                    Actors.AddEdge(temp[0], temp[1]);
                }

            }
        }


        /// <summary>
        /// Returns the shortest path from actor to actor in a string form.
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns>the string of the shortest path</returns>
        static void GetShortestPathToActor(string str1, string str2)
        {
            try
            {
                List<string> temp = Actors.FindShortestPath(str1, str2);

                for (int i = 0; i < temp.Count - 2; i += 2)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write($"{temp[i]}");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" is in ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{temp[i + 1]}");
                    if(i == 0)
                        funFactMovie = temp[i + 1];
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" as is ");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write($"{temp[i + 2]}\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    SeperationDegree++;
                }

            }
            catch (ArgumentException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("There is no path to this item or the actor does not exist.");
            }
        }


    }
}
