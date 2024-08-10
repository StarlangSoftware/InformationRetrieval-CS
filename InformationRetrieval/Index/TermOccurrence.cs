using System.Collections.Generic;
using Dictionary.Dictionary;

namespace InformationRetrieval.Index
{
    public class TermOccurrence
    {
        private Word _term;
        private int _docId;
        private int _position;

        /// <summary>
        /// Constructor for the TermOccurrence class. Sets the attributes.
        /// </summary>
        /// <param name="term">Term for this occurrence.</param>
        /// <param name="docId">Document id of the term occurrence.</param>
        /// <param name="position">Position of the term in the document for this occurrence.</param>
        public TermOccurrence(Word term, int docId, int position)
        {
            _term = term;
            _docId = docId;
            _position = position;
        }

        /// <summary>
        /// Accessor for the term.
        /// </summary>
        /// <returns>Term</returns>
        public Word GetTerm()
        {
            return _term;
        }

        /// <summary>
        /// Accessor for the document id.
        /// </summary>
        /// <returns>Document id.</returns>
        public int GetDocId()
        {
            return _docId;
        }

        /// <summary>
        /// Accessor for the position of the term.
        /// </summary>
        /// <returns>Position of the term.</returns>
        public int GetPosition()
        {
            return _position;
        }

        /// <summary>
        /// Checks if the current occurrence is different from the other occurrence.
        /// </summary>
        /// <param name="currentTerm">Term occurrence to be compared.</param>
        /// <param name="comparator">Comparator function to compare two terms.</param>
        /// <returns>True, if two terms are different; false if they are the same.</returns>
        public bool IsDifferent(TermOccurrence currentTerm, IComparer<Word> comparator)
        {
            return comparator.Compare(_term, currentTerm.GetTerm()) != 0;
        }
    }
}