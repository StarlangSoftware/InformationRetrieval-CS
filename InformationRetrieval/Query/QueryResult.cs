using System.Collections.Generic;
using DataStructure.Heap;

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

        public int Size(){
            return _items.Count;
        }

        public List<QueryResultItem> GetItems()
        {
            return _items;
        }

        public QueryResult Intersection(QueryResult queryResult){
            var result = new QueryResult();
            int i = 0, j = 0;
            while (i < Size() && j < queryResult.Size()){
                var item1 = _items[i];
                var item2 = queryResult._items[j];
                if (item1.GetDocId() == item2.GetDocId()){
                    result.Add(item1.GetDocId());
                    i++;
                    j++;
                } else {
                    if (item1.GetDocId() < item2.GetDocId()){
                        i++;
                    } else {
                        j++;
                    }
                }
            }
            return result;
        }

        public void GetBest(int K)
        {
            var comparator = new QueryResultItemComparator();
            var minHeap = new MinHeap<QueryResultItem>(2 * K, comparator);
            foreach (QueryResultItem queryResultItem in _items){
                minHeap.Insert(queryResultItem);
            }
            _items.Clear();
            for (var i = 0; i < K && !minHeap.IsEmpty(); i++){
                _items.Add(minHeap.Delete());
            }
        }
    }
}