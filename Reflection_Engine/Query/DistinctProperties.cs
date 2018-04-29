using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Extract all the distinct objects of a specific type from the properties of a list of objects")]
        [Input("objects", "List of objects to extract the properties from")]
        [Input("propertyType", "Type of property to extract")]
        public static IEnumerable<object> DistinctProperties<T>(this IEnumerable<T> objects, Type propertyType) where T : IBHoMObject
        {
            //MethodInfo method = typeof(Merge).GetMethod("MergePropertyObjects", new Type[] { typeof(List<T>) });
            var method = typeof(Query)
                        .GetMethods()
                        .Single(m => m.Name == "DistinctProperties" && m.IsGenericMethodDefinition && m.GetParameters().Count() == 1);

            MethodInfo generic = method.MakeGenericMethod(new Type[] { typeof(T), propertyType });
            return (IEnumerable<object>)generic.Invoke(null, new object[] { objects });

        }

        /***************************************************/

        [Description("Extract all the distinct objects of a specific type P from the properties of a list of objects")]
        public static IEnumerable<P> DistinctProperties<T, P>(this IEnumerable<T> objects) where T : IBHoMObject where P : IBHoMObject
        {
            // Get the list of properties corresponding to type P
            Dictionary<Type, List<PropertyInfo>> propertyDictionary = typeof(T).GetProperties().GroupBy(x => x.PropertyType).ToDictionary(x => x.Key, x => x.ToList());
            List<PropertyInfo> propertiesSingle, propertiesList;
            bool singleExist;
            if (!(singleExist = propertyDictionary.TryGetValue(typeof(P), out propertiesSingle)))
            {
                singleExist = false;
                propertiesSingle = new List<PropertyInfo>();
            }
            if (!propertyDictionary.TryGetValue(typeof(List<P>), out propertiesList))
            {
                if (!singleExist)
                    return new List<P>();

                propertiesList = new List<PropertyInfo>();
            }

            // Collect the property objects
            List<P> propertyObjects = new List<P>();
            Dictionary<PropertyInfo, Action<T, P>> settersSingle = new Dictionary<PropertyInfo, Action<T, P>>();
            Dictionary<PropertyInfo, Func<T, P>> gettersSingle = new Dictionary<PropertyInfo, Func<T, P>>();

            //Get the distinct properties for the single value properties
            foreach (PropertyInfo property in propertiesSingle)
            {
                // Optimisation using this article: https://blogs.msmvps.com/jonskeet/2008/08/09/making-reflection-fly-and-exploring-delegates/
                Func<T, P> getProp = (Func<T, P>)Delegate.CreateDelegate(typeof(Func<T, P>), property.GetGetMethod());
                Action<T, P> setProp = (Action<T, P>)Delegate.CreateDelegate(typeof(Action<T, P>), property.GetSetMethod());

                // Keep those for later
                settersSingle.Add(property, setProp);
                gettersSingle.Add(property, getProp);

                // Collect the objects from this property
                propertyObjects.AddRange(objects.Select(x => getProp(x)).Where(x => x!=null));
            }

            Dictionary<PropertyInfo, Action<T, List<P>>> settersList = new Dictionary<PropertyInfo, Action<T, List<P>>>();
            Dictionary<PropertyInfo, Func<T, List<P>>> gettersList = new Dictionary<PropertyInfo, Func<T, List<P>>>();

            //Get the distinct properties for the single value properties
            foreach (PropertyInfo property in propertiesList)
            {
                // Optimisation using this article: https://blogs.msmvps.com/jonskeet/2008/08/09/making-reflection-fly-and-exploring-delegates/
                Func<T, List<P>> getProp = (Func<T, List<P>>)Delegate.CreateDelegate(typeof(Func<T, List<P>>), property.GetGetMethod());
                Action<T, List<P>> setProp = (Action<T, List<P>>)Delegate.CreateDelegate(typeof(Action<T, List<P>>), property.GetSetMethod());

                // Keep those for later
                settersList.Add(property, setProp);
                gettersList.Add(property, getProp);

                // Collect the objects from this property
                propertyObjects.AddRange(objects.SelectMany(x => getProp(x)));
            }

            // Clone the distinct property objects
            Dictionary<Guid, P> cloneDictionary = CloneObjects<P>(propertyObjects.DistinctDictionary());


            //Assign cloned distinct single property objects back into input objects
            foreach (PropertyInfo property in propertiesSingle)
            {
                Action<T, P> setProp = settersSingle[property];
                Func<T, P> getProp = gettersSingle[property];
                foreach (T x in objects)
                {
                    P prop = getProp(x);
                    if(prop != null)
                        setProp(x, cloneDictionary[prop.BHoM_Guid]);
                }
            }

            //Assign cloned distinct list property objects back into input objects
            foreach (PropertyInfo property in propertiesList)
            {
                Action<T, List<P>> setProp = settersList[property];
                Func<T, List<P>> getProp = gettersList[property];
                
                foreach (T x in objects)
                    setProp(x, getProp(x).Select(y => cloneDictionary[y.BHoM_Guid]).ToList());

            }

            //Return the disticnt property objects
            return cloneDictionary.Values;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Dictionary<Guid, T> DistinctDictionary<T>(this IEnumerable<T> list) where T : IBHoMObject
        {
            return list.GroupBy(x => x.BHoM_Guid).Select(x => x.First()).ToDictionary(x => x.BHoM_Guid);
        }

        /***************************************************/

        private static Dictionary<Guid, T> CloneObjects<T>(Dictionary<Guid, T> dict) where T : IBHoMObject
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
