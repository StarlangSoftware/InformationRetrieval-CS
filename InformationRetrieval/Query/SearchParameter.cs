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

        public SearchParameter(){
        }

        public void SetRetrievalType(RetrievalType retrievalType) {
            _retrievalType = retrievalType;
        }

        public void SetDocumentWeighting(DocumentWeighting documentWeighting) {
            _documentWeighting = documentWeighting;
        }

        public void SetTermWeighting(TermWeighting termWeighting) {
            _termWeighting = termWeighting;
        }

        public void SetDocumentsRetrieved(int documentsRetrieved) {
            _documentsRetrieved = documentsRetrieved;
        }

        public void SetFocusType(FocusType focusType){
            this._focusType = focusType;
        }

        public void SetCategoryDeterminationType(CategoryDeterminationType categoryDeterminationType) {
            this._categoryDeterminationType = categoryDeterminationType;
        }
        
        public void SetSearchAttributes(bool searchAttributes) {
            _searchAttributes = searchAttributes;
        }

        public RetrievalType GetRetrievalType() {
            return _retrievalType;
        }

        public DocumentWeighting GetDocumentWeighting() {
            return _documentWeighting;
        }

        public TermWeighting GetTermWeighting() {
            return _termWeighting;
        }

        public int GetDocumentsRetrieved() {
            return _documentsRetrieved;
        }

        public FocusType GetFocusType() {
            return _focusType;
        }

        public CategoryDeterminationType GetCategoryDeterminationType() {
            return _categoryDeterminationType;
        }
        
        public bool GetSearchAttributes() {
            return _searchAttributes;
        }

    }
}