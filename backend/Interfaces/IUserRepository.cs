using System.Collections.Generic;
using System.Threading.Tasks;
using PreClear.Api.Models;

namespace PreClear.Api.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> ListAsync(int? skip = null, int? take = null);
        Task<User> FindAsync(long userId);
        Task UpdateAsync(User user);
        Task DeleteAsync(long userId);
        Task SaveAsync();
    }
}
