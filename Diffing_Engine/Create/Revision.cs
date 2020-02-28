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

        [Description("Creates new Stream Revision")]
        [Input("objects", "Objects to be included in the Stream Revision")]
        [Input("streamId", "Id of the Stream owning this Revision.")]
        [Input("revisionId", "If not specified, revisionId will be an auto-generated GUID.")]
        [Input("diffConfig", "Diffing settings for this Stream Revision. Hashes of objects contained in this stream will be computed based on these configs.")]
        [Input("comment", "Any comment to be added for this stream.")]
        public static Revision Revision(IEnumerable<IBHoMObject> objects, string streamId, string revisionId = null, DiffConfig diffConfig = null, string comment = null)
        {
            return new Revision(Modify.PrepareForDiffing(objects, diffConfig), streamId, diffConfig, revisionId, comment);
        }


        [Description("Returns a new Revision as an update of a previous one, replacing its objects with the input objects.")]
        [Input("stream", "Stream to be updated")]
        [Input("objects", "Objects to be included in the updated Revision")]
        [Input("newRevisionId", "If not specified, newRevisionId will be an auto-generated GUID.")]
        [Output("The new Revision containing the given objects.")]
        public static Revision Revision(Revision previousRevision, IEnumerable<IBHoMObject> objects, string newRevisionId = null)
        {
            return new Revision(Modify.PrepareForDiffing(objects, previousRevision.RevisionDiffConfing), previousRevision.StreamId, previousRevision.RevisionDiffConfing, newRevisionId);
        }
    }
}

