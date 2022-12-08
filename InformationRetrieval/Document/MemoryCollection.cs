using System.Collections.Generic;
using System.IO;
using InformationRetrieval.Index;
using InformationRetrieval.Query;

namespace InformationRetrieval.Document
{
    public class MemoryCollection : AbstractCollection
    {
        private IndexType indexType;

        public MemoryCollection(string directory, Parameter parameter) : base(directory, parameter)
        {
            indexType = parameter.GetIndexType();

            if (parameter.LoadIndexesFromFile())
            {
                LoadIndexesFromFile(directory);
            }
            else
            {
                ConstructIndexesInMemory();
            }

            if (parameter.GetDocumentType() == DocumentType.CATEGORICAL)
            {
                PositionalIndex.SetCategoryCounts(Documents);
                CategoryTree.SetRepresentativeCount(parameter.GetRepresentativeCount());
            }
        }

        protected void LoadIndexesFromFile(string directory)
        {
            Dictionary = new TermDictionary(Comparator, directory);
            InvertedIndex = new InvertedIndex(directory);
            if (Parameter.ConstructPositionalIndex())
            {
                PositionalIndex = new PositionalIndex(directory);
                PositionalIndex.SetDocumentSizes(Documents);
            }

            if (Parameter.ConstructPhraseIndex())
            {
                PhraseDictionary = new TermDictionary(Comparator, directory + "-phrase");
                PhraseIndex = new InvertedIndex(directory + "-phrase");
                if (Parameter.ConstructPositionalIndex())
                {
                    PhrasePositionalIndex = new PositionalIndex(directory + "-phrase");
                }
            }

            if (Parameter.ConstructNGramIndex())
            {
                BiGramDictionary = new TermDictionary(Comparator, directory + "-biGram");
                TriGramDictionary = new TermDictionary(Comparator, directory + "-triGram");
                BiGramIndex = new NGramIndex(directory + "-biGram");
                TriGramIndex = new NGramIndex(directory + "-triGram");
            }
        }

        public void Save()
        {
            if (indexType == IndexType.INVERTED_INDEX)
            {
                Dictionary.Save(Name);
                InvertedIndex.Save(Name);
                if (Parameter.ConstructPositionalIndex())
                {
                    PositionalIndex.Save(Name);
                }

                if (Parameter.ConstructPhraseIndex())
                {
                    PhraseDictionary.Save(Name + "-phrase");
                    PhraseIndex.Save(Name + "-phrase");
                    if (Parameter.ConstructPositionalIndex())
                    {
                        PhrasePositionalIndex.Save(Name + "-phrase");
                    }
                }

                if (Parameter.ConstructNGramIndex())
                {
                    BiGramDictionary.Save(Name + "-biGram");
                    TriGramDictionary.Save(Name + "-triGram");
                    BiGramIndex.Save(Name + "-biGram");
                    TriGramIndex.Save(Name + "-triGram");
                }
            }
            if (Parameter.GetDocumentType() == DocumentType.CATEGORICAL){
                SaveCategories();
            }
        }

        private void SaveCategories()
        {
            var printWriter = new StreamWriter(Name + "-categories.txt");
            foreach (var document in Documents){
                printWriter.Write(document.GetDocId() + "\t" + document.GetCategory() + "\n");
            }
            printWriter.Close();
        }
        
        private void ConstructIndexesInMemory()
        {
            var terms = ConstructTerms(TermType.TOKEN);
            Dictionary = new TermDictionary(Comparator, terms);
            switch (indexType)
            {
                case IndexType.INCIDENCE_MATRIX:
                    IncidenceMatrix = new IncidenceMatrix(terms, Dictionary, Documents.Count);
                    break;
                case IndexType.INVERTED_INDEX:
                    InvertedIndex = new InvertedIndex(Dictionary, terms, Comparator);
                    if (Parameter.ConstructPositionalIndex())
                    {
                        PositionalIndex = new PositionalIndex(Dictionary, terms, Comparator);
                    }

                    if (Parameter.ConstructPhraseIndex())
                    {
                        terms = ConstructTerms(TermType.PHRASE);
                        PhraseDictionary = new TermDictionary(Comparator, terms);
                        PhraseIndex = new InvertedIndex(PhraseDictionary, terms, Comparator);
                        if (Parameter.ConstructPositionalIndex())
                        {
                            PhrasePositionalIndex = new PositionalIndex(PhraseDictionary, terms, Comparator);
                        }
                    }

                    if (Parameter.ConstructNGramIndex())
                    {
                        ConstructNGramIndex();
                    }

                    if (Parameter.GetDocumentType() == DocumentType.CATEGORICAL){
                        CategoryTree = new CategoryTree(Name);
                        foreach (var document in Documents){
                            document.LoadCategory(CategoryTree);
                        }
                    }
                    break;
            }
        }

        private List<TermOccurrence> ConstructTerms(TermType termType)
        {
            var termComparator = new TermOccurrenceComparator(Comparator);
            var terms = new List<TermOccurrence>();
            List<TermOccurrence> docTerms;
            foreach (var doc in Documents)
            {
                var documentText = doc.LoadDocument();
                docTerms = documentText.ConstructTermList(doc.GetDocId(), termType);
                terms.AddRange(docTerms);
            }

            terms.Sort(termComparator);
            return terms;
        }
        
        private QueryResult AttributeSearch(Query.Query query){
            var termAttributes = new Query.Query();
            var phraseAttributes = new Query.Query();
            QueryResult termResult = new QueryResult(), phraseResult = new QueryResult();
            query.FilterAttributes(AttributeList, termAttributes, phraseAttributes);
            if (termAttributes.Size() > 0){
                termResult = InvertedIndex.Search(termAttributes, Dictionary);
            }
            if (phraseAttributes.Size() > 0){
                phraseResult = PhraseIndex.Search(phraseAttributes, PhraseDictionary);
            }
            if (termAttributes.Size() == 0){
                return phraseResult;
            }
            if (phraseAttributes.Size() == 0){
                return termResult;
            }
            return termResult.Intersection(phraseResult);
        }

        private QueryResult SearchWithInvertedIndex(Query.Query query, SearchParameter parameter)
        {
            switch (parameter.GetRetrievalType())
            {
                case RetrievalType.BOOLEAN: 
                    return InvertedIndex.Search(query, Dictionary);
                case RetrievalType.POSITIONAL: 
                    return PositionalIndex.PositionalSearch(query, Dictionary);
                case RetrievalType.RANKED:
                    return PositionalIndex.RankedSearch(query, Dictionary, Documents, parameter.GetTermWeighting(),
                        parameter.GetDocumentWeighting(), parameter.GetDocumentsRetrieved());
                case RetrievalType.ATTRIBUTE:
                    return AttributeSearch(query);
            }

            return new QueryResult();
        }
        
        private QueryResult FilterAccordingToCategories(QueryResult currentResult, List<CategoryNode> categories){
            var filteredResult = new QueryResult();
            var items = currentResult.GetItems();
            foreach (var queryResultItem in items) {
                var categoryNode = Documents[queryResultItem.GetDocId()].GetCategoryNode();
                foreach (var possibleAncestor in categories){
                    if (categoryNode.IsDescendant(possibleAncestor)) {
                        filteredResult.Add(queryResultItem.GetDocId(), queryResultItem.GetScore());
                        break;
                    }
                }
            }
            return filteredResult;
        }

        public QueryResult SearchCollection(Query.Query query, SearchParameter searchParameter)
        {
            if (searchParameter.GetFocusType().Equals(FocusType.CATEGORY)){
                var currentResult = SearchWithInvertedIndex(query, searchParameter);
                var categories = CategoryTree.GetCategories(query, Dictionary, searchParameter.GetCategoryDeterminationType());
                return FilterAccordingToCategories(currentResult, categories);
            }
            else
            {
                switch (indexType)
                {
                    case IndexType.INCIDENCE_MATRIX:
                        return IncidenceMatrix.Search(query, Dictionary);
                    case IndexType.INVERTED_INDEX:
                        return SearchWithInvertedIndex(query, searchParameter);
                }
   
            }
            return new QueryResult();
        }
    }
}