using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.GruopDtos;
using BE.Data.Dtos.ModuleDtos;
using BE.Data.Dtos.Permission_Use_Menus;
using BE.Data.Dtos.UserDtos;
using BE.Data.Models;
using BE.Response;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System.Reflection;

namespace BE.Services.GroupServices
{
    public interface IPermissionGroupServices
    {
        Task<BaseResponse<List<Permission_Group>>> GetAllPermissionGroupAsync();
        Task<BaseResponse<List<Permission_Group>>> GetPermissionGroupWithModuleId(int moduleId);
        Task<BaseResponse<List<Permission_Group>>> GetPermissionGroupWithGroupId(int groupId);
        Task<BaseResponse<List<Permission_Group>>> GetPermissionGroupWithGroupIdAcess(int groupId);
        Task<BaseResponse<List<Permission_Group>>> CreatePermissionGroup(List<PermissionGroupDto> permissionGroupDtos);
        Task<BaseResponse<Permission_Group>> UpdatePermissionGroup(int idGroup, int idModule, PermissionGroupDto permissionGroupDtos);
        Task<BaseResponse<List<Permission_Group>>> UpdateMultiPermissionGroup(int idGroup, List<ChangePermissionGroupDto> changePermissionGroupDtos);
        Task<BaseResponse<Permission_Group>> PermissionGroupAccessRefuse(int idGroup, int idModule, int idUserUpdated);
        Task<BaseResponse<List<Permission_Group>>> PermissionGroupAccessRefuseMulti(int idUserUpdateted, List<PermissionGroupRequestDto> permissionGroupRequestDtos);
        Task<BaseResponse<Permission_Group>> UpdatePermissionGroupAccessRefuse(int idGroup, int idModule, int idUserUpdated, List<int> listIdActions);
        Task<BaseResponse<List<Permission_Group>>> UpdatePermissionGroupAccessRefuseMulti(List<PermissionGroupAccessAllowMultiDto> permissionGroupAccessAllowMultiDtos);
        Task<BaseResponse<Permission_Group>> PermissionGroupAccessAllow(int idGroup, int idModule, int idUserUpdateted, List<int> listIdAction);
        Task<BaseResponse<List<Permission_Group>>> PermissionGroupAccessAllowMulti(List<PermissionGroupAccessAllowMultiDto> permissionGroupAccessAllowMultiDtos);
        Task<BaseResponse<Permission_Group>> DeletePermissionGroup(int idGroup, int idModule);
        Task<BaseResponse<List<Permission_Group>>> DeleteMultiPermissionGroup(List<PermissionGroupRequestDto> permissionGroupRequestDto);
        // Phân quyền access Group_Module
        Task<BaseResponse<List<Permission_Group>>> UpdatePermissionGroupModule(List<PermissionGroupDto> permissionGroupDtos);
    }

    public class PermissionGroupServices : IPermissionGroupServices
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public PermissionGroupServices(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<Permission_Group>>> GetAllPermissionGroupAsync()
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Group>();
            try
            {
                var permissionGroup = await _db.Permission_Groups.Where(s => s.IsDeleted == false).ToListAsync();

                success = true;
                message = "Get all data successfully";
                data.AddRange(permissionGroup);
                return (new BaseResponse<List<Permission_Group>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Group>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Group>>> GetPermissionGroupWithGroupId(int groupId)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Group>();
            try
            {
                var permissionGroup = await _db.Permission_Groups.Where(s => s.IsDeleted == false && s.IdGroup.Equals(groupId)).ToListAsync();
                success = true;
                message = "Get data successfully";
                data.AddRange(permissionGroup);
                return (new BaseResponse<List<Permission_Group>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Group>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Group>>> GetPermissionGroupWithModuleId(int moduleId)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Group>();
            try
            {
                var permissionGroup = await _db.Permission_Groups.Where(s => s.IsDeleted == false && s.IdModule.Equals(moduleId)).ToListAsync();
                success = true;
                message = "Get data successfully";
                data.AddRange(permissionGroup);
                return (new BaseResponse<List<Permission_Group>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Group>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Group>>> GetPermissionGroupWithGroupIdAcess(int groupId)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Group>();
            try
            {
                var permissionGroup = await _db.Permission_Groups.Where(s => s.IsDeleted == false && s.IdGroup.Equals(groupId) && s.Access == true).ToListAsync();
                success = true;
                message = "Get data successfully";
                return (new BaseResponse<List<Permission_Group>>(success, message, permissionGroup));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Group>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Group>>> CreatePermissionGroup(List<PermissionGroupDto> permissionGroupDtos)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Group>();
            try
            {
                foreach (var item in permissionGroupDtos)
                {
                    var permissionGroup = await _db.Permission_Groups.Where(s => s.IdGroup.Equals(item.IdGroup) && s.IdModule.Equals(item.IdModule)).FirstOrDefaultAsync();
                    if(permissionGroup == null)
                    {
                        permissionGroup = _mapper.Map<Permission_Group>(item);
                        await _db.Permission_Groups.AddAsync(permissionGroup);
                    }
                    else
                    {
                        permissionGroup = _mapper.Map<PermissionGroupDto, Permission_Group>(item, permissionGroup);
                        _db.Permission_Groups.Update(permissionGroup);
                    }
                    permissionGroup.IsDeleted = false;
                    data.Add(permissionGroup);
                }
                await _db.SaveChangesAsync();
                success = true;
                message = "Add new Permission_Group successfully";
                return new BaseResponse<List<Permission_Group>>(success, message, data.Distinct().ToList());
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Adding new Permission_Group failed! {ex.Message}";
                return new BaseResponse<List<Permission_Group>>(success, message, data);
            }
        }

        public async Task<BaseResponse<Permission_Group>> UpdatePermissionGroup(int idGroup, int idModule, PermissionGroupDto permissionGroupDtos)
        {
            var success = false;
            var message = "";
            var data = new Permission_Group();
            try
            {
                var permissionGroup = await _db.Permission_Groups.Where(s => s.IsDeleted == false && s.IdGroup.Equals(idGroup) && s.IdModule.Equals(idModule)).FirstOrDefaultAsync();
                if (permissionGroup is null)
                {
                    message = "Permission_Group doesn't exist !";
                    data = null;
                    return new BaseResponse<Permission_Group>(success, message, data);
                }
                var permissionGroupMapData = _mapper.Map<PermissionGroupDto, Permission_Group>(permissionGroupDtos, permissionGroup);

                _db.Permission_Groups.Update(permissionGroupMapData);
                await _db.SaveChangesAsync();

                success = true;
                message = "Editing Permission_Group successfully";
                data = permissionGroupMapData;
                return new BaseResponse<Permission_Group>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Editing Permission_Group failed! {ex.Message}";
                return new BaseResponse<Permission_Group>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Permission_Group>>> UpdateMultiPermissionGroup(int idGroup, List<ChangePermissionGroupDto> changePermissionGroupDtos)
        {
            var success = false;       
            var message = "";          
            var data = new List<Permission_Group>();
            try
            {
                foreach (var item in changePermissionGroupDtos)
                {
                    var permissionGroup = await _db.Permission_Groups.Where(s => s.IsDeleted == false && s.IdGroup.Equals(idGroup) && s.IdModule.Equals(item.IdModule)).FirstOrDefaultAsync();
                    if (permissionGroup is null)
                    {
                        permissionGroup = _mapper.Map<Permission_Group>(item);
                        permissionGroup.IdGroup = idGroup;
                        await _db.Permission_Groups.AddAsync(permissionGroup);
                    }
                    else
                    {
                        permissionGroup = _mapper.Map<ChangePermissionGroupDto, Permission_Group>(item, permissionGroup);
                        _db.Permission_Groups.Update(permissionGroup);
                    }
                    data.Add(permissionGroup);
                }
                await _db.SaveChangesAsync();
                success = true;
                message = "Updating Multi Permission_Groups successfully";
                return new BaseResponse<List<Permission_Group>>(success, message, data.Distinct().ToList());
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Updating Multi Permission_Groups failed! {ex.InnerException}";
                return new BaseResponse<List<Permission_Group>>(success, message, data);
            }
        }

        public async Task<BaseResponse<Permission_Group>> UpdatePermissionGroupAccessRefuse(int idGroup, int idModule, int idUserUpdated, List<int> listIdActions)
        {
            var success = false;
            var message = "";
            var data = new Permission_Group();
            try
            {
                var permissionGroup = await _db.Permission_Groups.Where(s => s.IsDeleted == false && s.Access == true && s.IdModule.Equals(idModule) && s.IdGroup.Equals(idGroup)).FirstOrDefaultAsync();
                if (permissionGroup != null)
                {
                    data = permissionGroup;
                    
                    var getGroupModuleActions = await _db.GroupModuleActions.Where(s => s.idGroup.Equals(idGroup) && s.idModule.Equals(idModule)).ToListAsync();

                    var getGroupModuleActionCurrent = getGroupModuleActions.Where(s => s.isDeleted == false ).Select(s => s.idAction).ToList();

                    var getGroupModuleActionHaveId = getGroupModuleActions.Where(s => s.isDeleted == false && listIdActions.Contains(s.idAction)).Select(s => s.idAction).ToList();

                    var getGroupModuleActionHaveNotId = listIdActions.Where(s => !getGroupModuleActions.Select(x => x.idAction).ToList().Contains(s)).ToList();

                    var getGroupModuleActionNeedAdd = getGroupModuleActions.Where(s => listIdActions.Contains(s.idAction) && !getGroupModuleActionHaveId.Contains(s.idAction)).ToList();

                    var getGroupModuleActionNeedDelete = getGroupModuleActions.Where(s => getGroupModuleActionCurrent.Contains(s.idAction) && !listIdActions.Contains(s.idAction)).ToList();

                    foreach(var addId in getGroupModuleActionHaveNotId)
                    {
                        var getByIdGroupModuleAction = await _db.GroupModuleActions.Where(s => s.idGroup.Equals(idGroup) && s.idModule.Equals(idModule) && s.idAction.Equals(addId)).FirstOrDefaultAsync();
                        if (getByIdGroupModuleAction == null)
                        {
                            getByIdGroupModuleAction = new Group_Module_Action();
                            getByIdGroupModuleAction.idGroup = idGroup;
                            getByIdGroupModuleAction.idModule = idModule;
                            getByIdGroupModuleAction.idAction = addId;
                            getByIdGroupModuleAction.userCreated = idUserUpdated;
                            getByIdGroupModuleAction.dateCreated = DateTime.Now;
                            getByIdGroupModuleAction.isDeleted = false;
                            await _db.GroupModuleActions.AddAsync(getByIdGroupModuleAction);
                            getGroupModuleActionNeedAdd.Add(getByIdGroupModuleAction);
                        }
                    }
                    await _db.SaveChangesAsync();
                       
                    // bo cac action khong chon
                    foreach (var action in getGroupModuleActionNeedDelete)
                    {
                        action.isDeleted = true;
                        _db.GroupModuleActions.Update(action);
                    }

                    var listUserGroups = await _db.UserGroups.Where(s => s.isDeleted == false && s.idGroup.Equals(idGroup)).ToListAsync();
                    if (listUserGroups.Count() > 0)
                    {
                        // bo quyen
                        foreach (var item in listUserGroups)
                        {
                            var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.isDeleted == false && s.IdUser.Equals(item.idUser) && s.idModule.Equals(idModule)).FirstOrDefaultAsync();
                            if (permissionUserMenu != null)
                            {
                                foreach (var gma in getGroupModuleActionNeedDelete)
                                {
                                    if (gma.idAction == 1)
                                    {
                                        permissionUserMenu.Add = --permissionUserMenu.Add <= 0 ? 0 : permissionUserMenu.Add;
                                    }
                                    else if (gma.idAction == 2)
                                    {
                                        permissionUserMenu.Update = --permissionUserMenu.Update <= 0 ? 0 : permissionUserMenu.Update;
                                    }
                                    else if (gma.idAction == 3)
                                    {
                                        permissionUserMenu.Delete = --permissionUserMenu.Delete <= 0 ? 0 : permissionUserMenu.Delete;
                                    }
                                    else if (gma.idAction == 4)
                                    {
                                        permissionUserMenu.DeleteMulti = --permissionUserMenu.DeleteMulti <= 0 ? 0 : permissionUserMenu.DeleteMulti;
                                    }
                                    else if (gma.idAction == 5)
                                    {
                                        permissionUserMenu.Confirm = --permissionUserMenu.Confirm <= 0 ? 0 : permissionUserMenu.Confirm;
                                    }
                                    else if (gma.idAction == 6)
                                    {
                                        permissionUserMenu.ConfirmMulti = --permissionUserMenu.ConfirmMulti <= 0 ? 0 : permissionUserMenu.ConfirmMulti;
                                    }
                                    else if (gma.idAction == 7)
                                    {
                                        permissionUserMenu.Refuse = --permissionUserMenu.Refuse <= 0 ? 0 : permissionUserMenu.Refuse;
                                    }
                                    else if (gma.idAction == 8)
                                    {
                                        permissionUserMenu.AddMember = --permissionUserMenu.AddMember <= 0 ? 0 : permissionUserMenu.AddMember;
                                    }
                                    else if (gma.idAction == 9)
                                    {
                                        permissionUserMenu.Export = --permissionUserMenu.Export <= 0 ? 0 : permissionUserMenu.Export;
                                    }
                                }
                                permissionUserMenu.userModified = idUserUpdated;
                                permissionUserMenu.dateModified = DateTime.Now;
                                _db.Permission_Use_Menus.Update(permissionUserMenu);
                            }
                        }
                        await _db.SaveChangesAsync();

                        // them quyen moi
                        // them cac action duoc chon
                        foreach (var action in getGroupModuleActionNeedAdd)
                        {
                            action.isDeleted = false;
                            _db.GroupModuleActions.Update(action);
                        }
                        foreach (var item in listUserGroups)
                        {
                            // them quyen moi
                            var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.IdUser.Equals(item.idUser) && s.idModule.Equals(idModule)).FirstOrDefaultAsync();
                            if (permissionUserMenu == null)
                            {
                                permissionUserMenu = new Permission_Use_Menu();
                                permissionUserMenu.IdUser = item.idUser;
                                permissionUserMenu.idModule = idModule;
                                permissionUserMenu.userCreated = idUserUpdated;
                                permissionUserMenu.dateCreated = DateTime.Now;
                                await _db.Permission_Use_Menus.AddAsync(permissionUserMenu);
                            }
                            else
                            {
                                permissionUserMenu.userModified = idUserUpdated;
                                permissionUserMenu.dateModified = DateTime.Now;
                                _db.Permission_Use_Menus.Update(permissionUserMenu);
                            }
                            permissionUserMenu.isDeleted = false;
                            foreach (var gma in getGroupModuleActionNeedAdd)
                            {
                                if (gma.idAction == 1)
                                {
                                    permissionUserMenu.Add++;
                                }
                                else if (gma.idAction == 2)
                                {
                                    permissionUserMenu.Update++ ;
                                }
                                else if (gma.idAction == 3)
                                {
                                    permissionUserMenu.Delete++ ;
                                }
                                else if (gma.idAction == 4)
                                {
                                    permissionUserMenu.DeleteMulti++ ;
                                }
                                else if (gma.idAction == 5)
                                {
                                    permissionUserMenu.Confirm++ ;
                                }
                                else if (gma.idAction == 6)
                                {
                                    permissionUserMenu.ConfirmMulti++ ;
                                }
                                else if (gma.idAction == 7)
                                {
                                    permissionUserMenu.Refuse++ ;
                                }
                                else if (gma.idAction == 8)
                                {
                                    permissionUserMenu.AddMember++ ;
                                }
                                else if (gma.idAction == 9)
                                {
                                    permissionUserMenu.Export++ ;
                                }
                            }
                        }
                    }
                    await _db.SaveChangesAsync();
                }
                success = true;
                message = "PermissionGroup access refuse with list IdAction successfully";
                return new BaseResponse<Permission_Group>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"PermissionGroup access refuse with list IdAction failed! {ex.InnerException}";
                return new BaseResponse<Permission_Group>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Permission_Group>>> UpdatePermissionGroupAccessRefuseMulti(List<PermissionGroupAccessAllowMultiDto> permissionGroupAccessAllowMultiDtos)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Group>();
            try
            {
                foreach (var item in permissionGroupAccessAllowMultiDtos)
                {
                    var result = await UpdatePermissionGroupAccessRefuse(item.idGroup, item.idModule, item.idUserUpdateted, item.listIdAction);
                    if (result._success)
                    {
                        data.Add(result._Data);
                    }
                }
                success = true;
                message = "PermissionGroupMulti access refuse with list IdAction successfully";
                return new BaseResponse<List<Permission_Group>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"PermissionGroupMulti access refuse with list IdAction failed! {ex.InnerException}";
                return new BaseResponse<List<Permission_Group>>(success, message, data);
            }
        }

        public async Task<BaseResponse<Permission_Group>> PermissionGroupAccessRefuse(int idGroup, int idModule, int idUserUpdated)
        {
            var success = false;
            var message = "";
            var data = new Permission_Group();
            try
            {
                var permissionGroup = await _db.Permission_Groups.Where(s => s.IsDeleted == false && s.Access == true && s.IdModule.Equals(idModule) && s.IdGroup.Equals(idGroup)).FirstOrDefaultAsync();
                if (permissionGroup != null)
                {
                    permissionGroup.Access = false;
                    _db.Permission_Groups.Update(permissionGroup);
                    data = permissionGroup;

                    var groupModuleActions = await _db.GroupModuleActions.Where(s => s.isDeleted == false && s.idGroup.Equals(idGroup) && s.idModule.Equals(idModule)).ToListAsync();
                    if (groupModuleActions.Count() > 0)
                    {
                        var listUserGroups = await _db.UserGroups.Where(s => s.isDeleted == false && s.idGroup.Equals(idGroup)).ToListAsync();
                        foreach (var item in listUserGroups)
                        {
                            var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.isDeleted == false && s.IdUser.Equals(item.idUser) && s.idModule.Equals(idModule)).FirstOrDefaultAsync();
                            if(permissionUserMenu != null)
                            {
                                bool checkDelete = false;
                                foreach (var gma in groupModuleActions)
                                {
                                    if (gma.idAction == 1)
                                    {
                                        --permissionUserMenu.Add;
                                        if (permissionUserMenu.Add <= 0)
                                        {
                                            permissionUserMenu.Add = 0;
                                            checkDelete = true;
                                        }
                                        else
                                        {
                                            checkDelete = false;
                                        }
                                    }
                                    else if (gma.idAction == 2)
                                    {
                                        --permissionUserMenu.Update;
                                        if (permissionUserMenu.Update <= 0)
                                        {
                                            permissionUserMenu.Update = 0;
                                            checkDelete = true;
                                        }
                                        else
                                        {
                                            checkDelete = false;
                                        }
                                    }
                                    else if (gma.idAction == 3)
                                    {
                                        --permissionUserMenu.Delete;
                                        if (permissionUserMenu.Delete <= 0)
                                        {
                                            permissionUserMenu.Delete = 0;
                                            checkDelete = true;
                                        }
                                        else
                                        {
                                            checkDelete = false;
                                        }
                                    }
                                    else if (gma.idAction == 4)
                                    {
                                        --permissionUserMenu.DeleteMulti;
                                        if (permissionUserMenu.DeleteMulti <= 0)
                                        {
                                            permissionUserMenu.DeleteMulti = 0;
                                            checkDelete = true;
                                        }
                                        else
                                        {
                                            checkDelete = false;
                                        }
                                    }
                                    else if (gma.idAction == 5)
                                    {
                                        --permissionUserMenu.Confirm;
                                        if (permissionUserMenu.Confirm <= 0)
                                        {
                                            permissionUserMenu.Confirm = 0;
                                            checkDelete = true;
                                        }
                                        else
                                        {
                                            checkDelete = false;
                                        }
                                    }
                                    else if (gma.idAction == 6)
                                    {
                                        --permissionUserMenu.ConfirmMulti;
                                        if (permissionUserMenu.ConfirmMulti <= 0)
                                        {
                                            permissionUserMenu.ConfirmMulti = 0;
                                            checkDelete = true;
                                        }
                                        else
                                        {
                                            checkDelete = false;
                                        }
                                    }
                                    else if (gma.idAction == 7)
                                    {
                                        --permissionUserMenu.Refuse;
                                        if (permissionUserMenu.Refuse <= 0)
                                        {
                                            permissionUserMenu.Refuse = 0;
                                            checkDelete = true;
                                        }
                                        else
                                        {
                                            checkDelete = false;
                                        }
                                    }
                                    else if (gma.idAction == 8)
                                    {
                                        --permissionUserMenu.AddMember;
                                        if (permissionUserMenu.AddMember <= 0)
                                        {
                                            permissionUserMenu.AddMember = 0;
                                            checkDelete = true;
                                        }
                                        else
                                        {
                                            checkDelete = false;
                                        }
                                    }
                                    else if (gma.idAction == 9)
                                    {
                                        --permissionUserMenu.Export;
                                        if (permissionUserMenu.Export <= 0)
                                        {
                                            permissionUserMenu.Export = 0;
                                            checkDelete = true;
                                        }
                                        else
                                        {
                                            checkDelete = false;
                                        }
                                    }
                                }
                                if (checkDelete == true)
                                {
                                    permissionUserMenu.isDeleted = true;
                                }
                                permissionUserMenu.userModified = idUserUpdated;
                                permissionUserMenu.dateModified = DateTime.Now;
                                _db.Permission_Use_Menus.Update(permissionUserMenu);
                            }
                        }
                        foreach (var item in groupModuleActions)
                        {
                            item.userDeleted = idUserUpdated;
                            item.dateDeleted = DateTime.Now;
                            item.isDeleted = true;
                            _db.GroupModuleActions.Update(item);
                        }
                    }
                    await _db.SaveChangesAsync();
                }
                success = true;
                message = "PermissionGroup access refuse successfully";
                return new BaseResponse<Permission_Group>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"PermissionGroup access refuse failed! {ex.InnerException}";
                return new BaseResponse<Permission_Group>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Permission_Group>>> PermissionGroupAccessRefuseMulti(int idUserUpdated, List<PermissionGroupRequestDto> permissionGroupRequestDtos)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Group>();
            try
            {
                foreach (var item in permissionGroupRequestDtos)
                {
                    var result = await PermissionGroupAccessRefuse(item.IdGroup, item.IdModule, idUserUpdated);
                    if (result._success)
                    {
                        data.Add(result._Data);
                    }
                }
                success = true;
                message = "PermissionGroup access refuse successfully";
                return new BaseResponse<List<Permission_Group>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"PermissionGroup access refuse failed! {ex.InnerException}";
                return new BaseResponse<List<Permission_Group>>(success, message, data);
            }
        }

        public async Task<BaseResponse<Permission_Group>> PermissionGroupAccessAllow(int idGroup, int idModule, int idUserUpdateted, List<int> listIdAction)
        {
            var success = false;
            var message = "";
            var data = new Permission_Group();
            try
            {
                var permissionGroup = await _db.Permission_Groups.Where(s => s.IsDeleted == false && s.IdModule.Equals(idModule) && s.IdGroup.Equals(idGroup)).FirstOrDefaultAsync();

                if (permissionGroup == null)
                {
                    permissionGroup = new Permission_Group();
                    permissionGroup.IdGroup = idGroup;
                    permissionGroup.IdModule = idModule;
                    permissionGroup.Access = true;
                    permissionGroup.IsDeleted = false;
                    await _db.Permission_Groups.AddAsync(permissionGroup);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    permissionGroup.Access = true;
                    _db.Permission_Groups.Update(permissionGroup);
                    data = permissionGroup;
                    List<int> listIdActionNeddAdds = new List<int>();
                    foreach (var item in listIdAction)
                    {
                        var groupModuleAction = await _db.GroupModuleActions.Where(s => s.idGroup.Equals(idGroup) && s.idModule.Equals(idModule) && s.idAction.Equals(item)).FirstOrDefaultAsync();
                        if (groupModuleAction == null)
                        {
                            groupModuleAction = new Group_Module_Action();
                            groupModuleAction.idGroup = idGroup;
                            groupModuleAction.idModule = idModule;
                            groupModuleAction.idAction = item;
                            await _db.GroupModuleActions.AddAsync(groupModuleAction);
                            listIdActionNeddAdds.Add(groupModuleAction.idAction);
                        }
                        else
                        {
                            if(groupModuleAction.isDeleted == true)
                            {
                                listIdActionNeddAdds.Add(groupModuleAction.idAction);
                            }
                            _db.GroupModuleActions.Update(groupModuleAction);
                        }
                        groupModuleAction.userCreated = idUserUpdateted;
                        groupModuleAction.dateCreated = DateTime.Now;
                        groupModuleAction.isDeleted = false;
                    }
                    await _db.SaveChangesAsync();

                    var listUserGroups = await _db.UserGroups.Where(s => s.isDeleted == false && s.idGroup.Equals(idGroup)).ToListAsync();
                    if (listUserGroups.Count > 0)
                    {
                        foreach (var usg in listUserGroups)
                        {
                            var groupModuleActions = await _db.GroupModuleActions.Where(s => s.isDeleted == false && s.idGroup.Equals(idGroup) && s.idModule.Equals(idModule)).ToListAsync();
                            if (groupModuleActions.Count > 0)
                            {
                                var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.IdUser.Equals(usg.idUser) && s.idModule.Equals(idModule)).FirstOrDefaultAsync();
                                if (permissionUserMenu == null)
                                {
                                    permissionUserMenu = new Permission_Use_Menu();
                                    permissionUserMenu.IdUser = usg.idUser;
                                    permissionUserMenu.idModule = idModule;
                                    permissionUserMenu.userCreated = idUserUpdateted;
                                    permissionUserMenu.dateCreated = DateTime.Now;
                                    await _db.Permission_Use_Menus.AddAsync(permissionUserMenu);
                                }
                                else
                                {
                                    permissionUserMenu.userModified = idUserUpdateted;
                                    permissionUserMenu.dateModified = DateTime.Now;
                                    _db.Permission_Use_Menus.Update(permissionUserMenu);
                                }
                                permissionUserMenu.isDeleted = false;
                                foreach (var gma in groupModuleActions)
                                {
                                    if (gma.idAction == 1 && listIdActionNeddAdds.Contains(1))
                                    {
                                        permissionUserMenu.Add++;
                                    }
                                    else if (gma.idAction == 2 && listIdActionNeddAdds.Contains(2))
                                    {
                                        permissionUserMenu.Update++;
                                    }
                                    else if (gma.idAction == 3 && listIdActionNeddAdds.Contains(3))
                                    {
                                        permissionUserMenu.Delete++;
                                    }
                                    else if (gma.idAction == 4 && listIdActionNeddAdds.Contains(4))
                                    {
                                        permissionUserMenu.DeleteMulti++;
                                    }
                                    else if (gma.idAction == 5 && listIdActionNeddAdds.Contains(5))
                                    {
                                        permissionUserMenu.Confirm++;
                                    }
                                    else if (gma.idAction == 6 && listIdActionNeddAdds.Contains(6))
                                    {
                                        permissionUserMenu.ConfirmMulti++;
                                    }
                                    else if (gma.idAction == 7 && listIdActionNeddAdds.Contains(7))
                                    {
                                        permissionUserMenu.Refuse++;
                                    }
                                    else if (gma.idAction == 8 && listIdActionNeddAdds.Contains(8))
                                    {
                                        permissionUserMenu.AddMember++;
                                    }
                                    else if (gma.idAction == 9 && listIdActionNeddAdds.Contains(9))
                                    {
                                        permissionUserMenu.Export++;
                                    }
                                }
                            }
                        }
                        await _db.SaveChangesAsync();
                    }
                }
                success = true;
                message = "PermissionGroup access allow successfully";
                return new BaseResponse<Permission_Group>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"PermissionGroup access allow failed! {ex.InnerException}";
                return new BaseResponse<Permission_Group>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Permission_Group>>> PermissionGroupAccessAllowMulti(List<PermissionGroupAccessAllowMultiDto> permissionGroupAccessAllowMultiDtos)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Group>();
            try
            {
                foreach (var item in permissionGroupAccessAllowMultiDtos)
                {
                    var result = await PermissionGroupAccessAllow(item.idGroup, item.idModule, item.idUserUpdateted, item.listIdAction);
                    if (result._success)
                    {
                        data.Add(result._Data);
                    }
                }

                success = true;
                message = "PermissionGroupMulti access refuse successfully";
                return new BaseResponse<List<Permission_Group>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"PermissionGroupMulti access refuse failed! {ex.InnerException}";
                return new BaseResponse<List<Permission_Group>>(success, message, data);
            }
        }

        public async Task<BaseResponse<Permission_Group>> DeletePermissionGroup(int idGroup, int idModule)
        {
            var success = false;
            var message = "";
            var data = new Permission_Group();
            try
            {
                var permissionGroup = await _db.Permission_Groups.Where(s => s.IsDeleted == false && s.IdModule.Equals(idModule) && s.IdGroup.Equals(idGroup)).FirstOrDefaultAsync();
                if (permissionGroup is null)
                {
                    success = false;
                    message = "UserGroup doesn't exist !";
                    return new BaseResponse<Permission_Group>(success, message, data);
                }

                permissionGroup.IsDeleted = true;
                _db.Permission_Groups.Update(permissionGroup);
                await _db.SaveChangesAsync();

                success = true;
                message = "Deleting Permission_Group successfully";
                return new BaseResponse<Permission_Group>(success, message, permissionGroup);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Permission_Group failed! {ex.InnerException}";
                return new BaseResponse<Permission_Group>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Permission_Group>>> DeleteMultiPermissionGroup(List<PermissionGroupRequestDto> permissionGroupRequestDto)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Group>();
            try
            {
                foreach (var item in permissionGroupRequestDto)
                {
                    var result = await DeletePermissionGroup(item.IdGroup, item.IdModule);
                    if (result._success)
                    {
                        data.Add(result._Data);
                    }
                    else
                    {
                        return new BaseResponse<List<Permission_Group>>(success, result._Message, data);
                    }
                }
                success = true;
                message = "Deleting Multi Permission_Group successfully";
                return new BaseResponse<List<Permission_Group>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Multi Permission_Group failed! {ex.InnerException}";
                return new BaseResponse<List<Permission_Group>>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Permission_Group>>> UpdatePermissionGroupModule(List<PermissionGroupDto> permissionGroupDtos)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Group>();
            try
            {
                foreach (var x in permissionGroupDtos)
                {
                    var oldObj = await _db.Permission_Groups.FirstOrDefaultAsync(c => c.IdGroup == x.IdGroup && c.IdModule == x.IdModule);
                    if (oldObj != null)
                    {
                        oldObj.Access = x.Access;
                    }
                    else
                    {
                        if (x.IdGroup > 0 && x.IdModule > 0)
                        {
                           var permissionGroup = _mapper.Map<Permission_Group>(x);
                           data.Add(permissionGroup);
                        }
                        else
                        {
                            success = false;
                            message = $"Adding new Permission_Group failed! ";
                            return new BaseResponse<List<Permission_Group>>(success, message, data);
                        }
                    }
                    
                }
                await _db.AddRangeAsync(data);
                await _db.SaveChangesAsync();
                success = true;
                message = "Add new Permission_Group successfully";
                return new BaseResponse<List<Permission_Group>>(success, message, data.Distinct().ToList());
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Adding new Permission_Group failed! {ex.Message}";
                return new BaseResponse<List<Permission_Group>>(success, message, data);
            }
          
        }
    }
}
