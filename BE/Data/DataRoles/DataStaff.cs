using BE.Data.Contexts;
using BE.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BE.Data.DataRoles
{
    public interface IDataStaff
    {
        List<Permission_Use_Menu> RoleStaff(int idUser, int idGroup, int idUserCreated = 0);
    }

    public class DataStaff : IDataStaff
    {
        private readonly AppDbContext _db;
        public DataStaff(AppDbContext db)
        {
            _db = db;
        }

        public List<Permission_Use_Menu> RoleStaff(int idUser, int idGroup, int idUserCreated = 0)
        {
            var data = new List<Permission_Use_Menu>();
            //var modules = await _db.modules.Where(s => s.isDeleted == 0).ToListAsync();
            var modules = (from m in _db.modules
                           join pg in _db.Permission_Groups on m.id equals pg.IdModule
                           where pg.Access == true && pg.IdGroup == idGroup
                           select new
                           {
                               nameModule = m.nameModule,
                               idModule = pg.IdModule,
                           }).ToList();

            foreach (var item in modules)
            {
                Permission_Use_Menu permissonUser;
                if (item.nameModule.ToLower().Equals("ots"))
                {
                    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, idUserCreated);
                }
                else if (item.nameModule.ToLower().Equals("leaveoffs"))
                {
                    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, idUserCreated);
                }
                else if (item.nameModule.ToLower().Equals("users"))
                {
                    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                }
                else if (item.nameModule.ToLower().Equals("paids"))
                {
                    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, idUserCreated);
                }
                else if (item.nameModule.ToLower().Equals("rules"))
                {
                    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, idUserCreated);
                }


                //else if (item.nameModule.ToLower().Equals("groups"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("remotes"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("equipments"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("modules"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("menus"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("permissiongroups"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("customers"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("paidresons"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("projects"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("actionmodule"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("permissionactionmodule"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}


                //else if (item.nameModule.ToLower().Equals("usergroups"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("actionModules"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("handovers"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("infodevices"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("blockingwebs"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("devices"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("devicesrepos"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("memberprojects"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("notifications"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("orders"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("tasks"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("wikicates"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
                //else if (item.nameModule.ToLower().Equals("wikiposts"))
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}

                else
                {
                    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                }
                data.Add(permissonUser);
            }
            return data;
        }

    }
}
