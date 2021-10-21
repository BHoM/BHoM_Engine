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
        [Input("diffingConfig", "(Optional) Additional settings for the Diffing.")]
        [Output("diff", "Object holding the detected changes.")]
        [PreviousVersion("5.0", "BH.Engine.Diffing.Compute.IDiffing(System.Collections.Generic.IEnumerable<object>, System.Collections.Generic.IEnumerable<object>, BH.oM.Diffing.DiffingType, BH.oM.Diffing.DiffingConfig)")]
        public static Diff IDiffing(IEnumerable<object> pastObjs, IEnumerable<object> followingObjs, DiffingConfig diffingConfig = null)
        {
            Diff outputDiff = null;
            if (InputObjectsNullOrEmpty(pastObjs, followingObjs, out outputDiff, diffingConfig))
                return outputDiff;

            // Set configurations if diffConfig is null. Clone it for immutability in the UI.
            DiffingConfig dc = diffingConfig == null ? new DiffingConfig() : diffingConfig.DeepClone();

            Dictionary<string, List<IBHoMObject>> pastBHoMObjs_perNamespace = pastObjs.OfType<IBHoMObject>().GroupBy(obj => obj.GetType().Namespace).ToDictionary(g => g.Key, g => g.ToList());
            Dictionary<string, List<IBHoMObject>> followingBHoMObjs_perNamespace = followingObjs.OfType<IBHoMObject>().GroupBy(obj => obj.GetType().Namespace).ToDictionary(g => g.Key, g => g.ToList());

            // Get all the Toolkit-specific ("Adapter") DiffingMethods, grouped per namespace (e.g. BH.Engine.Adapters.Revit)
            Dictionary<string, MethodBase> adaptersDiffingMethods_perNamespace = AdaptersDiffingMethods_perNamespace();
            // Change the grouping so we replace "Engine" with "oM" for easier matching with objects from the same namespace.
            Dictionary<string, MethodBase> adaptersDiffingMethods_modifiedNamespaces = adaptersDiffingMethods_perNamespace.ToDictionary(kv => kv.Key.Replace("Engine", "oM"), kv => kv.Value);

            // Check what oM namespace is in common between the retrieved Diffing methods and the input objects.
            List<string> commonObjectNameSpaces = pastBHoMObjs_perNamespace.Keys.Intersect(followingBHoMObjs_perNamespace.Keys).ToList();
            commonObjectNameSpaces = commonObjectNameSpaces
                .Where(cns => adaptersDiffingMethods_modifiedNamespaces.Keys.Any(n => cns.Contains(n))).ToList();

            bool performedToolkitDiffing = false;

            // Iterate each of the common namespaces between the retrieved Diffing methods and the input objects.
            foreach (string commonObjectNameSpace in commonObjectNameSpaces)
            {
                string adapterDiffMethodNamespace = adaptersDiffingMethods_modifiedNamespaces.Keys.Where(k => k.StartsWith(k)).FirstOrDefault();

                if (adapterDiffMethodNamespace.IsNullOrEmpty())
                    continue;

                // Invoke the Toolkit-specific ("Adapter") DiffingMethod on the objects of the corresponding oM namespace.
                MethodBase adapterDiffMethodToInvoke = adaptersDiffingMethods_modifiedNamespaces[adapterDiffMethodNamespace];
                BH.Engine.Reflection.Compute.RecordNote($"Invoking Diffing method `{adapterDiffMethodToInvoke.DeclaringType.FullName}.{adapterDiffMethodToInvoke.Name}` on the input objects belonging to namespace {commonObjectNameSpace}.");
                Diff result = InvokeAdapterDiffing(adapterDiffMethodToInvoke, pastBHoMObjs_perNamespace[commonObjectNameSpace], followingBHoMObjs_perNamespace[commonObjectNameSpace], dc);

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
            if (pastObjs.Count() == 1 && followingObjs.Count() == 1)
            {
                Revision pastRev = pastObjs.First() as Revision;
                Revision follRev = followingObjs.First() as Revision;

                if (pastRev != null && follRev != null)
                {
                    BH.Engine.Reflection.Compute.RecordNote($"Calling the diffing method '{nameof(DiffRevisions)}'.");

                    Diff result = DiffRevisions(pastRev, follRev, dc);

                    return outputDiff.CombineDiffs(result);
                }
            }

            // Get only the BHoMObjects from the input objects.
            IEnumerable<IBHoMObject> bHoMObjects_past = pastObjs.OfType<IBHoMObject>();
            IEnumerable<IBHoMObject> bHoMObjects_following = followingObjs.OfType<IBHoMObject>();

            // Check if the BHoMObjects all have a RevisionFragment assigned.
            // If so, we may attempt the Revision diffing.
            if (bHoMObjects_past.AllHaveRevisionFragment() && bHoMObjects_following.AllHaveRevisionFragment())
            {
                BH.Engine.Reflection.Compute.RecordNote($"Calling the diffing method '{nameof(DiffRevisionObjects)}'.");
                return DiffRevisionObjects(bHoMObjects_past, bHoMObjects_following, dc);
            }

            // Check if the bhomObjects have a persistentId assigned.
            List<object> remainder_past;
            List<object> remainder_following;
            Type commonPersistentId_past;
            Type commonPersistentId_following;
            List<IBHoMObject> bHoMObjects_past_persistId = bHoMObjects_past.WithCommonPersistentAdapterId(out remainder_past, out commonPersistentId_past);
            List<IBHoMObject> bHoMObjects_following_persistId = bHoMObjects_following.WithCommonPersistentAdapterId(out remainder_following, out commonPersistentId_following);
            Diff diffGeneric = null;
            Diff fragmentDiff = null;

            // For the BHoMObjects having a common PeristentAdapterId we can compute the Diff by using it.
            if (commonPersistentId_past != null && commonPersistentId_past == commonPersistentId_following
                && bHoMObjects_past_persistId.Count != 0 && bHoMObjects_following_persistId.Count != 0)
            {
                // Check if there is a Toolkit-specific Diffing method that accepts the specific IPersistentAdapterId type.
                var adapterDiffingMethods = adaptersDiffingMethods_modifiedNamespaces.Where(kv => commonPersistentId_past.Namespace.StartsWith(kv.Key)).Select(kv => kv.Value);
                MethodBase adapterDiffingMethod = adapterDiffingMethods.FirstOrDefault();

                if (adapterDiffingMethods.Count() == 1 && adapterDiffingMethod != null)
                {
                    // Invoke the Toolkit-specific ("Adapter") DiffingMethod on the objects of the corresponding oM namespace.
                    BH.Engine.Reflection.Compute.RecordNote($"Invoking Diffing method `{adapterDiffingMethod.DeclaringType.FullName}.{adapterDiffingMethod.Name}` on the input objects that have a common `{nameof(IPersistentAdapterId)}` fragment: `{commonPersistentId_past.FullName}`.");
                    fragmentDiff = InvokeAdapterDiffing(adapterDiffingMethod, bHoMObjects_past_persistId, bHoMObjects_following_persistId, dc);
                }
                else
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
            if (propertiesToConsider?.Any() ?? false)
                dc = new DiffingConfig() { ComparisonConfig = new ComparisonConfig { PropertiesToConsider = propertiesToConsider } };

            return IDiffing(pastObjs, followingObjs, dc);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        // Returns true if one or both the input collections are empty or null,
        // in which case the `out Diff` parameter is assigned with a Diff populated with the information provided.
        private static bool InputObjectsNullOrEmpty(IEnumerable<object> pastObjects, IEnumerable<object> followingObjs, out Diff diff, DiffingConfig diffingConfig = null)
        {
            diff = null;

            bool nullOrEmpty_pastObjects = !pastObjects?.Any() ?? true;
            bool nullOrEmpty_followingObjs = !pastObjects?.Any() ?? true;

            pastObjects = nullOrEmpty_pastObjects ? new List<object>() : pastObjects;
            followingObjs = nullOrEmpty_followingObjs ? new List<object>() : followingObjs;

            if (nullOrEmpty_pastObjects || nullOrEmpty_followingObjs)
                diff = new Diff(followingObjs, pastObjects, new List<object>(), diffingConfig ?? new DiffingConfig());

            return nullOrEmpty_pastObjects || nullOrEmpty_followingObjs;
        }

        /***************************************************/

        // Invokes the Query.AdaptersDiffingMethods(), then groups the fetched methods per parent namespace (e.g. BH.Engine.Adapters.Revit).
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

        // Invoke a Toolkit-specific ("Adapter") Diffing method.
        private static Diff InvokeAdapterDiffing(MethodBase adapterDiffingMethod, IEnumerable<object> pastObjects, IEnumerable<object> followingObjects, DiffingConfig dc)
        {
            Diff result = null;

            if (adapterDiffingMethod == null || (!pastObjects?.Any() ?? true) || (!followingObjects?.Any() ?? true))
                return null;

            try
            {
                List<ParameterInfo> parameterInfos = adapterDiffingMethod.GetParameters().ToList();
                int numberOfOptionalParams = parameterInfos.Where(p => p.IsOptional).Count();
                int indexOfDiffConfigParam = parameterInfos.IndexOf(parameterInfos.First(pi => pi.ParameterType == typeof(DiffingConfig)));
                var parameters = new List<object>() { pastObjects, followingObjects };
                parameters.AddRange(Enumerable.Repeat(Type.Missing, numberOfOptionalParams - 1));
                parameters.Insert(indexOfDiffConfigParam, dc);

                result = adapterDiffingMethod.Invoke(null, parameters.ToArray()) as Diff;
            }
            catch (Exception e)
            {
                BH.Engine.Reflection.Compute.RecordError($"Error invoking Toolkit-specific Diffing method. Error:\n\t{e.ToString()}");
            }

            return result;
        }

        /***************************************************/

        // Finds what objects in the given collection are BHoMObjects and own a PersistentAdapterId fragment of the same type. Useful to automate the IDiffing.
        private static List<IBHoMObject> WithCommonPersistentAdapterId(this IEnumerable<object> objects, out List<object> remainder, out Type commonPersistentIdType)
        {
            IEnumerable<IBHoMObject> allBHoMObjects = objects.OfType<IBHoMObject>();
            remainder = new List<object>();
            commonPersistentIdType = null;

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
                commonPersistentIdType = persistentIdFragmentTypesFound.Keys.FirstOrDefault();
                remainder = objects.Except(persistentIdFragmentTypesFound.Values.FirstOrDefault() ?? new List<IBHoMObject>()).ToList();
                return persistentIdFragmentTypesFound.Values.FirstOrDefault() ?? new List<IBHoMObject>();
            }

            // If multiple PersistentId were found on the objects overall,
            // check if we can find exactly one PersistentId fragment of a common type across all BHoMObjects.
            // (in case some PersistentId Fragment is not present across all objects, we can use the one that is present consistently on all).
            foreach (var kv in persistentIdFragmentTypesFound)
            {
                Type persistentIdFragment = kv.Key;
                List<IBHoMObject> bhomObjectsWithThisPersistentId = kv.Value;
                if (bhomObjectsWithThisPersistentId.Count == allBHoMObjects.Count())
                {
                    // All BHoMObjects share this same persistentIdFragment.
                    if (commonPersistentIdType == null)
                    {
                        // If the commonPersistentId was not already set, we can now set it.
                        commonPersistentIdType = persistentIdFragment;
                    }
                    else
                    {
                        // If a commonPersistentId was already found (it was not null), 
                        // it means that there is more than one PersistentAdapterId fragment type on all of the BHoMObjects.
                        // This brings us to an undefined situation where it is not possible to automate the Diffing:
                        // the user must manually specify what PersistentAdapterId they want to Diff with.
                        BH.Engine.Reflection.Compute.RecordWarning($"Input objects have multiple {nameof(IPersistentAdapterId)} fragments assigned: `{string.Join("`, ", persistentIdFragmentTypesFound.Keys.Select(t => t.FullName))}`." +
                            $"\nWhich one should be used for Diffing? Use the method {nameof(DiffWithFragmentId)} or a Toolkit-specific Diffing method (e.g. RevitDiffing) to manually specify it.");
                        remainder = objects.ToList();
                        return new List<IBHoMObject>();
                    }
                }
            }

            remainder = objects.Except(persistentIdFragmentTypesFound[commonPersistentIdType]).ToList();
            return persistentIdFragmentTypesFound[commonPersistentIdType];
        }

        /***************************************************/

        // Checks whether all input objects own a RevisionFragment. This generally can be ensured when the objects have been passed through a Revision.
        private static bool AllHaveRevisionFragment(this IEnumerable<IBHoMObject> bHoMObjects)
        {
            // Check if objects have hashfragment.
            if (bHoMObjects == null
                || bHoMObjects.Count() == 0
                || bHoMObjects.Select(o => o.RevisionFragment()).Where(o => o != null).Count() < bHoMObjects.Count())
                return false;

            return true;
        }
    }
}

