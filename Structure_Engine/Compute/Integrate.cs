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
using System.Linq;
using System.Collections.Generic;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Base;
using BH.Engine.Base;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using BH.Engine.Spatial;
using System.ComponentModel;


namespace BH.Engine.Structure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("This method is largely replaced by the Compute.IntegrateSection() Method. \n" +
                     "Calculates section constants for a region on the XY-Plane. \n" +
                     "The resulting properties are oriented to the XY-Plane.")]
        [Input("curves", "Non-intersecting planar edge curves that make up the section. All curves should be in the global XY-plane. Curves not in this plane will be projected which might give inaccurate results.")]
        [Input("tolerance", "The distance tolerance used in the algorithm.", typeof(Length))]
        [Output("V", "Dictionary containing the section properties for the X and Y axis as well as integration slices created and used in the algorithm.")]
        public static Dictionary<string, object> Integrate(List<ICurve> curves, double tolerance = Tolerance.Distance)
        {
            Dictionary<string, object> results = new Dictionary<string, object>();

            if (curves.Any(x => x.IsNull()))
            {
                return ZeroConstantsDictionary();
            }
            else if (curves.Count == 0)
            {
                return ZeroConstantsDictionary();
            }

            BoundingBox box = Geometry.Query.Bounds(curves.Select(x => Engine.Geometry.Query.IBounds(x)).ToList());

            Point min = box.Min;
            Point max = box.Max;
            double totalWidth = max.X - min.X;
            double totalHeight = max.Y - min.Y;

            List<IntegrationSlice> verticalSlices = Geometry.Create.IntegrationSlices(curves, Vector.XAxis, totalWidth / 1000, tolerance);
            List<IntegrationSlice> horizontalSlices = Geometry.Create.IntegrationSlices(curves, Vector.YAxis, totalHeight / 1000, tolerance);

            results["VerticalSlices"] = verticalSlices;
            results["HorizontalSlices"] = horizontalSlices;


            double centreZ = 0;
            double centreY = 0;
            double area = Engine.Geometry.Query.AreaIntegration(horizontalSlices, 1, min.Y, max.Y, ref centreZ);
            Engine.Geometry.Query.AreaIntegration(verticalSlices, 1, min.X, max.X, ref centreY);

            results["Area"] = area;
            results["CentreZ"] = centreZ;
            results["CentreY"] = centreY;

            results["TotalWidth"] = totalWidth;
            results["TotalDepth"] = totalHeight;
            results["Iy"] = Geometry.Query.AreaIntegration(horizontalSlices, 1, 2, 1, centreZ);
            results["Iz"] = Geometry.Query.AreaIntegration(verticalSlices, 1, 2, 1, centreY);
            //resutlts["Sy"] = 2 * Engine.Geometry.Query.AreaIntegration(horizontalSlices, min.Y, centreZ, 1, 1, 1);
            //results["Wply"] = 2 * Math.Abs(Geometry.Query.AreaIntegration(horizontalSlices, 1, 1, 1, min.Y, centreZ, centreZ));// + Math.Abs(Geometry.Query.AreaIntegration(horizontalSlices, 1, 1, 1, centreZ, max.Y, centreZ));
            results["Wply"] = PlasticModulus(horizontalSlices, area, max.Y, min.Y);
            //resutlts["Sz"] = 2 * Engine.Geometry.Query.AreaIntegration(verticalSlices, min.X, centreY, 1, 1, 1);
            //results["Wplz"] = Math.Abs(Geometry.Query.AreaIntegration(verticalSlices, 1, 1, 1, min.X, centreY, centreY)) + Math.Abs(Geometry.Query.AreaIntegration(verticalSlices, 1, 1, 1, centreY, max.X, centreY));
            results["Wplz"] = PlasticModulus(verticalSlices, area, min.X, max.X);
            results["Rgy"] = Math.Sqrt((double)results["Iy"] / area);
            results["Rgz"] = Math.Sqrt((double)results["Iz"] / area);
            results["Vy"] = max.X - centreY;
            results["Vpy"] = centreY - min.X;
            results["Vz"] = max.Y - centreZ;
            results["Vpz"] = centreZ - min.Y;
            results["Welz"] = (double)results["Iz"] / Math.Max((double)results["Vy"], (double)results["Vpy"]);
            results["Wely"] = (double)results["Iy"] / Math.Max((double)results["Vz"], (double)results["Vpz"]);
            results["Asy"] = Query.ShearArea(verticalSlices, (double)results["Iz"], centreY);
            results["Asz"] = Query.ShearArea(horizontalSlices, (double)results["Iy"], centreZ);

            return results;
        }

        /***************************************************/

        [Description("Calculates all non-torsional section constants for a section profile based on its edge curves and translates the profile curves to be centred around the origin.")]
        [Input("profile", "The profile to integrate.")]
        [Input("tolerance", "The angleTolerance for dividing the section curves.")]
        [MultiOutput(0, "profile", "The profile used in the integration. The section curves are translated to be centred around the global origin.")]
        [MultiOutput(1, "constants", "The section constants calculated based on the provided section profile.")]
        public static Output<IProfile, Dictionary<string, double>> Integrate(IProfile profile, double tolerance = Tolerance.Distance)
        {
            if (profile.IsNull())
                return new Output<IProfile, Dictionary<string, double>> { Item1 = null, Item2 = null };

            Dictionary<string, double> results = IntegrateSection(profile.Edges.ToList(), tolerance);

            Vector adjustment = new Vector() { X = -results["CentreY"], Y = -results["CentreZ"], Z = 0 };

            results["CentreY"] = 0.0d;
            results["CentreZ"] = 0.0d;

            return new Output<IProfile, Dictionary<string, double>> { Item1 = AdjustCurves(profile, adjustment), Item2 = results };
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static IProfile AdjustCurves(IProfile profile, Vector adjustment)
        {
            //Moving all curves to ensure the global origin sits in the centroid of the section.
            IProfile clone = profile.DeepClone();

            foreach (ICurve crv in clone.Edges)
            {
                AdjustCurve(crv, adjustment);
            }

            return clone;
        }

        /***************************************************/

        private static void AdjustCurve(ICurve crv, Vector adjustment)
        {

            if (crv is Line)
            {
                Line line = crv as Line;
                line.Start = line.Start.Translate(adjustment);
                line.End = line.End.Translate(adjustment);
            }
            else if (crv is Arc)
            {
                Arc arc = crv as Arc;
                arc.CoordinateSystem.Origin = arc.CoordinateSystem.Origin.Translate(adjustment);
            }
            else if (crv is Circle)
            {
                Circle circ = crv as Circle;
                circ.Centre = circ.Centre.Translate(adjustment);
            }
            else if (crv is Ellipse)
            {
                Ellipse ellipse = crv as Ellipse;
                ellipse.Centre = ellipse.Centre.Translate(adjustment);
            }
            else if (crv is PolyCurve)
            {
                PolyCurve poly = crv as PolyCurve;
                foreach (ICurve innerCrv in poly.Curves)
                {
                    AdjustCurve(innerCrv, adjustment);
                }
            }
            else
            {
                List<Point> pts = crv.IControlPoints();

                for (int i = 0; i < pts.Count; i++)
                {
                    pts[i] = pts[i].Translate(adjustment);
                }
            }
        }

        private static Dictionary<string, object> ZeroConstantsDictionary()
        {
            Dictionary<string, object> results = new Dictionary<string, object>();

            results["Area"] = 0;

            results["CentreZ"] = 0;
            results["CentreY"] = 0;

            results["TotalWidth"] = 0;
            results["TotalDepth"] = 0;

            results["Iy"] = 0;
            results["Iz"] = 0;

            results["Wply"] = 0;
            results["Wplz"] = 0;

            results["Rgy"] = 0;
            results["Rgz"] = 0;

            results["Vy"] = 0;
            results["Vpy"] = 0;
            results["Vz"] = 0;
            results["Vpz"] = 0;

            results["Welz"] = 0;
            results["Wely"] = 0;

            results["Asy"] = 0;
            results["Asz"] = 0;

            return results;
        }


        /***************************************************/
    }
}




