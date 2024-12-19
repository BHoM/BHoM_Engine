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
using BH.oM.Diffing;
using BH.oM.Base.Attributes;
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

        [Description("Creates new Stream Revision")]
        [Input("objects", "Objects to be included in the Stream Revision")]
        [Input("streamId", "Input either: a Guid of an existing stream; a previous Revision from which to extract the StreamId; or a StreamPointer object.")]
        [Input("revisionName", "Name of the Revision.")]
        [Input("comment", "Any comment to be added for this Revision. Much like git commit comment.")]
        [Input("diffConfig", "Diffing settings for this Stream Revision. Hashes of objects contained in this stream will be computed based on these configs.")]
        public static Revision Revision(IEnumerable<IBHoMObject> objects, object streamId, string revisionName = null, string comment = null, DiffingConfig diffConfig = null)
        {
            if(objects == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create a revision from a null collection of objects.");
                return null;
            }

            if(streamId == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create a revision from a null stream ID.");
                return null;
            }

            return new Revision(Modify.PrepareForRevision(objects, diffConfig), ProcessStreamId(streamId), diffConfig, revisionName, comment);
        }

        private static Guid ProcessStreamId(object streamId)
        {
            if (streamId == null)
                throw new ArgumentNullException($"Input {nameof(streamId)} cannot be null.");

            Guid _streamId;
            if (!Convert.TryParseObjectToGuid(streamId, out _streamId))
                BH.Engine.Base.Compute.RecordError($"Specified input in {nameof(streamId)} is not valid.");

            return _streamId;
        }
    }
}






