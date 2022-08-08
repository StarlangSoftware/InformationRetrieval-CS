using System.Collections.Generic;
using InformationRetrieval.Query;

namespace InformationRetrieval.Index
{
    public class IncidenceMatrix
    {
        private bool[][] _incidenceMatrix;
        private int _dictionarySize;
        private int _documentSize;

        public IncidenceMatrix(int dictionarySize, int documentSize)
        {
            this._dictionarySize = dictionarySize;
            this._documentSize = documentSize;
            _incidenceMatrix = new bool[dictionarySize][];
            for (var i = 0; i < dictionarySize; i++)
            {
                _incidenceMatrix[i] = new bool[documentSize];
            }
        }

        public IncidenceMatrix(List<TermOccurrence> terms, TermDictionary dictionary, int documentSize) : this(
            dictionary.Size(), documentSize)
        {
            int i;
            TermOccurrence term;
            if (terms.Count > 0)
            {
                term = terms[0];
                i = 1;
                Set(dictionary.GetWordIndex(term.GetTerm().GetName()), term.GetDocId());
                while (i < terms.Count)
                {
                    term = terms[i];
                    Set(dictionary.GetWordIndex(term.GetTerm().GetName()), term.GetDocId());
                    i++;
                }
            }
        }

        public void Set(int row, int col)
        {
            _incidenceMatrix[row][col] = true;
        }

        public QueryResult Search(Query.Query query, TermDictionary dictionary)
        {
            int i, j, termIndex;
            bool[] resultRow;
            var result = new QueryResult();
            resultRow = new bool[_documentSize];
            for (i = 0; i < _documentSize; i++)
            {
                resultRow[i] = true;
            }

            for (i = 0; i < query.Size(); i++)
            {
                termIndex = dictionary.GetWordIndex(query.GetTerm(i).GetName());
                if (termIndex != -1)
                {
                    for (j = 0; j < _documentSize; j++)
                    {
                        resultRow[j] = resultRow[j] && _incidenceMatrix[termIndex][j];
                    }
                }
                else
                {
                    return result;
                }
            }

            for (i = 0; i < _documentSize; i++)
            {
                if (resultRow[i])
                {
                    result.Add(i);
                }
            }

            return result;
        }
    }
}