using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFxConsoleApp
{
    sealed class Widget : MarshalByRefObject
    {
        private string _name;
        private double _price;
        private Color _color;

        public string Name
        {
            get => _name;

            set
            {
                _name = value;
                LastUpdated = DateTimeOffset.Now;
            }
        }

        public double Price
        {
            get => _price;

            set
            {
                _price = value;
                LastUpdated = DateTimeOffset.Now;
            }
        }
        public Color Color
        {
            get => _color;

            set
            {
                _color = value;
                LastUpdated = DateTimeOffset.Now;
            }
        }

        public DateTimeOffset LastUpdated { get; private set; }

        // Leave as a field to demonstrate proxy interaction with fields
        public string _description;

        public Widget(string name, double price)
        {
            Color = Color.White;
            Name = name;
            Price = price;
        }

        public async Task<int> SetDescriptionAsync(string newDescription)
        {
            await Task.Delay(100);
            _description = newDescription;
            LastUpdated = DateTimeOffset.Now;

            return _description.Length;
        }

        // Exercise out and ref parameters
        public void SetDescriptionAndGetPrice(string newDescription, out double price, ref bool dummy)
        {
            _description = newDescription;
            price = Price;
            dummy = !dummy;
        }

        public bool BuyWidget()
        {
            throw new NotSupportedException("Widget purchasing coming in v2");
        }

        public override string ToString() =>
            $"{Color} {Name} ({Price}){(string.IsNullOrEmpty(_description)?string.Empty: $": {_description}")}";
    }
}
