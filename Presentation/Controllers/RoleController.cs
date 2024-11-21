using Application.Services.Interfaces;
using Data.Repositories.Interfaces;
using Domain.Models.Creates;
using Domain.Models.Filters;
using Domain.Models.Pagination;
using Domain.Models.Updates;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;
[Route("api/roles")]
[ApiController]
public class RoleController : ControllerBase
{
   private readonly IRoleService _roleService;

   public RoleController(IRoleService roleService)
   {
      _roleService = roleService;
   }

   [HttpGet]
   public IActionResult GetRoles( [FromQuery] RoleFilterModel filter, [FromQuery] PaginationRequestModel pagination )
   {
    return _roleService.GetRoles(filter, pagination);
 
   }
   [HttpGet]
   [Route("{id}")]
   public IActionResult GetRoleById( [FromRoute] Guid id )
   {
       return _roleService.GetRoleById(id);
 
   }

   [HttpPost]
   public async Task<IActionResult> CreateRole([FromBody] RoleCreateModel model)
   {
     return await _roleService.CreateRole(model);
   }

   [HttpPut]
   [Route("{id}")]
   public async Task<IActionResult> UpdateRole( [FromRoute] Guid id ,[FromBody] RoleUpdateModel model)
   {
       return await _roleService.UpdateRole(id,model);
   }

   [HttpDelete]
   [Route("{id}")]
   public async Task<IActionResult> DeleteRole([FromRoute] Guid id)
   {
       return await _roleService.DeleteRole(id);
   }
}