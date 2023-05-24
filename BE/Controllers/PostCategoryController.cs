using BE.Data.Dtos.UserDtos;
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
    public class PostCategoryController : ControllerBase
    {
        private readonly IPostCategoryServices _postCategoryServices;
        public PostCategoryController(IPostCategoryServices postCategoryServices)
        {
            _postCategoryServices = postCategoryServices;
        }
        [HttpGet("getAllPostCategoryAsync")]
        public async Task<IActionResult> GetAllPostCategoryAsync()
        {
            var response = await _postCategoryServices.GetAllPostCategoryAsync();
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getPostCategoryByIdAsync/{id}")]
        public async Task<ActionResult<Users>> GetPostCategoryByIDAsync(int id)
        {
            var response = await _postCategoryServices.GetPostCategoryByIDAsync(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}
