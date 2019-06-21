using ProxyLibrary;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace DispatchProxyConsoleApp
{
    class Program
    {
        static async Task Main()
        {
            var undeocratedWidget = new Widget("Widgetty", 9.99);

            // Note that the proxy is of type IWidget rather than Widget.
            // The returned object is actually of type DispatchProxyLoggingDecorator
            // (so any helper methods on that type can be used in addition to IWidget APIs)
            var widget = DispatchProxyLoggingDecorator<IWidget>.Decorate(undeocratedWidget);

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
            // Since DispatchProxy only returns interfaces, there's not a good way to access fields
            // in the decorated object. One option would be to expose the decorated object so that
            // (with lots of casting) users can use it. Another option is to add 'field access'
            // methods to the proxy for getting/setting field values. This still requires some casting
            // (from T to DispatchProxyLoggingDecorator<T>) and requires specifying field name instead of 
            // just accessing the field directy. It does have the advantage of providing an opportunity 
            // to proxy the field access if desired,though.
            // Finally, it's possible to access fields by keeping a reference to the undecorated object
            // for that purpose.
            
            // Set field value using the decorated object
            ((Widget)((DispatchProxyLoggingDecorator<IWidget>)widget).Target)._description = "The best widget of all!";

            // Set field value with a helper method
            ((DispatchProxyLoggingDecorator<IWidget>)widget).SetFieldValue("_description", "The best widget of all");

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            // Get field
            // Get field value using the decorated object
            Console.WriteLine($"Description updated to : {((Widget)((DispatchProxyLoggingDecorator<IWidget>)widget).Target)._description}");

            // Get field value with a helper method
            Console.WriteLine($"Description updated to : {((DispatchProxyLoggingDecorator<IWidget>)widget).GetFieldValue("_description")}");

            // Get field value using the undecorated object
            Console.WriteLine($"Description updated to : {undeocratedWidget._description}");

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
