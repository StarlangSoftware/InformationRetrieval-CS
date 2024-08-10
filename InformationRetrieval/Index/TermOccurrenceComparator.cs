using System.Collections.Generic;
using Dictionary.Dictionary;

namespace InformationRetrieval.Index
{
    public class TermOccurrenceComparator : IComparer<TermOccurrence>
    {
        private IComparer<Word> _comparator;

        /// <summary>
        /// Constructor for the TermOccurrenceComparator class. Sets the word comparator.
        /// </summary>
        /// <param name="comparator">Word comparator used in term occurrence comparator.</param>
        public TermOccurrenceComparator(IComparer<Word> comparator)
        {
            _comparator = comparator;
        }

        /// <summary>
        /// Compares two term occurrences.
        /// </summary>
        /// <param name="termA">the first term occurrence to be compared.</param>
        /// <param name="termB">the second term occurrence to be compared.</param>
        /// <returns>If the term of the first term occurrence is different from the term of the second term occurrence then
        /// the method returns the comparison result between those two terms lexicographically. If the term of the first term
        /// occurrence is same as the term of the second term occurrence then the term occurrences are compared with respect
        /// to their document ids. If the first has smaller document id, the method returns -1; if the second has smaller
        /// document id, the method returns +1.  As the third comparison criteria, if also the document ids are the same,
        /// the method compares term occurrences with respect to the position. If the first has smaller position, the method
        /// returns -1; if the second has smaller position id, the method returns +1, and if all three features are the same,
        /// the method returns 0.</returns>
        public int Compare(TermOccurrence termA, TermOccurrence termB)
        {
            var wordComparisonResult = _comparator.Compare(termA.GetTerm(), termB.GetTerm());
            if (wordComparisonResult != 0)
            {
                return wordComparisonResult;
            }
            else
            {
                if (termA.GetDocId() == termB.GetDocId())
                {
                    if (termA.GetPosition() == termB.GetPosition())
                    {
                        return 0;
                    }
                    else
                    {
                        if (termA.GetPosition() < termB.GetPosition())
                        {
                            return -1;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
                else
                {
                    if (termA.GetDocId() < termB.GetDocId())
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
        }
    }
}