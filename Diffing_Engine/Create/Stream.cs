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
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Diffing
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates new Diffing Stream")]
        [Input("objects", "Objects to be included in the Stream")]
        [Input("streamId", "If not specified, streamId will be a GUID")]
        public static BH.oM.Diffing.Stream Stream(IEnumerable<IBHoMObject> objects, string comment = null)
        {
            return new BH.oM.Diffing.Stream(PrepareStreamObjects(objects), null, null, comment);
        }

        [Description("Creates new Diffing Stream")]
        [Input("objects", "Objects to be included in the Stream")]
        [Input("streamId", "If not specified, streamId will be a GUID")]
        [Input("revision", "If not specified, revision is initially set to 0")]
        public static BH.oM.Diffing.Stream Stream(IEnumerable<IBHoMObject> objects, string streamId = null, string revision = null, string comment = null)
        {
            return new BH.oM.Diffing.Stream(PrepareStreamObjects(objects), streamId, revision, comment);
        }

        private static List<IBHoMObject> PrepareStreamObjects(IEnumerable<IBHoMObject> objects)
        {
            // Clone the current objects to preserve immutability
            List<IBHoMObject> objs_cloned = objects.Select(obj => BH.Engine.Base.Query.DeepClone(obj)).ToList();

            // Calculate and set the hash fragment
            Modify.SetHashFragment(objs_cloned);

            // Remove duplicates by hash
            if (Query.RemoveDuplicatesByHash(objs_cloned))
                Reflection.Compute.RecordWarning("Some Objects were duplicates (same hash) and therefore have been discarded.");

            return objs_cloned;
        }

    }
}
