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

using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        public static List<List<Point>> PointClusters(this List<Point> points, double maxDist, int minPointCount = 1)
        {
            double maxSqrDist = maxDist * maxDist;
            Point[] testPoints = points.Select(p => p.Clone()).ToArray();
            DbscanAlgorithm dbs = new DbscanAlgorithm((x, y) => Query.SquareDistance(x,y));
            List<List<Point>> clusteredPoints = dbs.ComputeClusterDbscan(allPoints: testPoints, epsilon: maxSqrDist, minPts: minPointCount);

            return clusteredPoints;
        }


        /***************************************************/
        /****        private classes and methods        ****/
        /***************************************************/
        
        private class DbscanPoint
        {
            public bool IsVisited;
            public Point ClusterPoint;
            public int ClusterId;

            public DbscanPoint(Point x)
            {
                ClusterPoint = x;
                IsVisited = false;
                ClusterId = 0;                          // unclassified
            }
        }

        /***************************************************/

        private class DbscanAlgorithm
        {
            private readonly Func<Point, Point, double> _metricFunc;

            public DbscanAlgorithm(Func<Point, Point, double> metricFunc)
            {
                this._metricFunc = metricFunc;
            }

            public List<List<Point>> ComputeClusterDbscan(Point[] allPoints, double epsilon, int minPts)
            {
                DbscanPoint[] allPointsDbscan = allPoints.Select(x => new DbscanPoint(x)).ToArray();
                int C = 0;
                for (int i = 0; i < allPointsDbscan.Length; i++)
                {
                    DbscanPoint p = allPointsDbscan[i];
                    if (p.IsVisited) continue;
                    p.IsVisited = true;

                    DbscanPoint[] neighborPts = null;
                    RegionQuery(allPointsDbscan, p.ClusterPoint, epsilon, out neighborPts);

                    if (neighborPts.Length < minPts)
                        p.ClusterId = -1;               // noise
                    else
                    {
                        C++;
                        ExpandCluster(allPointsDbscan, p, neighborPts, C, epsilon, minPts);
                    }
                }

                List<List<Point>> clusters = new List<List<Point>>(
                    allPointsDbscan
                        .Where(x => x.ClusterId > 0)
                        .GroupBy(x => x.ClusterId)
                        .Select(x => x.Select(y => y.ClusterPoint).ToList())
                    );

                return clusters;
            }

            private void ExpandCluster(DbscanPoint[] allPoints, DbscanPoint p, DbscanPoint[] neighborPts, int c, double epsilon, int minPts)
            {
                p.ClusterId = c;
                for (int i = 0; i < neighborPts.Length; i++)
                {
                    DbscanPoint pn = neighborPts[i];

                    if (!pn.IsVisited)
                    {
                        pn.IsVisited = true;
                        DbscanPoint[] neighborPts2 = null;
                        RegionQuery(allPoints, pn.ClusterPoint, epsilon, out neighborPts2);
                        if (neighborPts2.Length >= minPts)
                            neighborPts = neighborPts.Union(neighborPts2).ToArray();
                    }

                    if (pn.ClusterId == 0)
                        pn.ClusterId = c;
                }
            }

            private void RegionQuery(DbscanPoint[] allPoints, Point p, double epsilon, out DbscanPoint[] neighborPts)
            {
                neighborPts = allPoints.Where(x => _metricFunc(p, x.ClusterPoint) <= epsilon).ToArray();
            }
        }

        /***************************************************/
    }
}
