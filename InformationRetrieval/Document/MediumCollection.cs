using System.Collections.Generic;
using InformationRetrieval.Index;

namespace InformationRetrieval.Document
{
    public class MediumCollection : DiskCollection
    {
        /// <summary>
        /// Constructor for the MediumCollection class. In medium collections, dictionary is kept in memory and indexes are
        /// stored in the disk and don't fit in memory in their construction phase and usage phase. For that reason, in their
        /// construction phase, multiple disk reads and optimizations are needed.
        /// </summary>
        /// <param name="directory">Directory where the document collection resides.</param>
        /// <param name="parameter">Search parameter</param>
        public MediumCollection(string directory, Parameter parameter) : base(directory, parameter)
        {
            ConstructIndexesInDisk();
        }

        /// <summary>
        /// Given the document collection, creates a hash set of distinct terms. If term type is TOKEN, the terms are single
        /// word, if the term type is PHRASE, the terms are bi-words. Each document is loaded into memory and distinct
        /// word list is created. Since the dictionary can be kept in memory, all operations can be done in memory.
        /// </summary>
        /// <param name="termType">If term type is TOKEN, the terms are single word, if the term type is PHRASE, the terms are
        /// bi-words.</param>
        /// <returns>Hash set of terms occurring in the document collection.</returns>
        private HashSet<string> ConstructDistinctWordList(TermType termType)
        {
            var words = new HashSet<string>();
            foreach (var doc in Documents)
            {
                var documentText = doc.LoadDocument();
                words.UnionWith(documentText.ConstructDistinctWordList(termType));
            }

            return words;
        }

        /// <summary>
        /// In block sort based indexing, the indexes are created in a block wise manner. They do not fit in memory, therefore
        /// documents are read one by one. According to the search parameter, inverted index, positional index, phrase
        /// indexes, N-Gram indexes are constructed in disk.
        /// </summary>
        private void ConstructIndexesInDisk()
        {
            var wordList = ConstructDistinctWordList(TermType.TOKEN);
            Dictionary = new TermDictionary(Comparator, wordList);
            ConstructInvertedIndexInDisk(Dictionary, TermType.TOKEN);
            if (Parameter.ConstructPositionalIndex())
            {
                ConstructPositionalIndexInDisk(Dictionary, TermType.TOKEN);
            }

            if (Parameter.ConstructPhraseIndex())
            {
                wordList = ConstructDistinctWordList(TermType.PHRASE);
                PhraseDictionary = new TermDictionary(Comparator, wordList);
                ConstructInvertedIndexInDisk(PhraseDictionary, TermType.PHRASE);
                if (Parameter.ConstructPositionalIndex())
                {
                    ConstructPositionalIndexInDisk(PhraseDictionary, TermType.PHRASE);
                }
            }

            if (Parameter.ConstructNGramIndex())
            {
                ConstructNGramIndex();
            }
        }

        /// <summary>
        /// In block sort based indexing, the inverted index is created in a block wise manner. It does not fit in memory,
        /// therefore documents are read one by one. For each document, the terms are added to the inverted index. If the
        /// number of documents read are above the limit, current partial inverted index file is saved and new inverted index
        /// file is open. After reading all documents, we combine the inverted index files to get the final inverted index
        /// file.
        /// </summary>
        /// <param name="dictionary">Term dictionary.</param>
        /// <param name="termType">If term type is TOKEN, the terms are single word, if the term type is PHRASE, the terms are
        /// bi-words.</param>
        private void ConstructInvertedIndexInDisk(TermDictionary dictionary, TermType termType)
        {
            int i = 0, blockCount = 0;
            var invertedIndex = new InvertedIndex();
            foreach (Document doc in Documents)
            {
                if (i < Parameter.GetDocumentLimit())
                {
                    i++;
                }
                else
                {
                    invertedIndex.Save("tmp-" + blockCount);
                    invertedIndex = new InvertedIndex();
                    blockCount++;
                    i = 0;
                }

                var documentText = doc.LoadDocument();
                var wordList = documentText.ConstructDistinctWordList(termType);
                foreach (string word in wordList)
                {
                    int termId = dictionary.GetWordIndex(word);
                    invertedIndex.Add(termId, doc.GetDocId());
                }
            }

            if (Documents.Count != 0)
            {
                invertedIndex.Save("tmp-" + blockCount);
                blockCount++;
            }

            if (termType == TermType.TOKEN)
            {
                CombineMultipleInvertedIndexesInDisk(Name, "", blockCount);
            }
            else
            {
                CombineMultipleInvertedIndexesInDisk(Name + "-phrase", "", blockCount);
            }
        }

        /// <summary>
        /// In block sort based indexing, the positional index is created in a block wise manner. It does not fit in memory,
        /// therefore documents are read one by one. For each document, the terms are added to the positional index. If the
        /// number of documents read are above the limit, current partial positional index file is saved and new positional
        /// index file is open. After reading all documents, we combine the posiitonal index files to get the final
        /// positional index file.
        /// </summary>
        /// <param name="dictionary">Term dictionary.</param>
        /// <param name="termType">If term type is TOKEN, the terms are single word, if the term type is PHRASE, the terms are
        /// bi-words.</param>
        private void ConstructPositionalIndexInDisk(TermDictionary dictionary, TermType termType)
        {
            int i = 0, blockCount = 0;
            var positionalIndex = new PositionalIndex();
            foreach (var doc in Documents)
            {
                if (i < Parameter.GetDocumentLimit())
                {
                    i++;
                }
                else
                {
                    positionalIndex.Save("tmp-" + blockCount);
                    positionalIndex = new PositionalIndex();
                    blockCount++;
                    i = 0;
                }

                var documentText = doc.LoadDocument();
                var terms = documentText.ConstructTermList(doc.GetDocId(), termType);
                foreach (TermOccurrence termOccurrence in terms)
                {
                    var termId = dictionary.GetWordIndex(termOccurrence.GetTerm().GetName());
                    positionalIndex.AddPosition(termId, termOccurrence.GetDocId(), termOccurrence.GetPosition());
                }
            }

            if (Documents.Count != 0)
            {
                positionalIndex.Save("tmp-" + blockCount);
                blockCount++;
            }

            if (termType == TermType.TOKEN)
            {
                CombineMultiplePositionalIndexesInDisk(Name, blockCount);
            }
            else
            {
                CombineMultiplePositionalIndexesInDisk(Name + "-phrase", blockCount);
            }
        }
    }
}