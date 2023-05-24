using BE.Data.Contexts;
using BE.Data.Dtos.GruopDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.GroupServices;
using BE.Services.PaginationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: groupModuleActions")]
    public class GroupModuleActionController : ControllerBase
    {
        private readonly IGroupModuleActionService _groupModuleActionService;
        private readonly IPaginationServices<Group_Module_Action> _paginationService;
        private readonly AppDbContext _db;

        public GroupModuleActionController(IGroupModuleActionService groupModuleActionService, IPaginationServices<Group_Module_Action> paginationService,AppDbContext db)
        {
            _groupModuleActionService = groupModuleActionService;
            _paginationService = paginationService;
            _db = db;
        }

        [HttpGet("getAllGroupModuleAction")]
        public async Task<IActionResult> GetAllGroupModuleAction(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _groupModuleActionService.GetAllGroupModuleAction();
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationService.paginationListTableAsync(response._Data, pageIndex, pageSize);
                return Ok(resultPage);
            }
            return BadRequest(response);
        }

        [HttpGet("getGroupModuleActionWithGroupId/{groupId}")]
        public async Task<IActionResult> GetGroupModuleActionWithGroupId([FromRoute] int groupId)
        {
            var response = await _groupModuleActionService.GetGroupModuleActionWithGroupId(groupId);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getGroupModuleActionWithModuleId/{moduleId}")]
        public async Task<IActionResult> GetGroupModuleActionWithModuleId([FromRoute] int moduleId)
        {
            var response = await _groupModuleActionService.GetGroupModuleActionWithModuleId(moduleId);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getGroupModuleActionWithActionId/{actionId}")]
        public async Task<IActionResult> GetGroupModuleActionWithActionId([FromRoute] int actionId)
        {
            var response = await _groupModuleActionService.GetGroupModuleActionWithActionId(actionId);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost("createGroupModuleActions")]
        [Authorize(Roles = "module:groupModuleActions action:add")]
        public async Task<IActionResult> CreateGroupModuleActions(List<AddGroupModuleActionDto> addGroupModuleActionDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _groupModuleActionService.CreateGroupModuleActions(addGroupModuleActionDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("updateGroupModuleAction/{idGroup}/{idModule}/{idAction}")]
        [Authorize(Roles = "module:groupModuleActions action:update")]
        public async Task<IActionResult> UpdateGroupModuleAction([FromRoute] RequestGroupModuleActionDto request, [FromBody] UpdateGroupModuleActionDto updateGroupModuleActionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _groupModuleActionService.UpdateGroupModuleAction(request, updateGroupModuleActionDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpDelete("deleteGroupModuleAction")]
        [Authorize(Roles = "module:groupModuleActions action:delete")]
        public async Task<IActionResult> DeleteGroupModuleAction([FromQuery] RequestGroupModuleActionDto request)
        {
            var response = await _groupModuleActionService.DeleteGroupModuleAction(request);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("deleteSoftGroupModuleAction")]
        public async Task<IActionResult> DeleteSoftGroupModuleAction([FromQuery] RequestGroupModuleActionDto request, DeleteGroupModuleActionDto deleteGroupModuleActionDto)
        {
            var response = await _groupModuleActionService.DeleteSoftGroupModuleAction(request, deleteGroupModuleActionDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("deleteMultiGroupModuleActions")]
        public async Task<IActionResult> DeleteMultiGroupModuleActions(List<RequestGroupModuleActionDto> requests)
        {
            var response = await _groupModuleActionService.DeleteMultiGroupModuleActions(requests);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("deleteMultiSoftMultiGroupModuleActions")]
        public async Task<IActionResult> DeleteMultiSoftMultiGroupModuleActions(List<DeleteMultiGroupModuleActionDto> requests)
        {
            var response = await _groupModuleActionService.DeleteMultiSoftMultiGroupModuleActions(requests);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        //Remove all GroupActionModule ( bỏ tất cả thao tác của module khi bỏ access module )
        [HttpPost("removeAllAction/idgroup={id}/idModule={idmodule}")]
        [Authorize(Roles = "module:groupModuleActions action:delete")]
        public IActionResult removeAllAction(int id,int idmodule)
        {
            var list = _db.GroupModuleActions.Where(x => x.idGroup == id && x.idModule == idmodule);
            if(list.Count() > 0)
            {
                foreach(var x in list)
                {
                    x.isDeleted = true;
                }
                _db.SaveChanges();
                return Ok("success");
            }
            else
            {
                return BadRequest("something went wrong !");
            }
        }
        [HttpPost("updateAction")]
        [Authorize(Roles = "module:groupModuleActions action:update")]
        public async Task<IActionResult> addActionAsync(List<AddGroupModuleActionDto> addGroupModuleActionDtos)
        {
            var reponse = await _groupModuleActionService.updateAction(addGroupModuleActionDtos);

            if (reponse._success)
            {
                return Ok(reponse);
            }
            return BadRequest(reponse);
        }
    }
}
