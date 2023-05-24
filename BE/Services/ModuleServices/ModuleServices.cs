using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.GruopDtos;
using BE.Data.Dtos.ModuleDtos;
using BE.Data.Models;
using BE.Response;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;

namespace BE.Services.ModuleServices
{
    public interface IModuleServices
    {
        Task<BaseResponse<List<Module>>> GetAllModuleAsync();
        Task<BaseResponse<Module>> GetModuleById(int id);
        Task<BaseResponse<List<Module>>> GetModuleAccessByIdGroup(int groupId);
        Task<BaseResponse<Module>> CreateModule(ModuleDtos moduleDtos);
        Task<BaseResponse<Module>> UpdateModule(int id, ModuleDtos moduleDtos);
        Task<BaseResponse<Module>> DeleteModule(int id);
        Task<BaseResponse<List<Module>>> DeleteMultiModule(List<int> listIdModule);
        Task<BaseResponse<List<Module>>> FilterModuleByName(string? name);
    }

    public class ModuleServices : IModuleServices
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public ModuleServices(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<Module>>> GetAllModuleAsync()
        {
            var success = false;
            var message = "";
            var data = new List<Module>();
            try
            {
                var module = await _db.modules.Where(s => s.isDeleted == 0).OrderBy(s => s.idSort).ToListAsync();
                success = true;
                message = "Get all data successfully";
                data.AddRange(module);
                return (new BaseResponse<List<Module>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Module>>(success, message, data));
            }
        }

        public async Task<BaseResponse<Module>> GetModuleById(int id)
        {
            var success = false;
            var message = "";
            try
            {
                var module = await _db.modules.OrderByDescending(s => s.idSort).Where(s => s.isDeleted == 0 && s.id.Equals(id)).FirstOrDefaultAsync();
                success = true;
                message = "Get data successfully";
                return (new BaseResponse<Module>(success, message, module));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<Module>(success, message, new Module()));
            }
        }

        public async Task<BaseResponse<List<Module>>> GetModuleAccessByIdGroup(int groupId)
        {
            var success = false;
            var message = "";
            var data = new List<Module>();
            try
            {
                var permissionModuleGroups = (from md in _db.modules
                                              join pg in _db.Permission_Groups on md.id equals pg.IdModule
                                              where pg.IdGroup == groupId && pg.Access == true
                                              orderby md.idSort ascending
                                              select md).ToList();

                success = true;
                message = "Get data successfully";
                return (new BaseResponse<List<Module>>(success, message, permissionModuleGroups));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Module>>(success, message, data));
            }
        }

        public async Task<BaseResponse<Module>> CreateModule(ModuleDtos moduleDtos)
        {
            var success = false;
            var message = "";
            try
            {
                var module = await _db.modules.Where(s => s.nameModule.Equals(moduleDtos.nameModule.ToLower())).FirstOrDefaultAsync();
                if (module == null)
                {
                    module = _mapper.Map<Module>(moduleDtos);
                    await _db.modules.AddAsync(module);
                }
                else
                {
                    module = _mapper.Map<ModuleDtos, Module>(moduleDtos, module);
                    _db.modules.Update(module);
                }
                module.isDeleted = 0;
                await _db.SaveChangesAsync();

                success = true;
                message = "Add new Module successfully";
                return new BaseResponse<Module>(success, message, module);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Adding new Module failed! {ex.Message}";
                return new BaseResponse<Module>(success, message, new Module());
            }
        }

        public async Task<BaseResponse<Module>> UpdateModule(int id, ModuleDtos moduleDtos)
        {
            var success = false;
            var message = "";
            try
            {
                var module = await _db.modules.Where(s => s.isDeleted == 0 && s.id.Equals(id)).FirstOrDefaultAsync();
                if (module is null)
                {
                    message = "Module doesn't exist !";
                    return new BaseResponse<Module>(success, message, new Module());
                }
                var moduleMapData = _mapper.Map<ModuleDtos, Module>(moduleDtos, module);
                _db.modules.Update(moduleMapData);
                await _db.SaveChangesAsync();

                success = true;
                message = "Editing Module successfully";
                return new BaseResponse<Module>(success, message, moduleMapData);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Editing Module failed! {ex.Message}";
                return new BaseResponse<Module>(success, message, new Module());
            }
        }

        public async Task<BaseResponse<Module>> DeleteModule(int id)
        {
            var success = false;
            var message = "";
            try
            {
                var module = await _db.modules.Where(s => s.isDeleted == 0 && s.id.Equals(id)).FirstOrDefaultAsync();
                if (module is null)
                {
                    message = "Group doesn't exist !";
                    return new BaseResponse<Module>(success, message, new Module());
                }

                module.isDeleted = 1;
                _db.modules.Update(module);
                await _db.SaveChangesAsync();

                success = true;
                message = "Deleting Module successfully";
                return new BaseResponse<Module>(success, message, module);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Module failed! {ex.InnerException}";
                return new BaseResponse<Module>(success, message, new Module());
            }
        }

        public async Task<BaseResponse<List<Module>>> DeleteMultiModule(List<int> listIdModule)
        {
            var success = false;
            var message = "";
            var data = new List<Module>();
            try
            {
                foreach (var item in listIdModule)
                {
                    var module = await _db.modules.Where(s => s.isDeleted == 0 && s.id.Equals(item)).FirstOrDefaultAsync();
                    if (module != null)
                    {
                        module.isDeleted = 1;
                        _db.modules.Update(module);
                        data.Add(module);
                    }
                    else
                    {
                        message = item + " group doesn't exist !";
                        return new BaseResponse<List<Module>>(success, message, data = null);
                    }
                }
                await _db.SaveChangesAsync();

                success = true;
                message = "Deleting Multi Module successfully";
                return new BaseResponse<List<Module>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Multi Module failed! {ex.InnerException}";
                return new BaseResponse<List<Module>>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Module>>> FilterModuleByName(string? name)
        {
            var success = false;
            var message = "";
            var data = new List<Module>();
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    var module = await _db.modules
                        .Where(s => s.isDeleted == 0 && s.nameModule.Trim().ToLower().Contains(name.Trim().ToLower()))
                        .OrderBy(s => s.idSort).ToListAsync();
                    success = true;
                    message = "Lộc dữ liệu thành công";
                    data.AddRange(module);
                    return (new BaseResponse<List<Module>>(success, message, data));
                }
                success = false;
                message = "Lộc dữ liệu không thành công";
                return (new BaseResponse<List<Module>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Module>>(success, message, data));
            }
        }
    }

}
