using System.Collections.Generic;
using InformationRetrieval.Index;

namespace InformationRetrieval.Document
{
    public class MediumCollection : DiskCollection
    {
        public MediumCollection(string directory, Parameter parameter) : base(directory, parameter)
        {
            ConstructIndexesInDisk();
        }

        private HashSet<string> ConstructDistinctWordList(TermType termType)
        {
            var words = new HashSet<string>();
            foreach (var doc in Documents)
            {
                var documentText = doc.LoadDocument();
                words.UnionWith(documentText.ConstructDistinctWordList(termType));
            }

            return words;
        }

        private void ConstructIndexesInDisk()
        {
            var wordList = ConstructDistinctWordList(TermType.TOKEN);
            Dictionary = new TermDictionary(Comparator, wordList);
            ConstructInvertedIndexInDisk(Dictionary, TermType.TOKEN);
            if (Parameter.ConstructPositionalIndex())
            {
                ConstructPositionalIndexInDisk(Dictionary, TermType.TOKEN);
            }

            if (Parameter.ConstructPhraseIndex())
            {
                wordList = ConstructDistinctWordList(TermType.PHRASE);
                PhraseDictionary = new TermDictionary(Comparator, wordList);
                ConstructInvertedIndexInDisk(PhraseDictionary, TermType.PHRASE);
                if (Parameter.ConstructPositionalIndex())
                {
                    ConstructPositionalIndexInDisk(PhraseDictionary, TermType.PHRASE);
                }
            }

            if (Parameter.ConstructNGramIndex())
            {
                ConstructNGramIndex();
            }
        }

        private void ConstructInvertedIndexInDisk(TermDictionary dictionary, TermType termType)
        {
            int i = 0, blockCount = 0;
            var invertedIndex = new InvertedIndex();
            foreach (Document doc in Documents)
            {
                if (i < Parameter.GetDocumentLimit())
                {
                    i++;
                }
                else
                {
                    invertedIndex.Save("tmp-" + blockCount);
                    invertedIndex = new InvertedIndex();
                    blockCount++;
                    i = 0;
                }

                var documentText = doc.LoadDocument();
                var wordList = documentText.ConstructDistinctWordList(termType);
                foreach (string word in wordList)
                {
                    int termId = dictionary.GetWordIndex(word);
                    invertedIndex.Add(termId, doc.GetDocId());
                }
            }

            if (Documents.Count != 0)
            {
                invertedIndex.Save("tmp-" + blockCount);
                blockCount++;
            }

            if (termType == TermType.TOKEN)
            {
                CombineMultipleInvertedIndexesInDisk(Name, "", blockCount);
            }
            else
            {
                CombineMultipleInvertedIndexesInDisk(Name + "-phrase", "", blockCount);
            }
        }

        private void ConstructPositionalIndexInDisk(TermDictionary dictionary, TermType termType)
        {
            int i = 0, blockCount = 0;
            var positionalIndex = new PositionalIndex();
            foreach (var doc in Documents)
            {
                if (i < Parameter.GetDocumentLimit())
                {
                    i++;
                }
                else
                {
                    positionalIndex.Save("tmp-" + blockCount);
                    positionalIndex = new PositionalIndex();
                    blockCount++;
                    i = 0;
                }

                var documentText = doc.LoadDocument();
                var terms = documentText.ConstructTermList(doc.GetDocId(), termType);
                foreach (TermOccurrence termOccurrence in terms)
                {
                    var termId = dictionary.GetWordIndex(termOccurrence.GetTerm().GetName());
                    positionalIndex.AddPosition(termId, termOccurrence.GetDocId(), termOccurrence.GetPosition());
                }
            }

            if (Documents.Count != 0)
            {
                positionalIndex.Save("tmp-" + blockCount);
                blockCount++;
            }

            if (termType == TermType.TOKEN)
            {
                CombineMultiplePositionalIndexesInDisk(Name, blockCount);
            }
            else
            {
                CombineMultiplePositionalIndexesInDisk(Name + "-phrase", blockCount);
            }
        }
    }
}