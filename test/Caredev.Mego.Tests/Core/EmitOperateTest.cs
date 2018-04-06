using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Caredev.Mego.Tests.Models.Simple;

namespace Caredev.Mego.Tests.Core
{
    [TestCategory(Constants.TestCategoryFoundation)]
    [TestClass]
    public class EmitOperateTest : ISimpleTest
    {
        [TestMethod]
        public void EmitCreateSimpleObject()
        {
            using (var db = CreateContext())
            {
                var metadata = db.Configuration.Metadata;
                var type = metadata.Type(typeof(Customer));

                var com = db.Database.Connection.CreateCommand();
                com.CommandText = "SELECT TOP 1 Id ,Code ,Name ,Zip ,Address1 ,Address2 FROM dbo.Customers";
                com.Connection.Open();
                var reader = com.ExecuteReader();
                Assert.IsTrue(reader.Read());
                var obj = type.CreateInstance(reader, new int[] { 0, 1, 2, 3, 4, 5 }, new object[type.ComplexMembers.Count]);
                com.Connection.Close();
                Assert.IsNotNull(obj);
                Assert.IsTrue(obj is Customer);
                var customer = (Customer)obj;
                Assert.IsTrue(customer.Id > 0);
            }
        }

        [TestMethod]
        public void EmitCreateAnonymousObject()
        {
            var readyObject = new
            {
                Id = 12,
                Code = "",
                Name = ""
            };

            using (var db = CreateContext())
            {
                var metadata = db.Configuration.Metadata;
                var type = metadata.Type(readyObject.GetType());

                var com = db.Database.Connection.CreateCommand();
                com.CommandText = "SELECT TOP 1 Id ,Code ,Name FROM dbo.Customers";
                com.Connection.Open();
                var reader = com.ExecuteReader();
                Assert.IsTrue(reader.Read());
                var obj = type.CreateInstance(reader, new int[] { 1, 0, 2 }, new object[type.ComplexMembers.Count]);
                com.Connection.Close();
                Assert.IsNotNull(obj);
                Assert.IsTrue(obj.GetType() == readyObject.GetType());
            }
        }

        [TestMethod]
        public void EmitCreateAnonymousObject2()
        {
            var readyObject = new
            {
                Id = 12,
                ProductId = new Nullable<int>(0),
                ModifyDate = DateTime.Now
            };

            using (var db = CreateContext())
            {
                var metadata = db.Configuration.Metadata;
                var type = metadata.Type(readyObject.GetType());

                var com = db.Database.Connection.CreateCommand();
                com.CommandText = "SELECT TOP 10 a.Id,b.ProductId,a.ModifyDate FROM dbo.Orders a INNER JOIN dbo.OrderDetails b ON b.OrderId = a.Id";
                com.Connection.Open();

                List<object> items = new List<object>();
                var reader = com.ExecuteReader();
                while (reader.Read())
                    items.Add(type.CreateInstance(reader, new int[] { 0, 2, 1 }, new object[type.ComplexMembers.Count]));
                com.Connection.Close();
                Assert.IsTrue(items.Count > 0);
            }
        }

        [TestMethod]
        public void EmitModifyPrimaryProperty()
        {
            using (var db = CreateContext())
            {
                var metadata = db.Configuration.Metadata;
                var type = metadata.Type(typeof(Customer));
                var customer = new Customer();
                var com = db.Database.Connection.CreateCommand();
                com.CommandText = "SELECT TOP 1 Id ,Code ,Name ,Zip ,Address1 ,Address2 FROM dbo.Customers";
                com.Connection.Open();
                var reader = com.ExecuteReader();
                Assert.IsTrue(reader.Read());

                type.ModifyProperty(reader, new int[] { 0, 4, 5, 1, 2, 3 }, customer);

                com.Connection.Close();

                Assert.IsTrue(customer.Id > 0);
            }
        }

        [TestMethod]
        public void EmitSetComplexProperty()
        {
            using (var db = CreateContext())
            {
                var metadata = db.Configuration.Metadata;
                Order order = new Order();
                Customer cus = new Customer();

                var type = metadata.Type(order.GetType());
                var property = order.GetType().GetProperty("Customer");

                type.SetComplexProperty(order, type.ComplexMembers.IndexOf(property), cus);

                Assert.ReferenceEquals(order.Customer, cus);
            }
        }

        [TestMethod]
        public void EmitGetComplexProperty()
        {
            using (var db = CreateContext())
            {
                var metadata = db.Configuration.Metadata;
                Order order = new Order() { Customer = new Customer() };

                var type = metadata.Type(order.GetType());
                var property = order.GetType().GetProperty("Customer");

                var cus = type.GetComplexProperty(order, type.ComplexMembers.IndexOf(property));

                Assert.ReferenceEquals(order.Customer, cus);
            }
        }

        [TestMethod]
        public void EmitGetProperty()
        {
            using (var db = CreateContext())
            {
                var metadata = db.Configuration.Metadata;

                Customer cus = new Customer()
                {
                    Id = 1234,
                    Name = "Name",
                    Address1 = "Address"
                };

                var indexs = new int[] { 0, 4, 1 };
                var values = new object[indexs.Length];
                metadata.Type(cus.GetType()).GetProperty(cus, indexs, values);

                Assert.AreEqual(1234, values[0]);
                Assert.AreEqual("Name", values[1]);
                Assert.AreEqual("Address", values[2]);
            }
        }

        [TestMethod]
        public void EmitSetProperty()
        {
            using (var db = CreateContext())
            {
                var metadata = db.Configuration.Metadata;

                Customer cus = new Customer()
                {
                    Id = 1234,
                    Name = "Name",
                    Address1 = "Address"
                };
                var type = metadata.Type(cus.GetType());

                type.SetProperty(cus, 0, 1000);
                type.SetProperty(cus, 4, "N");
                type.SetProperty(cus, 1, "A");

                Assert.AreEqual(cus.Id, 1000);
                Assert.AreEqual(cus.Name, "N");
                Assert.AreEqual(cus.Address1, "A");
            }
        }

        public OrderManageEntities CreateContext()
        {
            return new OrderManageEntities(Constants.ConnectionNameSimple);
        }
    }
}
