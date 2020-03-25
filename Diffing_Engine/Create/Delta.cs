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
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Diffing
{
    public static partial class Create
    {
        // ----------------------------------------------- //
        //                Diff-based Delta                 //
        // ----------------------------------------------- //

        [Description("Returns a Delta object with the Diff between the two input Revisions, also called `Diff-based Delta`.")]
        [Input("previousRevision", "A previous Revision")]
        [Input("currentRevision", "A new Revision")]
        [Input("diffConfig", "Sets configs such as properties to be ignored in the diffing, or enable/disable property-by-property diffing.\nBy default it takes the diffConfig property of the Revision. This input can be used to override it.")]
        public static Delta Delta(Revision pastRevision, Revision currentRevision, DiffConfig diffConfig = null, string comment = null)
        {
            Diff diff = Compute.Diffing(pastRevision.Objects, currentRevision.Objects, diffConfig);

            return new Delta(pastRevision.StreamId, diff, pastRevision.RevisionId, currentRevision.RevisionId, comment);
        }

        // ----------------------------------------------- //
        //              Revision-based Delta               //
        // ----------------------------------------------- //

        [Description("Returns a Delta object containing all the objects of the input Revision, also called `Revision-Based Delta`.")]
        [Input("revision", "A new Revision")]
        [Input("diffConfig", "Sets configs such as properties to be ignored in the diffing, or enable/disable property-by-property diffing.\nBy default it takes the diffConfig property of the Revision. This input can be used to override it.")]
        public static Delta Delta(Revision revision, DiffConfig diffConfig = null, string comment = null)
        {
            Diff diff = Compute.Diffing(null, revision.Objects, diffConfig);
           
            return new Delta(revision.StreamId, diff, revision.RevisionId, null, comment);
        }

        [Description("Returns a Delta object, containing all the input objects wrapped in a Revision. Also called `Revision-Based Delta`.")]
        [Input("objects", "Objects that will be wrapped into a new Revision in order to produce this Delta.")]
        [Input("streamId", "Id of the Stream that will own the revision produced by this Delta.")]
        [Input("revisionName", "Name to be assigned to the Revision that this Delta will produce.")]
        [Input("comment", "Comment to be stored along the Revision that this Delta will produce.")]
        [Input("diffConfig", "Sets configs such as properties to be ignored in the diffing, or enable/disable property-by-property diffing.\nBy default it takes the diffConfig property of the Revision. This input can be used to override it.")]
        public static Delta Delta(List<IBHoMObject> objects, object streamId, string revisionName = null,
             string comment = null, DiffConfig diffConfig = null)
        {
            Revision revision = Create.Revision(objects, streamId, revisionName, comment, diffConfig);
            return Delta(revision, diffConfig, comment);
        }

        // ----------------------------------------------- //

    }
}

