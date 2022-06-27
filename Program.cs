using System;
using System.Linq;
 
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
 
namespace CARS_TASK
{
    class Program
    {   
        //Get the Data in JSON Serialized Form
        static string carsData = JsonConvert.SerializeObject(new CarsDatabase(), Formatting.Indented);
        //Console.WriteLine(carsData);

        //Convert the JSON string into an Array
        static JArray carsArray = JArray.Parse(carsData);

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
            Console.WriteLine(" 9) Show only the model names and the year");
            Console.WriteLine("10) Show the year, model, and make");
            Console.WriteLine("11) What would the model name be if printed in reverse?");
            Console.WriteLine("12) Print out all the Chevy cars and all the properties, BUT, change Chevy to Chevrolet");
            Console.WriteLine("13) Print out the years but add a space between each number");
            Console.WriteLine(" 0) Exit");
            Console.Write("\r\nSelect an option: ");
 
            switch (Console.ReadLine())
            {
                case "1":
                    InsertCar();
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
        static void Main(string[] args)
        {   
            bool showMenu = true;
            while (showMenu)
            {
                showMenu = MainMenu();
            }                            
        }

        public static void InsertCar()
        {
            string make, model;
            int year, engineSize;

            Console.WriteLine("Method to create a car");

            Console.Write("\r\nCar Company: ");
                make = Console.ReadLine();  
            Console.Write("\r\nCar Model: ");
                model = Console.ReadLine();
            Console.Write("\r\nCar Year: ");
                year = int.Parse(Console.ReadLine());
            Console.Write("\r\nCar Engine Size: ");
                engineSize = int.Parse(Console.ReadLine());

            AddCar(carsArray, make, model, year, engineSize);
        }
        public static void AddCar(JArray cars_array, string? make, string? model, int year, int engineSize)
        {
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

            var resCars = from c in carsArray
                            select c;
                                     
            if (combination=="All") Console.WriteLine("Make \t Model \t Year \t Engine Size");

            if (combination=="MOY") Console.WriteLine("Model \t Year");

            if (combination=="YMOMA") Console.WriteLine("Year \t Model \t Make");

            foreach (var item in resCars)
            {
                if (combination=="All") Console.WriteLine(item["Make"] +"\t"+ item["Model"] +"\t"+ (string)item["Year"] +"\t"+ item["EngineSize"]);                
                
                if (combination=="MOY") Console.WriteLine(item["Model"] +"\t"+ item["Year"]);

                if (combination=="YMOMA")Console.WriteLine(item["Year"] +"\t"+ item["Model"] +"\t"+ item["Make"]);

            }

            Console.ReadLine();            
        }

        public static void MakeCount()
        {
            var makeCount = from g in carsArray
                                group g by g["Make"] into makeGroup                                 
                                orderby makeGroup.Count() descending
                            select new
                            {
                                Make = makeGroup.Key,
                                Count = makeGroup.Count()                               
                            };
            
            Console.WriteLine("Make \t Total of makes");          
            foreach (dynamic item in makeCount)
            {
                //Console.WriteLine(item);                
                Console.WriteLine(item.Make + " \t " + item.Count);
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

            Console.WriteLine("Average value for column "+ column +" is: "+avg);
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