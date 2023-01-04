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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        [Description("Modifies a BHoM IGeometry's coordinates to be rounded to the number of provided decimal places.")]
        [Input("geometry", "The BHoM IGeometry to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("geometry", "The modified BHoM IGeometry.")]
        public static IGeometry IRoundCoordinates(this IGeometry geometry, int decimalPlaces = 6)
        {
            return RoundCoordinates(geometry as dynamic, decimalPlaces);
        }

        /***************************************************/

        [Description("Modifies a BHoM ICurve's coordinates to be rounded to the number of provided decimal places.")]
        [Input("curve", "The BHoM ICurve to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("curve", "The modified BHoM ICurve.")]
        public static ICurve IRoundCoordinates(this ICurve curve, int decimalPlaces = 6)
        {
            return RoundCoordinates(curve as dynamic, decimalPlaces);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Modifies a BHoM Geometry Point to be rounded to the number of provided decimal places.")]
        [Input("point", "The BHoM Geometry Point to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("point", "The modified BHoM Geometry Point.")]
        public static Point RoundCoordinates(this Point point, int decimalPlaces = 6)
        {
            return new Point
            {
                X = Math.Round(point.X, decimalPlaces),
                Y = Math.Round(point.Y, decimalPlaces),
                Z = Math.Round(point.Z, decimalPlaces),
            };
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry Vector to be rounded to the number of provided decimal places.")]
        [Input("vector", "The BHoM Geometry Vector to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("vector", "The modified BHoM Geometry Vector.")]
        public static Vector RoundCoordinates(this Vector vector, int decimalPlaces = 6)
        {
            return new Vector
            {
                X = Math.Round(vector.X, decimalPlaces),
                Y = Math.Round(vector.Y, decimalPlaces),
                Z = Math.Round(vector.Z, decimalPlaces),
            };
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry Plane's Origin and Normal to be rounded to the number of provided decimal places.")]
        [Input("plane", "The BHoM Geometry Plane to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("plane", "The modified BHoM Geometry Plane.")]
        public static Plane RoundCoordinates(this Plane plane, int decimalPlaces = 6)
        {
            return new Plane()
            {
                Origin = plane.Origin.RoundCoordinates(decimalPlaces),
                Normal = plane.Normal.RoundCoordinates(decimalPlaces),
            };
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry Line's points to be rounded to the number of provided decimal places.")]
        [Input("line", "The BHoM Geometry Line to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("line", "The modified BHoM Geometry Line.")]
        public static Line RoundCoordinates(this Line line, int decimalPlaces = 6)
        {
            return new Line()
            {
                Start = line.Start.RoundCoordinates(decimalPlaces),
                End = line.End.RoundCoordinates(decimalPlaces)
            };
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry Arc's start and end coordinates to be rounded to the number of provided decimal places while maintaining the Arc's total angle.")]
        [Input("arc", "The BHoM Geometry Arc to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("curve", "The modified BHoM Geometry Arc.")]
        public static Arc RoundCoordinates(this Arc arc, int decimalPlaces = 6)
        {
            // do the rounding
            Point start = arc.StartPoint().RoundCoordinates(decimalPlaces);
            Point end = arc.EndPoint().RoundCoordinates(decimalPlaces);

            double angle = arc.Angle();
            double dist = start.Distance(end);

            if (dist == 0)
            {
                // translate the origin as one of the points were, and set both angles to that ones angle
                return new Arc()
                {
                    CoordinateSystem = arc.CoordinateSystem.Translate(start - arc.StartPoint()),
                    Radius = arc.Radius,
                    StartAngle = arc.StartAngle,
                    EndAngle = arc.StartAngle,
                };
            }

            // recalculate the radius based on not changing the total angle
            //      Consider a equal legged triangle with endpoints at the arc's endpoints
            //      we know the "top" angle and the "base" length and are solving for the last two sides length
            double radius = Math.Sqrt(
                Math.Pow(dist / (2 * Math.Tan(angle / 2)), 2) + // "Height"
                Math.Pow(dist / 2, 2)   // "half the base"
                );

            // Align the normal to the new endpoints
            Vector normal = arc.CoordinateSystem.Z.CrossProduct(end - start).CrossProduct(start - end).Normalise();

            Circle startCircle = new Circle() { Normal = normal, Centre = start, Radius = radius };
            Circle endCircle = new Circle() { Normal = normal, Centre = end, Radius = radius };

            List<Point> intersections = startCircle.CurveIntersections(endCircle).OrderBy(x => x.SquareDistance(arc.CoordinateSystem.Origin)).ToList();

            Point newOrigin = null;

            // 180degrees arc where the points got rounded away from eachother
            if (intersections.Count == 0)
            {
                newOrigin = (start + end) / 2;
                radius = newOrigin.Distance(start);
            }
            else
            {
                // Ensure that the new centre is at the same side of the start/end points
                Vector unitNormal = normal.Normalise();
                unitNormal *= angle > Math.PI ? -1 : 1;
                foreach (Point pt in intersections)
                {
                    Vector temp = (start - pt).CrossProduct(end - pt).Normalise();
                    if ((temp + unitNormal).SquareLength() >= 1)
                    {
                        newOrigin = pt;
                        break;
                    }
                }
            }

            Vector newX = (start - newOrigin).Normalise();
            oM.Geometry.CoordinateSystem.Cartesian coordClone = Create.CartesianCoordinateSystem(newOrigin, newX, Query.CrossProduct(normal, newX));

            double endAngle = (start - newOrigin).Angle(end - newOrigin);
            endAngle = angle > Math.PI ? 2 * Math.PI - endAngle : endAngle;

            Arc result = new Arc()
            {
                CoordinateSystem = coordClone,
                Radius = radius,
                StartAngle = 0,
                EndAngle = endAngle,
            };

            return result;
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry Circle's centre and radius to be rounded to the number of provided decimal places.")]
        [Input("circle", "The BHoM Geometry Circle to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("curve", "The modified BHoM Geometry Circle. As Line if zero length.")]
        public static Circle RoundCoordinates(this Circle circle, int decimalPlaces = 6)
        {
            return new Circle()
            {
                Centre = circle.Centre.RoundCoordinates(decimalPlaces),
                Radius = Math.Round(circle.Radius, decimalPlaces),
                Normal = circle.Normal,
            };
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry Ellipse's centre and radius to be rounded to the number of provided decimal places.")]
        [Input("ellipse", "The BHoM Geometry Ellipse to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("curve", "The modified BHoM Geometry Ellipse. As Line if zero length.")]
        public static Ellipse RoundCoordinates(this Ellipse ellipse, int decimalPlaces = 6)
        {
            return new Ellipse()
            {
                Centre = ellipse.Centre.RoundCoordinates(decimalPlaces),
                Radius1 = Math.Round(ellipse.Radius1, decimalPlaces),
                Radius2 = Math.Round(ellipse.Radius2, decimalPlaces),
                Axis1 = ellipse.Axis1,
                Axis2 = ellipse.Axis2
            };
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry Polyline's control points to be rounded to the number of provided decimal places.")]
        [Input("polyline", "The BHoM Geometry Polyline to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("curve", "The modified BHoM Geometry Polyline.")]
        public static Polyline RoundCoordinates(this Polyline polyline, int decimalPlaces = 6)
        {
            return new Polyline()
            {
                ControlPoints = polyline.ControlPoints.Select(x => x.RoundCoordinates(decimalPlaces)).ToList()
            };
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry PolyCurve's curves to be rounded to the number of provided decimal places.")]
        [Input("polycurve", "The BHoM Geometry PolyCurve to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("curve", "The modified BHoM Geometry PolyCurve.")]
        public static PolyCurve RoundCoordinates(this PolyCurve polycurve, int decimalPlaces = 6)
        {
            return new PolyCurve()
            {
                Curves = polycurve.Curves.Select(x => x.IRoundCoordinates(decimalPlaces)).ToList()
            };
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry NurbsCurve's control points to be rounded to the number of provided decimal places.")]
        [Input("nurbscurve", "The BHoM Geometry NurbsCurve to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("curve", "The modified BHoM Geometry NurbsCurve.")]
        public static NurbsCurve RoundCoordinates(this NurbsCurve nurbscurve, int decimalPlaces = 6)
        {
            return new NurbsCurve()
            {
                ControlPoints = nurbscurve.ControlPoints.Select(x => x.RoundCoordinates(decimalPlaces)).ToList(),
                Knots = nurbscurve.Knots.ToList(),
                Weights = nurbscurve.Weights.ToList()
            };
        }

        /***************************************************/

        [Description("Modifies a BHoM Geometry PlanarSurface's defining curves to be rounded to the number of provided decimal places.")]
        [Input("planarSurface", "The BHoM Geometry PlanarSurface to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("surface", "The modified BHoM Geometry PlanarSurface.")]
        public static PlanarSurface RoundCoordinates(this PlanarSurface planarSurface, int decimalPlaces = 6)
        {
            Vector normal = planarSurface.FitPlane().Normal;

            //If the PlanarSurface is aligned with one of the main coordinate system's planes then rounded element will get projected on this plane to keep it's planarity.
            if (Math.Abs(Math.Abs(normal.X) - 1) < Tolerance.Angle ||
                Math.Abs(Math.Abs(normal.Y) - 1) < Tolerance.Angle ||
                Math.Abs(Math.Abs(normal.Z) - 1) < Tolerance.Angle)
            {
                Plane plane = new Plane() { Origin = planarSurface.ExternalBoundary.IStartPoint().RoundCoordinates(decimalPlaces), Normal = normal.RoundCoordinates(0) };
                ICurve externalBoundary = planarSurface.ExternalBoundary.IProject(plane).IRoundCoordinates(decimalPlaces);
                List<ICurve> internalBoundaries = planarSurface.InternalBoundaries.Select(x => x.IProject(plane).IRoundCoordinates(decimalPlaces)).ToList();

                return Create.PlanarSurface(externalBoundary, internalBoundaries);
            }
            else
            {
                ICurve externalBoundary = planarSurface.ExternalBoundary.IRoundCoordinates(decimalPlaces);
                List<ICurve> internalBoundaries = planarSurface.InternalBoundaries.Select(x => x.IRoundCoordinates(decimalPlaces)).ToList();
                PlanarSurface newSurface = Create.PlanarSurface(externalBoundary, internalBoundaries);

                if (newSurface != null)
                    return newSurface;
            }

            Base.Compute.RecordWarning("Rounding the coordinates of a planar surface couldn't be achieved without losing planarity. No action has been taken.");
            return planarSurface;
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static IGeometry RoundCoordinates(this IGeometry geometry, int decimalPlaces = 6)
        {
            Base.Compute.RecordError($"RoundCoordinates is not implemented for IGeometry of type: {geometry.GetType().Name}.");
            return null;
        }

        /***************************************************/

    }
}



