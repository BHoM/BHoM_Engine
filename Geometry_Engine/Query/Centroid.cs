/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;
using System;
using BH.Engine.Data;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        [Description("Queries the centroid for the given IGeometry.")]
        [Input("surface", "The PlanarSurface to get the centre of area of.")]
        [Input("tolerance", "Distance tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Distance.", typeof(Length))]
        [Output("centroid", "The Point at the centre of given PlanarSurface.")]
        public static Point ICentroid(this IGeometry igeom, double tolerance = Tolerance.Distance)
        {
            return Centroid(igeom as dynamic, tolerance);
        }

        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        [Description("Queries the centre of area for a PlanarSurface.")]
        [Input("surface", "The PlanarSurface to get the centre of area of.")]
        [Input("tolerance", "Distance tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Distance.", typeof(Length))]
        [Output("centroid", "The Point at the centre of given PlanarSurface.")]
        public static Point Centroid(this PlanarSurface surface, double tolerance = Tolerance.Distance)
        {
            return Centroid(new List<ICurve> { surface.ExternalBoundary }, surface.InternalBoundaries, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Curves lists             ****/
        /***************************************************/

        [Description("Queries the combined centre of area enclosed by a set of ICurves with an optional set of openings.\nTo give a correct result all input curves must be planar, coplanar, closed and non-self-intersecting.\nOpening curves should also be inside of outline curves.")]
        [Input("outlines", "Set of planar, coplanar, closed and non-self-intersecting ICurves to get the combined centre of area of.")]
        [Input("openings", "Set of planar, coplanar, closed and non-self-intersecting ICurves illustrating openings in initial set of outlines.")]
        [Input("tolerance", "Distance tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Distance.", typeof(Length))]
        [Output("centroid", "The Point at the centre of given geometry.")]
        public static Point Centroid(this IEnumerable<ICurve> outlines, IEnumerable<ICurve> openings = null, double tolerance = Tolerance.Distance)
        {
            openings = openings ?? new List<ICurve>();

            Point centroid = new Point();
            double area = 0;

            double outlineArea, openingArea;
            Point outlineCentroid = outlines.CurveListCentroid(out outlineArea, tolerance);

            Point openingCentroid = openings.CurveListCentroid(out openingArea, tolerance);
            area = outlineArea - openingArea;
            return new Point
            {
                X = (outlineCentroid.X - openingCentroid.X) / area,
                Y = (outlineCentroid.Y - openingCentroid.Y) / area,
                Z = (outlineCentroid.Z - openingCentroid.Z) / area,
            };
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [PreviousVersion("6.0", "BH.Engine.Geometry.Query.Centroid(BH.oM.Geometry.Polyline, System.Double)")]
        [Description("Queries the centre of area enclosed by a closed, planar, non-self-intersecting Polyline.")]
        [Input("curve", "The Polyline to get the centre of area of.")]
        [Input("tolerance", "Distance tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Distance.", typeof(Length))]
        [Output("centroid", "The Point at the centre of a region enclosed by given Polyline.")]
        public static Point Centroid(this IPolyline curve, double tolerance = Tolerance.Distance)
        {
            if (curve == null)
            {
                Base.Compute.RecordError("Input curve is null. Cannot calculate centroid.");
                return null;
            }

            if (!curve.IIsPlanar(tolerance))
            {
                Base.Compute.RecordError("Input curve is not planar. Cannot calculate centroid.");
                return null;
            }
            else if (!curve.IIsClosed(tolerance))
            {
                Base.Compute.RecordError("Input curve is not closed. Cannot calculate centroid.");
                return null;
            }
            else if (curve.IIsSelfIntersecting(tolerance))
            {
                Base.Compute.RecordError("Input curve is self-intersecting. Cannot calculate centroid.");
                return null;
            }

            double xc, yc, zc;
            double xc0 = 0, yc0 = 0, zc0 = 0;

            List<Point> controlPoints = curve.IControlPoints();
            Point pA = controlPoints[0];

            Vector normal = Normal(curve, tolerance);

            //Check if a normal could be found.
            if (normal == null)
                return null;

            for (int i = 1; i < controlPoints.Count - 2; i++)
            {
                Point pB = controlPoints[i];
                Point pC = controlPoints[i + 1];

                double triangleArea = Area(pB - pA, pC - pA);

                if (DotProduct(CrossProduct(pB - pA, pC - pA), normal) > 0)
                {
                    xc0 += ((pA.X + pB.X + pC.X) / 3) * triangleArea;
                    yc0 += ((pA.Y + pB.Y + pC.Y) / 3) * triangleArea;
                    zc0 += ((pA.Z + pB.Z + pC.Z) / 3) * triangleArea;
                }
                else
                {
                    xc0 -= ((pA.X + pB.X + pC.X) / 3) * triangleArea;
                    yc0 -= ((pA.Y + pB.Y + pC.Y) / 3) * triangleArea;
                    zc0 -= ((pA.Z + pB.Z + pC.Z) / 3) * triangleArea;
                }
            }

            double curveArea = curve.Area();

            xc = xc0 / curveArea;
            yc = yc0 / curveArea;
            zc = zc0 / curveArea;

            return new Point { X = xc, Y = yc, Z = zc };
        }

        /***************************************************/

        [PreviousVersion("6.0", "BH.Engine.Geometry.Query.Centroid(BH.oM.Geometry.PolyCurve, System.Double)")]
        [Description("Queries the centre of area enclosed by a closed, planar, non-self-intersecting PolyCurve.")]
        [Input("curve", "The PolyCurve to get the centre of area of.")]
        [Input("tolerance", "Distance tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Distance.", typeof(Length))]
        [Output("centroid", "The Point at the centre of area enclosed by given PolyCurve.")]
        public static Point Centroid(this IPolyCurve curve, double tolerance = Tolerance.Distance)
        {
            if (curve == null)
            {
                Base.Compute.RecordError("Input curve is null. Cannot calculate centroid.");
                return null;
            }

            if (!curve.IIsPlanar(tolerance))
            {
                Base.Compute.RecordError("Input curve is not planar. Cannot calculate centroid.");
                return null;
            }
            else if (!curve.IIsClosed(tolerance))
            {
                Base.Compute.RecordError("Input curve is not closed. Cannot calculate centroid.");
                return null;
            }
            else if (curve.IIsSelfIntersecting(tolerance))
            {
                Base.Compute.RecordError("Input curve is self-intersecting. Cannot calculate centroid.");
                return null;
            }

            List<ICurve> curveSubParts = curve.ISubParts().ToList();

            if (curveSubParts.Count == 1 && curveSubParts[0] is Circle)
                return (curveSubParts[0] as Circle).Centre;

            List<Point> pts = new List<Point> { curveSubParts[0].IStartPoint() };
            foreach (ICurve crv in curveSubParts)
            {
                if (crv is Line)
                    pts.Add((crv as Line).End);
                else if (crv is Arc)
                    pts.Add(crv.IEndPoint());
                else
                {
                    Base.Compute.RecordError("PolyCurve consisting of type: " + crv.GetType().Name + " is not implemented for Centroid.");
                    return null;
                }
            }

            double xc, yc, zc;
            double xc0 = 0, yc0 = 0, zc0 = 0;

            Vector normal = Normal(curve, tolerance);

            //Check if a normal could be found.
            if (normal == null)
                return null;

            Point pA = pts[0];

            for (int i = 1; i < pts.Count - 2; i++)
            {

                Point pB = pts[i];
                Point pC = pts[i + 1];

                double triangleArea = Area(pB - pA, pC - pA);

                if (DotProduct(CrossProduct(pB - pA, pC - pA), normal) > 0)
                {
                    xc0 += ((pA.X + pB.X + pC.X) / 3) * triangleArea;
                    yc0 += ((pA.Y + pB.Y + pC.Y) / 3) * triangleArea;
                    zc0 += ((pA.Z + pB.Z + pC.Z) / 3) * triangleArea;
                }
                else
                {
                    xc0 -= ((pA.X + pB.X + pC.X) / 3) * triangleArea;
                    yc0 -= ((pA.Y + pB.Y + pC.Y) / 3) * triangleArea;
                    zc0 -= ((pA.Z + pB.Z + pC.Z) / 3) * triangleArea;
                }
            }

            foreach (ICurve crv in curveSubParts)
            {
                if (crv is Arc)
                {
                    double alpha = (crv as Arc).EndAngle - (crv as Arc).StartAngle;
                    double area = (Math.Pow((crv as Arc).Radius, 2) / 2) * (alpha - Math.Sin(alpha));

                    Point p1 = crv.IStartPoint();
                    Point p2 = PointAtParameter(crv as Arc, 0.5);
                    Point p3 = crv.IEndPoint();

                    Point arcCentr = CircularSegmentCentroid(crv as Arc);

                    if (DotProduct(CrossProduct(p2 - p1, p3 - p1), normal) > 0)
                    {
                        xc0 += arcCentr.X * area;
                        yc0 += arcCentr.Y * area;
                        zc0 += arcCentr.Z * area;
                    }
                    else
                    {
                        xc0 -= arcCentr.X * area;
                        yc0 -= arcCentr.Y * area;
                        zc0 -= arcCentr.Z * area;
                    }
                }
            }

            double curveArea = curve.Area();

            xc = xc0 / curveArea;
            yc = yc0 / curveArea;
            zc = zc0 / curveArea;

            return new Point { X = xc, Y = yc, Z = zc };
        }

        /***************************************************/

        [Description("Queries the centre of area for an Ellipse.")]
        [Input("ellipse", "The Ellipse to get the centre of area of.")]
        [Input("tolerance", "Distance tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Distance.", typeof(Length))]
        [Output("centroid", "The Point at the centre of given Ellipse.")]
        public static Point Centroid(this Ellipse ellipse, double tolerance = Tolerance.Distance)
        {
            return ellipse.Centre;
        }

        /***************************************************/

        [Description("Queries the centre of area for a Circle.")]
        [Input("circle", "The Circle to get the centre of area of.")]
        [Input("tolerance", "Distance tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Distance.", typeof(Length))]
        [Output("centroid", "The Point at the centre of given Circle.")]
        public static Point Centroid(this Circle circle, double tolerance = Tolerance.Distance)
        {
            return circle.Centre;
        }

        /***************************************************/

        [Description("Queries the centre of area for a Line.")]
        [Input("line", "The Line to get the centre of area of.")]
        [Input("tolerance", "Distance tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Distance.", typeof(Length))]
        [Output("centroid", "The Point at the centre of given Line.")]
        public static Point Centroid(this Line line, double tolerance = Tolerance.Distance)
        {
            return line.PointAtParameter(0.5);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Interface method that queries the centre of area for any ICurve.")]
        [Input("curve", "The ICurve to get the centre of area of.")]
        [Input("tolerance", "Distance tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Distance.", typeof(Length))]
        [Output("centroid", "The Point at the centre of given ICurve.")]
        public static Point ICentroid(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            return Centroid(curve as dynamic, tolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static Point Centroid(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordError($"Centroid is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }


        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private static Point CircularSegmentCentroid(this Arc arc)
        {
            Point o = arc.CoordinateSystem.Origin;
            double alpha = arc.EndAngle - arc.StartAngle;
            Point mid = PointAtParameter(arc, 0.5);

            Vector v = mid - o;

            Double length = (4 * arc.Radius * Math.Pow(Math.Sin(alpha / 2), 3)) / (3 * (alpha - Math.Sin(alpha)));

            return o + ((v / arc.Radius) * length);
        }

        /***************************************************/

        private static Point CurveListCentroid(this IEnumerable<ICurve> boundaryCurves, out double area, double tolerance)
        {
            Point centroid = new Point();
            area = 0;
            List<ICurve> curveList = boundaryCurves == null ? new List<ICurve>() : boundaryCurves.ToList();
            if (curveList.Count == 1)    //If only one outline curve provided, just add it. No need for additional clustering/uninon
            {
                ICurve outline = curveList[0];
                double outlineArea = outline.IArea();
                centroid += (outline.ICentroid() * outlineArea);
                area += outlineArea;
            }
            else if (curveList.Count > 1)    //If more than one outline, some processing required
            {
                //Cluster curves based on boundingboxes overlapping
                List<List<ICurve>> clusterCurves = Data.Compute.DomainTreeClusters(curveList, x => x.IBounds().DomainBox(), (a, b) => a.SquareDistance(b) < tolerance * tolerance, (a, b) => true, 1);

                foreach (List<ICurve> curves in clusterCurves)  //For each cluster
                {
                    if (curves.Count == 1)  //If only one curve in cluster, simply add it
                    {
                        ICurve outline = curves.First();
                        double outlineArea = outline.IArea();
                        centroid += (outline.ICentroid() * outlineArea);
                        area += outlineArea;
                    }
                    else
                    {
                        foreach (ICurve outline in curves.BooleanUnion(tolerance))  //More than one curve in cluster, run BooleanUnion
                        {
                            double outlineArea = outline.IArea();
                            centroid += (outline.ICentroid() * outlineArea);
                            area += outlineArea;
                        }
                    }
                }
            }

            return centroid;
        }

        /***************************************************/

        // Fallback
        private static Point Centroid(this object obj, double tolerance = Tolerance.Distance)
        {
            BH.Engine.Base.Compute.RecordWarning($"Could not compute Centroid for {obj.GetType().Name}.");
            return null;
        }
    }
}


