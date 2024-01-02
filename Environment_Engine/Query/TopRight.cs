/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using BH.oM.Environment.Elements;
using BH.Engine.Environment;
using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the top right most point of a panel when looking from within the space to the outside")]
        [Input("panel", "The Environment Panel to get the top right most point of")]
        [Input("panelsAsSpace", "A collection of Environment Panels representing a single space")]
        [Output("TopRightPoint", "The top right most point of the panel")]
        public static Point TopRight(this Panel panel, List<Panel> panelsAsSpace)
        {
            if(panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the top right corner of a null panel.");
                return null;
            }

            if(panelsAsSpace == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the top right corner of a panel without knowing the space it belongs to.");
                return null;
            }

            return panel.Polyline().TopRight(panelsAsSpace);
        }

        [Description("Gets the top right most point of a polyline when looking from within the space to the outside")]
        [Input("polyline", "The BHoM Geometry Polyline to get the bottom right most point of")]
        [Input("panelsAsSpace", "A collection of Environment Panels representing a single space")]
        [Output("topRightPoint", "The top right most point of the panel")]
        public static Point TopRight(this Polyline polyline, List<Panel> panelsAsSpace)
        {
            if(polyline == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the top right corner of a null polyline.");
                return null;
            }

            if(panelsAsSpace == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the top right corner of a polyline without knowing the space it belongs to.");
                return null;
            }

            Vector normal = polyline.Normal();
            if (!polyline.NormalAwayFromSpace(panelsAsSpace))
                normal = polyline.Flip().Normal();
            if (normal == null)
                return null;

            Point centre = polyline.Centroid();
            if (centre == null)
                centre = polyline.ControlPoints.CullDuplicates().Average();

            Line line = new Line
            {
                Start = centre,
                End = centre.Translate(normal),
            };

            List<Point> pnts = polyline.DiscontinuityPoints();

            bool wasFlat = false;
            TransformMatrix transform = null;

            if (pnts.Min(x => Math.Round(x.Z, 6)) == pnts.Max(x => Math.Round(x.Z, 6)))
            {
                //All the points are on the same Z level - we're looking at a floor/roof
                transform = BH.Engine.Geometry.Create.RotationMatrix(polyline.Centroid(), new Vector { X = 1, Y = 0, Z = 0 }, 1.5708);
                polyline = polyline.Transform(transform);
                pnts = polyline.ControlPoints;
                line.End = line.Start.Translate(polyline.Normal());
                wasFlat = true;
            }

            Point rightMost = null;
            foreach (Point p in pnts)
            {
                if (!IsLeft(line, p) && (rightMost == null || rightMost.Z < p.Z))
                    rightMost = p;
            }

            if (wasFlat && rightMost != null)
                rightMost = rightMost.Transform(transform.Invert());

            return rightMost;
        }
    }
}




