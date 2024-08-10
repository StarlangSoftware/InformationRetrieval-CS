using System.Collections.Generic;

namespace InformationRetrieval.Index
{
    public class PositionalPosting
    {
        private List<Posting> _positions;
        private int _docId;

        /// <summary>
        /// Constructor for the PositionalPosting class. Sets the document id and initializes the position array.
        /// </summary>
        /// <param name="docId">document id of the posting.</param>
        public PositionalPosting(int docId)
        {
            this._docId = docId;
            _positions = new List<Posting>();
        }

        /// <summary>
        /// Adds a position to the position list.
        /// </summary>
        /// <param name="position">Position added to the position list.</param>
        public void Add(int position)
        {
            _positions.Add(new Posting(position));
        }

        /// <summary>
        /// Accessor for the document id attribute.
        /// </summary>
        /// <returns>Document id.</returns>
        public int GetDocId()
        {
            return _docId;
        }

        /// <summary>
        /// Accessor for the positions attribute.
        /// </summary>
        /// <returns>Position list.</returns>
        public List<Posting> GetPositions()
        {
            return _positions;
        }

        /// <summary>
        /// Returns size of the position list.
        /// </summary>
        /// <returns>Size of the position list.</returns>
        public int Size()
        {
            return _positions.Count;
        }

        /// <summary>
        /// Converts the positional posting to a string. String is of the form, document id, number of positions, and all
        /// positions separated via space.
        /// </summary>
        /// <returns>String form of the positional posting.</returns>
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