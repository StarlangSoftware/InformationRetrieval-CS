
using System.Collections.Generic;
using InformationRetrieval.Query;

namespace InformationRetrieval.Index
{
    public class CategoryTree
    {
        private CategoryNode _root;

        /// <summary>
        /// Simple constructor of the tree. Sets the root node of the tree.
        /// </summary>
        /// <param name="rootName">Category name of the root node.</param>
        public CategoryTree(string rootName)
        {
            _root = new CategoryNode(rootName, null);
        }

        /// <summary>
        /// Adds a path (and if required nodes in the path) to the category tree according to the hierarchy string. Hierarchy
        /// string is obtained by concatenating the names of all nodes in the path from root node to a leaf node separated
        /// with '%'.
        /// </summary>
        /// <param name="hierarchy">Hierarchy string</param>
        /// <returns>The leaf node added when the hierarchy string is processed.</returns>
        public CategoryNode AddCategoryHierarchy(string hierarchy)
        {
            var categories = hierarchy.Split("%");
            var current = _root;
            foreach (var category in categories)
            {
                var node = current.GetChild(category);
                if (node == null)
                {
                    node = new CategoryNode(category, current);
                }

                current = node;
            }

            return current;
        }
        
        /// <summary>
        /// The method checks the query words in the category words of all nodes in the tree and returns the nodes that
        /// satisfies the condition. If any word in the query appears in any category word, the node will be returned.
        /// </summary>
        /// <param name="query">Query string</param>
        /// <param name="dictionary">Term dictionary</param>
        /// <param name="categoryDeterminationType">Category determination type</param>
        /// <returns>The category nodes whose names contain at least one word from the query string</returns>
        public List<CategoryNode> GetCategories(Query.Query query, TermDictionary dictionary, CategoryDeterminationType categoryDeterminationType){
            var result = new List<CategoryNode>();
            switch (categoryDeterminationType){
                default:
                case CategoryDeterminationType.KEYWORD:
                    _root.GetCategoriesWithKeyword(query, result);
                    break;
                case CategoryDeterminationType.COSINE:
                    _root.GetCategoriesWithCosine(query, dictionary, result);
                    break;
            }
            return result;
        }

        /// <summary>
        /// The method sets the representative count. The representative count filters the most N frequent words.
        /// </summary>
        /// <param name="representativeCount">Number of representatives.</param>
        public void SetRepresentativeCount(int representativeCount) {
            _root.SetRepresentativeCount(representativeCount);
        }

    }
}