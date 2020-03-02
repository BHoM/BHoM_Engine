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
using BH.oM.Diffing;
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
        [Input("diffConfig", "Diffing settings for this Stream. Hashes of objects contained in this stream will be computed based on these configs.")]
        [Input("comment", "Any comment to be added for this stream.")]
        public static BH.oM.Diffing.Stream Stream(IEnumerable<IBHoMObject> objects, DiffConfig diffConfig = null, string comment = null)
        {
            return new BH.oM.Diffing.Stream(PrepareStreamObjects(objects, diffConfig), diffConfig, null, null, comment);
        }
        
        /***************************************************/

        [Description("Creates new Diffing Stream")]
        [Input("objects", "Objects to be included in the Stream")]
        [Input("diffConfig", "Diffing settings for this Stream. Hashes of objects contained in this stream will be computed based on these configs.")]
        [Input("streamId", "If not specified, streamId will be a GUID")]
        [Input("revision", "If not specified, revision is initially set to 0")]
        [Input("comment", "Any comment to be added for this stream.")]
        public static BH.oM.Diffing.Stream Stream(IEnumerable<IBHoMObject> objects, DiffConfig diffConfig = null, string streamId = null, string revision = null, string comment = null)
        {
            return new BH.oM.Diffing.Stream(PrepareStreamObjects(objects, diffConfig), diffConfig, streamId, revision, comment);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static List<IBHoMObject> PrepareStreamObjects(IEnumerable<IBHoMObject> objects, DiffConfig diffConfig = null)
        {
            // Clone the current objects to preserve immutability
            List<IBHoMObject> objs_cloned = objects.Select(obj => BH.Engine.Base.Query.DeepClone(obj)).ToList();

            // Calculate and set the hash fragment
            Modify.SetHashFragment(objs_cloned, diffConfig);

            // Remove duplicates by hash
            if (Query.RemoveDuplicatesByHash(objs_cloned))
                Reflection.Compute.RecordWarning("Some Objects were duplicates (same hash) and therefore have been discarded.");

            return objs_cloned;
        }

        /***************************************************/
    }
}

