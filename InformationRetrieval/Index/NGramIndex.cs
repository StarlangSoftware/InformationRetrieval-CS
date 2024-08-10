using System.Collections.Generic;
using Dictionary.Dictionary;

namespace InformationRetrieval.Index
{
    public class NGramIndex : InvertedIndex
    {
        /**
         * Empty constructor for the NGram index.
         */
        public NGramIndex() : base()
        {
        }

        /// <summary>
        /// Constructs an NGram index from a list of sorted tokens. The terms array should be sorted before calling this
        /// method. Calls the constructor for the InvertedIndex.
        /// </summary>
        /// <param name="dictionary">Term dictionary</param>
        /// <param name="terms">Sorted list of tokens in the memory collection.</param>
        /// <param name="comparator">Comparator method to compare two terms.</param>
        public NGramIndex(TermDictionary dictionary, List<TermOccurrence> terms, IComparer<Word> comparator) : base(
            dictionary, terms, comparator)
        {
        }

        /// <summary>
        /// Reads the NGram index from an input file.
        /// </summary>
        /// <param name="fileName">Input file name for the NGram index.</param>
        public NGramIndex(string fileName) : base(fileName)
        {
        }
    }
}