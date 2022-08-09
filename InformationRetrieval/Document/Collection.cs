using System;
using System.Collections.Generic;
using System.IO;
using Dictionary.Dictionary;
using InformationRetrieval.Index;
using InformationRetrieval.Query;

namespace InformationRetrieval.Document
{
    public class Collection
    {
        private IndexType indexType;
        private TermDictionary _dictionary;
        private TermDictionary _phraseDictionary;
        private TermDictionary _biGramDictionary;
        private TermDictionary _triGramDictionary;
        private List<Document> documents;
        private IncidenceMatrix _incidenceMatrix;
        private InvertedIndex _invertedIndex;
        private NGramIndex _biGramIndex;
        private NGramIndex _triGramIndex;
        private PositionalIndex _positionalIndex;
        private InvertedIndex _phraseIndex;
        private PositionalIndex _phrasePositionalIndex;
        private IComparer<Word> comparator;
        private string name;
        private Parameter parameter;

        public Collection(string directory, Parameter parameter)
        {
            name = directory;
            indexType = parameter.GetIndexType();
            comparator = parameter.GetWordComparator();
            this.parameter = parameter;
            documents = new List<Document>();
            var listOfFiles = Directory.GetFiles(directory);
            if (listOfFiles != null)
            {
                Array.Sort(listOfFiles);
                var fileLimit = listOfFiles.Length;
                if (parameter.LimitNumberOfDocumentsLoaded())
                {
                    fileLimit = parameter.GetDocumentLimit();
                }

                var i = 0;
                var j = 0;
                while (i < listOfFiles.Length && j < fileLimit)
                {
                    var file = listOfFiles[i];
                    if (file.EndsWith(".txt"))
                    {
                        var document = new Document(file, file, j);
                        documents.Add(document);
                        j++;
                    }

                    i++;
                }
            }

            if (parameter.LoadIndexesFromFile())
            {
                _dictionary = new TermDictionary(comparator, directory);
                _invertedIndex = new InvertedIndex(directory);
                if (parameter.ConstructPositionalIndex())
                {
                    _positionalIndex = new PositionalIndex(directory);
                }

                if (parameter.ConstructPhraseIndex())
                {
                    _phraseDictionary = new TermDictionary(comparator, directory + "-phrase");
                    _phraseIndex = new InvertedIndex(directory + "-phrase");
                    if (parameter.ConstructPositionalIndex())
                    {
                        _phrasePositionalIndex = new PositionalIndex(directory + "-phrase");
                    }
                }

                if (parameter.ConstructNGramIndex())
                {
                    _biGramDictionary = new TermDictionary(comparator, directory + "-biGram");
                    _triGramDictionary = new TermDictionary(comparator, directory + "-triGram");
                    _biGramIndex = new NGramIndex(directory + "-biGram");
                    _triGramIndex = new NGramIndex(directory + "-triGram");
                }
            }
            else
            {
                if (parameter.ConstructDictionaryInDisk())
                {
                    ConstructDictionaryInDisk();
                }
                else
                {
                    if (parameter.ConstructIndexInDisk())
                    {
                        ConstructIndexesInDisk();
                    }
                    else
                    {
                        ConstructIndexesInMemory();
                    }
                }
            }
        }

        public int Size()
        {
            return documents.Count;
        }

        public int VocabularySize()
        {
            return _dictionary.Size();
        }

        public void Save()
        {
            if (indexType == IndexType.INVERTED_INDEX)
            {
                _dictionary.Save(name);
                _invertedIndex.Save(name);
                if (parameter.ConstructPositionalIndex())
                {
                    _positionalIndex.Save(name);
                }

                if (parameter.ConstructPhraseIndex())
                {
                    _phraseDictionary.Save(name + "-phrase");
                    _phraseIndex.Save(name + "-phrase");
                    if (parameter.ConstructPositionalIndex())
                    {
                        _phrasePositionalIndex.Save(name + "-phrase");
                    }
                }

                if (parameter.ConstructNGramIndex())
                {
                    _biGramDictionary.Save(name + "-biGram");
                    _triGramDictionary.Save(name + "-triGram");
                    _biGramIndex.Save(name + "-biGram");
                    _triGramIndex.Save(name + "-triGram");
                }
            }
        }

        private void ConstructDictionaryInDisk()
        {
            ConstructDictionaryAndInvertedIndexInDisk(TermType.TOKEN);
            if (parameter.ConstructPositionalIndex())
            {
                ConstructDictionaryAndPositionalIndexInDisk(TermType.TOKEN);
            }

            if (parameter.ConstructPhraseIndex())
            {
                ConstructDictionaryAndInvertedIndexInDisk(TermType.PHRASE);
                if (parameter.ConstructPositionalIndex())
                {
                    ConstructDictionaryAndPositionalIndexInDisk(TermType.PHRASE);
                }
            }

            if (parameter.ConstructNGramIndex())
            {
                ConstructNGramDictionaryAndIndexInDisk();
            }
        }

        private void ConstructIndexesInDisk()
        {
            var wordList = ConstructDistinctWordList(TermType.TOKEN);
            _dictionary = new TermDictionary(comparator, wordList);
            ConstructInvertedIndexInDisk(_dictionary, TermType.TOKEN);
            if (parameter.ConstructPositionalIndex())
            {
                ConstructPositionalIndexInDisk(_dictionary, TermType.TOKEN);
            }

            if (parameter.ConstructPhraseIndex())
            {
                wordList = ConstructDistinctWordList(TermType.PHRASE);
                _phraseDictionary = new TermDictionary(comparator, wordList);
                ConstructInvertedIndexInDisk(_phraseDictionary, TermType.PHRASE);
                if (parameter.ConstructPositionalIndex())
                {
                    ConstructPositionalIndexInDisk(_phraseDictionary, TermType.PHRASE);
                }
            }

            if (parameter.ConstructNGramIndex())
            {
                ConstructNGramIndex();
            }
        }

        private void ConstructIndexesInMemory()
        {
            List<TermOccurrence> terms = ConstructTerms(TermType.TOKEN);
            _dictionary = new TermDictionary(comparator, terms);
            switch (indexType)
            {
                case IndexType.INCIDENCE_MATRIX:
                    _incidenceMatrix = new IncidenceMatrix(terms, _dictionary, documents.Count);
                    break;
                case IndexType.INVERTED_INDEX:
                    _invertedIndex = new InvertedIndex(_dictionary, terms, comparator);
                    if (parameter.ConstructPositionalIndex())
                    {
                        _positionalIndex = new PositionalIndex(_dictionary, terms, comparator);
                    }

                    if (parameter.ConstructPhraseIndex())
                    {
                        terms = ConstructTerms(TermType.PHRASE);
                        _phraseDictionary = new TermDictionary(comparator, terms);
                        _phraseIndex = new InvertedIndex(_phraseDictionary, terms, comparator);
                        if (parameter.ConstructPositionalIndex())
                        {
                            _phrasePositionalIndex = new PositionalIndex(_phraseDictionary, terms, comparator);
                        }
                    }

                    if (parameter.ConstructNGramIndex())
                    {
                        ConstructNGramIndex();
                    }

                    break;
            }
        }

        private List<TermOccurrence> ConstructTerms(TermType termType)
        {
            var termComparator = new TermOccurrenceComparator(comparator);
            var terms = new List<TermOccurrence>();
            List<TermOccurrence> docTerms;
            foreach (var doc in documents)
            {
                var documentText = doc.LoadDocument();
                docTerms = documentText.ConstructTermList(doc.GetDocId(), termType);
                terms.AddRange(docTerms);
            }

            terms.Sort(termComparator);
            return terms;
        }

        private HashSet<string> ConstructDistinctWordList(TermType termType)
        {
            var words = new HashSet<string>();
            foreach (var doc in documents)
            {
                var documentText = doc.LoadDocument();
                words.UnionWith(documentText.ConstructDistinctWordList(termType));
            }

            return words;
        }

        private bool NotCombinedAllIndexes(int[] currentIdList)
        {
            foreach (var id in currentIdList)
            {
                if (id != -1)
                {
                    return true;
                }
            }

            return false;
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

        private List<int> SelectIndexesWithMinimumTermIds(int[] currentIdList)
        {
            var result = new List<int>();
            var min = int.MaxValue;
            foreach (var id in currentIdList)
            {
                if (id != -1 && id < min)
                {
                    min = id;
                }
            }

            for (var i = 0; i < currentIdList.Length; i++)
            {
                if (currentIdList[i] == min)
                {
                    result.Add(i);
                }
            }

            return result;
        }

        private List<int> SelectDictionariesWithMinimumWords(string[] currentWords)
        {
            var result = new List<int>();
            string min = null;
            foreach (var word in currentWords)
            {
                if (word != null && (min == null || comparator.Compare(new Word(word), new Word(min)) < 0))
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
                    string line = files[i].ReadLine();
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
            var biGramDictionary = new TermDictionary(comparator);
            var triGramDictionary = new TermDictionary(comparator);
            var biGramIndex = new NGramIndex();
            var triGramIndex = new NGramIndex();
            StreamReader streamReader;
            streamReader = new StreamReader(name + "-dictionary.txt");
            var line = streamReader.ReadLine();
            while (line != null)
            {
                if (i < parameter.GetWordLimit())
                {
                    i++;
                }
                else
                {
                    biGramDictionary.Save("tmp-biGram-" + blockCount);
                    triGramDictionary.Save("tmp-triGram-" + blockCount);
                    biGramDictionary = new TermDictionary(comparator);
                    triGramDictionary = new TermDictionary(comparator);
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

            if (documents.Count != 0)
            {
                biGramDictionary.Save("tmp-biGram-" + blockCount);
                triGramDictionary.Save("tmp-triGram-" + blockCount);
                biGramIndex.Save("tmp-biGram-" + blockCount);
                triGramIndex.Save("tmp-triGram-" + blockCount);
                blockCount++;
            }

            CombineMultipleDictionariesInDisk(name + "-biGram", "biGram-", blockCount);
            CombineMultipleDictionariesInDisk(name + "-triGram", "triGram-", blockCount);
            CombineMultipleInvertedIndexesInDisk(name + "-biGram", "biGram-", blockCount);
            CombineMultipleInvertedIndexesInDisk(name + "-triGram", "triGram-", blockCount);
        }

        private void CombineMultipleInvertedIndexesInDisk(string collectionName, string tmpName, int blockCount)
        {
            StreamReader[] files;
            int[] currentIdList;
            PostingList[] currentPostingLists;
            currentIdList = new int[blockCount];
            currentPostingLists = new PostingList[blockCount];
            files = new StreamReader[blockCount];
            var printWriter = new StreamWriter(collectionName + "-postings.txt");
            for (var i = 0; i < blockCount; i++)
            {
                files[i] = new StreamReader("tmp-" + tmpName + i + "-postings.txt");
                var line = files[i].ReadLine();
                var items = line.Split(" ");
                currentIdList[i] = int.Parse(items[0]);
                line = files[i].ReadLine();
                currentPostingLists[i] = new PostingList(line);
            }

            while (NotCombinedAllIndexes(currentIdList))
            {
                var indexesToCombine = SelectIndexesWithMinimumTermIds(currentIdList);
                var mergedPostingList = currentPostingLists[indexesToCombine[0]];
                for (var i = 1; i < indexesToCombine.Count; i++)
                {
                    mergedPostingList = mergedPostingList.Union(currentPostingLists[indexesToCombine[i]]);
                }

                mergedPostingList.WriteToFile(printWriter, currentIdList[indexesToCombine[0]]);
                foreach (var i in indexesToCombine)
                {
                    var line = files[i].ReadLine();
                    if (line != null)
                    {
                        var items = line.Split(" ");
                        currentIdList[i] = int.Parse(items[0]);
                        line = files[i].ReadLine();
                        currentPostingLists[i] = new PostingList(line);
                    }
                    else
                    {
                        currentIdList[i] = -1;
                    }
                }
            }

            for (var i = 0; i < blockCount; i++)
            {
                files[i].Close();
            }

            printWriter.Close();
        }

        private void ConstructInvertedIndexInDisk(TermDictionary dictionary, TermType termType)
        {
            int i = 0, blockCount = 0;
            var invertedIndex = new InvertedIndex();
            foreach (Document doc in documents)
            {
                if (i < parameter.GetDocumentLimit())
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

            if (documents.Count != 0)
            {
                invertedIndex.Save("tmp-" + blockCount);
                blockCount++;
            }

            if (termType == TermType.TOKEN)
            {
                CombineMultipleInvertedIndexesInDisk(name, "", blockCount);
            }
            else
            {
                CombineMultipleInvertedIndexesInDisk(name + "-phrase", "", blockCount);
            }
        }

        private void ConstructDictionaryAndInvertedIndexInDisk(TermType termType)
        {
            int i = 0, blockCount = 0;
            var invertedIndex = new InvertedIndex();
            var dictionary = new TermDictionary(comparator);
            foreach (Document doc in documents)
            {
                if (i < parameter.GetDocumentLimit())
                {
                    i++;
                }
                else
                {
                    dictionary.Save("tmp-" + blockCount);
                    dictionary = new TermDictionary(comparator);
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

            if (documents.Count != 0)
            {
                dictionary.Save("tmp-" + blockCount);
                invertedIndex.Save("tmp-" + blockCount);
                blockCount++;
            }

            if (termType == TermType.TOKEN)
            {
                CombineMultipleDictionariesInDisk(name, "", blockCount);
                CombineMultipleInvertedIndexesInDisk(name, "", blockCount);
            }
            else
            {
                CombineMultipleDictionariesInDisk(name + "-phrase", "", blockCount);
                CombineMultipleInvertedIndexesInDisk(name + "-phrase", "", blockCount);
            }
        }

        private void CombineMultiplePositionalIndexesInDisk(string collectionName, int blockCount)
        {
            StreamReader[] files;
            int[] currentIdList;
            PositionalPostingList[] currentPostingLists;
            currentIdList = new int[blockCount];
            currentPostingLists = new PositionalPostingList[blockCount];
            files = new StreamReader[blockCount];
            var printWriter = new StreamWriter(collectionName + "-positionalPostings.txt");
            for (var i = 0; i < blockCount; i++)
            {
                files[i] = new StreamReader("tmp-" + i + "-positionalPostings.txt");
                var line = files[i].ReadLine();
                var items = line.Split(" ");
                currentIdList[i] = int.Parse(items[0]);
                currentPostingLists[i] = new PositionalPostingList(files[i], int.Parse(items[1]));
            }

            while (NotCombinedAllIndexes(currentIdList))
            {
                var indexesToCombine = SelectIndexesWithMinimumTermIds(currentIdList);
                var mergedPostingList = currentPostingLists[indexesToCombine[0]];
                for (var i = 1; i < indexesToCombine.Count; i++)
                {
                    mergedPostingList = mergedPostingList.Union(currentPostingLists[indexesToCombine[i]]);
                }

                mergedPostingList.WriteToFile(printWriter, currentIdList[indexesToCombine[0]]);
                foreach (var i in indexesToCombine) {
                    var line = files[i].ReadLine();
                    if (line != null)
                    {
                        string[] items = line.Split(" ");
                        currentIdList[i] = int.Parse(items[0]);
                        currentPostingLists[i] = new PositionalPostingList(files[i], int.Parse(items[1]));
                    }
                    else
                    {
                        currentIdList[i] = -1;
                    }
                }
            }

            for (var i = 0; i < blockCount; i++)
            {
                files[i].Close();
            }

            printWriter.Close();
        }

        private void ConstructDictionaryAndPositionalIndexInDisk(TermType termType)
        {
            int i = 0, blockCount = 0;
            var positionalIndex = new PositionalIndex();
            var dictionary = new TermDictionary(comparator);
            foreach (Document doc in documents){
                if (i < parameter.GetDocumentLimit())
                {
                    i++;
                }
                else
                {
                    dictionary.Save("tmp-" + blockCount);
                    dictionary = new TermDictionary(comparator);
                    positionalIndex.Save("tmp-" + blockCount);
                    positionalIndex = new PositionalIndex();
                    blockCount++;
                    i = 0;
                }

                var documentText = doc.LoadDocument();
                var terms = documentText.ConstructTermList(doc.GetDocId(), termType);
                foreach (var termOccurrence in terms){
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
            if (documents.Count != 0)
            {
                dictionary.Save("tmp-" + blockCount);
                positionalIndex.Save("tmp-" + blockCount);
                blockCount++;
            }

            if (termType == TermType.TOKEN)
            {
                CombineMultipleDictionariesInDisk(name, "", blockCount);
                CombineMultiplePositionalIndexesInDisk(name, blockCount);
            }
            else
            {
                CombineMultipleDictionariesInDisk(name + "-phrase", "", blockCount);
                CombineMultiplePositionalIndexesInDisk(name + "-phrase", blockCount);
            }
        }

        private void ConstructPositionalIndexInDisk(TermDictionary dictionary, TermType termType)
        {
            int i = 0, blockCount = 0;
            var positionalIndex = new PositionalIndex();
            foreach (var doc in documents){
                if (i < parameter.GetDocumentLimit())
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
                foreach (TermOccurrence termOccurrence in terms){
                    var termId = dictionary.GetWordIndex(termOccurrence.GetTerm().GetName());
                    positionalIndex.AddPosition(termId, termOccurrence.GetDocId(), termOccurrence.GetPosition());
                }
            }
            if (documents.Count != 0)
            {
                positionalIndex.Save("tmp-" + blockCount);
                blockCount++;
            }

            if (termType == TermType.TOKEN)
            {
                CombineMultiplePositionalIndexesInDisk(name, blockCount);
            }
            else
            {
                CombineMultiplePositionalIndexesInDisk(name + "-phrase", blockCount);
            }
        }

        private void ConstructNGramIndex()
        {
            List<TermOccurrence> terms = _dictionary.ConstructTermsFromDictionary(2);
            _biGramDictionary = new TermDictionary(comparator, terms);
            _biGramIndex = new NGramIndex(_biGramDictionary, terms, comparator);
            terms = _dictionary.ConstructTermsFromDictionary(3);
            _triGramDictionary = new TermDictionary(comparator, terms);
            _triGramIndex = new NGramIndex(_triGramDictionary, terms, comparator);
        }

        public QueryResult SearchCollection(Query.Query query, RetrievalType retrievalType, TermWeighting termWeighting,
            DocumentWeighting documentWeighting)
        {
            switch (indexType)
            {
                case IndexType.INCIDENCE_MATRIX:
                    return _incidenceMatrix.Search(query, _dictionary);
                case IndexType.INVERTED_INDEX:
                    switch (retrievalType)
                    {
                        case RetrievalType.BOOLEAN: return _invertedIndex.Search(query, _dictionary);
                        case RetrievalType.POSITIONAL: return _positionalIndex.PositionalSearch(query, _dictionary);
                        case RetrievalType.RANKED:
                            return _positionalIndex.RankedSearch(query, _dictionary, documents, termWeighting,
                                documentWeighting);
                    }

                    break;
            }

            return new QueryResult();
        }
    }
}