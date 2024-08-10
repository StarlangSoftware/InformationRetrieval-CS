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

        /// <summary>
        /// Constructor for the Document class. Sets the attributes.
        /// </summary>
        /// <param name="documentType">Type of the document. Can be normal for normal documents, categorical for categorical
        ///                     documents.</param>
        /// <param name="absoluteFileName">Absolute file name of the document</param>
        /// <param name="fileName">Relative file name of the document.</param>
        /// <param name="docId">Id of the document</param>
        public Document(DocumentType documentType, string absoluteFileName, string fileName, int docId)
        {
            _docId = docId;
            _absoluteFileName = absoluteFileName;
            _fileName = fileName;
            _documentType = documentType;
        }

        /// <summary>
        /// Loads the document from input stream. For normal documents, it reads as a corpus. For categorical documents, the
        /// first line contains categorical information, second line contains name of the product, third line contains
        /// detailed info about the product.
        /// </summary>
        /// <returns>Loaded document text.</returns>
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

        /// <summary>
        /// Loads the category of the document and adds it to the category tree. Category information is stored in the first
        /// line of the document.
        /// </summary>
        /// <param name="categoryTree">ategory tree to which new product will be added.</param>
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
        
        /// <summary>
        /// Accessor for the docId attribute.
        /// </summary>
        /// <returns>docId attribute.</returns>
        public int GetDocId()
        {
            return _docId;
        }

        /// <summary>
        /// Accessor for the fileName attribute.
        /// </summary>
        /// <returns>fileName attribute.</returns>
        public string GetFileName()
        {
            return _fileName;
        }

        /// <summary>
        /// Accessor for the absoluteFileName attribute.
        /// </summary>
        /// <returns>absoluteFileName attribute.</returns>
        public string GetAbsoluteFileName()
        {
            return _absoluteFileName;
        }

        /// <summary>
        /// Accessor for the size attribute.
        /// </summary>
        /// <returns>size attribute.</returns>
        public int GetSize()
        {
            return _size;
        }

        /// <summary>
        /// Mutator for the size attribute.
        /// </summary>
        /// <param name="size">New size attribute.</param>
        public void SetSize(int size)
        {
            _size = size;
        }
        
        /// <summary>
        /// Mutator for the category attribute.
        /// </summary>
        /// <param name="categoryTree">Category tree to which new category will be added.</param>
        /// <param name="category">New category that will be added</param>
        public void SetCategory(CategoryTree categoryTree, string category){
            _category = categoryTree.AddCategoryHierarchy(category);
        }

        /// <summary>
        /// Accessor for the category attribute.
        /// </summary>
        /// <returns>Category attribute as a String</returns>
        public string GetCategory(){
            return _category.ToString();
        }

        /// <summary>
        /// Accessor for the category attribute.
        /// </summary>
        /// <returns>Category attribute as a CategoryNode.</returns>
        public CategoryNode GetCategoryNode()
        {
            return _category;
        }

    }
}