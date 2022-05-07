using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using classmaker_models.Config;
using classmaker_models.Dtos;
using classmaker_repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace classmaker_services.Services
{
    /// <summary>
    /// Service for authenticating and handling user tokens
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Check if a given username and password can be authenticated against a user in the db
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Result with a jwt token as its value, or errors</returns>
        Task<Result<string>> Authenticate(string username, string password);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly AppSettings _appSettings;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository,
            IOptions<AppSettings> appSettings,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _appSettings = appSettings.Value;
            _logger = logger;
        }
        
        public async Task<Result<string>> Authenticate(string username, string password)
        {
            var result = new Result<string>();

            var user = await _userRepository.GetUserByUsername(username);

            if (user == null)
            {
                result.AddError("User does not exist");
                return result;
            }

            try
            {
                if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                {
                    result.AddError("Cannot authenticate user");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred: {Message}", ex.Message);
                result.AddError(ex.Message);
            }

            result.Value = GenerateToken(user.UserId);
            
            return result;
        }

        private string GenerateToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.ApiKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return !computedHash.Where((t, i) => t != storedHash[i]).Any();
        }
    }
}