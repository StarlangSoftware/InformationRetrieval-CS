using System;
using System.Collections.Generic;
using System.IO;
using Dictionary.Dictionary;
using InformationRetrieval.Index;

namespace InformationRetrieval.Document
{
    public class LargeCollection : DiskCollection
    {
        /// <summary>
        /// Constructor for the LargeCollection class. In large collections, both dictionary and indexes are stored in the
        /// disk and don't fit in memory in their construction phase and usage phase. For that reason, in their construction
        /// phase, multiple disk reads and optimizations are needed.
        /// </summary>
        /// <param name="directory">Directory where the document collection resides.</param>
        /// <param name="parameter">Search parameter</param>
        public LargeCollection(string directory, Parameter parameter) : base(directory, parameter)
        {
            ConstructDictionaryAndIndexesInDisk();
        }

        /**
         * The method constructs the term dictionary and all indexes on disk.
         */
        private void ConstructDictionaryAndIndexesInDisk()
        {
            ConstructDictionaryAndInvertedIndexInDisk(TermType.TOKEN);
            if (Parameter.ConstructPositionalIndex())
            {
                ConstructDictionaryAndPositionalIndexInDisk(TermType.TOKEN);
            }

            if (Parameter.ConstructPhraseIndex())
            {
                ConstructDictionaryAndInvertedIndexInDisk(TermType.PHRASE);
                if (Parameter.ConstructPositionalIndex())
                {
                    ConstructDictionaryAndPositionalIndexInDisk(TermType.PHRASE);
                }
            }

            if (Parameter.ConstructNGramIndex())
            {
                ConstructNGramDictionaryAndIndexInDisk();
            }
        }

        /// <summary>
        /// In single pass in memory indexing, the dictionary files are merged to get the final dictionary file. This method
        /// checks if all parallel dictionaries are combined or not.
        /// </summary>
        /// <param name="currentWords">Current pointers for the words in parallel dictionaries. currentWords[0] is the current word
        ///                     in the first dictionary to be combined, currentWords[2] is the current word in the second
        ///                     dictionary to be combined etc.</param>
        /// <returns>True, if all merge operation is completed, false otherwise.</returns>
        private bool NotCombinedAllDictionaries(string[] currentWords)
        {
            foreach (var word in currentWords)
            {
                if (word != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// In single pass in memory indexing, the dictionary files are merged to get the final dictionary file. This method
        /// identifies the dictionaries whose words to be merged are lexicographically the first. They will be selected and
        /// combined in the next phase.
        /// </summary>
        /// <param name="currentWords">Current pointers for the words in parallel dictionaries. currentWords[0] is the current word
        ///                     in the first dictionary to be combined, currentWords[2] is the current word in the second
        ///                     dictionary to be combined etc.</param>
        /// <returns>An array list of indexes for the dictionaries, whose words to be merged are lexicographically the first.</returns>
        private List<int> SelectDictionariesWithMinimumWords(string[] currentWords)
        {
            var result = new List<int>();
            string min = null;
            foreach (var word in currentWords)
            {
                if (word != null && (min == null || Comparator.Compare(new Word(word), new Word(min)) < 0))
                {
                    min = word;
                }
            }

            for (var i = 0; i < currentWords.Length; i++)
            {
                if (currentWords[i] != null && currentWords[i].Equals(min))
                {
                    result.Add(i);
                }
            }

            return result;
        }

        /// <summary>
        /// In single pass in memory indexing, the dictionary files are merged to get the final dictionary file. This method
        /// implements the merging algorithm. Reads the dictionary files in parallel and at each iteration puts the smallest
        /// word to the final dictionary. Updates the pointers of the dictionaries accordingly.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="tmpName">Temporary name of the dictionary files.</param>
        /// <param name="blockCount">Number of dictionaries to be merged.</param>
        private void CombineMultipleDictionariesInDisk(string collectionName, string tmpName, int blockCount)
        {
            StreamReader[] files;
            int[] currentIdList;
            string[] currentWords;
            currentIdList = new int[blockCount];
            currentWords = new string[blockCount];
            files = new StreamReader[blockCount];
            var printWriter = new StreamWriter(collectionName + "-dictionary.txt");
            for (var i = 0; i < blockCount; i++)
            {
                files[i] = new StreamReader("tmp-" + tmpName + i + "-dictionary.txt");
                var line = files[i].ReadLine();
                currentIdList[i] = int.Parse(line.Substring(0, line.IndexOf(" ", StringComparison.Ordinal)));
                currentWords[i] = line.Substring(line.IndexOf(" ", StringComparison.Ordinal) + 1);
            }

            while (NotCombinedAllDictionaries(currentWords))
            {
                var indexesToCombine = SelectDictionariesWithMinimumWords(currentWords);
                printWriter.Write(currentIdList[indexesToCombine[0]] + " " +
                                  currentWords[indexesToCombine[0]] + "\n");
                foreach (var i in indexesToCombine)
                {
                    var line = files[i].ReadLine();
                    if (line != null)
                    {
                        currentIdList[i] = int.Parse(line.Substring(0, line.IndexOf(" ", StringComparison.Ordinal)));
                        currentWords[i] = line.Substring(line.IndexOf(" ", StringComparison.Ordinal) + 1);
                    }
                    else
                    {
                        currentWords[i] = null;
                    }
                }
            }

            for (var i = 0; i < blockCount; i++)
            {
                files[i].Close();
            }

            printWriter.Close();
        }

        /// <summary>
        /// In single pass in memory indexing, the dictionaries and inverted indexes are created in a block wise manner. They
        /// do not fit in memory, therefore documents are read one by one. For each document, the terms are added to the
        /// current dictionary and inverted index. If the number of documents read are above the limit, current partial
        /// dictionary and inverted index file are saved and new dictionary and inverted index file are open. After reading
        ///  all  documents, we combine the dictionary and inverted index files to get the final dictionary and inverted index
        /// file.
        /// </summary>
        /// <param name="termType">If term type is TOKEN, the terms are single word, if the term type is PHRASE, the terms are
        ///                 bi-words.</param>
        private void ConstructDictionaryAndInvertedIndexInDisk(TermType termType)
        {
            int i = 0, blockCount = 0;
            var invertedIndex = new InvertedIndex();
            var dictionary = new TermDictionary(Comparator);
            foreach (Document doc in Documents)
            {
                if (i < Parameter.GetDocumentLimit())
                {
                    i++;
                }
                else
                {
                    dictionary.Save("tmp-" + blockCount);
                    dictionary = new TermDictionary(Comparator);
                    invertedIndex.Save("tmp-" + blockCount);
                    invertedIndex = new InvertedIndex();
                    blockCount++;
                    i = 0;
                }

                var documentText = doc.LoadDocument();
                var wordList = documentText.ConstructDistinctWordList(termType);
                foreach (string word in wordList)
                {
                    int termId;
                    int wordIndex = dictionary.GetWordIndex(word);
                    if (wordIndex != -1)
                    {
                        termId = ((Term)dictionary.GetWord(wordIndex)).GetTermId();
                    }
                    else
                    {
                        termId = System.Math.Abs(word.GetHashCode());
                        dictionary.AddTerm(word, termId);
                    }

                    invertedIndex.Add(termId, doc.GetDocId());
                }
            }

            if (Documents.Count != 0)
            {
                dictionary.Save("tmp-" + blockCount);
                invertedIndex.Save("tmp-" + blockCount);
                blockCount++;
            }

            if (termType == TermType.TOKEN)
            {
                CombineMultipleDictionariesInDisk(Name, "", blockCount);
                CombineMultipleInvertedIndexesInDisk(Name, "", blockCount);
            }
            else
            {
                CombineMultipleDictionariesInDisk(Name + "-phrase", "", blockCount);
                CombineMultipleInvertedIndexesInDisk(Name + "-phrase", "", blockCount);
            }
        }

        /// <summary>
        /// In single pass in memory indexing, the dictionaries and positional indexes are created in a block wise manner.
        /// They do not fit in memory, therefore documents are read one by one. For each document, the terms are added to the
        /// current dictionary and positional index. If the number of documents read are above the limit, current partial
        /// dictionary and positional index file are saved and new dictionary and positional index file are open. After
        /// reading all documents, we combine the dictionary and positional index files to get the final dictionary and
        /// positional index file.
        /// </summary>
        /// <param name="termType">If term type is TOKEN, the terms are single word, if the term type is PHRASE, the terms are
        /// bi-words.</param>
        private void ConstructDictionaryAndPositionalIndexInDisk(TermType termType)
        {
            int i = 0, blockCount = 0;
            var positionalIndex = new PositionalIndex();
            var dictionary = new TermDictionary(Comparator);
            foreach (Document doc in Documents)
            {
                if (i < Parameter.GetDocumentLimit())
                {
                    i++;
                }
                else
                {
                    dictionary.Save("tmp-" + blockCount);
                    dictionary = new TermDictionary(Comparator);
                    positionalIndex.Save("tmp-" + blockCount);
                    positionalIndex = new PositionalIndex();
                    blockCount++;
                    i = 0;
                }

                var documentText = doc.LoadDocument();
                var terms = documentText.ConstructTermList(doc.GetDocId(), termType);
                foreach (var termOccurrence in terms)
                {
                    int termId;
                    int wordIndex = dictionary.GetWordIndex(termOccurrence.GetTerm().GetName());
                    if (wordIndex != -1)
                    {
                        termId = ((Term)dictionary.GetWord(wordIndex)).GetTermId();
                    }
                    else
                    {
                        termId = System.Math.Abs(termOccurrence.GetTerm().GetName().GetHashCode());
                        dictionary.AddTerm(termOccurrence.GetTerm().GetName(), termId);
                    }

                    positionalIndex.AddPosition(termId, termOccurrence.GetDocId(), termOccurrence.GetPosition());
                }
            }

            if (Documents.Count != 0)
            {
                dictionary.Save("tmp-" + blockCount);
                positionalIndex.Save("tmp-" + blockCount);
                blockCount++;
            }

            if (termType == TermType.TOKEN)
            {
                CombineMultipleDictionariesInDisk(Name, "", blockCount);
                CombineMultiplePositionalIndexesInDisk(Name, blockCount);
            }
            else
            {
                CombineMultipleDictionariesInDisk(Name + "-phrase", "", blockCount);
                CombineMultiplePositionalIndexesInDisk(Name + "-phrase", blockCount);
            }
        }

        /// <summary>
        /// The method constructs the N-Grams from the given tokens in a string. The method first identifies the tokens in
        /// the line by splitting from space, then constructs N-Grams for those tokens and adds N-Grams to the N-Gram
        /// dictionary and N-Gram index.
        /// </summary>
        /// <param name="line">String containing the tokens.</param>
        /// <param name="k">N in N-Gram.</param>
        /// <param name="nGramDictionary">N-Gram term dictionary</param>
        /// <param name="nGramIndex">N-Gram inverted index</param>
        private void AddNGramsToDictionaryAndIndex(string line, int k, TermDictionary nGramDictionary,
            NGramIndex nGramIndex)
        {
            var wordId = int.Parse(line.Substring(0, line.IndexOf(" ", StringComparison.Ordinal)));
            var word = line.Substring(line.IndexOf(" ", StringComparison.Ordinal) + 1);
            var biGrams = TermDictionary.ConstructNGrams(word, wordId, k);
            foreach (var term in biGrams)
            {
                int termId;
                var wordIndex = nGramDictionary.GetWordIndex(term.GetTerm().GetName());
                if (wordIndex != -1)
                {
                    termId = ((Term)nGramDictionary.GetWord(wordIndex)).GetTermId();
                }
                else
                {
                    termId = System.Math.Abs(term.GetTerm().GetName().GetHashCode());
                    nGramDictionary.AddTerm(term.GetTerm().GetName(), termId);
                }

                nGramIndex.Add(termId, wordId);
            }
        }

        /// <summary>
        /// In single pass in memory indexing, the dictionaries and N-gram indexes are created in a block wise manner.
        /// They do not fit in memory, therefore documents are read one by one. For each document, the terms are added to the
        /// current dictionary and N-gram index. If the number of documents read are above the limit, current partial
        /// dictionary and N-gram index file are saved and new dictionary and N-gram index file are open. After
        /// reading all documents, we combine the dictionary and N-gram index files to get the final dictionary and
        /// N-gram index file.
        /// </summary>
        private void ConstructNGramDictionaryAndIndexInDisk()
        {
            int i = 0, blockCount = 0;
            var biGramDictionary = new TermDictionary(Comparator);
            var triGramDictionary = new TermDictionary(Comparator);
            var biGramIndex = new NGramIndex();
            var triGramIndex = new NGramIndex();
            StreamReader streamReader;
            streamReader = new StreamReader(Name + "-dictionary.txt");
            var line = streamReader.ReadLine();
            while (line != null)
            {
                if (i < Parameter.GetWordLimit())
                {
                    i++;
                }
                else
                {
                    biGramDictionary.Save("tmp-biGram-" + blockCount);
                    triGramDictionary.Save("tmp-triGram-" + blockCount);
                    biGramDictionary = new TermDictionary(Comparator);
                    triGramDictionary = new TermDictionary(Comparator);
                    biGramIndex.Save("tmp-biGram-" + blockCount);
                    biGramIndex = new NGramIndex();
                    triGramIndex.Save("tmp-triGram-" + blockCount);
                    triGramIndex = new NGramIndex();
                    blockCount++;
                    i = 0;
                }

                AddNGramsToDictionaryAndIndex(line, 2, biGramDictionary, biGramIndex);
                AddNGramsToDictionaryAndIndex(line, 3, triGramDictionary, triGramIndex);
                line = streamReader.ReadLine();
            }

            streamReader.Close();

            if (Documents.Count != 0)
            {
                biGramDictionary.Save("tmp-biGram-" + blockCount);
                triGramDictionary.Save("tmp-triGram-" + blockCount);
                biGramIndex.Save("tmp-biGram-" + blockCount);
                triGramIndex.Save("tmp-triGram-" + blockCount);
                blockCount++;
            }

            CombineMultipleDictionariesInDisk(Name + "-biGram", "biGram-", blockCount);
            CombineMultipleDictionariesInDisk(Name + "-triGram", "triGram-", blockCount);
            CombineMultipleInvertedIndexesInDisk(Name + "-biGram", "biGram-", blockCount);
            CombineMultipleInvertedIndexesInDisk(Name + "-triGram", "triGram-", blockCount);
        }
    }
}