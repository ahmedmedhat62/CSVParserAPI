using InfoTecs_API.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace InfoTecs_API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<CSV_File> files { get; set; }

        public DbSet<value> values { get; set; }

        public DbSet<Result> results { get; set; }
    }
}
