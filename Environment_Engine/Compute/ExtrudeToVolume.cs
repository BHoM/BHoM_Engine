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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Architecture.Elements;

using BH.Engine.Geometry;
using BH.oM.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        [Description("Takes an Architecture room with a Floor Perimieter and creates a collection of Environment Panels which represent the closed volume of the room. The name of the room becomes the connected space for the panels")]
        [Input("room", "An Architecture Room with a floor perimeter to extrude into a collection of panels")]
        [Input("height", "The height of the room, as a double, to calculate the ceiling level of the room. This will be used as the Z value of the perimeter + the given height")]
        [Output("panels", "A collection of Environment Panels which represent the closed volume of the room")]
        public static List<Panel> ExtrudeToVolume(this Room room, double height)
        {
            Polyline floor = room.Perimeter.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle);

            List<Panel> panels = new List<Panel>();

            Panel floorPanel = new Panel { ExternalEdges = floor.ToEdges(), Type = PanelType.Floor };
            panels.Add(floorPanel);

            List<Point> floorPoints = floor.ControlPoints;
            List<Point> roofPoints = new List<Point>();
            foreach (Point p in floorPoints)
                roofPoints.Add(new Point { X = p.X, Y = p.Y, Z = p.Z + height });

            Panel roofPanel = new Panel { ExternalEdges = new Polyline { ControlPoints = roofPoints, }.ToEdges(), Type = PanelType.Ceiling };
            panels.Add(roofPanel);

            for(int a = 0; a < floorPoints.Count - 1; a++)
            {
                List<Point> panelPoints = new List<Point>();
                panelPoints.Add(floorPoints[a]);
                panelPoints.Add(new Point { X = floorPoints[a].X, Y = floorPoints[a].Y, Z = floorPoints[a].Z + height });
                panelPoints.Add(new Point { X = floorPoints[a + 1].X, Y = floorPoints[a + 1].Y, Z = floorPoints[a + 1].Z + height });
                panelPoints.Add(floorPoints[a + 1]);

                panels.Add(new Panel { ExternalEdges = new Polyline { ControlPoints = panelPoints, }.ToEdges(), Type = PanelType.Wall });
            }

            panels.ForEach(x => x.ConnectedSpaces.Add(room.Name));

            return panels;
        }
    }
}
