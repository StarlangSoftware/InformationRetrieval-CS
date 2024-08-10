using System.Collections.Generic;
using System.IO;
using Dictionary.Dictionary;
using InformationRetrieval.Query;

namespace InformationRetrieval.Index
{
    public class InvertedIndex
    {
        private SortedDictionary<int, PostingList> index;

        /**
         * Constructs an empty inverted index.
         */
        public InvertedIndex()
        {
            index = new SortedDictionary<int, PostingList>();
        }

        /// <summary>
        /// Constructs an inverted index from a list of sorted tokens. The terms array should be sorted before calling this
        /// method. Multiple occurrences of the same term from the same document are merged in the index. Instances of the
        /// same term are then grouped, and the result is split into a postings list.
        /// </summary>
        /// <param name="dictionary">Term dictionary</param>
        /// <param name="terms">Sorted list of tokens in the memory collection.</param>
        /// <param name="comparator">Comparator method to compare two terms.</param>
        public InvertedIndex(TermDictionary dictionary, List<TermOccurrence> terms, IComparer<Word> comparator) : this()
        {
            int i, termId, prevDocId;
            TermOccurrence term, previousTerm;
            if (terms.Count > 0)
            {
                term = terms[0];
                i = 1;
                previousTerm = term;
                termId = dictionary.GetWordIndex(term.GetTerm().GetName());
                Add(termId, term.GetDocId());
                prevDocId = term.GetDocId();
                while (i < terms.Count)
                {
                    term = terms[i];
                    termId = dictionary.GetWordIndex(term.GetTerm().GetName());
                    if (termId != -1)
                    {
                        if (term.IsDifferent(previousTerm, comparator))
                        {
                            Add(termId, term.GetDocId());
                            prevDocId = term.GetDocId();
                        }
                        else
                        {
                            if (prevDocId != term.GetDocId())
                            {
                                Add(termId, term.GetDocId());
                                prevDocId = term.GetDocId();
                            }
                        }
                    }

                    i++;
                    previousTerm = term;
                }
            }
        }

        /// <summary>
        /// Reads the postings list of the inverted index from an input file. The postings are stored in two lines. The first
        /// line contains the term id and the number of postings for that term. The second line contains the postings
        /// list for that term.
        /// </summary>
        /// <param name="fileName">Inverted index file.</param>
        private void ReadPostingList(string fileName)
        {
            var streamReader = new StreamReader(fileName + "-postings.txt");
            var line = streamReader.ReadLine();
            while (line != null)
            {
                var items = line.Split(" ");
                var wordId = int.Parse(items[0]);
                line = streamReader.ReadLine();
                index[wordId] = new PostingList(line);
                line = streamReader.ReadLine();
            }

            streamReader.Close();
        }

        /// <summary>
        /// Reads the inverted index from an input file.
        /// </summary>
        /// <param name="fileName">Input file name for the inverted index.</param>
        public InvertedIndex(string fileName)
        {
            index = new SortedDictionary<int, PostingList>();
            ReadPostingList(fileName);
        }

        /// <summary>
        /// Saves the inverted index into the index file. The postings are stored in two lines. The first
        /// line contains the term id and the number of postings for that term. The second line contains the postings
        /// list for that term.
        /// </summary>
        /// <param name="fileName">Index file name. Real index file name is created by attaching -postings.txt to this
        /// file name</param>
        public void Save(string fileName)
        {
            var printWriter = new StreamWriter(fileName + "-postings.txt");
            foreach (var key in index.Keys){
                index[key].WriteToFile(printWriter, key);
            }
            printWriter.Close();
        }

        /// <summary>
        /// Adds a possible new term with a document id to the inverted index. First the term is searched in the hash map,
        /// then the document id is put into the correct postings list.
        /// </summary>
        /// <param name="termId">Id of the term</param>
        /// <param name="docId">Document id in which the term exists</param>
        public void Add(int termId, int docId)
        {
            PostingList postingList;
            if (!index.ContainsKey(termId))
            {
                postingList = new PostingList();
            }
            else
            {
                postingList = index[termId];
            }

            postingList.Add(docId);
            index[termId] = postingList;
        }

        /// <summary>
        /// Constructs a sorted array list of frequency counts for a word list and also sorts the word list according to
        /// those frequencies.
        /// </summary>
        /// <param name="wordList">Word list for which frequency array is constructed.</param>
        /// <param name="dictionary">Term dictionary</param>
        public void AutoCompleteWord(List<string> wordList, TermDictionary dictionary){
            var counts = new List<int>();
            foreach (var word in wordList){
                counts.Add(index[dictionary.GetWordIndex(word)].Size());
            }
            for (var i = 0; i < wordList.Count - 1; i++){
                for (var j = i + 1; j < wordList.Count; j++){
                    if (counts[i] < counts[j]){
                        (counts[i], counts[j]) = (counts[j], counts[i]);
                        (wordList[i], wordList[j]) = (wordList[j], wordList[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Searches a given query in the document collection using inverted index boolean search.
        /// </summary>
        /// <param name="query">Query string</param>
        /// <param name="dictionary">Term dictionary</param>
        /// <returns>The result of the query obtained by doing inverted index boolean search in the collection.</returns>
        public QueryResult Search(Query.Query query, TermDictionary dictionary)
        {
            int i, termIndex;
            PostingList result;
            var comparator = new PostingListComparator();
            var queryTerms = new List<PostingList>();
            for (i = 0; i < query.Size(); i++)
            {
                termIndex = dictionary.GetWordIndex(query.GetTerm(i).GetName());
                if (termIndex != -1)
                {
                    queryTerms.Add(index[termIndex]);
                }
                else
                {
                    return new QueryResult();
                }
            }

            queryTerms.Sort(comparator);
            result = queryTerms[0];
            for (i = 1; i < queryTerms.Count; i++)
            {
                result = result.Intersection(queryTerms[i]);
            }

            return result.ToQueryResult();
        }
    }
}