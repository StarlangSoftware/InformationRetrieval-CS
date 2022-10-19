using System.Collections.Generic;

namespace InformationRetrieval.Document
{
    public class CategoryHierarchy
    {
        private List<string> _categoryList;
        
        public CategoryHierarchy(string list){
            _categoryList = new List<string>();
            _categoryList.AddRange(list.Split("%"));
        }

        public override string ToString(){
            var result = _categoryList[0];
            for (var i = 1; i < _categoryList.Count; i++){
                result += "%" + _categoryList[i];
            }
            return result;
        }

    }
}