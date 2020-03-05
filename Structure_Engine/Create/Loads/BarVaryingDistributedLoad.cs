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

using BH.oM.Geometry;
using BH.oM.Structure.Loads;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

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
        [InputFromProperty("distanceFromA")]
        [InputFromProperty("distanceFromB")]
        [InputFromProperty("forceA")]
        [InputFromProperty("forceB")]
        [InputFromProperty("momentA")]
        [InputFromProperty("momentB")]
        [InputFromProperty("axis")]
        [InputFromProperty("projected")]
        [Input("name", "The name of the created load.")]
        [Output("barVarLoad", "The created BarVaryingDistributedLoad.")]
        public static BarVaryingDistributedLoad BarVaryingDistributedLoad(Loadcase loadcase, BHoMGroup<Bar> group, double distanceFromA = 0, Vector forceA = null, Vector momentA = null, double distanceFromB = 0, Vector forceB = null, Vector momentB = null, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            if ((forceA == null || forceB == null) && (momentA == null || momentB == null))
                throw new ArgumentException("Bar varying load requires either the force at A and B AND/OR the moment at A and B to be defined");

            return new BarVaryingDistributedLoad
            {
                Loadcase = loadcase,
                Objects = group,
                DistanceFromA = distanceFromA,
                DistanceFromB = distanceFromB,
                ForceA = forceA == null ? new Vector() : forceA,
                ForceB = forceB == null ? new Vector() : forceB,
                MomentA = momentA == null ? new Vector() : momentA,
                MomentB = momentB == null ? new Vector() : momentB,
                Projected = projected,
                Axis = axis,
                Name = name
            };

        }

        /***************************************************/

        [Description("Creates a varying distributed load to be applied to Bar elements.")]
        [InputFromProperty("loadcase")]
        [Input("objects", "The collection of Bars the load should be applied to.")]
        [InputFromProperty("distanceFromA")]
        [InputFromProperty("distanceFromB")]
        [InputFromProperty("forceA")]
        [InputFromProperty("forceB")]
        [InputFromProperty("momentA")]
        [InputFromProperty("momentB")]
        [InputFromProperty("axis")]
        [InputFromProperty("projected")]
        [Input("name", "The name of the created load.")]
        [Output("barVarLoad", "The created BarVaryingDistributedLoad.")]
        public static BarVaryingDistributedLoad BarVaryingDistributedLoad(Loadcase loadcase, IEnumerable<Bar> objects, double distFromA = 0, Vector forceA = null, Vector momentA = null, double distFromB = 0, Vector forceB = null, Vector momentB = null, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            return BarVaryingDistributedLoad(loadcase, new BHoMGroup<Bar>() { Elements = objects.ToList() }, distFromA, forceA, momentA, distFromB, forceB, momentB, axis, projected, name);
        }

        /***************************************************/

    }
}

