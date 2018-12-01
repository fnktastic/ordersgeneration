using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersGenerations.Model
{
    public class Order
    {
        public int ID { get; set; }
        public DateTime CreatedDate { get; set; }

        public Client Client { get; set; }
 
        public List<Position> Positions { get; set; }
    }
}
