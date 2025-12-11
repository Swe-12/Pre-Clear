using System.Collections.Generic;
using System.Threading.Tasks;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _repo;

        public TagService(ITagRepository repo)
        {
            _repo = repo;
        }

        public async Task<Tag> AddTagAsync(long shipmentId, string name)
        {
            var tag = new Tag
            {
                ShipmentId = shipmentId,
                Name = name
            };
            return await _repo.CreateAsync(tag);
        }

        public async Task RemoveTagAsync(long tagId)
        {
            await _repo.DeleteAsync(tagId);
        }

        public async Task<List<Tag>> GetTagsByShipmentAsync(long shipmentId)
        {
            return await _repo.GetByShipmentAsync(shipmentId);
        }
    }
}
