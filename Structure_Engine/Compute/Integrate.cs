/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using BH.oM.Geometry.ShapeProfiles;
using BH.oM.Reflection;
using BH.Engine.Base;

namespace BH.Engine.Structure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Dictionary<string, object> Integrate(List<ICurve> curves, double tolerance = Tolerance.Distance)
        {
            Dictionary<string, object> results = new Dictionary<string, object>();

            BoundingBox box = new BoundingBox();

            for (int i = 0; i < curves.Count; i++)
            {
                box += curves[i].IBounds();
            }

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

        public static Output<IProfile, Dictionary<string, object>> Integrate(IProfile profile, double tolerance = Tolerance.Distance)
        {
            Dictionary<string, object> results = Integrate(profile.Edges.ToList(), tolerance);

            Vector adjustment = new Vector() { X = -(double)results["CentreY"], Y = -(double)results["CentreZ"], Z = 0 };

            results["CentreY"] = 0.0d;
            results["CentreZ"] = 0.0d;

            return new Output<IProfile, Dictionary<string, object>> { Item1 = AdjustCurves(profile, adjustment), Item2 = results };
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

        /***************************************************/

        public static double PlasticModulus(List<IntegrationSlice> slices, double area, double max, double min)
        {
            double result = 0;
            slices = slices.OrderBy(x => x.Centre).ToList();

            double plasticNeutralAxis = 0;
            for (int i = 0; i < slices.Count; i++)
            {
                IntegrationSlice slice = slices[i];
                double sliceArea = slice.Length * slice.Width;
                if (result + sliceArea < area / 2)
                {
                    result += sliceArea;
                }
                else
                {
                    plasticNeutralAxis = slices[i - 1].Centre + slices[i - 1].Width / 2;
                    double diff = area / 2 - result;
                    double ratio = diff / sliceArea;
                    plasticNeutralAxis += ratio * slice.Width;


                    break;
                }
            }

            double centreTop = 0;
            double centreBot = 0;

            double areaTop = Geometry.Query.AreaIntegration(slices, 1, max, plasticNeutralAxis, ref centreTop);
            double areaBot = Geometry.Query.AreaIntegration(slices, 1, min, plasticNeutralAxis, ref centreBot);

            return areaTop * Math.Abs(centreTop - plasticNeutralAxis) + areaBot * Math.Abs(centreBot - plasticNeutralAxis);

        }

        /***************************************************/
    }
}
