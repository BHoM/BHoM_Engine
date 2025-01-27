/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using System.Collections;
using BH.Engine.Base;

namespace BH.Engine.Diffing
{
    public static partial class Compute
    {
        [Description("Computes the Diffing for BHoMObjects based on an Id stored in their CustomData dictionary under a specific Key.")]
        [Input("pastObjects", "A set of objects coming from a past revision")]
        [Input("followingObjects", "A set of objects coming from a following Revision")]
        [Input("customdataIdKey", "Name of the key where the Id of the objects may be found in the BHoMObjects' CustomData. The diff will be attempted using the Ids found there." +
            "\nE.g. 'Revit_UniqueId' may be used; an id must be stored under object.CustomData['Revit_UniqueId'].")]
        [Input("diffingConfig", "Sets configs such as properties to be ignored in the diffing, or enable/disable property-by-property diffing.")]
        [Output("diff", "Object holding the detected changes.")]
        public static Diff DiffWithCustomDataKeyId(IEnumerable<IBHoMObject> pastObjects, IEnumerable<IBHoMObject> followingObjects, string customdataIdKey, DiffingConfig diffingConfig = null)
        {
            Diff outputDiff = null;
            if (InputObjectsNullOrEmpty(pastObjects, followingObjects, out outputDiff, diffingConfig))
                return outputDiff;

            List<string> pastObjectsIds = new List<string>();
            List<string> followingObjectsIds = new List<string>();

            // Verifies inputs and populates the id lists, then if successfull perform the Diffing.
            if (ExtractIdFromCustomData(pastObjects, followingObjects, customdataIdKey, out followingObjectsIds, out pastObjectsIds))
                return Diffing(pastObjects.OfType<object>(), pastObjectsIds, followingObjects.OfType<object>(), followingObjectsIds, diffingConfig);

            return null;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool ExtractIdFromCustomData(IEnumerable<IBHoMObject> pastObjects, IEnumerable<IBHoMObject> followingObjects, string customdataIdKey, out List<string> out_currentObjectsIds, out List<string> out_pastObjectsIds)
        {
            // Output ids
            out_currentObjectsIds = new List<string>();
            out_pastObjectsIds = new List<string>();

            List<string> followingObjectsIds = new List<string>();
            List<string> pastObjectsIds = new List<string>();

            // Check on customDataKey
            if (string.IsNullOrWhiteSpace(customdataIdKey))
            {
                BH.Engine.Base.Compute.RecordError($"Invalid {nameof(customdataIdKey)} provided.");
                return false;
            }

            // Flags
            bool allRetrieved = true;
            bool noDuplicates = true;

            // Retrieve Id from CustomData for current objects
            followingObjects.ToList().ForEach(o =>
            {
                object id = null;
                o.CustomData.TryGetValue(customdataIdKey, out id);
                if (!string.IsNullOrEmpty(id?.ToString()))
                    followingObjectsIds.Add(id.ToString());
                else
                    allRetrieved = false;
            });

            // Checks on current Objects
            if (!allRetrieved)
                BH.Engine.Base.Compute.RecordWarning($"Some or all of the {nameof(followingObjects)}' do not have a valid ID/Key usable for Diffing.");

            if (followingObjectsIds.Count != followingObjectsIds.Distinct().Count())
            {
                BH.Engine.Base.Compute.RecordWarning($"Some of the {nameof(followingObjects)} have duplicate Id.");
                noDuplicates = false;
            }

            // Retrieve Id from CustomData for past objects
            pastObjects.ToList().ForEach(o =>
            {
                object id = null;
                o.CustomData.TryGetValue(customdataIdKey, out id);
                if (!string.IsNullOrEmpty(id?.ToString()))
                    pastObjectsIds.Add(id.ToString());
                else
                    allRetrieved = false;
            });

            // Checks on past Objects
            if (!allRetrieved)
                BH.Engine.Base.Compute.RecordWarning($"Some or all of the {nameof(pastObjects)}' do not have a valid ID/Key usable for Diffing.");

            if (pastObjectsIds.Count != pastObjectsIds.Distinct().Count())
            {
                BH.Engine.Base.Compute.RecordWarning($"Some of the {nameof(pastObjects)} have duplicate Id.");
                noDuplicates = false;
            }

            if (!allRetrieved || !noDuplicates)
                return false;

            out_currentObjectsIds = followingObjectsIds;
            out_pastObjectsIds = pastObjectsIds;

            return true;
        }
    }
}






