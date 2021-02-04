using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderingSystem
{
    public class User
    {
        // Constructor
        public User(string username, string password, string tcID, string name, string surname, string phone, string address)
        {
            this.username = username;
            this.password = password;
            this.tcID = tcID;
            this.name = name;
            this.surname = surname;
            this.phone = phone;
            this.address = address;
        }

        public string tcID
        {
            get; set;
        }

        public string username
        {
            get; set;
        }

        public string password
        {
            get; set;
        }

        public string name
        {
            get; set;
        }

        public string surname
        {
            get; set;
        }

        public string phone
        {
            get; set;
        }

        public string address
        {
            get; set;
        }

    }
}
