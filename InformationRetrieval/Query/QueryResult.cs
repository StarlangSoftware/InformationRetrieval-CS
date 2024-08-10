using System.Collections.Generic;
using DataStructure.Heap;

namespace InformationRetrieval.Query
{
    public class QueryResult
    {
        private List<QueryResultItem> _items;

        /**
         * Empty constructor for the QueryResult object.
         */
        public QueryResult()
        {
            _items = new List<QueryResultItem>();
        }

        /// <summary>
        /// Adds a new result item to the list of query result.
        /// </summary>
        /// <param name="docId">Document id of the result</param>
        /// <param name="score">Score of the result</param>
        public void Add(int docId, double score)
        {
            _items.Add(new QueryResultItem(docId, score));
        }

        /// <summary>
        /// Adds a new result item with score 0 to the list of query result.
        /// </summary>
        /// <param name="docId">Document id of the result</param>
        public void Add(int docId)
        {
            _items.Add(new QueryResultItem(docId, 0.0));
        }

        /// <summary>
        /// Returns number of results for query
        /// </summary>
        /// <returns>Number of results for query</returns>
        public int Size(){
            return _items.Count;
        }

        /// <summary>
        /// Returns result list for query
        /// </summary>
        /// <returns>Result list for query</returns>
        public List<QueryResultItem> GetItems()
        {
            return _items;
        }

        /// <summary>
        /// Given two query results, this method identifies the intersection of those two results by doing parallel iteration
        /// in O(N).
        /// </summary>
        /// <param name="queryResult">Second query result to be intersected.</param>
        /// <returns>Intersection of this query result with the second query result</returns>
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

        /// <summary>
        /// Given two query results, this method identifies the intersection of those two results by doing binary search on
        /// the second list in O(N log N).
        /// </summary>
        /// <param name="queryResult">Second query result to be intersected.</param>
        /// <returns>Intersection of this query result with the second query result</returns>
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

        /// <summary>
        /// Given two query results, this method identifies the intersection of those two results by doing exhaustive search
        /// on the second list in O(N^2).
        /// </summary>
        /// <param name="queryResult">Second query result to be intersected.</param>
        /// <returns>Intersection of this query result with the second query result</returns>
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

        /// <summary>
        /// The method returns K best results from the query result using min heap in O(K log N + N log K) time.
        /// </summary>
        /// <param name="K">Size of the best subset.</param>
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