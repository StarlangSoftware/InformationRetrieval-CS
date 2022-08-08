using System;
using System.Collections.Generic;
using System.IO;
using Dictionary.Dictionary;

namespace InformationRetrieval.Index
{
    public class TermDictionary : Dictionary.Dictionary.Dictionary
    {
        public TermDictionary(IComparer<Word> comparator) : base(comparator)
        {
        }

        public TermDictionary(IComparer<Word> comparator, string fileName) : base(comparator)
        {
            var streamReader = new StreamReader(new FileStream(fileName + "-dictionary.txt", FileMode.Open));
            var line = streamReader.ReadLine();
            while (line != null)
            {
                var termId = int.Parse(line.Substring(0, line.IndexOf(" ", StringComparison.Ordinal)));
                words.Add(new Term(line.Substring(line.IndexOf(" ", StringComparison.Ordinal) + 1), termId));
                line = streamReader.ReadLine();
            }

            streamReader.Close();
        }

        public TermDictionary(IComparer<Word> comparator, List<TermOccurrence> terms) : base(comparator)
        {
            int i, termId = 0;
            TermOccurrence term, previousTerm;
            if (terms.Count > 0)
            {
                term = terms[0];
                AddTerm(term.GetTerm().GetName(), termId);
                termId++;
                previousTerm = term;
                i = 1;
                while (i < terms.Count)
                {
                    term = terms[i];
                    if (term.IsDifferent(previousTerm, comparator))
                    {
                        AddTerm(term.GetTerm().GetName(), termId);
                        termId++;
                    }

                    i++;
                    previousTerm = term;
                }
            }
        }

        public TermDictionary(IComparer<Word> comparator, HashSet<string> words) : base(comparator)
        {
            var wordList = new List<Word>();
            foreach (var word in words)
            {
                wordList.Add(new Word(word));
            }

            wordList.Sort(comparator);
            var termId = 0;
            foreach (var term in wordList)
            {
                AddTerm(term.GetName(), termId);
                termId++;
            }
        }

        public void AddTerm(string name, int termId)
        {
            var middle = words.BinarySearch(new Word(name), comparator);
            if (middle < 0)
            {
                words.Insert(~middle, new Term(name, termId));
            }
        }

        public void Save(string fileName)
        {
            var streamWriter = new StreamWriter(fileName + "-dictionary.txt");
            foreach (var word in words)
            {
                var term = (Term)word;
                streamWriter.WriteLine(term.GetTermId() + " " + term.GetName());
            }

            streamWriter.Close();
        }

        public static List<TermOccurrence> ConstructNGrams(string word, int termId, int k)
        {
            var nGrams = new List<TermOccurrence>();
            if (word.Length >= k - 1)
            {
                for (var l = -1; l < word.Length - k + 2; l++)
                {
                    string term;
                    if (l == -1)
                    {
                        term = "$" + word.Substring(0, k - 1);
                    }
                    else
                    {
                        if (l == word.Length - k + 1)
                        {
                            term = word.Substring(l, k - 1) + "$";
                        }
                        else
                        {
                            term = word.Substring(l, k);
                        }
                    }

                    nGrams.Add(new TermOccurrence(new Word(term), termId, l));
                }
            }

            return nGrams;
        }

        public List<TermOccurrence> ConstructTermsFromDictionary(int k)
        {
            var termComparator = new TermOccurrenceComparator(comparator);
            var terms = new List<TermOccurrence>();
            for (var i = 0; i < Size(); i++)
            {
                var word = GetWord(i).GetName();
                terms.AddRange(ConstructNGrams(word, i, k));
            }

            terms.Sort(termComparator);
            return terms;
        }
    }
}