using System;
using System.Linq;
using System.Threading.Tasks;
using classmaker_models;
using classmaker_models.Dtos;
using classmaker_models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace classmaker_repositories
{
    /// <summary>
    /// Access to user objects in the db
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Create a new user in the db
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns>Result with success or errors</returns>
        Task<Result> Create(User user, string password);
        
        /// <summary>
        /// Gets a user with a given userId
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User or null</returns>
        User? GetUserById(int id);
        
        /// <summary>
        /// Gets a user with a given username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>User or null</returns>
        Task<User?> GetUserByUsername(string username);

        /// <summary>
        /// Deletes a user with given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Result object with success or error</returns>
        Task<Result> DeleteById(int id);

        /// <summary>
        /// Deletes a user with a given username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Result object with success or error</returns>
        Task<Result> DeleteByUsername(string username);
    }

    public class UserRepository : IUserRepository
    {
        private readonly EntityContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(EntityContext context,
            ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public async Task<Result> Create(User user, string password)
        {
            var result = new Result();
            
            if (_context.Users.Any(x => x.Username == user.Username))
            {
                result.AddError("Username already taken");
                return result;
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred: {Message}", ex.Message);
                result.AddError(ex.Message);
            }
            

            return result;
        }

        public User? GetUserById(int id)
        {
            return _context.Users.Find(id);
        }
        
        public async Task<User?> GetUserByUsername(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.Username == username);
        }

        public async Task<Result> DeleteById(int id)
        {
            var result = new Result();

            var user = GetUserById(id);

            if (user == null)
            {
                result.AddError($"User does not exist with id : {id}");
                return result;
            }

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred: {Message}", ex.Message);
                result.AddError(ex.Message);
            }

            return result;
        }

        public async Task<Result> DeleteByUsername(string username)
        {
            var result = new Result();

            var user = await GetUserByUsername(username);

            if (user == null)
            {
                result.AddError($"User does not exist with id : {username}");
                return result;
            }

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred: {Message}", ex.Message);
                result.AddError(ex.Message);
            }

            return result;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}