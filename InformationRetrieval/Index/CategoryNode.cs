using System.Collections.Generic;
using DataStructure;
using Dictionary.Dictionary;

namespace InformationRetrieval.Index
{
    public class CategoryNode
    {
        private string _name;
        private List<CategoryNode> _children = new List<CategoryNode>();
        private CategoryNode _parent;
        private CounterHashMap<int> _counts = new CounterHashMap<int>();

        public CategoryNode(string name, CategoryNode parent)
        {
            _name = name;
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
            return _name;
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
        
        public string TopNString(TermDictionary dictionary, int n){
            var topN = TopN(n);
            var result = ToString();
            foreach (var item in topN){
                if (!Word.IsPunctuation(dictionary.GetWord(item.Key).GetName())){
                    result += "\t" + dictionary.GetWord(item.Key).GetName() + " (" + item.Value + ")";
                }
            }
            return result;
        }

        public string ToString()
        {
            if (_parent != null)
            {
                if (_parent._parent != null)
                {
                    return _parent.ToString() + "%" + _name;
                }
                else
                {
                    return _name;
                }
            }

            return "";
        }
    }
}