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
        [Description("Dispatches to the most appropriate Diffing method, depending on the provided inputs.")]
        public static Diff IDiffing(IEnumerable<object> pastObjs, IEnumerable<object> followingObjs, DiffingType diffingType = DiffingType.Automatic, DiffingConfig diffConfig = null)
        {
            Diff outputDiff = null;
            if (!pastObjs.Any() && !followingObjs.Any())
                return null;

            // Set configurations if diffConfig is null. Clone it for immutability in the UI.
            DiffingConfig dc = diffConfig == null ? new DiffingConfig() : diffConfig.DeepClone();

            // If requested, compute the Diffing comparing each object one by one, in the same order.
            if (diffingType == DiffingType.OneByOne)
            {
                // If objects do not have any persistentId, `AllowOneByOneDiffing` is enabled and the collections have the same length,
                // compare objects from the two collections one by one.

                BH.Engine.Reflection.Compute.RecordNote($"Calling the diffing method '{nameof(DiffOneByOne)}'" +
                    $"\nThis will only identify 'modified' or 'unchanged' objects. It will work correctly only if the input objects are in the same order.");

                return DiffOneByOne(pastObjs, followingObjs, dc);
            }

            Dictionary<string, List<IBHoMObject>> pastBHoMObjs_perNamespace = pastObjs.OfType<IBHoMObject>().GroupBy(obj => obj.GetType().Namespace).ToDictionary(g => g.Key, g => g.ToList());
            Dictionary<string, List<IBHoMObject>> followingBHoMObjs_perNamespace = followingObjs.OfType<IBHoMObject>().GroupBy(obj => obj.GetType().Namespace).ToDictionary(g => g.Key, g => g.ToList());

            Dictionary<string, MethodBase> adaptersDiffingMethods_perNamespace = Query.AdaptersDiffingMethods_perNamespace();
            List<string> adaptersDiffingMethods_modifiedNamespaces = adaptersDiffingMethods_perNamespace.Keys.Select(k => k.Replace("Engine", "oM")).ToList();

            List<string> commonOmNameSpaces = pastBHoMObjs_perNamespace.Keys.Intersect(followingBHoMObjs_perNamespace.Keys).ToList();
            commonOmNameSpaces = commonOmNameSpaces
                .Where(cns => adaptersDiffingMethods_modifiedNamespaces.Any(n => cns.Contains(n))).ToList();

            if (commonOmNameSpaces?.Any() ?? false)
            {
                foreach (string commonOmNameSpace in commonOmNameSpaces)
                {
                    string adapterDiffMethodNamespace = adaptersDiffingMethods_perNamespace.Keys.Where(k => commonOmNameSpace.Replace("oM", "Engine").StartsWith(k)).FirstOrDefault();

                    if (adapterDiffMethodNamespace.IsNullOrEmpty())
                        continue;

                    MethodBase adapterDiffMethodToInvoke = adaptersDiffingMethods_perNamespace[adapterDiffMethodNamespace];
                    List<ParameterInfo> parameterInfos = adapterDiffMethodToInvoke.GetParameters().ToList();
                    int numberOfOptionalParams = parameterInfos.Where(p => p.IsOptional).Count();
                    int indexOfDiffConfigParam = parameterInfos.IndexOf(parameterInfos.First(pi => pi.ParameterType == typeof(DiffingConfig)));
                    var parameters = new List<object>() { pastBHoMObjs_perNamespace[commonOmNameSpace], followingBHoMObjs_perNamespace[commonOmNameSpace] };
                    parameters.AddRange(Enumerable.Repeat(Type.Missing, numberOfOptionalParams - 1));
                    parameters.Insert(indexOfDiffConfigParam, dc);

                    BH.Engine.Reflection.Compute.RecordNote($"Invoking Diffing method `{adapterDiffMethodToInvoke.DeclaringType.FullName}.{adapterDiffMethodToInvoke.Name}` on the input objects belonging to namespace {commonOmNameSpace}.");

                    Diff result = adapterDiffMethodToInvoke.Invoke(null, parameters.ToArray()) as Diff;
                    outputDiff = outputDiff.CombineDiffs(result);

                    // Remove all objs that were found in common namespace. The remaining have still to be diffed.
                    pastObjs = pastObjs.Except(pastBHoMObjs_perNamespace[commonOmNameSpace]);
                    followingObjs = followingObjs.Except(followingBHoMObjs_perNamespace[commonOmNameSpace]);
                }
            }

            if (!pastObjs.Any() && !followingObjs.Any())
                return outputDiff;

            // Check if the inputs specified are Revisions. In that case, use the Diffing-Revision workflow.
            if (diffingType == DiffingType.Automatic || diffingType == DiffingType.Revision)
            {
                if (pastObjs.Count() == 1 && followingObjs.Count() == 1)
                {
                    Revision pastRev = pastObjs.First() as Revision;
                    Revision follRev = followingObjs.First() as Revision;

                    if (pastRev != null && follRev != null)
                    {
                        BH.Engine.Reflection.Compute.RecordNote($"Calling the diffing method '{nameof(DiffRevisions)}'.");

                        if (!string.IsNullOrWhiteSpace(dc.CustomDataKey))
                            BH.Engine.Reflection.Compute.RecordWarning($"The `{nameof(DiffingConfig)}.{nameof(dc.CustomDataKey)}` is not considered when the input objects are both of type {nameof(Revision)}.");

                        Diff result = DiffRevisions(pastRev, follRev, dc);

                        return outputDiff.CombineDiffs(result);
                    }
                }

                if (diffingType == DiffingType.Revision)
                    return DiffingError(diffingType);
            }

            IEnumerable<IBHoMObject> bHoMObjects_past = pastObjs.OfType<IBHoMObject>();
            IEnumerable<IBHoMObject> bHoMObjects_following = followingObjs.OfType<IBHoMObject>();

            // Check if the BHoMObjects all have a RevisionFragment assigned.
            // If so, we may attempt the Revision diffing.
            if (bHoMObjects_past.AllHaveRevisionFragment() && bHoMObjects_following.AllHaveRevisionFragment())
            {
                BH.Engine.Reflection.Compute.RecordNote($"Calling the diffing method '{nameof(DiffRevisionObjects)}'.");
                return DiffRevisionObjects(bHoMObjects_past, bHoMObjects_following, dc);
            }

            // If a customDataKey was specified, use the Id found under that key in customdata to perform the Diffing.
            if (diffingType == DiffingType.Automatic || diffingType == DiffingType.CustomDataId)
            {
                if (diffingType == DiffingType.CustomDataId && !string.IsNullOrWhiteSpace(dc.CustomDataKey))
                    return DiffingError(diffingType);

                if (!string.IsNullOrWhiteSpace(dc.CustomDataKey))
                {
                    BH.Engine.Reflection.Compute.RecordNote($"A {nameof(DiffingConfig.CustomDataKey)} was found on the input {nameof(DiffingConfig)}. Therefore, attempting to call the diffing method '{nameof(DiffWithCustomId)}'");

                    if (bHoMObjects_past.Count() != pastObjs.Count() && bHoMObjects_following.Count() != followingObjs.Count())
                        BH.Engine.Reflection.Compute.RecordNote($"To perform the diffing based on an Id stored in the Custom Data, the inputs collections must contain exclusively objects implementing IBHoMObject.");
                    else
                    {
                        Diff result = DiffWithCustomId(bHoMObjects_past, bHoMObjects_following, dc.CustomDataKey, dc);

                        return outputDiff.CombineDiffs(result);
                    }
                }
            }

            // Check if the bhomObjects have a persistentId assigned.
            List<object> reminder_past;
            List<object> reminder_following;
            List<IBHoMObject> bHoMObjects_past_persistId = bHoMObjects_past.WithNonNullPersistentAdapterId(out reminder_past);
            List<IBHoMObject> bHoMObjects_following_persistId = bHoMObjects_following.WithNonNullPersistentAdapterId(out reminder_following);
            Diff diffGeneric = null;
            Diff fragmentDiff = null;

            // For the BHoMObjects we can compute the Diff with the persistentId.
            if (diffingType == DiffingType.Automatic || diffingType == DiffingType.PersistentId)
            {
                if (bHoMObjects_past_persistId.Count != 0 && bHoMObjects_following_persistId.Count != 0)
                {
                    BH.Engine.Reflection.Compute.RecordNote($"Calling the diffing method '{nameof(DiffWithFragmentId)}'.");
                    fragmentDiff = DiffWithFragmentId(bHoMObjects_past_persistId, bHoMObjects_following_persistId, typeof(IPersistentAdapterId), nameof(IPersistentAdapterId.PersistentId), dc);
                }
            }

            if (reminder_past.Any() || reminder_following.Any())
            {
                // For the remaining objects (= all objects that are not BHoMObjects, and all BHoMObjects not having a PersistentId) we can Diff using the Hash.
                BH.Engine.Reflection.Compute.RecordNote($"Calling the most generic Diffing method, '{nameof(DiffWithHash)}'.");
                diffGeneric = DiffWithHash(pastObjs as dynamic, followingObjs as dynamic, dc);
            }

            return outputDiff.CombineDiffs(fragmentDiff.CombineDiffs(diffGeneric));
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Diff DiffingError(DiffingType diffingType)
        {
            BH.Engine.Reflection.Compute.RecordError($"Invalid inputs for the selected DiffingType `{diffingType}`.");
            return null;
        }
    }
}

