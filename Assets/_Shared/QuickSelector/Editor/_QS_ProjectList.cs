using UnityEngine;


namespace QuickSelect
{ 
    public class _QS_ProjectList : ScriptableObject
    {
        [HideInInspector]
        public ListType listType;
        [HideInInspector]
        public ObjectList list;

        public void Setup(string listName, ListType listType)
        {
            this.listType = listType;
            list = new ObjectList(listName);
        }
    }
}