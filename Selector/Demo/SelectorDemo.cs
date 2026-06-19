// *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-* //
// Simple test script to showcase 3 of the 4           //
// available selectors in the project. Simulates       //
// the user as a customer in a store. Select items     //
// and specify an amount of said item to purchase.     //
// When choosing the Quit option, the program presents //
// the total bill amount. This file is independent     //
// from the project and can be safetly deleted.        //
// *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-* //
using Selector;

public class Program
{
    public static void Main()
    {
        // read data from Market file
        string[] lines = File.ReadAllLines(@"Demo\Market.txt");

        List<Item> items = new List<Item>();

        // parse file
        foreach (string line in lines)
        {
            int divider = line.IndexOf(',');
            
            string name = line.Substring(0, divider);
            string priceAsString = line.Substring(divider + 1);
            float price = float.Parse(priceAsString);

            Item item = new Item();
            item.name = name;
            item.price = price;

            items.Add(item);
        }

        // send all items from the Market file over to a Vertical Scroll Selector. There are too many options to display on screen, so we'll need to scroll through all of them.
        VerticalScrollSelector<Item> itemSelector = new VerticalScrollSelector<Item>(items, "{0}");
        itemSelector.MaxDisplayOptions = 5;

        // create menu options
        Option[] menuOptions =
        {
            new Option()
            {
                name = "Shop"
            },
            new Option()
            {
                name = "Quit"
            }
        };

        // send menu options into a Fixed Horizontal Selector. No scrolling is needed
        HorizontalFixedSelector<Option> menuSelector = new HorizontalFixedSelector<Option>(menuOptions);

        // create a set of value options from 1 through 20. Will allow user to purchase in bulk.
        int valueCount = 20;
        ValueOption[] values = new ValueOption[valueCount];
        for (int i = 0; i < valueCount; i++)
        {
            values[i] = new ValueOption()
            {
                value = i + 1
            };
        }

        // send all value options into a horizontal scroll selector. Takes up little screen space as long as we specify a maximum selector width.
        HorizontalScrollSelector<ValueOption> valueSelector = new HorizontalScrollSelector<ValueOption>(values, format: "{0}");
        valueSelector.Model.highlightedBackgroundColor = Console.BackgroundColor;
        valueSelector.Model.highlightedForegroundColor = ConsoleColor.Green;
        valueSelector.ScrollLeftSymbol = "< ";
        valueSelector.ScrollRightSymbol = " >";
        valueSelector.MaxSelectorWidth = 7;

        float bill = 0;
        // loop until user selects the quit menu option
        while (true)
        {
            Console.Clear();

            // print header
            Console.WriteLine("Use the arrow keys to switch between the options");
            Console.WriteLine($"Current Bill: " + PrintAsUSCurrency(bill));
            Console.WriteLine();
            Console.WriteLine();

            // ask user to select a menu option
            Option? menuOption = menuSelector.QuerySelection();
            // clear the selection from the console
            menuSelector.ClearSelection();

            // menuOption cannot be null but we add the condition to avoid receiving any warnings from the console.
            // If the user selects "Quit", we exit the loop
            if (menuOption == null || menuOption.name == "Quit")
                break;

            // ask user to select an item
            Item? selectedItem = itemSelector.QuerySelection();
            if (selectedItem == null)
                return;

            // remove the item menu
            itemSelector.ClearSelection();

            Console.Write("Price of " + selectedItem.name + " is $" + selectedItem.price + ". Select amount: ");
            
            // ask user for the amount of the item chosen
            int value = valueSelector.QuerySelection()?.value ?? 0; // shortcut for null check

            // remove the value menu
            valueSelector.ClearSelection();

            // calculate current bill
            bill += selectedItem.price * value;

            Console.WriteLine(value + "x " + selectedItem.name + " added to your shopping cart!");

            Console.ReadKey();
        }
        // print farewell message
        Console.WriteLine("Your total bill is " + PrintAsUSCurrency(bill));
        Console.WriteLine("Thanks for shopping with us");

        Console.ReadKey();
    }

    public static string PrintAsUSCurrency(float value)
    {
        return $"${value:F2}";
    }

    class ValueOption : IOption
    {
        public int value;

        public string GetName() => value.ToString();
        public ConsoleColor GetColor() => ConsoleColor.White;
    }

    class Option : IOption
    {
        public string name = "";
        public ConsoleColor color = ConsoleColor.White;

        public string GetName()
        {
            return name;
        }
        public ConsoleColor GetColor()
        {
            return color;
        }
    }

    class Item : IOption
    {
        public string name = "";
        public float price = 0;
        public ConsoleColor color = ConsoleColor.White;

        public string GetName()
        {
            return name + " $" + price;
        }

        public ConsoleColor GetColor()
        {
            return color;
        }
    }
}
