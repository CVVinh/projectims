using BE.Data.Contexts;
using BE.Data.Dtos.ProjectDtos;
using BE.Data.Models;
using BE.Services.PaginationServices;
using BE.Services.TokenServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE.Data.Dtos.MenuDtos;
using BE.Data.Dtos.Handover;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using BE.Data.Enum;
using System;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: menus")]
    public class MenuController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPaginationServices<ListMenuDtos> _paginationService;

        public MenuController(AppDbContext context, IPaginationServices<ListMenuDtos> paginationService)
        {
            _context = context;
            _paginationService = paginationService;
        }

        [HttpGet]
        [Route("getListMenuModule")]
        public async Task<IActionResult> getListMenuModule()
        {
            try
            {
                var menuModule = await _context.modules.OrderBy(m => m.idSort).ToListAsync();
                return Ok(menuModule);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("getListMenuParent")]
        public async Task<ActionResult> getListMenuParent()
        {
            try
            {
                var menuParent = await (from menu in _context.Menus.Where(a => a.parent == 0)
                                        from modules in _context.modules.Where(a => a.id == menu.idModule).DefaultIfEmpty()
                                        select new
                                        {
                                            action = menu.action,
                                            controller = menu.controller,
                                            icon = menu.icon,
                                            id = menu.id,
                                            idModule = menu.idModule,
                                            isDeleted = menu.isDeleted,
                                            parent = menu.parent,
                                            title = menu.title,
                                            view = menu.view,
                                            idSort = modules.idSort,
                                        }).OrderBy(a => a.idSort).ToListAsync();

                return Ok(menuParent);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("getListMenu")]
        public async Task<ActionResult> getListMenu(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var menu = await _context.Menus
                    .Join(_context.modules,
                    menu1 => menu1.idModule, modules => modules.id,
                    (menu1, modules) => new { menu1, modules })
                    .Select(x => new ListMenuDtos()
                    {
                        menu1 = x.menu1,
                        nameModule = x.modules.nameModule,
                        note = x.modules.note,

                    })
                    .ToListAsync();
                if (menu.Count() != 0)
                {
                    var pageSize = (int)pageSizeEnum;
                    var resultPage = await _paginationService.paginationListTableAsync(menu, pageIndex, pageSize);
                    if (resultPage._success)
                    {
                        return Ok(resultPage);
                    }
                    return BadRequest(resultPage);
                }
                return NotFound("Khong co du lieu");
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("getMenuByModule")]
        public async Task<IActionResult> GetMenuByModule(int moduleId)
        {
            try
            {
                return Ok(await _context.Menus.AsNoTracking().Where(u => u.idModule == moduleId).ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getlistMenubyIdModoule/{ad}")]
        public async Task<ActionResult> getlistMenubyIdModoule(int ad)
        {
            try
            {
                var menuParent = await _context.Menus.Where(x => x.idModule == ad && x.isDeleted == 0).ToListAsync();

                return Ok(menuParent);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("getlistmenuID/{id}")]
        public async Task<ActionResult> getlistmenuID(int id)
        {
            try
            {
                var menuParent = await _context.Menus.FirstOrDefaultAsync(x => x.id == id);

                return Ok(menuParent);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetlistMenuByIdModule/{idmodule}")]
        public async Task<ActionResult> GetlistMenuByIdModule(int idmodule)
        {
            try
            {
                var menuParent = await _context.Menus.Where(x => x.idModule == idmodule).ToListAsync();

                return Ok(menuParent);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetlistMenuByIdModuleedit/{idmodule}/{idmenu}")]
        public async Task<ActionResult> GetlistMenuByIdModuleedit(int idmodule, int idmenu)
        {
            try
            {
                var menuParent = await _context.Menus.Where(x => x.idModule == idmodule && x.id != idmenu && x.isDeleted == 0).ToListAsync();

                return Ok(menuParent);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("addMenu")]
        [Authorize(Roles = "module:menus action:add")]
        public async Task<IActionResult> addMenu(addMenuDtos request)
        {
            try
            {
                var menu = new Menu();
                //menu.id = request.id;
                menu.title = request.title;
                menu.idModule = request.idModule;
                menu.controller = request.controller;
                menu.view = request.view;
                menu.icon = request.icon;
                menu.action = request.action;
                menu.parent = request.parent;
                menu.isDeleted = 0;
                _context.Menus.Add(menu);
                await _context.SaveChangesAsync();

                return Ok("Sucess");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("updateMenu")]
        [Authorize(Roles = "module:menus action:update")]
        public async Task<ActionResult> updateMenu(updateMenuDtos requests)
        {
            try
            {
                var acc = await _context.Menus.FindAsync(requests.id);
                if (acc == null)
                {
                    return NotFound();
                }
                acc.title = requests.title;
                acc.controller = requests.controller;
                acc.view = requests.view;
                acc.action = requests.action;
                acc.parent = requests.parent;
                acc.icon = requests.icon;
                acc.idModule = requests.idModule;
                await _context.SaveChangesAsync();
                return Ok("success");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }

        [HttpPut]
        [Route("deleteMenu/{idmenu}")]
        [Authorize(Roles = "module:menus action:delete")]
        public async Task<IActionResult> deleteMenu(int idmenu)
        {
            try
            {
                var menu = await _context.Menus.SingleOrDefaultAsync(p => p.id == idmenu);

                if (menu == null)
                {
                    return NotFound();
                }
                else
                {
                    var child = await _context.Menus.Where(pa => pa.parent == idmenu && pa.isDeleted == 0).ToListAsync();
                    if (child.Count != 0)
                    {
                        return BadRequest("Not found");
                    }
                    else
                    {
                        menu.isDeleted = 1;
                        _context.SaveChanges();
                        return Ok("success");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("SearchMenuByNameOrAction")]
        public async Task<ActionResult> SearchMenuByNameOrAction(string? name, int? idModule, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            try
            {
                var menu = _context.Menus
                    .Join(_context.modules, menu1 => menu1.idModule, modules => modules.id, (menu1, modules) => new { menu1, modules })
                    .Select(x => new ListMenuDtos()
                    {
                        menu1 = x.menu1,
                        nameModule = x.modules.nameModule,
                        note = x.modules.note,

                    }).AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    menu = menu.Where(s => s.menu1.title.Trim().ToLower().Contains(name.Trim().ToLower()) || s.menu1.view.Trim().ToLower().Contains(name.Trim().ToLower()));
                }
                if (idModule != 0)
                {
                    menu = menu.Where(s => s.menu1.idModule.Equals(idModule));
                }

                if (menu.Count() != 0)
                {
                    var pageSize = (int)pageSizeEnum;
                    var resultPage = await _paginationService.paginationListTableAsync(menu.ToList(), pageIndex, pageSize);
                    if (resultPage._success)
                    {
                        return Ok(resultPage);
                    }
                    return BadRequest(resultPage);
                }
                return NotFound("Không có dữ liệu");
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
