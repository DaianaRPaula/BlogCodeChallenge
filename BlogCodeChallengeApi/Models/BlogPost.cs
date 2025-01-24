namespace BlogCodeChallengeApi.Models
{
    public class BlogPost
    {
        /// <summary>
        /// The unique identifier of the blog post
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The title of the blog post
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The content of the blog post
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The list of comments on the blog post
        /// </summary>
        public List<Comment> Comments { get; set; }
    }
}
