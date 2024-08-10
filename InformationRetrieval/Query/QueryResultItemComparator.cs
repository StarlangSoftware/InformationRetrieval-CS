using System.Collections.Generic;

namespace InformationRetrieval.Query
{
    public class QueryResultItemComparator : Comparer<QueryResultItem>
    {
        /// <summary>
        /// Compares two query result items according to their scores.
        /// </summary>
        /// <param name="resultA">the first query result item to be compared.</param>
        /// <param name="resultB">the second query result item to be compared.</param>
        /// <returns>-1 if the score of the first item is smaller than the score of the second item; 1 if the score of the
        /// first item is larger than the score of the second item; 0 otherwise.</returns>
        public override int Compare(QueryResultItem resultA, QueryResultItem resultB)
        {
            if (resultA.GetScore() > resultB.GetScore())
            {
                return 1;
            }
            else
            {
                if (resultA.GetScore() < resultB.GetScore())
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}