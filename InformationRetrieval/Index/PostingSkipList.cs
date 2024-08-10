namespace InformationRetrieval.Index
{
    public class PostingSkipList : PostingList
    {
        bool _skipped;

        /// <summary>
        /// Constructor for the PostingSkipList class.
        /// </summary>
        public PostingSkipList()
        {
            _skipped = false;
        }

        /// <summary>
        /// Adds a new posting (document id) to the posting skip list.
        /// </summary>
        /// <param name="docId">New document id to be added to the posting skip list.</param>
        public new void Add(int docId)
        {
            PostingSkip p = new PostingSkip(docId);
            ((PostingSkip)Postings[Postings.Count - 1]).SetNext(p);
            Postings.Add(p);
        }

        /**
         * Augments postings lists with skip pointers. Skip pointers are effectively shortcuts that allow us to avoid
         * processing parts of the postings list that will not figure in the search results. We follow a simple heuristic
         * for placing skips, which has been found to work well in practice, is that for a postings list of length P, use
         * square root of P evenly-spaced skip pointers.
         */
        public void AddSkipPointers()
        {
            int i, j, skipLength = (int)System.Math.Sqrt(Size());
            int posting;
            int skip;
            if (!_skipped)
            {
                _skipped = true;
                for (i = 0, posting = 0; posting != Postings.Count; posting++, i++)
                {
                    if (i % skipLength == 0 && i + skipLength < Size())
                    {
                        for (j = 0, skip = posting; j < skipLength; skip++)
                        {
                            j++;
                        }

                        ((PostingSkip)Postings[posting]).AddSkip((PostingSkip)Postings[skip]);
                    }
                }
            }
        }

        /// <summary>
        /// Algorithm for the intersection of two postings skip lists p1 and p2. We maintain pointers into both lists and
        /// walk through the two postings lists simultaneously, in time linear in the total number of postings entries. At
        /// each step, we compare the docID pointed to by both pointers. If they are the same, we put that docID in the
        /// results list, and advance both pointers. Otherwise, we advance the pointer pointing to the smaller docID or use
        /// skip pointers to skip as many postings as possible.
        /// </summary>
        /// <param name="secondList">p2, second posting list.</param>
        /// <returns>Intersection of two postings lists p1 and p2.</returns>
        public PostingSkipList Intersection(PostingSkipList secondList)
        {
            var p1 = (PostingSkip)Postings[0];
            var p2 = (PostingSkip)secondList.Postings[0];
            var result = new PostingSkipList();
            while (p1 != null && p2 != null)
            {
                if (p1.GetId() == p2.GetId())
                {
                    result.Add(p1.GetId());
                    p1 = p1.Next();
                    p2 = p2.Next();
                }
                else
                {
                    if (p1.GetId() < p2.GetId())
                    {
                        if (_skipped && p1.HasSkip() && p1.GetSkip().GetId() < p2.GetId())
                        {
                            while (p1.HasSkip() && p1.GetSkip().GetId() < p2.GetId())
                            {
                                p1 = p1.GetSkip();
                            }
                        }
                        else
                        {
                            p1 = p1.Next();
                        }
                    }
                    else
                    {
                        if (_skipped && p2.HasSkip() && p2.GetSkip().GetId() < p1.GetId())
                        {
                            while (p2.HasSkip() && p2.GetSkip().GetId() < p1.GetId())
                            {
                                p2 = p2.GetSkip();
                            }
                        }
                        else
                        {
                            p2 = p2.Next();
                        }
                    }
                }
            }

            return result;
        }
    }
}