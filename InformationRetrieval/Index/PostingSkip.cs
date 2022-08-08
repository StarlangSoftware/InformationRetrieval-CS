namespace InformationRetrieval.Index
{
    public class PostingSkip : Posting
    {
        private bool _skipAvailable;
        private PostingSkip _skip;
        private PostingSkip _next;

        public PostingSkip(int id) : base(id)
        {
        }

        public bool HasSkip()
        {
            return _skipAvailable;
        }

        public void AddSkip(PostingSkip skip)
        {
            _skipAvailable = true;
            _skip = skip;
        }

        public void SetNext(PostingSkip next)
        {
            _next = next;
        }

        public PostingSkip Next()
        {
            return _next;
        }

        public PostingSkip GetSkip()
        {
            return _skip;
        }
    }
}