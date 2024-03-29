﻿using System;
using System.Linq;
 
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
 
namespace Cars_NS
{
    class Program
    {   
        //Get the Data in JSON Serialized Form
        static string carsData = JsonConvert.SerializeObject(new CarsDatabase(), Formatting.Indented);
        
        //Convert the JSON string into an Array
        static JArray carsArray = JArray.Parse(carsData);

        static void Main(string[] args)
        {   
            bool showMenu = true;
            while (showMenu)
            {
                showMenu = MainMenu();
            }

        }
        private static bool MainMenu()
        {
            Console.Clear();
            Console.WriteLine("Choose an option:");
            Console.WriteLine(" 1) Add a Car");
            Console.WriteLine(" 2) Which Make appears the most?");
            Console.WriteLine(" 3) List of every car and its properties");
            Console.WriteLine(" 4) What's the largest engine?");
            Console.WriteLine(" 5) What's the smallest engine?");
            Console.WriteLine(" 6) What's the average year?");
            Console.WriteLine(" 7) What's the newest year?");
            Console.WriteLine(" 8) What's the total amount of cars?");
            Console.WriteLine(" 9) Show only the model names, and the year");
            Console.WriteLine("10) Show the year, model, and make");
            Console.WriteLine("11) What would the model name be if printed in reverse?");
            Console.WriteLine("12) Print out all the Chevy cars and all the properties, BUT, change Chevy to Chevrolet");
            Console.WriteLine("13) Print out the years but add a space between each number");
            Console.WriteLine(" 0) Exit");
            Console.Write("\r\nSelect an option: ");
 
            switch (Console.ReadLine())
            {
                case "1":
                    AddCar(carsArray);
                    return true;
                case "2":
                    MakeCount();
                    return true;                
                case "3":
                    ListCars("All");
                    return true;                
                case "4":
                    MaxValue("EngineSize");
                    return true;
                case "5":
                    MinValue("EngineSize");
                    return true;
                case "6":
                    AvgValue("Year");
                    return true;
                case "7":
                    MaxValue("Year");
                    return true;
                case "8":
                    CarCount();
                    return true;
                case "9":
                    ListCars("MOY");
                    return true;
                case "10":
                    ListCars("YMOMA");
                    return true;
                case "11":
                    ReverseModel();
                    return true;
                case "12":
                    SearchCar("Chevy", "Chevrolet");
                    return true;
                case "13":
                    YearSpace();
                    return true;               
                case "0":
                    return false;
                default:
                    return true;
            }
        }

        public static void AddCar(JArray cars_array)
        {                      
            Console.WriteLine("Method to create a car");

            Console.Write("\r\nCar Company: ");
                string make = Console.ReadLine();  
            
            Console.Write("\r\nCar Model: ");
                string model = Console.ReadLine();
            
            Console.Write("\r\nCar Year: ");
                int year = int.Parse(Console.ReadLine());
            
            Console.Write("\r\nCar Engine Size: ");
                int engineSize = int.Parse(Console.ReadLine());
            
            cars_array.Add(new JObject()        
            {
                {"Make", make},
                {"Model", model},
                {"Year", year},
                {"EngineSize", engineSize}
            });
        }

        public static void ListCars(string combination)
        {   /*Combination parameter
                All = All columns
                MOY = Model, Year
                YMOMA = Year, Model, Make
            */

            var resCars = carsArray.Select( c => c);

            if (combination=="All")                
                Console.WriteLine("Make \t\t Model \t\t Year \t\t Engine Size");
            else if (combination=="MOY")
                Console.WriteLine("Model \t\t Year");
            else if (combination=="YMOMA") 
                Console.WriteLine("Year \t\t Model \t\t Make");
            
            foreach (var item in resCars)
            {
                if (combination=="All")
                    Console.WriteLine(item["Make"] +"\t\t"+ item["Model"] +"\t\t"+ item["Year"] +"\t\t"+ item["EngineSize"]);                
                else if (combination=="MOY") 
                    Console.WriteLine(item["Model"] +"\t\t"+ item["Year"]);
                else if (combination=="YMOMA")
                    Console.WriteLine(item["Year"] +"\t\t"+ item["Model"] +"\t\t"+ item["Make"]);

            }

            Console.ReadLine();            
        }

        public static void MakeCount()
        {
            var makeCount = carsArray.GroupBy (g => g["Make"])
                                .OrderByDescending (makeGroup => makeGroup.Count ())
                                .Select (
                                    makeGroup =>
                                        new 
                                        {
                                            Make = makeGroup.Key,
                                            Count = makeGroup.Count ()
                                        }
                                );

            Console.WriteLine("Make \t\t Total of makes");          
            foreach (var item in makeCount)
            {
                                
                Console.WriteLine(item.Make + " \t\t " + item.Count);
                break;
            }
    
            Console.ReadLine();
        }

        public static void MaxValue(string column)
        {
            int max = carsArray
                    .SelectTokens("$.."+column)
                    .Select(s => s.Value<int>())
                    .Max();
            
            Console.WriteLine("Max value for column "+column+" is: "+max);           

            Console.ReadLine();
        }

        public static void MinValue(string column)
        {
            int min = carsArray
                    .SelectTokens("$.."+column)
                    .Select(s => s.Value<int>())                    
                    .Min();

            Console.WriteLine("Min value for column "+column+" is: "+min);

            Console.ReadLine();
        }        

        public static void AvgValue(string column)
        {                      
            decimal avg = carsArray
                            .SelectTokens("$.."+column)
                            .Select(a => a.Value<decimal>())
                            .Average();

            Console.WriteLine("Average value for column "+ column +" is: "+string.Format("{0:N}",avg));
            Console.ReadLine();
        }

        public static void CarCount(){
            int count = carsArray.Count();
            Console.WriteLine("Total amount of cars: "+count);
            Console.ReadLine();
        }

        public static void ReverseModel()
        {
            var reverseModel = from s in carsArray
                                select s["Model"];
            
            Console.WriteLine("Car Model in reverse:");
            foreach (var item in reverseModel)
            {
                string itemString = item.ToString();
                char[] cArray = itemString.ToCharArray();
                Array.Reverse(cArray);
                Console.WriteLine(cArray);
            }

            Console.ReadLine();
        }

        public static void SearchCar(string makeKeyName, string newValue)
        {
            string makeBefore;
            var items = carsArray
                            .SelectTokens("$.[?(@.Make=='"+makeKeyName+"')]");
                    
            Console.WriteLine("Before update");
            foreach(var item in items)
                Console.WriteLine(item);
            
            Console.WriteLine("After update");
            foreach(var item in items)
            {
                makeBefore = (string)item["Make"];              
                if ( makeBefore == makeKeyName)
                    item["Make"] = newValue;

                Console.WriteLine(item);
            }

            Console.ReadLine();
        }

        public static void YearSpace()
        {
            var yearSpace = from s in carsArray
                            select s["Year"];
            
            foreach (var item in yearSpace)
            {
                string itemString = item.ToString();
                char[] cArray = itemString.ToCharArray();
                Console.WriteLine(String.Join(" ",cArray));
            }

            Console.ReadLine();
        }
    }
}