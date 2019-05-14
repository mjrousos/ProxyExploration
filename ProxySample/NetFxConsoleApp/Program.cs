using ProxyLibrary;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace NetFxConsoleApp
{
    class ConsoleApp
    {
        public static async Task Main()
        {
            var widget = RealProxyLoggingDecorator<Widget>.Decorate(new Widget("Widgetty", 9.99));

            // Set property
            widget.Color = Color.Red;

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            // Get property
            Console.WriteLine($"Last updated: {widget.LastUpdated}");

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            // Set field
            widget._description = "The best widget of all!";

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            // Get field
            Console.WriteLine($"Description updated to : {widget._description}");

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            // Call overridden method
            widget.ToString();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            // Call async method
            Console.WriteLine($"SetDescriptionAsync return: {await widget.SetDescriptionAsync("The best widget ever!")}");

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            // Call method with out and ref parameters
            var boolValue = false;
            widget.SetDescriptionAndGetPrice("It's a widget!", out double priceOut, ref boolValue);
            Console.WriteLine($"Received {priceOut} & {boolValue} from out and ref parameters");

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            // Call method that throws exception

            try
            {
                widget.BuyWidget();
            }
            catch (NotSupportedException) { }
        }
    }
}