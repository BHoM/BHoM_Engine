using BH.oM.Base;
using BH.Engine.Base;
using BH.Engine.Reflection;
using System.Collections;

namespace BH.oM.Base
{
    // Must be unpackable
    public class Container<T> : BHoMObject, IContainer
    {
        public T? SomeObject { get; set; }
        public IEnumerable<T> ListOfObjects { get; set; } = new List<T>();
        public IEnumerable<IEnumerable<T>> ListOfLists { get; set; } = new List<List<T>>();
    }

    // We want to support this, it should get the values of the dictionary.
    public class DictionaryContainer<T> : Container<T>
    {
        public Dictionary<string, T> Dictionary { get; set; } = new Dictionary<string, T>();
    }

    // We want to support this, it should get the values of the dictionary and flatten them.
    public class DictionaryListContainer<T> : Container<T>
    {
        public Dictionary<string, IEnumerable<T>> DictionaryOfLists { get; set; } = new Dictionary<string, IEnumerable<T>>();
    }

    // Not supported.
    public class ListOfDictionariesContainer<T> : Container<T>
    {
        public IEnumerable<Dictionary<string, T>> ListOfDictionaries { get; set; } = new List<Dictionary<string, T>>();
    }

    // Not supported.
    public class ListOfListOfListContainer<T> : Container<T>
    {
        public IEnumerable<IEnumerable<IEnumerable<T>>> ListOfListOfLists { get; set; } = new List<List<List<T>>>();
    }
}