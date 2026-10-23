using Microsoft.EntityFrameworkCore;
using PackageFlow.API.Data;

namespace PackageFlow.API.Tests
{
    public class TestDbContext : AppDbContext
    {
        public TestDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    var defaultSql = property.GetDefaultValueSql();
                    if (defaultSql != null && defaultSql.Contains("GETUTCDATE()"))
                    {
                        property.SetDefaultValueSql("CURRENT_TIMESTAMP");
                    }
                }
            }
        }
    }
}
