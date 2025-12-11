using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly ILogger<UserService> _logger;

        private static readonly string[] ValidRoles = { "admin", "broker", "shipper" };

        public UserService(IUserRepository repo, ILogger<UserService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<List<User>> ListAsync(int? skip = null, int? take = null)
        {
            try
            {
                return await _repo.ListAsync(skip, take);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing users");
                throw;
            }
        }

        public async Task<User> GetAsync(long userId)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId must be positive", nameof(userId));

            var user = await _repo.FindAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User {userId} not found");

            return user;
        }

        public async Task<User> ChangeRoleAsync(long userId, string newRole)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId must be positive", nameof(userId));

            if (string.IsNullOrWhiteSpace(newRole))
                throw new ArgumentException("Role is required", nameof(newRole));

            if (Array.IndexOf(ValidRoles, newRole.ToLowerInvariant()) < 0)
                throw new ArgumentException($"Invalid role. Allowed: {string.Join(", ", ValidRoles)}", nameof(newRole));

            var user = await _repo.FindAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User {userId} not found");

            var oldRole = user.Role;
            user.Role = newRole.ToLowerInvariant();
            user.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(user);
            _logger.LogInformation($"User {userId} role changed from {oldRole} to {newRole}");

            return user;
        }

        public async Task DeleteAsync(long userId)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId must be positive", nameof(userId));

            var user = await _repo.FindAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User {userId} not found");

            await _repo.DeleteAsync(userId);
            _logger.LogInformation($"User {userId} deleted");
        }
    }
}
