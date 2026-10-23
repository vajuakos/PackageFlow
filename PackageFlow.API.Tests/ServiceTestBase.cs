using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PackageFlow.API.Data;

namespace PackageFlow.API.Tests
{
    public abstract class ServiceTestBase : IDisposable
    {
        protected readonly int _userId;
        protected readonly AppDbContext _context;
        private readonly SqliteConnection _connection;

        protected ServiceTestBase()
        {
            _userId = 77;

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new TestDbContext(options);
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _connection.Close();
            _context.Dispose();
        }
    }
}
