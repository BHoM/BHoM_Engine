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
using BH.oM.Facade.Elements;
using BH.oM.Facade.SectionProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Analytical.Elements;
using BH.oM.Physical.FramingProperties;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
 
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

        [Description("Returns a simple rectangular geometric representation a frame edge property")]
        [Input("frameEdgeProp", "FrameEdgeProperty to get total profile width of")]
        [Output("geo", "Simplified geometric representation of the FrameEdgeProperty")]
        public static Polyline SimpleGeometry(this FrameEdgeProperty frameEdgeProp)
        {
            if(frameEdgeProp == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the simple geometry of a null frame edge property.");
                return null;
            }

            List<BoundingBox> propProfileBounds = new List<BoundingBox>();
            List<ICurve> profileOutlines = new List<ICurve>();

            if  (frameEdgeProp.SectionProperties.Count == 0)
            {
                BH.Engine.Base.Compute.RecordWarning("This FrameEdgeProperty has no SectionProperties and therefore no geometry associated with it.");
                return new Polyline();
            }

            foreach (ConstantFramingProperty prop in frameEdgeProp.SectionProperties)
            {
               if (prop.Profile != null)
                {
                    List<ICurve> crv = new List<ICurve>(prop.Profile.Edges);
                    profileOutlines.AddRange(crv);
                }
            }

            if (profileOutlines.Count == 0)
            {
                BH.Engine.Base.Compute.RecordWarning("This FrameEdgeProperty's SectionProperties have no profile geometry associated with them.");
                return new Polyline();
            }

            List<PolyCurve> crvs = profileOutlines.IJoin();
            foreach (PolyCurve outline in crvs)
            {
                propProfileBounds.Add(outline.Bounds());
            }

            BoundingBox bounds = propProfileBounds.Bounds();
            double maxY = bounds.Max.Y;
            double minY = bounds.Min.Y;
            double maxX = bounds.Max.X;
            double minX = bounds.Min.X;

            Polyline rect = Engine.Geometry.Create.Polyline(new List<Point>() { new Point { X = minX, Y = maxY }, new Point { X = maxX, Y = maxY }, new Point { X = maxX, Y = minY }, new Point { X = minX, Y = minY }, new Point { X = minX, Y = maxY } });
            return rect;
        }

    }
}




