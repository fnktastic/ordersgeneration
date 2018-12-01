using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersGenerations.Model
{
    public class Product
    {
        public int ID { get; set; }
        public string Barcode { get; set; }
        public string Caption { get; set; }
        public double Price { get; set; }
    }
}
