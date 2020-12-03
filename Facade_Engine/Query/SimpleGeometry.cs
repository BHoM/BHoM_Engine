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

using System;
using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.oM.Facade.Elements;
using BH.oM.Facade.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Analytical.Elements;
using BH.oM.Physical.FramingProperties;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Reflection;
using System.Collections.Generic;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Facade
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a simple rectangular geometric representation a frame edge property")]
        [Input("frameEdgeProp", "FrameEdgeProperty to get total profile width of")]
        [Output("geo", "Simplified geometric representation of the FrameEdgeProperty")]

        public static Polyline SimpleGeometry(this FrameEdgeProperty frameEdgeProp)
        {
            List<BoundingBox> propProfileBounds = new List<BoundingBox>();
            List<ICurve> profileOutlines = new List<ICurve>();

            foreach (ConstantFramingProperty prop in frameEdgeProp.SectionProperties)
            {
                List<ICurve> crv = new List<ICurve>(prop.Profile.Edges);
                profileOutlines.AddRange(crv);
            }

            List<PolyCurve> crvs = profileOutlines.IJoin();
            foreach (PolyCurve outline in crvs)
            {
                propProfileBounds.Add(outline.Bounds());
            }

            BoundingBox bounds = propProfileBounds.Bounds();
            List<Line> rectEdges = bounds.Edges();
            rectEdges.CullDuplicateLines();
            Polyline rect = rectEdges.Join()[0];
            return rect;
        }

    }
}