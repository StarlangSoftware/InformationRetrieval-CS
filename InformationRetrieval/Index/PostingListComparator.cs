using System.Collections.Generic;

namespace InformationRetrieval.Index
{
    public class PostingListComparator : IComparer<PostingList>
    {
        public int Compare(PostingList listA, PostingList listB)
        {
            if (listA.Size() < listB.Size()){
                return -1;
            } else {
                if (listA.Size() > listB.Size()){
                    return 1;
                } else {
                    return 0;
                }
            }
        }
    }
}