using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace QuickSelect
{
        public enum ListType
        {
            None,
            Project,
            Hierarchy,
            SceneSelection
        }
    
        [System.Serializable]
        public class ObjectList
        {
            public string listName;
            public List<Object> objects;
            private List<Object> orderedObjects;

            public ObjectList(string listName)
            {
                this.listName = listName;
                objects = new List<Object>();
            }

            public void Add(Object addObject)
            {
                objects.Add(addObject);
                orderedObjects = objects.OrderBy(x => x.name).ToList();
            }

            public void Remove(Object removeObject)
            {
                objects.Remove(removeObject);
                orderedObjects = objects.OrderBy(x => x.name).ToList();
            }

            public void MoveUp(int nrToMove)
            {
                Object first = objects[nrToMove];
                Object inPlace = objects[nrToMove - 1];
                objects[nrToMove - 1] = first;
                objects[nrToMove] = inPlace;
            }

            public void MoveDown(int nrToMove)
            {
                Object first = objects[nrToMove];
                Object inPlace = objects[nrToMove + 1];
                objects[nrToMove + 1] = first;
                objects[nrToMove] = inPlace;
            }


            public int ListNr(Object activeObject, bool abc)
            {
                if ( orderedObjects == null || orderedObjects.Count == 0 )
                    orderedObjects = objects.OrderBy(x => x.name).ToList();

                int listNr = -1;

                for ( int i = 0; i < objects.Count; i++ )
                    if ( activeObject == (abc ? orderedObjects[i] : objects[i]) )
                    {
                        listNr = i;
                        break;
                    }

                return listNr;
            }

            public bool IsInList(Object compareThis)
            {
                for ( int i = 0; i < objects.Count; i++ )
                    if ( objects[i] == compareThis )
                        return true;

                return false;
            }

            public void Validate()
            {
                int count = objects.Count;
                for ( int i = 0; i < count; i++ )
                    if ( objects[i] == null )
                    {
                        objects.RemoveAt(i);
                        count--;
                        i--;
                    }
                
                orderedObjects = objects.OrderBy(x => x.name).ToList();
            }

            public float Count { get { return objects.Count; }}

            public Object GetObject(int nr, bool abc)
            {
                if ( orderedObjects == null || orderedObjects.Count == 0 )
                    orderedObjects = objects.OrderBy(x => x.name).ToList();
                return abc ? orderedObjects[nr] : objects[nr];
            }
        }

        [System.Serializable]
        public class ListContainer
        {
            public ListType listType;
            public List<ObjectList> lists;

            public ListContainer(ListType listType)
            {
                this.listType = listType;
                lists = new List<ObjectList>();
            }

            public void CreateList(string listName)
            {
                if ( lists == null )
                    lists = new List<ObjectList>();

                lists.Add(new ObjectList(listName));
            }

            public void AddList(ObjectList list)
            {
                if ( lists == null )
                    lists = new List<ObjectList>();

                lists.Add(list);
            }

            public ObjectList GetList(string listName)
            {
                if ( lists == null )
                    lists = new List<ObjectList>();

                for ( int i = 0; i < lists.Count; i++ )
                        if ( lists[i].listName == listName )
                            return lists[i];

                return null;
            }
    }
}