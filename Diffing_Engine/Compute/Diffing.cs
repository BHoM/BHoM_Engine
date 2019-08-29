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
using BH.oM.Testing;

namespace BH.Engine.Diffing
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a Delta object with the differences between the Stream objects and the provided object list.")]
        [Input("stream", "Diffing Stream.")]
        [Input("currentObjs", "Objects you want to compare the Stream's object against")]
        [Input("enablePropertyDiff", "If true, enables the Property-level diffing, which returns the differences down to the individual object properties.")]
        [Input("exceptions", "List of strings specifying the names of the properties that should be ignored in the calculation, e.g. 'BHoM_Guid'")]
        [Input("useDefaultExceptions", "If true, adds a list of default exceptions: 'BHoM_Guid', 'CustomData', 'Fragments'. Defaults to true.")]
        public static Delta Diffing(BH.oM.Diffing.Stream stream, List<IBHoMObject> currentObjs, bool enablePropertyDiff = true, List<string> exceptions = null, bool useDefaultExceptions = true)
        {
            List<IBHoMObject> readObjs = stream.Objects.Cast<IBHoMObject>().ToList();

            // Clone the objects to assure immutability
            List<IBHoMObject> CurrentObjs_cloned = currentObjs.Select(obj => BH.Engine.Base.Query.DeepClone(obj)).ToList();
            List<IBHoMObject> ReadObjs_cloned = readObjs.Select(obj => BH.Engine.Base.Query.DeepClone(obj)).ToList();

            // Make sure that ReadObjs have an hash. Calculate it if not.
            foreach (var obj in ReadObjs_cloned)
            {
                if (obj.GetHashFragment() == null)
                    obj.DiffingHash(exceptions, true);
            }

            if (useDefaultExceptions)
                SetDefaultExceptions(ref exceptions);

            // Check and process the HashFragment of the current objects
            CurrentObjs_cloned.ForEach((Action<IBHoMObject>)(obj =>
            {
                HashFragment hashFragm = obj.GetHashFragment();

                if (hashFragm == null)
                    // Current objs may not have any HashFragment if they have been created new, or if their modification was done through reinstantiating them.
                    // We need to calculate their hash for the first time, and add to them a HashFragment with that hash. 
                    obj.Fragments.Add(new HashFragment(Compute.SHA256Hash(obj, exceptions), null));
                else
                {
                    // Calculate and set the new object hash, keeping track of its old hash
                    string previousHash = hashFragm.Hash;
                    string newHash = Compute.SHA256Hash(obj, exceptions);

                    obj.Fragments[obj.Fragments.IndexOf(hashFragm)] = new HashFragment(newHash, previousHash);
                }
            }));

            // Remove duplicates by hash
            int numObjs_current = CurrentObjs_cloned.Count();
            CurrentObjs_cloned = CurrentObjs_cloned.GroupBy(obj => obj.GetHashFragment().Hash).Select(gr => gr.First()).ToList();

            int numObjs_read = ReadObjs_cloned.Count();
            ReadObjs_cloned = ReadObjs_cloned.GroupBy(obj => obj.GetHashFragment().Hash).Select(gr => gr.First()).ToList();

            if (numObjs_current != CurrentObjs_cloned.Count())
                BH.Engine.Reflection.Compute.RecordWarning("Some Current Objects were duplicates (same hash) and therefore removed.");

            if (numObjs_read != ReadObjs_cloned.Count())
                BH.Engine.Reflection.Compute.RecordWarning("Some Read Objects were duplicates (same hash) and therefore removed.");

            // Make dictionary of the objects with their hashes to speed up the next lookups
            Dictionary<string, IBHoMObject> ReadObjs_cloned_dict = ReadObjs_cloned.ToDictionary(obj => obj.GetHashFragment().Hash, obj => obj);

            // Dispatch the objects: new, modified or old
            List<IBHoMObject> toBeCreated = new List<IBHoMObject>();
            List<IBHoMObject> toBeUpdated = new List<IBHoMObject>();
            List<IBHoMObject> toBeDeleted = new List<IBHoMObject>();
            List<IBHoMObject> unchanged = new List<IBHoMObject>();
            var objModifiedProps = new Dictionary<string, Dictionary<string, Tuple<object, object>>>();

            foreach (var obj in CurrentObjs_cloned)
            {
                var hashFragm = obj.GetHashFragment();

                if (hashFragm?.PreviousHash == null)
                {
                    toBeCreated.Add(obj); // It's a new object
                }

                else if (hashFragm.PreviousHash == hashFragm.Hash)
                {
                    unchanged.Add(obj); // It's NOT been modified
                }

                else if (hashFragm.PreviousHash != hashFragm.Hash)
                {
                    toBeUpdated.Add(obj); // It's been modified

                    if (enablePropertyDiff)
                    {
                        // Determine changed properties
                        IBHoMObject oldObjState = null;
                        ReadObjs_cloned_dict.TryGetValue(hashFragm.PreviousHash, out oldObjState);

                        if (oldObjState == null) continue;

                        IsEqualConfig ignoreProps = new IsEqualConfig();
                        ignoreProps.PropertiesToIgnore = exceptions;

                        var differentProps = BH.Engine.Testing.Query.DifferentProperties(obj, oldObjState, ignoreProps);

                        objModifiedProps.Add(hashFragm.Hash, differentProps);
                    }
                }
                else
                {
                    BH.Engine.Reflection.Compute.RecordError("Could not find hash information to perform Diffing on some objects.");
                    return null;
                }
            }

            // If no modified property was found, set the field to null (otherwise will get empty list)
            objModifiedProps = objModifiedProps.Count == 0 ? null : objModifiedProps;

            // All ReadObjs that cannot be found by hash in the previousHash of the CurrentObjs are toBeDeleted
            Dictionary<string, IBHoMObject> CurrentObjs_withPreviousHash_dict = CurrentObjs_cloned
                  .Where(obj => obj.GetHashFragment().PreviousHash != null)
                  .ToDictionary(obj => obj.GetHashFragment().PreviousHash, obj => obj);

            toBeDeleted = ReadObjs_cloned_dict.Keys.Except(CurrentObjs_withPreviousHash_dict.Keys)
                .Where(k => ReadObjs_cloned_dict.ContainsKey(k)).Select(k => ReadObjs_cloned_dict[k]).ToList();


            return new Delta(toBeCreated, toBeDeleted, toBeUpdated, objModifiedProps);
        }

        /***************************************************/



    }
}
