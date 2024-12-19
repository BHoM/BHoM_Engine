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

using BH.oM.Environment.Elements;
using BH.oM.Architecture.Elements;

using BH.Engine.Geometry;
using BH.oM.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Analytical.Elements;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        [Description("Takes a region with a Floor Perimeter and creates a collection of Environment Panels which represent the closed volume of the region. The name of the region becomes the connected space for the panels.")]
        [Input("region", "A region with a floor perimeter to extrude into a collection of panels.")]
        [Input("height", "The height of the region, as a double, to calculate the ceiling level of the region. This will be used as the Z value of the perimeter + the given height.")]
        [Input("tolerance", "The degree of tolerance on the angle calculation for collapsing the regions perimeter to a polyline. Default is equal to BH.oM.Geometry.Tolerance.Angle.")]
        [Output("panels", "A collection of Environment Panels which represent the closed volume of the region.")]
        [PreviousInputNames("region", "room, space")]
        public static List<Panel> ExtrudeToVolume(this IRegion region, double height, double tolerance = BH.oM.Geometry.Tolerance.Angle)
        {
            if (region == null)
                return new List<Panel>();

            Polyline floor = region.Perimeter.ICollapseToPolyline(tolerance);
            return floor.ExtrudeToVolume(region.Name, height);
        }

        [Description("Takes a polyline perimeter and creates a collection of Environment Panels which represent the closed volume of a space.")]
        [Input("pLine", "A polyline perimeter to extrude into a collection of panels.")]
        [Input("connectingSpaceName", "The name of the space the panels will enclose.")]
        [Input("height", "The height of the space, as a double, to calculate the ceiling level of the room. This will be used as the Z value of the perimeter + the given height.")]
        [Output("panels", "A collection of Environment Panels which represent the closed volume of the space.")]
        public static List<Panel> ExtrudeToVolume(this Polyline pLine, string connectingSpaceName, double height)
        {
            if (pLine == null)
                return new List<Panel>();

            List<Point> floorPoints = pLine.ControlPoints;
            List<Point> checkPoints = floorPoints.CullDuplicates();

            if (floorPoints.Count != (checkPoints.Count + 1))
            {
                BH.Engine.Base.Compute.RecordError("The polyline has duplicate control points and cannot be extruded to a volume.");
                return new List<Panel>();
            }                     

            List<Panel> panels = new List<Panel>();

            Panel floorPanel = new Panel { ExternalEdges = pLine.ToEdges(), Type = PanelType.Floor };
            panels.Add(floorPanel);
            
            List<Point> roofPoints = new List<Point>();
            foreach (Point p in floorPoints)
                roofPoints.Add(new Point { X = p.X, Y = p.Y, Z = p.Z + height });

            Panel roofPanel = new Panel { ExternalEdges = new Polyline { ControlPoints = roofPoints, }.ToEdges(), Type = PanelType.Ceiling };
            panels.Add(roofPanel);

            for (int a = 0; a < floorPoints.Count - 1; a++)
            {
                List<Point> panelPoints = new List<Point>();
                panelPoints.Add(floorPoints[a]);
                panelPoints.Add(new Point { X = floorPoints[a].X, Y = floorPoints[a].Y, Z = floorPoints[a].Z + height });
                panelPoints.Add(new Point { X = floorPoints[a + 1].X, Y = floorPoints[a + 1].Y, Z = floorPoints[a + 1].Z + height });
                panelPoints.Add(floorPoints[a + 1]);
                panelPoints.Add(floorPoints[a]);

                panels.Add(new Panel { ExternalEdges = new Polyline { ControlPoints = panelPoints, }.ToEdges(), Type = PanelType.Wall });
            }

            panels.ForEach(x => x.ConnectedSpaces.Add(connectingSpaceName));

            return panels;
        }
    }
}




