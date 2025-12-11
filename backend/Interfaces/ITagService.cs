using System.Collections.Generic;
using System.Threading.Tasks;
using PreClear.Api.Models;

namespace PreClear.Api.Interfaces
{
    public interface ITagService
    {
        Task<Tag> AddTagAsync(long shipmentId, string name);
        Task RemoveTagAsync(long tagId);
        Task<List<Tag>> GetTagsByShipmentAsync(long shipmentId);
    }
}
