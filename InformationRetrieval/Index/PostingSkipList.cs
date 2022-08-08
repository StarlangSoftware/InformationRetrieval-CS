namespace InformationRetrieval.Index
{
    public class PostingSkipList : PostingList
    {
        bool _skipped;

        public PostingSkipList()
        {
            _skipped = false;
        }

        public new void Add(int docId)
        {
            PostingSkip p = new PostingSkip(docId);
            ((PostingSkip)Postings[Postings.Count - 1]).SetNext(p);
            Postings.Add(p);
        }

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