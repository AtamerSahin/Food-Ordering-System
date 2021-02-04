using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderingSystem
{
    public class Restaurant
    {
        public List<Menu> menus = new List<Menu>();

        // Constructor

        public Restaurant(int id, string name, string phone)
        {
            this.id = id;
            this.name = name;
            this.phone = phone;
        }

        // Adding a menu to the Restaurant

        public void AddMenu(int id,string name, int price)
        {
            this.menus.Add(new Menu(id, name, price));
        }

        public int id
        {
            get; set;
        }

        public string name
        {
            get; set;
        }

        public string phone
        {
            get; set;
        }

    }
}
