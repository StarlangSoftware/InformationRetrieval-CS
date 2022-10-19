using System.Collections.Generic;
using Corpus;
using Dictionary.Dictionary;
using InformationRetrieval.Index;

namespace InformationRetrieval.Document
{
    public class DocumentText : Corpus.Corpus
    {
        public DocumentText()
        {
            
        }
        public DocumentText(string fileName) : base(fileName)
        {
        }

        public DocumentText(string fileName, SentenceSplitter sentenceSplitter) : base(fileName, sentenceSplitter)
        {
        }

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