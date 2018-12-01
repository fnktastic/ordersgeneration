using OrdersGenerations.DataAccess;
using OrdersGenerations.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersGenerations.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Context _context;

        public OrderRepository()
        {
        }

        public OrderRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<Order> Orders => _context.Orders.Include(x => x.Positions);

        public void SaveOrder(Order order)
        {
            if(order.ID == 0)
            {
                _context.Orders.Add(order);
            }
            else
            {
                var positions = new List<Position>();
                order.Positions.ForEach(x =>
                {
                    positions.Add(new Position()
                    {
                        Dimension = x.Dimension,
                        Product = x.Product,
                        ProductQuantity = x.ProductQuantity,
                        TotalPrice = x.TotalPrice
                    });
                });

                _context.Orders.Add(new Order()
                {
                    Client = order.Client,
                    CreatedDate = order.CreatedDate,
                    Positions = positions
                });
            }

            _context.SaveChanges();
        }
    }
}
