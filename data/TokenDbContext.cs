using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

using dotnet.Models;


namespace dotnet.DataContext
{
    public class TokenDbContext : DbContext
    {
        public TokenDbContext()
        { }
        public TokenDbContext(DbContextOptions<TokenDbContext> options)
            : base(options)
        {
        }

        public DbSet<Token> TokenDb{ get; set; }
         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;ConnectRetryCount=0");
            }
        }
    }
}