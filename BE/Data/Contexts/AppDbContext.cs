using System.Reflection;
using BE.Data.Enums.TaskEnums;
using BE.Data.Models;
using Microsoft.EntityFrameworkCore;
using BE.Helpers;
using BE.Data.Enum.Wiki;
using static BE.Data.Enum.Handover.Status;
using Module = BE.Data.Models.Module;
using BE.Data.Configurations;

namespace BE.Data.Contexts
{
    public class AppDbContext : DbContext
    {
        #region Constructor
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        #endregion

        #region Property
        public DbSet<Tasks> tasks { get; set; }
        public DbSet<OTs> OTs { get; set; }
        public DbSet<Roles> roles { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Projects> Projects { get; set; }
        public DbSet<Member_Project> Member_Projects { get; set; }
        public DbSet<Wiki_CateGogy> Wiki_Categogy { get; set; }
        public DbSet<Wiki_Post> Wiki_Post { get; set; }
        public DbSet<LeaveOff> leaveOffs { get; set; }
        public DbSet<Devices> Devices { get; set; }
        public DbSet<Handover> Handovers { get; set; }
        public DbSet<Branchs> Branchs { get; set; }


        public DbSet<Orders> Orders { get; set; }

        //phan quyen use menu
        public DbSet<Group> Groups { get; set; }
        public DbSet<Permission_Use_Menu> Permission_Use_Menus { get; set; }


        public DbSet<Menu> Menus { get; set; }
        public DbSet<Module> modules { get; set; }

        public DbSet<Permission_Group> Permission_Groups { get; set; }
        public DbSet<Paids> Paids { get; set; }
        public DbSet<PaidImage> PaidImages { get; set; }
        public DbSet<Rules> Rules { get; set; }
        public DbSet<InfoDevices> InfoDevices { get; set; }
        public DbSet<DeviceInstalledApps> DeviceInstalledApps { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<PaidReasons> PaidReasons { get; set; }
        public DbSet<Customer_PaidReason> CustomerPaidReasons { get; set; }
        public DbSet<User_Group> UserGroups { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Action_Module> ActionModules { get; set; }
        public DbSet<Permission_Action_Module> PermissionActionModules { get; set; }
        public DbSet<BlockingWeb> BlockingWebs { get; set; }
        public DbSet<StaffReview> StaffReviews { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<ReviewResult> ReviewResults { get; set; }
        public DbSet<Group_Module_Action> GroupModuleActions { get; set; }
        public DbSet<PostImages>  ImagePosts { get; set; }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<PostCategories> Categories { get; set; }
        public DbSet<PostComments> Comments { get; set; }
        public DbSet<StaffReviewTime> StaffReviewTimes { get; set; }
        public DbSet<TimeSheetDaily> TimeSheetDailys { get; set; }
        public DbSet<Firebase_Token> Firebase_Token { get; set; }


        #endregion

        #region Method
        // Use Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Users>(e =>
            {
                e.ToTable("Users").HasIndex(k => k.userCode).IsUnique();
                e.HasKey(k => k.id);
                e.Property(k => k.userCode).IsRequired().HasMaxLength(30).HasColumnType("varchar");
                e.Property(k => k.userPassword).IsRequired().HasMaxLength(1000).HasColumnType("varchar");
                e.Property(k => k.isDeleted);
                e.Property(k => k.firstName).HasMaxLength(50).HasColumnType("varchar");
                e.Property(k => k.lastName).HasMaxLength(50).HasColumnType("varchar");
                e.Property(k => k.phoneNumber).HasMaxLength(11);
                e.Property(k => k.dOB).HasColumnType("date");
                e.Property(k => k.address).HasMaxLength(200).HasColumnType("varchar");
                e.Property(k => k.university).HasMaxLength(100).HasColumnType("varchar");
                e.Property(k => k.email).HasMaxLength(50);
                e.Property(k => k.emailPassword).HasMaxLength(1000);
                e.Property(k => k.skype).HasMaxLength(100);
                e.Property(k => k.skypePassword).HasMaxLength(1000);
                e.Property(k => k.workStatus);
                e.Property(k => k.dateStartWork).HasDefaultValue(DateTime.Now).HasColumnType("date");
                e.Property(k => k.dateLeave).HasColumnType("date");
                e.Property(k => k.maritalStatus);
                e.Property(k => k.isDeleted).HasDefaultValue(0);
                e.Property(k => k.reasonResignation).HasMaxLength(1000);
                e.Property(k => k.rank).HasMaxLength(100).HasColumnType("varchar");
                e.Property(k => k.idUserGitLab).IsRequired();
                e.Property(k => k.tokenGitLab).IsRequired();
            });

            modelBuilder.Entity<Member_Project>(e =>
            {
                e.ToTable("Member_Project");
                e.HasKey(e => e.Id);
                e.Property(e => e.isDeleted).HasDefaultValue(false);
                e.Property(e => e.createDate).HasColumnType("timestamp");
                e.Property(e => e.createUser).IsRequired();
                e.Property(e => e.member).IsRequired();
                e.Property(e => e.idProject).IsRequired();
            });

            modelBuilder.Entity<Tasks>(e =>
            {
                e.ToTable("Tasks");
                e.HasKey(e => e.idTask);
                e.Property(e => e.taskName).IsRequired().HasColumnType("varchar(50)");
                e.Property(e => e.description).HasColumnType("text");
                //e.Property(e => e.taskCode).IsRequired().HasColumnType("varchar(20)");
                //e.Property(e => e.status).IsRequired().HasDefaultValue(Status.Open);
                e.Property(e => e.startTaskDate).HasColumnType("timestamp");
                e.Property(e => e.endTaskDate).HasColumnType("timestamp");
                e.Property(e => e.createTaskDate).HasColumnType("timestamp");
                e.Property(e => e.createUser).IsRequired();
                e.Property(e => e.idProject).IsRequired();
            });
            modelBuilder.Entity<OTs>(e =>
            {
                e.ToTable("OTs");
                e.Property(e => e.Date).HasColumnType("date");
            });
            modelBuilder.Entity<Projects>(e =>
            {
                /*e.ToTable("Projects").HasIndex(e => e.ProjectCode).IsUnique();*/
                e.Property(e => e.StartDate).HasColumnType("date");
                e.Property(e => e.EndDate).HasColumnType("date");
                e.Property(e => e.DateCreated).HasColumnType("date");
                e.Property(e => e.DateUpdate).HasColumnType("date");
                e.Property(e=>e.SubProjectCode).IsRequired();
                e.Property(e => e.ProjectCode).IsRequired();

            });
            modelBuilder.Entity<Wiki_CateGogy>(e =>
            {
                e.ToTable("Wiki_CateGogy");
                e.HasKey(e => e.idCateWiki);
                e.Property(e => e.wikiName).IsRequired().HasColumnType("varchar(50)");
                e.Property(e => e.wiktNote).HasColumnType("text");
            });
            modelBuilder.Entity<Wiki_Post>(e =>
            {
                e.ToTable("Wiki_Post");
                e.HasKey(e => e.idPost);
                e.Property(e => e.title).IsRequired().HasColumnType("varchar(50)");
                e.Property(e => e.content).HasColumnType("text");
                e.Property(e => e.status).IsRequired().HasDefaultValue(Status_Wiki.published);
                e.Property(e => e.note).HasColumnType("text");
                e.Property(e => e.userCrete).IsRequired();
                e.Property(e => e.dateCreate).HasColumnType("date");
                e.Property(e => e.userUpdate);
                e.Property(e => e.dateUpdate).HasColumnType("date");
            });
            modelBuilder.Entity<LeaveOff>(e =>
            {
                e.ToTable("Leave_Off");
                e.HasKey(e => e.id);
                e.Property(e => e.status).IsRequired();
                e.Property(e => e.reasons).HasColumnType("text");
                e.Property(e => e.startTime).HasColumnType("timestamp").IsRequired();
                e.Property(e => e.endTime).HasColumnType("timestamp").IsRequired();
                e.Property(e => e.createTime).HasColumnType("timestamp").IsRequired();
                e.Property(e => e.idLeaveUser).IsRequired();
            });
            //equipment
            modelBuilder.Entity<Devices>(e =>
            {
                e.ToTable("Devices");
                e.HasKey(e => e.IdDevice);
                e.Property(e => e.DeviceName).IsRequired();
                e.Property(e => e.Info).IsRequired();
                e.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(0);
                e.Property(e => e.UserCreated).IsRequired();
                e.Property(e => e.UserUpdated).IsRequired().HasDefaultValue(0);
                e.Property(e => e.DateUpdated).HasColumnType("timestamp").IsRequired().HasDefaultValue(DateTime.MinValue);
                e.Property(e => e.Note).HasColumnType("text");
            });
            modelBuilder.Entity<Handover>(e =>
            {
                e.ToTable("Handover");
                e.HasKey(e => e.IdHandover);
                e.Property(e => e.IdDevice).IsRequired();
                e.Property(e => e.UserReceive).IsRequired();
                e.Property(e => e.Amount).IsRequired();
                e.Property(e => e.Status).HasDefaultValue(StatusHandover.Waiting);
                e.Property(e => e.UserCreated).IsRequired();
                e.Property(e => e.DateCreated).HasColumnType("timestamp").IsRequired().HasDefaultValue(DateTime.Now);
                e.Property(e => e.UserUpdated).IsRequired().HasDefaultValue(0);
                e.Property(e => e.DateUpdated).HasColumnType("timestamp").IsRequired().HasDefaultValue(DateTime.MinValue);
                e.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(0);
            });
            modelBuilder.Entity<Branchs>(e =>
            {
                e.ToTable("Branchs");
                e.HasKey(e => e.idBranch);
                e.Property(e => e.branchName).HasColumnType("text");
                e.Property(e => e.isDeleted).HasDefaultValue(0);
            });
            modelBuilder.Entity<Orders>(e =>
            {
                e.ToTable("Orders");
                e.HasKey(e => e.idOrder);
                e.Property(e => e.idBranch).IsRequired();
                e.Property(e => e.idDevice).IsRequired();
                e.Property(e => e.amount).IsRequired();
                e.Property(e => e.dateCreated).HasColumnType("date").IsRequired();
                e.Property(e => e.userCreated).IsRequired();
                e.Property(e => e.dateUpdated).HasColumnType("date").IsRequired();
                e.Property(e => e.userUpdated).IsRequired();
                e.Property(e => e.isDeleted).HasDefaultValue(0);
                e.Property(e => e.note).HasColumnType("text");
                e.Property(e => e.statusOrder).IsRequired();
                e.Property(e => e.statusDevice).IsRequired();

            });
            //phan quyen use menu
            modelBuilder.Entity<Group>(e =>
            {
                e.ToTable("Group");
                e.HasKey(e => e.Id);
                e.Property(e => e.NameGroup).IsRequired();
                e.Property(e => e.Key).HasColumnType("varchar");
                e.Property(e => e.Discription).HasColumnType("text");
                e.Property(e => e.userCreated);
                e.Property(e => e.dateCreated).HasColumnType("date");
                e.HasIndex("Key").IsUnique();
            });
            modelBuilder.Entity<Permission_Use_Menu>(e =>
            {
                e.ToTable("Permission_Use_Menu");
                e.HasKey(e => e.Id);
                e.Property(e => e.IdUser).IsRequired();
                e.Property(e => e.IdMenu).IsRequired();
                e.Property(e => e.Add).IsRequired().HasDefaultValue(0);
                e.Property(e => e.Update).IsRequired().HasDefaultValue(0);
                e.Property(e => e.Delete).IsRequired().HasDefaultValue(0);
                e.Property(e => e.DeleteMulti).IsRequired().HasDefaultValue(0);
                e.Property(e => e.Confirm).IsRequired().HasDefaultValue(0);
                e.Property(e => e.ConfirmMulti).IsRequired().HasDefaultValue(0);
                e.Property(e => e.Refuse).IsRequired().HasDefaultValue(0);
                e.Property(e => e.AddMember).IsRequired().HasDefaultValue(0);
                e.Property(e => e.Export).IsRequired().HasDefaultValue(0);
                e.Property(e => e.isDeleted).HasDefaultValue(false);
            });
            modelBuilder.Entity<Menu>(e =>
            {
                e.ToTable("Menu");
                e.HasKey(e => e.id);
                e.Property(e => e.idModule);
                e.Property(e => e.title).HasColumnType("Text");
                e.Property(e => e.icon).HasColumnType("Text");
                e.Property(e => e.action).HasColumnType("Text");
                e.Property(e => e.view).HasColumnType("Text");
                e.Property(e => e.controller).HasColumnType("Text");
                e.Property(e => e.isDeleted).HasDefaultValue(0);
                e.Property(e => e.parent);
            });
            modelBuilder.Entity<Module>(e =>
            {
                e.ToTable("Module");
                e.HasKey(e => e.id);
                e.Property(e => e.nameModule).IsRequired().HasMaxLength(255).HasColumnType("varchar");
                e.Property(e => e.note).HasMaxLength(500).HasColumnType("varchar");
                e.Property(e => e.isDeleted).HasDefaultValue(0);
                e.Property(e => e.idSort).HasDefaultValue(0);
            });
            modelBuilder.Entity<Permission_Group>(e =>
            {
                e.ToTable("Permission_Group");
                e.HasKey(e => e.Id);
                e.Property(e => e.IdGroup).IsRequired();
                e.Property(e => e.IdModule).IsRequired();
                e.Property(e => e.Access).HasDefaultValue(true);
                e.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<Customer>(e =>
            {
                e.ToTable("Customer");
                e.HasKey(e => e.id);
                e.Property(k => k.firstName).HasMaxLength(50).HasColumnType("varchar");
                e.Property(k => k.lastName).HasMaxLength(50).HasColumnType("varchar");
                e.Property(e => e.isDeleted).HasDefaultValue(false);
                e.Property(e => e.dateCreated).HasColumnType("timestamp");
                e.Property(e => e.userCreated).IsRequired();
                e.Property(k => k.phoneNumber).HasMaxLength(11);
                e.Property(k => k.address).HasMaxLength(255).HasColumnType("varchar");
                e.Property(k => k.company).HasMaxLength(255).HasColumnType("varchar");
                e.Property(k => k.email).HasMaxLength(255).HasColumnType("varchar");
                e.Property(k => k.identityCard).HasMaxLength(255).HasColumnType("varchar");
                e.Property(k => k.accountNumber).HasMaxLength(255).HasColumnType("varchar");
            });

            modelBuilder.Entity<Paids>(e =>
            {
                e.ToTable("Paids");
                e.HasKey(e => e.Id);
                e.Property(e => e.PaidPerson).IsRequired();
                e.Property(e => e.CustomerName).IsRequired();
                e.Property(e => e.PaidReason).IsRequired();
                e.Property(e => e.PaidPerson).IsRequired();
                e.Property(e => e.ProjectId).HasDefaultValue(0);
                e.Property(e => e.IsAccept).HasDefaultValue(0);
                e.Property(e => e.ContentReason).HasMaxLength(255).HasColumnType("varchar");
                e.Property(e => e.ReasonRefusal).HasColumnType("varchar");
            });

            modelBuilder.Entity<PaidReasons>(e =>
            {
                e.ToTable("PaidReasons");
                e.HasKey(e => e.id);
                e.Property(e => e.isDeleted).HasDefaultValue(false);
                e.Property(e => e.dateCreated).HasColumnType("timestamp");
                e.Property(e => e.userCreated).IsRequired();
                e.Property(k => k.name).HasMaxLength(255).HasColumnType("varchar");
            });

            modelBuilder.Entity<Customer_PaidReason>(e =>
            {
                e.ToTable("Customer_PaidReason");
                e.HasKey(e => e.id);
                e.Property(e => e.isDeleted).HasDefaultValue(false);
                e.Property(e => e.dateCreated).HasColumnType("timestamp");
                e.Property(e => e.idCustomer).IsRequired();
                e.Property(e => e.idPaidReason).IsRequired();
            });

            modelBuilder.Entity<User_Group>(e =>
            {
                e.ToTable("User_Group");
                e.HasKey(e => e.id);
                e.Property(e => e.idUser).IsRequired();
                e.Property(e => e.idGroup).IsRequired();
                e.Property(e => e.isDeleted).HasDefaultValue(false);
                e.Property(e => e.dateCreated).HasColumnType("date");
                e.Property(e => e.dateUpdated).HasColumnType("date");
                e.Property(e => e.dateDeleted).HasColumnType("date");
            });

            modelBuilder.Entity<Action_Module>(e =>
            {
                e.ToTable("Action_Module");
                e.HasKey(e => e.id);
                e.Property(e => e.name).IsRequired().HasMaxLength(255).HasColumnType("varchar");
                e.Property(e => e.description).IsRequired().HasMaxLength(255).HasColumnType("varchar");
                e.Property(e => e.isDeleted).HasDefaultValue(false);
                e.Property(e => e.dateCreated).HasColumnType("date");
                e.Property(e => e.dateUpdated).HasColumnType("date");
                e.Property(e => e.dateDeleted).HasColumnType("date");
            });

            modelBuilder.Entity<Permission_Action_Module>(e =>
            {
                e.ToTable("Permission_Action_Module");
                e.HasKey(e => e.id);
                e.Property(e => e.moduleId).IsRequired();
                e.Property(e => e.actionModuleId).IsRequired();
                e.Property(e => e.isDeleted).HasDefaultValue(false);
                e.Property(e => e.dateCreated).HasColumnType("date");
                e.Property(e => e.dateUpdated).HasColumnType("date");
                e.Property(e => e.dateDeleted).HasColumnType("date");
            });

            modelBuilder.Entity<Group_Module_Action>(e =>
            {
                e.ToTable("Group_Module_Action");
                e.HasKey(e => e.id);
                e.Property(e => e.idGroup).IsRequired();
                e.Property(e => e.idModule).IsRequired();
                e.Property(e => e.idAction).IsRequired();
                e.Property(e => e.isDeleted).HasDefaultValue(false);
                e.Property(e => e.dateCreated).HasColumnType("date");
                e.Property(e => e.dateUpdated).HasColumnType("date");
                e.Property(e => e.dateDeleted).HasColumnType("date");
            });

            modelBuilder.Entity<TimeSheetDaily>(e =>
            {
                e.ToTable("TimeSheetDaily");
                e.HasKey(e => e.id);
                e.Property(e => e.idUser).IsRequired();
                e.Property(e => e.isConfirm).HasDefaultValue(false);
                e.Property(e => e.isDeleted).HasDefaultValue(false);
                e.Property(e => e.sum).HasDefaultValue(0);
                e.Property(e => e.layout).HasDefaultValue(0);
                e.Property(e => e.spec).HasDefaultValue(0);
                e.Property(e => e.api).HasDefaultValue(0);
                e.Property(e => e.web).HasDefaultValue(0);
                e.Property(e => e.utc).HasDefaultValue(0);
                e.Property(e => e.ute).HasDefaultValue(0);
                e.Property(e => e.integration).HasDefaultValue(0);
                e.Property(e => e.deployment).HasDefaultValue(0);
                e.Property(e => e.fixbug).HasDefaultValue(0);
                e.Property(e => e.support).HasDefaultValue(0);
                e.Property(e => e.others).HasDefaultValue(0);
                e.Property(e => e.sum).HasDefaultValue(0);
                e.Property(e => e.dateInputTimeSheet).HasColumnType("timestamp");
                e.Property(e => e.dateCreated).HasColumnType("timestamp");
                e.Property(e => e.dateUpdated).HasColumnType("timestamp");
                e.Property(e => e.dateDeleted).HasColumnType("timestamp");
            });


            #region Config entity with fluent Api
            modelBuilder.ApplyConfiguration(new RulesConfig());
            modelBuilder.ApplyConfiguration(new BlockingWebConfig());
            modelBuilder.ApplyConfiguration(new StaffReviewConfig());
            modelBuilder.ApplyConfiguration(new ExperienceConfig());
            modelBuilder.ApplyConfiguration(new ReviewResultConfig());
            modelBuilder.ApplyConfiguration(new PostCategoriesConfig());
            modelBuilder.ApplyConfiguration(new PostCommentsConfig());
            modelBuilder.ApplyConfiguration(new PostImagesConfig());
            modelBuilder.ApplyConfiguration(new PostsConfig());
            modelBuilder.ApplyConfiguration(new StaffReviewTimeConfig());

            #endregion
        }



        #endregion

    }
}
