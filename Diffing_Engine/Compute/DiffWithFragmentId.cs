/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
        [Description("Computes the Diffing for BHoMObjects based on an id stored in a Fragment.")]
        [Input("pastObjects", "A set of objects coming from a past revision")]
        [Input("currentObjects", "A set of objects coming from a following Revision")]
        [Input("fragmentType", "(Optional - defaults to the `IPersistentId` fragment)\nFragment Type where the Id of the objects may be found in the BHoMObjects. The diff will be attempted using the Ids found there.")]
        [Input("fragmentIdProperty", "(Optional - defaults to `PersistentId`)\nName of the property of the Fragment where the Id is stored.")]
        [Input("diffConfig", "(Optional) Sets configs such as properties to be ignored in the diffing, or enable/disable property-by-property diffing.")]
        public static Diff DiffWithFragmentId(IEnumerable<IBHoMObject> pastObjects, IEnumerable<IBHoMObject> currentObjects, Type fragmentType = null, string fragmentIdProperty = null, DiffConfig diffConfig = null)
        {
            if (fragmentType == null)
            {
                fragmentType = typeof(IPersistentId);
                fragmentIdProperty = nameof(IPersistentId.PersistentId);
            }

            // Set configurations if diffConfig is null. Clone it for immutability in the UI.
            DiffConfig diffConfigCopy = diffConfig == null ? new DiffConfig() : (DiffConfig)diffConfig.DeepClone();

            if (string.IsNullOrWhiteSpace(fragmentIdProperty))
            {
                BH.Engine.Reflection.Compute.RecordError($"The DiffConfig must specify a valid {nameof(fragmentIdProperty)}.");
                return null;
            }

            // Clone for immutability
            List<IBHoMObject> currentObjs = currentObjects.ToList();
            List<IBHoMObject> pastObjs = pastObjects.ToList();

            string customDataIdKey = fragmentType.Name + "_fragmentId";

            currentObjs.ForEach(o => o.CustomData[customDataIdKey] = o.GetIdFromFragment(fragmentType, fragmentIdProperty));
            pastObjs.ForEach(o => o.CustomData[customDataIdKey] = o.GetIdFromFragment(fragmentType, fragmentIdProperty));

            return DiffWithCustomId(pastObjs, currentObjs, customDataIdKey, diffConfig);
        }

        private static string GetIdFromFragment(this IBHoMObject obj, Type fragmentType, string fragmentIdProperty)
        {
            if (!typeof(IFragment).IsAssignableFrom(fragmentType))
            {
                BH.Engine.Reflection.Compute.RecordError("The specified Type must be a fragment Type.");
                return null;
            }

            // Check on fragmentIdProperty
            if (string.IsNullOrWhiteSpace(fragmentIdProperty))
            {
                BH.Engine.Reflection.Compute.RecordError($"Invalid {nameof(fragmentIdProperty)} provided.");
                return null;
            }

            IFragment idFragm = obj.FindFragment<IFragment>(fragmentType);
            if (idFragm == null)
            {
                BH.Engine.Reflection.Compute.RecordWarning($"Object of type {obj.GetType()}, guid {obj.BHoM_Guid} does not contain a fragment of the provided Fragment type {fragmentType}.");
                return null;
            }

            object value = BH.Engine.Reflection.Query.PropertyValue(idFragm, fragmentIdProperty);
            if (value == null)
            {
                BH.Engine.Reflection.Compute.RecordWarning($"The retrieved fragment for an object of type {obj.GetType()}, guid {obj.BHoM_Guid} has not any value under the property named {fragmentIdProperty}.");
                return null;
            }

            return value.ToString();
        }
    }
}

