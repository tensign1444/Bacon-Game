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
        private static MathGraph<string> Actors;
        private static SortedDictionary<string, List<string>> ActorsAndMovies;
        private static int SeperationDegree;
        

        static void Main(string[] args)
        {
            Console.WriteLine("Please choose two actors.");
            
            Driver($"DataBase.txt");
            
            
        }



        /// <summary>
        /// This is the driver method which drives the whole program
        /// </summary>
        /// <param name="fileText"></param>
        static void Driver(string fileText)
        {
            
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Actors = new MathGraph<string>("Actors");

            ReadFile(fileText);
            AddActorsToGraph();
            timer.Stop();

            Console.Write($"Time Elapsed To Read File and Add to Graph : {timer.ElapsedMilliseconds} ms");

            while (true)
            {
                SeperationDegree = 0;
                string read = Console.ReadLine();

                if(read.ToUpper().Equals("EXIT") || read.ToUpper().Equals("QUIT"))
                    Environment.Exit(0);
                

                string[] str = read.Split(" and ");

                if (!Actors.ContainsVertex(str[0]))
                {
                    Console.WriteLine($"The actor {str[0]} is not in the graph.");
                    timer.Stop();
                    
                }
                if (!Actors.ContainsVertex(str[1]))
                {
                    Console.WriteLine($"The actor {str[1]} is not in the graph.");
                    timer.Stop();

                }
                else
                {
                    if (!CheckIfConnected(str[0], str[1]))
                    {
                        Console.WriteLine("The two actors do not have a connection.");
                        timer.Stop();
                    }
                    else
                    {
                        Console.WriteLine("_________CONNECTION BELOW____________");
                        Console.WriteLine($"There are {SeperationDegree} degrees of seperation between '{str[0]}' and '{str[1]}'\n{GetShortestPathToActor(str[0], str[1])}");
                        timer.Stop();
                    }

                }

                Console.Write($"Time Elapsed To Find Connection: {timer.ElapsedMilliseconds} ms");
            }

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
                else if(Actors.ContainsVertex(temp[0]) && !Actors.ContainsVertex(temp[1]))
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
        static string GetShortestPathToActor(string str1, string str2)
        {
            try
            {
                List<string> temp = Actors.FindShortestPath(str1, str2);
              
                string returnVal = "";

                for(int i = 0; i < temp.Count - 2; i+=2)
                {
                    returnVal = $"{returnVal}{temp[i]} is in {temp[i + 1]} as is {temp[i + 2]}\n";
                    SeperationDegree++;
                }
                return returnVal;

            }
            catch (ArgumentException e)
            {
                return "There is no path to this item or the actor does not exist.";
            }
        }
        
       
    }
}
