using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace WebAPI
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbConnection GetDbConnection()
        {
            return Database.GetDbConnection();
        }

        public async Task OpenConnectionAsync()
        {
            await Database.OpenConnectionAsync();
        }

        public async Task CloseConnectionAsync()
        {
            await Database.CloseConnectionAsync();
        }
    }
}