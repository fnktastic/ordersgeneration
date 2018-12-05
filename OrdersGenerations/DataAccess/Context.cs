using OrdersGenerations.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersGenerations.DataAccess
{
    public class Context : DbContext
    {
        public Context() : base("orders")
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Dimension> Dimensions { get; set; }
    }

    public class Initializer : DropCreateDatabaseIfModelChanges<Context>
    {
        public Initializer()
        {
            using (var context = new Context())
            {
                InitializeDatabase(context);
            }
        }

        protected override void Seed(Context context)
        {
            context.Dimensions.Add(new Dimension() { Caption = "шт." });
            context.Dimensions.Add(new Dimension() { Caption = "к-т." });

            context.SaveChanges();

            Order order1 = new Order()
            {
                CreatedDate = DateTime.Now,
                Client = new Client()
                {
                    FirstName = "Петро",
                    SurnameName = "Олексійович",
                    LastName = "Порошенко",
                    Address = "вул. Банкова, 1",
                    Description = "Президент України"
                },
                Positions = new List<Position>()
                {
                    new Position()
                    {
                        DimensionID = 1,
                        ProductQuantity = 10,
                        TotalPrice = 100,
                        Product = new Product()
                        {
                            Barcode = "8699947702669",
                            Caption = "Трос металевий",
                            Price = 10
                        }
                    },
                    new Position()
                    {
                        DimensionID = 2,
                        ProductQuantity = 60,
                        TotalPrice = 120,
                        Product = new Product()
                        {
                            Barcode = "00002",
                            Caption = "Сірники",
                            Price = 2
                        }
                    }
                }

            };
            Order order2 = new Order()
            {
                CreatedDate = DateTime.Now.AddDays(-54),
                Client = new Client()
                {
                    FirstName = "Володимир",
                    SurnameName = "Володимирович",
                    LastName = "Путін",
                    Address = "вул. Леніна, 1",
                    Description = "Президент Росії"
                },
                Positions = new List<Position>()
                {
                    new Position()
                    {
                        DimensionID = 2,
                        ProductQuantity = 10,
                        TotalPrice = 100,
                        Product = new Product()
                        {
                            Barcode = "00003",
                            Caption = "Ква свіжий",
                            Price = 10
                        }
                    },
                    new Position()
                    {
                        DimensionID = 2,
                        ProductQuantity = 60,
                        TotalPrice = 120,
                        Product = new Product()
                        {
                            Barcode = "00004",
                            Caption = "Пиво темне",
                            Price = 2
                        }
                    },
                    new Position()
                    {
                        DimensionID = 2,
                        ProductQuantity = 160,
                        TotalPrice = 1250,
                        Product = new Product()
                        {
                            Barcode = "00005",
                            Caption = "Гнль для душу",
                            Price = 25
                        }
                    }
                }

            };
            context.Orders.Add(order1);
            context.Orders.Add(order2);
            context.SaveChanges();
        }
    }
}
