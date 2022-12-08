using System;
using System.Collections.Generic;
using System.IO;
using Dictionary.Dictionary;
using InformationRetrieval.Index;

namespace InformationRetrieval.Document
{
    public class LargeCollection : DiskCollection
    {
        public LargeCollection(string directory, Parameter parameter) : base(directory, parameter)
        {
            ConstructDictionaryAndIndexesInDisk();
        }

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