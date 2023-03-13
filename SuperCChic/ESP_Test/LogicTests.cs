using Backend;
using Backend.Models;
using Backend.TransactionBuilder;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESP_Test
{
    [TestClass]
    public class LogicTests
    {
        private TestDbContext _testContext;
        private Product _testProduct;
        private Department _testDepart;
        private Company _testCompany;

        [TestInitialize]
        public void Init()
        {
            _testContext = new();
            try
            {
                _testContext.Database.EnsureCreated();

            }catch(SqliteException sex)
            {
                string exMsg = sex.Message;
            }

            _testDepart = new Department()
            {
                Name = "TestDept"
            };
            _testCompany = new Company()
            {
                Name = "TestCompany"
            };
            _testProduct = new Product()
            {
                Name = "test",
                Qty = 2,
                Cup = "123456789123",
                Price = 12.99f,
                UnitType = "TestType",
                ApplyTps = 1u,
                ApplyTvq = 1u,
                DiscountType = 0,
                Company = _testCompany,
                Department = _testDepart
            };

            _testContext.Products.Add(_testProduct);
            _testContext.SaveChanges();

            if (!Directory.Exists(@"./Factures"))
                Directory.CreateDirectory(@"./Factures");
        }


        [TestMethod]
        public void Printing()
        {
            using (var db = new A22Sda1532463Context())
            {
                var transacts = db.Transactions.ToList();
                if (transacts.Count() == 0)
                    throw new AssertFailedException();
                if (transacts[0] == null)
                    throw new AssertFailedException();

                Printer p = new(false);
                p.Print(transacts[0].Id);
                bool fileCreated = File.Exists(@$"./Factures/{transacts[0].Id}.txt");
                fileCreated.Should().BeTrue();
            }
        }

        [TestMethod]
        public void AddTransaction()
        {
            var transactionBuilder = TransactionBuilder.StartTransaction(_testContext);
            var addedRow = transactionBuilder.AddProduct(_testProduct.Cup, 1);

            addedRow.ProductId.Should().Be(_testProduct.Id);
        }




        [TestCleanup] 
        public void Cleanup()
        {
            if (Directory.Exists(@"./Factures"))
                Directory.Delete(@"./Factures", true);

            _testContext.Database.EnsureDeleted();
        }
    }
}
