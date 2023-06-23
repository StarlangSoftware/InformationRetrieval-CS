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

        public Query()
        {
            _terms = new List<Word>();
        }

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