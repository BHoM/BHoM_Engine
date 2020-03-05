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

        [Description("Creates a point load to be applied to Node elements.")]
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
                throw new ArgumentException("Point force requires either the force or the moment vector to be defined");

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

        [Description("Creates a point load to be applied to Node elements.")]
        [InputFromProperty("loadcase")]
        [Input("objects", "The collection of Nodes the load should be applied to.")]
        [InputFromProperty("force")]
        [InputFromProperty("moment")]
        [InputFromProperty("axis")]
        [Input("name", "The name of the created load.")]
        [Output("ptLoad", "The created PointLoad.")]
        public static PointLoad PointLoad(Loadcase loadcase, IEnumerable<Node> objects, Vector force = null, Vector moment = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            return PointLoad(loadcase, new BHoMGroup<Node>() { Elements = objects.ToList() }, force, moment, axis, name);
        }

        /***************************************************/
        /**** Public Methods - Deprecated               ****/
        /***************************************************/

        [Deprecated("2.3", "Method and class name updated from PointForce to PointLoad", null, "PointLoad")]
        public static PointLoad PointForce(Loadcase loadcase, BHoMGroup<Node> group, Vector force = null, Vector moment = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            return PointLoad(loadcase, group, force, moment, axis, name);

        }

        /***************************************************/

        [Deprecated("2.3", "Method and class name updated from PointForce to PointLoad", null, "PointLoad")]
        public static PointLoad PointForce(Loadcase loadcase, IEnumerable<Node> objects, Vector force = null, Vector moment = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            return PointLoad(loadcase, objects, force, moment, axis, name);
        }

        /***************************************************/

    }
}

