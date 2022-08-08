using System.Collections.Generic;

namespace InformationRetrieval.Query
{
    public class QueryResult
    {
        private List<QueryResultItem> _items;

        public QueryResult()
        {
            _items = new List<QueryResultItem>();
        }

        public void Add(int docId, double score)
        {
            _items.Add(new QueryResultItem(docId, score));
        }

        public void Add(int docId)
        {
            _items.Add(new QueryResultItem(docId, 0.0));
        }

        public List<QueryResultItem> GetItems()
        {
            return _items;
        }

        public void Sort()
        {
            var comparator = new QueryResultItemComparator();
            _items.Sort(comparator);
        }
    }
}