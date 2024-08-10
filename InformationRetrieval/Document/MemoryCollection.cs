using System.Collections.Generic;
using System.IO;
using InformationRetrieval.Index;
using InformationRetrieval.Query;

namespace InformationRetrieval.Document
{
    public class MemoryCollection : AbstractCollection
    {
        private IndexType indexType;

        /// <summary>
        /// Constructor for the MemoryCollection class. In small collections, dictionary and indexes are kept in memory.
        /// Memory collection also supports categorical documents.
        /// </summary>
        /// <param name="directory">Directory where the document collection resides.</param>
        /// <param name="parameter">Search parameter</param>
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

        /// <summary>
        /// The method loads the term dictionary, inverted index, positional index, phrase and N-Gram indexes from dictionary
        /// and index files to the memory.
        /// </summary>
        /// <param name="directory">Directory where the document collection resides.</param>
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

        /// <summary>
        /// The method saves the term dictionary, inverted index, positional index, phrase and N-Gram indexes to the dictionary
        /// and index files. If the collection is a categorical collection, categories are also saved to the category
        /// files.
        /// </summary>
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

        /// <summary>
        /// The method saves the category tree for the categorical collections.
        /// </summary>
        private void SaveCategories()
        {
            var printWriter = new StreamWriter(Name + "-categories.txt");
            foreach (var document in Documents){
                printWriter.Write(document.GetDocId() + "\t" + document.GetCategory() + "\n");
            }
            printWriter.Close();
        }
        
        /// <summary>
        /// The method constructs the term dictionary, inverted index, positional index, phrase and N-Gram indexes in memory.
        /// </summary>
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

        /// <summary>
        /// Given the document collection, creates an array list of terms. If term type is TOKEN, the terms are single
        /// word, if the term type is PHRASE, the terms are bi-words. Each document is loaded into memory and
        /// word list is created. Since the dictionary can be kept in memory, all operations can be done in memory.
        /// </summary>
        /// <param name="termType">If term type is TOKEN, the terms are single word, if the term type is PHRASE, the terms are
        /// bi-words.</param>
        /// <returns>Array list of terms occurring in the document collection.</returns>
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
        
        /// <summary>
        /// The method searches given query string in the document collection using the attribute list according to the
        /// given search parameter. First, the original query is filtered by removing phrase attributes, shortcuts and single
        /// word attributes. At this stage, we get the word and phrase attributes in the original query and the remaining
        /// words in the original query as two separate queries. Second, both single word and phrase attributes in the
        /// original query are searched in the document collection. Third, these intermediate query results are then
        /// intersected. Fourth, we put this results into either (i) an inverted index (ii) or a ranked based positional
        /// filtering with the filtered query to get the end result.
        /// </summary>
        /// <param name="query">Query string</param>
        /// <param name="parameter">Search parameter for the query</param>
        /// <returns>The intermediate result of the query obtained by doing attribute list based search in the collection.</returns>
        private QueryResult AttributeSearch(Query.Query query, SearchParameter parameter){
            var termAttributes = new Query.Query();
            var phraseAttributes = new Query.Query();
            QueryResult termResult = new QueryResult(), phraseResult = new QueryResult();
            QueryResult attributeResult, filteredResult;
            var filteredQuery = query.FilterAttributes(AttributeList, termAttributes, phraseAttributes);
            if (termAttributes.Size() > 0){
                termResult = InvertedIndex.Search(termAttributes, Dictionary);
            }
            if (phraseAttributes.Size() > 0){
                phraseResult = PhraseIndex.Search(phraseAttributes, PhraseDictionary);
            }
            if (termAttributes.Size() == 0){
                attributeResult = phraseResult;
            }
            else
            {
                if (phraseAttributes.Size() == 0){
                    attributeResult = termResult;
                }
                else
                {
                    attributeResult = termResult.IntersectionFastSearch(phraseResult);
                }
            }
            if (filteredQuery.Size() == 0){
                return attributeResult;
            } else {
                if (parameter.GetRetrievalType() != RetrievalType.RANKED){
                    filteredResult = SearchWithInvertedIndex(filteredQuery, parameter);
                    return filteredResult.IntersectionFastSearch(attributeResult);
                } else {
                    filteredResult = PositionalIndex.RankedSearch(filteredQuery,
                        Dictionary,
                        Documents,
                        parameter);
                    if (attributeResult.Size() < 10){
                        filteredResult = filteredResult.IntersectionLinearSearch(attributeResult);
                    } else {
                        filteredResult = filteredResult.IntersectionBinarySearch(attributeResult);
                    }
                    filteredResult.GetBest(parameter.GetDocumentsRetrieved());
                    return filteredResult;
                }
            }
        }

        /// <summary>
        /// The method searches given query string in the document collection using the inverted index according to the
        /// given search parameter. If the search is (i) boolean, inverted index is used (ii) positional, positional
        /// inverted index is used, (iii) ranked, positional inverted index is used with a ranking algorithm at the end.
        /// </summary>
        /// <param name="query">Query string</param>
        /// <param name="parameter">Search parameter for the query</param>
        /// <returns>The intermediate result of the query obtained by doing inverted index based search in the collection.</returns>
        private QueryResult SearchWithInvertedIndex(Query.Query query, SearchParameter parameter)
        {
            switch (parameter.GetRetrievalType())
            {
                case RetrievalType.BOOLEAN: 
                    return InvertedIndex.Search(query, Dictionary);
                case RetrievalType.POSITIONAL: 
                    return PositionalIndex.PositionalSearch(query, Dictionary);
                case RetrievalType.RANKED:
                    var result = PositionalIndex.RankedSearch(query, Dictionary, Documents, parameter);
                    result.GetBest(parameter.GetDocumentsRetrieved());
                    return result;
            }

            return new QueryResult();
        }
        
        /// <summary>
        /// Filters current search result according to the predicted categories from the query string. For every search
        /// result, if it is in one of the predicated categories, is added to the filtered end result. Otherwise, it is
        /// omitted in the end result.
        /// </summary>
        /// <param name="currentResult">Current search result before filtering.</param>
        /// <param name="categories">Predicted categories that match the query string.</param>
        /// <returns>Filtered query result</returns>
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
        
        /// <summary>
        /// Constructs an auto complete list of product names for a given prefix. THe results are sorted according to
        /// frequencies.
        /// </summary>
        /// <param name="prefix">Prefix of the name of the product.</param>
        /// <returns>An auto complete list of product names for a given prefix.</returns>
        public List<string> AutoCompleteWord(string prefix){
            var result = new List<string>();
            var i = Dictionary.GetWordStartingWith(prefix);
            while (i < Dictionary.Size()){
                if (Dictionary.GetWord(i).GetName().StartsWith(prefix)){
                    result.Add(Dictionary.GetWord(i).GetName());
                } else {
                    break;
                }
                i++;
            }
            InvertedIndex.AutoCompleteWord(result, Dictionary);
            return result;
        }

        /// <summary>
        /// Searches a document collection for a given query according to the given search parameters. The documents are
        /// searched using (i) incidence matrix if the index type is incidence matrix, (ii) attribute list if search
        /// attributes option is selected, (iii) inverted index if the index type is inverted index and no attribute
        /// search is done. After the initial search, if there is a categorical focus, it filters the result
        /// according to the predicted categories from the query string.
        /// </summary>
        /// <param name="query">Query string</param>
        /// <param name="searchParameter">Search parameter for the query</param>
        /// <returns>The result of the query obtained by doing search in the collection.</returns>
        public QueryResult SearchCollection(Query.Query query, SearchParameter searchParameter)
        {
            QueryResult currentResult;
            if (searchParameter.GetFocusType().Equals(FocusType.CATEGORY)){
                if (searchParameter.GetSearchAttributes()){
                    currentResult = AttributeSearch(query, searchParameter);
                } else {
                    currentResult = SearchWithInvertedIndex(query, searchParameter);
                }
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
                        if (searchParameter.GetSearchAttributes()){
                            return AttributeSearch(query, searchParameter);
                        } else {
                            return SearchWithInvertedIndex(query, searchParameter);
                        }
                }
   
            }
            return new QueryResult();
        }
    }
}