using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Microsoft.EntityFrameworkCore;

namespace CocktailConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            InitializeAndSeedDatabase();

            while (true)
            {
                PrintMenuAndWaitForInput();
            }
        }


        private static void PrintMenuAndWaitForInput()
        {
            Console.Clear();
            Console.WriteLine("---Cocktail Application---");
            Console.WriteLine("Input number finish with ENTER: ⏎");
            Console.WriteLine("1. Search for Drinks");
            Console.WriteLine("2. Add new Drink");
            Console.WriteLine("3. Modify Existing Drink");
            Console.WriteLine("4. Exit Application");

            Console.Write("#(1-4): ");

            switch (WaitForValidUserInput(1, 4))
            {
                case 1:
                    PrintSearchForDrinksMenu();
                    break;
                case 2:
                    PrintAddNewDrinkMenu();
                    break;
                case 3:
                    PrintModifyExistingDrinkMenu();
                    break;
                case 4:
                    Environment.Exit(0);
                    break;
            }
        }


        private static int WaitForValidUserInput(int minValue, int maxValue)
        {
            bool validInputEntered = false;
            int selectedOption = -1;


            while (!validInputEntered)
            {
                string userInput = Console.ReadLine();

                bool validIntegerEntered = int.TryParse(userInput, out selectedOption);

                validInputEntered = validIntegerEntered && (selectedOption >= minValue && selectedOption <= maxValue);

                if (!validInputEntered)
                {
                    Console.WriteLine("Invalid input entered, please try again.");
                    Console.Write($"#({minValue}-{maxValue}): ");
                }
            }

            return selectedOption;
        }

        private static void PrintSearchForDrinksMenu()
        {
            Console.Clear();
            Console.WriteLine("---Search For Drinks Menu---");
            Console.WriteLine("Input number finish with ENTER 🐶");
            Console.WriteLine("1. Search by Drink Name");
            Console.WriteLine("2. Search by Ingredient Name");
            Console.WriteLine("3. Back to Main Menu");

            switch (WaitForValidUserInput(1, 3))
            {
                case 1:
                    PrintSearchByDrinkNameMenu();
                    break;
                case 2:
                    PrintSearchByIngredientMenu();
                    break;
                case 3:
                    PrintMenuAndWaitForInput();
                    break;
            }
        }


        private static void PrintAddNewDrinkMenu()
        {
            Console.WriteLine("---Add new Drink---");
            Console.Write("Name: ");

            string name = Console.ReadLine();
            
            Drink d = new Drink();

            d.Name = name;

            Console.WriteLine($"{d.Name} .. OK lets add some ingredients");

            bool finishedWithIngredients = false;
            
            while (!finishedWithIngredients)
            {
                Console.WriteLine("Current drink is this:");
                Console.WriteLine(d);
                Console.WriteLine("1. Use existing Ingredient");
                Console.WriteLine("2. Add new ingredient and use it in this drink.");
                Console.WriteLine("3. Finished adding ingredients...");

                int selectedOption = WaitForValidUserInput(1, 3);

                switch (selectedOption)
                {
                    case 1:
                        PrintAddExistingIngredientMenu(d);
                        break;
                    case 2:
                        
                        break;
                    case 3:
                        finishedWithIngredients = true;
                        break;
                }

            }
            

        }

        private static void PrintAddExistingIngredientMenu(Drink d)
        {
            Console.WriteLine("Choose from existing ingredient below...");

            List<AbstractIngredient> existingIngredients = null;
            
            using (var ctx = new CocktailContext())
            {
                existingIngredients = ctx.Ingredients.ToList();
            }

            if (existingIngredients.Count == 0)
            {
                Console.WriteLine("No existing ingredients found 😿");
                return;
            }

            int count = 1;

            foreach (AbstractIngredient ingredient in existingIngredients)
            {
                if (ingredient is Liquid)
                {
                    Console.WriteLine($"{count++}. [LIQUID]: {ingredient.Name} (ID: {ingredient.Id})");
                }

                if (ingredient is Accessory)
                {
                    Console.WriteLine($"{count++}. [ACCESSORY]: {ingredient.Name} (ID: {ingredient.Id})");
                }
            }

            int selectedIngredientIndex = WaitForValidUserInput(1, existingIngredients.Count) -1;

            AbstractIngredient selectedIngredient = existingIngredients[selectedIngredientIndex];

            Console.WriteLine("Please choose the measurement unit: ");

            int measurementUnitIndex = 1;
            
            foreach (var value in Enum.GetNames(typeof(MeasurementUnit)))
            {
                Console.WriteLine($"{measurementUnitIndex++}. ENUM->{value}");
            }

            int selectedMeasurementUnitIndex =
                WaitForValidUserInput(1, Enum.GetValues(typeof(MeasurementUnit)).Length) - 1;

            MeasurementUnit selectedUnit = (MeasurementUnit) selectedMeasurementUnitIndex;
            
            Console.WriteLine($"Okay now how many {Enum.GetName(typeof(MeasurementUnit),selectedUnit)} do you want?");

            int selectedAmount = WaitForValidUserInput(1, 1000);


            Console.WriteLine("OK lets add the ingredient...");
            
            if (existingIngredients[selectedIngredientIndex] != null)
            {
                DrinkIngredient di = new DrinkIngredient
                {
                    Amount = selectedAmount, Ingredient = selectedIngredient, MeasurementUnit = selectedUnit
                };

                d.Ingredients.Add(di);
            }

            Console.WriteLine("Your drink currently looks like this:");
            Console.WriteLine(d);

            Console.ReadKey();

        }

        private static void PrintSearchByIngredientMenu()
        {
            Console.WriteLine("---Search For Drink by Ingredient---");
            Console.Write("Ingredient: ");

            string userInput = Console.ReadLine();
            
            

            Console.WriteLine($"Searching database for drinks containing ingredient: {userInput}...");

            List<Drink> matchingDrinks = null;

            using (var ctx = new CocktailContext())
            {
                matchingDrinks = ctx
                    .Drinks
                    .Include(x => x.Ingredients)
                    .ThenInclude(x => x.Ingredient)
                    .Where(x => x.Ingredients.Any(x => x.Ingredient.Name.Contains(userInput.ToLower())))
                    .ToList();
            }

            PrintFoundDrinks(matchingDrinks);

            Console.ReadKey();

        }

        private static void PrintSearchByDrinkNameMenu()
        {
            Console.WriteLine("---Search For Drink by Name---");
            Console.Write("Name: ");

            string userInput = Console.ReadLine();

            Console.WriteLine($"Searching database for drinks with name: {userInput}...");

            List<Drink> matchingDrinks = null;
            
            using (var ctx = new CocktailContext())
            {
                matchingDrinks = ctx
                    .Drinks
                    .Include(x => x.Ingredients)
                    .ThenInclude(x => x.Ingredient)
                    .Where(x => x.Name.ToLower().Contains(userInput.ToLower())).ToList();
                
            }

            PrintFoundDrinks(matchingDrinks);

            Console.ReadKey();

        }

        private static void PrintFoundDrinks(ICollection<Drink> matchingDrinks)
        {
            if (matchingDrinks.Count > 0)
            {
                Console.WriteLine("Found the following drinks:");
                foreach (Drink drink in matchingDrinks)
                {
                    Console.WriteLine(drink);
                }
            }
            else
            {
                Console.WriteLine("No drinks found...");
            }
        }
        
     

        private static void PrintModifyExistingDrinkMenu()
        {
            Console.Clear();
            Console.WriteLine("---Modify Existing Drink Menu---");
            Console.WriteLine("Input number finish with ENTER 🙃");
        }


        private static void InitializeAndSeedDatabase()
        {
            using (var ctx = new CocktailContext())
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();

                var margarita = new Drink {Name = "Margarita"};
                var saltRim = new Accessory("salt rim");
                var crushedIce = new Accessory("crushed ice");
                var limeSegment = new Accessory("lime segment");
                var limeJuice = new Liquid("Lime Juice");
                var tripleSec = new Liquid("Triple Sec");
                var tequila = new Liquid("Tequila");


                var drinkIngredients = new List<DrinkIngredient>()
                {
                    new DrinkIngredient()
                    {
                        Amount = 60, Ingredient = limeJuice, MeasurementUnit = MeasurementUnit.ML
                    },
                    new DrinkIngredient()
                    {
                        Amount = 30, Ingredient = tripleSec, MeasurementUnit = MeasurementUnit.ML
                    },
                    new DrinkIngredient()
                    {
                        Amount = 60, Ingredient = tequila, MeasurementUnit = MeasurementUnit.ML
                    },
                    new DrinkIngredient()
                    {
                        Amount = 1, Ingredient = saltRim, MeasurementUnit = MeasurementUnit.None
                    },
                    new DrinkIngredient()
                    {
                        Amount = 1, Ingredient = crushedIce, MeasurementUnit = MeasurementUnit.None
                    },
                    new DrinkIngredient()
                    {
                        Amount = 1, Ingredient = limeSegment, MeasurementUnit = MeasurementUnit.None
                    }
                };

                foreach (DrinkIngredient drinkIngredient in drinkIngredients)
                {
                    margarita.Ingredients.Add(drinkIngredient);
                }

                ctx.Drinks.Add(margarita);
                ctx.SaveChanges();
            }
        }
    }
}