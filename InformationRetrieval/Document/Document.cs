using Corpus;
using InformationRetrieval.Index;

namespace InformationRetrieval.Document
{
    public class Document
    {
        private string _absoluteFileName;
        private string _fileName;
        private int _docId;
        private int _size;
        private DocumentType _documentType;
        private CategoryNode _category;

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

        public void LoadCategory(CategoryTree categoryTree)
        {
            if (_documentType == DocumentType.CATEGORICAL)
            {
                var corpus = new Corpus.Corpus(_absoluteFileName);
                if (corpus.SentenceCount() >= 2)
                {
                    _category = categoryTree.AddCategoryHierarchy(corpus.GetSentence(0).ToString());
                }
            }
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
        
        public void SetCategory(CategoryTree categoryTree, string category){
            _category = categoryTree.AddCategoryHierarchy(category);
        }

        public string GetCategory(){
            return _category.ToString();
        }

        public CategoryNode GetCategoryNode()
        {
            return _category;
        }

    }
}