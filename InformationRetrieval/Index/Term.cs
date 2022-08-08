using Dictionary.Dictionary;

namespace InformationRetrieval.Index
{
    public class Term : Word
    {
        private int _termId;

        public Term(string name, int termId) : base(name)
        {
            _termId = termId;
        }

        public int GetTermId()
        {
            return _termId;
        }
    }
}