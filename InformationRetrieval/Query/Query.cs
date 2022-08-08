using System.Collections.Generic;
using Dictionary.Dictionary;

namespace InformationRetrieval.Query
{
    public class Query
    {
        private List<Word> _terms;

        public Query(string query)
        {
            _terms = new List<Word>();
            var terms = query.Split(" ");
            foreach (var term in terms)
            {
                _terms.Add(new Word(term));
            }
        }

        public Word GetTerm(int index)
        {
            return _terms[index];
        }

        public int Size()
        {
            return _terms.Count;
        }
    }
}