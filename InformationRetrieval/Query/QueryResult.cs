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

        public QueryResult IntersectionFastSearch(QueryResult queryResult){
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

        public QueryResult IntersectionBinarySearch(QueryResult queryResult){
            var result = new QueryResult();
            foreach (QueryResultItem searchedItem in _items){
                var low = 0;
                var high = queryResult.Size() - 1;
                var middle = (low + high) / 2;
                var found = false;
                while (low <= high){
                    if (searchedItem.GetDocId() > queryResult._items[middle].GetDocId()){
                        low = middle + 1;
                    } else {
                        if (searchedItem.GetDocId() < queryResult._items[middle].GetDocId()){
                            high = middle - 1;
                        } else {
                            found = true;
                            break;
                        }
                    }
                    middle = (low + high) / 2;
                }
                if (found){
                    result.Add(searchedItem.GetDocId(), searchedItem.GetScore());
                }
            }
            return result;
        }

        public QueryResult IntersectionLinearSearch(QueryResult queryResult)
        {
            QueryResult result = new QueryResult();
            foreach (var searchedItem in _items){
                foreach (var item in queryResult._items){
                    if (searchedItem.GetDocId() == item.GetDocId()){
                        result.Add(searchedItem.GetDocId(), searchedItem.GetScore());
                    }
                }
            }
            return result;
        }

        public void GetBest(int K)
        {
            var comparator = new QueryResultItemComparator();
            var minHeap = new MinHeap<QueryResultItem>(K, comparator);
            for (int i = 0; i < K && i < _items.Count; i++){
                minHeap.Insert(_items[i]);
            }
            for (int i = K + 1; i < _items.Count; i++){
                QueryResultItem top = minHeap.Delete();
                if (comparator.Compare(top, _items[i]) > 0){
                    minHeap.Insert(top);
                } else {
                    minHeap.Insert(_items[i]);
                }
            }
            _items.Clear();
            for (var i = 0; i < K && !minHeap.IsEmpty(); i++){
                _items.Insert(0, minHeap.Delete());
            }
        }
    }
}