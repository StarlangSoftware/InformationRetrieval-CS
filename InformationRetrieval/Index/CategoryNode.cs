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

        public CategoryNode(string name, CategoryNode parent)
        {
            _categoryWords = new List<string>(name.Split(" "));
            _parent = parent;
            if (parent != null)
            {
                parent.AddChild(this);
            }
        }

        private void AddChild(CategoryNode child)
        {
            _children.Add(child);
        }

        public string GetName()
        {
            var result = _categoryWords[0];
            for (var i = 1; i < _categoryWords.Count; i++){
                result += " " + _categoryWords[i];
            }
            return result;
        }

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

        public void AddCounts(int termId, int count)
        {
            var current = this;
            while (current._parent != null)
            {
                current._counts.PutNTimes(termId, count);
                current = current._parent;
            }
        }
        
        public bool IsDescendant(CategoryNode ancestor){
            if (Equals(ancestor)){
                return true;
            }
            if (_parent == null){
                return false;
            }
            return _parent.IsDescendant(ancestor);
        }

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
        
        public void SetRepresentativeCount(int representativeCount){
            if (representativeCount <= _counts.Keys.Count){
                var topList = _counts.TopN(representativeCount);
                _counts = new CounterHashMap<int>();
                foreach (var entry in topList){
                    _counts.PutNTimes(entry.Key, entry.Value);
                }
            }
        }

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