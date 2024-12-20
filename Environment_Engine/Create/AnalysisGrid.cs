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
using System.Collections.ObjectModel;

using BH.oM.Environment.Analysis;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        [Description("An AnalysisGrid generated from a boundary curve representing a surface. There will not be any nodes in areas within the inner boundaries, with the analysis grid only providing analysis nodes for the surface area without openings")]
        [Input("externalBoundary", "Surface outline representing the external boundary of the surface")]
        [Input("innerBoundaries", "Holes in the surface representing openings. There will not be any analysis grid nodes within the inner boundaries")]
        [Input("id", "A unique identifier for this Analysis Grid. This should unqiuely identify only this analysis grid in a project, which is used to perform result analysis later on")]
        [Input("name", "A name for the Analysis Grid which is human readable and represents what this Analysis Grid is for, to aid engineers know where this grid is being utilised")]
        [Input("gridSpacing", "Analysis grid spacing/resolution (default = 0.2)")]
        [Input("edgeOffset", "Distance from the curve edges within which nodes will be generated (default = 0.1)")]
        [Input("pointOffset", "Distance offset from surface in normal direction (default = 0.765)")]
        [Output("analysisGrid", "An analysis grid object containing nodes with positions")]
        public static AnalysisGrid AnalysisGrid(this Polyline externalBoundary, List<Polyline> innerBoundaries = null, int id = -1, string name = "", double gridSpacing = 0.2, double edgeOffset = 0.1, double pointOffset = 0.765)
        {
            if (externalBoundary == null)
            {
                BH.Engine.Base.Compute.RecordError("ExternalBoundary must be set in order to calculate the analysis grid.");
                return null;
            }

            innerBoundaries = innerBoundaries ?? new List<Polyline>();

            if(id == -1)
                BH.Engine.Base.Compute.RecordWarning("ID has not been set to a valid ID, this may cause errors in processing results from this AnalysisGrid if it cannot be uniquely identified later");
            if (name == "")
                BH.Engine.Base.Compute.RecordWarning("Name has not been set to a valid Name, this may cause confusion in reading results from this AnalysisGrid if it cannot be attributed to the model later");

            //Get the normal from the external boundary
            Vector surfaceNormal = externalBoundary.Normal().Normalise();

            //Offset the external boundary inwards by the edgeOffset amount
            Polyline offsetCurve = externalBoundary.Offset(-edgeOffset);

            //Offset the opening curves by the edgeOffset amount
            List<Polyline> offsetInner = new List<Polyline>();
            foreach (Polyline p in innerBoundaries)
                offsetInner.Add(p.Offset(edgeOffset));

            //Project all of the geometry to the XY plane
            Vector zVector = BH.Engine.Geometry.Create.Vector(0, 0, 1);
            Plane curvePlane = offsetCurve.IFitPlane();
            Vector curvePlaneNormal = curvePlane.Normal;
            List<Point> vertices = offsetCurve.IDiscontinuityPoints();
            Point referencePoint = vertices.Min();
            Point xyReferencePoint = BH.Engine.Geometry.Create.Point(referencePoint.X, referencePoint.Y, 0);
            Vector translateVector = xyReferencePoint - referencePoint;
            Vector rotationVector = curvePlaneNormal.CrossProduct(zVector).Normalise();
            double rotationAngle = curvePlaneNormal.Angle(zVector);
            TransformMatrix transformMatrix = BH.Engine.Geometry.Create.RotationMatrix(vertices.Min(), rotationVector, rotationAngle);

            Polyline transformedCurve = offsetCurve.Transform(transformMatrix).Translate(translateVector);
            List<Polyline> transformedHoles = new List<Polyline>();
            foreach (Polyline h in offsetInner)
                transformedHoles.Add(h.Transform(transformMatrix).Translate(translateVector));

            //Create a grid of points across the flattened curves
            List<Point> vs = transformedCurve.IDiscontinuityPoints();

            List<double> xValues = vs.Select(x => x.X).ToList();
            List<double> yValues = vs.Select(x => x.Y).ToList();

            double gridSizeX = Math.Round((xValues.Max() - xValues.Min()) / gridSpacing);
            double gridSizeY = Math.Round((yValues.Max() - yValues.Min()) / gridSpacing);

            List<Point> pts = new List<Point>();
            for (int i = 0; i < gridSizeX; i++)
            {
                for (int j = 0; j < gridSizeY; j++)
                {
                    pts.Add(new Point
                    {
                        X = xValues.Min() + i * gridSpacing + 0.5 * gridSpacing,
                        Y = yValues.Min() + j * gridSpacing + 0.5 * gridSpacing
                    });
                }
            }

            //Remove points within hole curves
            List<Point> ptsCleaned = new List<Point>();
            foreach (Point pt in pts)
            {
                Polyline containedByHole = transformedHoles.Where(x => x.IsContaining(new List<Point> { pt })).FirstOrDefault();

                if(containedByHole == null)
                {
                    //Point is not contained by any opening
                    if (transformedCurve.IsContaining(new List<Point> { pt }))
                        ptsCleaned.Add(pt); //Point is contained by the external boundary
                }
            }

            //Retransform the analsyis points back to their original plane
            List<Point> ptsCleanedRetransformed = new List<Point>();
            foreach (Point p in ptsCleaned)
                ptsCleanedRetransformed.Add(p.Translate(translateVector.Reverse()).Transform(transformMatrix.Invert()).Translate(surfaceNormal * pointOffset));

            //Create the nodes for the grid
            List<Node> nodes = new List<Node>();
            for (int x = 0; x < ptsCleanedRetransformed.Count; x++)
                nodes.Add(new Node { ID = (x + 1), Position = ptsCleanedRetransformed[x] });

            AnalysisGrid grid = new AnalysisGrid(externalBoundary, new ReadOnlyCollection<Polyline>(innerBoundaries), new ReadOnlyCollection<Node>(nodes), id);
            grid.Name = name;
            
            return grid;
        }
    }
}





