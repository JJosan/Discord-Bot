using System;
using System.IO;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using C5;

// constructor: new BDOapi(string start, string end)
// object.CheapestImperialMeals

namespace HappyFunTimes.BDO
{
    class BDOapi
    {

        System.Collections.Generic.IList<System.Collections.Generic.IList<object>> values;

        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };

        // can be anything
        static readonly string ApplicationName = "BDO things";

        // from the url
        static readonly string SpreadsheetId = "14CBNviQYCmdkL7jEoFnHFnz_18qlWKbDZ70r3M2KkrM";

        // name of page
        static readonly string sheet = "BDO Market Prices";

        static SheetsService service;

        // start: top right of the chart
        // end: bottom left of the charte
        public BDOapi(string start, string end)
        {
            GetCredentials();
            SearchDoc(start, end);
        }

        // giving the application access to the sheet using "client_secrets.json found in the directory"
        // the json file can be downloaded after creating the service email from google
        private void GetCredentials()
        {
            GoogleCredential credential;
            using (var stream = new FileStream("../../BDO/client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }

            service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        private void SearchDoc(string start, string end)
        {
            start = start.ToUpper();
            end = end.ToUpper();
            // goes through the api and finds all of the meals
            var range = $"{sheet}!{start}:{end}";
            var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);
            var response = request.Execute();
            // cells can be accessed as values[row][column] starting
            // from A2 of the BDO api google sheet
            values = response.Values;
        }

        public IPriorityQueue<MealNode> CheapestImperialMeals()
        {
            IPriorityQueue<MealNode> sortedMeals = new IntervalHeap<MealNode>();
            List<string[]> meals = new List<string[]>();
            foreach (string line in File.ReadLines("../../BDO/MealList.txt"))
            {
                string[] item = line.Split('\t');
                meals.Add(item);
            }

            // when they are found, do stuff with each row
            if (values != null && values.Count > 0)
            {
                /*
                // O(n^2) sequential implementation
                for (int i = 0; i < values.Count; i++)
                {
                    for (int j = 0; j < meals.Count; j++)
                    {
                        // if (meal from file == meal from spreadsheet && market count > 0)
                        if (meals[j][0].ToString().Equals(values[i][0]) && Int32.Parse(values[i][2].ToString()) > 0)
                        {
                            MealNode newItem = new MealNode(meals[j][0].ToString(),
                                Int32.Parse(meals[j][1].ToString()),
                                Int32.Parse(values[i][4].ToString().Replace(",", "")));
                            sortedMeals.Add(newItem);
                        }
                    }
                }
                */
                
                
                // log(N) binary search implementation
                int startingPlace = 0;
                for (int i = 0; i < values.Count; i++)
                {  
                    if (values[i][0].ToString()[0] == 'A')
                    { 
                        startingPlace = i;
                        break;
                    }
                }
                for (int i = 0; i < meals.Count; i++)
                {
                    binarySearch(startingPlace, values.Count, this.values, meals, i, sortedMeals);
                }
                


            }
            return sortedMeals;
        }
        
        public void binarySearch(int low, int high, System.Collections.Generic.IList<System.Collections.Generic.IList<object>> array, List<string[]> theMeal, int currentI, IPriorityQueue<MealNode> sortedMeals)
        {
            int mid = (low + high) / 2;
            if (theMeal[currentI][0].Equals(array[mid][0].ToString()) && Int32.Parse(array[mid][2].ToString()) != 0)
            {
                string meal = theMeal[currentI][0];
                int mealsPerBox = Int32.Parse(theMeal[currentI][1]);
                int basePrice = Int32.Parse(array[mid][4].ToString().Replace(",", ""));
                var node = new MealNode(meal, mealsPerBox, basePrice);
                sortedMeals.Add(node);
            }
            if (theMeal[currentI][0].CompareTo(array[mid][0]) > 0)
            {
                binarySearch(mid + 1, high, array, theMeal, currentI, sortedMeals);
            }
            if (theMeal[currentI][0].CompareTo(array[mid][0]) < 0)
            {
                binarySearch(low, mid - 1, array, theMeal, currentI, sortedMeals);
            }
            
        }
        

        // working basic version of a binary search
        /*
        public void binarySearch(int low, int high, string[] array, string word)
        {
            int mid = (low + high) / 2;

            Console.WriteLine("low: " + low);
            Console.WriteLine("mid: " + mid);
            Console.WriteLine("high: " + high);
            Console.WriteLine(array);
            if (word.Equals(array[mid]))
            {
                Console.WriteLine(word);
            } else if (low > high)
            {
                Console.WriteLine("word not found");
            }
            if (word.CompareTo(array[mid]) > 0)
            {
                Console.WriteLine(">");
                binarySearch(mid + 1, high, array, word);
            }
            if (word.CompareTo(array[mid]) < 0)
            {
                Console.WriteLine("<");
                binarySearch(low, mid - 1, array, word);
            }
            
        }
        */
    }

    

    class MealNode : IComparable<MealNode>
    {
        public string meal;
        public int mealsPerBox;
        public int basePrice;
        public int packageCost;

        public MealNode(string meal, int mealsPerBox, int basePrice)
        {
            this.meal = meal;
            this.mealsPerBox = mealsPerBox;
            this.basePrice = basePrice;
            this.packageCost = mealsPerBox * basePrice;
        }

        public int CompareTo(MealNode other)
        {
            return this.packageCost - other.packageCost;
        }
    }
}

