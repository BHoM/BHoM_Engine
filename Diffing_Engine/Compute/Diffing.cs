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

        public static Delta Diffing(List<IBHoMObject> currentObjs, List<IBHoMObject> readObjs = null, bool propertyLevelDiffing = true, List<string> exceptions = null, bool useDefaultExceptions = true, BH.oM.Diffing.Stream diffStream = null)
        {
            // Clone the objects to assure immutability
            List<IBHoMObject> CurrentObjs_cloned = currentObjs.Select(obj => BH.Engine.Base.Query.DeepClone(obj)).ToList();
            List<IBHoMObject> ReadObjs_cloned = readObjs.Select(obj => BH.Engine.Base.Query.DeepClone(obj)).ToList();

            if (diffStream == null)
            {
                if (ReadObjs_cloned != null && ReadObjs_cloned.Count != 0)
                {
                    // Get project fragment from one of the objects and use it as the base, if exists. Else throw error.
                    var readObjWithFragms = ReadObjs_cloned.Where(obj => obj.Fragments.Count != 0).ToList();
                    if (readObjWithFragms.Count != 0)
                        diffStream = readObjWithFragms.First()
                            .Fragments.Where(fragm => fragm?.GetType() == typeof(DiffHashFragment))
                            .First().GetHashFragment().DiffingStream;
                    //else
                    //    BH.Engine.Reflection.Compute.RecordError("You must either specify a Stream input or input ReadObjs that were added to a Stream previously.");
                }
            }

            if (useDefaultExceptions)
                SetDefaultExceptions(ref exceptions);

            // Make sure that ReadObjs have an hash. Calculate it if not.
            foreach (var obj in ReadObjs_cloned)
            {
                if (obj.GetHashFragment() == null)
                    obj.DiffHash(exceptions,true,diffStream);
            }

            List<IBHoMObject> toBeCreated = new List<IBHoMObject>();
            List<IBHoMObject> toBeUpdated = new List<IBHoMObject>();
            List<IBHoMObject> toBeDeleted = new List<IBHoMObject>();
            List<IBHoMObject> unchanged = new List<IBHoMObject>();
            var objModifiedProps = new Dictionary<string, Dictionary<string, Tuple<object, object>>>();

            // Check and process the DiffHashFragment of the current objects
            CurrentObjs_cloned.ForEach(obj =>
            {
                var hashFragm = obj.GetHashFragment();

                if (hashFragm == null)
                {
                    // If the current object doesn't already have an hash, that object is new.
                    // Calculate its hash and add it to the toBeCreated.
                    obj.DiffHash(exceptions, true, diffStream);
                    toBeCreated.Add(obj);
                }
                else
                {
                    // If the current object already has a hash,
                    // calculate and set the new object hash, keeping track of its old hash.
                    string previousHash = hashFragm.Hash;
                    string newHash = Compute.SHA256Hash(obj, exceptions);

                    obj.Fragments[obj.Fragments.IndexOf(hashFragm)] = new DiffHashFragment(newHash, previousHash, diffStream);

                    // If the new hash is equal to the old hash, that object is unchanged.
                    if (previousHash == newHash)
                    {
                        unchanged.Add(obj);
                    }

                    // If the new hash is different from the old hash, that object has been modified.
                    else if (previousHash != newHash)
                    {
                        toBeUpdated.Add(obj);

                        if (propertyLevelDiffing)
                        {
                            // Determine changed properties
                            var oldObjState = ReadObjs_cloned.Single(rObj => rObj.GetHashFragment().Hash == hashFragm.PreviousHash);

                            IsEqualConfig ignoreProps = new IsEqualConfig();
                            ignoreProps.PropertiesToIgnore = exceptions;
                            var differentProps = BH.Engine.Testing.Query.IsEqual(obj, oldObjState, ignoreProps);
                            var differentProps1 = BH.Engine.Testing.Query.DifferentProperties(obj, oldObjState, ignoreProps);
                            var changes = new Tuple<List<string>, List<string>>(differentProps.Item2, differentProps.Item3);

                            objModifiedProps.Add(hashFragm.Hash, differentProps1);
                        }
                    }
                }
            });

            // If no modified property was found, set the field to null (otherwise will get empty list)
            objModifiedProps = objModifiedProps.Count == 0 ? null : objModifiedProps;

            // Make dictionary of the objects with their hashes to speed up the next lookup
            Dictionary<string, IBHoMObject> CurrentObjs_cloned_dict = CurrentObjs_cloned.ToDictionary(obj => obj.GetHashFragment().Hash, obj => obj);
            Dictionary<string, IBHoMObject> ReadObjs_cloned_dict = ReadObjs_cloned.ToDictionary(obj => obj.GetHashFragment().Hash, obj => obj);

            // All ReadObjs that cannot be found by hash in the CurrentObjs are old
            toBeDeleted = ReadObjs_cloned_dict.Keys.Except(CurrentObjs_cloned_dict.Keys)
                .Where(k => ReadObjs_cloned_dict.ContainsKey(k)).Select(k => ReadObjs_cloned_dict[k]).ToList();

            return new Delta(toBeCreated, toBeDeleted, toBeUpdated, objModifiedProps, diffStream);
        }

        /***************************************************/



    }
}
