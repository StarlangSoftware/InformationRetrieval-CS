using InformationRetrieval.Document;
using InformationRetrieval.Index;

namespace InformationRetrieval.Query
{
    public class SearchParameter
    {
        private CategoryDeterminationType _categoryDeterminationType = CategoryDeterminationType.KEYWORD;
        private FocusType _focusType = FocusType.OVERALL;
        private RetrievalType _retrievalType = RetrievalType.RANKED;
        private DocumentWeighting _documentWeighting = DocumentWeighting.NO_IDF;
        private TermWeighting _termWeighting = TermWeighting.NATURAL;
        private int _documentsRetrieved = 1;
        private bool _searchAttributes = false;

        /**
         * Empty constructor for SearchParameter object.
         */
        public SearchParameter(){
        }

        /// <summary>
        /// Setter for the retrievalType.
        /// </summary>
        /// <param name="retrievalType">New retrieval type</param>
        public void SetRetrievalType(RetrievalType retrievalType) {
            _retrievalType = retrievalType;
        }

        /// <summary>
        /// Mutator for the documentWeighting scheme used in tf-idf search.
        /// </summary>
        /// <param name="documentWeighting">New document weighting scheme for tf-idf search.</param>
        public void SetDocumentWeighting(DocumentWeighting documentWeighting) {
            _documentWeighting = documentWeighting;
        }

        /// <summary>
        /// Mutator for the termWeighting scheme used in tf-idf search.
        /// </summary>
        /// <param name="termWeighting">New term weighting scheme for tf-idf search.</param>
        public void SetTermWeighting(TermWeighting termWeighting) {
            _termWeighting = termWeighting;
        }

        /// <summary>
        /// Mutator for the maximum number of documents retrieved.
        /// </summary>
        /// <param name="documentsRetrieved">New value for the maximum number of documents retrieved.</param>
        public void SetDocumentsRetrieved(int documentsRetrieved) {
            _documentsRetrieved = documentsRetrieved;
        }

        /// <summary>
        /// Mutator for the focus type.
        /// </summary>
        /// <param name="focusType">New focus type.</param>
        public void SetFocusType(FocusType focusType){
            this._focusType = focusType;
        }

        /// <summary>
        /// Mutator for the category determination type.
        /// </summary>
        /// <param name="categoryDeterminationType">New category determination type.</param>
        public void SetCategoryDeterminationType(CategoryDeterminationType categoryDeterminationType) {
            this._categoryDeterminationType = categoryDeterminationType;
        }
        
        /// <summary>
        /// Mutator for the search attributes field. The parameter will determine if an attribute search is performed.
        /// </summary>
        /// <param name="searchAttributes">New value for search attribute.</param>
        public void SetSearchAttributes(bool searchAttributes) {
            _searchAttributes = searchAttributes;
        }

        /// <summary>
        /// Accessor for the retrieval type
        /// </summary>
        /// <returns>Retrieval type.</returns>
        public RetrievalType GetRetrievalType() {
            return _retrievalType;
        }

        /// <summary>
        /// Accessor for the document weighting scheme in tf-idf search
        /// </summary>
        /// <returns>Document weighting scheme in tf-idf search</returns>
        public DocumentWeighting GetDocumentWeighting() {
            return _documentWeighting;
        }

        /// <summary>
        /// Accessor for the term weighting scheme in tf-idf search
        /// </summary>
        /// <returns>Term weighting scheme in tf-idf search</returns>
        public TermWeighting GetTermWeighting() {
            return _termWeighting;
        }

        /// <summary>
        /// Accessor for the maximum number of documents retrieved.
        /// </summary>
        /// <returns>The maximum number of documents retrieved.</returns>
        public int GetDocumentsRetrieved() {
            return _documentsRetrieved;
        }

        /// <summary>
        /// Accessor for the focus type.
        /// </summary>
        /// <returns>Focus type.</returns>
        public FocusType GetFocusType() {
            return _focusType;
        }

        /// <summary>
        /// Accessor for the category determination type.
        /// </summary>
        /// <returns>Category determination type.</returns>
        public CategoryDeterminationType GetCategoryDeterminationType() {
            return _categoryDeterminationType;
        }
        
        /// <summary>
        /// Accessor for the search attributes field. The parameter will determine if an attribute search is performed.
        /// </summary>
        /// <returns>Search attribute.</returns>
        public bool GetSearchAttributes() {
            return _searchAttributes;
        }

    }
}