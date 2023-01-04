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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates Section properties for a region on the XY-Plane. /n" +
                     "The resulting properties are oriented to the XY-Plane.")]
        [Input("curves", "Non-intersecting edge curves that make up the section.")]
        [Input("tolerance", "The angleTolerance for dividing the section curves.")]
        [Output("V", "Dictionary containing the section properties for the X and Y axis.")]
        public static Dictionary<string, double> IntegrateSection(List<ICurve> curves, double tolerance = 0.04)
        {
            Dictionary<string, double> results = new Dictionary<string, double>();

            if (curves.Count == 0)
            {
                #region assing zero to everything
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

                #endregion 
                return results;
            }

            List<PolyCurve> curvesZ = Geometry.Compute.IJoin(curves);

            foreach (PolyCurve c in curvesZ)
            {
                if (!c.IsClosed())
                {
                    Engine.Base.Compute.RecordError(c.ToString() + " is not closed");
                }
            }

            int[] depth = new int[curvesZ.Count];


            //----- //how much testing is needed?
            // selfintersections, intersections, closed (...?)

            if (curvesZ.Count > 1)
            {
                // find which is in which
                for (int i = 0; i < curvesZ.Count; i++)
                    for (int j = 0; j < curvesZ.Count; j++)
                        if (i != j)
                            if (curvesZ[i].IsContaining(new List<Point>() { curvesZ[j].IStartPoint() }))
                                depth[j]++;
            }
            bool discontinius = depth.Where(x => x == 0).Count() > 1;
            for (int i = 0; i < depth.Length; i++)
            {
                if (depth[i] % 2 == 0)
                    depth[i] = 1;
                else
                    depth[i] = -1;
            }
            //-----

            double area = 0;
            for (int i = 0; i < curvesZ.Count; i++)
            {
                double areaTemp = curvesZ[i].IIntegrateRegion(0);
                if (areaTemp < 0 ^ depth[i] == -1)      // make sure that it's counter-clockwise for area and clockwise for hole
                {
                    curvesZ[i] = curvesZ[i].Flip();
                }
                area += Math.Abs(areaTemp) * depth[i];
            }

            BoundingBox box = Geometry.Query.Bounds(curvesZ.Select(x => x.IBounds()).ToList());
            
            Point min = box.Min;
            Point max = box.Max;
            double totalWidth = max.X - min.X;
            double totalHeight = max.Y - min.Y;

            //-----
            // To Polyline
            List<Polyline> pLines = new List<Polyline>();
            for (int i = 0; i < curvesZ.Count; i++)
            {
                pLines.Add(curvesZ[i].CollapseToScaledPolyline(tolerance, 0.0025, 200, Math.Max(totalHeight, totalWidth)));
            }
            Polyline pLineZ = Engine.Geometry.Compute.WetBlanketInterpretation(pLines, Tolerance.MicroDistance);

            // Rotate
            List<PolyCurve> curvesY = new List<PolyCurve>();
            for (int i = 0; i < curvesZ.Count; i++)
            {
                curvesY.Add(curvesZ[i].Rotate(Point.Origin, Vector.ZAxis, -Math.PI / 2));
                pLines[i] = pLines[i].Rotate(Point.Origin, Vector.ZAxis, -Math.PI / 2);
            }
            Polyline pLineY = Engine.Geometry.Compute.WetBlanketInterpretation(pLines, Tolerance.MicroDistance);

            // Centre Point
            double centreZ = curvesY.Sum(x => x.IIntegrateRegion(1)) / area;
            double centreY = curvesZ.Sum(x => x.IIntegrateRegion(1)) / area;

            // Moment of Inertia
            double momentOfInertiaZ = 0;
            double momentOfInertiaY = 0;

            for (int i = 0; i < curvesZ.Count; i++)
            {
                curvesZ[i] = curvesZ[i].Translate(new Vector() { X = -centreY });
                momentOfInertiaZ += curvesZ[i].IIntegrateRegion(2);
                curvesY[i] = curvesY[i].Translate(new Vector() { X = -centreZ });
                momentOfInertiaY += curvesY[i].IIntegrateRegion(2);
            }
            pLineZ = pLineZ.Translate(new Vector() { X = -centreY });
            pLineY = pLineY.Translate(new Vector() { X = -centreZ });

            // Assigning Results
            results["Area"] = area;

            results["CentreZ"] = centreZ;
            results["CentreY"] = centreY;

            results["TotalWidth"] = totalWidth;
            results["TotalDepth"] = totalHeight;

            results["Iy"] = momentOfInertiaY;
            results["Iz"] = momentOfInertiaZ;

            results["Wply"] = pLineY.PlasticModulus(curvesY, area);
            results["Wplz"] = pLineZ.PlasticModulus(curvesZ, area);

            results["Rgy"] = Math.Sqrt(results["Iy"] / area);
            results["Rgz"] = Math.Sqrt(results["Iz"] / area);

            results["Vy"] = max.X - centreY;
            results["Vpy"] = centreY - min.X;
            results["Vz"] = max.Y - centreZ;
            results["Vpz"] = centreZ - min.Y;

            results["Welz"] = results["Iz"] / Math.Max(results["Vy"], results["Vpy"]);
            results["Wely"] = results["Iy"] / Math.Max(results["Vz"], results["Vpz"]);

            if (discontinius)
            {
                Engine.Base.Compute.RecordWarning("Asy and Asz are not calculated for discontinuous sections. They have ben set to 0");
                results["Asy"] = 0;
                results["Asz"] = 0;
            }
            else
            {
                results["Asy"] = pLineZ.ShearAreaPolyline(results["Iz"]);
                results["Asz"] = pLineY.ShearAreaPolyline(results["Iy"]);
            }

            return results;
        }
    }
}



