using BlogCodeChallengeApi.Models;
using BlogCodeChallengeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogCodeChallengeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostService _blogPostService;
        private readonly ILogger<BlogPostsController> _logger;

        public BlogPostsController(IBlogPostService blogPostService, ILogger<BlogPostsController> logger)
        {
            _blogPostService = blogPostService;
            _logger = logger;
        }

        /// <summary>
        /// Get all blog posts
        /// </summary>
        /// <returns>A list of all blog posts</returns>
        /// <response code="200">Returns the list of blog posts</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BlogPost>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPosts()
        {
            var result = await _blogPostService.GetBlogPostsAsync();
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            _logger.LogError("An error occurred while retrieving blog posts: {Error}", result.Error);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving blog posts.");
        }

        /// <summary>
        /// Create a new blog post
        /// </summary>
        /// <param name="blogPost">The blog post to create</param>
        /// <returns>A success message</returns>
        /// <response code="200">If the blog post was created successfully</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PostBlogPost(BlogPost blogPost)
        {
            var result = await _blogPostService.CreateBlogPostAsync(blogPost);
            if (result.IsSuccess)
            {
                return Ok("Blog post created successfully.");
            }
            _logger.LogError("An error occurred while creating the blog post: {Error}", result.Error);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the blog post.");
        }

        /// <summary>
        /// Get a specific blog post by ID
        /// </summary>
        /// <param name="id">The ID of the blog post to retrieve</param>
        /// <returns>The requested blog post</returns>
        /// <response code="200">Returns the requested blog post</response>
        /// <response code="404">If the blog post was not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BlogPost), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BlogPost>> GetBlogPost(int id)
        {
            var result = await _blogPostService.GetBlogPostByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            if (result.Error.Contains("not found"))
            {
                return NotFound(result.Error);
            }
            _logger.LogError("An error occurred while retrieving the blog post with ID {Id}: {Error}", id, result.Error);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the blog post.");
        }

        /// <summary>
        /// Add a comment to a blog post
        /// </summary>
        /// <param name="id">The ID of the blog post to add the comment to</param>
        /// <param name="comment">The comment to add</param>
        /// <returns>A success message</returns>
        /// <response code="200">If the comment was added successfully</response>
        /// <response code="404">If the blog post was not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost("{id}/comments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PostComment(int id, Comment comment)
        {
            var result = await _blogPostService.AddCommentToBlogPostAsync(id, comment);
            if (result.IsSuccess)
            {
                return Ok("Comment added successfully.");
            }
            if (result.Error.Contains("not found"))
            {
                return NotFound(result.Error);
            }
            _logger.LogError("An error occurred while adding a comment to the blog post with ID {Id}: {Error}", id, result.Error);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the comment.");
        }
    }
}
