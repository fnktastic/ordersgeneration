using OrdersGenerations.DataAccess;
using OrdersGenerations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrdersGenerations.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly Context _context;

        public ProductRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<Product> Products => _context.Products;

        public void SaveProduct(Product product)
        {
            if(product.ID == 0)
            {
                _context.Products.Add(product);
            }
            else
            {
                Product dbEntry = _context.Products.FirstOrDefault(x => x.ID == product.ID);
                if(dbEntry != null)
                {
                    dbEntry.Caption = product.Caption;
                    dbEntry.Barcode = product.Barcode;
                    dbEntry.Price = product.Price;
                }
            }

            _context.SaveChanges();
        }
    }
}
