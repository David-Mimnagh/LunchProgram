using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LunchToday
{
    class Program
    {
        public static string FILEPATH = "../../Files/";
        public class LunchClass
        {
            public string Name { get; set; }
            public string[] Type { get; set; }
            public string Price { get; set; }
            public int Percentage { get; set; }

            public static LunchClass ReadCSV(string csvLine)
            {
                string[] values = csvLine.Split(',');

                LunchClass l = new LunchClass();
                l.Name = values[0];
                l.Type = values[1].Split('/');
                l.Price = values[2];
                l.Percentage = Convert.ToInt32(values[3]);
                return l;
            }
        }
      
        static List<string> GetTypes(List<LunchClass> lunches, List<string> toldTypes)
        {
            List<string> copyToldTypes = new List<string>();
            int index = 0;
            for (int i = 0; i < lunches.Count; i++)
            {
                if (i == 0)
                {
                    index++;
                    for (int j = 0; j < lunches[i].Type.Length; j++)
                    {
                        toldTypes.Add(lunches[i].Type[j]);
                    }
                }
                else
                {
                    copyToldTypes = toldTypes;
                    for (int j = 0; j < lunches[i].Type.Length; j++)
                    {
                        if (!copyToldTypes.Contains(lunches[i].Type[j]))
                        {
                            toldTypes.Add(lunches[i].Type[j]);
                        }


                    }

                }
            }
            return toldTypes;
        }

        static int GetUserType(int userType)
        {
            string userTypeString = "";
            do
            {
                try
                {
                    Console.Write("\n\tWhat type of food would you like to have today?(select by index)");
                    userTypeString = Console.ReadLine();
                    Convert.ToInt32(userTypeString);
                    if (Convert.ToInt32(userTypeString) > 0)
                        userType = Convert.ToInt32(userTypeString);
                }
                catch (Exception ex)
                {
                    Console.Write("\n\nPlease make sure you write a number and that it is > 0.");
                }
            } while (userType <= 0);
            return userType;
        }
        static string GetLunchByType(string newLunchName, int userType, int amountSpending, List<string> toldTypes, List<LunchClass> lunches)
        {
            string lunchType = toldTypes[userType-1];
            amountSpending = GetAmountSpending(amountSpending);
            List<string> poss = new List<string>();
            foreach(var l in lunches)
            {
                if (l.Type.Contains(lunchType))
                {
                    if( Convert.ToInt32(l.Price) <= amountSpending)
                         poss.Add(l.Name);
                }
            }
            if (poss.Count > 0)
                return GetNewLunchName(newLunchName, poss);
            else
                return "nothing";

        }
        static void PrintList(List<string> toldTypes)
        {
            for (int i = 0; i < toldTypes.Count; i++)
            {
                Console.Write("\n\t" + (i + 1).ToString() + ". " + toldTypes[i]);
            }
        }

        static List<string> GetPlacesOverAmount(int amount, List<LunchClass> lunches)
        {
            List<string> poss = new List<string>();

            foreach (var l in lunches)
            {
                if (Convert.ToInt32(l.Price) <= amount)
                    poss.Add(l.Name);
            }

            return poss;
        }

        static int GetRandom(int max)
        {
            Random r = new Random();
            int num = r.Next(0, max);
            return num;
        }
        static int GetAmountSpending(int amountSpending)
        {
            string userAmount = "";
            do
            {
                try
                {
                    Console.Write("\n\t How much would you roughly like to spend today?");
                    userAmount = Console.ReadLine();
                    Convert.ToInt32(userAmount);
                    if (Convert.ToInt32(userAmount) > 0)
                        amountSpending = Convert.ToInt32(userAmount);
                }
                catch (Exception ex)
                {
                    Console.Write("\n\nPlease make sure you write a number and that it is > 0.");
                }
            } while (amountSpending <= 0);
            return amountSpending;
        }

        static string GetNewLunchName(string newLunchName, List<string> couldHave)
        {

            List<string> prevLunchNames = new List<string>();
            prevLunchNames = File.ReadAllLines(FILEPATH + "previousLunches.txt").ToList();
            List<string> prevTwo = new List<string>();
            prevTwo.Add(prevLunchNames.Last());
            prevTwo.Add(prevLunchNames.ElementAt(prevLunchNames.Count - 2));
            do
            {
                newLunchName = couldHave[GetRandom(couldHave.Count)];
            } while (prevTwo.Contains(newLunchName));

            return newLunchName;
        }

        static void Main(string[] args)
        {
            string newLunchName = "nothing";
            string answer = "";
            char ans = new char();
            int amountSpending = 0;
            int userType = 0;
            List<string> toldTypes = new List<string>();
            List<string> couldHave = new List<string>();
            List<LunchClass> lunches = File.ReadAllLines(FILEPATH + "lunchlist.csv").Skip(1).Select(l => LunchClass.ReadCSV(l)).ToList();
            toldTypes = GetTypes(lunches, toldTypes);
            Console.Write("-------Feeling Hungry?--------");

            Console.Write("\n\tGreat that's what I am here for!");
            Console.Write("\n\tDo you have a specific type of food in mind? (yes/no)");
            answer = Console.ReadLine();
            answer = answer.ToLower();
            ans = answer[0];
            switch (ans)
            {
                case 'y':
                    Console.Write("\n\tGood, this will make it easier...");
                    Console.Write("\n\tHere is your current list of types to choose from:");
                    do
                    {
                        PrintList(toldTypes);
                        userType = GetUserType(userType);
                        newLunchName = GetLunchByType(newLunchName, userType, amountSpending, toldTypes, lunches);
                        if(newLunchName == "nothing")
                        {
                            Console.Write("\n\tCouldn't find any restraunts of that type for that type...");
                            Console.Write("\n\tHere is your current list of types to choose from:");
                        }
                    } while (newLunchName == "nothing");
                    break;
                case 'n':
                    Console.Write("\n\tNo problem, I can choose for you!");
                    amountSpending = GetAmountSpending(amountSpending);
                    couldHave = GetPlacesOverAmount(amountSpending, lunches);
                    newLunchName = GetNewLunchName(newLunchName, couldHave);
                    break;
            }
            Console.Write("\n\n\n Looks like we're going to: " + newLunchName + " for lunch today!");
            File.AppendAllText(FILEPATH + "previousLunches.txt", Environment.NewLine + newLunchName);

            Console.Read();
        }
    }
}
