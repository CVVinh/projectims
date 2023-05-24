using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.GruopDtos;
using BE.Data.Dtos.UserDtos;
using BE.Data.Models;
using BE.Response;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Tls;
using System.Reflection;

namespace BE.Services.GroupServices
{
    public interface IGroupModuleActionService
    {
        Task<BaseResponse<List<Group_Module_Action>>> GetAllGroupModuleAction();
        Task<BaseResponse<List<Group_Module_Action>>> GetGroupModuleActionWithGroupId(int groupId);
        Task<BaseResponse<List<Group_Module_Action>>> GetGroupModuleActionWithModuleId(int moduleId);
        Task<BaseResponse<List<Group_Module_Action>>> GetGroupModuleActionWithActionId(int actionId);
        Task<BaseResponse<List<Group_Module_Action>>> CreateGroupModuleActions(List<AddGroupModuleActionDto> addGroupModuleActionDtos);
        Task<BaseResponse<List<Group_Module_Action>>> updateAction(List<AddGroupModuleActionDto> addGroupModuleActionDtos);
        Task<BaseResponse<Group_Module_Action>> UpdateGroupModuleAction(RequestGroupModuleActionDto request, UpdateGroupModuleActionDto updateGroupModuleActionDto);
        Task<BaseResponse<Group_Module_Action>> DeleteGroupModuleAction(RequestGroupModuleActionDto request);
        Task<BaseResponse<Group_Module_Action>> DeleteSoftGroupModuleAction(RequestGroupModuleActionDto request, DeleteGroupModuleActionDto deleteGroupModuleActionDto);
        Task<BaseResponse<List<Group_Module_Action>>> DeleteMultiGroupModuleActions(List<RequestGroupModuleActionDto> requests);
        Task<BaseResponse<List<Group_Module_Action>>> DeleteMultiSoftMultiGroupModuleActions(List<DeleteMultiGroupModuleActionDto> requests);
    }

    public class GroupModuleActionService : IGroupModuleActionService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public GroupModuleActionService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<Group_Module_Action>>> GetAllGroupModuleAction()
        {
            var success = false;
            var message = "";
            var data = new List<Group_Module_Action>();
            try
            {
                var groupModuleActions = await _db.GroupModuleActions.Where(s => s.isDeleted == false).ToListAsync();

                success = true;
                message = "Get all data successfully";
                return (new BaseResponse<List<Group_Module_Action>>(success, message, groupModuleActions));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Group_Module_Action>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Group_Module_Action>>> GetGroupModuleActionWithGroupId(int groupId)
        {
            var success = false;
            var message = "";
            var data = new List<Group_Module_Action>();
            try
            {
                var groupModuleActions = await _db.GroupModuleActions.Where(s => s.isDeleted == false && s.idGroup.Equals(groupId)).ToListAsync();
                success = true;
                message = "Get data successfully";
                return (new BaseResponse<List<Group_Module_Action>>(success, message, groupModuleActions));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Group_Module_Action>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Group_Module_Action>>> GetGroupModuleActionWithModuleId(int moduleId)
        {
            var success = false;
            var message = "";
            var data = new List<Group_Module_Action>();
            try
            {
                var groupModuleActions = await _db.GroupModuleActions.Where(s => s.isDeleted == false && s.idModule.Equals(moduleId)).ToListAsync();
                success = true;
                message = "Get data successfully";
                return (new BaseResponse<List<Group_Module_Action>>(success, message, groupModuleActions));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Group_Module_Action>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Group_Module_Action>>> GetGroupModuleActionWithActionId(int actionId)
        {
            var success = false;
            var message = "";
            var data = new List<Group_Module_Action>();
            try
            {
                var groupModuleActions = await _db.GroupModuleActions.Where(s => s.isDeleted == false && s.idAction.Equals(actionId)).ToListAsync();
                success = true;
                message = "Get data successfully";
                return (new BaseResponse<List<Group_Module_Action>>(success, message, groupModuleActions));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Group_Module_Action>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Group_Module_Action>>> CreateGroupModuleActions(List<AddGroupModuleActionDto> addGroupModuleActionDtos)
        {
            var success = false;
            var message = "";
            var data = new List<Group_Module_Action>();
            try
            {
                foreach (var item in addGroupModuleActionDtos)
                {
                    var groupModuleAction = await _db.GroupModuleActions.Where(s => s.idGroup.Equals(item.idGroup) && s.idModule.Equals(item.idModule) && s.idAction.Equals(item.idAction)).FirstOrDefaultAsync();
                    if (groupModuleAction == null)
                    {
                        groupModuleAction = _mapper.Map<Group_Module_Action>(item);
                        await _db.GroupModuleActions.AddAsync(groupModuleAction);
                    }
                    else
                    {
                        groupModuleAction = _mapper.Map<AddGroupModuleActionDto, Group_Module_Action>(item, groupModuleAction);
                        _db.GroupModuleActions.Update(groupModuleAction);
                    }
                    groupModuleAction.dateCreated = DateTime.Now;
                    groupModuleAction.isDeleted = false;
                    data.Add(groupModuleAction);
                }
                await _db.SaveChangesAsync();
                success = true;
                message = "Add new Group_Module_Action successfully";
                return new BaseResponse<List<Group_Module_Action>>(success, message, data.Distinct().ToList());
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Adding new Group_Module_Action failed! {ex.Message}";
                return new BaseResponse<List<Group_Module_Action>>(success, message, data);
            }
        }

        public async Task<BaseResponse<Group_Module_Action>> UpdateGroupModuleAction(RequestGroupModuleActionDto request, UpdateGroupModuleActionDto updateGroupModuleActionDto)
        {
            var success = false;
            var message = "";
            var data = new Group_Module_Action();
            try
            {
                var groupModuleAction = await _db.GroupModuleActions.Where(s => s.isDeleted==false && s.idGroup.Equals(request.idGroup) && s.idModule.Equals(request.idModule) && s.idAction.Equals(request.idAction)).FirstOrDefaultAsync();
                if (groupModuleAction is null)
                {
                    message = "Group_Module_Action doesn't exist !";
                    return new BaseResponse<Group_Module_Action>(success, message, data);
                }
                var groupModuleActionMapData = _mapper.Map<UpdateGroupModuleActionDto, Group_Module_Action>(updateGroupModuleActionDto, groupModuleAction);

                _db.GroupModuleActions.Update(groupModuleActionMapData);
                groupModuleActionMapData.dateUpdated = DateTime.Now;
                await _db.SaveChangesAsync();

                success = true;
                message = "Editing Group_Module_Action successfully";
                return new BaseResponse<Group_Module_Action>(success, message, groupModuleActionMapData);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Editing Group_Module_Action failed! {ex.Message}";
                return new BaseResponse<Group_Module_Action>(success, message, data);
            }
        }

        public async Task<BaseResponse<Group_Module_Action>> DeleteGroupModuleAction(RequestGroupModuleActionDto request)
        {
            var success = false;
            var message = "";
            var data = new Group_Module_Action();
            try
            {
                var groupModuleAction = await _db.GroupModuleActions.Where(s => s.isDeleted == false && s.idGroup.Equals(request.idGroup) && s.idModule.Equals(request.idModule) && s.idAction.Equals(request.idAction)).FirstOrDefaultAsync();
                if (groupModuleAction is null)
                {
                    success = false;
                    message = "Group_Module_Action doesn't exist !";
                    return new BaseResponse<Group_Module_Action>(success, message, data);
                }

                _db.GroupModuleActions.Remove(groupModuleAction);
                await _db.SaveChangesAsync();

                success = true;
                message = "Deleting Group_Module_Action successfully";
                return new BaseResponse<Group_Module_Action>(success, message, groupModuleAction);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Group_Module_Action failed! {ex.InnerException}";
                return new BaseResponse<Group_Module_Action>(success, message, data);
            }
        }

        public async Task<BaseResponse<Group_Module_Action>> DeleteSoftGroupModuleAction(RequestGroupModuleActionDto request, DeleteGroupModuleActionDto deleteGroupModuleActionDto)
        {
            var success = false;
            var message = "";
            var data = new Group_Module_Action();
            try
            {
                var groupModuleAction = await _db.GroupModuleActions.Where(s => s.isDeleted == false && s.idGroup.Equals(request.idGroup) && s.idModule.Equals(request.idModule) && s.idAction.Equals(request.idAction)).FirstOrDefaultAsync();
                if (groupModuleAction is null)
                {
                    success = false;
                    message = "Group_Module_Action doesn't exist !";
                    return new BaseResponse<Group_Module_Action>(success, message, data);
                }
                var groupModuleActionMapData = _mapper.Map<DeleteGroupModuleActionDto, Group_Module_Action>(deleteGroupModuleActionDto, groupModuleAction);
                groupModuleActionMapData.dateDeleted = DateTime.Now;
                groupModuleActionMapData.isDeleted = true;
                _db.GroupModuleActions.Update(groupModuleActionMapData);
                await _db.SaveChangesAsync();

                success = true;
                message = "Deleting soft Group_Module_Action successfully";
                return new BaseResponse<Group_Module_Action>(success, message, groupModuleAction);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting soft Group_Module_Action failed! {ex.InnerException}";
                return new BaseResponse<Group_Module_Action>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Group_Module_Action>>> DeleteMultiGroupModuleActions(List<RequestGroupModuleActionDto> requests)
        {
            var success = false;
            var message = "";
            var data = new List<Group_Module_Action>();
            try
            {
                foreach (var item in requests)
                {
                    var groupModuleAction = await _db.GroupModuleActions.Where(s => s.isDeleted == false && s.idGroup.Equals(item.idGroup) && s.idModule.Equals(item.idModule) && s.idAction.Equals(item.idAction)).FirstOrDefaultAsync();
                    if (groupModuleAction != null)
                    {
                        _db.GroupModuleActions.Remove(groupModuleAction);
                        data.Add(groupModuleAction);
                    }
                }
                await _db.SaveChangesAsync();
                success = true;
                message = "Deleting Multi Group_Module_Action successfully";
                return new BaseResponse<List<Group_Module_Action>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Multi Group_Module_Action failed! {ex.InnerException}";
                return new BaseResponse<List<Group_Module_Action>>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Group_Module_Action>>> DeleteMultiSoftMultiGroupModuleActions(List<DeleteMultiGroupModuleActionDto> requests)
        {
            var success = false;
            var message = "";
            var data = new List<Group_Module_Action>();
            try
            {
                foreach (var item in requests)
                {
                    var groupModuleAction = await _db.GroupModuleActions.Where(s => s.isDeleted == false && s.idGroup.Equals(item.idGroup) && s.idModule.Equals(item.idModule) && s.idAction.Equals(item.idAction)).FirstOrDefaultAsync();
                    if (groupModuleAction != null)
                    {
                        groupModuleAction.userDeleted = item.userDeleted;
                        groupModuleAction.dateDeleted = DateTime.Now;
                        groupModuleAction.isDeleted = true;
                        _db.GroupModuleActions.Update(groupModuleAction);
                        data.Add(groupModuleAction);
                    }
                }
                await _db.SaveChangesAsync();
                success = true;
                message = "Deleting Multi Group_Module_Action successfully";
                return new BaseResponse<List<Group_Module_Action>>(success, message, data.Distinct().ToList());
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Multi Group_Module_Action failed! {ex.InnerException}";
                return new BaseResponse<List<Group_Module_Action>>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Group_Module_Action>>> updateAction(List<AddGroupModuleActionDto> addGroupModuleActionDtos)
        {
            var success = false;
            var message = "";
            var data = new List<Group_Module_Action>();
            try
            {
                foreach(var x in addGroupModuleActionDtos)
                {
                    var oldObj = await _db.GroupModuleActions.FirstOrDefaultAsync(c => c.idModule == x.idModule && c.idGroup == x.idGroup && c.idAction == x.idAction);
                    if (oldObj != null)
                    {
                        oldObj.isDeleted = x.isDeleted;
                    }
                    else
                    {
                        if (x.idModule > 0 && x.idGroup > 0 && x.idAction > 0)
                        {
                            
                            var permissionGroup = _mapper.Map<Group_Module_Action>(x);
                            permissionGroup.dateCreated = DateTime.Now;
                            data.Add(permissionGroup);
                        }
                        else
                        {
                            success = false;
                            message = $"Adding new Group_Module_Action failed! ";
                            return new BaseResponse<List<Group_Module_Action>>(success, message, data);
                        }
                    }
                }
                await _db.GroupModuleActions.AddRangeAsync(data);
                await _db.SaveChangesAsync();
                success = true;
                message = "Add new Group_Module_Action successfully";
                return new BaseResponse<List<Group_Module_Action>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Adding new Group_Module_Action failed! {ex.Message}";
                return new BaseResponse<List<Group_Module_Action>>(success, message, data);
            }
        }
    }
}
