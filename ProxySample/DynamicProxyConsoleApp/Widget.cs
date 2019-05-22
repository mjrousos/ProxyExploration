using System;
using System.Drawing;
using System.Threading.Tasks;

namespace DynamicProxyConsoleApp
{
    public class Widget : IWidget
    {
        private string _name;
        private double _price;
        private Color _color;

        // Class proxies from DynamicProxy work by inheritance, so
        // all proxied methods and properties must be virtual
        public virtual string Name
        {
            get => _name;

            set
            {
                _name = value;
                LastUpdated = DateTimeOffset.Now;
            }
        }

        public virtual double Price
        {
            get => _price;

            set
            {
                _price = value;
                LastUpdated = DateTimeOffset.Now;
            }
        }
        public virtual Color Color
        {
            get => _color;

            set
            {
                _color = value;
                LastUpdated = DateTimeOffset.Now;
            }
        }

        public virtual DateTimeOffset LastUpdated { get; private set; }

        // Leave as a field to demonstrate proxy interaction with fields
        public string _description;

        public Widget() { }

        public Widget(string name, double price)
        {
            Color = Color.White;
            Name = name;
            Price = price;
        }

        public virtual async Task<int> SetDescriptionAsync(string newDescription)
        {
            await Task.Delay(100);
            _description = newDescription;
            LastUpdated = DateTimeOffset.Now;

            return _description.Length;
        }

        // Exercise out and ref parameters
        public virtual void SetDescriptionAndGetPrice(string newDescription, out double price, ref bool dummy)
        {
            _description = newDescription;
            price = Price;
            dummy = !dummy;
        }

        public virtual bool BuyWidget()
        {
            throw new NotSupportedException("Widget purchasing coming in v2");
        }

        public override string ToString() =>
            $"{Color} {Name} ({Price}){(string.IsNullOrEmpty(_description) ? string.Empty : $": {_description}")}";
    }
}
