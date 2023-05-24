using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.PostDtos;
using BE.Data.Dtos.UserDtos;
using BE.Data.Models;
using BE.Helpers;
using BE.Response;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;

namespace BE.Services.PostServices
{
    public interface IPostCommentServices
    {
        Task<BaseResponse<List<PostCommentDto>>> GetAllPostCommentAsync();
        Task<BaseResponse<PostComments>> CreateNewPostComment(CreatePostCommentDto postCommentDto);
        Task<BaseResponse<PostComments>> UpdatePostComment(int id, UpdatePostCommentDto updatePostCommentDto);
        Task<BaseResponse<PostComments>> DeletePostComment(int id);
        Task<BaseResponse<List<PostCommentDto>>> GetAllCommentByIdPost(int id);
        Task<BaseResponse<PostComments>> ReplayCommentPost(CreatePostCommentDto postCommentDto);
    }
    public class PostCommentServices : IPostCommentServices
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        public PostCommentServices(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }
        public async Task<BaseResponse<List<PostCommentDto>>> GetAllPostCommentAsync()
        {
            var success = false;
            var message = "";
            var data = new List<PostCommentDto>();
            try
            {

                var comment = await _appDbContext.Comments
               .Where(s => s.isDeleted == false)
               .OrderByDescending(s => s.dateCreated)
               .Join(_appDbContext.Users, c => c.userCreated, u => u.id, (c, u) => new { Comment = c, User = u })
               .Select(x => new PostCommentDto
               {
                   id = x.Comment.id,
                   content = x.Comment.content,
                   dateCreated = x.Comment.dateCreated,
                   postId = x.Comment.postId,
                   userCreated = x.Comment.userCreated ?? 0,
                   userCreatedNavigation = new UserWithNameDto
                   {
                       Id = x.User.id,
                       firstName = x.User.firstName,
                       lastName = x.User.lastName,
                       userCode = x.User.userCode,
                   }
               })
               .ToListAsync();
                success = true;
                message = "Nhận tất cả dữ liệu thành công!";
                data.AddRange(comment);
                return (new BaseResponse<List<PostCommentDto>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<PostCommentDto>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<PostCommentDto>>> GetAllCommentByIdPost(int id)
        {
            var success = false;
            var message = "";
            var data = new List<PostCommentDto>();
            try
            {

                var comment = await _appDbContext.Comments
                    .Include(s => s.PostCommentsNavigations)
                    .Include(s => s.userComment)
                    .Where(s => s.isDeleted == false && s.postId == id && s.parentId == null)
                    .OrderByDescending(s => s.dateCreated)
                    .Join(_appDbContext.Users, c => c.userCreated, u => u.id, (c, u) => new { Comment = c, User = u })
                    .Select(x => new PostCommentDto
                    {
                        id = x.Comment.id,
                        content = x.Comment.content,
                        dateCreated = x.Comment.dateCreated,
                        postId = x.Comment.postId,
                        userCreated = x.Comment.userCreated ?? 0,
                        userCreatedNavigation = new UserWithNameDto
                        {
                            Id = x.User.id,
                            firstName = x.User.firstName,
                            lastName = x.User.lastName,
                            userCode = x.User.userCode,
                        },
                        PostCommentsNavigations = x.Comment.PostCommentsNavigations.AsQueryable().Include(s => s.userComment).ToList() ?? null,
                        parentId = x.Comment.parentId,
                        userComment = new UserWithNameDto
                        {
                            Id = x.Comment.userComment.id,
                            firstName = x.Comment.userComment.firstName,
                            lastName = x.Comment.userComment.lastName,
                            userCode = x.Comment.userComment.userCode,
                        },
                    }).ToListAsync();
                success = true;
                message = "Nhận tất cả dữ liệu thành công!";
                data.AddRange(comment);
                return (new BaseResponse<List<PostCommentDto>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<PostCommentDto>>(success, message, data));
            }
        }

        public async Task<BaseResponse<PostComments>> CreateNewPostComment(CreatePostCommentDto postCommentDto)
        {
            var success = false;
            var message = "";
            var data = new PostComments();
            try
            {
                var comment = _mapper.Map<PostComments>(postCommentDto);
                if (comment != null)
                {
                    comment.dateCreated = DateTime.Now;
                    await _appDbContext.Comments.AddAsync(comment);
                    await _appDbContext.SaveChangesAsync();
                    success = true;
                    message = "Tạo mới bình luận thành công!";
                    data = comment;
                }

                return new BaseResponse<PostComments>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Tạo mới bình luận không thành công! {ex}";
                return new BaseResponse<PostComments>(success, message, data);
            }
        }
        public async Task<BaseResponse<PostComments>> UpdatePostComment(int id, UpdatePostCommentDto updatePostCommentDto)
        {
            var success = false;
            var message = "";
            var data = new PostComments();
            try
            {
                var comment = await _appDbContext.Comments.Where(s => s.id.Equals(id)).FirstOrDefaultAsync();
                if (comment is null)
                {
                    message = "Bình luận không tồn tại !";
                    data = null;
                    return new BaseResponse<PostComments>(success, message, data);
                }
                var commentMapdata = _mapper.Map<UpdatePostCommentDto, PostComments>(updatePostCommentDto, comment);
                commentMapdata.dateUpdated = DateTime.Now;
                _appDbContext.Comments.Update(commentMapdata);
                await _appDbContext.SaveChangesAsync();
                success = true;
                message = "Cập nhật bình luận thành công!";
                data = commentMapdata;
                return new BaseResponse<PostComments>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Cập nhật bình luận thất bại! {ex.Message}";
                return new BaseResponse<PostComments>(success, message, data = null);
            }
        }
        public async Task<BaseResponse<PostComments>> DeletePostComment(int id)
        {
            var success = false;
            var message = "";
            var data = new PostComments();
            try
            {
                var comment = await _appDbContext.Comments.Where(s => s.isDeleted == false && s.id.Equals(id)).FirstOrDefaultAsync();
                if (comment is null)
                {
                    success = false;
                    message = "Bình luận không tồn tại !";
                    return new BaseResponse<PostComments>(success, message, data);
                }
                comment.dateDeleted = DateTime.Now;
                comment.isDeleted = true;
                _appDbContext.Comments.Remove(comment);
                await _appDbContext.SaveChangesAsync();
                success = true;
                message = "Xóa bình luận thành công!";
                return new BaseResponse<PostComments>(success, message, comment);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Xóa bình luận không thành công! {ex.InnerException}";
                return new BaseResponse<PostComments>(success, message, data = null);
            }
        }
        public async Task<BaseResponse<PostComments>> ReplayCommentPost(CreatePostCommentDto postCommentDto)
        {
            var success = false;
            var message = "";
            var data = new PostComments();
            try
            {
                var comment = _mapper.Map<PostComments>(postCommentDto);
                if (comment != null)
                {
                    comment.dateCreated = DateTime.Now;
                    await _appDbContext.Comments.AddAsync(comment);
                    await _appDbContext.SaveChangesAsync();
                    success = true;
                    message = "Trả lời bình luận thành công!";
                    data = comment;
                }

                return new BaseResponse<PostComments>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Trả lời bình luận không thành công! {ex}";
                return new BaseResponse<PostComments>(success, message, data);
            }
        }
    }
}
