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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Analysis;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Geometry;
using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        [Description("Gets the geometry of a Node as a Point. Method required for automatic display in UI packages")]
        [Input("node", "Node to get the Point from")]
        [Output("point", "The geometry of the Node")]
        public static Point Geometry(this Node node)
        {
            if(node == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the geometry of a null node.");
                return null;
            }

            return node.Position;
        }

        /***************************************************/

        [Description("Gets the geometry of an Edge as an ICurve")]
        [Input("edge", "Edge to get the ICurve from")]
        [Output("curve", "The geometry of the curve")]
        public static ICurve Geometry(this Edge edge)
        {
            if(edge == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the geometry of a null edge.");
                return null;
            }

            return edge.Curve;
        }
    }
}





