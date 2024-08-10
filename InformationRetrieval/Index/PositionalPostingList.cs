using System.Collections.Generic;
using System.IO;
using InformationRetrieval.Query;

namespace InformationRetrieval.Index
{
    public class PositionalPostingList
    {
        private List<PositionalPosting> _postings;

        /**
         * Constructor of the PositionalPostingList class. Initializes the list.
         */
        public PositionalPostingList()
        {
            _postings = new List<PositionalPosting>();
        }

        /// <summary>
        /// Reads a positional posting list from a file. Reads N lines, where each line stores a positional posting. The
        /// first item in the line shows document id. The second item in the line shows the number of positional postings.
        /// Other items show the positional postings.
        /// </summary>
        /// <param name="br">Input stream to read from.</param>
        /// <param name="count">Number of positional postings for this positional posting list.</param>
        public PositionalPostingList(StreamReader br, int count)
        {
            _postings = new List<PositionalPosting>();
            for (var i = 0; i < count; i++)
            {
                var line = br.ReadLine().Trim();
                var ids = line.Split(" ");
                var numberOfPositionalPostings = int.Parse(ids[1]);
                if (ids.Length == numberOfPositionalPostings + 2)
                {
                    var docId = int.Parse(ids[0]);
                    for (var j = 0; j < numberOfPositionalPostings; j++)
                    {
                        var positionalPosting = int.Parse(ids[j + 2]);
                        Add(docId, positionalPosting);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the number of positional postings in the posting list.
        /// </summary>
        /// <returns>Number of positional postings in the posting list.</returns>
        public int Size()
        {
            return _postings.Count;
        }

        /// <summary>
        /// Does a binary search on the positional postings list for a specific document id.
        /// </summary>
        /// <param name="docId">Document id to be searched.</param>
        /// <returns>The position of the document id in the positional posting list. If it does not exist, the method returns
        /// -1.</returns>
        public int GetIndex(int docId)
        {
            int begin = 0, end = Size() - 1, middle;
            while (begin <= end)
            {
                middle = (begin + end) / 2;
                if (docId == _postings[middle].GetDocId())
                {
                    return middle;
                }
                else
                {
                    if (docId < _postings[middle].GetDocId())
                    {
                        end = middle - 1;
                    }
                    else
                    {
                        begin = middle + 1;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Converts the positional postings list to a query result object. Simply adds all positional postings one by one
        /// to the result.
        /// </summary>
        /// <returns>QueryResult object containing the positional postings in this object.</returns>
        public QueryResult ToQueryResult()
        {
            var result = new QueryResult();
            foreach (var posting in _postings)
            {
                result.Add(posting.GetDocId());
            }

            return result;
        }

        /// <summary>
        /// Adds a new positional posting (document id and position) to the posting list.
        /// </summary>
        /// <param name="docId">New document id to be added to the positional posting list.</param>
        /// <param name="position">New position to be added to the positional posting list.</param>
        public void Add(int docId, int position)
        {
            var index = GetIndex(docId);
            if (index == -1)
            {
                _postings.Add(new PositionalPosting(docId));
                _postings[_postings.Count - 1].Add(position);
            }
            else
            {
                _postings[index].Add(position);
            }
        }

        /// <summary>
        /// Gets the positional posting at position index.
        /// </summary>
        /// <param name="index">Position of the positional posting.</param>
        /// <returns>The positional posting at position index.</returns>
        public PositionalPosting Get(int index)
        {
            return _postings[index];
        }

        /// <summary>
        /// Returns simple union of two positional postings list p1 and p2. The algorithm assumes the intersection of two
        /// positional postings list is empty, therefore the union is just concatenation of two positional postings lists.
        /// </summary>
        /// <param name="secondList">p2</param>
        /// <returns>Union of two positional postings lists.</returns>
        public PositionalPostingList Union(PositionalPostingList secondList)
        {
            var result = new PositionalPostingList();
            result._postings = new List<PositionalPosting>();
            result._postings.AddRange(_postings);
            result._postings.AddRange(secondList._postings);
            return result;
        }

        /// <summary>
        /// Algorithm for the intersection of two positional postings lists p1 and p2. We maintain pointers into both lists
        /// and walk through the two positional postings lists simultaneously, in time linear in the total number of postings
        /// entries. At each step, we compare the docID pointed to by both pointers. If they are not the same, we advance the
        /// pointer pointing to the smaller docID. Otherwise, we advance both pointers and do the same intersection search on
        /// the positional lists of two documents. Similarly, we compare the positions pointed to by both position pointers.
        /// If they are successive, we add the position to the result and advance both position pointers. Otherwise, we
        /// advance the pointer pointing to the smaller position.
        /// </summary>
        /// <param name="secondList">p2, second posting list.</param>
        /// <returns>Intersection of two postings lists p1 and p2.</returns>
        public PositionalPostingList Intersection(PositionalPostingList secondList)
        {
            int i = 0, j = 0;
            var result = new PositionalPostingList();
            while (i < _postings.Count && j < secondList._postings.Count)
            {
                var p1 = _postings[i];
                var p2 = secondList._postings[j];
                if (p1.GetDocId() == p2.GetDocId())
                {
                    var position1 = 0;
                    var position2 = 0;
                    var postings1 = p1.GetPositions();
                    var postings2 = p2.GetPositions();
                    while (position1 < postings1.Count && position2 < postings2.Count)
                    {
                        if (postings1[position1].GetId() + 1 == postings2[position2].GetId())
                        {
                            result.Add(p1.GetDocId(), postings2[position2].GetId());
                            position1++;
                            position2++;
                        }
                        else
                        {
                            if (postings1[position1].GetId() + 1 < postings2[position2].GetId())
                            {
                                position1++;
                            }
                            else
                            {
                                position2++;
                            }
                        }
                    }

                    i++;
                    j++;
                }
                else
                {
                    if (p1.GetDocId() < p2.GetDocId())
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
        /// Prints this object into a file with the given index.
        /// </summary>
        /// <param name="streamWriter">Output stream to write the file.</param>
        /// <param name="index">Position of this positional posting list in the inverted index.</param>
        public void WriteToFile(StreamWriter streamWriter, int index)
        {
            if (Size() > 0)
            {
                streamWriter.Write(index + " " + Size() + "\n");
                streamWriter.Write(ToString());
            }
        }

        /// <summary>
        /// Converts the positional posting list to a string. String is of the form all postings separated via space.
        /// </summary>
        /// <returns>String form of the positional posting list.</returns>
        public new string ToString()
        {
            var result = "";
            foreach (var positionalPosting in _postings)
            {
                result += "\t" + positionalPosting.ToString() + "\n";
            }

            return result;
        }
    }
}