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

        public PositionalIndex()
        {
            positionalIndex = new SortedDictionary<int, PositionalPostingList>();
        }

        public PositionalIndex(string fileName)
        {
            positionalIndex = new SortedDictionary<int, PositionalPostingList>();
            ReadPositionalPostingList(fileName);
        }

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

        public void Save(string fileName)
        {
            var printWriter = new StreamWriter(fileName + "-positionalPostings.txt");
            foreach (var key in positionalIndex.Keys){
                positionalIndex[key].WriteToFile(printWriter, key);
            }
            printWriter.Close();
        }

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

        public QueryResult RankedSearch(Query.Query query, TermDictionary dictionary, List<Document.Document> documents,
            TermWeighting termWeighting, DocumentWeighting documentWeighting, int documentsReturned)
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
                            var score = VectorSpaceModel.Weighting(tf, df, n, termWeighting, documentWeighting);
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
            result.GetBest(documentsReturned);
            return result;
        }
    }
}