using System;
using System.Collections.Generic;
using System.IO;
using Dictionary.Dictionary;
using InformationRetrieval.Index;

namespace InformationRetrieval.Document
{
    public class AbstractCollection
    {
        protected TermDictionary Dictionary;
        protected TermDictionary PhraseDictionary;
        protected TermDictionary BiGramDictionary;
        protected TermDictionary TriGramDictionary;
        protected List<Document> Documents;
        protected IncidenceMatrix IncidenceMatrix;
        protected InvertedIndex InvertedIndex;
        protected NGramIndex BiGramIndex;
        protected NGramIndex TriGramIndex;
        protected PositionalIndex PositionalIndex;
        protected InvertedIndex PhraseIndex;
        protected PositionalIndex PhrasePositionalIndex;
        protected IComparer<Word> Comparator;
        protected string Name;
        protected Parameter Parameter;
        protected CategoryTree CategoryTree;
        protected HashSet<string> AttributeList;

        /// <summary>
        /// Constructor for the AbstractCollection class. All collections, disk, memory, large, medium are extended from this
        /// basic class. Loads the attribute list from attribute file if required. Loads the names of the documents from
        /// the document collection. If the collection is a categorical collection, also loads the category tree.
        /// </summary>
        /// <param name="directory">Directory where the document collection resides.</param>
        /// <param name="parameter">Search parameter</param>
        public AbstractCollection(string directory, Parameter parameter)
        {
            Name = directory;
            Comparator = parameter.GetWordComparator();
            Parameter = parameter;
            if (parameter.GetDocumentType() == DocumentType.CATEGORICAL)
            {
                LoadAttributeList();
            }
            Documents = new List<Document>();
            var listOfFiles = Directory.GetFiles(directory);
            if (listOfFiles != null)
            {
                Array.Sort(listOfFiles);
                var fileLimit = listOfFiles.Length;
                if (parameter.LimitNumberOfDocumentsLoaded())
                {
                    fileLimit = parameter.GetDocumentLimit();
                }

                var i = 0;
                var j = 0;
                while (i < listOfFiles.Length && j < fileLimit)
                {
                    var file = listOfFiles[i];
                    if (file.EndsWith(".txt"))
                    {
                        var document = new Document(parameter.GetDocumentType(), file, file, j);
                        Documents.Add(document);
                        j++;
                    }

                    i++;
                }
            }
            if (parameter.GetDocumentType() == DocumentType.CATEGORICAL) {
                LoadCategories();
            }
        }
        
        /**
         * Loads the category tree for the categorical collections from category index file. Each line of the category index
         * file stores the index of the category and the category name with its hierarchy. Hierarchy string is obtained by
         * concatenating the names of all nodes in the path from root node to a leaf node separated with '%'.
         */
        private void LoadCategories()
        {
            CategoryTree = new CategoryTree(Name);
            var streamReader = new StreamReader(Name + "-categories.txt");
            var line = streamReader.ReadLine();
            while (line != null){
                var items = line.Split("\t");
                if (items.Length > 0)
                {
                    var docId = int.Parse(items[0]);
                    if (items.Length > 1)
                    {
                        Documents[docId].SetCategory(CategoryTree, items[1]);
                    }
                }
                line = streamReader.ReadLine();
            }
            streamReader.Close();
        }

        /**
         * Loads the attribute list from attribute index file. Attributes are single or bi-word phrases representing the
         * important features of products in the collection. Each line of the attribute file contains either single or a two
         * word expression.
         */
        private void LoadAttributeList()
        {
            AttributeList = new HashSet<string>();
            var streamReader = new StreamReader(Name + "-attributelist.txt");
            var line = streamReader.ReadLine();
            while (line != null)
            {
                AttributeList.Add(line);
                line = streamReader.ReadLine();
            }
            streamReader.Close();
        }
        
        /// <summary>
        /// Returns size of the document collection.
        /// </summary>
        /// <returns>Size of the document collection.</returns>
        public int Size()
        {
            return Documents.Count;
        }

        /// <summary>
        /// Returns size of the term dictionary.
        /// </summary>
        /// <returns>Size of the term dictionary.</returns>
        public int VocabularySize()
        {
            return Dictionary.Size();
        }

        /**
         * Constructs bi-gram and tri-gram indexes in memory.
         */
        protected void ConstructNGramIndex()
        {
            var terms = Dictionary.ConstructTermsFromDictionary(2);
            BiGramDictionary = new TermDictionary(Comparator, terms);
            BiGramIndex = new NGramIndex(BiGramDictionary, terms, Comparator);
            terms = Dictionary.ConstructTermsFromDictionary(3);
            TriGramDictionary = new TermDictionary(Comparator, terms);
            TriGramIndex = new NGramIndex(TriGramDictionary, terms, Comparator);
        }

    }
}