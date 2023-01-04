/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

        [Description("Creates a point acceleration to be applied to Nodes.")]
        [InputFromProperty("loadcase")]
        [InputFromProperty("group", "Objects")]
        [InputFromProperty("translationAcc", "TranslationalAcceleration")]
        [InputFromProperty("rotationAcc", "RotationalAcceleration")]
        [InputFromProperty("axis")]
        [Input("name", "The name of the created load.")]
        [Output("ptAcc", "The created PointAcceleration.")]
        public static PointAcceleration PointAcceleration(Loadcase loadcase, BHoMGroup<Node> group, Vector translationAcc = null, Vector rotationAcc = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            if (translationAcc == null && rotationAcc == null)
            {
                Base.Compute.RecordError("PointAcceleration requires at least the translation or rotation vector to be defined");
                return null;
            }


            return new PointAcceleration
            {
                Loadcase = loadcase,
                Objects = group,
                TranslationalAcceleration = translationAcc == null ? new Vector() : translationAcc,
                RotationalAcceleration = rotationAcc == null ? new Vector() : rotationAcc,
                Axis = axis,
                Name = name
            };
        }

        /***************************************************/

        [Description("Creates a point acceleration to be applied to Nodes.")]
        [InputFromProperty("loadcase")]
        [Input("objects", "The collection of Nodes the load should be applied to.")]
        [InputFromProperty("translationAcc", "TranslationalAcceleration")]
        [InputFromProperty("rotationAcc", "RotationalAcceleration")]
        [InputFromProperty("axis")]
        [Input("name", "The name of the created load.")]
        [Output("ptAcc", "The created PointAcceleration.")]
        public static PointAcceleration PointAcceleration(Loadcase loadcase, IEnumerable<Node> objects, Vector translationAcc = null, Vector rotationAcc = null, LoadAxis axis = LoadAxis.Global, string name = "")
        {
            BHoMGroup<Node> group = new BHoMGroup<Node>();
            if (objects == null)
                group = null;
            else
                group.Elements = objects.ToList();

            return PointAcceleration(loadcase, group, translationAcc, rotationAcc, axis, name);
        }

        /***************************************************/

    }
}




