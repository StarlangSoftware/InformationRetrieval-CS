using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dictionary.Dictionary;

namespace InformationRetrieval.Query
{
    public class Query
    {
        private List<Word> _terms;
        private static readonly List<string> Shortcuts = new List<string>()
        {
            "cc", "cm2", "cm", "gb", "ghz", "gr", "gram", "hz", "inc", "inch", "inç", "kg", "kw", "kva", "litre", "lt",
            "m2", "m3", "mah", "mb", "metre", "mg", "mhz", "ml", "mm", "mp", "ms", "kb", "mb", "gb", "tb", "pb", "kbps",
            "mt", "mv", "tb", "tl", "va", "volt", "watt", "ah", "hp", "oz", "rpm", "dpi", "ppm", "ohm", "kwh", "kcal",
            "kbit", "mbit", "gbit", "bit", "byte", "mbps", "gbps", "cm3", "mm2", "mm3", "khz", "ft", "db", "sn", "g", 
            "v", "m", "l", "w", "s"
        };

        /**
         * Constructor of the Query class. Initializes the terms array.
         */
        public Query()
        {
            _terms = new List<Word>();
        }

        /// <summary>
        /// Another constructor of the Query class. Splits the query into multiple words and put them into the terms array.
        /// </summary>
        /// <param name="query">Query string</param>
        public Query(string query)
        {
            _terms = new List<Word>();
            var terms = query.Split(" ");
            foreach (var term in terms)
            {
                _terms.Add(new Word(term));
            }
        }

        /// <summary>
        /// Accessor for the terms array. Returns the term at position index.
        /// </summary>
        /// <param name="index">Position of the term in the terms array.</param>
        /// <returns>The term at position index.</returns>
        public Word GetTerm(int index)
        {
            return _terms[index];
        }

        /// <summary>
        /// Returns the size of the query, i.e. number of words in the query.
        /// </summary>
        /// <returns>Size of the query, i.e. number of words in the query.</returns>
        public int Size()
        {
            return _terms.Count;
        }

        /// <summary>
        /// Filters the original query by removing phrase attributes, shortcuts and single word attributes.
        /// </summary>
        /// <param name="attributeList">Hash set containing all attributes (phrase and single word)</param>
        /// <param name="termAttributes">New query that will accumulate single word attributes from the original query.</param>
        /// <param name="phraseAttributes">New query that will accumulate phrase attributes from the original query.</param>
        /// <returns>Filtered query after removing single word and phrase attributes from the original query.</returns>
        public Query FilterAttributes(HashSet<string> attributeList, Query termAttributes, Query phraseAttributes)
        {
            var i = 0;
            var filteredQuery = new Query();
            while (i < _terms.Count)
            {
                if (i < _terms.Count - 1)
                {
                    var pair = _terms[i].GetName() + " " + _terms[i + 1].GetName();
                    if (attributeList.Contains(pair))
                    {
                        phraseAttributes._terms.Add(new Word(pair));
                        i += 2;
                        continue;
                    }

                    if (Shortcuts.Contains(_terms[i + 1].GetName()) && new Regex("^([-+]?\\d+\\.\\d+)|(\\d*\\.\\d+)$|^[-+]?\\d+$").IsMatch(_terms[i].GetName()))
                    {
                        phraseAttributes._terms.Add(new Word(pair));
                        i += 2;
                        continue;
                    }
                }

                if (attributeList.Contains(_terms[i].GetName()))
                {
                    termAttributes._terms.Add(_terms[i]);
                }
                else
                {
                    filteredQuery._terms.Add(_terms[i]);
                }

                i++;
            }

            return filteredQuery;
        }
    }
}