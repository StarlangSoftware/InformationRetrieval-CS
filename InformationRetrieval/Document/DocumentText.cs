using System.Collections.Generic;
using Corpus;
using Dictionary.Dictionary;
using InformationRetrieval.Index;

namespace InformationRetrieval.Document
{
    public class DocumentText : Corpus.Corpus
    {
        /**
         * Empty constructor for the DocumentText class.
         */
        public DocumentText()
        {
            
        }
        
        /// <summary>
        /// Another constructor for the DocumentText class. Calls super with the given file name.
        /// </summary>
        /// <param name="fileName">File name of the corpus</param>
        public DocumentText(string fileName) : base(fileName)
        {
        }

        /// <summary>
        /// Another constructor for the DocumentText class. Calls super with the given file name and sentence splitter.
        /// </summary>
        /// <param name="fileName">File name of the corpus</param>
        /// <param name="sentenceSplitter">Sentence splitter class that separates sentences.</param>
        public DocumentText(string fileName, SentenceSplitter sentenceSplitter) : base(fileName, sentenceSplitter)
        {
        }

        /// <summary>
        /// Given the corpus, creates a hash set of distinct terms. If term type is TOKEN, the terms are single word, if
        /// the term type is PHRASE, the terms are bi-words.
        /// </summary>
        /// <param name="termType">If term type is TOKEN, the terms are single word, if the term type is PHRASE, the terms are
        ///                 bi-words.</param>
        /// <returns>Hash set of terms occurring in the document.</returns>
        public HashSet<string> ConstructDistinctWordList(TermType termType)
        {
            var words = new HashSet<string>();
            for (var i = 0; i < SentenceCount(); i++)
            {
                var sentence = GetSentence(i);
                for (var j = 0; j < sentence.WordCount(); j++)
                {
                    switch (termType)
                    {
                        case TermType.TOKEN:
                            words.Add(sentence.GetWord(j).GetName());
                            break;
                        case TermType.PHRASE:
                            if (j < sentence.WordCount() - 1)
                            {
                                words.Add(sentence.GetWord(j).GetName() + " " + sentence.GetWord(j + 1).GetName());
                            }

                            break;
                    }
                }
            }

            return words;
        }

        /// <summary>
        /// Given the corpus, creates an array of terms occurring in the document in that order. If term type is TOKEN, the
        /// terms are single word, if the term type is PHRASE, the terms are bi-words.
        /// </summary>
        /// <param name="docId">Id of the document</param>
        /// <param name="termType">If term type is TOKEN, the terms are single word, if the term type is PHRASE, the terms are
        ///                 bi-words.</param>
        /// <returns>Array list of terms occurring in the document.</returns>
        public List<TermOccurrence> ConstructTermList(int docId, TermType termType)
        {
            var terms = new List<TermOccurrence>();
            var size = 0;
            for (var i = 0; i < SentenceCount(); i++)
            {
                var sentence = GetSentence(i);
                for (var j = 0; j < sentence.WordCount(); j++)
                {
                    switch (termType)
                    {
                        case TermType.TOKEN:
                            terms.Add(new TermOccurrence(sentence.GetWord(j), docId, size));
                            size++;
                            break;
                        case TermType.PHRASE:
                            if (j < sentence.WordCount() - 1)
                            {
                                terms.Add(new TermOccurrence(
                                    new Word(sentence.GetWord(j).GetName() + " " + sentence.GetWord(j + 1).GetName()),
                                    docId, size));
                                size++;
                            }

                            break;
                    }
                }
            }

            return terms;
        }
    }
}