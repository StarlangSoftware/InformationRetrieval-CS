using Dictionary.Dictionary;

namespace InformationRetrieval.Index
{
    public class Term : Word
    {
        private int _termId;

        /// <summary>
        /// Constructor for the Term class. Sets the fields.
        /// </summary>
        /// <param name="name">Text of the term</param>
        /// <param name="termId">Id of the term</param>
        public Term(string name, int termId) : base(name)
        {
            _termId = termId;
        }

        /// <summary>
        /// Accessor for the term id attribute.
        /// </summary>
        /// <returns>Term id attribute</returns>
        public int GetTermId()
        {
            return _termId;
        }
    }
}