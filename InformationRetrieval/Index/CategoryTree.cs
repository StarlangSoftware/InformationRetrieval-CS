
using System.Collections.Generic;
using InformationRetrieval.Query;

namespace InformationRetrieval.Index
{
    public class CategoryTree
    {
        private CategoryNode _root;

        public CategoryTree(string rootName)
        {
            _root = new CategoryNode(rootName, null);
        }

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

        public void SetRepresentativeCount(int representativeCount) {
            _root.SetRepresentativeCount(representativeCount);
        }

    }
}