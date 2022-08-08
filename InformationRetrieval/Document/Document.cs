using Corpus;
using Dictionary.Dictionary;
using MorphologicalAnalysis;
using MorphologicalDisambiguation;

namespace InformationRetrieval.Document
{
    public class Document
    {
        private string _absoluteFileName;
        private string _fileName;
        private int _docId;
        private int _size = 0;

        public Document(string absoluteFileName, string fileName, int docId)
        {
            _docId = docId;
            _absoluteFileName = absoluteFileName;
            _fileName = fileName;
        }

        public DocumentText LoadDocument()
        {
            DocumentText documentText;
            documentText = new DocumentText(_absoluteFileName, new TurkishSplitter());
            _size = documentText.NumberOfWords();
            return documentText;
        }

        public Corpus.Corpus NormalizeDocument(MorphologicalDisambiguator disambiguator, FsmMorphologicalAnalyzer fsm)
        {
            var corpus = new Corpus.Corpus(_absoluteFileName);
            for (var i = 0; i < corpus.SentenceCount(); i++)
            {
                var sentence = corpus.GetSentence(i);
                var parses = fsm.RobustMorphologicalAnalysis(sentence);
                var correctParses = disambiguator.Disambiguate(parses);
                var newSentence = new Sentence();
                foreach (var fsmParse in correctParses)
                {
                    newSentence.AddWord(new Word(fsmParse.GetWord().GetName()));
                }

                corpus.AddSentence(newSentence);
            }

            _size = corpus.NumberOfWords();
            return corpus;
        }

        public int GetDocId()
        {
            return _docId;
        }

        public string GetFileName()
        {
            return _fileName;
        }

        public string GetAbsoluteFileName()
        {
            return _absoluteFileName;
        }

        public int GetSize()
        {
            return _size;
        }
    }
}