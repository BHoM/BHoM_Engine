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

using System;
using System.Linq;
using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.oM.Facade.Elements;
using BH.oM.Facade.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Analytical.Elements;
using BH.oM.Physical.FramingProperties;
using BH.Engine.Geometry;
using BH.Engine.Analytical;
using BH.Engine.Base;
 
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Facade
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns 2D Geometry representing the frame's projected elevation extents")]
        [Input("opening", "The opening to get the frame geometry for")]
        [Output("geo", "The projected elevation extents of the frame")]

        public static IGeometry FrameGeometry2D(this Opening opening)
        {
            if (opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the Frame Geometry of a null opening.");
                return null;
            }

            PolyCurve extCrv = opening.Geometry();
            List<double> widths = new List<double>();

            if (!extCrv.IsPlanar(Tolerance.Distance))
            {
                BH.Engine.Base.Compute.RecordWarning("This method only works on planar curves. Opening " + opening.BHoM_Guid + " has non-planar curves and will be ignored.");
                return null;
            }
            
            foreach (FrameEdge edge in opening.Edges)
            {
                double width = edge.FrameEdgeProperty.WidthIntoOpening();
                widths.Add(width*-1);
            }

            if (widths.Min() == 0)
            {
                BH.Engine.Base.Compute.RecordWarning("Opening " + opening.BHoM_Guid + " has no 2D frame geometry because frame edges all have widths of zero.");
                return null;
            }

            PolyCurve intCrv = Modify.OffsetVariable(extCrv, widths);

            PlanarSurface geo = BH.Engine.Geometry.Create.PlanarSurface(extCrv, new List<ICurve> { intCrv });

            return geo;
        }

    }
}


