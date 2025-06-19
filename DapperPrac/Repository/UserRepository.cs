using Dapper;
using DapperPrac.Data;
using DapperPrac.Models;

namespace DapperPrac.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var query = "SELECT * FROM Users";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<User>(query);
        }

        public async Task<User?> GetUser(int id)
        {
            var query = "SELECT * FROM Users WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
        }

        public async Task CreateUser(User user)
        {
            var query = "INSERT INTO Users (Username, Email) VALUES (@Username, @Email) RETURNING Id;";
            var connection = _context.CreateConnection();
            var id = await connection.ExecuteScalarAsync<int>(query, user);
            user.Id = id;
        }

        public async Task UpdateUser(User user)
        {
            var query = "UPDATE Users SET Username = @Username, Email = @Email WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(query, user);
        }

        public async Task DeleteUser(int id)
        {
            var query = "DELETE FROM Users WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(query, new { Id = id });
        }
    }
}
