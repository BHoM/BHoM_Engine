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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Converts the list of double[] control points to a list of Points and Weights.")]
        [Input("controlPoints", "The list of control points to convert.")]
        [Input("isRational", "If true, the controlPoints are assumed to be 4d in honogenous coordinates, if false assumed to be 3d in cartesian coordinates.")]
        [MultiOutput(0, "pts", "The points resulting from the conversion from either cartesian or homogenous coordinates, depending on isRational.")]
        [MultiOutput(1, "weights", "The weights from the controlpoints if rational, otherwise a list of 1's of equal length to the list of control points.")]
        public static Output<List<Point>, List<double>> ToPointAndWeight(this List<double[]> controlPoints, bool isRational)
        {
            return ToPointAndWeight(controlPoints, isRational, false);
        }

        /***************************************************/

        [Description("Converts the list of double[] control points to a list of Points and Weights.")]
        [Input("controlPoints", "The list of control points to convert.")]
        [Input("isRational", "If true, the controlPoints are assumed to be 4d or 3d (depending on if planar) in honogenous coordinates, if false assumed to be 3d or 2d (depending on if planar) in cartesian coordinates.")]
        [Input("planar", "If true, the controlpoints are assumed to have only x and y coordinates defined, if false assumed to have x, y and z coordinates defined.")]
        [MultiOutput(0, "pts", "The points resulting from the conversion from either cartesian or homogenous coordinates, depending on isRational.")]
        [MultiOutput(1, "weights", "The weights from the controlpoints if rational, otherwise a list of 1's of equal length to the list of control points.")]
        public static Output<List<Point>, List<double>> ToPointAndWeight(this List<double[]> controlPoints, bool isRational, bool planar)
        {
            List<Point> points = new List<Point>();
            List<double> weight = new List<double>();

            if (isRational)
            {
                if (planar)
                {
                    for (int i = 0; i < controlPoints.Count; i++)
                    {
                        double[] ptW = controlPoints[i];
                        double w = ptW[2];
                        points.Add(new Point() { X = ptW[0] / w, Y = ptW[1] / w });
                        weight.Add(w);
                    }
                }
                else
                {
                    for (int i = 0; i < controlPoints.Count; i++)
                    {
                        double[] ptW = controlPoints[i];
                        double w = ptW[3];
                        points.Add(new Point() { X = ptW[0] / w, Y = ptW[1] / w, Z = ptW[2] / w });
                        weight.Add(w);
                    }
                }
            }
            else
            {
                if (planar)
                {
                    for (int i = 0; i < controlPoints.Count; i++)
                    {
                        double[] pt = controlPoints[i];
                        points.Add(new Point() { X = pt[0], Y = pt[1] });
                        weight.Add(1.0);    //All weights equal to 1.0 for non-rational
                    }
                }
                else
                {
                    for (int i = 0; i < controlPoints.Count; i++)
                    {
                        double[] pt = controlPoints[i];
                        points.Add(new Point() { X = pt[0], Y = pt[1], Z = pt[2] });
                        weight.Add(1.0);    //All weights equal to 1.0 for non-rational
                    }
                }
            }
            return new Output<List<Point>, List<double>> { Item1 = points, Item2 = weight };
        }

        /***************************************************/

    }
}
