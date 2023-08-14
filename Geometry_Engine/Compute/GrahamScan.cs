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
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Sorting out the boundry point of a set of points.")]
        [Input("Set of Points", "X.")]
        [Output("Boundry points", ".")]
        public static List<Point> GrahamScan(List<Point> pts)
        {
            pts = pts.OrderBy(pt => pt.Y).ToList();
            pts = pts.OrderBy(pt => Math.Atan2(pt.Y - pts[0].Y, pt.X - pts[0].X)).ToList();

            List<Point> selPts = new List<Point>();

            while (pts.Count > 0)
            {
                GrahamScanInt(ref pts, ref selPts);
            };

            pts = selPts;

            selPts.Add(selPts[0]);

            return selPts;
        }

        private static List<Point> GrahamScanInt(ref List<Point> pts, ref List<Point> selPts)
        {
            if (pts.Count > 0)
            {
                var pt = pts[0];

                if (selPts.Count <= 1)
                {
                    selPts.Add(pt);
                    pts.RemoveAt(0);
                }
                else
                {
                    var pt1 = selPts[selPts.Count - 1];
                    var pt2 = selPts[selPts.Count - 2];
                    Vector dir1 = pt1 - pt2;
                    Vector dir2 = pt - pt1;
                    var cross = Engine.Geometry.Query.CrossProduct(dir1, dir2);


                    if (cross.Z < 0)
                    {
                        selPts.RemoveAt(selPts.Count - 1);
                    }
                    else
                    {
                        selPts.Add(pt);
                        pts.RemoveAt(0);
                    }
                }
            }
            return selPts;
        }
    }
}




