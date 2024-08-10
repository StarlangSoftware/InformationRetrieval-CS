using System.Collections.Generic;
using DataStructure;

namespace InformationRetrieval.Index
{
    public class CategoryNode
    {
        private List<CategoryNode> _children = new List<CategoryNode>();
        private CategoryNode _parent;
        private CounterHashMap<int> _counts = new CounterHashMap<int>();
        private List<string> _categoryWords;

        /// <summary>
        /// Constructor for the category node. Each category is represented as a tree node in the category tree. Category
        /// words are constructed by splitting the name of the category w.r.t. space. Sets the parent node and adds this
        /// node as a child to parent node.
        /// </summary>
        /// <param name="name">Name of the category.</param>
        /// <param name="parent">Parent node of this node.</param>
        public CategoryNode(string name, CategoryNode parent)
        {
            _categoryWords = new List<string>(name.Split(" "));
            _parent = parent;
            if (parent != null)
            {
                parent.AddChild(this);
            }
        }

        /// <summary>
        /// Adds the given child node to this node.
        /// </summary>
        /// <param name="child">New child node</param>
        private void AddChild(CategoryNode child)
        {
            _children.Add(child);
        }

        /// <summary>
        /// Constructs the category name from the category words. Basically combines all category words separated with space.
        /// </summary>
        /// <returns>Category name.</returns>
        public string GetName()
        {
            var result = _categoryWords[0];
            for (var i = 1; i < _categoryWords.Count; i++){
                result += " " + _categoryWords[i];
            }
            return result;
        }

        /// <summary>
        /// Searches the children of this node for a specific category name.
        /// </summary>
        /// <param name="childName">Category name of the child.</param>
        /// <returns>The child with the given category name.</returns>
        public CategoryNode GetChild(string childName)
        {
            foreach (var child in _children)
            {
                if (child.GetName() == childName)
                {
                    return child;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds frequency count of the term to the counts hash map of all ascendants of this node.
        /// </summary>
        /// <param name="termId">ID of the occurring term.</param>
        /// <param name="count">Frequency of the term.</param>
        public void AddCounts(int termId, int count)
        {
            var current = this;
            while (current._parent != null)
            {
                current._counts.PutNTimes(termId, count);
                current = current._parent;
            }
        }
        
        /// <summary>
        /// Checks if the given node is an ancestor of the current node.
        /// </summary>
        /// <param name="ancestor">Node for which ancestor check will be done</param>
        /// <returns>True, if the given node is an ancestor of the current node.</returns>
        public bool IsDescendant(CategoryNode ancestor){
            if (Equals(ancestor)){
                return true;
            }
            if (_parent == null){
                return false;
            }
            return _parent.IsDescendant(ancestor);
        }

        /// <summary>
        /// Accessor of the children attribute
        /// </summary>
        /// <returns>Children of the node</returns>
        public List<CategoryNode> GetChildren()
        {
            return _children;
        }

        public List<KeyValuePair<int, int>> TopN(int n)
        {
            if (n <= _counts.Keys.Count)
            {
                return _counts.TopN(n);
            }
            else
            {
                return _counts.TopN(_counts.Keys.Count);
            }
        }

        /// <summary>
        /// Recursive method that returns the hierarchy string of the node. Hierarchy string is obtained by concatenating the
        /// names of all ancestor nodes separated with '%'.
        /// </summary>
        /// <returns>Hierarchy string of this node</returns>
        public string ToString()
        {
            if (_parent != null)
            {
                if (_parent._parent != null)
                {
                    return _parent.ToString() + "%" + GetName();
                }
                else
                {
                    return GetName();
                }
            }

            return "";
        }
        
        /// <summary>
        /// Recursive method that sets the representative count. The representative count filters the most N frequent words.
        /// </summary>
        /// <param name="representativeCount">Number of representatives.</param>
        public void SetRepresentativeCount(int representativeCount){
            if (representativeCount <= _counts.Keys.Count){
                var topList = _counts.TopN(representativeCount);
                _counts = new CounterHashMap<int>();
                foreach (var entry in topList){
                    _counts.PutNTimes(entry.Key, entry.Value);
                }
            }
        }

        /// <summary>
        /// Recursive method that checks the query words in the category words of all descendants of this node and
        /// accumulates the nodes that satisfies the condition. If any word  in the query appears in any category word, the
        /// node will be accumulated.
        /// </summary>
        /// <param name="query">Query string</param>
        /// <param name="result">Accumulator array</param>
        public void GetCategoriesWithKeyword(Query.Query query, List<CategoryNode> result){
            double categoryScore = 0;
            for (var i = 0; i < query.Size(); i++){
                if (_categoryWords.Contains(query.GetTerm(i).GetName())){
                    categoryScore++;
                }
            }
            if (categoryScore > 0){
                result.Add(this);
            }
            foreach (var child in _children){
                child.GetCategoriesWithKeyword(query, result);
            }
        }

        /// <summary>
        /// Recursive method that checks the query words in the category words of all descendants of this node and
        /// accumulates the nodes that satisfies the condition. If any word  in the query appears in any category word, the
        /// node will be accumulated.
        /// </summary>
        /// <param name="query">Query string</param>
        /// <param name="dictionary">Term dictionary</param>
        /// <param name="result">Accumulator array</param>
        public void GetCategoriesWithCosine(Query.Query query, TermDictionary dictionary, List<CategoryNode> result){
            double categoryScore = 0;
            for (var i = 0; i < query.Size(); i++){
                var term = (Term) dictionary.GetWord(query.GetTerm(i).GetName());
                if (term != null){
                    categoryScore += _counts.Count(term.GetTermId());
                }
            }
            if (categoryScore > 0){
                result.Add(this);
            }
            foreach (var child in _children){
                child.GetCategoriesWithCosine(query, dictionary, result);
            }
        }

    }
}