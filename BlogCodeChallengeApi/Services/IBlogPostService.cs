using BlogCodeChallengeApi.Common;
using BlogCodeChallengeApi.Models;

namespace BlogCodeChallengeApi.Services
{
    public interface IBlogPostService
    {
        Task<Result<IEnumerable<BlogPost>>> GetBlogPostsAsync();
        Task<Result<BlogPost>> GetBlogPostByIdAsync(int id);
        Task<Result<BlogPost>> CreateBlogPostAsync(BlogPost blogPost);
        Task<Result<Comment>> AddCommentToBlogPostAsync(int blogPostId, Comment comment);
    }
}
