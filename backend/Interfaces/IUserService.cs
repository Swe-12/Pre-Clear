using System.Collections.Generic;
using System.Threading.Tasks;
using PreClear.Api.Models;

namespace PreClear.Api.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> ListAsync(int? skip = null, int? take = null);
        Task<User> GetAsync(long userId);
        Task<User> ChangeRoleAsync(long userId, string newRole);
        Task DeleteAsync(long userId);
    }
}
