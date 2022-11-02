using System.Collections.Generic;

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

        public string TopNString(TermDictionary dictionary, int n)
        {
            var queue = new Queue<CategoryNode>();
            queue.Enqueue(_root);
            var result = "";
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node != _root)
                {
                    result += node.TopNString(dictionary, n) + "\n";
                }

                foreach (var child in node.GetChildren())
                {
                    queue.Enqueue(child);
                }
            }

            return result;
        }
    }
}