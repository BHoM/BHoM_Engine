using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IEnumerable<object> DistinctProperties<T>(this IEnumerable<T> objects, Type propertyType) where T : IObject
        {
            //MethodInfo method = typeof(Merge).GetMethod("MergePropertyObjects", new Type[] { typeof(List<T>) });
            var method = typeof(Query)
                        .GetMethods()
                        .Single(m => m.Name == "DistinctProperties" && m.IsGenericMethodDefinition && m.GetParameters().Count() == 1);

            MethodInfo generic = method.MakeGenericMethod(new Type[] { typeof(T), propertyType });
            return (IEnumerable<object>)generic.Invoke(null, new object[] { objects });

        }

        /***************************************************/

        public static IEnumerable<P> DistinctProperties<T, P>(this IEnumerable<T> objects) where T : IObject where P : IObject
        {
            // Get the list of properties corresponding to type P
            Dictionary<Type, List<PropertyInfo>> propertyDictionary = typeof(T).GetProperties().GroupBy(x => x.PropertyType).ToDictionary(x => x.Key, x => x.ToList());
            List<PropertyInfo> propertiesValue, propertiesCollection;
            if (!propertyDictionary.TryGetValue(typeof(P), out propertiesValue) && !propertyDictionary.TryGetValue(typeof(IEnumerable<P>), out propertiesCollection))
                return new List<P>();

            // Collect the property objects
            List<P> propertyObjects = new List<P>();
            Dictionary<PropertyInfo, Action<T, P>> setters = new Dictionary<PropertyInfo, Action<T, P>>();
            Dictionary<PropertyInfo, Func<T, P>> getters = new Dictionary<PropertyInfo, Func<T, P>>();

            foreach (PropertyInfo property in propertiesValue)
            {
                // Optimisation using this article: https://blogs.msmvps.com/jonskeet/2008/08/09/making-reflection-fly-and-exploring-delegates/
                Func<T, P> getProp = (Func<T, P>)Delegate.CreateDelegate(typeof(Func<T, P>), property.GetGetMethod());
                Action<T, P> setProp = (Action<T, P>)Delegate.CreateDelegate(typeof(Action<T, P>), property.GetSetMethod());


                // Keep those for later
                setters.Add(property, setProp);
                getters.Add(property, getProp);

                // Collect the objects from this property
                propertyObjects.AddRange(objects.Select(x => getProp(x)));
            }

            // Clone the distinct property objects
            Dictionary<Guid, P> cloneDictionary = CloneObjects<P>(propertyObjects.DistinctDictionary());


            //Assign cloned distinct property objects back into input objects
            foreach (PropertyInfo property in propertiesValue)
            {
                Action<T, P> setProp = setters[property];
                Func<T, P> getProp = getters[property];
                foreach (T x in objects)
                    setProp(x, cloneDictionary[getProp(x).BHoM_Guid]);
            }

            //Return the disticnt property objects
            return cloneDictionary.Values;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Dictionary<Guid, T> DistinctDictionary<T>(this IEnumerable<T> list) where T : IObject
        {
            return list.GroupBy(x => x.BHoM_Guid).Select(x => x.First()).ToDictionary(x => x.BHoM_Guid);
        }

        /***************************************************/

        private static Dictionary<Guid, T> CloneObjects<T>(Dictionary<Guid, T> dict) where T : IObject
        {
            Dictionary<Guid, T> clones = new Dictionary<Guid, T>();

            foreach (KeyValuePair<Guid, T> kvp in dict)
            {
                T obj = (T)kvp.Value.GetShallowClone();
                obj.CustomData = new Dictionary<string, object>(kvp.Value.CustomData);
                clones.Add(kvp.Key, obj);
            }

            return clones;
        }

        /***************************************************/
    }
}
