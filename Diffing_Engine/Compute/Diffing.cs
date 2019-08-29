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
        [Input("previousStream", "A previous version of a Stream")]
        [Input("currentStream", "A new version of a Stream")]
        [Input("enablePropertyDiff", "If true, enables the Property-level diffing, which returns the differences down to the individual object properties.")]
        [Input("exceptions", "List of strings specifying the names of the properties that should be ignored in the calculation, e.g. 'BHoM_Guid'")]
        [Input("useDefaultExceptions", "If true, adds a list of default exceptions: 'BHoM_Guid', 'CustomData', 'Fragments'. Defaults to true.")]
        public static Delta Diffing(Stream previousStream, Stream currentStream, bool enablePropertyDiff = true, List<string> exceptions = null, bool useDefaultExceptions = true)
        {
            // Take the Stream's objects
            List<IBHoMObject> CurrentObjs_cloned = currentStream.Objects.Cast<IBHoMObject>().ToList();
            List<IBHoMObject> ReadObjs_cloned = previousStream.Objects.Cast<IBHoMObject>().ToList();

            // Make dictionary with object hashes to speed up the next lookups
            Dictionary<string, IBHoMObject> ReadObjs_cloned_dict = ReadObjs_cloned.ToDictionary(obj => obj.GetHashFragment().Hash, obj => obj);

            // Set exceptions
            if (useDefaultExceptions)
                Compute.SetDefaultExceptions(ref exceptions);

            // Dispatch the objects: new, modified or old
            List<IBHoMObject> toBeCreated = new List<IBHoMObject>();
            List<IBHoMObject> toBeUpdated = new List<IBHoMObject>();
            List<IBHoMObject> toBeDeleted = new List<IBHoMObject>();
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
                    // It's NOT been modified
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

                        IsEqualConfig config = new IsEqualConfig();
                        config.PropertiesToIgnore = exceptions;

                        var differentProps = BH.Engine.Testing.Query.DifferentProperties(obj, oldObjState, config);

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
