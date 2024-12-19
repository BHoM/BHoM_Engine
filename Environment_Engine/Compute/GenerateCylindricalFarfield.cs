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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Environment.Elements;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        [Description("Generates the cylindrical farfield for a collection of Environment Panels")]
        [Input("panels", "A collection of Environment Panel which will define the extents of the farfield geometry")]
        [Input("resolution", "Fineness of the cylindrical shape")]
        [Input("offsetDistance", "Set the distance for expanding the radius")]
        [Input("addedHeight", "Set the distance for expansion in the positive z-direction")]
        [Output("farfield", "The cylindrical farfield represented as a collection of Environment Panels")]
        public static List<Panel> GenerateCylindricalFarfield(List<Panel> panels, int resolution, double offsetDistance = 0, double addedHeight = 0)
        {
            if (resolution < 3)
            {
                BH.Engine.Base.Compute.RecordError("The base for the three dimensional farfield cannot be generated from less than three vertices.");
                return null;
            }

            List<Point> vertices = new List<Point>();
            foreach (Panel panel in panels)
                vertices.AddRange(panel.Vertices());

            Point centroid = vertices.Average();
            double centroidZ = centroid.Z;
            List<double> distances = new List<double>();
            List<double> zValues = new List<double>();
            foreach (Point point in vertices)
            {
                zValues.Add(point.Z);
                point.Z = centroidZ;
                distances.Add(point.Distance(centroid));
            }

            centroid.Z = zValues.Min();
            double rotationAngle = (2 * Math.PI / resolution);
            double radius = (distances.Max() / Math.Cos(rotationAngle / 2)) + offsetDistance;
            double height = zValues.Max() - zValues.Min() + addedHeight;

            List<Point> baseCirclePoints = new List<Point>();
            List<Point> topCirclePoints = new List<Point>();
            Point baseCirclePoint = Geometry.Create.Point(centroid.X + radius, centroid.Y, 0);
            Point topCirclePoint = Geometry.Create.Point(centroid.X + radius, centroid.Y, height);

            for (int i = 0; i < resolution; i++)
            {
                double angle = (2 * Math.PI) * i / resolution;
                Point baseCirclePointRotated = baseCirclePoint.Rotate(centroid, Vector.ZAxis, angle);
                baseCirclePoints.Add(baseCirclePointRotated);
                Point TopCirclePointRotated = topCirclePoint.Rotate(centroid, Vector.ZAxis, angle);
                topCirclePoints.Add(TopCirclePointRotated);
            }

            baseCirclePoints.Add(baseCirclePoints.First());
            topCirclePoints.Add(topCirclePoints.First());
            Polyline basePolyline = Geometry.Create.Polyline(baseCirclePoints);
            Polyline topPolyline = Geometry.Create.Polyline(topCirclePoints);

            if (resolution <= 5)
                BH.Engine.Base.Compute.RecordWarning("Be aware, the low value of resolution might make the farfield too different from a perfect cylinder.");

            List<Polyline> rectanglePolylines = new List<Polyline>();
            foreach (Point startingVertex in baseCirclePoints)
            {
                List<Point> rectanglePoints = new List<Point>();
                rectanglePoints.Add(startingVertex);
                rectanglePoints.Add(baseCirclePoints[(baseCirclePoints.IndexOf(startingVertex) + 1) % (baseCirclePoints.Count)]);
                rectanglePoints.Add(topCirclePoints[(baseCirclePoints.IndexOf(startingVertex) + 1) % (baseCirclePoints.Count)]);
                rectanglePoints.Add(topCirclePoints[baseCirclePoints.IndexOf(startingVertex)]);
                rectanglePoints.Add(startingVertex);
                rectanglePolylines.Add(Geometry.Create.Polyline(rectanglePoints));
            }
            rectanglePolylines.RemoveAt(rectanglePolylines.Count - 1); //Because the first and last point in basePlane are the same, duplicate panels are created. Therefore the last one is removed manually.

            Panel basePanel = new Panel() { ExternalEdges = basePolyline.ToEdges() };
            Panel topPanel = new Panel() { ExternalEdges = topPolyline.ToEdges() };
            List<Panel> rectanglePanels = new List<Panel>();

            foreach (Polyline rectangle in rectanglePolylines)
                rectanglePanels.Add(new Panel() { ExternalEdges = rectangle.ToEdges() });

            List<Panel> allPanels = new List<Panel>();
            allPanels.Add(basePanel);
            allPanels.Add(topPanel);
            allPanels.AddRange(rectanglePanels);
            return allPanels;
        }
    }
}




