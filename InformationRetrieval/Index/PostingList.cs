using System.Collections.Generic;
using System.IO;
using InformationRetrieval.Query;

namespace InformationRetrieval.Index
{
    public class PostingList
    {
        protected List<Posting> Postings;

        /**
         * Constructor of the PostingList class. Initializes the list.
         */
        public PostingList()
        {
            Postings = new List<Posting>();
        }

        /// <summary>
        /// Constructs a posting list from a line, which contains postings separated with space.
        /// </summary>
        /// <param name="line">A string containing postings separated with space character.</param>
        public PostingList(string line)
        {
            Postings = new List<Posting>();
            var ids = line.Split(" ");
            foreach (var id in ids)
            {
                Add(int.Parse(id));
            }
        }

        /// <summary>
        /// Adds a new posting (document id) to the posting list.
        /// </summary>
        /// <param name="docId">New document id to be added to the posting list.</param>
        public void Add(int docId)
        {
            Postings.Add(new Posting(docId));
        }

        /// <summary>
        /// Returns the number of postings in the posting list.
        /// </summary>
        /// <returns>Number of postings in the posting list.</returns>
        public int Size()
        {
            return Postings.Count;
        }

        /// <summary>
        /// Algorithm for the intersection of two postings lists p1 and p2. We maintain pointers into both lists and walk
        /// through the two postings lists simultaneously, in time linear in the total number of postings entries. At each
        /// step, we compare the docID pointed to by both pointers. If they are the same, we put that docID in the results
        /// list, and advance both pointers. Otherwise, we advance the pointer pointing to the smaller docID.
        /// </summary>
        /// <param name="secondList">p2, second posting list.</param>
        /// <returns>Intersection of two postings lists p1 and p2.</returns>
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

        /// <summary>
        /// Returns simple union of two postings list p1 and p2. The algorithm assumes the intersection of two postings list
        /// is empty, therefore the union is just concatenation of two postings lists.
        /// </summary>
        /// <param name="secondList">p2</param>
        /// <returns>Union of two postings lists.</returns>
        public PostingList Union(PostingList secondList)
        {
            var result = new PostingList();
            result.Postings = new List<Posting>();
            result.Postings.AddRange(Postings);
            result.Postings.AddRange(secondList.Postings);
            return result;
        }

        /// <summary>
        /// Converts the postings list to a query result object. Simply adds all postings one by one to the result.
        /// </summary>
        /// <returns>QueryResult object containing the postings in this object.</returns>
        public QueryResult ToQueryResult()
        {
            var result = new QueryResult();
            foreach (var posting in Postings)
            {
                result.Add(posting.GetId());
            }

            return result;
        }

        /// <summary>
        /// Prints this object into a file with the given index.
        /// </summary>
        /// <param name="printWriter">Output stream to write the file.</param>
        /// <param name="index">Position of this posting list in the inverted index.</param>
        public void WriteToFile(StreamWriter printWriter, int index)
        {
            if (Size() > 0)
            {
                printWriter.Write(index + " " + Size() + "\n");
                printWriter.Write(ToString());
            }
        }

        /// <summary>
        /// Converts the posting list to a string. String is of the form all postings separated via space.
        /// </summary>
        /// <returns>String form of the posting list.</returns>
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