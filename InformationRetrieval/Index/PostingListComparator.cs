using System.Collections.Generic;

namespace InformationRetrieval.Index
{
    public class PostingListComparator : IComparer<PostingList>
    {
        /// <summary>
        /// Comparator method to compare two posting lists.
        /// </summary>
        /// <param name="listA">the first posting list to be compared.</param>
        /// <param name="listB">the second posting list to be compared.</param>
        /// <returns>1 if the size of the first posting list is larger than the second one, -1 if the size
        /// of the first posting list is smaller than the second one, 0 if they are the same.</returns>
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