using OrdersGenerations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrdersGenerations.Repository
{
    public interface IProductRepository
    {
        IEnumerable<Product> Products { get; }
        void SaveProduct(Product product);
    }
}
