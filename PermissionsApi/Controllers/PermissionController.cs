using Microsoft.AspNetCore.Mvc;
using PermissionsApi.command;
using PermissionsApi.Models.Dto;

namespace PermissionsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController(
    ICommandHandler<CreatePermissionCommand> createPermissionHandler,
    ICommandHandler<UpdatePermissionCommand> updatePermissionHandler,
    IQueryHandler<GetPermissionsQuery, IEnumerable<PermissionDto>> getPermissionsHandler,
    IQueryHandler<GetPermissionByIdQuery, PermissionDto> getPermissionByIdHandler) : ControllerBase
    {
        private readonly ICommandHandler<CreatePermissionCommand> _createPermissionHandler = createPermissionHandler;
        private readonly ICommandHandler<UpdatePermissionCommand> _updatePermissionHandler = updatePermissionHandler;
        private readonly IQueryHandler<GetPermissionsQuery, IEnumerable<PermissionDto>> _getPermissionsHandler = getPermissionsHandler;
        private readonly IQueryHandler<GetPermissionByIdQuery, PermissionDto> _getPermissionByIdHandler = getPermissionByIdHandler;

        [HttpGet]
        [Route("GetPermissions")]
        public async Task<IActionResult> GetPermissions()
        {
            try
            {
                var query = new GetPermissionsQuery();
                var result = await _getPermissionsHandler.HandleAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet]
        [Route("GetPermissionById/{id}")]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            try
            {
                var query = new GetPermissionByIdQuery(id);
                var result = await _getPermissionByIdHandler.HandleAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("RequestPermission")]
        public async Task<IActionResult> RequestPermission([FromBody] CreatePermissionDto permissionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var command = new CreatePermissionCommand { PermissionDto = permissionDto };
                await _createPermissionHandler.HandleAsync(command);
                return CreatedAtAction(nameof(GetPermissions), null);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("ModifyPermission/{id}")]
        public async Task<IActionResult> ModifyPermission(int id, CreatePermissionDto permissionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var command = new UpdatePermissionCommand { Id = id, PermissionDto = permissionDto };
                await _updatePermissionHandler.HandleAsync(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}