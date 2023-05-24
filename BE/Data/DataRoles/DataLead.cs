using BE.Data.Contexts;
using BE.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BE.Data.DataRoles
{
    public interface IDataLead
    {
        List<Permission_Use_Menu> RoleLead(int idUser, int idGroup, int idUserCreated = 0);
    }

    public class DataLead : IDataLead
    {
        private readonly AppDbContext _db;
        public DataLead(AppDbContext db)
        {
            _db = db;
        }

        public List<Permission_Use_Menu> RoleLead(int idUser, int idGroup, int idUserCreated = 0)
        {
            var data = new List<Permission_Use_Menu>();
            var modules = (from pg in _db.Permission_Groups
                           join m in _db.modules on pg.IdModule equals m.id
                           where pg.Access == true && pg.IdGroup == idGroup
                           select new
                           {
                               idModule = pg.IdModule,
                               nameModule = m.nameModule,
                           }).ToList();

            foreach (var item in modules)
            {
                Permission_Use_Menu permissonUser;
                
                if (item.nameModule.ToLower().Equals("ots")) //5
                {
                    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, idUserCreated);
                }
                else if (item.nameModule.ToLower().Equals("leaveoffs"))//8
                {
                    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, idUserCreated);
                }
                else if (item.nameModule.ToLower().Equals("projects")) //1
                {
                    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, idUserCreated);
                }
                else if (item.nameModule.ToLower().Equals("users"))//2
                {
                    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                }
                else if (item.nameModule.ToLower().Equals("paids")) //13
                {
                    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, idUserCreated);
                }
                else if (item.nameModule.ToLower().Equals("memberprojects")) //26
                {
                    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, idUserCreated);
                }
                else if (item.nameModule.ToLower().Equals("rules")) //14
                {
                    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, idUserCreated);
                }
                else if (item.nameModule.ToLower().Equals("tasks")) //32
                {
                    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                }

                //else if (item.nameModule.ToLower().Equals("notifications"))//27
                //{
                //    permissonUser = new Permission_Use_Menu(item.idModule, idUser, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, idUserCreated);
                //}
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
                //else if (item.nameModule.ToLower().Equals("actionmodules"))
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

                //else if (item.nameModule.ToLower().Equals("orders"))
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
