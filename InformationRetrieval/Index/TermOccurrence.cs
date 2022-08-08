using System.Collections.Generic;
using Dictionary.Dictionary;

namespace InformationRetrieval.Index
{
    public class TermOccurrence
    {
        private Word _term;
        private int _docId;
        private int _position;

        public TermOccurrence(Word term, int docId, int position)
        {
            _term = term;
            _docId = docId;
            _position = position;
        }

        public Word GetTerm()
        {
            return _term;
        }

        public int GetDocId()
        {
            return _docId;
        }

        public int GetPosition()
        {
            return _position;
        }

        public bool IsDifferent(TermOccurrence currentTerm, IComparer<Word> comparator)
        {
            return comparator.Compare(_term, currentTerm.GetTerm()) != 0;
        }
    }
}