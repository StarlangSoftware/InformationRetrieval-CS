using System;
using System.Collections.Generic;
using System.IO;
using Dictionary.Dictionary;
using InformationRetrieval.Document;
using InformationRetrieval.Query;

namespace InformationRetrieval.Index
{
    public class PositionalIndex
    {
        private SortedDictionary<int, PositionalPostingList> positionalIndex;

        /**
         * Constructs an empty inverted index.
         */
        public PositionalIndex()
        {
            positionalIndex = new SortedDictionary<int, PositionalPostingList>();
        }

        /// <summary>
        /// Reads the positional inverted index from an input file.
        /// </summary>
        /// <param name="fileName">Input file name for the positional inverted index.</param>
        public PositionalIndex(string fileName)
        {
            positionalIndex = new SortedDictionary<int, PositionalPostingList>();
            ReadPositionalPostingList(fileName);
        }

        /// <summary>
        /// Constructs a positional inverted index from a list of sorted tokens. The terms array should be sorted before
        /// calling this method. Multiple occurrences of the same term from the same document are enlisted separately in the
        /// index.
        /// </summary>
        /// <param name="dictionary">Term dictionary</param>
        /// <param name="terms">Sorted list of tokens in the memory collection.</param>
        /// <param name="comparator">Comparator method to compare two terms.</param>
        public PositionalIndex(TermDictionary dictionary, List<TermOccurrence> terms, IComparer<Word> comparator)
        {
            int i, termId, prevDocId;
            TermOccurrence term, previousTerm;
            positionalIndex = new SortedDictionary<int, PositionalPostingList>();
            if (terms.Count > 0)
            {
                term = terms[0];
                i = 1;
                previousTerm = term;
                termId = dictionary.GetWordIndex(term.GetTerm().GetName());
                AddPosition(termId, term.GetDocId(), term.GetPosition());
                prevDocId = term.GetDocId();
                while (i < terms.Count)
                {
                    term = terms[i];
                    termId = dictionary.GetWordIndex(term.GetTerm().GetName());
                    if (termId != -1)
                    {
                        if (term.IsDifferent(previousTerm, comparator))
                        {
                            AddPosition(termId, term.GetDocId(), term.GetPosition());
                            prevDocId = term.GetDocId();
                        }
                        else
                        {
                            if (prevDocId != term.GetDocId())
                            {
                                AddPosition(termId, term.GetDocId(), term.GetPosition());
                                prevDocId = term.GetDocId();
                            }
                            else
                            {
                                AddPosition(termId, term.GetDocId(), term.GetPosition());
                            }
                        }
                    }

                    i++;
                    previousTerm = term;
                }
            }
        }

        /// <summary>
        /// Reads the positional postings list of the positional index from an input file. The postings are stored in n
        /// lines. The first line contains the term id and the number of documents that term occurs. Other n - 1 lines
        /// contain the postings list for that term for a separate document.
        /// </summary>
        /// <param name="fileName">Positional index file.</param>
        private void ReadPositionalPostingList(string fileName)
        {
            var streamReader = new StreamReader(fileName + "-positionalPostings.txt");
            var line = streamReader.ReadLine();
            while (line != null)
            {
                String[] items = line.Split(" ");
                int wordId = int.Parse(items[0]);
                positionalIndex[wordId] = new PositionalPostingList(streamReader, int.Parse(items[1]));
                line = streamReader.ReadLine();
            }

            streamReader.Close();
        }

        /// <summary>
        /// Saves the positional index into the index file. The postings are stored in n lines. The first line contains the
        /// term id and the number of documents that term occurs. Other n - 1 lines contain the postings list for that term
        /// for a separate document.
        /// </summary>
        /// <param name="fileName">Index file name. Real index file name is created by attaching -positionalPostings.txt to this
        /// file name</param>
        public void Save(string fileName)
        {
            var printWriter = new StreamWriter(fileName + "-positionalPostings.txt");
            foreach (var key in positionalIndex.Keys){
                positionalIndex[key].WriteToFile(printWriter, key);
            }
            printWriter.Close();
        }

        /// <summary>
        /// Adds a possible new term with a position and document id to the positional index. First the term is searched in
        /// the hash map, then the position and the document id is put into the correct postings list.
        /// </summary>
        /// <param name="termId">Id of the term</param>
        /// <param name="docId">Document id in which the term exists</param>
        /// <param name="position">Position of the term in the document with id docId</param>
        public void AddPosition(int termId, int docId, int position)
        {
            PositionalPostingList positionalPostingList;
            if (!positionalIndex.ContainsKey(termId))
            {
                positionalPostingList = new PositionalPostingList();
            }
            else
            {
                positionalPostingList = positionalIndex[termId];
            }

            positionalPostingList.Add(docId, position);
            positionalIndex[termId] = positionalPostingList;
        }

        /// <summary>
        /// Searches a given query in the document collection using positional index boolean search.
        /// </summary>
        /// <param name="query">Query string</param>
        /// <param name="dictionary">Term dictionary</param>
        /// <returns>The result of the query obtained by doing positional index boolean search in the collection.</returns>
        public QueryResult PositionalSearch(Query.Query query, TermDictionary dictionary)
        {
            int i, term;
            PositionalPostingList postingResult = null;
            for (i = 0; i < query.Size(); i++)
            {
                term = dictionary.GetWordIndex(query.GetTerm(i).GetName());
                if (term != -1)
                {
                    if (i == 0)
                    {
                        postingResult = positionalIndex[term];
                    }
                    else
                    {
                        if (postingResult != null)
                        {
                            postingResult = postingResult.Intersection(positionalIndex[term]);
                        }
                        else
                        {
                            return new QueryResult();
                        }
                    }
                }
                else
                {
                    return new QueryResult();
                }
            }

            if (postingResult != null)
                return postingResult.ToQueryResult();
            else
                return new QueryResult();
        }

        /// <summary>
        /// Returns the term frequencies  in a given document.
        /// </summary>
        /// <param name="docId">Id of the document</param>
        /// <returns>Term frequencies of the given document.</returns>
        public int[] GetTermFrequencies(int docId)
        {
            int[] tf;
            int index;
            PositionalPostingList positionalPostingList;
            tf = new int[positionalIndex.Count];
            var i = 0;
            foreach (var key in positionalIndex.Keys){
                positionalPostingList = positionalIndex[key];
                index = positionalPostingList.GetIndex(docId);
                if (index != -1)
                {
                    tf[i] = positionalPostingList.Get(index).Size();
                }
                else
                {
                    tf[i] = 0;
                }

                i++;
            }
            return tf;
        }

        /// <summary>
        /// Returns the document frequencies of the terms in the collection.
        /// </summary>
        /// <returns>The document frequencies of the terms in the collection.</returns>
        public int[] GetDocumentFrequencies()
        {
            int[] df;
            df = new int[positionalIndex.Count];
            var i = 0;
            foreach (var key in positionalIndex.Keys){
                df[i] = positionalIndex[key].Size();
                i++;
            }
            return df;
        }
        
        /// <summary>
        /// Calculates and sets the number of terms in each document in the document collection.
        /// </summary>
        /// <param name="documents">Document collection.</param>
        public void SetDocumentSizes(List<Document.Document> documents){
            var sizes = new int[documents.Count];
            foreach (var key in positionalIndex.Keys){
                var positionalPostingList = positionalIndex[key];
                for (var j = 0; j < positionalPostingList.Size(); j++) {
                    var positionalPosting = positionalPostingList.Get(j);
                    var docId = positionalPosting.GetDocId();
                    sizes[docId] += positionalPosting.Size();
                }
            }
            foreach (var document in documents){
                document.SetSize(sizes[document.GetDocId()]);
            }
        }

        /// <summary>
        /// Calculates and updates the frequency counts of the terms in each category node.
        /// </summary>
        /// <param name="documents">Document collection.</param>
        public void SetCategoryCounts(List<Document.Document> documents){
            foreach (var termId in positionalIndex.Keys){
                var positionalPostingList = positionalIndex[termId];
                for (var j = 0; j < positionalPostingList.Size(); j++) {
                    var positionalPosting = positionalPostingList.Get(j);
                    var docId = positionalPosting.GetDocId();
                    documents[docId].GetCategoryNode().AddCounts(termId, positionalPosting.Size());
                }
            }
        }
        
        /// <summary>
        /// Searches a given query in the document collection using inverted index ranked search.
        /// </summary>
        /// <param name="query">Query string</param>
        /// <param name="dictionary">Term dictionary</param>
        /// <param name="documents">Document collection</param>
        /// <param name="parameter">Search parameter</param>
        /// <returns>The result of the query obtained by doing inverted index ranked search in the collection.</returns>
        public QueryResult RankedSearch(Query.Query query, 
            TermDictionary dictionary, 
            List<Document.Document> documents,
            SearchParameter parameter)
        {
            int i, j, term, docID, n = documents.Count, tf, df;
            var result = new QueryResult();
            var scores = new Dictionary<int, double>();
            PositionalPostingList positionalPostingList;
            for (i = 0; i < query.Size(); i++)
            {
                term = dictionary.GetWordIndex(query.GetTerm(i).GetName());
                if (term != -1)
                {
                    positionalPostingList = positionalIndex[term];
                    for (j = 0; j < positionalPostingList.Size(); j++)
                    {
                        var positionalPosting = positionalPostingList.Get(j);
                        docID = positionalPosting.GetDocId();
                        tf = positionalPosting.Size();
                        df = positionalIndex[term].Size();
                        if (tf > 0 && df > 0)
                        {
                            var score = VectorSpaceModel.Weighting(tf, df, n, parameter.GetTermWeighting(), parameter.GetDocumentWeighting());
                            if (scores.ContainsKey(docID)){
                                scores[docID] = scores[docID] + score;
                            } else {
                                scores[docID] = score;
                            }
                        }
                    }
                }
            }

            foreach (var docId in scores.Keys){
                result.Add(docId, scores[docId] / documents[docId].GetSize());
            }
            return result;
        }
    }
}