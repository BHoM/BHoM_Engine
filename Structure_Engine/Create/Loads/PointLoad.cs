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
using BH.Engine.Base;
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

        [Description("Creates a point load to be applied to Nodes.")]
        [InputFromProperty("loadcase")]
        [InputFromProperty("group", "Objects")]
        [InputFromProperty("force")]
        [InputFromProperty("moment")]
        [InputFromProperty("axis")]
        [Input("name", "The name of the created load.")]
        [Output("ptLoad", "The created PointLoad.")]
        public static PointLoad PointLoad(Loadcase loadcase, BHoMGroup<Node> group, Vector force = null, Vector moment = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            if (force == null && moment == null)
            {
                Base.Compute.RecordError("PointForce requires at least the force or moment vector to be defined");
                return null;
            }

            return new PointLoad
            {
                Loadcase = loadcase,
                Objects = group,
                Force = force == null ? new Vector() : force,
                Moment = moment == null ? new Vector() : moment,
                Axis = axis,
                Name = name
            };

        }

        /***************************************************/

        [Description("Creates a point load to be applied to Nodes.")]
        [InputFromProperty("loadcase")]
        [Input("objects", "The collection of Nodes the load should be applied to.")]
        [InputFromProperty("force")]
        [InputFromProperty("moment")]
        [InputFromProperty("axis")]
        [Input("name", "The name of the created load.")]
        [Output("ptLoad", "The created PointLoad.")]
        public static PointLoad PointLoad(Loadcase loadcase, IEnumerable<Node> objects, Vector force = null, Vector moment = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            BHoMGroup<Node> group = new BHoMGroup<Node>();
            if (objects == null)
                group = null;
            else
                group.Elements = objects.ToList();

            return PointLoad(loadcase, group, force, moment, axis, name);
        }

        /***************************************************/
    }
}






