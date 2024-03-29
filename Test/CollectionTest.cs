using InformationRetrieval.Document;
using InformationRetrieval.Query;
using NUnit.Framework;

namespace Test
{
    public class CollectionTest
    {
        [Test]
        public void TestIncidenceMatrixSmall()
        {
            var parameter = new Parameter();
            parameter.SetIndexType(IndexType.INCIDENCE_MATRIX);
            var collection = new MemoryCollection("../../../testCollection2", parameter);
            Assert.AreEqual(2, collection.Size());
            Assert.AreEqual(26, collection.VocabularySize());
        }

        [Test]
        public void TestIncidenceMatrixQuery()
        {
            var parameter = new Parameter();
            parameter.SetIndexType(IndexType.INCIDENCE_MATRIX);
            var collection = new MemoryCollection("../../../testCollection2", parameter);
            var query = new Query("Brutus");
            var searchParameter = new SearchParameter();
            searchParameter.SetRetrievalType(RetrievalType.BOOLEAN);
            var result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(2, result.GetItems().Count);
            query = new Query("Brutus Caesar");
            result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(2, result.GetItems().Count);
            query = new Query("enact");
            result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(1, result.GetItems().Count);
            query = new Query("noble");
            result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(1, result.GetItems().Count);
            query = new Query("a");
            result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(0, result.GetItems().Count);
        }

        [Test]
        public void TestInvertedIndexBooleanQuery()
        {
            var parameter = new Parameter();
            parameter.SetNGramIndex(true);
            var collection = new MemoryCollection("../../../testCollection2", parameter);
            var query = new Query("Brutus");
            var searchParameter = new SearchParameter();
            searchParameter.SetRetrievalType(RetrievalType.BOOLEAN);
            var result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(2, result.GetItems().Count);
            query = new Query("Brutus Caesar");
            result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(2, result.GetItems().Count);
            query = new Query("enact");
            result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(1, result.GetItems().Count);
            query = new Query("noble");
            result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(1, result.GetItems().Count);
            query = new Query("a");
            result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(0, result.GetItems().Count);
        }

        [Test]
        public void TestPositionalIndexBooleanQuery()
        {
            var parameter = new Parameter();
            parameter.SetNGramIndex(true);
            var collection = new MemoryCollection("../../../testCollection2", parameter);
            var query = new Query("Julius Caesar");
            var searchParameter = new SearchParameter();
            searchParameter.SetRetrievalType(RetrievalType.POSITIONAL);
            var result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(2, result.GetItems().Count);
            query = new Query("I was killed");
            result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(1, result.GetItems().Count);
            query = new Query("The noble Brutus");
            result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(1, result.GetItems().Count);
            query = new Query("a");
            result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(0, result.GetItems().Count);
        }

        [Test]
        public void TestPositionalIndexRankedQuery()
        {
            var parameter = new Parameter();
            parameter.SetLoadIndexesFromFile(true);
            var collection = new MemoryCollection("../../../testCollection2", parameter);
            var query = new Query("Caesar");
            var searchParameter = new SearchParameter();
            searchParameter.SetRetrievalType(RetrievalType.RANKED);
            searchParameter.SetDocumentsRetrieved(2);
            var result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(2, result.GetItems().Count);
            Assert.AreEqual(1, result.GetItems()[0].GetDocId());
            query = new Query("Caesar was killed");
            result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(2, result.GetItems().Count);
            Assert.AreEqual(0, result.GetItems()[0].GetDocId());
            query = new Query("in the Capitol");
            result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(1, result.GetItems().Count);
            query = new Query("a");
            result = collection.SearchCollection(query, searchParameter);
            Assert.AreEqual(0, result.GetItems().Count);
        }
        
        [Test]
        public void TestLoadIndexesFromFileSmall()
        {
            var parameter = new Parameter();
            parameter.SetNGramIndex(true);
            parameter.SetLoadIndexesFromFile(true);
            var collection = new MemoryCollection("../../../testCollection2", parameter);
            Assert.AreEqual(2, collection.Size());
            Assert.AreEqual(26, collection.VocabularySize());
        }
        
        [Test]
        public void TestLimitNumberOfDocumentsSmall()
        {
            var parameter = new Parameter();
            parameter.SetNGramIndex(false);
            parameter.SetLimitNumberOfDocumentsLoaded(true);
            parameter.SetDocumentLimit(1);
            var collection = new MemoryCollection("../../../testCollection2", parameter);
            Assert.AreEqual(1, collection.Size());
            Assert.AreEqual(15, collection.VocabularySize());
        }
        
        [Test]
        public void TestCategoricalCollection()
        {
            var parameter = new Parameter();
            parameter.SetDocumentType(DocumentType.CATEGORICAL);
            parameter.SetLoadIndexesFromFile(true);
            parameter.SetPhraseIndex(false);
            parameter.SetNGramIndex(false);
            var collection = new MemoryCollection("../../../testCollection3", parameter);
            Assert.AreEqual(1000, collection.Size());
            Assert.AreEqual(2283, collection.VocabularySize());
        }
        
        [Test] 
        public void TestAttributeQuery() {
            var parameter = new Parameter();
            parameter.SetDocumentType(DocumentType.CATEGORICAL);
            parameter.SetLoadIndexesFromFile(true);
            var memoryCollection = new MemoryCollection("../../../testCollection3", parameter);
            var searchParameter = new SearchParameter();
            searchParameter.SetSearchAttributes(true);
            searchParameter.SetDocumentsRetrieved(400);
            searchParameter.SetRetrievalType(RetrievalType.RANKED);
            var query = new Query("Çift Yönlü");
            var result = memoryCollection.SearchCollection(query, searchParameter);
            Assert.AreEqual(10, result.GetItems().Count);
            query = new Query("Müzikli");
            result = memoryCollection.SearchCollection(query, searchParameter);
            Assert.AreEqual(4, result.GetItems().Count);
            query = new Query("Çift Yönlü Alüminyum Bebek Arabası");
            result = memoryCollection.SearchCollection(query, searchParameter);
            Assert.AreEqual(2, result.GetItems().Count);
        }

        [Test]
        public void TestCategoricalQuery() {
            var parameter = new Parameter();
            parameter.SetDocumentType(DocumentType.CATEGORICAL);
            parameter.SetLoadIndexesFromFile(true);
            var memoryCollection = new MemoryCollection("../../../testCollection3", parameter);
            var searchParameter = new SearchParameter();
            searchParameter.SetFocusType(FocusType.CATEGORY);
            searchParameter.SetRetrievalType(RetrievalType.BOOLEAN);
            var query = new Query("Çift Yönlü Bebek Arabası");
            var result = memoryCollection.SearchCollection(query, searchParameter);
            Assert.AreEqual(10, result.GetItems().Count);
            searchParameter.SetRetrievalType(RetrievalType.BOOLEAN);
            query = new Query("Terlik");
            result = memoryCollection.SearchCollection(query, searchParameter);
            Assert.AreEqual(5, result.GetItems().Count);
        }

        [Test]
        public void TestAutoCompleteWord() {
            var parameter = new Parameter();
            parameter.SetNGramIndex(true);
            parameter.SetLoadIndexesFromFile(true);
            var memoryCollection = new MemoryCollection("../../../testCollection2", parameter);
            var autoCompleteList = memoryCollection.AutoCompleteWord("kill");
            Assert.AreEqual(1, autoCompleteList.Count);
            autoCompleteList = memoryCollection.AutoCompleteWord("Ca");
            Assert.AreEqual(2, autoCompleteList.Count);
            memoryCollection = new MemoryCollection("../../../testCollection3", parameter);
            parameter.SetDocumentType(DocumentType.CATEGORICAL);
            autoCompleteList = memoryCollection.AutoCompleteWord("Yeni");
            Assert.AreEqual(6, autoCompleteList.Count);
            autoCompleteList = memoryCollection.AutoCompleteWord("Ka");
            Assert.AreEqual(68, autoCompleteList.Count);
            autoCompleteList = memoryCollection.AutoCompleteWord("Bebe");
            Assert.AreEqual(12, autoCompleteList.Count);
        }

    }
}