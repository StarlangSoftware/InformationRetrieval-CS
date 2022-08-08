using System.Collections.Generic;
using System.IO;
using InformationRetrieval.Query;

namespace InformationRetrieval.Index
{
    public class PositionalPostingList
    {
        private List<PositionalPosting> _postings;

        public PositionalPostingList()
        {
            _postings = new List<PositionalPosting>();
        }

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

        public int Size()
        {
            return _postings.Count;
        }

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

        public QueryResult ToQueryResult()
        {
            var result = new QueryResult();
            foreach (var posting in _postings)
            {
                result.Add(posting.GetDocId());
            }

            return result;
        }

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

        public PositionalPosting Get(int index)
        {
            return _postings[index];
        }

        public PositionalPostingList Union(PositionalPostingList secondList)
        {
            var result = new PositionalPostingList();
            result._postings = new List<PositionalPosting>();
            result._postings.AddRange(_postings);
            result._postings.AddRange(secondList._postings);
            return result;
        }

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

        public void WriteToFile(StreamWriter streamWriter, int index)
        {
            if (Size() > 0)
            {
                streamWriter.Write(index + " " + Size() + "\n");
                streamWriter.Write(ToString());
            }
        }

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