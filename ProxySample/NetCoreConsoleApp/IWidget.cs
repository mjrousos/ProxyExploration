using System;
using System.Drawing;
using System.Threading.Tasks;

namespace NetCoreConsoleApp
{
    interface IWidget
    {
        Color Color { get; set; }
        DateTimeOffset LastUpdated { get; }
        string Name { get; set; }
        double Price { get; set; }

        bool BuyWidget();
        void SetDescriptionAndGetPrice(string newDescription, out double price, ref bool dummy);
        Task<int> SetDescriptionAsync(string newDescription);
        string ToString();
    }
}