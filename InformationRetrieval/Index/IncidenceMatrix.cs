using System.Collections.Generic;
using InformationRetrieval.Query;

namespace InformationRetrieval.Index
{
    public class IncidenceMatrix
    {
        private bool[][] _incidenceMatrix;
        private int _dictionarySize;
        private int _documentSize;

        /// <summary>
        /// Empty constructor for the incidence matrix representation. Initializes the incidence matrix according to the
        /// given dictionary and document size.
        /// </summary>
        /// <param name="dictionarySize">Number of words in the dictionary (number of distinct words in the collection)</param>
        /// <param name="documentSize">Number of documents in the collection</param>
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

        /// <summary>
        /// Constructs an incidence matrix from a list of sorted tokens in the given terms array.
        /// </summary>
        /// <param name="terms">List of tokens in the memory collection.</param>
        /// <param name="dictionary">Term dictionary</param>
        /// <param name="documentSize">Number of documents in the collection</param>
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

        /// <summary>
        /// Sets the given cell in the incidence matrix to true.
        /// </summary>
        /// <param name="row">Row no of the cell</param>
        /// <param name="col">Column no of the cell</param>
        public void Set(int row, int col)
        {
            _incidenceMatrix[row][col] = true;
        }

        /// <summary>
        /// Searches a given query in the document collection using incidence matrix boolean search.
        /// </summary>
        /// <param name="query">Query string</param>
        /// <param name="dictionary">Term dictionary</param>
        /// <returns>The result of the query obtained by doing incidence matrix boolean search in the collection.</returns>
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