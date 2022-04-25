/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BH.oM.Analytical.Results;
using BH.oM.Base.Attributes;
using BH.Engine.Base;
using BH.Engine.Reflection;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/


        [Description("Group results by ResultCase.")]
        [Input("results", "Results to group.")]
        [Output("groupedResults", "Results grouped as List of Lists.")]
        public static List<List<T>> GroupByCase<T>(this IEnumerable<T> results) where T : ICasedResult
        {
            return results.GroupBy(x => x.ResultCase).Select(x => x.ToList()).ToList();
        }

        /***************************************************/

        [Description("Group results by ObjectID.")]
        [Input("results", "Results to group.")]
        [Output("groupedResults", "Results grouped as List of Lists.")]
        public static List<List<T>> GroupByObjectId<T>(this IEnumerable<T> results) where T : IObjectIdResult
        {
            return results.GroupBy(x => x.ObjectId).Select(x => x.ToList()).ToList();
        }

        /***************************************************/

        [Description("Group results by TimeStep.")]
        [Input("results", "Results to group.")]
        [Output("groupedResults", "Results grouped as List of Lists.")]
        public static List<List<T>> GroupByTimeStep<T>(this IEnumerable<T> results) where T : ITimeStepResult
        {
            return results.GroupBy(x => x.TimeStep).Select(x => x.ToList()).ToList();
        }

        /***************************************************/

        [Description("Group results by all Scenario type properties of the object.")]
        [Input("results", "Results to group.")]
        [Output("groupedResults", "Results grouped as List of Lists.")]
        public static List<List<T>> GroupByScenario<T>(this IEnumerable<T> results) where T : IResult
        {
            if (results == null || !results.Any())
                return new List<List<T>>();

            return GroupByMultipleSelectors(results, IdentifierFunctions(results.First().GetType(), typeof(ScenarioIdentifierAttribute)));
        }

        /***************************************************/

        [Description("Group results by Object identifiers of the object.")]
        [Input("results", "Results to group.")]
        [Output("groupedResults", "Results grouped as List of Lists.")]
        public static List<List<T>> GroupByObjectIdentifiers<T>(this IEnumerable<T> results) where T : IResult
        {
            if (results == null || !results.Any())
                return new List<List<T>>();

            return GroupByMultipleSelectors(results, IdentifierFunctions(results.First().GetType(), typeof(ObjectIdentifierAttribute)));
        }

        /***************************************************/

        [Description("Group results by all identifiers of the object.")]
        [Input("results", "Results to group.")]
        [Output("groupedResults", "Results grouped as List of Lists.")]
        public static List<List<T>> GroupByAllIdentifiers<T>(this IEnumerable<T> results) where T : IResult
        {
            if (results == null || !results.Any())
                return new List<List<T>>();

            return GroupByMultipleSelectors(results, IdentifierFunctions(results.First().GetType(), typeof(IdentifierAttribute)));
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Groups by multiple selectors by contiously grouping by each provided selector.")]
        private static List<List<T>> GroupByMultipleSelectors<T>(this IEnumerable<T> results, IEnumerable<Func<object, object>> funcs)
        {
            IEnumerable<IEnumerable<T>> listGroup = new List<IEnumerable<T>>() { results };
            foreach (var func in funcs)
                listGroup = listGroup.Select(x => x.GroupBy(y => func(y))).SelectMany(x => x);

            return listGroup.Select(x => x.ToList()).ToList();
        }

        /***************************************************/

        [Description("Gets a list of compiled selectors corresponding to all properties with the attribute type on the type. Allows the attribute type to be defined on an interface of the object type.")]
        private static List<Func<object, object>> IdentifierFunctions(Type type, Type attributeType)
        {
            Tuple<Type, Type> key = new Tuple<Type, Type>(type, attributeType);
            List<Func<object, object>> funcs;
            lock (m_funcLock)   //Lock extraction for thread saftey
            {
                if (!m_identifierFunctions.TryGetValue(key, out funcs)) //Try get stored pre-compiled functions
                {
                    //Get all properties with the provided attribute and compile to functions
                    funcs = type.PropertiesWithAttribute(attributeType, true, true).Select(x => x.ToFunc()).ToList();
                    m_identifierFunctions[key] = funcs; //Store functions for future usage
                }
            }
            return funcs;
        }

        /***************************************************/

        [Description("Compiles the property info into a Func<object,object>.")]
        private static Func<object, object> ToFunc(this PropertyInfo prop)
        {
            var method = typeof(Query)
                      .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                      .Single(m => m.Name == nameof(CompileCastProperty) && m.IsGenericMethodDefinition && m.GetParameters().Count() == 1);

            MethodInfo generic = method.MakeGenericMethod(new Type[] { prop.DeclaringType, prop.PropertyType });
            return (Func<object, object>)generic.Invoke(null, new object[] { prop });
        }

        /***************************************************/

        [Description("Creates a delegate of type Func<T,P> that matches that of the provided PropertyInfo. Returns a Func<object,object> that casts the object into a T.")]
        private static Func<object, object> CompileCastProperty<T, P>(PropertyInfo prop)
        {
            Func<T, P> func = (Func<T, P>)Delegate.CreateDelegate(typeof(Func<T, P>), prop.GetGetMethod());
            return x => func((T)x);
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<Tuple<Type, Type>, List<Func<object, object>>> m_identifierFunctions = new Dictionary<Tuple<Type, Type>, List<Func<object, object>>>();
        private static object m_funcLock = new object();

        /***************************************************/

    }
}



