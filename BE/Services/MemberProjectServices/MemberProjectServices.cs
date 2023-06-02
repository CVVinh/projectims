﻿using BE.Data.Contexts;
using BE.Data.Dtos.MemberProjectDtos;
using BE.Data.Models;
using BE.Response;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;

namespace BE.Services.MemberProjectServices
{
    public interface IMemberProjectServices
    {
        Task<BaseResponse<ICollection<Member_Project>>> GetMembersByIdProjectAsync(int idProject);
        Task<BaseResponse<ICollection<Member_Project>>> GetMembersByListIdProjectAsync(List<int> idProject);
        Task<BaseResponse<Member_Project>> AddNewMemberAsync(AddMemberDto addMemberDto);
        Task<BaseResponse<List<Member_Project>>> AddMemberFromGitAsync(List<AddMemberDto> addMemberDtos);
        Task<BaseResponse<Member_Project>> DeleteMemberAsync(int idMember);
        Task<BaseResponse<Member_Project>> DeleteMemberInProjectAsync(int idMember, int idProject);
        Task<BaseResponse<ICollection<Member_Project>>> GetMemberByIdAsync(int idMemberProject);
        Task<BaseResponse<Member_Project>> GetMemberByIdUserAtProjectAsync(int idUser, int idProject);
    }

    public class MemberProjectServices : IMemberProjectServices
    {
        private readonly AppDbContext _appContext;
        public MemberProjectServices(AppDbContext appContext)
        {
            _appContext = appContext;
        }

        public async Task<BaseResponse<Member_Project>> AddNewMemberAsync(AddMemberDto addMemberDto)
        {
            var success = false;
            var message = "";
            var data = new Member_Project();
            try
            {
                var newMember = new Member_Project
                {
                    createUser = addMemberDto.createUser,
                    createDate = DateTime.Now,
                    member = addMemberDto.member,
                    idProject = addMemberDto.idProject
                };

                await _appContext.Member_Projects.AddAsync(newMember);
                await _appContext.SaveChangesAsync();
                success = true;
                message = "Add new member sucessfully";
                data = newMember;
                return new BaseResponse<Member_Project>(success, message, data);
            }
            catch (Exception)
            {
                message = "Add new member failed !";
                data = null;
                return new BaseResponse<Member_Project>(success, message, data);
            }
        }

        public async Task<BaseResponse<List<Member_Project>>> AddMemberFromGitAsync(List<AddMemberDto> addMemberDtos)
        {
            var success = false;
            var message = "";
            var data = new List<Member_Project>();
            try
            {
                foreach (var item in addMemberDtos)
                {
                    var getUser = await _appContext.Users.Where(u => u.isDeleted == 0 && u.idUserGitLab.Equals(item.member)).FirstOrDefaultAsync();
                    if (getUser != null)
                    {
                        var newMember = new Member_Project
                        {
                            createUser = item.createUser,
                            createDate = DateTime.Now,
                            member = getUser.id,
                            idProject = item.idProject
                        };
                        await _appContext.Member_Projects.AddAsync(newMember);
                        data.Add(newMember);
                    }
                }
                await _appContext.SaveChangesAsync();
                success = true;
                message = "Add new member sucessfully";
                return new BaseResponse<List<Member_Project>>(success, message, data);
            }
            catch (Exception)
            {
                message = "Add new member failed!";
                return new BaseResponse<List<Member_Project>>(success, message, data);
            }
        }

        public async Task<BaseResponse<Member_Project>> DeleteMemberAsync(int idMember)
        {
            var success = false;
            var message = "";
            var data = new Member_Project();
            try
            {
                var member = await _appContext.Member_Projects.Where(mp => mp.Id == idMember && mp.isDeleted == false).FirstOrDefaultAsync();
                if (member is null)
                {
                    message = "Member dosen't exist !";
                    data = null;
                    return new BaseResponse<Member_Project>(success, message, data);
                }

                member.isDeleted = true;
                _appContext.Member_Projects.Update(member);
                await _appContext.SaveChangesAsync();
                success = true;
                message = "Delete member successfully";
                data = member;
                return new BaseResponse<Member_Project>(success, message, data);
            }
            catch (Exception)
            {
                message = "Don't connect database !";
                data = null;
                return new BaseResponse<Member_Project>(success, message, data);
            }
        }

        public async Task<BaseResponse<Member_Project>> DeleteMemberInProjectAsync(int idMember, int idProject)
        {
            var success = false;
            var message = "";
            var data = new Member_Project();
            try
            {
                var member = await _appContext.Member_Projects.Where(mp => mp.member == idMember &&
                                                                           mp.isDeleted == false &&
                                                                           mp.idProject == idProject).FirstOrDefaultAsync();
                if (member is null)
                {
                    message = "Member dosen't exist !";
                    data = null;
                    return new BaseResponse<Member_Project>(success, message, data);
                }

                _appContext.Member_Projects.Remove(member);
                await _appContext.SaveChangesAsync();
                success = true;
                message = "Delete member successfully";
                data = member;
                return new BaseResponse<Member_Project>(success, message, data);
            }
            catch (Exception)
            {
                message = "Don't connect database !";
                data = null;
                return new BaseResponse<Member_Project>(success, message, data);
            }
        }

        public async Task<BaseResponse<ICollection<Member_Project>>> GetMemberByIdAsync(int idMemberProject)
        {
            var success = false;
            var message = "";
            ICollection<Member_Project>? data = null;
            var checkIdMemberProject = await _appContext.Member_Projects.Where(t => t.member == idMemberProject && t.isDeleted == false).ToListAsync();

            if (checkIdMemberProject is null)
            {
                message = "IdMemberProject does not exist";
                return new BaseResponse<ICollection<Member_Project>>(success, message, data);
            }

            success = true;
            message = "Getting a member in project by idMemberProject sucessfully";
            data = checkIdMemberProject;
            return new BaseResponse<ICollection<Member_Project>>(success, message, data);
        }

        public async Task<BaseResponse<Member_Project>> GetMemberByIdUserAtProjectAsync(int idUser, int idProject)
        {
            var success = false;
            var message = "";
            Member_Project? data = null;
            try
            {
                var testMemberProject = await _appContext.Member_Projects.Where(mp => mp.member == idUser &&
                                                                                mp.idProject == idProject &&
                                                                                mp.isDeleted == false).FirstOrDefaultAsync();
                if (testMemberProject is null)
                {
                    message = "Don't found member in project !";
                    return new BaseResponse<Member_Project>(success, message, data);
                }

                success = true;
                message = "Getting member in project successfully";
                data = testMemberProject;
                return new BaseResponse<Member_Project>(success, message, data);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return new BaseResponse<Member_Project>(success, message, data);
            }
        }

        public async Task<BaseResponse<ICollection<Member_Project>>> GetMembersByIdProjectAsync(int idProject)
        {
            ICollection<Member_Project>? listMemberProject = null;
            var success = false;
            var message = "";
            var data = listMemberProject;
            var checkIdProject = await _appContext.Projects.Where(p => p.Id == idProject && p.IsDeleted == false).FirstOrDefaultAsync();
            if (checkIdProject is null)
            {
                message = "Id project does not exist";
                return new BaseResponse<ICollection<Member_Project>>(success, message, data);
            }

            success = true;
            message = "Getting all member by id project sucessfully";
            data = await _appContext.Member_Projects.Where(mp => mp.idProject == idProject).ToListAsync();
            return new BaseResponse<ICollection<Member_Project>>(success, message, data);
        }
        public async Task<BaseResponse<ICollection<Member_Project>>> GetMembersByListIdProjectAsync(List<int> idProject)
        {
            ICollection<Member_Project>? listMemberProject = null;
            var success = false;
            var message = "";
            var data = listMemberProject;
            var checkIdProject = await _appContext.Projects.Where(p => idProject.Contains(p.Id) && p.IsDeleted == false).FirstOrDefaultAsync();
            if (checkIdProject is null)
            {
                message = "Id project does not exist";
                return new BaseResponse<ICollection<Member_Project>>(success, message, data);
            }

            success = true;
            message = "Getting all member by id project sucessfully";
            data = await _appContext.Member_Projects.Where(mp => idProject.Contains(mp.idProject)).ToListAsync();
            return new BaseResponse<ICollection<Member_Project>>(success, message, data);
        }
    }
}
