using System;
using System.Collections.Generic;
using System.IO;
using Dictionary.Dictionary;

namespace InformationRetrieval.Index
{
    public class TermDictionary : Dictionary.Dictionary.Dictionary
    {
        /// <summary>
        /// Constructor of the TermDictionary. Initializes the comparator for terms and the hasp map.
        /// </summary>
        /// <param name="comparator">Comparator method to compare two terms.</param>
        public TermDictionary(IComparer<Word> comparator) : base(comparator)
        {
        }

        /// <summary>
        /// Constructor of the TermDictionary. Reads the terms and their ids from the given dictionary file. Each line stores
        /// the term id and the term name separated via space.
        /// </summary>
        /// <param name="comparator">Comparator method to compare two terms.</param>
        /// <param name="fileName">Dictionary file name</param>
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

        /// <summary>
        /// Constructs the TermDictionary from a list of tokens (term occurrences). The terms array should be sorted
        /// before calling this method. Constructs the distinct terms and their corresponding term ids.
        /// </summary>
        /// <param name="comparator">Comparator method to compare two terms.</param>
        /// <param name="terms">Sorted list of tokens in the memory collection.</param>
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

        /// <summary>
        /// Constructs the TermDictionary from a hash set of tokens (strings). Constructs sorted dictinct terms array and
        /// their corresponding term ids.
        /// </summary>
        /// <param name="comparator">Comparator method to compare two terms.</param>
        /// <param name="words">Hash set of tokens in the memory collection.</param>
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

        /// <summary>
        /// Adds a new term to the sorted words array. First the term is searched in the words array using binary search,
        /// then the word is added into the correct place.
        /// </summary>
        /// <param name="name">Lemma of the term</param>
        /// <param name="termId">Id of the term</param>
        public void AddTerm(string name, int termId)
        {
            var middle = words.BinarySearch(new Word(name), comparator);
            if (middle < 0)
            {
                words.Insert(~middle, new Term(name, termId));
            }
        }

        /// <summary>
        /// Saves the term dictionary into the dictionary file. Each line stores the term id and the term name separated via
        /// space.
        /// </summary>
        /// <param name="fileName">Dictionary file name. Real dictionary file name is created by attaching -dictionary.txt to this
        /// file name</param>
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

        /// <summary>
        /// Constructs all NGrams from a given word. For example, 3 grams for word "term" are "$te", "ter", "erm", "rm$".
        /// </summary>
        /// <param name="word">Word for which NGrams will b created.</param>
        /// <param name="termId">Term id to add into the posting list.</param>
        /// <param name="k">N in NGram.</param>
        /// <returns>An array of NGrams for a given word.</returns>
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

        /// <summary>
        /// Constructs all NGrams for all words in the dictionary. For example, 3 grams for word "term" are "$te", "ter",
        /// "erm", "rm$".
        /// </summary>
        /// <param name="k">N in NGram.</param>
        /// <returns>A sorted array of NGrams for all words in the dictionary.</returns>
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