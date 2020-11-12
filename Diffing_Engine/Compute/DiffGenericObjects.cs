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
        [Description("Computes the diffing for generic objects that do not have any Id or HashFragment assigned." +
            "\nShould be seen as last resort if no other diffing method can be applied.")]
        [Input("pastObjects", "Past objects. Objects whose creation precedes 'currentObjects'.")]
        [Input("currentObjects", "Following objects. Objects that were created after 'pastObjects'.")]
        [Input("diffConfig", "Sets configs such as properties to be ignored in the diffing, or enable/disable property-by-property diffing.")]
        [Input("useExistingHash", "Advanced setting. If the objects already have an HashFragment assigned, but that only has the 'currentHash' populated. Can be used to avoid recomputing hash in some scenarios.")]
        public static Diff DiffGenericObjects(IEnumerable<object> pastObjects, IEnumerable<object> currentObjects, DiffConfig diffConfig = null, bool keepExistingHash = false)
        {
            BH.Engine.Reflection.Compute.RecordNote("This diffing method cannot track modified objects between different revisions." +
                "\nIt will simply return the objects that appear exclusively in the past set, in the following set, and in both." +
                $"\nConsider using '{nameof(DiffWithCustomId)}', '{nameof(DiffWithFragmentId)}' or '{nameof(DiffRevisions)}' if this feature is needed.");

            // Set configurations if diffConfig is null. Clone it for immutability in the UI.
            DiffConfig diffConfigCopy = diffConfig == null ? new DiffConfig() : (DiffConfig)diffConfig.DeepClone();

            // Clone objects for immutability in the UI.
            List<object> pastObjects_cloned = BH.Engine.Base.Query.DeepClone(pastObjects).ToList();
            List<object> currentObjects_cloned = BH.Engine.Base.Query.DeepClone(currentObjects).ToList();

            if (!keepExistingHash)
            {
                // Clean any existing hash fragment. 
                // This ensures the hash will be re-computed within this method using the provided DiffConfig.
                pastObjects_cloned.OfType<IBHoMObject>().ToList().ForEach(o => o.Fragments.Remove(typeof(HashFragment)));
                currentObjects_cloned.OfType<IBHoMObject>().ToList().ForEach(o => o.Fragments.Remove(typeof(HashFragment)));
            }

            // Compute the "Diffing" by means of a VennDiagram.
            // Hashes are computed in the DiffingHashComparer, once per each object (the hash is stored in a hashFragment).
            VennDiagram<object> vd = Engine.Data.Create.VennDiagram(pastObjects_cloned, currentObjects_cloned, new HashComparer<object>(diffConfigCopy, true));
            
            return new Diff(vd.OnlySet2, vd.OnlySet1, null, diffConfigCopy, null, vd.Intersection);
        }
    }
}

