using Caredev.Mego.DataAnnotations;
using Caredev.Mego.Resolve.ValueGenerates;
using Caredev.Mego.Tests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Caredev.Mego.Tests.Models.Inherit
{
    public class OrderManageEntities : DbContext
    {
        public OrderManageEntities(string name)
           : base(name)
        { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }

        public void InitialData()
        {
            if (this.Products.Count() == 0)
            {
                Random r = new Random(DateTime.Now.Millisecond);
                Product[] products = new Product[50];
                for (int i = 0; i < products.Length; i++)
                {
                    products[i] = new Product()
                    {
                        Code = "Pro" + i.ToString(),
                        Name = "Product " + i.ToString().PadLeft(4, '0'),
                        Category = r.Next(1, 5),
                        IsValid = ((i / 5) == 0),
                    };
                }
                this.Products.AddRange(a => new Product()
                {
                    Code = a.Code,
                    Name = a.Name,
                    Category = a.Category,
                    IsValid = a.IsValid,
                    UpdateDate = DateTime.Now,
                }, products);

                Customer[] customers = new Customer[100];
                for (int i = 0; i < customers.Length; i++)
                {
                    customers[i] = new Customer()
                    {
                        Id = i + 1,
                        Code = "C" + i.ToString().PadLeft(2, '0'),
                        Name = "Customer " + i.ToString().PadLeft(3, '0'),
                        Zip = r.Next(100, 400).ToString(),
                        Address1 = "Address Master " + r.NextDouble().ToString(),
                        Address2 = "This is data  " + r.NextDouble().ToString(),
                    };
                }
                this.Customers.AddRange(customers);

                List<Warehouse> warehouses = new List<Warehouse>();
                for (int i = 0; i < 30; i++)
                {
                    var count = r.Next(3, 10);
                    for (int j = 0; j < count; j++)
                    {
                        warehouses.Add(new Warehouse()
                        {
                            Id = i,
                            Number = j,
                            Name = "Warehouse" + i.ToString().PadLeft(3, '0') + j.ToString().PadLeft(4, '0'),
                            Address = "Address Master " + r.NextDouble().ToString(),
                        });
                    }
                }
                this.Warehouses.AddRange(warehouses);

                Order[] orders = new Order[40];
                for (int i = 0; i < orders.Length; i++)
                {
                    orders[i] = new Order()
                    {
                        Id = i + 1,
                        ModifyDate = DateTime.Now.AddDays(r.Next(-365, 365)),
                        State = r.Next(1, 10)
                    };
                    this.Orders.Add(orders[i]);
                    this.Orders.AddRelation(orders[i], a => a.Customer, customers[r.Next(0, customers.Length - 1)]);

                    var count = r.Next(3, 10);
                    for (int j = 0; j < count; j++)
                    {
                        var detail = new OrderDetail()
                        {
                            Quantity = r.Next(100, 500),
                            Discount = r.Next(1, 100),
                            Price = Convert.ToDecimal(r.NextDouble() * 100)
                        };
                        this.OrderDetails.Add(detail);
                        this.Orders.AddRelation(orders[i], a => a.Details, detail);
                        this.OrderDetails.AddRelation(detail, a => a.Product, products[r.Next(0, products.Length - 1)]);
                    }
                }
                this.Executor.Execute();
            }
        }

        public void InitialTable()
        {
            var manager = this.Database.Manager;
            if (!Database.SqlQuery<bool>(manager.TableIsExsit<Product>().GenerateSql()).First())
            {
                var list = new List<Resolve.Operates.DbOperateBase>()
                {
                    manager.CreateTable<OrderBase>(),
                    manager.CreateTable<Order>(),
                    manager.CreateTable<OrderDetailBase>(),
                    manager.CreateTable<OrderDetail>(),
                    manager.CreateTable<CustomerBase>(),
                    manager.CreateTable<Customer>(),
                    manager.CreateTable<ProductBase>(),
                    manager.CreateTable<Product>(),
                    manager.CreateTable<WarehouseBase>(),
                    manager.CreateTable<Warehouse>(),
                };
                if (this.Configuration.DatabaseFeature.HasCapable(Resolve.EDbCapable.Relation))
                {
                    list.Add(manager.CreateRelation<Order>());
                    list.Add(manager.CreateRelation<OrderDetail>());
                    list.Add(manager.CreateRelation<Customer>());
                    list.Add(manager.CreateRelation<Product>());
                    list.Add(manager.CreateRelation<Warehouse>());
                    list.AddRange(manager.CreateRelation((Order o) => o.Customer));
                    list.AddRange(manager.CreateRelation((Order o) => o.Details));
                    list.AddRange(manager.CreateRelation((OrderDetail o) => o.Product));
                }
                this.Executor.Execute(list);
            }
        }
    }

    [Table("OrderBases")]
    public class OrderBase
    {
        [Key]
        public int Id { get; set; }

        [GeneratedDateTime(EGeneratedPurpose.Insert)]
        public DateTime CreateDate { get; set; }

        [GeneratedDateTime(EGeneratedPurpose.Update)]
        public DateTime ModifyDate { get; set; }
    }

    [Table("Orders", true)]
    public class Order : OrderBase
    {
        public int CustomerId { get; set; }

        public int State { get; set; }

        [ForeignKey("CustomerId", "Id")]
        public virtual Customer Customer { get; set; }

        [InverseProperty("OrderId", "Id")]
        public virtual ICollection<OrderDetail> Details { get; set; }

        [Relationship(typeof(OrderDetail), "OrderId", "Id", "ProductId", "Id")]
        public virtual ICollection<Product> Products { get; set; }
    }

    [Table("OrderDetailBases")]
    public class OrderDetailBase
    {
        [Key, Identity]
        public int Id { get; set; }

        [GeneratedGuid]
        public Guid Key { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }

    [Table("OrderDetails", true)]
    public class OrderDetail : OrderDetailBase
    {
        public int OrderId { get; set; }

        public int? ProductId { get; set; }
        [ConcurrencyCheck]
        public int Discount { get; set; }

        [ForeignKey("OrderId", "Id")]
        public virtual Order Order { get; set; }

        [ForeignKey("ProductId", "Id")]
        public virtual Product Product { get; set; }
    }

    [Table("CustomerBases")]
    public class CustomerBase
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }

    [Table("Customers", true)]
    public class Customer : CustomerBase
    {
        [ConcurrencyCheck]
        public string Zip { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        [InverseProperty("CustomerId", "Id")]
        public virtual ICollection<Order> Orders { get; set; }
    }

    [Table("ProductBases")]
    public class ProductBase
    {
        [Key, Identity]
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }

    [Table("Products", true)]
    public class Product : ProductBase
    {
        public int Category { get; set; }

        public bool IsValid { get; set; }

        [GeneratedDateTime(EGeneratedPurpose.InsertUpdate)]
        public DateTime UpdateDate { get; set; }

        [InverseProperty("OrderId", "Id")]
        public virtual ICollection<Order> Orders { get; set; }
    }

    [Table("WarehouseBases")]
    public class WarehouseBase
    {
        [Key, Column(nameof(Id), Order = 1)]
        public int Id { get; set; }

        [Key, Column(nameof(Number), Order = 2)]
        public int Number { get; set; }

        public string Name { get; set; }
    }

    [Table("Warehouses", true)]
    public class Warehouse : WarehouseBase
    {
        [ConcurrencyCheck]
        public string Address { get; set; }
    }
}
