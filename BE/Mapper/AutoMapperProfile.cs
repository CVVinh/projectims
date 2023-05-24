using AutoMapper;
using BE.Data.Dtos.ActionModuleDtos;
using BE.Data.Dtos.CustomerDtos;
using BE.Data.Dtos.Firebase_TokenDtos;
using BE.Data.Dtos.GruopDtos;
using BE.Data.Dtos.InfoDeviceDtos;
using BE.Data.Dtos.ModuleDtos;
using BE.Data.Dtos.NotificationDtos;
using BE.Data.Dtos.PaidDtos;
using BE.Data.Dtos.PaidReasonDtos;
using BE.Data.Dtos.Permission_Use_Menus;
using BE.Data.Dtos.PermissionActionModuleDtos;
using BE.Data.Dtos.PostDtos;
using BE.Data.Dtos.ProjectDtos;
using BE.Data.Dtos.RulesDTOs;
using BE.Data.Dtos.StaffReviewDtos;
using BE.Data.Dtos.TaskDtos;
using BE.Data.Dtos.TimeSheetDtos;
using BE.Data.Dtos.UserDtos;
using BE.Data.Models;
using BE.Helpers;
using System.Collections.Generic;

namespace BE.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Projects, AddNewProjectDto>().ReverseMap();
            CreateMap<Projects, EditProjectDto>().ReverseMap();
            CreateMap<Paids, CreatePaidDtos>().ReverseMap();
            CreateMap<Paids, AcceptPaymentPaidDtos>().ReverseMap();
            CreateMap<IdPaidImgDtos, PaidImage>().ReverseMap();

            CreateMap<Rules, AddOrUpdateRulesDTO>().ReverseMap();
            CreateMap<InfoDevices, CreateInfoDeviceDto>().ReverseMap();
            CreateMap<DeviceInstalledApps, CreateInfoDeviceDto>().ReverseMap();
            CreateMap<DeviceInstalledApps, ApplicationDto>().ReverseMap();

            CreateMap<UserGroupDeletedDto, User_Group>().ReverseMap();
            CreateMap<UserGroupUpdatedDto, User_Group>().ReverseMap();
            CreateMap<UserGroupCreatedDto, User_Group>().ReverseMap();
            CreateMap<PermissionUserMenuAddDto, Permission_Use_Menu>().ReverseMap();
            CreateMap<PermissionUserMenuEditDto, Permission_Use_Menu>().ReverseMap();
            CreateMap<PermissionUserMenuRequestDto, Permission_Use_Menu>().ReverseMap();
            CreateMap<PermissionUserMenuRequesNoIdGrouptDto, Permission_Use_Menu>().ReverseMap();

            CreateMap<UpdateGroupDtos, Group>().ReverseMap();
            CreateMap<AddGroupDtos, Group>().ReverseMap();

            CreateMap<AddGroupModuleActionDto, Group_Module_Action>().ReverseMap();
            CreateMap<UpdateGroupModuleActionDto, Group_Module_Action>().ReverseMap();
            CreateMap<DeleteGroupModuleActionDto, Group_Module_Action>().ReverseMap();
            CreateMap<DeleteMultiGroupModuleActionDto, Group_Module_Action>().ReverseMap();
            CreateMap<RequestGroupModuleActionDto, Group_Module_Action>().ReverseMap();
            CreateMap<ChangeGroupModuleActionDto, Group_Module_Action>().ReverseMap();


            CreateMap<AddActionModuleDto, Action_Module>().ReverseMap();
            CreateMap<EditActionModuleDto, Action_Module>().ReverseMap();
            CreateMap<DeleteActionModuleDto, Action_Module>().ReverseMap();
            CreateMap<ResponseActionModuleDto, Action_Module>().ReverseMap();

            CreateMap<ModuleDtos, Module>().ReverseMap();

            CreateMap<PermissionGroupDto, Permission_Group>().ReverseMap();
            CreateMap<ChangePermissionGroupDto, Permission_Group>().ReverseMap();
            CreateMap<PermissionGroupRequestDto, Permission_Group>().ReverseMap();

            CreateMap<AddPermissionActionModuleDto, Permission_Action_Module>().ReverseMap();
            CreateMap<UpdatePermissionActionModuleDto, Permission_Action_Module>().ReverseMap();
            CreateMap<DeletePermissionActionModuleDto, Permission_Action_Module>().ReverseMap();
            CreateMap<RequestPermissionActionModuleDto, Permission_Action_Module>().ReverseMap();
            CreateMap<DeleteMultiPermissionActionModuleDto, Permission_Action_Module>().ReverseMap();

            CreateMap<Notification, CreateRequireDeleteApplicationDTO>().ReverseMap();
            CreateMap<Users, UserDto>().ReverseMap();
            CreateMap<Users, UserWithNameDto>().ReverseMap();
            CreateMap<StaffReview, StaffReviewDto>().ReverseMap();
            CreateMap<Experience, ExperienceDto>().ReverseMap();
            CreateMap<ReviewResult, ReviewResultDto>().ReverseMap();

            CreateMap<StaffReview, CreateStaffReviewDto>().ReverseMap();
            CreateMap<Experience, CreateExperienceDto>().ReverseMap();
            CreateMap<ReviewResult, CreateReviewResultDto>().ReverseMap();
            CreateMap<Users, UpdateProfileDto>().ReverseMap();

            CreateMap<Tasks, AddNewTaskDto>().ReverseMap();
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<UpdateCustomerDto, Customer>().ReverseMap();
            CreateMap<PaidReasons, CreatePaidReasonDto>().ReverseMap();
            CreateMap<UpdatePaidReasonDto, PaidReasons>().ReverseMap();

            CreateMap<AddNewTaskDto, Tasks>()
                .ForMember(dest => dest.taskCode, opt => opt.MapFrom(src => CommonHelper.RandomCode()))
                .ForMember(dest => dest.status, opt => opt.Ignore()).ReverseMap();

            CreateMap<Tasks, UpdateTaskDto>().ReverseMap();
            CreateMap<PostDto, Posts>().ReverseMap();
            CreateMap<UpdatePostDto, Posts>().ReverseMap();
            CreateMap<PostComments, PostCommentDto>().ReverseMap();
            CreateMap<UpdatePostCommentDto, PostComments>().ReverseMap();
            CreateMap<CreatePostCommentDto, PostComments>().ReverseMap();
            CreateMap<UpdateStaffViewTimeDto, StaffReviewTime>().ReverseMap();
            CreateMap<StaffReviewTimeDto, StaffReviewTime>().ReverseMap();
            CreateMap<CreateMultiStaffReviewTimeDto, StaffReviewTime>().ReverseMap();
            CreateMap<UpdateMultiStaffViewTimeDto, StaffReviewTime>().ReverseMap();

            CreateMap<CreatedTimeSheetDailyDto, TimeSheetDaily>().ReverseMap();
            CreateMap<UpdatedMultiTimeSheetDailyDto, TimeSheetDaily>().ReverseMap();
            
            CreateMap<Firebase_TokenDTO, Firebase_Token>().ReverseMap();
        }
    }
}
