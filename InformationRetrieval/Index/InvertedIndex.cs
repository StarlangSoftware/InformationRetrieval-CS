using System.Collections.Generic;
using System.IO;
using Dictionary.Dictionary;
using InformationRetrieval.Query;

namespace InformationRetrieval.Index
{
    public class InvertedIndex
    {
        private SortedDictionary<int, PostingList> index;

        public InvertedIndex()
        {
            index = new SortedDictionary<int, PostingList>();
        }

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

        public InvertedIndex(string fileName)
        {
            index = new SortedDictionary<int, PostingList>();
            ReadPostingList(fileName);
        }

        public void Save(string fileName)
        {
            var printWriter = new StreamWriter(fileName + "-postings.txt");
            foreach (var key in index.Keys){
                index[key].WriteToFile(printWriter, key);
            }
            printWriter.Close();
        }

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