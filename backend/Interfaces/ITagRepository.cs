using System.Collections.Generic;
using System.Threading.Tasks;
using PreClear.Api.Models;

namespace PreClear.Api.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag> CreateAsync(Tag tag);
        Task DeleteAsync(long tagId);
        Task<List<Tag>> GetByShipmentAsync(long shipmentId);
    }
}
