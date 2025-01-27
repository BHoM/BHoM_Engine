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
        [Description("Computes the Diffing for BHoMObjects based on an id stored in a Fragment.")]
        [Input("pastObjects", "A set of objects coming from a past revision")]
        [Input("followingObjs", "A set of objects coming from a following Revision")]
        [Input("fragmentType", "(Optional - defaults to the `IPersistentAdapterId` fragment)\nFragment Type where the Id of the objects may be found in the BHoMObjects. The diff will be attempted using the Ids found there.")]
        [Input("fragmentIdProperty", "(Optional - defaults to `PersistentId`)\nName of the property of the Fragment where the Id is stored.")]
        [Input("diffConfig", "(Optional) Sets configs such as properties to be ignored in the diffing, or enable/disable property-by-property diffing.")]
        [Output("diff", "Object holding the detected changes.")]
        public static Diff DiffWithFragmentId(IEnumerable<IBHoMObject> pastObjects, IEnumerable<IBHoMObject> followingObjs, Type fragmentType = null, string fragmentIdProperty = null, DiffingConfig diffConfig = null)
        {
            Diff outputDiff = null;
            if (InputObjectsNullOrEmpty(pastObjects, followingObjs, out outputDiff, diffConfig))
                return outputDiff;

            // Set configurations if diffConfig is null. Clone it for immutability in the UI.
            DiffingConfig diffConfigCopy = diffConfig == null ? new DiffingConfig() : diffConfig.DeepClone();

            // If null, set the default fragmentType/fragmentIdProperty.
            if (fragmentType == null || string.IsNullOrWhiteSpace(fragmentIdProperty))
            {
                fragmentType = typeof(IPersistentAdapterId);
                fragmentIdProperty = nameof(IPersistentAdapterId.PersistentId);

                BH.Engine.Base.Compute.RecordNote($"No `{nameof(fragmentType)}` or `{nameof(fragmentIdProperty)}` specified." +
                    $"\nDefaulted to `{typeof(IPersistentAdapterId).FullName}.{nameof(IPersistentAdapterId.PersistentId)}`.");
            }

            // Checks on the specified fragmentType/fragmentIdProperty combination.
            var propertiesOnFragment = fragmentType.GetProperties().Where(pi => pi.Name == fragmentIdProperty);
            if (!propertiesOnFragment.Any())
            {
                BH.Engine.Base.Compute.RecordError($"No property named `{fragmentIdProperty}` was found on the specified {nameof(fragmentType)} `{fragmentType.FullName}`.");
                return null;
            }

            List<string> pastObjsIds = pastObjects.Select(o => o.GetIdFromFragment(fragmentType, fragmentIdProperty)).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            List<string> follObjsIds = followingObjs.Select(o => o.GetIdFromFragment(fragmentType, fragmentIdProperty)).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            bool missingIds = false;
            if (pastObjsIds.Count() != pastObjects.Count())
            {
                missingIds = true;
                BH.Engine.Base.Compute.RecordError($"Could not find the Id in some of the input {nameof(pastObjects)} within the specified fragment property `{fragmentType.Name}.{fragmentIdProperty}`.");
            }

            if (follObjsIds.Count() != followingObjs.Count())
            {
                missingIds = true;
                BH.Engine.Base.Compute.RecordError($"Could not find the Id in some of the input {nameof(followingObjs)} within the specified fragment property `{fragmentType.Name}.{fragmentIdProperty}`.");
            }

            if (missingIds)
                return null;

            return Diffing(pastObjects, pastObjsIds, followingObjs, follObjsIds, diffConfigCopy);
        }

        private static string GetIdFromFragment(this IBHoMObject obj, Type fragmentType, string fragmentIdProperty)
        {
            if (!typeof(IFragment).IsAssignableFrom(fragmentType))
            {
                BH.Engine.Base.Compute.RecordError("The specified Type must be a fragment Type.");
                return null;
            }

            // Check on fragmentIdProperty
            if (string.IsNullOrWhiteSpace(fragmentIdProperty))
            {
                BH.Engine.Base.Compute.RecordError($"Invalid {nameof(fragmentIdProperty)} provided.");
                return null;
            }

            IFragment idFragm = null;
            var idFragments = obj.GetAllFragments(fragmentType);
            if (idFragments != null && idFragments.Count > 1)
            {
                BH.Engine.Base.Compute.RecordWarning($"Object of type {obj.GetType()}, guid {obj.BHoM_Guid} contains more than one fragment of the provided Fragment type {fragmentType}. Unable to decide which one to pick.");
                return null;
            }
            else
                idFragm = idFragments?.FirstOrDefault();

            if (idFragm == null)
            {
                BH.Engine.Base.Compute.RecordWarning($"Object of type {obj.GetType()}, guid {obj.BHoM_Guid} does not contain a fragment of the provided Fragment type {fragmentType}.");
                return null;
            }

            object value = BH.Engine.Base.Query.PropertyValue(idFragm, fragmentIdProperty);
            if (value == null)
            {
                BH.Engine.Base.Compute.RecordWarning($"The retrieved fragment for an object of type {obj.GetType()}, guid {obj.BHoM_Guid} has not any value under the property named {fragmentIdProperty}.");
                return null;
            }

            return value.ToString();
        }
    }
}






