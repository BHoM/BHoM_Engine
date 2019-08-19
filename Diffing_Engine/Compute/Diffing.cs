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

        public static Delta Diffing(List<IBHoMObject> currentObjs, List<IBHoMObject> readObjs = null)
        {
            return Diffing(currentObjs, readObjs);
        }


        public static Delta Diffing(List<IBHoMObject> currentObjs, List<IBHoMObject> readObjs = null, bool propertyLevelDiffing = true, List<string> exceptions = null, bool useDefaultExceptions = true, BH.oM.Diffing.Stream diffStream = null)
        {
            // Clone the objects to assure immutability
            List<IBHoMObject> CurrentObjs_cloned = currentObjs.Select(obj => BH.Engine.Base.Query.DeepClone(obj)).ToList();
            List<IBHoMObject> ReadObjs_cloned = readObjs.Select(obj => BH.Engine.Base.Query.DeepClone(obj)).ToList();

            if (diffStream == null)
            {
                // Get project fragment from one of the objects and use it as the base, if exists
                diffStream = ReadObjs_cloned
                    .Where(obj => obj.Fragments.Exists(fragm => fragm?.GetType() == typeof(DiffHashFragment)))
                    .First()
                    .GetHashFragment().DiffingStream;
            }

            if (useDefaultExceptions)
                SetDefaultExceptions(ref exceptions);

            // Check and process the DiffHashFragment of the objects
            CurrentObjs_cloned.ForEach(obj =>
            {
                DiffHashFragment hashFragment = obj.GetHashFragment();

                if (hashFragment == null)
                    // Current objs may not have any DiffHashFragment if they have been created new, or if their modification was done through reinstantiating them.
                    // We need to calculate their hash for the first time, and add to them a DiffHashFragment with that hash. 
                    obj.Fragments.Add(new DiffHashFragment(Compute.SHA256Hash(obj, exceptions), null, diffStream));
                else
                {
                    // Calculate and set the new object hash, keeping track of its old hash
                    string previousHash = hashFragment.Hash;
                    string newHash = Compute.SHA256Hash(obj, exceptions);

                    obj.Fragments[obj.Fragments.IndexOf(hashFragment)] = new DiffHashFragment(newHash, previousHash, diffStream);
                }
            });

            Dictionary<string, IBHoMObject> CurrentObjs_cloned_dict = CurrentObjs_cloned.ToDictionary(obj => obj.GetHashFragment().Hash, obj => obj);

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

                else
                {

                    BH.Engine.Reflection.Compute.RecordError("Could not find hash information to perform Diffing on some objects.");
                    return null;
                }
            }

            foreach (var obj in ReadObjs_cloned)
            {
                var hashFragm = obj.GetHashFragment();

                if (!CurrentObjs_cloned.Any(cObj => cObj.GetHashFragment().PreviousHash == hashFragm.Hash))
                {
                    toBeDeleted.Add(obj); // It doesn't exist anymore (it's not among the current objects)
                    continue;
                }
            }

            return new Delta(toBeCreated, toBeDeleted, toBeUpdated, objModifiedProps, diffStream);
        }

        /***************************************************/



    }
}
