/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Base;
using BH.oM.Base.Attributes;
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
            if (objects == null)
                return new List<P>();

            // Get the list of properties corresponding to type P
            Dictionary<Type, List<PropertyInfo>> propertyDictionary = typeof(T).GetProperties().GroupBy(x => x.PropertyType).ToDictionary(x => x.Key, x => x.ToList());
            List<PropertyInfo> propertiesSingle, propertiesList, propertiesGroups;
            bool isFragment = typeof(IFragment).IsAssignableFrom(typeof(P));
            bool singleExist, listExists;
            if (!(singleExist = propertyDictionary.TryGetValue(typeof(P), out propertiesSingle)))
            {
                singleExist = false;
                propertiesSingle = new List<PropertyInfo>();
            }
            if (!(listExists = propertyDictionary.TryGetValue(typeof(List<P>), out propertiesList)))
            {
                listExists = false;
                propertiesList = new List<PropertyInfo>();
            }
            if (!propertyDictionary.TryGetValue(typeof(BH.oM.Base.BHoMGroup<P>), out propertiesGroups))
            {
                if (!singleExist && !listExists && !isFragment)
                {
                    return new List<P>();
                }

                propertiesGroups = new List<PropertyInfo>();
            }

            // Collect the property objects
            List<P> propertyObjects = new List<P>();

            //Get the distinct properties for the single value properties
            foreach (PropertyInfo property in propertiesSingle)
            {
                // Optimisation using this article: https://blogs.msmvps.com/jonskeet/2008/08/09/making-reflection-fly-and-exploring-delegates/
                Func<T, P> getProp = (Func<T, P>)Delegate.CreateDelegate(typeof(Func<T, P>), property.GetGetMethod());

                // Collect the objects from this property
                propertyObjects.AddRange(objects.Select(x => getProp(x)).Where(x => x!=null));
            }

            //Get the distinct properties for the list value properties
            foreach (PropertyInfo property in propertiesList)
            {
                // Optimisation using this article: https://blogs.msmvps.com/jonskeet/2008/08/09/making-reflection-fly-and-exploring-delegates/
                Func<T, List<P>> getProp = (Func<T, List<P>>)Delegate.CreateDelegate(typeof(Func<T, List<P>>), property.GetGetMethod());

                // Collect the objects from this property
                propertyObjects.AddRange(objects.SelectMany(x => (getProp(x) ?? new List<P>()).Where(p => p != null)));
            }

            //Get the distinct properties for the group value properties
            foreach (PropertyInfo property in propertiesGroups)
            {
                // Optimisation using this article: https://blogs.msmvps.com/jonskeet/2008/08/09/making-reflection-fly-and-exploring-delegates/
                Func<T, BH.oM.Base.BHoMGroup<P>> getProp = (Func<T, BHoMGroup<P>>)Delegate.CreateDelegate(typeof(Func<T, BHoMGroup<P>>), property.GetGetMethod());

                // Collect the objects from this property
                propertyObjects.AddRange(objects.SelectMany(x => (getProp(x)?.Elements ?? new List<P>()).Where(p => p != null)));
            }

            //If P is a Fragment, check for any instances in the fragment list
            if (isFragment)
            {
                propertyObjects.AddRange(objects.SelectMany(x => x.Fragments.OfType<P>()));
            }

            //Return the disticnt property objects
            return propertyObjects;
        }


        /***************************************************/
    }
}




