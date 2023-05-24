using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Models;
using BE.Response;
using Microsoft.EntityFrameworkCore;
using System;

namespace BE.Services.PostServices
{
    public interface IPostCategoryServices
    {
        Task<BaseResponse<List<PostCategories>>> GetAllPostCategoryAsync();
        Task<BaseResponse<List<PostCategories>>> GetPostCategoryByIDAsync(int categoryID);
    }
    public class PostCategoryService : IPostCategoryServices
    {
        private readonly AppDbContext _appDbContext;
        public PostCategoryService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<BaseResponse<List<PostCategories>>> GetAllPostCategoryAsync()
        {
            var success = false;
            var message = "";
            var data = new List<PostCategories>();
            try
            {
                var categories = await _appDbContext.Categories.ToListAsync();
                success = true;
                message = "Nhận tất cả dữ liệu thành công!";
                data.AddRange(categories);
                return (new BaseResponse<List<PostCategories>>(success, message, categories));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<PostCategories>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<PostCategories>>> GetPostCategoryByIDAsync(int categoryID)
        {
            var success = false;
            var message = "";
            List<PostCategories> data = null;
            try
            {
                var categories = await _appDbContext.Categories
                    .Where(c => c.id == categoryID)
                    .ToListAsync();

                success = true;
                message = "Lấy tất cả các danh mục bài đăng thành công!";
                data = categories;
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return new BaseResponse<List<PostCategories>>(success, message, data);
        }
    }
}
