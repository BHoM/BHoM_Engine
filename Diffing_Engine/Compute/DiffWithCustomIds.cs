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
        [Description("Computes the Diffing for BHoMObjects based on the input Id lists.")]
        [Input("pastObjects", "A set of objects coming from a past revision.")]
        [Input("pastObjectsIds", "The Ids corresponding to the input pastObjects. This Id will be used for Diffing by matching them with the following Objects Ids.")]
        [Input("followingObjects", "A set of objects coming from a following Revision.")]
        [Input("followingObjectsIds", "The Ids corresponding to the input followingObjs. This Id will be used for Diffing by matching them with the past Objects Ids.")]
        [Input("diffingConfig", "Sets configs such as properties to be ignored in the diffing, or enable/disable property-by-property diffing.")]
        [Output("diff", "Object holding the detected changes.")]
        public static Diff DiffWithCustomIds(List<object> pastObjects, List<string> pastObjectsIds, List<object> followingObjects, List<string> followingObjectsIds, DiffingConfig diffingConfig = null)
        {
            Diff outputDiff = null;
            if (InputObjectsNullOrEmpty(pastObjects, followingObjects, out outputDiff, diffingConfig))
                return outputDiff;

            // Set configurations if diffConfig is null. Clone it for immutability in the UI.
            DiffingConfig diffConfigCopy = diffingConfig == null ? new DiffingConfig() : (DiffingConfig)diffingConfig.DeepClone();

            outputDiff = Diffing(pastObjects, pastObjectsIds, followingObjects, followingObjectsIds, diffConfigCopy);

            return outputDiff;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        // Main method to perform the Diffing. Most of the public Diffing methods will call this.
        // "pastObjects": Objects that were created before "followingObjects".
        // "pastObjectsIds": Ids of the "pastObjects" that should be used to match them with the "followingObjects". If no match is found, a pastObject is identified as "deleted".
        // "followingObjects": Objects that were created after "pastObjects".
        // "followingObjectsIds": Ids of the "followingObjects" that should be used to match them with the "pastObjects". If no match is found, a followingObject is identified as "added".
        // "diffingConfig": Additional configurations.
        // "recordEvents": Because this method can be called from many different places, some Notes/Warnings/Errors may not be relevant in all cases, so we give the option to turn them off.
        private static Diff Diffing(IEnumerable<object> pastObjects, IEnumerable<string> pastObjectsIds, IEnumerable<object> followingObjects, IEnumerable<string> followingObjectsIds, DiffingConfig diffingConfig = null, bool recordEvents = true)
        {
            // Null guards.
            if (pastObjects == null) pastObjects = new List<object>();
            if (pastObjectsIds == null) pastObjectsIds = new List<string>();
            if (followingObjects == null) followingObjects = new List<object>();
            if (followingObjectsIds == null) followingObjectsIds = new List<string>();
            if (diffingConfig == null) diffingConfig = new DiffingConfig();

            if (!diffingConfigCopy.AllowDuplicateIds)
            {
                // Get the distinct Ids from the input Id lists (do not admit duplicates).
                HashSet<string> pastObjsIdsDistinct = new HashSet<string>(pastObjectsIds);
                HashSet<string> follObjsIdsDistinct = new HashSet<string>(followingObjectsIds);
                if (pastObjsIdsDistinct.Count() != pastObjectsIds.Count())
                    if (recordEvents) BH.Engine.Reflection.Compute.RecordWarning($"Some of the input {pastObjectsIds} were duplicate.");

                if (follObjsIdsDistinct.Count() != followingObjectsIds.Count())
                    if (recordEvents) BH.Engine.Reflection.Compute.RecordWarning($"Some of the input {followingObjectsIds} were duplicate.");

                // Replace the input Id collections with their distinct version.
                pastObjectsIds = pastObjsIdsDistinct.ToList();
                followingObjectsIds = follObjsIdsDistinct.ToList();
            }

            // Check if input objects and correspondent Id lists are of equal size.
            if (pastObjects.Count() != pastObjectsIds.Count())
            {
                if (recordEvents) BH.Engine.Reflection.Compute.RecordError($"The number of input `{nameof(pastObjects)}` must be the same as the number of input `{nameof(pastObjectsIds)}`.");
                return null;
            }

            if (followingObjects.Count() != followingObjectsIds.Count())
            {
                if (recordEvents) BH.Engine.Reflection.Compute.RecordError($"The number of input `{nameof(followingObjects)}` must be the same as the number of input `{nameof(followingObjectsIds)}`.");
                return null;
            }

            // Make dictionary with object ids to speed up/simplify the lookups.
            Dictionary<string, object> pastObjs_dict = pastObjectsIds.Zip(pastObjects, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
            Dictionary<string, object> follObjs_dict = followingObjectsIds.Zip(followingObjects, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

            // Dispatch the objects: new, modified or deleted
            List<object> addedObjs = new List<object>();
            List<object> modifiedObjs = new List<object>();
            List<object> removedObjs = new List<object>();
            List<object> unchangedObjs = new List<object>();
            Dictionary<string, Dictionary<string, Tuple<object, object>>> objModifiedProps = new Dictionary<string, Dictionary<string, Tuple<object, object>>>();

            foreach (var kv_curr in follObjs_dict)
            {
                object currentObj = kv_curr.Value;
                string currentObjID = kv_curr.Key;

                // Try to find an object between the pastObjs that has the same ID of the current one.
                object correspondingObj = null;
                pastObjs_dict.TryGetValue(kv_curr.Key, out correspondingObj);

                // If none is found, the current object is new.
                if (correspondingObj == null)
                {
                    addedObjs.Add(kv_curr.Value);
                    continue;
                }

                // Otherwise, the current object existed in the past set.
                // Let's see if it was modified or not.
                if (diffingConfigCopy.EnablePropertyDiffing)
                {
                    // If we are also asking for what properties changed, let's rely on DifferentProperties to see whether the object changed or not.

                    // Determine the changed properties.
                    var differentProps = Query.DifferentProperties(currentObj, correspondingObj, diffingConfigCopy);

                    if (differentProps != null && differentProps.Count > 0)
                    {
                        // It's been modified
                        modifiedObjs.Add(currentObj);
                        objModifiedProps.Add(currentObjID, differentProps);
                    }
                    else
                    {
                        // It's NOT been modified
                        if (diffingConfigCopy.IncludeUnchangedObjects)
                            unchangedObjs.Add(currentObj);
                    }
                }
                else
                {
                    // If they are BHoMObjects, let's first rely on the BHoMobject hash to see if they are different.
                    // Relying on the Hashes first allows to use the same ComparisonConfig in both the Hash Computation and the DifferentProperties() method.
                    // This way, users can control how the hash is computed, and therefore effectively what properties need to be included, ignored, etc (all the settings in ComparisonConfig).
                    IBHoMObject currentBHoMObj = currentObj as IBHoMObject;
                    IBHoMObject correspondingBHoMObj = correspondingObj as IBHoMObject;
                    string currentObjHash, correspondingObjHash;

                    if (currentBHoMObj != null && correspondingBHoMObj != null)
                    {
                        // Compute the hash of both objects and compare them.
                        currentObjHash = currentBHoMObj.Hash(diffingConfigCopy.ComparisonConfig);
                        correspondingObjHash = correspondingBHoMObj.Hash(diffingConfigCopy.ComparisonConfig);

                        if (currentObjHash != correspondingObjHash)
                        {
                            // It's been modified
                            modifiedObjs.Add(currentObj);
                        }
                        else
                        {
                            // It's NOT been modified
                            if (diffingConfigCopy.IncludeUnchangedObjects)
                                unchangedObjs.Add(currentObj);
                        }
                    }
                    else
                    {
                        // If the objects are non-BHoMObjects, we cannot use the Hash, but we can always rely on the object comparison to see if they are different.
                        if (!currentObj.Equals(correspondingObj))
                        {
                            // It's been modified
                            modifiedObjs.Add(currentObj);
                        }
                        else
                        {
                            // It's NOT been modified
                            if (diffingConfigCopy.IncludeUnchangedObjects)
                                unchangedObjs.Add(currentObj);
                        }
                    }
                }
            }

            // If no modifed property was found, set the field to null (otherwise will get empty list)
            objModifiedProps = !objModifiedProps.Any() ? null : objModifiedProps;

            // All PastObjects that cannot be found by id in the CurrentObjs are old.
            removedObjs = pastObjs_dict.Keys.Except(follObjs_dict.Keys)
                .Select(k => pastObjs_dict[k]).ToList();

            // Add user warnings/notes for specific usage scenarios.
            if (!followingObjectsIds.Intersect(pastObjectsIds).Any())
            {
                // If there is no overlap in the keys between the two sets, no "modified" object can have been detected.
                // This could be either because there are truly no modified objects, or more likely because the user has input objects that do not have a valid Id assigned. 

                if (!addedObjs.Any() && !removedObjs.Any())
                {
                    // If no Added/Removed objs were found, then the input objects had no valid Ids. Return Error.
                    if (recordEvents) BH.Engine.Reflection.Compute.RecordError(
                        $"\nSome or all of the input objects had no valid ID/Key usable for diffing." +
                        $"\nPlease make sure that:" +
                        $"\n\t * all the objects that changed between revisions were included in the input;" +
                        $"\n\t * you specified valid IDs to be used for Diffing; or you specified a valid Property/Fragment/CustomData Key that stores the Ids to be fetched from the objects.");

                    return null;
                }
                else // If some Added/Removed objs were found, then simply add a Warning to inform the user that the Id used for the Diffing may have been invalid.
                    if (recordEvents) BH.Engine.Reflection.Compute.RecordWarning(
                        $"\nThe {nameof(pastObjects)} and {nameof(followingObjects)} were either identical or completely different." +
                        $"\nPlease make sure that:" +
                        $"\n\t * all the objects that changed between revisions were included in the input;" +
                        $"\n\t * you specified valid IDs to be used for Diffing; or you specified a valid Property/Fragment/CustomData Key that stores the Ids to be fetched from the objects." +
                        $"\nThis can also happen if the input objects come from models that were completely re-created between revisions (i.e. their IDs are completely different). In this latter case, the Diffing worked successfully but you may want to use a different ID.");
            }

            if (followingObjectsIds.Intersect(pastObjectsIds).Any() && diffingConfigCopy.ComparisonConfig.PropertiesToConsider.Any() && !modifiedObjs.Any())
            {
                // If no modified object was found and some PropertiesToConsider was specified,
                // add a Note to remind the user that if no differences were found it's probably because of that.
                if (recordEvents) BH.Engine.Reflection.Compute.RecordNote(
                    $"No {nameof(BH.oM.Diffing.Diff.ModifiedObjects)} were found. Make sure that the specified `{nameof(DiffingConfig)}.{nameof(DiffingConfig.ComparisonConfig)}.{nameof(DiffingConfig.ComparisonConfig.PropertiesToConsider)}` is set correctly.");
            }

            return new Diff(addedObjs, removedObjs, modifiedObjs, diffingConfigCopy, objModifiedProps, unchangedObjs);
        }
    }
}
