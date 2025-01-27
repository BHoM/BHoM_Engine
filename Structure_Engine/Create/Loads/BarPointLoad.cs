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

using BH.oM.Geometry;
using BH.oM.Structure.Loads;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using BH.Engine.Geometry;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a point load to be applied to Bar elements.")]
        [InputFromProperty("loadcase")]
        [InputFromProperty("group", "Objects")]
        [InputFromProperty("distFromA")]
        [InputFromProperty("force")]
        [InputFromProperty("moment")]
        [InputFromProperty("axis")]
        [Input("name", "The name of the created load.")]
        [Output("barPtLoad", "The created BarPointLoad.")]
        public static BarPointLoad BarPointLoad(Loadcase loadcase, BHoMGroup<Bar> group, double distFromA, Vector force = null, Vector moment = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            if (force == null && moment == null)
            {
                Base.Compute.RecordError("BarPointLoad requires at least the force or the moment vector to be defined.");
                return null;
            }

            return new BarPointLoad
            {
                Loadcase = loadcase,
                DistanceFromA = distFromA,
                Force = force == null ? new Vector() : force,
                Moment = moment == null ? new Vector() : moment,
                Objects = group,
                Axis = axis,
                Name = name
            };
        }

        /***************************************************/

        [Description("Creates a point load to be applied to Bar elements.")]
        [InputFromProperty("loadcase")]
        [Input("objects", "The collection of Bars the load should be applied to.")]
        [InputFromProperty("distFromA")]
        [InputFromProperty("force")]
        [InputFromProperty("moment")]
        [InputFromProperty("axis")]
        [Input("name", "The name of the created load.")]
        [Output("barPtLoad", "The created BarPointLoad.")]
        public static BarPointLoad BarPointLoad(Loadcase loadcase, double distFromA, IEnumerable<Bar> objects, Vector force = null, Vector moment = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            BHoMGroup<Bar> group = new BHoMGroup<Bar>();
            if (objects == null)
                group = null;
            else
                group.Elements = objects.ToList();

            return BarPointLoad(loadcase, group, distFromA, force, moment, axis, name);
        }

        /***************************************************/

    }
}






