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

using BH.oM.Geometry;
using BH.oM.Structure.Loads;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a varying distributed load to be applied to Bar elements.")]
        [InputFromProperty("loadcase")]
        [InputFromProperty("group", "Objects")]
        [InputFromProperty("startPosition")]
        [InputFromProperty("endPosition")]
        [InputFromProperty("forceAtStart")]
        [InputFromProperty("forceAtEnd")]
        [InputFromProperty("momentAtStart")]
        [InputFromProperty("momentAtEnd")]
        [InputFromProperty("relativePositions")]
        [InputFromProperty("axis")]
        [InputFromProperty("projected")]
        [Input("name", "The name of the created load.")]
        [Output("barVarLoad", "The created BarVaryingDistributedLoad.")]
        public static BarVaryingDistributedLoad BarVaryingDistributedLoad(Loadcase loadcase, BHoMGroup<Bar> group, double startPosition = 0, Vector forceAtStart = null, Vector momentAtStart = null, double endPosition = 1, Vector forceAtEnd = null, Vector momentAtEnd = null, bool relativePositions = true, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            if ((forceAtStart == null || forceAtEnd == null) && (momentAtStart == null || momentAtEnd == null))
            {
                Base.Compute.RecordError("BarVaryingDistributedLoad requires at least the force at start and end or the moment at start and end to be defined.");
                return null;
            }

            if (startPosition < 0 || endPosition < 0)
            {
                Base.Compute.RecordError("Positions need to be greater or equal to 0.");
                return null;
            }

            if (relativePositions && (startPosition > 1 || endPosition > 1))
            {
                Base.Compute.RecordError("Positions must exist between 0 and 1 (inclusive) for relative positions set to true.");
                return null;
            }

            return new BarVaryingDistributedLoad
            {
                Loadcase = loadcase,
                Objects = group,
                StartPosition = startPosition,
                EndPosition = endPosition,
                ForceAtStart = forceAtStart ?? new Vector(),
                ForceAtEnd = forceAtEnd ?? new Vector(),
                MomentAtStart = momentAtStart ?? new Vector(),
                MomentAtEnd = momentAtEnd ?? new Vector(),
                Projected = projected,
                RelativePositions = relativePositions,
                Axis = axis,
                Name = name
            };

        }

        /***************************************************/

        [Description("Creates a varying distributed load to be applied to Bar elements.")]
        [InputFromProperty("loadcase")]
        [Input("objects", "The collection of Bars the load should be applied to.")]
        [InputFromProperty("startPosition")]
        [InputFromProperty("endPosition")]
        [InputFromProperty("forceAtStart")]
        [InputFromProperty("forceAtEnd")]
        [InputFromProperty("momentAtStart")]
        [InputFromProperty("momentAtEnd")]
        [InputFromProperty("relativePositions")]
        [InputFromProperty("axis")]
        [InputFromProperty("projected")]
        [Input("name", "The name of the created load.")]
        [Output("barVarLoad", "The created BarVaryingDistributedLoad.")]
        public static BarVaryingDistributedLoad BarVaryingDistributedLoad(Loadcase loadcase, IEnumerable<Bar> objects, double startPosition = 0, Vector forceAtStart = null, Vector momentAtStart = null, double endPosition = 1, Vector forceAtEnd = null, Vector momentAtEnd = null, bool relativePositions = true, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            BHoMGroup<Bar> group = new BHoMGroup<Bar>();
            if (objects == null)
                group = null;
            else
                group.Elements = objects.ToList();

            return BarVaryingDistributedLoad(loadcase, group, startPosition, forceAtStart, momentAtStart, endPosition, forceAtEnd, momentAtEnd, relativePositions, axis, projected, name);

        }

        /***************************************************/

    }
}





