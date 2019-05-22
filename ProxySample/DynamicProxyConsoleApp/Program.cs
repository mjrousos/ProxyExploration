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

            var widget = DynamicProxyLoggingDecorator.DecorateViaInheritance(new Widget("Widgetty", 9.99));

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
            // but don't intercept the access and - more confusingly - don't work properly in
            // 'class proxy with target' scenarios since field access will go to the proxy object - 
            // *not* to the target object.
            widget._description = "The best widget of all!";
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            // Get field
            // This appears to work, but the updated description won't be used by the subsequent ToString call
            // (since that method will run on the target object and the field was updated on the proxy object).
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
            // This appears to work, but the updated description won't be used by the subsequent ToString call
            // (since that method will run on the target object and the field was updated on the proxy object).
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
