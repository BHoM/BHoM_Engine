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

using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using System.Collections.Generic;
using System.Linq;
using System;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Draws lines representing the area load, either as a grid over the element, or along the boundary of the elements.")]
        [Input("areaTempLoad", "The area load to visualise. Currently only supports area loads with Panels.")]
        [Input("scaleFactor", "Scales the lines drawn. Default scaling of 1 means 1 Kelvin per metre.")]
        [Input("displayForces", "Toggles whether forces should be displayed or not. Unused for temperature loads.")]
        [Input("displayMoments", "Toggles whether moments should be displayed or not. Unused for temperature loads.")]
        [Input("asResultants", "Toggles whether loads should be displayed as resultant vectors or as components. Unused for temperature loads.")]
        [Input("edgeDisplay", "Set to true to visualise the loads along the boundary of the elements.")]
        [Input("gridDisplay", "Set to true to visualise the load as a grid over the elements.")]
        [Output("lines", "A list of lines representing the load.")]
        public static List<ICurve> Visualize(this AreaUniformTemperatureLoad areaTempLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true, bool edgeDisplay = true, bool gridDisplay = false)
        {
            if (areaTempLoad.IsNull())
                return null;

            List<ICurve> arrows = new List<ICurve>();
            double loadFactor = areaTempLoad.TemperatureChange * 1000 * scaleFactor; //Arrow methods are scaling down force to 1/1000

            foreach (IAreaElement element in areaTempLoad.Objects.Elements)
            {
                List<List<ICurve>> allEdges = edgeDisplay ? ISubElementBoundaries(element) : new List<List<ICurve>>();
                List<Basis> allOrientations = IAllLocalOrientations(element);
                List<List<Point>> subElementGrids = gridDisplay ? ISubElementPointGrids(element) : new List<List<Point>>();

                for (int i = 0; i < allOrientations.Count; i++)
                {
                    IEnumerable<ICurve> edges = edgeDisplay ? allEdges[i] : null;
                    List<Point> pts = gridDisplay ? subElementGrids[i] : null;
                    Basis orientation = allOrientations[i];
                    Vector vector = orientation.Z * loadFactor;
                    if (edgeDisplay) arrows.AddRange(ConnectedArrows(edges, vector, true, null, 0, true));
                    if (gridDisplay) arrows.AddRange(MultipleArrows(pts, vector, true, null, 0, true));
                }
            }

            return arrows;
        }

        /***************************************************/

        [Description("Draws arrows representing the area load, either as a grid over the element, or along the boundary of the elements.")]
        [Input("areaUDL", "The area load to visualise. Currently only supports area loads with Panels.")]
        [Input("scaleFactor", "Scales the arrows drawn. Default scaling of 1 means 1 kN per metre.")]
        [Input("displayForces", "Toggles whether forces should be displayed or not.")]
        [Input("displayMoments", "Toggles whether moments should be displayed or not. Unused for area loads.")]
        [Input("asResultants", "Toggles whether loads should be displayed as resultant vectors or as components.")]
        [Input("edgeDisplay", "Set to true to visualise the loads along the boundary of the elements.")]
        [Input("gridDisplay", "Set to true to visualise the load as a grid over the elements.")]
        [Output("arrows", "A list of arrows representing the load.")]
        public static List<ICurve> Visualize(this AreaUniformlyDistributedLoad areaUDL, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true, bool edgeDisplay = true, bool gridDisplay = false)
        {
            if (areaUDL.IsNull())
                return null;

            if (!displayForces)
                return new List<ICurve>();

            List<ICurve> arrows = new List<ICurve>();
            Vector globalForceVec = areaUDL.Pressure * scaleFactor;

            foreach (IAreaElement element in areaUDL.Objects.Elements)
            {
                Vector forceVec;

                List<List<ICurve>> allEdges = edgeDisplay ? ISubElementBoundaries(element) : new List<List<ICurve>>();
                List<Basis> allOrientations = IAllLocalOrientations(element);
                List<List<Point>> subElementGrids = gridDisplay ? ISubElementPointGrids(element) : new List<List<Point>>();

                for (int i = 0; i < allOrientations.Count; i++)
                {
                    IEnumerable<ICurve> edges = edgeDisplay ? allEdges[i] : null;
                    List<Point> pts = gridDisplay ? subElementGrids[i] : null;
                    Basis orientation = allOrientations[i];

                    if (areaUDL.Axis == LoadAxis.Global)
                    {
                        if (areaUDL.Projected)
                        {
                            Vector normal = orientation.Z;
                            double scale = Math.Abs(normal.DotProduct(globalForceVec.Normalise()));
                            forceVec = globalForceVec * scale;
                        }
                        else
                        {
                            forceVec = globalForceVec;
                        }
                        orientation = Basis.XY;
                    }
                    else
                    {
                        forceVec = globalForceVec;
                    }

                    if (edgeDisplay) arrows.AddRange(ConnectedArrows(edges, forceVec, asResultants, orientation, 1, true));
                    if (gridDisplay) arrows.AddRange(MultipleArrows(pts, forceVec, asResultants, orientation, 1, true));
                }
            }

            return arrows;
        }

        /***************************************************/

        [Description("Draws arrows representing the Bar point load at its location on the Bar.")]
        [Input("barPointForce", "The Bar load to visualise.")]
        [Input("scaleFactor", "Scales the arrows drawn. Default scaling of 1 means 1 kN per metre.")]
        [Input("displayForces", "Toggles whether forces should be displayed or not.")]
        [Input("displayMoments", "Toggles whether moments should be displayed or not.")]
        [Input("asResultants", "Toggles whether loads should be displayed as resultant vectors or as components.")]
        [Output("arrows", "A list of arrows representing the load.")]
        public static List<ICurve> Visualize(this BarPointLoad barPointForce, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            if (barPointForce.IsNull())
                return null;

            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = barPointForce.Force * scaleFactor;
            Vector momentVec = barPointForce.Moment * scaleFactor;

            foreach (Bar bar in barPointForce.Objects.Elements)
            {
                Basis orientation;
                Vector[] loads = BarForceVectors(bar, forceVec, momentVec, barPointForce.Axis, barPointForce.Projected, out orientation);
                Point point = bar.Start.Position;
                Vector tan = bar.Tangent(true);
                point += tan * barPointForce.DistanceFromA;

                if (displayForces) arrows.AddRange(Arrows(point, loads[0], true, asResultants, orientation));
                if (displayMoments) arrows.AddRange(Arrows(point, loads[1], false, asResultants, orientation));
            }

            return arrows;
        }

        /***************************************************/

        [Description("Draws lines representing the Bar load over the length of the Bar elements in the load.")]
        [Input("barPrestressLoad", "The Bar load to visualise.")]
        [Input("scaleFactor", "Scales the lines drawn. Default scaling of 1 means 1 kN per metre.")]
        [Input("displayForces", "Toggles whether forces should be displayed or not.")]
        [Input("displayMoments", "Toggles whether moments should be displayed or not. Unused for Bar prestress loads.")]
        [Input("asResultants", "Toggles whether loads should be displayed as resultant vectors or as components. Unused for Bar prestress loads.")]
        [Output("lines", "A list of lines representing the load.")]
        public static List<ICurve> Visualize(this BarPrestressLoad barPrestressLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            if (barPrestressLoad.IsNull())
                return null;

            List<ICurve> arrows = new List<ICurve>();

            double scaledForce = barPrestressLoad.Prestress * scaleFactor;

            foreach (Bar bar in barPrestressLoad.Objects.Elements)
            {
                if (displayForces) arrows.AddRange(ConnectedArrows(new List<ICurve> { bar.Centreline() }, bar.Normal() * scaledForce, true, null, 0, true));
            }

            return arrows;
        }

        /***************************************************/

        [Description("Draws lines representing the Bar load over the length of the Bar elements in the load.")]
        [Input("barTempLoad", "The Bar load to visualise.")]
        [Input("scaleFactor", "Scales the lines drawn. Default scaling of 1 means 1 kN per metre.")]
        [Input("displayForces", "Toggles whether forces should be displayed or not. Unused for Bar temprature loads.")]
        [Input("displayMoments", "Toggles whether moments should be displayed or not. Unused for Bar temprature loads.")]
        [Input("asResultants", "Toggles whether loads should be displayed as resultant vectors or as components. Unused for Bar temprature loads.")]
        [Output("lines", "A list of lines representing the load.")]
        public static List<ICurve> Visualize(this BarUniformTemperatureLoad barTempLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            if (barTempLoad.IsNull())
                return null;

            List<ICurve> arrows = new List<ICurve>();
            double loadFactor = barTempLoad.TemperatureChange * 1000 * scaleFactor; //Arrow methods are scaling down force to 1/1000

            foreach (Bar bar in barTempLoad.Objects.Elements)
            {
                arrows.AddRange(ConnectedArrows(new List<ICurve> { bar.Centreline() }, bar.Normal() * loadFactor, true, null, 0, true));
            }

            return arrows;
        }

        /***************************************************/

        [Description("Draws arrows representing the Bar load over the length of the Bar elements in the load.")]
        [Input("barUDL", "The Bar load to visualise.")]
        [Input("scaleFactor", "Scales the arrows drawn. Default scaling of 1 means 1 kN per metre.")]
        [Input("displayForces", "Toggles whether forces should be displayed or not.")]
        [Input("displayMoments", "Toggles whether moments should be displayed or not.")]
        [Input("asResultants", "Toggles whether loads should be displayed as resultant vectors or as components.")]
        [Output("arrows", "A list of arrows representing the load.")]
        public static List<ICurve> Visualize(this BarUniformlyDistributedLoad barUDL, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            if (barUDL.IsNull())
                return null;

            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = barUDL.Force * scaleFactor;
            Vector momentVec = barUDL.Moment * scaleFactor;

            double sqTol = Tolerance.Distance * Tolerance.Distance;

            foreach (Bar bar in barUDL.Objects.Elements)
            {
                Basis orientation;

                Vector[] forceVectors = BarForceVectors(bar, forceVec, momentVec, barUDL.Axis, barUDL.Projected, out orientation);

                if (displayForces && forceVectors[0].SquareLength() > sqTol)
                    arrows.AddRange(ConnectedArrows(new List<ICurve> { bar.Centreline() }, forceVectors[0], asResultants, orientation, 1, true));
                if (displayMoments && forceVectors[1].SquareLength() > sqTol)
                    arrows.AddRange(ConnectedArrows(new List<ICurve> { bar.Centreline() }, forceVectors[1], asResultants, orientation, 1, false));
            }


            return arrows;
        }

        /***************************************************/

        [Description("Draws arrows representing the Bar load over the length of the Bar elements in the load.")]
        [Input("barVaryingDistLoad", "The Bar load to visualise.")]
        [Input("scaleFactor", "Scales the arrows drawn. Default scaling of 1 means 1 kN per metre.")]
        [Input("displayForces", "Toggles whether forces should be displayed or not.")]
        [Input("displayMoments", "Toggles whether moments should be displayed or not.")]
        [Input("asResultants", "Toggles whether loads should be displayed as resultant vectors or as components.")]
        [Output("arrows", "A list of arrows representing the load.")]
        public static List<ICurve> Visualize(this BarVaryingDistributedLoad barVaryingDistLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            if (barVaryingDistLoad.IsNull())
                return null;

            List<ICurve> arrows = new List<ICurve>();

            Vector forceA = barVaryingDistLoad.ForceAtStart * scaleFactor;
            Vector forceB = barVaryingDistLoad.ForceAtEnd * scaleFactor;
            Vector momentA = barVaryingDistLoad.MomentAtStart * scaleFactor;
            Vector momentB = barVaryingDistLoad.MomentAtEnd * scaleFactor;

            int divisions = 5;
            double sqTol = Tolerance.Distance * Tolerance.Distance;

            foreach (Bar bar in barVaryingDistLoad.Objects.Elements)
            {
                double length = bar.Length();

                double startLength = barVaryingDistLoad.RelativePositions ? length * barVaryingDistLoad.StartPosition : barVaryingDistLoad.StartPosition;
                double endLength = barVaryingDistLoad.RelativePositions ? length * (1.0 - barVaryingDistLoad.EndPosition) : length - barVaryingDistLoad.EndPosition;

                List<Point> pts = DistributedPoints(bar, divisions, startLength, endLength);

                Basis orientation;

                Vector[] forcesA = BarForceVectors(bar, forceA, momentA, barVaryingDistLoad.Axis, barVaryingDistLoad.Projected, out orientation);
                Vector[] forcesB = BarForceVectors(bar, forceB, momentB, barVaryingDistLoad.Axis, barVaryingDistLoad.Projected, out orientation);

                if (displayForces && (forcesA[0].SquareLength() > sqTol || forcesB[0].SquareLength() > sqTol))
                {
                    Point[] prevPt = null;
                    for (int i = 0; i < pts.Count; i++)
                    {
                        double factor = (double)i / (double)divisions;
                        Point[] basePt;
                        Vector v = (1 - factor) * forcesA[0] + factor * forcesB[0];
                        arrows.AddRange(Arrows(pts[i], v, true, asResultants, out basePt, orientation, 1));

                        if (i > 0)
                        {
                            for (int j = 0; j < basePt.Length; j++)
                            {
                                arrows.Add(new Line { Start = prevPt[j], End = basePt[j] });
                            }

                        }
                        prevPt = basePt;
                    }
                }
                if (displayMoments && (forcesA[1].SquareLength() > sqTol || forcesB[1].SquareLength() > sqTol))
                {
                    Point[] prevPt = null;
                    for (int i = 0; i < pts.Count; i++)
                    {
                        double factor = (double)i / (double)divisions;
                        Point[] basePt;
                        Vector v = (1 - factor) * forcesA[1] + factor * forcesB[1];
                        arrows.AddRange(Arrows(pts[i], v, false, asResultants, out basePt, orientation, 1));

                        //if (i > 0)
                        //{
                        //    for (int j = 0; j < basePt.Length; j++)
                        //    {
                        //        arrows.Add(new Line { Start = prevPt[j], End = basePt[j] });
                        //    }

                        //}
                        prevPt = basePt;
                    }
                }
            }


            return arrows;
        }

        /***************************************************/

        [Description("Draws arrows representing the gravity load. FOr bars it will be drawn as a series of arrows over the length of the Bar elements in the load. For panels it will be displayed as a list of arrows along the boundary on the element.")]
        [Input("gravityLoad", "The gravity load to visualise.")]
        [Input("scaleFactor", "Scales the arrows drawn. Default scaling of 1 means 1 kN per metre.")]
        [Input("displayForces", "Toggles whether forces should be displayed or not.")]
        [Input("displayMoments", "Toggles whether moments should be displayed or not. Not in use for gravity loads.")]
        [Input("asResultants", "Toggles whether loads should be displayed as resultant vectors or as components. Not in use for gravity loads.")]
        [Output("arrows", "A list of arrows representing the load.")]
        public static List<ICurve> Visualize(this GravityLoad gravityLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            if (gravityLoad.IsNull())
                return null;

            List<ICurve> arrows = new List<ICurve>();

            Vector gravityDir = gravityLoad.GravityDirection * scaleFactor * 9.80665;
            int barDivisions = 5;

            foreach (BH.oM.Base.BHoMObject obj in gravityLoad.Objects.Elements)
            {
                if (obj is Bar)
                {
                    Bar bar = obj as Bar;

                    if (bar.SectionProperty == null || bar.SectionProperty.Material == null)
                    {
                        Base.Compute.RecordWarning("Bar needs a valid sectionproperty and material to display gravity loading");
                        continue;
                    }

                    Vector loadVector = bar.SectionProperty.IMassPerMetre() * gravityDir;

                    List<Point> pts = DistributedPoints(bar, barDivisions);

                    if (displayForces) arrows.AddRange(ConnectedArrows(new List<ICurve> { bar.Centreline() }, loadVector, true, null, 1, true));
                }
                else if (obj is IAreaElement)
                {
                    IAreaElement element = obj as IAreaElement;

                    Vector loadVector = element.Property.IMassPerArea() * gravityDir;

                    if (displayForces) arrows.AddRange(ConnectedArrows(element.IEdges(), loadVector, true));
                }
                else
                {
                    Base.Compute.RecordWarning("Display for gravity loads only implemented for Bars and IAreaElements. No area elements will be displayed");
                }
            }

            return arrows;
        }

        /***************************************************/

        [Description("Draws arrows representing the point load at the location of the Nodes of the load.")]
        [Input("pointAcceleration", "The node load to visualise.")]
        [Input("scaleFactor", "Scales the arrows drawn. Default scaling of 1 means 1 m/s� per metre.")]
        [Input("displayTranslations", "Toggles whether translational acceleration should be displayed or not.")]
        [Input("displayRotations", "Toggles whether rotational acceleration should be displayed or not.")]
        [Input("asResultants", "Toggles whether loads should be displayed as resultant vectors or as components.")]
        [Output("arrows", "A list of arrows representing the load.")]
        public static List<ICurve> Visualize(this PointAcceleration pointAcceleration, double scaleFactor = 1.0, bool displayTranslations = true, bool displayRotations = true, bool asResultants = true)
        {
            if (pointAcceleration.IsNull())
                return null;

            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = pointAcceleration.TranslationalAcceleration * scaleFactor * 1000;
            Vector momentVec = pointAcceleration.RotationalAcceleration * scaleFactor * 1000;

            foreach (Node node in pointAcceleration.Objects.Elements)
            {
                if (displayTranslations) arrows.AddRange(Arrows(node.Position, forceVec, true, asResultants));
                if (displayRotations) arrows.AddRange(Arrows(node.Position, momentVec, false, asResultants));
            }

            return arrows;
        }

        /***************************************************/

        [Description("Draws arrows representing the point load at the location of the Nodes of the load.")]
        [Input("pointDisplacement", "The node load to visualise.")]
        [Input("scaleFactor", "Scales the arrows drawn. Default scaling of 1 means 1 m per metre.")]
        [Input("displayTranslations", "Toggles whether translations should be displayed or not.")]
        [Input("displayRotations", "Toggles whether rotations should be displayed or not.")]
        [Input("asResultants", "Toggles whether loads should be displayed as resultant vectors or as components.")]
        [Output("arrows", "A list of arrows representing the load.")]
        public static List<ICurve> Visualize(this PointDisplacement pointDisplacement, double scaleFactor = 1.0, bool displayTranslations = true, bool displayRotations = true, bool asResultants = true)
        {
            if (pointDisplacement.IsNull())
                return null;

            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = pointDisplacement.Translation * scaleFactor * 1000;
            Vector momentVec = pointDisplacement.Rotation * scaleFactor * 1000;

            foreach (Node node in pointDisplacement.Objects.Elements)
            {
                if (displayTranslations) arrows.AddRange(Arrows(node.Position, forceVec, true, asResultants));
                if (displayRotations) arrows.AddRange(Arrows(node.Position, momentVec, false, asResultants));
            }

            return arrows;
        }

        /***************************************************/

        [Description("Draws arrows representing the point load at the location of the Nodes of the load.")]
        [Input("pointForce", "The node load to visualise.")]
        [Input("scaleFactor", "Scales the arrows drawn. Default scaling of 1 means 1 kN per metre.")]
        [Input("displayForces", "Toggles whether forces should be displayed or not.")]
        [Input("displayMoments", "Toggles whether moments should be displayed or not.")]
        [Input("asResultants", "Toggles whether loads should be displayed as resultant vectors or as components.")]
        [Output("arrows", "A list of arrows representing the load.")]
        public static List<ICurve> Visualize(this PointLoad pointForce, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            if (pointForce.IsNull())
                return null;

            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = pointForce.Force * scaleFactor;
            Vector momentVec = pointForce.Moment * scaleFactor;

            foreach (Node node in pointForce.Objects.Elements)
            {
                if (displayForces) arrows.AddRange(Arrows(node.Position, forceVec, true, asResultants));
                if (displayMoments) arrows.AddRange(Arrows(node.Position, momentVec, false, asResultants));
            }

            return arrows;
        }

        /***************************************************/

        [Description("Draws arrows representing the point load at the location of the Nodes of the load.")]
        [Input("pointVelocity", "The node load to visualise.")]
        [Input("scaleFactor", "Scales the arrows drawn. Default scaling of 1 means 1 m/s per metre.")]
        [Input("displayTranslations", "Toggles whether translational velocity should be displayed or not.")]
        [Input("displayRotations", "Toggles whether rotational velocity should be displayed or not.")]
        [Input("asResultants", "Toggles whether loads should be displayed as resultant vectors or as components.")]
        [Output("arrows", "A list of arrows representing the load.")]
        public static List<ICurve> Visualize(this PointVelocity pointVelocity, double scaleFactor = 1.0, bool displayTranslations = true, bool displayRotations = true, bool asResultants = true)
        {
            if (pointVelocity.IsNull())
                return null;

            List<ICurve> arrows = new List<ICurve>();

            Vector forceVec = pointVelocity.TranslationalVelocity * scaleFactor * 1000;
            Vector momentVec = pointVelocity.RotationalVelocity * scaleFactor * 1000;

            foreach (Node node in pointVelocity.Objects.Elements)
            {
                if (displayTranslations) arrows.AddRange(Arrows(node.Position, forceVec, true, asResultants));
                if (displayRotations) arrows.AddRange(Arrows(node.Position, momentVec, false, asResultants));
            }

            return arrows;
        }

        /***************************************************/

        [Description("Draws arrows representing the load along the edges of the contour of the load.")]
        [Input("contourLoad", "The ContourLoad to visualise.")]
        [Input("scaleFactor", "Scales the arrows drawn. Default scaling of 1 means 1 m/s per metre.")]
        [Input("displayForces", "Toggles whether forces should be displayed or not.")]
        [Input("displayMoments", "Toggles whether moments should be displayed or not. Unused for contour loads.")]
        [Input("asResultants", "Toggles whether loads should be displayed as resultant vectors or as components.")]
        [Output("arrows", "A list of arrows representing the load.")]
        public static List<ICurve> Visualize(this ContourLoad contourLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            if (contourLoad.IsNull())
                return null;

            if (!displayForces)
                return new List<ICurve>();

            List<ICurve> arrows = new List<ICurve>();
            Vector globalForceVec = contourLoad.Force * scaleFactor;

            Vector forceVec;
            Basis orientation;

            if (contourLoad.Axis == LoadAxis.Global)
            {
                if (contourLoad.Projected)
                {
                    Vector normal = contourLoad.Contour.Normal();
                    double scale = Math.Abs(normal.DotProduct(globalForceVec.Normalise()));
                    forceVec = globalForceVec * scale;
                }
                else
                {
                    forceVec = globalForceVec;
                }
                orientation = Basis.XY;
            }
            else
            {
                orientation = LocalOrientation(contourLoad.Contour.Normal(), 0);
                forceVec = globalForceVec;
            }

            arrows.AddRange(ConnectedArrows(contourLoad.Contour.SubParts(), forceVec, asResultants, orientation, 1, true));

            return arrows;
        }

        /***************************************************/

        [Description("Draws arrows representing the load along the length of the line of the load.")]
        [Input("lineLoad", "The GeometricalLineLoad to visualise.")]
        [Input("scaleFactor", "Scales the arrows drawn. Default scaling of 1 means 1 m/s per metre.")]
        [Input("displayForces", "Toggles whether forces should be displayed or not.")]
        [Input("displayMoments", "Toggles whether moments should be displayed or not. Unused for contour loads.")]
        [Input("asResultants", "Toggles whether loads should be displayed as resultant vectors or as components.")]
        [Output("arrows", "A list of arrows representing the load.")]
        public static List<ICurve> Visualize(this GeometricalLineLoad lineLoad, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            if (lineLoad.IsNull())
                return null;

            List<ICurve> arrows = new List<ICurve>();

            Vector forceA = lineLoad.ForceA * scaleFactor;
            Vector forceB = lineLoad.ForceB * scaleFactor;
            Vector momentA = lineLoad.MomentA * scaleFactor;
            Vector momentB = lineLoad.MomentB * scaleFactor;

            int divisions = 5;
            double sqTol = Tolerance.Distance * Tolerance.Distance;

            if (lineLoad.Projected || lineLoad.Axis == LoadAxis.Local)
            {
                Engine.Base.Compute.RecordWarning("Can not currently visualize GeometricalLineLoads that are projected or in local coordinates.");
                return arrows;
            }

            List<Point> pts = lineLoad.Location.SamplePoints(divisions);

            Basis orientation = Basis.XY;

            Vector[] forcesA = new Vector[] { forceA, momentA };    //TODO: handle local orientation and projected values
            Vector[] forcesB = new Vector[] { forceB, momentB };    //TODO: handle local orientation and projected values

            if (displayForces && (forcesA[0].SquareLength() > sqTol || forcesB[0].SquareLength() > sqTol))
            {
                Point[] prevPt = null;
                for (int i = 0; i < pts.Count; i++)
                {
                    double factor = (double)i / (double)divisions;
                    Point[] basePt;
                    Vector v = (1 - factor) * forcesA[0] + factor * forcesB[0];
                    arrows.AddRange(Arrows(pts[i], v, true, asResultants, out basePt, orientation, 1));

                    if (i > 0)
                    {
                        for (int j = 0; j < basePt.Length; j++)
                        {
                            arrows.Add(new Line { Start = prevPt[j], End = basePt[j] });
                        }

                    }
                    prevPt = basePt;
                }
            }
            if (displayMoments && (forcesA[1].SquareLength() > sqTol || forcesB[1].SquareLength() > sqTol))
            {
                Point[] prevPt = null;
                for (int i = 0; i < pts.Count; i++)
                {
                    double factor = (double)i / (double)divisions;
                    Point[] basePt;
                    Vector v = (1 - factor) * forcesA[1] + factor * forcesB[1];
                    arrows.AddRange(Arrows(pts[i], v, false, asResultants, out basePt, orientation, 1));

                    prevPt = basePt;
                }

            }

            return arrows;
        }


        /***************************************************/
        /**** Public Methods Interface                  ****/
        /***************************************************/

        [Description("Draws arrows representing the load. Visualisation will depend on the load type.")]
        [Input("load", "The node load to visualise.")]
        [Input("scaleFactor", "Scales the arrows drawn. Default scaling of 1 means 1 kN per metre.")]
        [Input("displayForces", "Toggles whether forces or other translational loads should be displayed or not. .")]
        [Input("displayMoments", "Toggles whether moments or other rotational loads should be displayed or not.")]
        [Input("asResultants", "Toggles whether loads should be displayed as resultant vectors or as components.")]
        [Output("arrows", "A list of arrows representing the load.")]
        public static IEnumerable<IGeometry> IVisualize(this ILoad load, double scaleFactor = 1.0, bool displayForces = true, bool displayMoments = true, bool asResultants = true)
        {
            return load.IsNull() ? null : Visualize(load as dynamic, scaleFactor, displayForces, displayMoments, asResultants);
        }

        /***************************************************/
        /**** Private Methods - fall back               ****/
        /***************************************************/

        private static IEnumerable<IGeometry> Visualize(this ILoad load, double scaleFactor, bool displayForces, bool displayMoments, bool asResultants)
        {
            Base.Compute.RecordWarning("No load visualisation is yet implemented for load of type " + load.GetType().Name);
            return new List<IGeometry>();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<List<Point>> ISubElementPointGrids(IAreaElement element)
        {
            return SubElementPointGrids(element as dynamic);
        }

        /***************************************************/

        private static List<List<Point>> SubElementPointGrids(Panel element)
        {
            return new List<List<Point>>() { element.PointGrid() };
        }

        /***************************************************/

        private static List<List<Point>> SubElementPointGrids(FEMesh element)
        {
            return element.PointGrid();
        }

        /***************************************************/

        private static List<Basis> IAllLocalOrientations(IAreaElement element)
        {
            return AllLocalOrientations(element as dynamic);
        }

        /***************************************************/

        private static List<Basis> AllLocalOrientations(Panel element)
        {
            return new List<Basis> { element.LocalOrientation() };
        }

        /***************************************************/

        private static List<Basis> AllLocalOrientations(FEMesh element)
        {
            return element.LocalOrientations();
        }

        /***************************************************/

        private static List<List<ICurve>> ISubElementBoundaries(IAreaElement element)
        {
            return SubElementBoundaries(element as dynamic);
        }

        /***************************************************/

        private static List<List<ICurve>> SubElementBoundaries(Panel element)
        {
            return new List<List<ICurve>> { element.ElementCurves() };
        }

        /***************************************************/

        private static List<List<ICurve>> SubElementBoundaries(FEMesh element)
        {
            List<List<ICurve>> elementCurves = new List<List<ICurve>>();

            foreach (FEMeshFace face in element.Faces)
            {
                List<ICurve> faceEdges = new List<ICurve>();
                for (int i = 0; i < face.NodeListIndices.Count; i++)
                {
                    int next = (i + 1) % face.NodeListIndices.Count;
                    Line edge = new Line { Start = element.Nodes[face.NodeListIndices[i]].Position, End = element.Nodes[face.NodeListIndices[next]].Position };
                    faceEdges.Add(edge);
                }
                elementCurves.Add(faceEdges);
            }
            return elementCurves;
        }

        /***************************************************/

        private static Vector[] BarForceVectors(Bar bar, Vector globalForce, Vector globalMoment, LoadAxis axis, bool isProjected, out Basis orientation)
        {
            if (axis == LoadAxis.Global)
            {
                orientation = null;
                if (isProjected)
                {
                    Point startPos = bar.Start.Position;
                    Vector tan = bar.Tangent();

                    Vector tanUnit = tan.Normalise();
                    Vector forceUnit = globalForce.Normalise();
                    Vector momentUnit = globalMoment.Normalise();

                    double scaleFactorForce = (tanUnit - tanUnit.DotProduct(forceUnit) * forceUnit).Length();
                    double scaleFactorMoment = (tanUnit - tanUnit.DotProduct(momentUnit) * momentUnit).Length();

                    return new Vector[] { globalForce * scaleFactorForce, globalMoment * scaleFactorMoment };
                }
                else
                {
                    return new Vector[] { globalForce, globalMoment };
                }
            }
            else
            {
                orientation = (Basis)bar.CoordinateSystem();
                return new Vector[] { globalForce, globalMoment };
            }
        }

        /***************************************************/

        private static List<ICurve> Arrows(Point pt, Vector load, bool straightArrow, bool asResultant, Basis orientation = null, int nbArrowHeads = 1)
        {
            Point[] basePoints;
            return Arrows(pt, load, straightArrow, asResultant, out basePoints, orientation, nbArrowHeads);
        }

        /***************************************************/

        private static List<ICurve> Arrows(Point pt, Vector load, bool straightArrow, bool asResultant, out Point[] basePoints, Basis orientation = null, int nbArrowHeads = 1)
        {
            if (asResultant)
            {
                Vector vector;
                if (orientation == null)
                    vector = load;
                else
                    vector = orientation.X * load.X + orientation.Y * load.Y + orientation.Z * load.Z;

                basePoints = new Point[1];
                if (straightArrow)
                    return Arrow(pt, vector, out basePoints[0], nbArrowHeads);
                else
                    return ArcArrow(pt, vector, out basePoints[0]);
            }
            else
            {
                List<ICurve> arrows = new List<ICurve>();
                basePoints = new Point[3];
                Vector[] vectors;

                if (orientation == null)
                    vectors = new Vector[] { new Vector { X = load.X }, new Vector { Y = load.Y }, new Vector { Z = load.Z } };
                else
                    vectors = new Vector[] { orientation.X * load.X, orientation.Y * load.Y, orientation.Z * load.Z };

                for (int i = 0; i < 3; i++)
                {
                    if (straightArrow)
                        arrows.AddRange(Arrow(pt, vectors[i], out basePoints[i], nbArrowHeads, 0.02));
                    else
                        arrows.AddRange(ArcArrow(pt, vectors[i], out basePoints[i]));
                }
                return arrows;
            }
        }

        /***************************************************/

        private static List<ICurve> Arrow(Point pt, Vector v, int nbArrowHeads = 1, double offsetRatio = 0.0)
        {
            Point basePt;
            return Arrow(pt, v, out basePt, nbArrowHeads, offsetRatio);
        }
        /***************************************************/

        private static List<ICurve> Arrow(Point pt, Vector v, out Point basePt, int nbArrowHeads = 1, double offsetRatio = 0.0)
        {
            List<ICurve> arrow = new List<ICurve>();

            //scale from N to kN and flip to get correct arrows
            v /= -1000;

            pt = pt + v * offsetRatio;
            Point end = pt + v;

            arrow.Add(Engine.Geometry.Create.Line(pt, end));

            double length = v.Length();

            Vector tan = v / length;

            Vector v1 = Vector.XAxis;

            double dot = v1.DotProduct(tan);

            if (Math.Abs(1 - Math.Abs(dot)) < Tolerance.Angle)
            {
                v1 = Vector.YAxis;
                dot = v1.DotProduct(tan);
            }

            v1 = (v1 - dot * tan).Normalise();

            Vector v2 = v1.CrossProduct(tan).Normalise();

            v1 /= 2;
            v2 /= 2;

            double factor = length / 10;

            int m = 0;

            while (m < nbArrowHeads)
            {
                arrow.Add(Engine.Geometry.Create.Line(pt, (v1 + tan) * factor));
                arrow.Add(Engine.Geometry.Create.Line(pt, (-v1 + tan) * factor));
                arrow.Add(Engine.Geometry.Create.Line(pt, (v2 + tan) * factor));
                arrow.Add(Engine.Geometry.Create.Line(pt, (-v2 + tan) * factor));

                pt = pt + tan * factor;
                m++;
            }

            basePt = end;

            return arrow;
        }

        /***************************************************/

        private static List<ICurve> ArcArrow(Point pt, Vector v)
        {
            Point startPt;
            return ArcArrow(pt, v, out startPt);
        }

        /***************************************************/

        private static List<ICurve> ArcArrow(Point pt, Vector v, out Point startPt)
        {
            List<ICurve> arrow = new List<ICurve>();

            //Scale from Nm to kNm
            v = v / 1000;

            double length = v.Length();

            Vector cross;
            if (v.IsParallel(Vector.ZAxis) == 0)
                cross = Vector.ZAxis;
            else
                cross = Vector.YAxis;

            Vector yAxis = v.CrossProduct(cross);
            Vector xAxis = yAxis.CrossProduct(v);

            double pi4over3 = Math.PI * 4 / 3;
            Arc arc = Engine.Geometry.Create.Arc(Engine.Geometry.Create.CartesianCoordinateSystem(pt, xAxis, yAxis), length / pi4over3, 0, pi4over3);

            startPt = arc.StartPoint();

            arrow.Add(arc);

            Vector tan = -arc.EndDir();

            Vector v1 = Vector.XAxis;

            double dot = v1.DotProduct(tan);

            if (Math.Abs(1 - dot) < Tolerance.Angle)
            {
                v1 = Vector.YAxis;
                dot = v1.DotProduct(tan);
            }

            v1 = (v1 - dot * tan).Normalise();

            Vector v2 = v1.CrossProduct(tan).Normalise();

            v1 /= 2;
            v2 /= 2;

            double factor = length / 10;

            pt = arc.EndPoint();

            arrow.Add(Engine.Geometry.Create.Line(pt, (v1 + tan) * factor));
            arrow.Add(Engine.Geometry.Create.Line(pt, (-v1 + tan) * factor));
            arrow.Add(Engine.Geometry.Create.Line(pt, (v2 + tan) * factor));
            arrow.Add(Engine.Geometry.Create.Line(pt, (-v2 + tan) * factor));


            return arrow;
        }

        /***************************************************/

        private static List<ICurve> ConnectedArrows(IEnumerable<ICurve> curves, Vector vector, bool asResultant, Basis orientation = null, int nbArrowHeads = 1, bool straightArrow = true)
        {
            List<ICurve> allCurves = new List<ICurve>();
            Vector[] baseVec;

            int divisions = straightArrow ? 5 : 7;

            allCurves = MultipleArrows(curves.SelectMany(x => x.SamplePoints((int)divisions)), vector, asResultant, out baseVec, orientation, nbArrowHeads, straightArrow);
            if (straightArrow) allCurves.AddRange(curves.SelectMany(x => baseVec.Select(v => x.ITranslate(v))));

            return allCurves;
        }

        /***************************************************/

        private static List<ICurve> MultipleArrows(IEnumerable<Point> basePoints, Vector vector, bool asResultant, Basis orientation = null, int nbArrowHeads = 1, bool straightArrow = true)
        {
            Vector[] baseVec;
            return MultipleArrows(basePoints, vector, asResultant, out baseVec, orientation, nbArrowHeads, straightArrow);
        }

        /***************************************************/

        private static List<ICurve> MultipleArrows(IEnumerable<Point> basePoints, Vector vector, bool asResultant, out Vector[] baseVec, Basis orientation = null, int nbArrowHeads = 1, bool straightArrow = true)
        {
            List<ICurve> allCurves = new List<ICurve>();
            List<ICurve> arrow = new List<ICurve>();
            Point[] basePts;

            arrow = Arrows(Point.Origin, vector, straightArrow, asResultant, out basePts, orientation, nbArrowHeads);

            baseVec = basePts.Select(x => x - Point.Origin).ToArray();

            foreach (Point pt in basePoints)
            {
                Vector vec = pt - Point.Origin;
                allCurves.AddRange(arrow.Select(x => x.ITranslate(vec)));
            }

            return allCurves;
        }

        /***************************************************/



    }

}





