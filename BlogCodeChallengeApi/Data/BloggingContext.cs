using BlogCodeChallengeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogCodeChallengeApi.Data
{
    public class BloggingContext : DbContext
    {
        public BloggingContext(DbContextOptions<BloggingContext> options) : base(options) { }

        public virtual DbSet<BlogPost> BlogPosts { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
    }
}
