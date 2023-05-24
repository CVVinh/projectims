using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.PaidDtos;
using BE.Data.Dtos.PostDtos;
using BE.Data.Models;
using BE.Helpers;
using BE.Response;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using DocumentFormat.OpenXml.Vml;
using Path = System.IO.Path;
using DocumentFormat.OpenXml.Spreadsheet;
using BE.Data.Dtos.LeaveOffDtos;

namespace BE.Services.PostServices
{
    public interface IPostServices
    {
        Task<BaseResponse<List<Posts>>> GetAllPostAsync();
        Task<BaseResponse<Posts>> CreateNewPost(PostDto postDto, string root, string local);
        Task<BaseResponse<Posts>> UpdatePost(int id, UpdatePostDto updatePostDto, string root, string local);
        Task<BaseResponse<Posts>> DeletePost(int id, string root, string local);
        Task<BaseResponse<List<Posts>>> Search(int? postId, string title, string user, string Category);
        Task<BaseResponse<Posts>> GetPostById(int id);
    }
    public class PostServices : IPostServices
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PostServices(AppDbContext appDbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<BaseResponse<List<Posts>>> GetAllPostAsync()
        {
            var success = false;
            var message = "";
            var data = new List<Posts>();
            try
            {

                var post = await _appDbContext.Posts.Where(s => s.isDeleted == false)
                                                    .OrderByDescending(s => s.dateCreated)
                                                    .Include(s => s.commentPostNavigations)
                                                    .Include(s => s.imagePostsNavigations)
                                                    .Include(s => s.postCate)
                                                    .ToListAsync();
                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                data.AddRange(post);
                return (new BaseResponse<List<Posts>>(success, message, data));
            }

            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Posts>>(success, message, data));
            }
        }
        public async Task<BaseResponse<Posts>> GetPostById(int id)
        {
            var success = false;
            var message = "";
            var data = new Posts();
            try
            {
                var post = await _appDbContext.Posts.Where(x => x.id == id)
                    .Include(s => s.commentPostNavigations)
                    .Include(s => s.imagePostsNavigations)
                    .Include(s => s.postCate)
                    .FirstOrDefaultAsync();

                if (post == null)
                {
                    success = false;
                    message = $"Không có bài đăng với Id là {id}";
                    data = null;
                    return (new BaseResponse<Posts>(success, message, data));
                }

                success = true;
                message = "Lấy tất cả dữ liệu thành công";
                data = post;
                return (new BaseResponse<Posts>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<Posts>(success, message, data));
            }
        }
        public async Task<BaseResponse<Posts>> CreateNewPost(PostDto postDto, string root, string local)
        {
            var success = false;
            var message = "";
            var data = new Posts();
            try
            {
                var post = _mapper.Map<Posts>(postDto);
                if (postDto.imagePostsNavigationss != null && postDto.imagePostsNavigationss.Count > 0)
                {
                    foreach (var item in postDto.imagePostsNavigationss)
                    {
                        post.imagePostsNavigations = post.imagePostsNavigations ?? new List<PostImages>();
                        var ms = new MemoryStream();
                        await item.CopyToAsync(ms);

                        var base64String = Convert.ToBase64String(ms.ToArray());
                        var imgSrc = $"data:{item.ContentType};base64,{base64String}";

                        post.imagePostsNavigations.Add(new PostImages()
                        {
                            imageName = item.FileName,
                            imageType = item.ContentType,
                            imageCode = ms.ToArray(),
                            pathImage = imgSrc,
                        });
                    }
                }
                else
                {
                    post.imagePostsNavigations = new List<PostImages>();
                }
                if (post != null)
                {
                    post.dateCreated = DateTime.Now;
                    await _appDbContext.Posts.AddAsync(post);
                    await _appDbContext.SaveChangesAsync();
                    success = true;
                    message = "Tạo mới bài viết thành công!";
                    data = post;
                }

                return new BaseResponse<Posts>(success, message, data);
            }
            catch (Exception ex)
            {
                message = $"Tạo mới bài viết không thành công! {ex}";
                return new BaseResponse<Posts>(success, message, data);
            }
        }
        public async Task<BaseResponse<Posts>> UpdatePost(int id, UpdatePostDto updatePostDto, string root, string local)
        {
            var success = false;
            var message = "";
            var data = new Posts();
            try
            {
                var post = await _appDbContext.Posts
                        .Include(p => p.imagePostsNavigations) // lấy tất cả các hình ảnh liên quan đến bài đăng
                        .Where(s => s.id.Equals(id))
                        .FirstOrDefaultAsync();

                if (post is null)
                {
                    // xử lý khi không tìm thấy bài đăng
                    message = "Bài post không tồn tại !";
                    data = null;
                    return new BaseResponse<Posts>(success, message, data);
                }
                var postMapdata = _mapper.Map<UpdatePostDto, Posts>(updatePostDto, post);

                if (updatePostDto.imagePostsNavigationss != null)
                {
                    foreach (var item in updatePostDto.imagePostsNavigationss)
                    {
                        var ms = new MemoryStream();
                        await item.CopyToAsync(ms);

                        var base64String = Convert.ToBase64String(ms.ToArray());
                        var imgSrc = $"data:{item.ContentType};base64,{base64String}";

                        // kiểm tra xem postid đã tồn tại hình ảnh trong DB chưa
                        var existingImage = post.imagePostsNavigations.FirstOrDefault(i => i.postId == id);
                        if (existingImage != null)
                        {
                            // nếu đã tồn tại, thay thế đường dẫn hình ảnh trong DB bằng đường dẫn mới
                            //existingImage.pathImage = local + FilesHelper.UploadFileAndReturnPath(item, root, "/PostPicture/");
                            existingImage.imageName = item.FileName;
                            existingImage.imageType = item.ContentType;
                            existingImage.imageCode = ms.ToArray();
                            existingImage.pathImage = imgSrc;
                        }
                        else
                        {
                            // nếu chưa tồn tại, thêm hình ảnh mới vào danh sách hình ảnh của bài đăng
                            post.imagePostsNavigations.Add(new PostImages()
                            {
                                //pathImage = local + FilesHelper.UploadFileAndReturnPath(item, root, "/PostPicture/")
                                imageName = item.FileName,
                                imageType = item.ContentType,
                                imageCode = ms.ToArray(),
                                pathImage = imgSrc,
                            });
                        }
                    }
                }

                postMapdata.dateUpdated = DateTime.Now;

                _appDbContext.Posts.Update(postMapdata);

                await _appDbContext.SaveChangesAsync();

                success = true;
                message = "Cập nhật bài post thành công!";
                data = postMapdata;
                return new BaseResponse<Posts>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Cập nhật bài post thất bại! {ex.Message}";
                return new BaseResponse<Posts>(success, message, data = null);
            }
        }
        public async Task<BaseResponse<Posts>> DeletePost(int id, string root, string local)
        {
            var success = false;
            var message = "";
            var data = new Posts();
            try
            {
                var post = await _appDbContext.Posts.Where(s => s.isDeleted == false && s.id.Equals(id)).FirstOrDefaultAsync();
                if (post is null)
                {
                    success = false;
                    message = "Bài post không tồn tại !";
                    return new BaseResponse<Posts>(success, message, data);
                }
                _appDbContext.Posts.Remove(post);

                if (post.imagePostsNavigations != null)
                {
                    foreach (var item in post.imagePostsNavigations)
                    {
                        var fileName = item.pathImage?.Replace($"{local}/PostPicture/", "");
                        string filePath = System.IO.Path.Combine(root, "PostPicture", fileName ?? "");
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        _appDbContext.ImagePosts.Remove(item);
                    }
                }
                post.dateDeleted = DateTime.Now;
                post.isDeleted = true;
                await _appDbContext.SaveChangesAsync();
                success = true;
                message = "Xóa bài post thành công!";
                return new BaseResponse<Posts>(success, message, post);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Xóa bài post không thành công! {ex.InnerException}";
                return new BaseResponse<Posts>(success, message, data = null);
            }
        }
        public async Task<BaseResponse<List<Posts>>> Search(int? postId, string? title, string? user, string? category)
        {
            var success = false;
            var message = "";
            var data = new List<Posts>();
            try
            {
                if (postId != null)
                {
                    var post = await _appDbContext.Posts
                        .Where(s => s.isDeleted == false && s.id.Equals(postId))
                        .OrderByDescending(s => s.dateCreated)
                        .Include(s => s.commentPostNavigations)
                        .Include(s => s.imagePostsNavigations)
                        .Include(s => s.postCate)
                        .ToListAsync();
                    if (post.Count() == 0)
                    {
                        success = true;
                        message = "Không có dữ liệu!";
                        data = new List<Posts>();
                    }
                    else
                    {
                        data.AddRange(post);
                        success = true;
                        message = "Lấy dữ liệu thành công!";
                    }
                }
                else if (!string.IsNullOrEmpty(title))
                {
                    var post = await _appDbContext.Posts
                        .Where(s => s.isDeleted == false && s.title.ToLower().Contains(title.ToLower().Trim()))
                        .OrderByDescending(s => s.dateCreated)
                        .Include(s => s.commentPostNavigations)
                        .Include(s => s.imagePostsNavigations)
                        .Include(s => s.postCate)
                        .ToListAsync();
                    if (post.Count() == 0)
                    {
                        success = true;
                        message = "Không có dữ liệu!";
                        data = new List<Posts>();
                    }
                    else
                    {
                        data.AddRange(post);
                        success = true;
                        message = "Lấy dữ liệu thành công!";
                    }
                }
                else if (!string.IsNullOrEmpty(user))
                {
                    var post = await _appDbContext.Posts
                                .Include(p => p.commentPostNavigations)
                                .Include(p => p.imagePostsNavigations)
                                .Include(p => p.postCate)
                                .Join(_appDbContext.Users,
                                      p => p.userCreated,
                                      u => u.id,
                                      (p, u) => new { Post = p, User = u })
                                .Where(x => x.User.firstName.ToLower().Contains(user.ToLower().Trim())
                                            || x.User.lastName.ToLower().Contains(user.ToLower().Trim()))
                                .Select(x => x.Post)
                                .ToListAsync();

                    if (post.Count() == 0)
                    {
                        success = true;
                        message = "Không có dữ liệu!";
                        data = new List<Posts>();
                    }
                    else
                    {
                        data.AddRange(post);
                        success = true;
                        message = "Lấy dữ liệu thành công!";
                    }
                }

                else if (!string.IsNullOrEmpty(user))
                {
                    var post = await _appDbContext.Posts
                                .Join(_appDbContext.Users,
                                      p => p.userCreated,
                                      u => u.id,
                                      (p, u) => new { Post = p, User = u })
                                .Where(x => x.User.firstName.ToLower().Contains(user.ToLower().Trim())
                                            || x.User.lastName.ToLower().Contains(user.ToLower().Trim()))
                                .Include(x => x.Post.commentPostNavigations)
                                .Include(x => x.Post.imagePostsNavigations)
                                .Include(x => x.Post.postCate)
                                .Select(x => x.Post)
                                .ToListAsync();

                    if (post.Count() == 0)
                    {
                        success = true;
                        message = "Không có dữ liệu!";
                        data = new List<Posts>();
                    }
                    else
                    {
                        data.AddRange(post);
                        success = true;
                        message = "Lấy dữ liệu thành công!";
                    }
                }
                else if (!string.IsNullOrEmpty(category))
                {
                    var post = await _appDbContext.Posts
                        .Where(s => s.isDeleted == false && s.postCate.name.ToLower().Contains(category.ToLower().Trim()))
                        .OrderByDescending(s => s.dateCreated)
                        .Include(s => s.commentPostNavigations)
                        .Include(s => s.imagePostsNavigations)
                        .Include(s => s.postCate)
                        .ToListAsync();
                    if (post.Count() == 0)
                    {
                        success = true;
                        message = "Không có dữ liệu!";
                        data = new List<Posts>();
                    }
                    else
                    {
                        data.AddRange(post);
                        success = true;
                        message = "Lấy dữ liệu thành công!";
                    }
                }
                else
                {
                    success = true;
                    var post = await _appDbContext.Posts
                        .Where(s => s.isDeleted == false)
                        .OrderByDescending(s => s.dateCreated)
                        .Include(s => s.commentPostNavigations)
                        .Include(s => s.imagePostsNavigations)
                        .Include(s => s.postCate)
                        .ToListAsync();
                    data.AddRange(post);
                }
                return (new BaseResponse<List<Posts>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Posts>>(success, message, data));
            }
        }
    }
}
