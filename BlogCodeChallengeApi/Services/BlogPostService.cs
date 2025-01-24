using BlogCodeChallengeApi.Common;
using BlogCodeChallengeApi.Data;
using BlogCodeChallengeApi.Models;
using BlogCodeChallengeApi.Services;
using Microsoft.EntityFrameworkCore;

public class BlogPostService : IBlogPostService
{
    private readonly BloggingContext _context;
    private readonly ILogger<BlogPostService> _logger;

    public BlogPostService(BloggingContext context, ILogger<BlogPostService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<BlogPost>>> GetBlogPostsAsync()
    {
        try
        {
            var blogPosts = await _context.BlogPosts
                .Include(bp => bp.Comments)
                .ToListAsync();
            return Result<IEnumerable<BlogPost>>.Success(blogPosts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving blog posts.");
            return Result<IEnumerable<BlogPost>>.Failure("Failed to retrieve blog posts.");
        }
    }

    public async Task<Result<BlogPost>> GetBlogPostByIdAsync(int id)
    {
        try
        {
            var blogPost = await _context.BlogPosts
                .Include(bp => bp.Comments)
                .FirstOrDefaultAsync(bp => bp.Id == id);

            if (blogPost == null)
            {
                return Result<BlogPost>.Failure($"Blog post with ID {id} not found.");
            }

            return Result<BlogPost>.Success(blogPost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while retrieving the blog post with ID {id}.");
            return Result<BlogPost>.Failure($"Failed to retrieve blog post with ID {id}.");
        }
    }

    public async Task<Result<BlogPost>> CreateBlogPostAsync(BlogPost blogPost)
    {
        try
        {
            var existingPost = await _context.BlogPosts
                .FirstOrDefaultAsync(bp => bp.Title == blogPost.Title && bp.Content == blogPost.Content);

            if (existingPost != null)
            {
                return Result<BlogPost>.Failure("A blog post with the same title and content already exists.");
            }

            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();
            return Result<BlogPost>.Success(blogPost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the blog post.");
            return Result<BlogPost>.Failure("Failed to create blog post.");
        }
    }

    public async Task<Result<Comment>> AddCommentToBlogPostAsync(int blogPostId, Comment comment)
    {
        try
        {
            var blogPost = await _context.BlogPosts.FindAsync(blogPostId);
            if (blogPost == null)
            {
                return Result<Comment>.Failure($"Blog post with ID {blogPostId} not found.");
            }

            var existingComment = await _context.Comments
                .FirstOrDefaultAsync(c => c.BlogPostId == blogPostId && c.Content == comment.Content);

            if (existingComment != null)
            {
                return Result<Comment>.Failure("A comment with the same content already exists for this blog post.");
            }

            comment.BlogPostId = blogPostId;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return Result<Comment>.Success(comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while adding a comment to the blog post with ID {blogPostId}.");
            return Result<Comment>.Failure("Failed to add comment to blog post.");
        }
    }
}
