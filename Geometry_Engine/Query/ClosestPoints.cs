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
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        [Description("Find the closest n-points in a point cloud")]
        [Input("cloud", "Cloud of points")]
        [Input("samplePoint", "Point about which to sample from the cloud")]
        [Input("nPoints", "Number of near points to find")]
        [MultiOutput(0, "indices", "Index of the near points within the cloud")]
        [MultiOutput(1, "distance", "Distance to near points")]
        [MultiOutput(2, "nearPoints", "Near points within the cloud")]
        public static Output<List<int>, List<double>, List<Point>> ClosestPoints(List<Point> clouds, Point samplePoint, int nPoints)
        {
            Point[] cloud = clouds.ToArray();
            // For each cloud point, get the distance to the sample point and populate the indices
            int[] nearPointIndices = new int[cloud.Length];
            double[] nearPointDistances = new double[cloud.Length];
            for (int i = 0; i < cloud.Length; i++)
            {
                nearPointIndices[i] = i;
                nearPointDistances[i] = cloud[i].Distance(samplePoint);
            }

            // Sort the arrays based on distance
            Array.Sort(nearPointDistances, nearPointIndices);

            // Get the closest n-Point indices and distances
            int[] sampleIndices = new int[nPoints];
            double[] sampleDistances = new double[nPoints];
            for (int i = 0; i < nPoints; i++)
            {
                sampleIndices[i] = nearPointIndices[i];
                sampleDistances[i] = nearPointDistances[i];
            }

            // Get the closest n-Points
            Point[] nearPoints = new Point[nPoints];
            for (int i = 0; i < sampleIndices.Length; i++)
            {
                nearPoints[i] = cloud[sampleIndices[i]];
            }

            // Return the n-closest cloud Points to the samplePoint
            return new Output<List<int>, List<double>, List<Point>>
            {
                Item1 = sampleIndices.ToList(),
                Item2 = sampleDistances.ToList(),
                Item3 = nearPoints.ToList()
            };
        }
    }
}





