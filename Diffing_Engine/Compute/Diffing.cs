/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.Engine;
using BH.oM.Data.Collections;
using BH.oM.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using BH.Engine.Serialiser;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Reflection;

namespace BH.Engine.Diffing
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a Delta object with the differences between the Stream objects and the provided object list.")]
        [Input("previousStream", "A previous version of a Stream")]
        [Input("currentStream", "A new version of a Stream")]
        [Input("diffConfig", "Sets configs such as properties to be ignored in the diffing, or enable/disable property-by-property diffing.\nBy default it takes the diffConfig property of the Stream. This input can be used to override it.")]
        public static Delta Diffing(Stream previousStream, Stream currentStream, DiffConfig diffConfig = null)
        {
            // Set configurations if diffConfig is null
            diffConfig = diffConfig == null ? new DiffConfig() : diffConfig;

            // Take the Stream's objects
            List<IBHoMObject> currentObjs = currentStream.Objects.ToList();
            List<IBHoMObject> readObjs = previousStream.Objects.ToList();

            // Make dictionary with object hashes to speed up the next lookups
            Dictionary<string, IBHoMObject> readObjs_dict = readObjs.ToDictionary(obj => obj.GetHashFragment().Hash, obj => obj);

            // Dispatch the objects: new, modified or old
            List<IBHoMObject> toBeCreated = new List<IBHoMObject>();
            List<IBHoMObject> toBeUpdated = new List<IBHoMObject>();
            List<IBHoMObject> toBeDeleted = new List<IBHoMObject>();
            var objModifiedProps = new Dictionary<string, Dictionary<string, Tuple<object, object>>>();

            foreach (var obj in currentObjs)
            {
                var hashFragm = obj.GetHashFragment();

                if (hashFragm?.PreviousHash == null)
                {
                    toBeCreated.Add(obj); // It's a new object
                }

                else if (hashFragm.PreviousHash == hashFragm.Hash)
                {
                    // It's NOT been modified
                }

                else if (hashFragm.PreviousHash != hashFragm.Hash)
                {
                    toBeUpdated.Add(obj); // It's been modified

                    if (diffConfig.EnablePropertyDiffing)
                    {
                        // Determine changed properties
                        IBHoMObject oldObjState = null;
                        readObjs_dict.TryGetValue(hashFragm.PreviousHash, out oldObjState);

                        if (oldObjState == null) continue;

                        var differentProps = Query.DifferentProperties(obj, oldObjState, diffConfig);

                        objModifiedProps.Add(hashFragm.Hash, differentProps);
                    }
                }
                else
                {
                    throw new Exception("Could not find hash information to perform Diffing on some objects.");
                }
            }

            // If no modified property was found, set the field to null (otherwise will get empty list)
            objModifiedProps = objModifiedProps.Count == 0 ? null : objModifiedProps;

            // All ReadObjs that cannot be found by hash in the previousHash of the CurrentObjs are toBeDeleted
            Dictionary<string, IBHoMObject> CurrentObjs_withPreviousHash_dict = currentObjs
                  .Where(obj => obj.GetHashFragment().PreviousHash != null)
                  .ToDictionary(obj => obj.GetHashFragment().PreviousHash, obj => obj);

            toBeDeleted = readObjs_dict.Keys.Except(CurrentObjs_withPreviousHash_dict.Keys)
                .Where(k => readObjs_dict.ContainsKey(k)).Select(k => readObjs_dict[k]).ToList();

            return new Delta(toBeCreated, toBeDeleted, toBeUpdated, objModifiedProps);

        }

        [Description("Dispatch objects in two sets into the ones exclusive to one set, the other, or both.")]
        [Input("setA", "A previous version of a Stream")]
        [Input("setB", "A new version of a Stream")]
        [Input("diffConfig", "Sets configs such as properties to be ignored in the diffing, or enable/disable property-by-property diffing.")]
        [MultiOutputAttribute(0, "OnlySetA", "Object existing exclusively in setA")]
        [MultiOutputAttribute(1, "Intersection-A", "Objects existing in both sets. Returns objects originally from set A.")]
        [MultiOutputAttribute(2, "Intersection-B", "Objects existing in both sets. Returns objects originally from set B.")]
        [MultiOutputAttribute(3, "OnlySetB", "Object existing exclusively in setB")]
        public static Output<List<IBHoMObject>, List<IBHoMObject>, List<IBHoMObject>, List<IBHoMObject>> Diffing(IEnumerable<IBHoMObject> setA, IEnumerable<IBHoMObject> setB, DiffConfig diffConfig = null)
        {
            Stream streamA = BH.Engine.Diffing.Create.Stream(setA, diffConfig, "");
            Stream streamB = BH.Engine.Diffing.Create.Stream(setB, diffConfig, "");

            VennDiagram<IBHoMObject> diagram = Engine.Data.Create.VennDiagram(streamA.Objects, streamB.Objects, new HashFragmComparer<IBHoMObject>());

            return new Output<List<IBHoMObject>, List<IBHoMObject>, List<IBHoMObject>, List<IBHoMObject>>
            {
                Item1 = diagram.OnlySet1,
                Item2 = diagram.Intersection.Select(tuple => tuple.Item1).ToList(),
                Item3 = diagram.Intersection.Select(tuple => tuple.Item2).ToList(),
                Item4 = diagram.OnlySet2,
            };
        }


        /***************************************************/


        /***************************************************/
        /**** Public Fields                             ****/
        /***************************************************/

        public class HashFragmComparer<T> : IEqualityComparer<T> where T : IBHoMObject
        {
            public bool Equals(T x, T y)
            {
                string firstHash = x.GetHashFragment().Hash;
                string secHash = y.GetHashFragment().Hash;
                if (!string.IsNullOrEmpty(firstHash) && !string.IsNullOrEmpty(secHash) && firstHash == secHash)
                    return true;
                else
                    return false;
            }
            public int GetHashCode(T obj)
            {
                return obj.GetHashFragment().GetHashCode();
            }
        }

        /***************************************************/
    }
}
