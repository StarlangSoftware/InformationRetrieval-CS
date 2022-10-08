using InformationRetrieval.Document;
using InformationRetrieval.Index;
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
            var collection = new Collection("../../../testCollection2", parameter);
            Assert.AreEqual(2, collection.Size());
            Assert.AreEqual(26, collection.VocabularySize());
        }

        [Test]
        public void TestIncidenceMatrixQuery()
        {
            var parameter = new Parameter();
            parameter.SetIndexType(IndexType.INCIDENCE_MATRIX);
            var collection = new Collection("../../../testCollection2", parameter);
            var query = new Query("Brutus");
            var result = collection.SearchCollection(query, RetrievalType.BOOLEAN, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(2, result.GetItems().Count);
            query = new Query("Brutus Caesar");
            result = collection.SearchCollection(query, RetrievalType.BOOLEAN, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(2, result.GetItems().Count);
            query = new Query("enact");
            result = collection.SearchCollection(query, RetrievalType.BOOLEAN, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(1, result.GetItems().Count);
            query = new Query("noble");
            result = collection.SearchCollection(query, RetrievalType.BOOLEAN, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(1, result.GetItems().Count);
            query = new Query("a");
            result = collection.SearchCollection(query, RetrievalType.BOOLEAN, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(0, result.GetItems().Count);
        }

        [Test]
        public void TestInvertedIndexBooleanQuery()
        {
            var parameter = new Parameter();
            parameter.SetNGramIndex(true);
            var collection = new Collection("../../../testCollection2", parameter);
            var query = new Query("Brutus");
            var result = collection.SearchCollection(query, RetrievalType.BOOLEAN, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(2, result.GetItems().Count);
            query = new Query("Brutus Caesar");
            result = collection.SearchCollection(query, RetrievalType.BOOLEAN, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(2, result.GetItems().Count);
            query = new Query("enact");
            result = collection.SearchCollection(query, RetrievalType.BOOLEAN, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(1, result.GetItems().Count);
            query = new Query("noble");
            result = collection.SearchCollection(query, RetrievalType.BOOLEAN, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(1, result.GetItems().Count);
            query = new Query("a");
            result = collection.SearchCollection(query, RetrievalType.BOOLEAN, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(0, result.GetItems().Count);
        }

        [Test]
        public void TestPositionalIndexBooleanQuery()
        {
            var parameter = new Parameter();
            parameter.SetNGramIndex(true);
            var collection = new Collection("../../../testCollection2", parameter);
            var query = new Query("Julius Caesar");
            var result = collection.SearchCollection(query, RetrievalType.POSITIONAL, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(2, result.GetItems().Count);
            query = new Query("I was killed");
            result = collection.SearchCollection(query, RetrievalType.POSITIONAL, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(1, result.GetItems().Count);
            query = new Query("The noble Brutus");
            result = collection.SearchCollection(query, RetrievalType.POSITIONAL, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(1, result.GetItems().Count);
            query = new Query("a");
            result = collection.SearchCollection(query, RetrievalType.POSITIONAL, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(0, result.GetItems().Count);
        }

        [Test]
        public void TestPositionalIndexRankedQuery()
        {
            var parameter = new Parameter();
            parameter.SetLoadIndexesFromFile(true);
            var collection = new Collection("../../../testCollection2", parameter);
            var query = new Query("Caesar");
            var result = collection.SearchCollection(query, RetrievalType.RANKED, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(2, result.GetItems().Count);
            Assert.AreEqual(1, result.GetItems()[0].GetDocId());
            query = new Query("Caesar was killed");
            result = collection.SearchCollection(query, RetrievalType.RANKED, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(2, result.GetItems().Count);
            Assert.AreEqual(0, result.GetItems()[0].GetDocId());
            query = new Query("in the Capitol");
            result = collection.SearchCollection(query, RetrievalType.RANKED, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(1, result.GetItems().Count);
            query = new Query("a");
            result = collection.SearchCollection(query, RetrievalType.RANKED, TermWeighting.NATURAL, DocumentWeighting.NO_IDF);
            Assert.AreEqual(0, result.GetItems().Count);
        }

        [Test]
        public void TestSaveIndexesToFileSmall()
        {
            var parameter = new Parameter();
            parameter.SetNGramIndex(true);
            var collection = new Collection("../../../testCollection2", parameter);
            collection.Save();
        }

        [Test]
        public void TestLoadIndexesFromFileSmall()
        {
            var parameter = new Parameter();
            parameter.SetNGramIndex(true);
            parameter.SetLoadIndexesFromFile(true);
            var collection = new Collection("../../../testCollection2", parameter);
            Assert.AreEqual(2, collection.Size());
            Assert.AreEqual(26, collection.VocabularySize());
        }

        [Test]
        public void TestConstructIndexesInDiskSmall()
        {
            var parameter = new Parameter();
            parameter.SetConstructIndexInDisk(true);
            parameter.SetNGramIndex(false);
            parameter.SetDocumentLimit(1);
            var collection = new Collection("../../../testCollection2", parameter);
        }

        [Test]
        public void TestLimitNumberOfDocumentsSmall()
        {
            var parameter = new Parameter();
            parameter.SetNGramIndex(false);
            parameter.SetLimitNumberOfDocumentsLoaded(true);
            parameter.SetDocumentLimit(1);
            var collection = new Collection("../../../testCollection2", parameter);
            Assert.AreEqual(1, collection.Size());
            Assert.AreEqual(15, collection.VocabularySize());
        }

        [Test]
        public void TestConstructDictionaryAndIndexesInDiskSmall()
        {
            var parameter = new Parameter();
            parameter.SetConstructDictionaryInDisk(true);
            parameter.SetDocumentLimit(1);
            parameter.SetWordLimit(10);
            var collection = new Collection("../../../testCollection2", parameter);
        }

    }
}