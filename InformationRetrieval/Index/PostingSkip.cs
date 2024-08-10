namespace InformationRetrieval.Index
{
    public class PostingSkip : Posting
    {
        private bool _skipAvailable;
        private PostingSkip _skip;
        private PostingSkip _next;

        /// <summary>
        /// Constructor for the PostingSkip class. Sets the document id.
        /// </summary>
        /// <param name="id">Document id.</param>
        public PostingSkip(int id) : base(id)
        {
        }

        /// <summary>
        /// Checks if this posting has a skip pointer or not.
        /// </summary>
        /// <returns>True, if this posting has a skip pointer, false otherwise.</returns>
        public bool HasSkip()
        {
            return _skipAvailable;
        }

        /// <summary>
        /// Adds a skip pointer to the next skip posting.
        /// </summary>
        /// <param name="skip">Next posting to jump.</param>
        public void AddSkip(PostingSkip skip)
        {
            _skipAvailable = true;
            _skip = skip;
        }

        /// <summary>
        /// Updated the skip pointer.
        /// </summary>
        /// <param name="next">New skip pointer</param>
        public void SetNext(PostingSkip next)
        {
            _next = next;
        }

        /// <summary>
        /// Accessor for the skip pointer.
        /// </summary>
        /// <returns>Next posting to skip.</returns>
        public PostingSkip Next()
        {
            return _next;
        }

        /// <summary>
        /// Accessor for the skip.
        /// </summary>
        /// <returns>Skip</returns>
        public PostingSkip GetSkip()
        {
            return _skip;
        }
    }
}