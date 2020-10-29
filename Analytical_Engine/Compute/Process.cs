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

using System.Collections.Generic;
using System.ComponentModel;
using BH.Engine.Spatial;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Dimensional;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Analytical
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Execute processes assigned to a relation")]
        [Input("process", "IProcess to execute.")]
        [Input("source", "IBHoMObject that is the source of this relation.")]
        [Input("target", "IBHoMObject that is the target of this relation.")]
        [Output("process result", "Results from the process.")]
        public static ProcessResult IProcess(this IProcess process, IBHoMObject source, IBHoMObject target)
        {
            return Process(process as dynamic, source, target);
        }

        /***************************************************/

        [Description("Execute processes assigned to a relation")]
        [Input("process", "ColumnGridProcess to execute.")]
        [Input("source", "IBHoMObject that is the source of this relation.")]
        [Input("target", "IBHoMObject that is the target of this relation.")]
        [Output("process result", "Results from the process.")]
        public static ProcessResult Process(this ColumnGridProcess process, IBHoMObject source, IBHoMObject target)
        {
            IElement1D sourceElement = source as IElement1D;
            IElement1D targetElement = target as IElement1D;
            double distance = Geometry.Query.Distance(sourceElement.IGeometry(), targetElement.IGeometry());
            bool passed = distance < process.Tolerance;
            ColumnGridResult processResult = new ColumnGridResult(process.BHoM_Guid, "column grid result", -1, distance, passed);
            return processResult;
        }
        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        private static ProcessResult Process(this IProcess process, IBHoMObject source, IBHoMObject target)
        {
            // Do nothing
            return null;
        }

        /***************************************************/
    }
}
