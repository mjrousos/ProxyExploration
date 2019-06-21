using ProxyLibrary;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace DynamicProxyConsoleApp
{
    class Program
    {
        static async Task Main()
        {
            await TestInheritanceProxy();
            await TestCompositionProxy();
        }

        static async Task TestInheritanceProxy()
        {
            Console.WriteLine();
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Creating inheritance-based proxy");
            Console.WriteLine("--------------------------------");
            Console.WriteLine();

            var widget = DynamicProxyLoggingDecorator.DecorateViaInheritance<Widget>();

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
            // DynamicProxy class proxies allow access to fields (since they subclass the target),
            // but don't intercept the access. 
            widget._description = "The best widget of all!";
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            // Get field
            // As with setting fields, this will work but will not be intercepted.
            Console.WriteLine($"Description updated to : {widget._description}");

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            // Call overridden method
            Console.WriteLine($"Widget: {widget.ToString()}");

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
            catch (NotSupportedException)
            {
                Console.WriteLine("Caught expected exception");
            }
        }

        static async Task TestCompositionProxy()
        {
            Console.WriteLine();
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Creating composition-based proxy");
            Console.WriteLine("--------------------------------");
            Console.WriteLine();

            var undecoratedWidget = new Widget("Widgetty", 9.99);
            var widget = DynamicProxyLoggingDecorator.DecorateViaComposition<IWidget>(undecoratedWidget);

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
            // DynamicProxy interface proxies don't allow access to fields (since they implement an interface)
            // The only way to get/set fields is via private reflection or by accessing the undecorated object
            undecoratedWidget._description = "The best widget of all!";
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            // Get field
            Console.WriteLine($"Description updated to : {undecoratedWidget._description}");

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            // Call overridden method
            Console.WriteLine($"Widget: {widget.ToString()}");

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
            catch (NotSupportedException)
            {
                Console.WriteLine("Caught expected exception");
            }
        }

    }
}
