using AutoFixture;
using BlogCodeChallengeApi.Common;
using BlogCodeChallengeApi.Controllers;
using BlogCodeChallengeApi.Models;
using BlogCodeChallengeApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace BlogCodeChallengeTest.Controllers
{
    [TestFixture]
    public class BlogPostsControllerTests
    {
        private Mock<IBlogPostService> _mockBlogPostService;
        private Mock<ILogger<BlogPostsController>> _mockLogger;
        private BlogPostsController _controller;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockBlogPostService = new Mock<IBlogPostService>();
            _mockLogger = new Mock<ILogger<BlogPostsController>>();
            _controller = new BlogPostsController(_mockBlogPostService.Object, _mockLogger.Object);
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetBlogPosts_ReturnsOkResult_WithListOfBlogPosts()
        {
            // Arrange
            var blogPosts = _fixture.CreateMany<BlogPost>();
            _mockBlogPostService.Setup(service => service.GetBlogPostsAsync())
                .ReturnsAsync(Result<IEnumerable<BlogPost>>.Success(blogPosts));

            // Act
            var result = await _controller.GetBlogPosts();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(okResult.Value, Is.EqualTo(blogPosts));
        }

        [Test]
        public async Task GetBlogPosts_ReturnsInternalServerError_WhenServiceFails()
        {
            // Arrange
            _mockBlogPostService.Setup(service => service.GetBlogPostsAsync())
                .ReturnsAsync(Result<IEnumerable<BlogPost>>.Failure("Service error"));

            // Act
            var result = await _controller.GetBlogPosts();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(objectResult.Value, Is.EqualTo("An error occurred while retrieving blog posts."));
        }

        [Test]
        public async Task PostBlogPost_ReturnsOkResult_WhenBlogPostIsCreated()
        {
            // Arrange
            var blogPost = _fixture.Create<BlogPost>();
            _mockBlogPostService.Setup(service => service.CreateBlogPostAsync(blogPost))
                .ReturnsAsync(Result<BlogPost>.Success(blogPost));

            // Act
            var result = await _controller.PostBlogPost(blogPost);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(okResult.Value, Is.EqualTo("Blog post created successfully."));
        }

        [Test]
        public async Task PostBlogPost_ReturnsInternalServerError_WhenServiceFails()
        {
            // Arrange
            var blogPost = _fixture.Create<BlogPost>();
            _mockBlogPostService.Setup(service => service.CreateBlogPostAsync(blogPost))
                .ReturnsAsync(Result<BlogPost>.Failure("Service error"));

            // Act
            var result = await _controller.PostBlogPost(blogPost);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(objectResult.Value, Is.EqualTo("An error occurred while creating the blog post."));
        }

        [Test]
        public async Task GetBlogPost_ReturnsOkResult_WithBlogPost()
        {
            // Arrange
            var blogPost = _fixture.Create<BlogPost>();
            _mockBlogPostService.Setup(service => service.GetBlogPostByIdAsync(blogPost.Id))
                .ReturnsAsync(Result<BlogPost>.Success(blogPost));

            // Act
            var result = await _controller.GetBlogPost(blogPost.Id);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(okResult.Value, Is.EqualTo(blogPost));
        }

        [Test]
        public async Task GetBlogPost_ReturnsNotFound_WhenBlogPostNotFound()
        {
            // Arrange
            var blogPostId = _fixture.Create<int>();
            _mockBlogPostService.Setup(service => service.GetBlogPostByIdAsync(blogPostId))
                .ReturnsAsync(Result<BlogPost>.Failure("Blog post not found"));

            // Act
            var result = await _controller.GetBlogPost(blogPostId);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(notFoundResult.Value, Is.EqualTo("Blog post not found"));
        }

        [Test]
        public async Task GetBlogPost_ReturnsInternalServerError_WhenServiceFails()
        {
            // Arrange
            var blogPostId = _fixture.Create<int>();
            _mockBlogPostService.Setup(service => service.GetBlogPostByIdAsync(blogPostId))
                .ReturnsAsync(Result<BlogPost>.Failure("Service error"));

            // Act
            var result = await _controller.GetBlogPost(blogPostId);

            // Assert
            Assert.That(result.Result, Is.TypeOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(objectResult.Value, Is.EqualTo("An error occurred while retrieving the blog post."));
        }

        [Test]
        public async Task PostComment_ReturnsOkResult_WhenCommentIsAdded()
        {
            // Arrange
            var comment = _fixture.Create<Comment>();
            _mockBlogPostService.Setup(service => service.AddCommentToBlogPostAsync(comment.BlogPostId, comment))
                .ReturnsAsync(Result<Comment>.Success(comment));

            // Act
            var result = await _controller.PostComment(comment.BlogPostId, comment);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(okResult.Value, Is.EqualTo("Comment added successfully."));
        }

        [Test]
        public async Task PostComment_ReturnsNotFound_WhenBlogPostNotFound()
        {
            // Arrange
            var comment = _fixture.Create<Comment>();
            _mockBlogPostService.Setup(service => service.AddCommentToBlogPostAsync(comment.BlogPostId, comment))
                .ReturnsAsync(Result<Comment>.Failure("Blog post not found"));

            // Act
            var result = await _controller.PostComment(comment.BlogPostId, comment);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(notFoundResult.Value, Is.EqualTo("Blog post not found"));
        }

        [Test]
        public async Task PostComment_ReturnsInternalServerError_WhenServiceFails()
        {
            // Arrange
            var comment = _fixture.Create<Comment>();
            _mockBlogPostService.Setup(service => service.AddCommentToBlogPostAsync(comment.BlogPostId, comment))
                .ReturnsAsync(Result<Comment>.Failure("Service error"));

            // Act
            var result = await _controller.PostComment(comment.BlogPostId, comment);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(objectResult.Value, Is.EqualTo("An error occurred while adding the comment."));
        }
    }
}
