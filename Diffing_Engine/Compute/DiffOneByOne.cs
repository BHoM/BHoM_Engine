/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
        [Description("Computes the diffing for two lists of objects, comparing objects one by one." +
            "\nThis will only identify 'modified' or 'unchanged' objects. For 'modified' objects, the property differences are also returned." +
            "\nIt will work correctly only if the input lists are of the same length and objects are in the same order (i.e. it will only discover modified objects, not added/removed).")]
        [Input("pastObjects", "Past objects. Objects whose creation precedes 'currentObjects'.")]
        [Input("followingObjects", "Following objects. Objects that were created after 'pastObjects'.")]
        [Input("diffConfig", "Sets configs such as properties to be ignored in the diffing, or enable/disable property-by-property diffing.")]
        [Output("diff", "Object holding the detected changes.")]
        public static Diff DiffOneByOne(IEnumerable<object> pastObjects, IEnumerable<object> followingObjects, DiffingConfig diffConfig = null)
        {
            Diff outputDiff = null;
            if (InputObjectsNullOrEmpty(pastObjects, followingObjects, out outputDiff, diffConfig))
                return outputDiff;

            if (pastObjects.Count() != followingObjects.Count())
            {
                BH.Engine.Base.Compute.RecordWarning($"Input collections must be of the same length for '{nameof(DiffOneByOne)}' to work.");
                return null;
            }

            BH.Engine.Base.Compute.RecordNote($"This diffing method is equivalent to calling '{nameof(Query.ObjectDifferences)}' on the input lists. " +
                $"\nThis will only identify 'modified' or 'unchanged' objects. For 'modified' objects, the property differences are also returned." +
                $"\nIt will work correctly only if the input objects are in the same order (i.e. it will only discover modified objects by comparing them one by one; it will not discover added/removed objects).");

            // Set configurations if diffConfig is null. Clone it for immutability in the UI.
            DiffingConfig diffConfigCopy = diffConfig == null ? new DiffingConfig() : (DiffingConfig)diffConfig.DeepClone();
            diffConfigCopy.EnablePropertyDiffing = true; // must be forced on for this Diffing method to make sense.

            // Clone objects for immutability in the UI.
            List<object> pastObjects_cloned = BH.Engine.Base.Query.DeepClone(pastObjects).ToList();
            List<object> currentObjects_cloned = BH.Engine.Base.Query.DeepClone(followingObjects).ToList();

            List<object> modifiedObjects = new List<object>();
            List<object> unchangedObjects = new List<object>();

            bool anyChangeDetected = false;

            List<ObjectDifferences> modifiedObjectDifferences = new List<ObjectDifferences>();
            for (int i = 0; i < pastObjects_cloned.Count(); i++)
            {
                ObjectDifferences objectDifferences = Query.ObjectDifferences(pastObjects_cloned[i], currentObjects_cloned[i], diffConfigCopy.ComparisonConfig);

                if (objectDifferences.Differences?.Any() ?? false)
                {
                    modifiedObjects.Add(currentObjects_cloned[i]);
                    anyChangeDetected = true;
                }
                else if (diffConfigCopy.IncludeUnchangedObjects)
                    unchangedObjects.Add(currentObjects_cloned[i]);

                modifiedObjectDifferences.Add(objectDifferences);
            }

            if (!anyChangeDetected)
                modifiedObjectDifferences = null;

            return new Diff(new List<object>(), new List<object>(), modifiedObjects, diffConfigCopy, modifiedObjectDifferences, unchangedObjects);
        }
    }
}





