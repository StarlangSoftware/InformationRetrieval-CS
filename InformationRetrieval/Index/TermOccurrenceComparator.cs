using System.Collections.Generic;
using Dictionary.Dictionary;

namespace InformationRetrieval.Index
{
    public class TermOccurrenceComparator : IComparer<TermOccurrence>
    {
        private IComparer<Word> _comparator;

        public TermOccurrenceComparator(IComparer<Word> comparator)
        {
            _comparator = comparator;
        }

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