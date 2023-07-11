using System;
using Microsoft.AspNetCore.Mvc;
using Streamish.Repositories;
using Streamish.Models;
using Streamish.Utils;


namespace Streamish.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;

        public CommentController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_commentRepository.GetAll());
        }



    }
}
