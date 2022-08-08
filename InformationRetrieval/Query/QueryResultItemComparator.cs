using System.Collections.Generic;

namespace InformationRetrieval.Query
{
    public class QueryResultItemComparator : IComparer<QueryResultItem>
    {
        public int Compare(QueryResultItem resultA, QueryResultItem resultB)
        {
            if (resultA.GetScore() > resultB.GetScore())
            {
                return -1;
            }
            else
            {
                if (resultA.GetScore() < resultB.GetScore())
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}