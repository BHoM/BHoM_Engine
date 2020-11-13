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
        [Description("Computes the diffing for two lists of objects, comparing objects one by one." +
            "\nThis will only identify 'modified' or 'unchanged' objects. For 'modified' objects, the property differences are also returned." +
            "\nIt will work correctly only if the input objects are in the same order.")]
        [Input("pastObjects", "Past objects. Objects whose creation precedes 'currentObjects'.")]
        [Input("currentObjects", "Following objects. Objects that were created after 'pastObjects'.")]
        [Input("diffConfig", "Sets configs such as properties to be ignored in the diffing, or enable/disable property-by-property diffing.")]
        public static Diff DiffOneByOne(IEnumerable<object> pastObjects, IEnumerable<object> currentObjects, DiffingConfig diffConfig = null)
        {
            if (pastObjects.Count() != currentObjects.Count())
            {
                BH.Engine.Reflection.Compute.RecordWarning($"Input collections must be of the same length for '{nameof(DiffOneByOne)}' to work.");
                return null;
            }

            BH.Engine.Reflection.Compute.RecordNote($"This diffing method is equivalent to calling '{nameof(Query.DifferentProperties)}' on the input lists. " +
                $"\nThis will only identify 'modified' or 'unchanged' objects. For 'modified' objects, the property differences are also returned." +
                $"\nIt will work correctly only if the objects in the lists are in the same order and at most they have been modified (i.e. no new object has been added, no object has been deleted).");

            // Set configurations if diffConfig is null. Clone it for immutability in the UI.
            DiffingConfig diffConfigCopy = diffConfig == null ? new DiffingConfig() : (DiffingConfig)diffConfig.DeepClone();
            diffConfigCopy.EnablePropertyDiffing = true; // must be forced on for this Diffing method to make sense.

            // Clone objects for immutability in the UI.
            List<object> pastObjects_cloned = BH.Engine.Base.Query.DeepClone(pastObjects).ToList();
            List<object> currentObjects_cloned = BH.Engine.Base.Query.DeepClone(currentObjects).ToList();

            List<object> modifiedObjects = new List<object>();
            List<object> unchangedObjects = new List<object>();

            bool anyChangeDetected = false;

            var allModifiedProps = new Dictionary<string, Dictionary<string, Tuple<object, object>>>();
            for (int i = 0; i < pastObjects_cloned.Count(); i++)
            {
                var modifiedProps = Query.DifferentProperties(currentObjects_cloned[i], pastObjects_cloned[i], diffConfigCopy);

                if (modifiedProps != null && modifiedProps.Any())
                {
                    modifiedObjects.Add(currentObjects_cloned[i]);
                    anyChangeDetected = true;
                }
                else if (diffConfig.IncludeUnchangedObjects)
                    unchangedObjects.Add(currentObjects_cloned[i]);

                allModifiedProps[$"Object #{i}"] = modifiedProps ?? new Dictionary<string, Tuple<object, object>>();
            }

            if (!anyChangeDetected)
                allModifiedProps = null;

            return new Diff(new List<object>(), new List<object>(), modifiedObjects, diffConfigCopy, allModifiedProps, unchangedObjects);
        }
    }
}

