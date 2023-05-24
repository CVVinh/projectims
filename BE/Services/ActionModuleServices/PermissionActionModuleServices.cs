using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.ActionModuleDtos;
using BE.Data.Dtos.PermissionActionModuleDtos;
using BE.Data.Dtos.UserDtos;
using BE.Data.Models;
using BE.Response;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System.Reflection;

namespace BE.Services.ActionModuleServices
{
    public interface IPermissionActionModuleServices
    {
        Task<BaseResponse<List<Permission_Action_Module>>> GetAllPermissionActionModuleAsync();
        Task<BaseResponse<List<GetAllPermissionActionModuleDto>>> GetNameAllPermissionActionModule();
        Task<BaseResponse<List<Permission_Action_Module>>> GetPermissionActionModuleWithModuleId(int moduleId);
        Task<BaseResponse<List<Permission_Action_Module>>> GetPermissionActionModuleWithActionId(int actionId);
        Task<BaseResponse<List<Permission_Action_Module>>> CreatePermissionActionModule(List<AddPermissionActionModuleDto> addPermissionActionModuleDtos);
        Task<BaseResponse<Permission_Action_Module>> UpdateUPermissionActionModule(RequestPermissionActionModuleDto requestPermissionActionModuleDto, UpdatePermissionActionModuleDto updatePermissionActionModuleDto);
        Task<BaseResponse<Permission_Action_Module>> DeletePermissionActionModule(RequestPermissionActionModuleDto requestPermissionActionModuleDto, DeletePermissionActionModuleDto deletePermissionActionModuleDto);
        Task<BaseResponse<List<Permission_Action_Module>>> DeleteMultiPermissionActionModule(List<DeleteMultiPermissionActionModuleDto> deleteMultiPermissionActionModuleDtos);
    
    }

    public class PermissionActionModuleServices : IPermissionActionModuleServices
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public PermissionActionModuleServices(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<Permission_Action_Module>>> GetAllPermissionActionModuleAsync()
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Action_Module>();
            try
            {
                var permissionActionModule = await _db.PermissionActionModules.Where(s => s.isDeleted == false).OrderBy(s => s.actionModule.id).Include(s => s.module).Select(s => new Permission_Action_Module
                {
                    id = s.id,
                    moduleId= s.moduleId,
                    actionModuleId= s.actionModuleId,
                    module = s.module,
                    actionModule= s.actionModule,
                    
                }).ToListAsync();
                success = true;
                message = "Get all data successfully";
                data.AddRange(permissionActionModule);
                return (new BaseResponse<List<Permission_Action_Module>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Action_Module>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<GetAllPermissionActionModuleDto>>> GetNameAllPermissionActionModule()
        {
            var success = false;
            var message = "";
            List<GetAllPermissionActionModuleDto> data = new List<GetAllPermissionActionModuleDto>();
            try
            {
                var permissionActionModules = await _db.modules.Where(s => s.isDeleted == 0).Select(s => new GetAllPermissionActionModuleDto
                {
                    //id = pam.id,
                    moduleId = s.id,
                    nameModule = s.nameModule,
                    note = s.note,

                }).ToListAsync();

                //var permissionActionModules = await (from pam in _db.PermissionActionModules
                //                              join md in _db.modules on pam.moduleId equals md.id
                //                              where pam.isDeleted == false && md.isDeleted == 0
                //                              select new GetAllPermissionActionModuleDto
                //                              {
                //                                  //id = pam.id,
                //                                  moduleId = pam.moduleId,
                //                                  nameModule = md.nameModule,
                //                                  note = md.note,
                                                 
                //                              }).Distinct().OrderBy(x => x.moduleId).ToListAsync();

                foreach (var item in permissionActionModules)
                {
                    item.actionModule = await (_db.PermissionActionModules.Where(s => s.isDeleted == false && s.moduleId == item.moduleId).Include(s => s.actionModule).Select(s => new ResponsePermissionActionModuleDto
                    {
                        id = s.id,
                        actionModuleId = s.actionModule.id,
                        name = s.actionModule.name,
                        description = s.actionModule.description,
                    })).OrderBy(x => x.actionModuleId).ToListAsync();
                }
                success = true;
                message = "Get all data successfully";
                data.AddRange(permissionActionModules);
                return (new BaseResponse<List<GetAllPermissionActionModuleDto>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<GetAllPermissionActionModuleDto>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Action_Module>>> GetPermissionActionModuleWithModuleId(int moduleId)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Action_Module>();
            try
            {
                var permissionActionModule = await _db.PermissionActionModules.OrderBy(s => s.actionModule.id).Where(s => s.isDeleted == false && s.moduleId.Equals(moduleId)).Include(s => s.module).Select(s => new Permission_Action_Module
                {
                    id = s.id,
                    moduleId = s.moduleId,
                    actionModuleId = s.actionModuleId,
                    module = s.module,
                    actionModule = s.actionModule,

                }).ToListAsync();
                success = true;
                message = "Get data successfully";
                data.AddRange(permissionActionModule);
                return (new BaseResponse<List<Permission_Action_Module>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Action_Module>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Action_Module>>> GetPermissionActionModuleWithActionId(int actionId)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Action_Module>();
            try
            {
                var permissionActionModule = await _db.PermissionActionModules.OrderBy(s => s.actionModule.id).Where(s => s.isDeleted == false && s.actionModuleId.Equals(actionId)).Include(s => s.module).Select(s => new Permission_Action_Module
                {
                    id = s.id,
                    moduleId = s.moduleId,
                    actionModuleId = s.actionModuleId,
                    module = s.module,
                    actionModule = s.actionModule,

                }).ToListAsync(); ;
                success = true;
                message = "Get data successfully";
                data.AddRange(permissionActionModule);
                return (new BaseResponse<List<Permission_Action_Module>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Action_Module>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Action_Module>>> CreatePermissionActionModule(List<AddPermissionActionModuleDto> addPermissionActionModuleDtos)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Action_Module>();
            try
            {
                foreach (var item in addPermissionActionModuleDtos)
                {
                    var permissionActionModule = await _db.PermissionActionModules.Where(s => s.moduleId.Equals(item.moduleId) && s.actionModuleId.Equals(item.actionModuleId)).FirstOrDefaultAsync();
                    if (permissionActionModule is null)
                    {
                        permissionActionModule = _mapper.Map<Permission_Action_Module>(item);
                        await _db.PermissionActionModules.AddAsync(permissionActionModule);
                    }
                    else
                    {
                        permissionActionModule = _mapper.Map<AddPermissionActionModuleDto, Permission_Action_Module>(item, permissionActionModule);
                        _db.PermissionActionModules.Update(permissionActionModule);
                    }
                    permissionActionModule.dateCreated = DateTime.Now;
                    permissionActionModule.isDeleted = false;
                    data.Add(permissionActionModule);
                }

                await _db.SaveChangesAsync();
                success = true;
                message = "Add new Permission_Action_Module successfully";
                return new BaseResponse<List<Permission_Action_Module>>(success, message, data.Distinct().ToList());
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Adding new Permission_Action_Module failed! {ex.Message}";
                return new BaseResponse<List<Permission_Action_Module>>(success, message, data);
            }
        }

        public async Task<BaseResponse<Permission_Action_Module>> UpdateUPermissionActionModule(RequestPermissionActionModuleDto requestPermissionActionModuleDto, UpdatePermissionActionModuleDto updatePermissionActionModuleDto)
        {
            var success = false;
            var message = "";
            var data = new Permission_Action_Module();
            try
            {
                var permissionActionModule = await _db.PermissionActionModules.Where(s => s.isDeleted == false && s.moduleId.Equals(requestPermissionActionModuleDto.moduleId) && s.actionModuleId.Equals(requestPermissionActionModuleDto.actionModuleId)).FirstOrDefaultAsync();
                
                if (permissionActionModule is null)
                {
                    message = "Permission_Action_Module doesn't exist !";
                    return new BaseResponse<Permission_Action_Module>(success, message, data = null);
                }

                var permissionActionModuleMapData = _mapper.Map<UpdatePermissionActionModuleDto, Permission_Action_Module>(updatePermissionActionModuleDto, permissionActionModule);

                permissionActionModuleMapData.dateUpdated = DateTime.Now;
                _db.PermissionActionModules.Update(permissionActionModuleMapData);
                await _db.SaveChangesAsync();

                success = true;
                message = "Editing Permission_Action_Module successfully";
                data = permissionActionModuleMapData;
                return new BaseResponse<Permission_Action_Module>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Editing Permission_Action_Module failed! {ex.Message}";
                return new BaseResponse<Permission_Action_Module>(success, message, data = null);
            }
        }

        public async Task<BaseResponse<Permission_Action_Module>> DeletePermissionActionModule(RequestPermissionActionModuleDto requestPermissionActionModuleDto, DeletePermissionActionModuleDto deletePermissionActionModuleDto)
        {
            var success = false;
            var message = "";
            var data = new Permission_Action_Module();
            try
            {
                var permissionActionModule = await _db.PermissionActionModules.Where(s => s.isDeleted == false && s.moduleId.Equals(requestPermissionActionModuleDto.moduleId) && s.actionModuleId.Equals(requestPermissionActionModuleDto.actionModuleId)).FirstOrDefaultAsync();
                if (permissionActionModule is null)
                {
                    message = "Permission_Action_Module doesn't exist !";
                    return new BaseResponse<Permission_Action_Module>(success, message, data);
                }

                var permissionActionModuleMapData = _mapper.Map<DeletePermissionActionModuleDto, Permission_Action_Module>(deletePermissionActionModuleDto, permissionActionModule);
                permissionActionModuleMapData.dateDeleted = DateTime.Now;
                permissionActionModuleMapData.isDeleted = true;
                _db.PermissionActionModules.Update(permissionActionModuleMapData);
                await _db.SaveChangesAsync();

                success = true;
                message = "Deleting Permission_Action_Module successfully";
                return new BaseResponse<Permission_Action_Module>(success, message, permissionActionModuleMapData);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Permission_Action_Module failed! {ex.InnerException}";
                return new BaseResponse<Permission_Action_Module>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Permission_Action_Module>>> DeleteMultiPermissionActionModule(List<DeleteMultiPermissionActionModuleDto> deleteMultiPermissionActionModuleDtos)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Action_Module>();
            try
            {
                foreach (var item in deleteMultiPermissionActionModuleDtos)
                {
                    var result = await _db.PermissionActionModules.Where(s => s.isDeleted == false && s.moduleId.Equals(item.moduleId) && s.actionModuleId.Equals(item.actionModuleId)).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        var permissionActionModuleMapData = _mapper.Map<DeleteMultiPermissionActionModuleDto, Permission_Action_Module>(item, result);
                        permissionActionModuleMapData.dateDeleted = DateTime.Now;
                        permissionActionModuleMapData.isDeleted = true;
                        _db.PermissionActionModules.Update(permissionActionModuleMapData);
                        data.Add(result);
                    }
                }
                await _db.SaveChangesAsync();
                success = true;
                message = "Deleting Multi Permission_Action_Module successfully";
                return new BaseResponse<List<Permission_Action_Module>>(success, message, data.Distinct().ToList());
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Multi Permission_Action_Module failed! {ex.InnerException}";
                return new BaseResponse<List<Permission_Action_Module>>(success, message, data);
            }
        }


    }
}
