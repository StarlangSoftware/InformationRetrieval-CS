using System.Collections.Generic;

namespace InformationRetrieval.Index
{
    public class PositionalPosting
    {
        private List<Posting> _positions;
        private int _docId;

        public PositionalPosting(int docId)
        {
            this._docId = docId;
            _positions = new List<Posting>();
        }

        public void Add(int position)
        {
            _positions.Add(new Posting(position));
        }

        public int GetDocId()
        {
            return _docId;
        }

        public List<Posting> GetPositions()
        {
            return _positions;
        }

        public int Size()
        {
            return _positions.Count;
        }

        public new string ToString()
        {
            var result = _docId + " " + _positions.Count;
            foreach (var posting in _positions)
            {
                result += " " + posting.GetId();
            }

            return result;
        }
    }
}