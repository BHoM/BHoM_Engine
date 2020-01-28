/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates Section properties for a region")]
        [Input("curves", "Edge curves that make up the section")]
        [Input("tolerance", "The angleTolerance for dividing the section curves")]
        [Output("V", "Dictionary containing the calculated values")]
        public static Dictionary<string, object> IntegrateSection(List<ICurve> curves, double tolerance = 0.04)
        {
            Dictionary<string, object> results = new Dictionary<string, object>();

            List<PolyCurve> curvesZ = Geometry.Compute.IJoin(curves);

            foreach (PolyCurve c in curvesZ)
            {
                if (!c.IsClosed())
                {
                    Engine.Reflection.Compute.RecordError(c.ToString() + "is not closed");
                }
            }

            int[] depth = new int[curvesZ.Count];


            //----- //how much testing is needed?
            // selfintersections, intersections, closed (...?)

            //Engine.Geometry.Compute.DistributeOutlines(curvesZ); (?)
            if (curvesZ.Count > 1)
            {
                // find which is in which
                for (int i = 0; i < curvesZ.Count; i++)
                    for (int j = 0; j < curvesZ.Count; j++)
                        if (i != j)
                            if (curvesZ[i].IsContaining(curvesZ[j]))
                                depth[j]++;
            }
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

            BoundingBox box = new BoundingBox();
            for (int i = 0; i < curvesZ.Count; i++)
                box += curvesZ[i].IBounds();
            
            Point min = box.Min;
            Point max = box.Max;
            double totalWidth = max.X - min.X;
            double totalHeight = max.Y - min.Y;

            //-----
            // To Polyline
            List<Polyline> pLines = new List<Polyline>();
            for (int i = 0; i < curvesZ.Count; i++)
            {
                pLines.Add(curvesZ[i].CollapseToScaledPolyline(tolerance, 0.0025, Math.Max(totalHeight, totalWidth)));
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

            results["Rgy"] = Math.Sqrt((double)results["Iy"] / area);
            results["Rgz"] = Math.Sqrt((double)results["Iz"] / area);

            results["Vy"] = max.X - centreY;
            results["Vpy"] = centreY - min.X;
            results["Vz"] = max.Y - centreZ;
            results["Vpz"] = centreZ - min.Y;

            results["Welz"] = (double)results["Iz"] / Math.Max((double)results["Vy"], (double)results["Vpy"]);
            results["Wely"] = (double)results["Iy"] / Math.Max((double)results["Vz"], (double)results["Vpz"]);

            results["Asy"] = pLineZ.ShearAreaPolyline((double)results["Iz"]);
            results["Asz"] = pLineY.ShearAreaPolyline((double)results["Iy"]);

            // For testing
            results["pLineY"] = pLineY;
            results["pLineZ"] = pLineZ;

            return results;
        }
    }
}
