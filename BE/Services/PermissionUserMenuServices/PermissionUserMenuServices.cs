using AutoMapper;
using BE.Data.Contexts;
using BE.Data.DataRoles;
using BE.Data.Dtos.Permission_Use_Menus;
using BE.Data.Dtos.UserDtos;
using BE.Data.Models;
using BE.Response;
using BE.Services.GroupServices;
using BE.Services.UserServices;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BE.Services.PermissionUserMenuServices
{
    public interface IPermissionUserMenuServices
    {
        Task<BaseResponse<List<Permission_Use_Menu>>> GetAllPermissionUserMenuAsync();
        Task<BaseResponse<List<Permission_Use_Menu>>> GetPermissionUserMenuWithUserId(int userId);
        Task<BaseResponse<List<Permission_Use_Menu>>> GetPermissionUserMenuWithModuleId(int moduleId);
        Task<BaseResponse<List<Permission_Use_Menu>>> GetPermissionUserMenuWithMenuId(int menuId);
        Task<BaseResponse<List<Permission_Use_Menu>>> CreatePermissionUserMenu(List<PermissionUserMenuAddDto> permissionUserMenuAddDtos);
        Task<BaseResponse<Permission_Use_Menu>> UpdatePermissionUserMenu(PermissionUserMenuRequestDto permissionUserMenuRequest, PermissionUserMenuEditDto permissionUserMenuEditDto);
        Task<BaseResponse<Permission_Use_Menu>> DeletePermissionUserMenu(PermissionUserMenuRequesNoIdGrouptDto permissionUserMenuRequesNoIdGrouptDto);
        Task<BaseResponse<List<Permission_Use_Menu>>> DeleteMultiPermissionUserMenu(List<PermissionUserMenuRequesNoIdGrouptDto> permissionUserMenuRequesNoIdGrouptDto);
        Task<BaseResponse<List<Permission_Use_Menu>>> AddPermissionRoleUserMenuDefault(int idUser, int idUserCreated, List<int> listIdGroup);
        Task<BaseResponse<List<Permission_Use_Menu>>> AddPermissionRoleUserMenuUpgrade(int idUser, int idUserCreated, List<int> listIdGroup);
        Task<BaseResponse<List<Permission_Use_Menu>>> DeletePermissionRoleUserMenuDefault(int idUser, List<int> listIdGroup);
        Task<BaseResponse<List<Permission_Use_Menu>>> DeletePermissionRoleUserMenuUpgrade(int idUser, int idUserUpdated, List<int> listIdGroup);
    }

    public class PermissionUserMenuServices : IPermissionUserMenuServices
    {

        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IDataAdmin _dataAdmin;
        private readonly IDataPm _dataPm;
        private readonly IDataLead _dataLead;
        private readonly IDataSample _dataSample;
        private readonly IDataStaff _dataStaff;

        public PermissionUserMenuServices(AppDbContext db, IMapper mapper, IDataAdmin dataAdmin, IDataPm dataPm, IDataLead dataLead, IDataSample dataSample, IDataStaff dataStaff)
        {
            _db = db;
            _mapper = mapper;
            _dataAdmin = dataAdmin;
            _dataPm = dataPm;
            _dataLead = dataLead;
            _dataSample = dataSample;
            _dataStaff = dataStaff;
        }

        public async Task<BaseResponse<List<Permission_Use_Menu>>> GetAllPermissionUserMenuAsync()
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Use_Menu>();
            try
            {
                var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.isDeleted == false).ToListAsync();

                success = true;
                message = "Get all data successfully";
                data.AddRange(permissionUserMenu);
                return (new BaseResponse<List<Permission_Use_Menu>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Use_Menu>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Use_Menu>>> GetPermissionUserMenuWithMenuId(int menuId)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Use_Menu>();
            try
            {
                var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.isDeleted == false && s.IdMenu.Equals(menuId)).ToListAsync();
                success = true;
                message = "Get data successfully";
                data.AddRange(permissionUserMenu);
                return (new BaseResponse<List<Permission_Use_Menu>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Use_Menu>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Use_Menu>>> GetPermissionUserMenuWithModuleId(int moduleId)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Use_Menu>();
            try
            {
                var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.isDeleted == false && s.idModule.Equals(moduleId)).ToListAsync();
                success = true;
                message = "Get data successfully";
                data.AddRange(permissionUserMenu);
                return (new BaseResponse<List<Permission_Use_Menu>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Use_Menu>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Use_Menu>>> GetPermissionUserMenuWithUserId(int userId)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Use_Menu>();
            try
            {
                var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.isDeleted == false && s.IdUser.Equals(userId)).ToListAsync();
                success = true;
                message = "Get data successfully";
                data.AddRange(permissionUserMenu);
                return (new BaseResponse<List<Permission_Use_Menu>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Use_Menu>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Use_Menu>>> CreatePermissionUserMenu(List<PermissionUserMenuAddDto> permissionUserMenuAddDtos)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Use_Menu>();
            try
            {
                foreach (var item in permissionUserMenuAddDtos)
                {
                    var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.IdUser.Equals(item.IdUser)
                                && s.idModule.Equals(item.idModule)).FirstOrDefaultAsync();
                    if (permissionUserMenu == null)
                    {
                        permissionUserMenu = _mapper.Map<Permission_Use_Menu>(item);
                        await _db.Permission_Use_Menus.AddAsync(permissionUserMenu);
                    }
                    else
                    {
                        permissionUserMenu = _mapper.Map<PermissionUserMenuAddDto, Permission_Use_Menu>(item, permissionUserMenu);
                        _db.Permission_Use_Menus.Update(permissionUserMenu);
                    }
                    permissionUserMenu.dateCreated = DateTime.Now;
                    permissionUserMenu.isDeleted = false;
                    data.Add(permissionUserMenu);
                }
                await _db.SaveChangesAsync();
                success = true;
                message = "Add new Permission_Use_Menu successfully";
                return new BaseResponse<List<Permission_Use_Menu>>(success, message, data.Distinct().ToList());
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Adding new Permission_Use_Menu failed! {ex.Message}";
                return new BaseResponse<List<Permission_Use_Menu>>(success, message, data);
            }
        }

        public async Task<BaseResponse<Permission_Use_Menu>> UpdatePermissionUserMenu(PermissionUserMenuRequestDto permissionUserMenuRequest, PermissionUserMenuEditDto permissionUserMenuEditDto)
        {
            var success = false;
            var message = "";
            Permission_Use_Menu permissionUserMenuMapData;
            try
            {
                var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.isDeleted == false && s.IdUser.Equals(permissionUserMenuRequest.IdUser)
                                && s.idModule.Equals(permissionUserMenuRequest.idModule)).FirstOrDefaultAsync();
                if (permissionUserMenu is null)
                {
                    var permissionGroup = await _db.Permission_Groups.Where(s => s.IsDeleted == false && s.IdModule.Equals(permissionUserMenuRequest.idModule) && s.IdGroup.Equals(permissionUserMenuRequest.idGroup) && s.Access == true).FirstOrDefaultAsync();
                    if (permissionGroup is null)
                    {
                        message = "The user does not have permission to perform this function!";
                        return new BaseResponse<Permission_Use_Menu>(success, message, permissionUserMenuMapData = null);
                    }
                    else
                    {
                        permissionUserMenuMapData = _mapper.Map<Permission_Use_Menu>(permissionUserMenuEditDto);
                        permissionUserMenuMapData.IdUser = permissionUserMenuRequest.IdUser;
                        permissionUserMenuMapData.idModule = permissionUserMenuRequest.idModule;
                        permissionUserMenuMapData.dateCreated = DateTime.Now;
                        permissionUserMenuMapData.userCreated = permissionUserMenuEditDto.userModified;
                        permissionUserMenuMapData.userModified = null;
                        await _db.Permission_Use_Menus.AddAsync(permissionUserMenuMapData);
                    }
                }
                else
                {
                    permissionUserMenuMapData = _mapper.Map<PermissionUserMenuEditDto, Permission_Use_Menu>(permissionUserMenuEditDto, permissionUserMenu);
                    permissionUserMenuMapData.dateModified = DateTime.Now;
                    _db.Permission_Use_Menus.Update(permissionUserMenuMapData);
                }
                await _db.SaveChangesAsync();
                success = true;
                message = "Editing Permission_Use_Menu successfully";
                return new BaseResponse<Permission_Use_Menu>(success, message, permissionUserMenuMapData);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Editing Permission_Use_Menu failed! {ex.Message}";
                return new BaseResponse<Permission_Use_Menu>(success, message, permissionUserMenuMapData = null);
            }
        }

        public async Task<BaseResponse<Permission_Use_Menu>> DeletePermissionUserMenu(PermissionUserMenuRequesNoIdGrouptDto permissionUserMenuRequesNoIdGrouptDto)
        {
            var success = false;
            var message = "";
            var data = new Permission_Use_Menu();
            try
            {
                var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.isDeleted == false && s.IdUser.Equals(permissionUserMenuRequesNoIdGrouptDto.IdUser) && s.idModule.Equals(permissionUserMenuRequesNoIdGrouptDto.idModule)).FirstOrDefaultAsync();

                if (permissionUserMenu is null)
                {
                    success = false;
                    message = "Permission_Use_Menus doesn't exist !";
                    return new BaseResponse<Permission_Use_Menu>(success, message, data);
                }

                var permissionUserMenuMapData = _mapper.Map<PermissionUserMenuRequesNoIdGrouptDto, Permission_Use_Menu>(permissionUserMenuRequesNoIdGrouptDto, permissionUserMenu);
                permissionUserMenuMapData.isDeleted = true;
                permissionUserMenuMapData.dateModified = DateTime.Now;
                _db.Permission_Use_Menus.Update(permissionUserMenuMapData);
                await _db.SaveChangesAsync();

                success = true;
                message = "Deleting Permission_Use_Menu successfully";
                return new BaseResponse<Permission_Use_Menu>(success, message, permissionUserMenuMapData);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Permission_Use_Menu failed! {ex.InnerException}";
                return new BaseResponse<Permission_Use_Menu>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Permission_Use_Menu>>> DeleteMultiPermissionUserMenu(List<PermissionUserMenuRequesNoIdGrouptDto> permissionUserMenuRequesNoIdGrouptDto)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Use_Menu>();
            try
            {
                foreach (var item in permissionUserMenuRequesNoIdGrouptDto)
                {
                    var result = await DeletePermissionUserMenu(item);
                    if (result._success)
                    {
                        data.Add(result._Data);
                    }
                }
                success = true;
                message = "Deleting Multi Permission_Use_Menu successfully";
                return new BaseResponse<List<Permission_Use_Menu>>(success, message, data.Distinct().ToList());
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Multi Permission_Use_Menu failed! {ex.InnerException}";
                return new BaseResponse<List<Permission_Use_Menu>>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Permission_Use_Menu>>> AddPermissionRoleUserMenuDefault(int idUser, int idUserCreated, List<int> listIdGroup)
        {
            var success = false;
            var message = "";
            var arrPerUser = new List<Permission_Use_Menu>();
            var data = new List<Permission_Use_Menu>();
            try
            {
                listIdGroup = listIdGroup.OrderBy(s => s).ToList();
                foreach (var idGroup in listIdGroup)
                {
                    var group = await _db.Groups.Where(s => s.IsDeleted == 0 && s.Id.Equals(idGroup)).FirstOrDefaultAsync();
                    if (group != null)
                    {
                        var checkGroupUser = await _db.UserGroups.Where(x => x.idGroup == idGroup && x.idUser == idUser).FirstOrDefaultAsync();
                        if (checkGroupUser != null)
                        {
                            if (group.NameGroup.ToLower().Equals("admin"))
                            {
                                arrPerUser = _dataAdmin.RoleAdmin(idUser, idGroup, idUserCreated);
                            }
                            else if (group.NameGroup.ToLower().Equals("pm"))
                            {
                                arrPerUser = _dataPm.RolePm(idUser, idGroup, idUserCreated);
                            }
                            else if (group.NameGroup.ToLower().Equals("lead"))
                            {
                                arrPerUser = _dataLead.RoleLead(idUser, idGroup, idUserCreated);
                            }
                            else if (group.NameGroup.ToLower().Equals("office"))
                            {
                                arrPerUser = _dataSample.RoleSample(idUser, idGroup, idUserCreated);
                            }
                            else if (group.NameGroup.ToLower().Equals("staff"))
                            {
                                arrPerUser = _dataStaff.RoleStaff(idUser, idGroup, idUserCreated);
                            }
                            else
                            {
                                arrPerUser = _dataStaff.RoleStaff(idUser, idGroup, idUserCreated);
                            }

                            foreach (var item in arrPerUser)
                            {
                                var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.isDeleted == false && s.IdUser.Equals(item.IdUser)
                                            && s.idModule.Equals(item.idModule)).FirstOrDefaultAsync();
                                if (permissionUserMenu == null)
                                {
                                    await _db.Permission_Use_Menus.AddAsync(item);
                                    data.Add(item);
                                }
                            }
                            await _db.SaveChangesAsync();
                            if (group.NameGroup.ToLower().Equals("admin")) { break; }
                        }
                    }
                }
                success = true;
                message = "Add Role Permission_Use_Menu successfully";
                return new BaseResponse<List<Permission_Use_Menu>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Add Role Permission_Use_Menu failed! {ex.InnerException}";
                return new BaseResponse<List<Permission_Use_Menu>>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Permission_Use_Menu>>> AddPermissionRoleUserMenuUpgrade(int idUser, int idUserCreated, List<int> listIdGroup)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Use_Menu>();
            try
            {
                var getAllActionModules = await _db.ActionModules.Where(s => s.isDeleted == false).ToListAsync();
                int countActionModules = getAllActionModules.Count;
                foreach (var idGroup in listIdGroup)
                {
                    var checkGroupUser = await _db.UserGroups.Where(x => x.idGroup == idGroup && x.idUser == idUser).FirstOrDefaultAsync();
                    if (checkGroupUser != null)
                    {
                        var groupModuleActions = await _db.GroupModuleActions.Where(s => s.isDeleted == false && s.idGroup.Equals(idGroup)).ToListAsync();
                        if (groupModuleActions.Count() > 0)
                        {
                            var getAllModules = (from md in _db.modules
                                                 join pg in _db.Permission_Groups on md.id equals pg.IdModule
                                                 where md.isDeleted == 0 && pg.IsDeleted == false && pg.Access == true && pg.IsDeleted == false && pg.IdGroup == idGroup
                                                 select md).Distinct().ToList();
                            foreach (var module in getAllModules)
                            {
                                var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.IdUser.Equals(idUser) && s.idModule.Equals(module.id)).FirstOrDefaultAsync();
                                if (permissionUserMenu == null)
                                {
                                    permissionUserMenu = new Permission_Use_Menu();
                                    permissionUserMenu.IdUser = idUser;
                                    permissionUserMenu.idModule = module.id;
                                    permissionUserMenu.userCreated = idUserCreated;
                                    permissionUserMenu.dateCreated = DateTime.Now;
                                    await _db.Permission_Use_Menus.AddAsync(permissionUserMenu);
                                }
                                else
                                {
                                    permissionUserMenu.userModified = idUserCreated;
                                    permissionUserMenu.dateModified = DateTime.Now;
                                    _db.Permission_Use_Menus.Update(permissionUserMenu);
                                }
                                permissionUserMenu.isDeleted = false;
                                int count = 0;
                                foreach (var item in groupModuleActions)
                                {

                                    if (item.idModule == module.id)
                                    {
                                        if (item.idAction == 1)
                                        {
                                            permissionUserMenu.Add++;
                                        }
                                        else if (item.idAction == 2)
                                        {
                                            permissionUserMenu.Update++;
                                        }
                                        else if (item.idAction == 3)
                                        {
                                            permissionUserMenu.Delete++;
                                        }
                                        else if (item.idAction == 4)
                                        {
                                            permissionUserMenu.DeleteMulti++;
                                        }
                                        else if (item.idAction == 5)
                                        {
                                            permissionUserMenu.Confirm++;
                                        }
                                        else if (item.idAction == 6)
                                        {
                                            permissionUserMenu.ConfirmMulti++;
                                        }
                                        else if (item.idAction == 7)
                                        {
                                            permissionUserMenu.Refuse++;
                                        }
                                        else if (item.idAction == 8)
                                        {
                                            permissionUserMenu.AddMember++;
                                        }
                                        else if (item.idAction == 9)
                                        {
                                            permissionUserMenu.Export++;
                                        }
                                        if (++count == countActionModules) { break; }

                                    }
                                }
                                data.Add(permissionUserMenu);
                            }
                        }
                    }
                }

                await _db.SaveChangesAsync();
                success = true;
                message = "Add Role Permission_Use_Menu successfully";
                return new BaseResponse<List<Permission_Use_Menu>>(success, message, data.Distinct().ToList());
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Add Role Permission_Use_Menu failed! {ex.InnerException}";
                return new BaseResponse<List<Permission_Use_Menu>>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Permission_Use_Menu>>> DeletePermissionRoleUserMenuDefault(int idUser, List<int> listIdGroup)
        {
            var success = false;
            var message = "";
            var arrPerUser = new List<Permission_Use_Menu>();
            var data = new List<Permission_Use_Menu>();
            try
            {
                foreach (var idGroup in listIdGroup)
                {
                    var getUserGroup = await _db.UserGroups.Where(s => s.isDeleted == false && s.idUser.Equals(idUser)).ToListAsync();
                    if (getUserGroup.Count() != 0)
                    {
                        var getDataNeedDelete = await _db.Permission_Groups.Where(s => s.IsDeleted == false && s.IdGroup.Equals(idGroup) && s.Access == true).ToListAsync();
                        if (getDataNeedDelete.Count() != 0)
                        {
                            var mergeGroupUser = new List<Permission_Group>();
                            foreach (var item in getUserGroup)
                            {
                                if (item.idGroup != idGroup)
                                {
                                    var itemGroup = await _db.Permission_Groups.Where(s => s.IsDeleted == false && s.IdGroup.Equals(item.idGroup) && s.Access == true).ToListAsync();
                                    mergeGroupUser.AddRange(itemGroup);
                                }
                                else
                                {
                                    _db.UserGroups.Remove(item);
                                }
                            }

                            foreach (var itemOn in mergeGroupUser)
                            {
                                foreach (var itemIn in getDataNeedDelete)
                                {
                                    if (itemOn.IdModule == itemIn.IdModule)
                                    {
                                        getDataNeedDelete.Remove(itemIn);
                                        break;
                                    }
                                }
                            }

                            foreach (var itemIn in getDataNeedDelete)
                            {
                                var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.IdUser.Equals(idUser) && s.idModule.Equals(itemIn.IdModule)).FirstOrDefaultAsync();
                                if (permissionUserMenu != null)
                                {
                                    _db.Permission_Use_Menus.Remove(permissionUserMenu);
                                    data.Add(permissionUserMenu);
                                }

                            }
                            await _db.SaveChangesAsync();
                        }
                        //else
                        //{
                        //    message = "Empty delete data!";
                        //    return new BaseResponse<List<Permission_Use_Menu>>(success, message, data = null);
                        //}
                    }
                }
                //else
                //{
                //    message = "No data to delete!";
                //    return new BaseResponse<List<Permission_Use_Menu>>(success, message, data = null);
                //}
                success = true;
                message = "Deleting Role Permission_Use_Menu successfully";
                return new BaseResponse<List<Permission_Use_Menu>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Role Permission_Use_Menu failed! {ex.InnerException}";
                return new BaseResponse<List<Permission_Use_Menu>>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Permission_Use_Menu>>> DeletePermissionRoleUserMenuUpgrade(int idUser, int idUserUpdated, List<int> listIdGroup)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Use_Menu>();
            try
            {
                var getAllActionModules = await _db.ActionModules.Where(s => s.isDeleted == false).ToListAsync();
                int countActionModules = getAllActionModules.Count;
                foreach (var idGroup in listIdGroup)
                {
                    var getUserGroup = await _db.UserGroups.Where(s => s.isDeleted == false && s.idUser.Equals(idUser) && s.idGroup.Equals(idGroup)).FirstOrDefaultAsync();
                    if (getUserGroup != null)
                    {
                        getUserGroup.isDeleted = true;
                        getUserGroup.userDeleted = idUserUpdated;
                        getUserGroup.dateDeleted = DateTime.Now;
                        _db.UserGroups.Update(getUserGroup);

                        var groupModuleActions = await _db.GroupModuleActions.Where(s => s.isDeleted == false && s.idGroup.Equals(idGroup)).ToListAsync();
                        if (groupModuleActions.Count() > 0)
                        {
                            var getAllModules = (from md in _db.modules
                                                 join pg in _db.Permission_Groups on md.id equals pg.IdModule
                                                 where md.isDeleted == 0 && pg.IsDeleted == false && pg.Access == true && pg.IdGroup == idGroup
                                                 select md).Distinct().ToList();
                            foreach (var module in getAllModules)
                            {
                                var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.isDeleted == false && s.IdUser.Equals(idUser) && s.idModule.Equals(module.id)).FirstOrDefaultAsync();
                                if (permissionUserMenu != null)
                                {
                                    int count = 0;
                                    bool checkDelete = false;
                                    foreach (var item in groupModuleActions)
                                    {
                                        if (item.idModule == module.id)
                                        {
                                            if (item.idAction == 1)
                                            {
                                                --permissionUserMenu.Add;
                                                if(permissionUserMenu.Add <= 0)
                                                {
                                                    permissionUserMenu.Add = 0;
                                                    checkDelete = true;
                                                }else
                                                {
                                                    checkDelete = false;
                                                }
                                            }
                                            else if (item.idAction == 2)
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
                                            else if (item.idAction == 3)
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
                                            else if (item.idAction == 4)
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
                                            else if (item.idAction == 5)
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
                                            else if (item.idAction == 6)
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
                                            else if (item.idAction == 7)
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
                                            else if (item.idAction == 8)
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
                                            else if (item.idAction == 9)
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
                                            if (++count == countActionModules) { break; }
                                        }
                                    }
                                    if(checkDelete == true)
                                    {
                                        permissionUserMenu.isDeleted = true;
                                    }
                                    permissionUserMenu.userModified = idUserUpdated;
                                    permissionUserMenu.dateModified = DateTime.Now;
                                    _db.Permission_Use_Menus.Update(permissionUserMenu);
                                    data.Add(permissionUserMenu);
                                }
                            }
                        }

                    }
                }
                await _db.SaveChangesAsync();
                success = true;
                message = "Deleting Role Permission_Use_Menu successfully";
                return new BaseResponse<List<Permission_Use_Menu>>(success, message, data.Distinct().ToList());
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Role Permission_Use_Menu failed! {ex.InnerException}";
                return new BaseResponse<List<Permission_Use_Menu>>(success, message, data);
            }
        }



    }
}
