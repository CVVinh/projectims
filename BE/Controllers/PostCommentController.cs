using BE.Data.Dtos.PostDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.PostServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: blog")]
    public class PostCommentController : ControllerBase
    {
        private readonly IPostCommentServices _postCommentServices;

        public PostCommentController(IPostCommentServices postCommentServices, IWebHostEnvironment host)
        {
            _postCommentServices = postCommentServices;
        }

        [HttpGet("getAllPostCommentAsync")]
        public async Task<IActionResult> GetAllPostCommentAsync()
        {
            var response = await _postCommentServices.GetAllPostCommentAsync();
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("createNewPostComment")]
        [Authorize(Roles= "module:blogComment action:add")]
        public async Task<IActionResult> CreateNewPostComment(CreatePostCommentDto postCommentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _postCommentServices.CreateNewPostComment(postCommentDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("updatePostComment/{id}")]
        [Authorize(Roles = "module:blogComment action:edit")]
        public async Task<IActionResult> UpdatePostComment(int id, UpdatePostCommentDto updatePostCommentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var rules = await _postCommentServices.UpdatePostComment(id, updatePostCommentDto);
            if (rules._success)
            {
                return Ok(rules);
            }
            return BadRequest(rules);
        }

        [HttpDelete("deletePostComment/{id}")]
        [Authorize(Roles = "module:blogComment action:delete")]
        public async Task<IActionResult> DeletePostComment(int id)
        {
            var response = await _postCommentServices.DeletePostComment(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("GetAllCommentByIdPost/{id}")]
        public async Task<IActionResult> GetAllCommentByIdPost(int id)
        {
            var response = await _postCommentServices.GetAllCommentByIdPost(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost("ReplyCommentOfPost")]
        public async Task<IActionResult> ReplyCommentOfPost(CreatePostCommentDto postCommentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _postCommentServices.ReplayCommentPost(postCommentDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
