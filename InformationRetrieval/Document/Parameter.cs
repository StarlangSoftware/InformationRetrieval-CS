using System.Collections.Generic;
using Dictionary.Dictionary;
using MorphologicalAnalysis;
using MorphologicalDisambiguation;

namespace InformationRetrieval.Document
{
    public class Parameter
    {
        private IndexType _indexType = IndexType.INVERTED_INDEX;
        private IComparer<Word> _wordComparator = new TurkishWordComparator();
        private bool _loadIndexesFromFile;
        private MorphologicalDisambiguator _disambiguator;
        private FsmMorphologicalAnalyzer _fsm;
        private bool _normalizeDocument;
        private bool _phraseIndex = true;
        private bool _positionalIndex = true;
        private bool _constructNGramIndex = true;
        private bool _limitNumberOfDocumentsLoaded;
        private int _documentLimit = 1000;
        private int _wordLimit = 10000;
        private DocumentType _documentType = DocumentType.NORMAL;
        private int _representativeCount = 10;

        public IndexType GetIndexType()
        {
            return _indexType;
        }

        public IComparer<Word> GetWordComparator()
        {
            return _wordComparator;
        }

        public bool LoadIndexesFromFile()
        {
            return _loadIndexesFromFile;
        }

        public MorphologicalDisambiguator GetDisambiguator()
        {
            return _disambiguator;
        }

        public FsmMorphologicalAnalyzer GetFsm()
        {
            return _fsm;
        }

        public bool ConstructPhraseIndex()
        {
            return _phraseIndex;
        }

        public bool NormalizeDocument()
        {
            return _normalizeDocument;
        }

        public bool ConstructPositionalIndex()
        {
            return _positionalIndex;
        }

        public bool ConstructNGramIndex()
        {
            return _constructNGramIndex;
        }

        public bool LimitNumberOfDocumentsLoaded()
        {
            return _limitNumberOfDocumentsLoaded;
        }

        public int GetDocumentLimit()
        {
            return _documentLimit;
        }

        public int GetWordLimit()
        {
            return _wordLimit;
        }

        public int GetRepresentativeCount()
        {
            return _representativeCount;
        }

        public void SetIndexType(IndexType indexType)
        {
            _indexType = indexType;
        }

        public void SetWordComparator(IComparer<Word> wordComparator)
        {
            _wordComparator = wordComparator;
        }

        public void SetLoadIndexesFromFile(bool loadIndexesFromFile)
        {
            _loadIndexesFromFile = loadIndexesFromFile;
        }

        public void SetDisambiguator(MorphologicalDisambiguator disambiguator)
        {
            _disambiguator = disambiguator;
        }

        public void SetFsm(FsmMorphologicalAnalyzer fsm)
        {
            _fsm = fsm;
        }

        public void SetNormalizeDocument(bool normalizeDocument)
        {
            _normalizeDocument = normalizeDocument;
        }

        public void SetPhraseIndex(bool phraseIndex)
        {
            _phraseIndex = phraseIndex;
        }

        public void SetPositionalIndex(bool positionalIndex)
        {
            _positionalIndex = positionalIndex;
        }

        public void SetNGramIndex(bool nGramIndex)
        {
            _constructNGramIndex = nGramIndex;
        }

        public void SetLimitNumberOfDocumentsLoaded(bool limitNumberOfDocumentsLoaded)
        {
            _limitNumberOfDocumentsLoaded = limitNumberOfDocumentsLoaded;
        }

        public void SetDocumentLimit(int documentLimit)
        {
            _documentLimit = documentLimit;
        }

        public void SetWordLimit(int wordLimit)
        {
            _wordLimit = wordLimit;
        }

        public void SetRepresentativeCount(int representativeCount)
        {
            _representativeCount = representativeCount;
        }

        public DocumentType GetDocumentType()
        {
            return _documentType;
        }

        public void SetDocumentType(DocumentType documentType)
        {
            _documentType = documentType;
        }
    }
}