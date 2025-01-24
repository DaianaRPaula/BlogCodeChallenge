namespace BlogCodeChallengeApi.Models
{
    public class Comment
    {
        /// <summary>
        /// The unique identifier of the comment
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the blog post this comment belongs to
        /// </summary>
        public int BlogPostId { get; set; }

        /// <summary>
        /// The content of the comment
        /// </summary>
        public string Content { get; set; }
    }
}
