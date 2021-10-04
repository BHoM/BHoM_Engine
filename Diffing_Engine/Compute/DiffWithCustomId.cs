/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.Engine.Base;

namespace BH.Engine.Diffing
{
    public static partial class Compute
    {
        [Description("Computes the Diffing for BHoMObjects based on an id stored in their CustomData.")]
        [Input("pastObjects", "A set of objects coming from a past revision")]
        [Input("currentObjects", "A set of objects coming from a following Revision")]
        [Input("customdataIdKey", "Name of the key where the Id of the objects may be found in the BHoMObjects' CustomData. The diff will be attempted using the Ids found there." +
            "\nE.g. 'Revit_UniqueId' may be used; an id must be stored under object.CustomData['Revit_UniqueId'].")]
        [Input("diffConfig", "Sets configs such as properties to be ignored in the diffing, or enable/disable property-by-property diffing.")]
        public static Diff DiffWithCustomId(IEnumerable<IBHoMObject> pastObjects, IEnumerable<IBHoMObject> currentObjects, string customdataIdKey, DiffingConfig diffConfig = null)
        {
            // Set configurations if diffConfig is null. Clone it for immutability in the UI.
            DiffingConfig diffConfigCopy = diffConfig == null ? new DiffingConfig() : (DiffingConfig)diffConfig.DeepClone();

            HashSet<string> currentObjectsIds = new HashSet<string>();
            HashSet<string> pastObjectsIds = new HashSet<string>();

            // Verifies inputs and populates the id lists.
            ProcessObjectsForDiffing(pastObjects, currentObjects, customdataIdKey, out currentObjectsIds, out pastObjectsIds);

            // Actual diffing
            // Clone for immutability in the UI
            List<IBHoMObject> currentObjs = currentObjects.ToList();
            List<IBHoMObject> pastObjs = pastObjects.ToList();

            // Make dictionary with object ids to speed up the next lookups
            Dictionary<string, IBHoMObject> currObjs_dict = currentObjectsIds.Zip(currentObjs, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
            Dictionary<string, IBHoMObject> pastObjs_dict = pastObjectsIds.Zip(pastObjs, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

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
                // Let's see if it was modified or not.
                if (diffConfig.EnablePropertyDiffing)
                {
                    // If we are also asking for what properties changed, let's rely on DifferentProperties to see whether the object changed or not.
                    
                    // Determine the changed properties.
                    var differentProps = Query.DifferentProperties(currentObj, correspondingObj, diffConfigCopy);

                    if (differentProps != null && differentProps.Count > 0)
                    {
                        // It's been modified
                        modifiedObjs.Add(currentObj);
                        objModifiedProps.Add(currentObjID, differentProps);

                    }
                    else
                    {
                        // It's NOT been modified
                        if (diffConfigCopy.IncludeUnchangedObjects)
                            unChanged.Add(currentObj);
                    }
                }
                else
                {
                    // Rely on the objects hash to see if they are different.

                    string currentObjHash = currentObj.Hash(diffConfigCopy.ComparisonConfig);
                    string correspondingObjHash = correspondingObj.Hash(diffConfigCopy.ComparisonConfig);

                    if (currentObjHash != correspondingObjHash)
                    {
                        // It's been modified
                        modifiedObjs.Add(currentObj);
                    }
                    else
                    {
                        // It's NOT been modified
                        if (diffConfigCopy.IncludeUnchangedObjects)
                            unChanged.Add(currentObj);
                    }

                }
            }

            // If no modified property was found, set the field to null (otherwise will get empty list)
            objModifiedProps = objModifiedProps.Count == 0 ? null : objModifiedProps;

            // All PastObjects that cannot be found by id in the CurrentObjs are old.
            deletedObjs = pastObjs_dict.Keys.Except(currObjs_dict.Keys)
                .Select(k => pastObjs_dict[k]).ToList();

            if (!newObjs.Any() && !deletedObjs.Any() && !modifiedObjs.Any())
            {
                BH.Engine.Reflection.Compute.RecordWarning($"No difference could be found." +
                    $"\nThe provided Id of the objects were completely different between {nameof(pastObjects)} and {nameof(currentObjects)}." +
                    $"\nPlease make sure that:" +
                    $"\n\t * The input objects constitute the entirety of the model that changed between revisions;" +
                    $"\n\t * the input objects come from models that were not completely re-created between revisions.");
            }
            else if (!diffConfig.EnablePropertyDiffing)
                BH.Engine.Reflection.Compute.RecordWarning($"For this Diffing method to detect modified/unchanged objects, you need to set '{nameof(DiffingConfig.EnablePropertyDiffing)}' to true in the DiffingConfig.");

            return new Diff(newObjs, deletedObjs, modifiedObjs, diffConfigCopy, objModifiedProps, unChanged);
        }

        private static bool ProcessObjectsForDiffing(IEnumerable<IBHoMObject> pastObjects, IEnumerable<IBHoMObject> currentObjects, string customdataIdKey, out HashSet<string> out_currentObjectsIds, out HashSet<string> out_pastObjectsIds)
        {
            // Output ids
            out_currentObjectsIds = new HashSet<string>();
            out_pastObjectsIds = new HashSet<string>();

            HashSet<string> currentObjectsIds = new HashSet<string>();
            HashSet<string> pastObjectsIds = new HashSet<string>();

            // Check on customDataKey
            if (string.IsNullOrWhiteSpace(customdataIdKey))
            {
                BH.Engine.Reflection.Compute.RecordError($"Invalid {nameof(customdataIdKey)} provided.");
                return false;
            }

            // Flags
            bool allRetrieved = true;
            bool noDuplicates = true;

            // Retrieve Id from CustomData for current objects
            currentObjects.ToList().ForEach(o =>
            {
                object id = null;
                allRetrieved &= o.CustomData.TryGetValue(customdataIdKey, out id);
                currentObjectsIds.Add(id?.ToString());
            });

            // Checks on current Objects
            if (!allRetrieved)
                BH.Engine.Reflection.Compute.RecordWarning($"Some or all of the {nameof(currentObjects)}' CustomData do not contain a key `{customdataIdKey}`.");

            if (allRetrieved && currentObjectsIds.Count != currentObjects.Count())
            {
                BH.Engine.Reflection.Compute.RecordWarning($"Some of the {nameof(currentObjects)} have duplicate Id.");
                noDuplicates = false;
            }

            // Retrieve Id from CustomData for past objects
            pastObjects.ToList().ForEach(o =>
            {
                object id = null;
                allRetrieved &= o.CustomData.TryGetValue(customdataIdKey, out id);
                pastObjectsIds.Add(id?.ToString());
            });

            // Checks on past Objects
            if (!allRetrieved)
                BH.Engine.Reflection.Compute.RecordWarning($"Some or all of the {nameof(pastObjects)}' CustomData do not contain a key `{customdataIdKey}`.");

            if (allRetrieved && pastObjectsIds.Count != pastObjects.Count())
            {
                BH.Engine.Reflection.Compute.RecordError($"Some of the {nameof(pastObjects)} have duplicate Id.");
                noDuplicates = false;
            }

            if (!allRetrieved || !noDuplicates)
                return false;

            out_currentObjectsIds = currentObjectsIds;
            out_pastObjectsIds = pastObjectsIds;

            return true;
        }
    }
}


