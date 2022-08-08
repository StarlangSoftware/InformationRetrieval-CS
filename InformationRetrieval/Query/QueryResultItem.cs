namespace InformationRetrieval.Query
{
    public class QueryResultItem
    {
        private int _docId;
        private double _score;

        public QueryResultItem(int docId, double score)
        {
            _docId = docId;
            _score = score;
        }

        public int GetDocId()
        {
            return _docId;
        }

        public double GetScore()
        {
            return _score;
        }
    }
}