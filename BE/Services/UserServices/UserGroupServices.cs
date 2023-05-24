using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.RulesDTOs;
using BE.Data.Dtos.UserDtos;
using BE.Data.Models;
using BE.Helpers;
using BE.Response;
using BE.Services.PermissionUserMenuServices;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace BE.Services.UserServices
{
    public interface IUserGroupServices
    {
        Task<BaseResponse<List<User_Group>>> GetAllUserGroupAsync();
        Task<BaseResponse<List<User_Group>>> GetUserGroupWithUserId(int userId);
        Task<BaseResponse<List<UserGroupResponse>>> GetUserObjectGroupWithUserId(int groupId);
        Task<BaseResponse<List<User_Group>>> GetUserGroupWithGroupId(int groupId);
        Task<BaseResponse<List<UserNameGroupResponse>>> GetAllUserNameGroupWithGroupId(int groupId);
        Task<BaseResponse<List<UserNameGroupResponse>>> GetAllSearchUserNameGroupWithGroupId(int groupId, string userName);
        Task<BaseResponse<List<User_Group>>> CreateUserGroup(List<UserGroupCreatedDto> userGroupCreatedDto);
        Task<BaseResponse<User_Group>> UpdateUserGroup(int idUser, int idGroup, UserGroupUpdatedDto userGroupUpdatedDto);
        Task<BaseResponse<List<User_Group>>> UpdateUserGroupMultiRoleDefault(int idUser, int idUserCreated, List<int> listIdGroup);
        Task<BaseResponse<List<User_Group>>> UpdateUserGroupMultiRoleUpgrade(int idUser, int idUserUpdated, List<int> listIdGroup);
        Task<BaseResponse<User_Group>> DeleteUserGroup(UserGroupDeletedDto userGroupDeletedDto);
        Task<BaseResponse<List<User_Group>>> DeleteMultiUserGroup(List<UserGroupDeletedDto> userGroupDeletedDto);
    }

    public class UserGroupServices : IUserGroupServices
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IPermissionUserMenuServices _permissionUserMenuServices;

        public UserGroupServices(AppDbContext db, IMapper mapper, IPermissionUserMenuServices permissionUserMenuServices)
        {
            _db = db;
            _mapper = mapper;
            _permissionUserMenuServices = permissionUserMenuServices;
        }

        public async Task<BaseResponse<List<User_Group>>> GetAllUserGroupAsync()
        {
            var success = false;
            var message = "";
            var data = new List<User_Group>();
            try
            {
                var userGroup = await _db.UserGroups.Where(s => s.isDeleted == false).OrderBy(s => s.idUser).ToListAsync();

                success = true;
                message = "Get all data successfully";
                data.AddRange(userGroup);
                return (new BaseResponse<List<User_Group>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<User_Group>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<User_Group>>> GetUserGroupWithGroupId(int groupId)
        {
            var success = false;
            var message = "";
            var data = new List<User_Group>();
            try
            {
                var userGroup = await _db.UserGroups
                    .OrderBy(s => s.idUser)
                    .Where(s => s.isDeleted == false
                            && s.idGroup.Equals(groupId))
                    .ToListAsync();
                success = true;
                message = "Get data successfully";
                data.AddRange(userGroup);
                return (new BaseResponse<List<User_Group>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<User_Group>>(success, message, data));
            }
        }
        private List<UserNameGroupResponse> GetUserNameGroupWithGroupId(int groupId)
        {
            var userGroups = (from us in _db.Users
                                   join ug in _db.UserGroups on us.id equals ug.idUser
                                   where ug.isDeleted == false && us.isDeleted == 0 && ug.idGroup == groupId
                                   orderby us.firstName ascending
                                   select new UserNameGroupResponse
                                   {
                                       id = ug.id,
                                       idUser = ug.idUser,
                                       firstName = us.firstName,
                                       fullNameUser = us.lastName + " " + us.firstName,
                                       idGroup = ug.idGroup,
                                       isDeleted = ug.isDeleted,
                                   }).ToList();
            return userGroups;
        }
        public async Task<BaseResponse<List<UserNameGroupResponse>>> GetAllUserNameGroupWithGroupId(int groupId)
        {
            var success = false;
            var message = "";
            var data = new List<UserNameGroupResponse>();
            try
            {
                var userGroups = GetUserNameGroupWithGroupId(groupId);
                success = true;
                message = "Get data successfully";
                return (new BaseResponse<List<UserNameGroupResponse>>(success, message, userGroups));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<UserNameGroupResponse>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<UserNameGroupResponse>>> GetAllSearchUserNameGroupWithGroupId(int groupId, string userName)
        {
            var success = false;
            var message = "";
            var data = new List<UserNameGroupResponse>();
            try
            {
                var userGroups = GetUserNameGroupWithGroupId(groupId).Where(s => s.firstName.ToLower().Contains(userName.Trim().ToLower())).ToList();
                success = true;
                message = "Get data successfully";
                return (new BaseResponse<List<UserNameGroupResponse>>(success, message, userGroups));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<UserNameGroupResponse>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<User_Group>>> GetUserGroupWithUserId(int userId)
        {
            var success = false;
            var message = "";
            var data = new List<User_Group>();
            try
            {
                var userGroup = await _db.UserGroups.OrderBy(s => s.idUser).Where(s => s.isDeleted == false && s.idUser.Equals(userId)).ToListAsync();

                success = true;
                message = "Get data successfully";
                data.AddRange(userGroup);
                return (new BaseResponse<List<User_Group>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<User_Group>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<UserGroupResponse>>> GetUserObjectGroupWithUserId(int userId)
        {
            var success = false;
            var message = "";
            var data = new List<UserGroupResponse>();
            try
            {
                var userGroup = await _db.UserGroups.OrderBy(s => s.idUser).Where(s => s.isDeleted == false && s.idUser.Equals(userId)).ToListAsync();
                foreach (var item in userGroup)
                {
                    var group = await _db.Groups.Where(s => s.IsDeleted == 0 && s.Id.Equals(item.idGroup)).FirstOrDefaultAsync();
                    var user = new UserGroupResponse
                    {
                        id = item.id,
                        idGroup = item.idGroup,
                        idUser = item.idUser,
                        group = group,
                    };
                    data.Add(user);
                }
                success = true;
                message = "Get data successfully";
                return (new BaseResponse<List<UserGroupResponse>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<UserGroupResponse>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<User_Group>>> CreateUserGroup(List<UserGroupCreatedDto> userGroupCreatedDto)
        {
            var success = false;
            var message = "";
            var data = new List<User_Group>();
            try
            {
                foreach (var item in userGroupCreatedDto)
                {
                    var userGroupFind = await _db.UserGroups.Where(s => s.idUser.Equals(item.idUser) && s.idGroup.Equals(item.idGroup)).FirstOrDefaultAsync();
                    if(userGroupFind == null)
                    {
                        userGroupFind = _mapper.Map<User_Group>(item);
                        await _db.UserGroups.AddAsync(userGroupFind);
                    }
                    else
                    {
                        userGroupFind = _mapper.Map<UserGroupCreatedDto, User_Group>(item, userGroupFind);
                        _db.UserGroups.Update(userGroupFind);
                    }
                    data.Add(userGroupFind);
                    userGroupFind.dateCreated = DateTime.Now;
                    userGroupFind.isDeleted = false;
                }

                await _db.SaveChangesAsync();
                success = true;
                message = "Add new User Group successfully";
                return new BaseResponse<List<User_Group>>(success, message, data.Distinct().ToList());
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Adding new User Group failed! {ex.Message}";
                return new BaseResponse<List<User_Group>>(success, message, data);
            }
        }

        public async Task<BaseResponse<User_Group>> UpdateUserGroup(int idUser, int idGroup, UserGroupUpdatedDto userGroupUpdatedDto)
        {
            var success = false;
            var message = "";
            var data = new User_Group();
            try
            {
                var userGroup = await _db.UserGroups.Where(s => s.isDeleted == false && s.idUser.Equals(idUser) && s.idGroup.Equals(idGroup)).FirstOrDefaultAsync();
                if (userGroup is null)
                {
                    message = "UserGroup doesn't exist !";
                    data = null;
                    return new BaseResponse<User_Group>(success, message, data);
                }
                var userGroupMapData = _mapper.Map<UserGroupUpdatedDto, User_Group>(userGroupUpdatedDto, userGroup);

                userGroupMapData.dateUpdated = DateTime.Now;
                _db.UserGroups.Update(userGroupMapData);
                await _db.SaveChangesAsync();

                success = true;
                message = "Editing UserGroup successfully";
                data = userGroupMapData;
                return new BaseResponse<User_Group>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Editing UserGroup failed! {ex.Message}";
                return new BaseResponse<User_Group>(success, message, data = null);
            }
        }

        public async Task<BaseResponse<List<User_Group>>> UpdateUserGroupMultiRoleDefault(int idUser, int idUserCreated, List<int> listIdGroup)
        {
            var success = false;
            var message = "";
            var data = new List<User_Group>();
            try
            {
                // get all list group
                var userOldGroup = await _db.UserGroups.Where(s => s.isDeleted == false && s.idUser.Equals(idUser)).ToListAsync();
                var arrayDelGroupId = new List<int>();

                if (userOldGroup.Count > 0)
                {
                    var getGroupHaveId = userOldGroup.Where(s => listIdGroup.Contains(s.idGroup)).ToList();
                    var getGroupHaveNotId = userOldGroup.Where(s => !listIdGroup.Contains(s.idGroup)).ToList();
                    if (getGroupHaveNotId.Count > 0)
                    {
                        foreach (var item in getGroupHaveNotId)
                        {
                            _db.UserGroups.Remove(item);
                            arrayDelGroupId.Add(item.idGroup);
                        }
                    }
                    if (arrayDelGroupId.Count > 0)
                    {
                        await _permissionUserMenuServices.DeletePermissionRoleUserMenuDefault(idUser, arrayDelGroupId);
                    }
                    var groupAdds = await _db.Groups.Where(s => listIdGroup.Contains(s.Id) && !getGroupHaveId.Select(x => x.idGroup).ToList().Contains(s.Id)).ToListAsync();
                    if (groupAdds.Count > 0)
                    {
                        foreach (var item in groupAdds)
                        {
                            var userGroup = new User_Group
                            {
                                idUser = idUser,
                                idGroup = item.Id,
                                userCreated = idUserCreated,
                                dateCreated = DateTime.Now,
                                isDeleted = false,
                            };
                            await _db.UserGroups.AddAsync(userGroup);
                            data.Add(userGroup);
                        }
                    }
                    await _db.SaveChangesAsync();
                    if (arrayDelGroupId.Count > 0)
                    {
                        List<int> filterGroup = listIdGroup.AsQueryable().Where(s => !arrayDelGroupId.Contains(s)).ToList();
                        await _permissionUserMenuServices.AddPermissionRoleUserMenuDefault(idUser, idUserCreated, filterGroup);
                    }
                    else if (groupAdds.Count > 0)
                    {
                        await _permissionUserMenuServices.AddPermissionRoleUserMenuDefault(idUser, idUserCreated, listIdGroup);
                    }   
                }
                else
                {
                    var addNeedGroups = await _db.Groups.Where(s => s.IsDeleted == 0 && listIdGroup.Contains(s.Id)).ToListAsync();
                    foreach (var item in addNeedGroups)
                    {
                        var addUserGroup = new User_Group
                        {
                            idUser = idUser,
                            idGroup = item.Id,
                            userCreated = idUserCreated,
                            dateCreated = DateTime.Now,
                            isDeleted = false,
                        };
                        await _db.UserGroups.AddAsync(addUserGroup);
                        data.Add(addUserGroup);
                    }
                    await _db.SaveChangesAsync();
                    await _permissionUserMenuServices.AddPermissionRoleUserMenuDefault(idUser, idUserCreated, listIdGroup);
                }
                success = true;
                message = "Updating UserGroup successfully";
                return new BaseResponse<List<User_Group>>(success, message, data.Distinct().ToList());
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Updating UserGroup failed! {ex.Message}";
                return new BaseResponse<List<User_Group>>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<User_Group>>> UpdateUserGroupMultiRoleUpgrade(int idUser, int idUserUpdated, List<int> listIdGroup)
        {
            var success = false;
            var message = "";
            var data = new List<User_Group>();
            try
            {
                // get all list group
                var userOldGroup = await _db.UserGroups.Where(s => s.isDeleted == false && s.idUser.Equals(idUser)).ToListAsync();
                var arrayDelGroupId = new List<int>();

                if (userOldGroup.Count > 0)
                {
                    var getGroupHaveId = userOldGroup.Where(s => listIdGroup.Contains(s.idGroup)).ToList();
                    var getGroupHaveNotId = userOldGroup.Where(s => !listIdGroup.Contains(s.idGroup)).ToList();
                    arrayDelGroupId = getGroupHaveNotId.Select(s => s.idGroup).ToList();
                    
                    if (arrayDelGroupId.Count > 0)
                    {
                        await _permissionUserMenuServices.DeletePermissionRoleUserMenuUpgrade(idUser, idUserUpdated, arrayDelGroupId);
                    }                                     
                    var groupAdds = await _db.Groups.Where(s => listIdGroup.Contains(s.Id) && !getGroupHaveId.Select(x => x.idGroup).ToList().Contains(s.Id)).ToListAsync();
                    var filterGroup = groupAdds.Select(s => s.Id).ToList(); 
                    if (groupAdds.Count > 0)
                    {
                        var createNewUserGroup = new List<UserGroupCreatedDto>();
                        foreach (var item in groupAdds)
                        {
                            var userGroup = new UserGroupCreatedDto
                            {
                                idUser = idUser,
                                idGroup = item.Id,
                                userCreated = idUserUpdated,
                            };
                            createNewUserGroup.Add(userGroup);
                        }
                        var resultCreateUserGroup = await CreateUserGroup(createNewUserGroup);
                        if (resultCreateUserGroup._success)
                        {
                            data.AddRange(resultCreateUserGroup._Data);
                            await _permissionUserMenuServices.AddPermissionRoleUserMenuUpgrade(idUser, idUserUpdated, filterGroup);
                        }
                    }
                    await _db.SaveChangesAsync();
                }
                else
                {
                    var addNeedGroups = await _db.Groups.Where(s => s.IsDeleted == 0 && listIdGroup.Contains(s.Id)).ToListAsync();
                    var createNewUserGroup = new List<UserGroupCreatedDto>();
                    foreach (var item in addNeedGroups)
                    {
                        var addUserGroup = new UserGroupCreatedDto
                        {
                            idUser = idUser,
                            idGroup = item.Id,
                            userCreated = idUserUpdated,
                        };
                        createNewUserGroup.Add(addUserGroup);
                    }
                    var resultCreateUserGroup = await CreateUserGroup(createNewUserGroup);
                    if (resultCreateUserGroup._success)
                    {
                        data.AddRange(resultCreateUserGroup._Data);
                    }
                    await _db.SaveChangesAsync();
                    await _permissionUserMenuServices.AddPermissionRoleUserMenuUpgrade(idUser, idUserUpdated, listIdGroup);
                }
                success = true;
                message = "Updating UserGroup successfully";
                return new BaseResponse<List<User_Group>>(success, message, data.Distinct().ToList());
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Updating UserGroup failed! {ex.Message}";
                return new BaseResponse<List<User_Group>>(success, message, data);
            }
        }

        public async Task<BaseResponse<User_Group>> DeleteUserGroup(UserGroupDeletedDto userGroupDeletedDto)
        {
            var success = false;
            var message = "";
            var data = new User_Group();
            try
            {
                var userGroup = await _db.UserGroups.Where(s => s.isDeleted == false && s.idUser.Equals(userGroupDeletedDto.idUser) && s.idGroup.Equals(userGroupDeletedDto.idGroup)).FirstOrDefaultAsync();
                if (userGroup is null)
                {
                    success = false;
                    message = "UserGroup doesn't exist !";
                    return new BaseResponse<User_Group>(success, message, data);
                }

                var userGroupMapData = _mapper.Map<UserGroupDeletedDto, User_Group>(userGroupDeletedDto, userGroup);
                userGroupMapData.dateDeleted = DateTime.Now;
                userGroupMapData.isDeleted = true;
                _db.UserGroups.Update(userGroupMapData);
                await _db.SaveChangesAsync();

                success = true;
                message = "Deleting UserGroup successfully";
                return new BaseResponse<User_Group>(success, message, userGroupMapData);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting UserGroup failed! {ex.InnerException}";
                return new BaseResponse<User_Group>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<User_Group>>> DeleteMultiUserGroup(List<UserGroupDeletedDto> userGroupDeletedDto)
        {
            var success = false;
            var message = "";
            var data = new List<User_Group>();
            try
            {
                foreach (var item in userGroupDeletedDto)
                {
                    var result = await DeleteUserGroup(item);
                    if (result._success)
                    {
                        data.Add(result._Data);
                    }
                }
                success = true;
                message = "Deleting Multi UserGroup successfully";
                return new BaseResponse<List<User_Group>>(success, message, data.Distinct().ToList());
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Multi UserGroup failed! {ex.InnerException}";
                return new BaseResponse<List<User_Group>>(success, message, data);
            }
        }


    }
}
