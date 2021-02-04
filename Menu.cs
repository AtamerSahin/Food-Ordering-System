using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderingSystem
{
    public class Menu
    {
        // Constructor

        public Menu(int id, string name, int price)
        {
            this.id = id;
            this.name = name;
            this.price = price;
        }

        public string name
        {
            get; set;
        }

        public int id
        {
            get; set;
        }

        public int price
        {
            get; set;
        }
    }
}
