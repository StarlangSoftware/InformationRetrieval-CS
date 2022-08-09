using System.Collections.Generic;
using Dictionary.Dictionary;

namespace InformationRetrieval.Index
{
    public class NGramIndex : InvertedIndex
    {
        public NGramIndex() : base()
        {
        }

        public NGramIndex(TermDictionary dictionary, List<TermOccurrence> terms, IComparer<Word> comparator) : base(
            dictionary, terms, comparator)
        {
        }

        public NGramIndex(string fileName) : base(fileName)
        {
        }
    }
}