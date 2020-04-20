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

namespace BH.Engine.Diffing
{
    public static partial class Compute
    {
        [Description("Dispatch objects in two sets into the ones exclusive to one set, the other, or both.")]
        [Input("pastObjects", "A set of object representing a previous revision")]
        [Input("currentObjects", "A set of object representing a new Revision")]
        [Input("diffConfig", "Sets configs such as properties to be ignored in the diffing, or enable/disable property-by-property diffing.")]
        public static Diff Diffing(IEnumerable<IBHoMObject> pastObjects, IEnumerable<IBHoMObject> currentObjects, DiffConfig diffConfig = null, string customdataIdName = null)
        {
            // Set configurations if diffConfig is null
            diffConfig = diffConfig == null ? new DiffConfig() : diffConfig;

            // Check if objects have hashfragment.
            // If they don't, try to rely on something else (e.g. their CustomData - SoftwareId).
            if (pastObjects.Select(o => o.GetHashFragment()).Where(o => o != null).Count() < pastObjects.Count())
                if (customdataIdName == null)
                {
                    BH.Engine.Reflection.Compute.RecordError("The objects did not pass through a Diffing Revision." +
                        "\nIn order to do the Diffing, specify a CustomData key where to find the ID to be used (e.g. Revit_elementId).");
                    return null;
                }
                else
                    return DiffingWithCustomId(pastObjects, currentObjects, customdataIdName, diffConfig);
        
            // Take the Revision's objects
            List<IBHoMObject> currentObjs = currentObjects.ToList();
            List<IBHoMObject> readObjs = pastObjects.ToList();

            // Make dictionary with object hashes to speed up the next lookups
            Dictionary<string, IBHoMObject> readObjs_dict = readObjs.ToDictionary(obj => obj.GetHashFragment().CurrentHash, obj => obj);

            // Dispatch the objects: new, modified or old
            List<IBHoMObject> newObjs = new List<IBHoMObject>();
            List<IBHoMObject> modifiedObjs = new List<IBHoMObject>();
            List<IBHoMObject> oldObjs = new List<IBHoMObject>();
            List<IBHoMObject> unChanged = new List<IBHoMObject>();

            var objModifiedProps = new Dictionary<string, Dictionary<string, Tuple<object, object>>>();

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


        [Description("Attempts the diffing for objects of mixed types, also non-BHoMObjects.")]
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

        private static Diff DiffingWithCustomId(IEnumerable<IBHoMObject> pastObjects, IEnumerable<IBHoMObject> currentObjects, string customdataIdName, DiffConfig diffConfig = null)
        {
            // Here we are in the scenario where the objects do not have an HashFragment,
            // but we assume they an identifier in CustomData that let us identify the objects

            // Set configurations if diffConfig is null
            diffConfig = diffConfig == null ? new DiffConfig() : diffConfig;

            List<IBHoMObject> currentObjs = currentObjects.ToList();
            List<IBHoMObject> pastObjs = pastObjects.ToList();

            // Make dictionary with object ids to speed up the next lookups
            Dictionary<string, IBHoMObject> currObjs_dict = currentObjs.ToDictionary(obj => obj.CustomData[customdataIdName].ToString(), obj => obj);
            Dictionary<string, IBHoMObject> pastObjs_dict = pastObjs.ToDictionary(obj => obj.CustomData[customdataIdName].ToString(), obj => obj); 

            // Dispatch the objects: new, modified or deleted
            List<IBHoMObject> newObjs = new List<IBHoMObject>();
            List<IBHoMObject> modifiedObjs = new List<IBHoMObject>();
            List<IBHoMObject> deletedObjs = new List<IBHoMObject>();
            List<IBHoMObject> unChanged = new List<IBHoMObject>();

            var objModifiedProps = new Dictionary<string, Dictionary<string, Tuple<object, object>>>();

            foreach (var kv_curr in currObjs_dict)
            {
                IBHoMObject currentObj = kv_curr.Value;
                string currentObjID = kv_curr.Key;

                // Try to find an object between the pastObjs that has the same ID of the current one.
                IBHoMObject correspondingObj = null;
                pastObjs_dict.TryGetValue(kv_curr.Key, out correspondingObj);

                // If none is found, the current object is new.
                if (correspondingObj == null)
                {
                    newObjs.Add(kv_curr.Value);
                    continue;
                }

                // Otherwise, the current object existed in the past set.

                // Compute the hashes to find if they are different
                string currentHash = Compute.DiffingHash(currentObj, diffConfig);
                string pastHash = Compute.DiffingHash(correspondingObj, diffConfig);

                if (pastHash == currentHash)
                {
                    // It's NOT been modified
                    if (diffConfig.StoreUnchangedObjects)
                        unChanged.Add(currentObj);

                    continue;
                }

                if (pastHash != currentHash)
                {
                    // It's been modified
                    modifiedObjs.Add(currentObj); 

                    if (diffConfig.EnablePropertyDiffing)
                    {
                        // Determine changed properties
                        var differentProps = Query.DifferentProperties(currentObj, correspondingObj, diffConfig);

                        objModifiedProps.Add(currentObjID, differentProps);
                    }

                    continue;
                }
                else
                    throw new Exception("Could not find hash information to perform Diffing on some objects.");
            }

            // If no modified property was found, set the field to null (otherwise will get empty list)
            objModifiedProps = objModifiedProps.Count == 0 ? null : objModifiedProps;

            // All ReadObjs that cannot be found by id in the previousHash of the CurrentObjs are toBeDeleted
            deletedObjs = pastObjs_dict.Keys.Except(currObjs_dict.Keys)
                .Where(k => pastObjs_dict.ContainsKey(k)).Select(k => pastObjs_dict[k]).ToList();

            return new Diff(newObjs, deletedObjs, modifiedObjs, diffConfig, objModifiedProps, unChanged);
        }
    }
}

