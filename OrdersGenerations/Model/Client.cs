using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersGenerations.Model
{
    public class Client
    {        
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string SurnameName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4}", Description, LastName, FirstName, SurnameName, Address);
        }
    }
}
