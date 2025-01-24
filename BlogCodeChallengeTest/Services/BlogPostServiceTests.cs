using AutoFixture;
using BlogCodeChallengeApi.Data;
using BlogCodeChallengeApi.Models;
using BlogCodeChallengeTest.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Legacy;

namespace BlogCodeChallengeTest.Services
{
    [TestFixture]
    public class BlogPostServiceTests
    {
        private Mock<BloggingContext> _mockContext;
        private Mock<ILogger<BlogPostService>> _mockLogger;
        private BlogPostService _service;
        private Fixture _fixture;
        private Mock<DatabaseFacade> _mockDatabase;

        [SetUp]
        public void SetUp()
        {
            _mockContext = new Mock<BloggingContext>(new DbContextOptions<BloggingContext>());
            _mockLogger = new Mock<ILogger<BlogPostService>>();
            _mockDatabase = new Mock<DatabaseFacade>(MockBehavior.Strict, _mockContext.Object);
            _mockContext.Setup(c => c.Database).Returns(_mockDatabase.Object);
            _mockDatabase.Setup(d => d.EnsureCreatedAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _service = new BlogPostService(_mockContext.Object, _mockLogger.Object);
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetBlogPostsAsync_ReturnsBlogPosts()
        {
            // Arrange
            var blogPosts = _fixture.CreateMany<BlogPost>().ToList();
            var dbSetMock = new Mock<DbSet<BlogPost>>();

            dbSetMock.As<IQueryable<BlogPost>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<BlogPost>(blogPosts.AsQueryable().Provider));

            dbSetMock.As<IQueryable<BlogPost>>()
                .Setup(m => m.Expression)
                .Returns(blogPosts.AsQueryable().Expression);

            dbSetMock.As<IQueryable<BlogPost>>()
                .Setup(m => m.ElementType)
                .Returns(blogPosts.AsQueryable().ElementType);

            dbSetMock.As<IQueryable<BlogPost>>()
                .Setup(m => m.GetEnumerator())
                .Returns(blogPosts.AsQueryable().GetEnumerator());

            dbSetMock.As<IAsyncEnumerable<BlogPost>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<BlogPost>(blogPosts.GetEnumerator()));

            _mockContext.Setup(c => c.BlogPosts).Returns(dbSetMock.Object);

            // Act
            var result = await _service.GetBlogPostsAsync();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            CollectionAssert.AreEqual(blogPosts, result.Value);
        }

        [Test]
        public async Task GetBlogPostsAsync_ReturnsFailureOnException()
        {
            // Arrange
            _mockContext.Setup(c => c.BlogPosts).Throws(new System.Exception());

            // Act
            var result = await _service.GetBlogPostsAsync();

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Failed to retrieve blog posts."));
        }

        [Test]
        public async Task GetBlogPostByIdAsync_ReturnsBlogPost()
        {
            // Arrange
            var blogPost = _fixture.Create<BlogPost>();
            var blogPosts = new List<BlogPost> { blogPost }.AsQueryable();
            var mockDbSet = new Mock<DbSet<BlogPost>>();

            mockDbSet.As<IQueryable<BlogPost>>()
                .Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<BlogPost>(blogPosts.Provider));
            mockDbSet.As<IQueryable<BlogPost>>()
                .Setup(m => m.Expression).Returns(blogPosts.Expression);
            mockDbSet.As<IQueryable<BlogPost>>()
                .Setup(m => m.ElementType).Returns(blogPosts.ElementType);
            mockDbSet.As<IQueryable<BlogPost>>()
                .Setup(m => m.GetEnumerator()).Returns(blogPosts.GetEnumerator());
            mockDbSet.Setup(m => m.FindAsync(blogPost.Id)).ReturnsAsync(blogPost);

            _mockContext.Setup(c => c.BlogPosts).Returns(mockDbSet.Object);

            // Act
            var result = await _service.GetBlogPostByIdAsync(blogPost.Id);

            // Assert
            Assert.That(result.IsSuccess, Is.True, "Expected the result to be successful.");
            Assert.That(result.Value, Is.EqualTo(blogPost), "Expected the returned blog post to match the mock blog post.");
        }

        [Test]
        public async Task GetBlogPostByIdAsync_ReturnsFailureWhenNotFound()
        {
            // Arrange
            var blogPostId = _fixture.Create<int>();
            var dbSetMock = new Mock<DbSet<BlogPost>>();
            dbSetMock.Setup(m => m.FindAsync(blogPostId)).ReturnsAsync((BlogPost)null);
            _mockContext.Setup(c => c.BlogPosts).Returns(dbSetMock.Object);

            // Act
            var result = await _service.GetBlogPostByIdAsync(blogPostId);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo($"Failed to retrieve blog post with ID {blogPostId}."));
        }

        [Test]
        public async Task GetBlogPostByIdAsync_ReturnsFailureOnException()
        {
            // Arrange
            var blogPostId = _fixture.Create<int>();
            _mockContext.Setup(c => c.BlogPosts).Throws(new System.Exception());

            // Act
            var result = await _service.GetBlogPostByIdAsync(blogPostId);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo($"Failed to retrieve blog post with ID {blogPostId}."));
        }

        [Test]
        public async Task CreateBlogPostAsync_ReturnsSuccess()
        {
            // Arrange
            var blogPost = _fixture.Create<BlogPost>();
            var blogPosts = new List<BlogPost>().AsQueryable();
            var comments = new List<Comment>().AsQueryable();

            var blogPostDbSetMock = new Mock<DbSet<BlogPost>>();
            blogPostDbSetMock.As<IQueryable<BlogPost>>()
                .Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<BlogPost>(blogPosts.Provider));
            blogPostDbSetMock.As<IQueryable<BlogPost>>()
                .Setup(m => m.Expression).Returns(blogPosts.Expression);
            blogPostDbSetMock.As<IQueryable<BlogPost>>()
                .Setup(m => m.ElementType).Returns(blogPosts.ElementType);
            blogPostDbSetMock.As<IQueryable<BlogPost>>()
                .Setup(m => m.GetEnumerator()).Returns(blogPosts.GetEnumerator());
            blogPostDbSetMock.Setup(m => m.AddAsync(blogPost, It.IsAny<CancellationToken>())).ReturnsAsync((EntityEntry<BlogPost>)null);

            var commentDbSetMock = new Mock<DbSet<Comment>>();
            commentDbSetMock.As<IQueryable<Comment>>()
                .Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Comment>(comments.Provider));
            commentDbSetMock.As<IQueryable<Comment>>()
                .Setup(m => m.Expression).Returns(comments.Expression);
            commentDbSetMock.As<IQueryable<Comment>>()
                .Setup(m => m.ElementType).Returns(comments.ElementType);
            commentDbSetMock.As<IQueryable<Comment>>()
                .Setup(m => m.GetEnumerator()).Returns(comments.GetEnumerator());

            _mockContext.Setup(c => c.BlogPosts).Returns(blogPostDbSetMock.Object);
            _mockContext.Setup(c => c.Comments).Returns(commentDbSetMock.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _service.CreateBlogPostAsync(blogPost);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(blogPost));
        }

        [Test]
        public async Task CreateBlogPostAsync_ReturnsFailureOnException()
        {
            // Arrange
            var blogPost = _fixture.Create<BlogPost>();
            _mockContext.Setup(c => c.BlogPosts).Throws(new System.Exception());

            // Act
            var result = await _service.CreateBlogPostAsync(blogPost);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Failed to create blog post."));
        }

        [Test]
        public async Task AddCommentToBlogPostAsync_ReturnsSuccess()
        {
            // Arrange
            var blogPost = _fixture.Create<BlogPost>();
            var comment = _fixture.Create<Comment>();
            var blogPosts = new List<BlogPost> { blogPost }.AsQueryable();
            var comments = new List<Comment>().AsQueryable();

            var blogPostDbSetMock = new Mock<DbSet<BlogPost>>();
            blogPostDbSetMock.As<IQueryable<BlogPost>>()
                .Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<BlogPost>(blogPosts.Provider));
            blogPostDbSetMock.As<IQueryable<BlogPost>>()
                .Setup(m => m.Expression).Returns(blogPosts.Expression);
            blogPostDbSetMock.As<IQueryable<BlogPost>>()
                .Setup(m => m.ElementType).Returns(blogPosts.ElementType);
            blogPostDbSetMock.As<IQueryable<BlogPost>>()
                .Setup(m => m.GetEnumerator()).Returns(blogPosts.GetEnumerator());
            blogPostDbSetMock.Setup(m => m.FindAsync(blogPost.Id)).ReturnsAsync(blogPost);

            var commentDbSetMock = new Mock<DbSet<Comment>>();
            commentDbSetMock.As<IQueryable<Comment>>()
                .Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Comment>(comments.Provider));
            commentDbSetMock.As<IQueryable<Comment>>()
                .Setup(m => m.Expression).Returns(comments.Expression);
            commentDbSetMock.As<IQueryable<Comment>>()
                .Setup(m => m.ElementType).Returns(comments.ElementType);
            commentDbSetMock.As<IQueryable<Comment>>()
                .Setup(m => m.GetEnumerator()).Returns(comments.GetEnumerator());

            _mockContext.Setup(c => c.BlogPosts).Returns(blogPostDbSetMock.Object);
            _mockContext.Setup(c => c.Comments).Returns(commentDbSetMock.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _service.AddCommentToBlogPostAsync(blogPost.Id, comment);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(comment));
        }


        [Test]
        public async Task AddCommentToBlogPostAsync_ReturnsFailureWhenBlogPostNotFound()
        {
            // Arrange
            var blogPostId = _fixture.Create<int>();
            var comment = _fixture.Create<Comment>();
            var dbSetMock = new Mock<DbSet<BlogPost>>();
            dbSetMock.Setup(m => m.FindAsync(blogPostId)).ReturnsAsync((BlogPost)null);
            _mockContext.Setup(c => c.BlogPosts).Returns(dbSetMock.Object);

            // Act
            var result = await _service.AddCommentToBlogPostAsync(blogPostId, comment);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo($"Blog post with ID {blogPostId} not found."));
        }

        [Test]
        public async Task AddCommentToBlogPostAsync_ReturnsFailureOnException()
        {
            // Arrange
            var blogPostId = _fixture.Create<int>();
            var comment = _fixture.Create<Comment>();
            _mockContext.Setup(c => c.BlogPosts).Throws(new System.Exception());

            // Act
            var result = await _service.AddCommentToBlogPostAsync(blogPostId, comment);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Failed to add comment to blog post."));
        }
    }
}
