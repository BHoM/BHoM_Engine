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
        [Input("pastObjs", "Set of objects belonging to a past (previous) revision.")]
        [Input("followingObjs", "Set of objects belonging to a following revision.")]
        [Input("diffingType", "(Optional) Defaults to Automatic. Allows to choose between different kinds of Diffing.")]
        [Input("diffConfig", "(Optional) Additional settings for the Diffing.")]
        [Output("diff", "Object holding the detected changes.")]
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

            Dictionary<string, MethodBase> adaptersDiffingMethods_perNamespace = AdaptersDiffingMethods_perNamespace();
            List<string> adaptersDiffingMethods_modifiedNamespaces = adaptersDiffingMethods_perNamespace.Keys.Select(k => k.Replace("Engine", "oM")).ToList();

            List<string> commonObjectNameSpaces = pastBHoMObjs_perNamespace.Keys.Intersect(followingBHoMObjs_perNamespace.Keys).ToList();
            commonObjectNameSpaces = commonObjectNameSpaces
                .Where(cns => adaptersDiffingMethods_modifiedNamespaces.Any(n => cns.Contains(n))).ToList();

            bool performedToolkitDiffing = false;

            foreach (string commonObjectNameSpace in commonObjectNameSpaces)
            {
                string adapterDiffMethodNamespace = adaptersDiffingMethods_perNamespace.Keys.Where(k => commonObjectNameSpace.Replace("oM", "Engine").StartsWith(k)).FirstOrDefault();

                if (adapterDiffMethodNamespace.IsNullOrEmpty())
                    continue;

                MethodBase adapterDiffMethodToInvoke = adaptersDiffingMethods_perNamespace[adapterDiffMethodNamespace];
                List<ParameterInfo> parameterInfos = adapterDiffMethodToInvoke.GetParameters().ToList();
                int numberOfOptionalParams = parameterInfos.Where(p => p.IsOptional).Count();
                int indexOfDiffConfigParam = parameterInfos.IndexOf(parameterInfos.First(pi => pi.ParameterType == typeof(DiffingConfig)));
                var parameters = new List<object>() { pastBHoMObjs_perNamespace[commonObjectNameSpace], followingBHoMObjs_perNamespace[commonObjectNameSpace] };
                parameters.AddRange(Enumerable.Repeat(Type.Missing, numberOfOptionalParams - 1));
                parameters.Insert(indexOfDiffConfigParam, dc);

                BH.Engine.Reflection.Compute.RecordNote($"Invoking Diffing method `{adapterDiffMethodToInvoke.DeclaringType.FullName}.{adapterDiffMethodToInvoke.Name}` on the input objects belonging to namespace {commonObjectNameSpace}.");

                Diff result = adapterDiffMethodToInvoke.Invoke(null, parameters.ToArray()) as Diff;
                outputDiff = outputDiff.CombineDiffs(result);

                // Remove all objs that were found in common namespace. The remaining have still to be diffed.
                pastObjs = pastObjs.Except(pastBHoMObjs_perNamespace[commonObjectNameSpace]);
                followingObjs = followingObjs.Except(followingBHoMObjs_perNamespace[commonObjectNameSpace]);

                performedToolkitDiffing = true;
            }

            if (!pastObjs.Any() && !followingObjs.Any())
                return outputDiff;

            if (performedToolkitDiffing)
                BH.Engine.Reflection.Compute.RecordNote("Continuing the Diffing procedure with the remaining objects.");

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
            List<object> remainder_past;
            List<object> remainder_following;
            List<IBHoMObject> bHoMObjects_past_persistId = bHoMObjects_past.WithCommonPersistentAdapterId(out remainder_past);
            List<IBHoMObject> bHoMObjects_following_persistId = bHoMObjects_following.WithCommonPersistentAdapterId(out remainder_following);
            Diff diffGeneric = null;
            Diff fragmentDiff = null;

            // For the BHoMObjects having a common PeristentAdapterId we can compute the Diff by using it.
            if (diffingType == DiffingType.Automatic || diffingType == DiffingType.PersistentId)
            {
                if (bHoMObjects_past_persistId.Count != 0 && bHoMObjects_following_persistId.Count != 0)
                {
                    BH.Engine.Reflection.Compute.RecordNote($"Calling the diffing method '{nameof(DiffWithFragmentId)}'.");
                    fragmentDiff = DiffWithFragmentId(bHoMObjects_past_persistId, bHoMObjects_following_persistId, typeof(IPersistentAdapterId), nameof(IPersistentAdapterId.PersistentId), dc);
                }
            }

            if (remainder_past.Any() || remainder_following.Any())
            {
                // For the remaining objects (= all objects that are not BHoMObjects, and all BHoMObjects not having a PersistentId) we can Diff using the Hash.
                BH.Engine.Reflection.Compute.RecordNote($"Calling the most generic Diffing method, '{nameof(DiffWithHash)}'.");
                diffGeneric = DiffWithHash(pastObjs as dynamic, followingObjs as dynamic, dc);
            }

            return outputDiff.CombineDiffs(fragmentDiff.CombineDiffs(diffGeneric));
        }

        /***************************************************/

        [Description("Dispatches to the most appropriate Diffing method, depending on the provided inputs.")]
        [Input("pastObjs", "Set of objects belonging to a past (previous) revision.")]
        [Input("followingObjs", "Set of objects belonging to a following revision.")]
        [Input("propertiesToConsider", "(Optional) Properties to be considered by the Diffing when determining what objects changed. See the DiffingConfig tooltip for more info.")]
        [Output("diff", "Object holding the detected changes.")]
        public static Diff IDiffing(IEnumerable<object> pastObjs, IEnumerable<object> followingObjs, List<string> propertiesToConsider = null)
        {
            DiffingConfig dc = null;
            if (toConsider?.Any() ?? false)
                dc = new DiffingConfig() { ComparisonConfig = new ComparisonConfig { PropertiesToConsider = toConsider } };

            return IDiffing(pastObjs, followingObjs, DiffingType.Automatic, dc);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Diff DiffingError(DiffingType diffingType)
        {
            BH.Engine.Reflection.Compute.RecordError($"Invalid inputs for the selected DiffingType `{diffingType}`.");
            return null;
        }

        /***************************************************/

        private static Dictionary<string, MethodBase> AdaptersDiffingMethods_perNamespace()
        {
            List<MethodBase> adaptersDiffingMethods = Query.AdaptersDiffingMethods();

            var AdaptersDiffingMethods_GroupedPerNamespace = adaptersDiffingMethods.GroupBy(m => m.DeclaringType.Namespace);

            foreach (var g in AdaptersDiffingMethods_GroupedPerNamespace)
            {
                if (g.Count() > 1)
                {
                    BH.Engine.Reflection.Compute.RecordError($"{g.Count()} Diffing methods found in namespace {g.Key}. Only one is allowed. Returning only the first one.");
                }
            }

            return AdaptersDiffingMethods_GroupedPerNamespace.ToDictionary(g => g.Key, g => g.FirstOrDefault());
        }

        /***************************************************/

        // Finds what objects in the given collection are BHoMObjects and own a PersistentAdapterId fragment of the same type.
        // This is useful to automate the IDiffing.
        private static List<IBHoMObject> WithCommonPersistentAdapterId(this IEnumerable<object> objects, out List<object> remainder)
        {
            IEnumerable<IBHoMObject> allBHoMObjects = objects.OfType<IBHoMObject>();
            remainder = new List<object>();

            Dictionary<Type, List<IBHoMObject>> persistentIdFragmentTypesFound = new Dictionary<Type, List<IBHoMObject>>();

            foreach (var bhomObj in allBHoMObjects)
            {
                var persistentIdFragments = bhomObj.GetAllFragments(typeof(IPersistentAdapterId));

                if (persistentIdFragments.Any())
                    foreach (var fr in persistentIdFragments)
                    {
                        Type frType = fr.GetType();

                        if (!persistentIdFragmentTypesFound.ContainsKey(frType))
                            persistentIdFragmentTypesFound[frType] = new List<IBHoMObject>();

                        persistentIdFragmentTypesFound[frType].Add(bhomObj);
                    }
            }

            // If one or zero persistentId were found on the objects, we can return.
            if (persistentIdFragmentTypesFound.Count <= 1)
            {
                remainder = objects.Except(persistentIdFragmentTypesFound.Values.FirstOrDefault() ?? new List<IBHoMObject>()).ToList();
                return persistentIdFragmentTypesFound.Values.FirstOrDefault() ?? new List<IBHoMObject>();
            }

            // If multiple PersistentId were found on the objects overall,
            // check if we can find exactly one PersistentId fragment of a common type across all BHoMObjects.
            // (in case some PersistentId Fragment is not present across all objects, we can use the one that is present consistently on all).
            Type commonPersistentId = null;
            foreach (var kv in persistentIdFragmentTypesFound)
            {
                Type persistentIdFragment = kv.Key;
                List<IBHoMObject> bhomObjectsWithThisPersistentId = kv.Value;
                if (bhomObjectsWithThisPersistentId.Count == allBHoMObjects.Count())
                {
                    // All BHoMObjects share this same persistentIdFragment.
                    if (commonPersistentId == null)
                    {
                        // If the commonPersistentId was not already set, we can now set it.
                        commonPersistentId = persistentIdFragment;
                    }
                    else
                    {
                        // If a commonPersistentId was already found (it was not null), 
                        // it means that there is more than one PersistentAdapterId fragment type on all of the BHoMObjects.
                        // This brings us to an undefined situation where it is not possible to automate the Diffing:
                        // the user must manually specify what PersistentAdapterId they want to Diff with.
                        BH.Engine.Reflection.Compute.RecordWarning($"Input objects have multiple {nameof(IPersistentAdapterId)} fragments assigned: `{string.Join("`, ", persistentIdFragmentTypesFound.Keys.Select(t => t.FullName))}`." +
                            $"\nWhich one should be used for Diffing? Use the method {nameof(DiffWithFragmentId)} to manually specify it.");
                        remainder = objects.ToList();
                        return new List<IBHoMObject>();
                    }
                }
            }

            remainder = objects.Except(persistentIdFragmentTypesFound[commonPersistentId]).ToList();
            return persistentIdFragmentTypesFound[commonPersistentId];
        }
    }
}

