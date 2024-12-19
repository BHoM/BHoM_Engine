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
using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Facade.Elements;
using BH.oM.Facade.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Analytical.Elements;
using BH.oM.Physical.FramingProperties;
 
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Facade
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        [Description("Returns edge type (Sill, Head, or Jamb) from a FrameEdge and the Opening it belongs to.")]
        [Input("frameEdge", "FrameEdge to get edge type from.")]
        [Input("opening", "Opening the FrameEdge belongs to.")]
        [Input("jambMinimumAngle", "Minimum angle from horizontal (in radians) at which edges are considered jambs.")]
        [Output("type", "FrameEdge type (Sill, Head, or Jamb).")]
        public static string FrameEdgeType(this FrameEdge frameEdge, Opening opening, double jambMinimumAngle = 0.7854)
        {
            if (frameEdge == null || opening == null || frameEdge.Curve == null)
                return null;
            else
            {
                ICurve crv = frameEdge.Curve;
                double zVal = Math.Abs(crv.IEndDir().Z);
                if (zVal > Math.Sin(jambMinimumAngle))
                    return "Jamb";
                else if (crv.IPointAtParameter(0.5).Z > opening.IElementVertices().Average().Z)
                    return "Head";
                else
                    return "Sill";
            }
        }

        /***************************************************/

    }
}




