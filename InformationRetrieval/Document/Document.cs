using Corpus;

namespace InformationRetrieval.Document
{
    public class Document
    {
        private string _absoluteFileName;
        private string _fileName;
        private int _docId;
        private int _size = 0;
        private DocumentType _documentType;
        private CategoryHierarchy _categoryHierarchy;

        public Document(DocumentType documentType, string absoluteFileName, string fileName, int docId)
        {
            _docId = docId;
            _absoluteFileName = absoluteFileName;
            _fileName = fileName;
            _documentType = documentType;
        }

        public DocumentText LoadDocument()
        {
            DocumentText documentText;
            switch (_documentType)
            {
                case DocumentType.NORMAL:
                    default:
                    documentText = new DocumentText(_absoluteFileName, new TurkishSplitter());
                    _size = documentText.NumberOfWords();
                    break;
                case DocumentType.CATEGORICAL:
                    var corpus = new Corpus.Corpus(_absoluteFileName);
                    if (corpus.SentenceCount() >= 2){
                        _categoryHierarchy = new CategoryHierarchy(corpus.GetSentence(0).ToString());
                        documentText = new DocumentText();
                        var sentences = new TurkishSplitter().Split(corpus.GetSentence(1).ToString());
                        foreach (var sentence in sentences){
                            documentText.AddSentence(sentence);
                        }
                        _size = documentText.NumberOfWords();
                    } else {
                        return null;
                    }
                    break;
            }
            return documentText;
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

        public void SetSize(int size)
        {
            _size = size;
        }
        
        public void SetCategoryHierarchy(string categoryHierarchy){
            _categoryHierarchy = new CategoryHierarchy(categoryHierarchy);
        }

        public CategoryHierarchy GetCategoryHierarchy(){
            return _categoryHierarchy;
        }

    }
}