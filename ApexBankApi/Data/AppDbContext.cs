using ApexBankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ApexBankApi.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}
