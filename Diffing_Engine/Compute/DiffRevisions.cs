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
        [Description("Computes the diffing for Revisions containing objects of any type (also non-BHoMObjects).")]
        [Input("pastRevision", "A past Revision. It must have been created before the 'followingRevision'.")]
        [Input("followingRevision", "A following Revision. It must have been created after 'pastRevision'.")]
        [Input("DiffingConfig", "Sets configs such as properties to be ignored in the diffing, or enable/disable property-by-property diffing.")]
        public static Diff DiffRevisions(Revision pastRevision, Revision followingRevision, DiffingConfig diffingConfig = null)
        {
            if(pastRevision == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot compute the diff between revisions when the past revision is null.");
                return null;
            }

            if(followingRevision == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot compute the diff between revisions when the following revision is null.");
                return null;
            }

            return DiffRevisionObjects(pastRevision.Objects, followingRevision.Objects, diffingConfig);
        }

        // Computes the diffing for IEnumerable<object>.
        // For BHoMObjects, it assumes that they all have a HashFragment assigned (like when they have been passed through a Revision).
        // For non-BHoMObjects, it performs the VennDiagram comparision with a HashComparer. 
        // Results for BHoMObjects and non are concatenated.
        private static Diff DiffRevisionObjects(IEnumerable<object> pastRevisionObjs, IEnumerable<object> followingRevisionObjs, DiffingConfig diffingConfig = null)
        {
            // Set configurations if DiffingConfig is null. Clone it for immutability in the UI.
            DiffingConfig diffConfigCopy = diffingConfig == null ? new DiffingConfig() : diffingConfig.DeepClone() as DiffingConfig;

            // Dispatch the objects in BHoMObjects and generic objects.
            IEnumerable<IBHoMObject> prevObjs_BHoM = pastRevisionObjs.OfType<IBHoMObject>();
            IEnumerable<IBHoMObject> currObjs_BHoM = followingRevisionObjs.OfType<IBHoMObject>();
            IEnumerable<object> prevObjs_nonBHoM = pastRevisionObjs.Where(o => !(o is IBHoMObject));
            IEnumerable<object> currObjs_nonBHoM = followingRevisionObjs.Where(o => !(o is IBHoMObject));

            // Compute the specific Diffing for the BHoMObjects.
            Diff diff = Compute.DiffRevisionObjects(prevObjs_BHoM, currObjs_BHoM, diffConfigCopy);

            // If all objects are BHoMObjects, we are done.
            if (pastRevisionObjs.Count() != 0 && pastRevisionObjs.Count() == prevObjs_BHoM.Count() && followingRevisionObjs.Count() == currObjs_BHoM.Count())
                return diff;

            // Compute the generic Diffing for the other objects.
            // This is left to the VennDiagram with a HashComparer.
            VennDiagram<object> vd = Engine.Data.Create.VennDiagram(prevObjs_nonBHoM, currObjs_nonBHoM, new HashComparer<object>(diffingConfig.ComparisonConfig));

            // Concatenate the results of the two diffing operations.
            List<object> allPrevObjs = new List<object>();
            List<object> allCurrObjs = new List<object>();
            List<object> allUnchangedObjs = new List<object>();

            allCurrObjs.AddRange(diff.AddedObjects);
            allCurrObjs.AddRange(vd.OnlySet1);

            allPrevObjs.AddRange(diff.RemovedObjects);
            allPrevObjs.AddRange(vd.OnlySet2);

            // Create the final, actual diff.
            Diff finalDiff = new Diff(allCurrObjs, allPrevObjs, diff.ModifiedObjects, diffConfigCopy, diff.ModifiedPropsPerObject, diff.UnchangedObjects);

            return finalDiff;
        }

        // Computes the Diffing for BHoMObjects that all have a HashFragment assigned (like when they have been passed through a Revision).
        private static Diff DiffRevisionObjects(IEnumerable<IBHoMObject> pastObjects, IEnumerable<IBHoMObject> currentObjects, DiffingConfig diffingConfig = null)
        {
            // Set configurations if DiffingConfig is null. Clone it for immutability in the UI.
            DiffingConfig dc = diffingConfig == null ? new DiffingConfig() : diffingConfig.DeepClone() as DiffingConfig;

            // Take the Revision's objects
            List<IBHoMObject> currentObjs = currentObjects.ToList();
            List<IBHoMObject> readObjs = pastObjects.ToList();

            // Make dictionary with object hashes to speed up the next lookups
            Dictionary<string, IBHoMObject> readObjs_dict = readObjs.ToDictionary(obj => obj.RevisionFragment().Hash, obj => obj);

            // Dispatch the objects: new, modified or old
            List<IBHoMObject> newObjs = new List<IBHoMObject>();
            List<IBHoMObject> modifiedObjs = new List<IBHoMObject>();
            List<IBHoMObject> oldObjs = new List<IBHoMObject>();
            List<IBHoMObject> unChanged = new List<IBHoMObject>();

            var objModifiedProps = new Dictionary<string, Dictionary<string, Tuple<object, object>>>();

            foreach (IBHoMObject bhomObj in currentObjs)
            {
                RevisionFragment revisionFragm = bhomObj.RevisionFragment();

                if (revisionFragm?.PreviousHash == null)
                {
                    newObjs.Add(bhomObj); // It's a new object
                }

                else if (revisionFragm.PreviousHash == revisionFragm.Hash)
                {
                    // It's NOT been modified
                    if (dc.IncludeUnchangedObjects)
                        unChanged.Add(bhomObj);
                }

                else if (revisionFragm.PreviousHash != revisionFragm.Hash)
                {
                    modifiedObjs.Add(bhomObj); // It's been modified

                    if (dc.EnablePropertyDiffing)
                    {
                        // Determine changed properties
                        IBHoMObject oldBhomObj = null;
                        readObjs_dict.TryGetValue(revisionFragm.PreviousHash, out oldBhomObj);

                        if (oldBhomObj == null) continue;

                        // To compute differentProps in a Revision-Diffing, make sure we remove the RevisionFragment. We don't want to consider that.
                        var differentProps = Query.DifferentProperties(bhomObj.RemoveFragment(typeof(RevisionFragment)), oldBhomObj.RemoveFragment(typeof(RevisionFragment)), dc);

                        if (differentProps != null)
                            objModifiedProps.Add(revisionFragm.Hash, differentProps);
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
                  .Where(obj => obj.RevisionFragment().PreviousHash != null)
                  .ToDictionary(obj => obj.RevisionFragment().PreviousHash, obj => obj);

            oldObjs = readObjs_dict.Keys.Except(CurrentObjs_withPreviousHash_dict.Keys)
                .Where(k => readObjs_dict.ContainsKey(k)).Select(k => readObjs_dict[k]).ToList();

            return new Diff(newObjs, oldObjs, modifiedObjs, diffingConfig, objModifiedProps, unChanged);
        }

        private static bool AllHaveRevisionFragment(this IEnumerable<IBHoMObject> bHoMObjects)
        {
            // Check if objects have hashfragment.
            if (bHoMObjects == null 
                || bHoMObjects.Count() == 0 
                || bHoMObjects.Select(o => o.RevisionFragment()).Where(o => o != null).Count() < bHoMObjects.Count())
                    return false;

            return true;
        }

        private static List<IBHoMObject> WithNonNullPersistentAdapterId(this IEnumerable<object> objects, out List<object> reminder)
        {
            List<IBHoMObject> output = new List<IBHoMObject>();
            reminder = new List<object>();

            foreach (var obj in objects)
            {
                IBHoMObject ibhomobject  = obj as IBHoMObject;
                if (ibhomobject != null && ibhomobject.FindFragment<IPersistentAdapterId>()?.PersistentId != null)
                    output.Add(ibhomobject);
                else
                    reminder.Add(obj);
            }

            return output;
        }
    }
}


