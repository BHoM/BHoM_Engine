/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System.Collections;
using System.Collections.Concurrent;

namespace BH.Engine.Diffing
{
    public static partial class Compute
    {
        [Description("Dispatch objects in two sets into the ones exclusive to one set, the other, or both.")]
        [Input("previousObjects", "A set of object representing a previous revision")]
        [Input("currentObjects", "A set of object representing a new Revision")]
        [Input("diffConfig", "Sets configs such as properties to be ignored in the diffing, or enable/disable property-by-property diffing.")]
        public static Diff Diffing(IEnumerable<IBHoMObject> previousObjects, IEnumerable<IBHoMObject> currentObjects, DiffConfig diffConfig = null)
        {
            // Set configurations if diffConfig is null
            diffConfig = diffConfig == null ? new DiffConfig() : diffConfig;

            // Take the Revision's objects
            List<IBHoMObject> currentObjs = currentObjects.ToList();
            List<IBHoMObject> readObjs = previousObjects.ToList();

            // Make dictionary with object hashes to speed up the next lookups
            Dictionary<string, IBHoMObject> readObjs_dict = readObjs.ToDictionary(obj => obj.GetHashFragment().CurrentHash, obj => obj);

            // Dispatch the objects: new, modified or old
            List<IBHoMObject> newObjs = new List<IBHoMObject>();
            List<IBHoMObject> modifiedObjs = new List<IBHoMObject>();
            List<IBHoMObject> oldObjs = new List<IBHoMObject>();
            List<IBHoMObject> unChanged = new List<IBHoMObject>();

            Dictionary<string, Dictionary<string, Tuple<object, object>>> objModifiedProps = new Dictionary<string, Dictionary<string, Tuple<object, object>>>();

            if (diffConfig.EnablePropertyDiffing && currentObjects.Count() < 1000)
                ComputeDiff(readObjs_dict, currentObjs, diffConfig, newObjs, modifiedObjs, unChanged, objModifiedProps);
            else
                ComputeDiff_Parall(readObjs_dict, currentObjs, diffConfig, newObjs, modifiedObjs, unChanged, objModifiedProps);

            // If no modified property was found, set the field to null (otherwise will get empty list)
            objModifiedProps = objModifiedProps.Count == 0 ? null : objModifiedProps;

            // All ReadObjs that cannot be found by hash in the previousHash of the CurrentObjs are toBeDeleted
            Dictionary<string, IBHoMObject> CurrentObjs_withPreviousHash_dict = currentObjs
                  .Where(obj => obj.GetHashFragment().PreviousHash != null)
                  .ToDictionary(obj => obj.GetHashFragment().PreviousHash, obj => obj);

            oldObjs = readObjs_dict.Keys.Except(CurrentObjs_withPreviousHash_dict.Keys)
                .Where(k => readObjs_dict.ContainsKey(k)).Select(k => readObjs_dict[k]).ToList();

            return new Diff(newObjs, oldObjs, modifiedObjs, diffConfig, objModifiedProps, unChanged);
        }


        [Description("Dispatch objects in two sets into the ones exclusive to one set, the other, or both.")]
        [Input("pastObjects", "A set of object representing a past revision")]
        [Input("currentObjects", "A set of object representing a newer Revision")]
        [Input("diffConfig", "Sets configs such as properties to be ignored in the diffing, or enable/disable property-by-property diffing.")]
        public static Diff Diffing(IEnumerable<object> pastObjects, IEnumerable<object> currentObjects, DiffConfig diffConfig = null)
        {
            // Dispatch the objects in BHoMObjects and generic objects.
            IEnumerable<IBHoMObject> prevObjs_BHoM = pastObjects.OfType<IBHoMObject>();
            IEnumerable<IBHoMObject> currObjs_BHoM = currentObjects.OfType<IBHoMObject>();

            // If all objects are bhomobjects, just call the appropriate method
            if (pastObjects.Count() == prevObjs_BHoM.Count() && currentObjects.Count() == currObjs_BHoM.Count())
                return Diffing(prevObjs_BHoM, currObjs_BHoM, diffConfig);

            IEnumerable<object> prevObjs_nonBHoM = pastObjects.Where(o => !(o is IBHoMObject));
            IEnumerable<object> currObjs_nonBHoM = currentObjects.Where(o => !(o is IBHoMObject));

            // Compute the specific Diffing for the BHoMObjects.
            Diff diff = Compute.Diffing(prevObjs_BHoM, currObjs_BHoM, diffConfig);

            // Compute the generic Diffing for the other objects.
            // This is left to the VennDiagram with a HashComparer (specifically, this doesn't use the HashFragment).
            VennDiagram<object> vd = Engine.Data.Create.VennDiagram(prevObjs_nonBHoM, currObjs_nonBHoM, new DiffingHashComparer<object>());

            // Concatenate the results of the two diffing operations.
            List<object> allPrevObjs = new List<object>();
            List<object> allCurrObjs = new List<object>();

            allPrevObjs.AddRange(diff.AddedObjects);
            allPrevObjs.AddRange(vd.OnlySet1);

            allPrevObjs.AddRange(diff.RemovedObjects);
            allPrevObjs.AddRange(vd.OnlySet2);

            // Create the final, actual diff.
            Diff finalDiff = new Diff(allCurrObjs, allPrevObjs, diff.ModifiedObjects, diffConfig, diff.ModifiedPropsPerObject, diff.UnchangedObjects);

            return finalDiff;
        }

        private static void ComputeDiff(Dictionary<string, IBHoMObject> readObjs_dict, List<IBHoMObject> currentObjs, DiffConfig diffConfig,
            List<IBHoMObject> newObjs, List<IBHoMObject> modifiedObjs, List<IBHoMObject> unChanged,
            Dictionary<string, Dictionary<string, Tuple<object, object>>> objModifiedProps)
        {
            foreach (var obj in currentObjs)
            {
                var hashFragm = obj.GetHashFragment();

                if (hashFragm?.PreviousHash == null)
                {
                    newObjs.Add(obj); // It's a new object
                }

                else if (hashFragm.PreviousHash == hashFragm.CurrentHash)
                {
                    // It's NOT been modified
                    if (diffConfig.StoreUnchangedObjects)
                        unChanged.Add(obj);
                }

                else if (hashFragm.PreviousHash != hashFragm.CurrentHash)
                {
                    modifiedObjs.Add(obj); // It's been modified

                    if (diffConfig.EnablePropertyDiffing)
                    {
                        // Determine changed properties
                        IBHoMObject oldObjState = null;
                        readObjs_dict.TryGetValue(hashFragm.PreviousHash, out oldObjState);

                        if (oldObjState == null) continue;

                        var differentProps = Query.DifferentProperties(obj, oldObjState, diffConfig);

                        objModifiedProps.Add(hashFragm.CurrentHash, differentProps);
                    }
                }
                else
                {
                    throw new Exception("Could not find hash information to perform Diffing on some objects.");
                }
            }

        }

        private static void ComputeDiff_Parall(Dictionary<string, IBHoMObject> readObjs_dict, List<IBHoMObject> currentObjs, DiffConfig diffConfig,
          List<IBHoMObject> newObjs, List<IBHoMObject> modifiedObjs, List<IBHoMObject> unChanged,
          Dictionary<string, Dictionary<string, Tuple<object, object>>> objModifiedProps)
        {
            ConcurrentBag<IBHoMObject> newObjsCD = new ConcurrentBag<IBHoMObject>();
            ConcurrentBag<IBHoMObject> modObjsCD =  new ConcurrentBag<IBHoMObject>();
            ConcurrentBag<IBHoMObject> unChangedCD = new ConcurrentBag<IBHoMObject>();

            ConcurrentDictionary<string, Dictionary<string, Tuple<object, object>>> objModifiedPropsCD = new ConcurrentDictionary<string, Dictionary<string, Tuple<object, object>>>();

            int objCount = currentObjs.Count;

            Parallel.For(0, objCount, i =>
            {
                IBHoMObject obj = currentObjs[i];

                var hashFragm = obj.GetHashFragment();

                if (hashFragm?.PreviousHash == null)
                {
                    newObjsCD.Add(obj); // It's a new object
                }

                else if (hashFragm.PreviousHash == hashFragm.CurrentHash)
                {
                    // It's NOT been modified
                    if (diffConfig.StoreUnchangedObjects)
                        unChangedCD.Add(obj);
                }

                else if (hashFragm.PreviousHash != hashFragm.CurrentHash)
                {
                    modObjsCD.Add(obj); // It's been modified

                    // Determine changed properties
                    IBHoMObject oldObjState = null;
                    readObjs_dict.TryGetValue(hashFragm.PreviousHash, out oldObjState);

                    if (oldObjState != null)
                    {
                        var differentProps = Query.DifferentProperties(obj, oldObjState, diffConfig);

                        objModifiedProps.Add(hashFragm.CurrentHash, differentProps);
                    }
                }
                else
                {
                    throw new Exception("Could not find hash information to perform Diffing on some objects.");
                }
            });

            newObjs = newObjsCD.ToList();
            modifiedObjs = modObjsCD.ToList();
            unChanged = unChangedCD.ToList();

            objModifiedProps = new Dictionary<string, Dictionary<string, Tuple<object, object>>>(objModifiedPropsCD);
        }

    }
}

