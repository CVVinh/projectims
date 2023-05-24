using BE.Data.Dtos.PostDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.PaginationServices;
using BE.Services.PostServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: blog")]
    public class PostsController : ControllerBase
    {
        private readonly IPostServices _postServices;
        private readonly IPaginationServices<Posts> _paginationService;
        private readonly IWebHostEnvironment _host;

        public PostsController(IPostServices postServices, IPaginationServices<Posts> paginationServices, IWebHostEnvironment host)
        {
            _postServices = postServices;
            _paginationService = paginationServices;
            _host = host;
        }

        [HttpGet("getAllPostAsync")]
        public async Task<IActionResult> GetAllPostAsync(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _postServices.GetAllPostAsync();
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationService.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }

        [HttpPost("createNewPost")]
        [Authorize(Roles = "module:blog action:add")]
        public async Task<IActionResult> CreateNewPost([FromForm] PostDto postDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var pathServer = $"{Request.Scheme}://{Request.Host}";
            var response = await _postServices.CreateNewPost(postDto, _host.WebRootPath, pathServer);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("GetPostById/{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var response = await _postServices.GetPostById(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("updatePost/{id}")]
        [Authorize(Roles = "module:blog action:update")]
        public async Task<IActionResult> UpdatePost(int id, [FromForm] UpdatePostDto updatePostDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var pathServer = $"{Request.Scheme}://{Request.Host}";
            var rules = await _postServices.UpdatePost(id, updatePostDto, _host.WebRootPath, pathServer);
            if (rules._success)
            {
                return Ok(rules);
            }
            return BadRequest(rules);
        }

        [HttpDelete("deletePost/{id}")]
        [Authorize(Roles = "module:blog action:delete")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var pathServer = $"{Request.Scheme}://{Request.Host}";
            var response = await _postServices.DeletePost(id, _host.WebRootPath, pathServer);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(int? postId, string? title, string? user, string? category, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _postServices.Search(postId, title, user, category);
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationService.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }
    }
}
