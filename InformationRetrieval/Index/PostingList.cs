using System.Collections.Generic;
using System.IO;
using InformationRetrieval.Query;

namespace InformationRetrieval.Index
{
    public class PostingList
    {
        protected List<Posting> Postings;

        public PostingList()
        {
            Postings = new List<Posting>();
        }

        public PostingList(string line)
        {
            Postings = new List<Posting>();
            var ids = line.Split(" ");
            foreach (var id in ids)
            {
                Add(int.Parse(id));
            }
        }

        public void Add(int docId)
        {
            Postings.Add(new Posting(docId));
        }

        public int Size()
        {
            return Postings.Count;
        }

        public PostingList Intersection(PostingList secondList)
        {
            int i = 0, j = 0;
            var result = new PostingList();
            while (i < Size() && j < secondList.Size())
            {
                var p1 = Postings[i];
                var p2 = secondList.Postings[j];
                if (p1.GetId() == p2.GetId())
                {
                    result.Add(p1.GetId());
                    i++;
                    j++;
                }
                else
                {
                    if (p1.GetId() < p2.GetId())
                    {
                        i++;
                    }
                    else
                    {
                        j++;
                    }
                }
            }

            return result;
        }

        public PostingList Union(PostingList secondList)
        {
            var result = new PostingList();
            result.Postings = new List<Posting>();
            result.Postings.AddRange(Postings);
            result.Postings.AddRange(secondList.Postings);
            return result;
        }

        public QueryResult ToQueryResult()
        {
            var result = new QueryResult();
            foreach (var posting in Postings)
            {
                result.Add(posting.GetId());
            }

            return result;
        }

        public void WriteToFile(StreamWriter printWriter, int index)
        {
            if (Size() > 0)
            {
                printWriter.Write(index + " " + Size() + "\n");
                printWriter.Write(ToString());
            }
        }

        public new string ToString()
        {
            var result = "";
            foreach (var posting in Postings)
            {
                result += posting.GetId() + " ";
            }

            return result.Trim() + "\n";
        }
    }
}