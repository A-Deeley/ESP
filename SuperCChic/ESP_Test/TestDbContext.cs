using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESP_Test
{
    public partial class TestDbContext : A22Sda1532463Context
    {
        public TestDbContext()
            :base()
        {

        }

        public TestDbContext(DbContextOptions<A22Sda1532463Context> options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseLazyLoadingProxies()
            .UseInMemoryDatabase("TestDb");
    }
}
