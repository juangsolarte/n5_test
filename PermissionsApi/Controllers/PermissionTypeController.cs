using Microsoft.AspNetCore.Mvc;
using PermissionsApi.command;
using PermissionsApi.Models.Dto;

namespace PermissionsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionTypeController(
    IQueryHandler<GetPermissionTypesQuery, IEnumerable<PermissionTypeDto>> getPermissionTypesHandler) : ControllerBase
    {
        private readonly IQueryHandler<GetPermissionTypesQuery, IEnumerable<PermissionTypeDto>> _getPermissionTypesHandler = getPermissionTypesHandler;

        [HttpGet]
        [Route("GetPermissionTypes")]
        public async Task<IActionResult> GetPermissionTypes()
        {
            try
            {
                var query = new GetPermissionTypesQuery();
                var result = await _getPermissionTypesHandler.HandleAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        
    }
}