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
        
        private void LoadCategories()
        {
            CategoryTree = new CategoryTree(Name);
            var streamReader = new StreamReader(Name + "-categories.txt");
            var line = streamReader.ReadLine();
            while (line != null){
                var items = line.Split("\t");
                var docId = int.Parse(items[0]);
                Documents[docId].SetCategory(CategoryTree, items[1]);
                line = streamReader.ReadLine();
            }
            streamReader.Close();
        }

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

        public int Size()
        {
            return Documents.Count;
        }

        public int VocabularySize()
        {
            return Dictionary.Size();
        }

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