namespace InformationRetrieval.Query
{
    public class QueryResultItem
    {
        private int _docId;
        private double _score;

        /// <summary>
        /// Constructor for the QueryResultItem class. Sets the document id and score of a single query result.
        /// </summary>
        /// <param name="docId">Id of the document that satisfies the query.</param>
        /// <param name="score">Score of the document for the query.</param>
        public QueryResultItem(int docId, double score)
        {
            _docId = docId;
            _score = score;
        }

        /// <summary>
        /// Accessor for the docID attribute.
        /// </summary>
        /// <returns>docID attribute</returns>
        public int GetDocId()
        {
            return _docId;
        }

        /// <summary>
        /// Accessor for the score attribute.
        /// </summary>
        /// <returns>score attribute.</returns>
        public double GetScore()
        {
            return _score;
        }
    }
}