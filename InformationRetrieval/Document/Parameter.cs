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

        /// <summary>
        /// Accessor for the index type search parameter. Index can be inverted index or incidence matrix.
        /// </summary>
        /// <returns>Index type search parameter</returns>
        public IndexType GetIndexType()
        {
            return _indexType;
        }

        /// <summary>
        /// Accessor for the word comparator. Word comparator is a function to compare terms.
        /// </summary>
        /// <returns>Word comparator</returns>
        public IComparer<Word> GetWordComparator()
        {
            return _wordComparator;
        }

        /// <summary>
        /// Accessor for the loadIndexesFromFile search parameter. If loadIndexesFromFile is true, all the indexes will be
        /// read from the file, otherwise they will be reconstructed.
        /// </summary>
        /// <returns>loadIndexesFromFile search parameter</returns>
        public bool LoadIndexesFromFile()
        {
            return _loadIndexesFromFile;
        }

        /// <summary>
        /// Accessor for the disambiguator search parameter. The disambiguator is used for morphological disambiguation for
        /// the terms in Turkish.
        /// </summary>
        /// <returns>disambiguator search parameter</returns>
        public MorphologicalDisambiguator GetDisambiguator()
        {
            return _disambiguator;
        }

        /// <summary>
        /// Accessor for the fsm search parameter. The fsm is used for morphological analysis for  the terms in Turkish.
        /// </summary>
        /// <returns>fsm search parameter</returns>
        public FsmMorphologicalAnalyzer GetFsm()
        {
            return _fsm;
        }

        /// <summary>
        /// Accessor for the constructPhraseIndex search parameter. If constructPhraseIndex is true, phrase indexes will be
        /// reconstructed or used in query processing.
        /// </summary>
        /// <returns>constructPhraseIndex search parameter</returns>
        public bool ConstructPhraseIndex()
        {
            return _phraseIndex;
        }

        /// <summary>
        /// Accessor for the normalizeDocument search parameter. If normalizeDocument is true, the terms in the document will
        /// be preprocessed by morphological anaylysis and some preprocessing techniques.
        /// </summary>
        /// <returns>normalizeDocument search parameter</returns>
        public bool NormalizeDocument()
        {
            return _normalizeDocument;
        }

        /// <summary>
        /// Accessor for the positionalIndex search parameter. If positionalIndex is true, positional indexes will be
        /// reconstructed or used in query processing.
        /// </summary>
        /// <returns>positionalIndex search parameter</returns>
        public bool ConstructPositionalIndex()
        {
            return _positionalIndex;
        }

        /// <summary>
        /// Accessor for the constructNGramIndex search parameter. If constructNGramIndex is true, N-Gram indexes will be
        /// reconstructed or used in query processing.
        /// </summary>
        /// <returns>constructNGramIndex search parameter</returns>
        public bool ConstructNGramIndex()
        {
            return _constructNGramIndex;
        }

        /// <summary>
        /// Accessor for the limitNumberOfDocumentsLoaded search parameter. If limitNumberOfDocumentsLoaded is true,
        /// the query result will be filtered according to the documentLimit search parameter.
        /// </summary>
        /// <returns>limitNumberOfDocumentsLoaded search parameter</returns>
        public bool LimitNumberOfDocumentsLoaded()
        {
            return _limitNumberOfDocumentsLoaded;
        }

        /// <summary>
        /// Accessor for the documentLimit search parameter. If limitNumberOfDocumentsLoaded is true,  the query result will
        /// be filtered according to the documentLimit search parameter.
        /// </summary>
        /// <returns>documentLimit search parameter</returns>
        public int GetDocumentLimit()
        {
            return _documentLimit;
        }

        /// <summary>
        /// Accessor for the wordLimit search parameter. wordLimit is the limit on the partial term dictionary size. For
        /// large collections, we term dictionaries are divided into multiple files, this parameter sets the number of terms
        /// in those separate dictionaries.
        /// </summary>
        /// <returns>wordLimit search parameter</returns>
        public int GetWordLimit()
        {
            return _wordLimit;
        }

        /// <summary>
        /// Accessor for the representativeCount search parameter. representativeCount is the maximum number of representative
        /// words in the category based query search.
        /// </summary>
        /// <returns>representativeCount search parameter</returns>
        public int GetRepresentativeCount()
        {
            return _representativeCount;
        }

        /// <summary>
        /// Mutator for the index type search parameter. Index can be inverted index or incidence matrix.
        /// </summary>
        /// <param name="indexType">Index type search parameter</param>
        public void SetIndexType(IndexType indexType)
        {
            _indexType = indexType;
        }

        /// <summary>
        /// Mutator for the word comparator. Word comparator is a function to compare terms.
        /// </summary>
        /// <param name="wordComparator">Word comparator</param>
        public void SetWordComparator(IComparer<Word> wordComparator)
        {
            _wordComparator = wordComparator;
        }

        /// <summary>
        /// Mutator for the loadIndexesFromFile search parameter. If loadIndexesFromFile is true, all the indexes will be
        /// read from the file, otherwise they will be reconstructed.
        /// </summary>
        /// <param name="loadIndexesFromFile">loadIndexesFromFile search parameter</param>
        public void SetLoadIndexesFromFile(bool loadIndexesFromFile)
        {
            _loadIndexesFromFile = loadIndexesFromFile;
        }

        /// <summary>
        /// Mutator for the disambiguator search parameter. The disambiguator is used for morphological disambiguation for
        /// the terms in Turkish.
        /// </summary>
        /// <param name="disambiguator">disambiguator search parameter</param>
        public void SetDisambiguator(MorphologicalDisambiguator disambiguator)
        {
            _disambiguator = disambiguator;
        }

        /// <summary>
        /// Mutator for the fsm search parameter. The fsm is used for morphological analysis for the terms in Turkish.
        /// </summary>
        /// <param name="fsm">fsm search parameter</param>
        public void SetFsm(FsmMorphologicalAnalyzer fsm)
        {
            _fsm = fsm;
        }

        /// <summary>
        /// Mutator for the normalizeDocument search parameter. If normalizeDocument is true, the terms in the document will
        /// be preprocessed by morphological anaylysis and some preprocessing techniques.
        /// </summary>
        /// <param name="normalizeDocument">normalizeDocument search parameter</param>
        public void SetNormalizeDocument(bool normalizeDocument)
        {
            _normalizeDocument = normalizeDocument;
        }

        /// <summary>
        /// Mutator for the constructPhraseIndex search parameter. If constructPhraseIndex is true, phrase indexes will be
        /// reconstructed or used in query processing.
        /// </summary>
        /// <param name="phraseIndex">constructPhraseIndex search parameter</param>
        public void SetPhraseIndex(bool phraseIndex)
        {
            _phraseIndex = phraseIndex;
        }

        /// <summary>
        /// Mutator for the positionalIndex search parameter. If positionalIndex is true, positional indexes will be
        /// reconstructed or used in query processing.
        /// </summary>
        /// <param name="positionalIndex">positionalIndex search parameter</param>
        public void SetPositionalIndex(bool positionalIndex)
        {
            _positionalIndex = positionalIndex;
        }

        /// <summary>
        /// Mutator for the constructNGramIndex search parameter. If constructNGramIndex is true, N-Gram indexes will be
        /// reconstructed or used in query processing.
        /// </summary>
        /// <param name="nGramIndex">constructNGramIndex search parameter</param>
        public void SetNGramIndex(bool nGramIndex)
        {
            _constructNGramIndex = nGramIndex;
        }

        /// <summary>
        /// Mutator for the limitNumberOfDocumentsLoaded search parameter. If limitNumberOfDocumentsLoaded is true,
        /// the query result will be filtered according to the documentLimit search parameter.
        /// </summary>
        /// <param name="limitNumberOfDocumentsLoaded">limitNumberOfDocumentsLoaded search parameter</param>
        public void SetLimitNumberOfDocumentsLoaded(bool limitNumberOfDocumentsLoaded)
        {
            _limitNumberOfDocumentsLoaded = limitNumberOfDocumentsLoaded;
        }

        /// <summary>
        /// Mutator for the documentLimit search parameter. If limitNumberOfDocumentsLoaded is true,  the query result will
        /// be filtered according to the documentLimit search parameter.
        /// </summary>
        /// <param name="documentLimit">documentLimit search parameter</param>
        public void SetDocumentLimit(int documentLimit)
        {
            _documentLimit = documentLimit;
        }

        /// <summary>
        /// Mutator for the documentLimit search parameter. If limitNumberOfDocumentsLoaded is true,  the query result will
        /// be filtered according to the documentLimit search parameter.
        /// </summary>
        /// <param name="wordLimit">wordLimit search parameter</param>
        public void SetWordLimit(int wordLimit)
        {
            _wordLimit = wordLimit;
        }

        /// <summary>
        /// Mutator for the representativeCount search parameter. representativeCount is the maximum number of representative
        /// words in the category based query search.
        /// </summary>
        /// <param name="representativeCount">representativeCount search parameter</param>
        public void SetRepresentativeCount(int representativeCount)
        {
            _representativeCount = representativeCount;
        }

        /// <summary>
        /// Accessor for the document type search parameter. Document can be normal or a categorical document.
        /// </summary>
        /// <returns></returns>
        public DocumentType GetDocumentType()
        {
            return _documentType;
        }

        /// <summary>
        /// Mutator for the document type search parameter. Document can be normal or a categorical document.
        /// </summary>
        /// <param name="documentType">Document type search parameter</param>
        public void SetDocumentType(DocumentType documentType)
        {
            _documentType = documentType;
        }
    }
}