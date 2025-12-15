using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PreClear.Api.Interfaces;
using PreClear.Api.Models;

namespace PreClear.Api.Controllers
{
    [ApiController]
    [Route("api/shipments/{shipmentId:long}/tags")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpPost]
        public async Task<IActionResult> AddTag(long shipmentId, [FromBody] TagRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Name))
                return BadRequest("Name is required");

            var tag = await _tagService.AddTagAsync(shipmentId, request.Name.Trim());
            return CreatedAtAction(null, new { id = tag.Id }, tag);
        }

        [HttpDelete("{tagId:long}")]
        public async Task<IActionResult> DeleteTag(long shipmentId, long tagId)
        {
            // Service will delete by id; controller ensures route contains shipmentId for context
            await _tagService.RemoveTagAsync(tagId);
            return NoContent();
        }

        public class TagRequest
        {
            public string Name { get; set; } = null!;
        }
    }
}
