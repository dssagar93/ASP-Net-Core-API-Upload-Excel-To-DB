using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelToDatabase.Models
{
    public class BrandChannelDbContext : DbContext
    {
        public DbSet<BrandChannel> BrandChannels { get; set; }

        public BrandChannelDbContext(DbContextOptions<BrandChannelDbContext> options) : base(options)
        {
            //this.Database.EnsureCreated();
        }

    }
}
