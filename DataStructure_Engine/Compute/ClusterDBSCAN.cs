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

using BH.oM.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.DataStructure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<List<T>> ClusterDBSCAN<T>(this List<T> items, Func<T, T, bool> metricFunction, int minCount = 1)
        {
            DBSCANAlgorithm<T> DBSCAN = Create.DBSCANAlgorithm<T>(metricFunction);
            return DBSCAN.ComputeClustersDBSCAN(items, minCount);
        }

        /***************************************************/

        public static List<List<T>> ComputeClustersDBSCAN<T>(this DBSCANAlgorithm<T> dbscan, List<T> allItems, int minCount)
        {
            DBSCANObject<T>[] DBSCANItems = allItems.Select(x => Create.DBSCANObject<T>(x)).ToArray();
            int c = 0;
            for (int i = 0; i < DBSCANItems.Length; i++)
            {
                DBSCANObject<T> p = DBSCANItems[i];
                if (p.IsVisited)
                    continue;
                p.IsVisited = true;

                DBSCANObject<T>[] neighbourItems = null;
                dbscan.RegionQuery(DBSCANItems, p.ClusterItem, out neighbourItems);

                if (neighbourItems.Length < minCount)
                    p.ClusterId = -1;
                else
                {
                    c++;
                    dbscan.ExpandCluster(DBSCANItems, p, neighbourItems, c, minCount);
                }
            }

            List<List<T>> clusters = new List<List<T>>(
                DBSCANItems
                    .Where(x => x.ClusterId > 0)
                    .GroupBy(x => x.ClusterId)
                    .Select(x => x.Select(y => y.ClusterItem).ToList())
                );

            return clusters;
        }


        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private static void ExpandCluster<T>(this DBSCANAlgorithm<T> dbscan, DBSCANObject<T>[] allItems, DBSCANObject<T> item, DBSCANObject<T>[] neighbourItems, int c, int minCount)
        {
            item.ClusterId = c;
            for (int i = 0; i < neighbourItems.Length; i++)
            {
                DBSCANObject<T> neighbourItem = neighbourItems[i];

                if (!neighbourItem.IsVisited)
                {
                    neighbourItem.IsVisited = true;
                    DBSCANObject<T>[] neighbourItems2 = null;
                    dbscan.RegionQuery(allItems, neighbourItem.ClusterItem, out neighbourItems2);
                    if (neighbourItems2.Length >= minCount)
                        neighbourItems = neighbourItems.Union(neighbourItems2).ToArray();
                }

                if (neighbourItem.ClusterId == 0)
                    neighbourItem.ClusterId = c;
            }
        }

        /***************************************************/

        private static void RegionQuery<T>(this DBSCANAlgorithm<T> dbscan, DBSCANObject<T>[] allItems, T queryObject, out DBSCANObject<T>[] neighbourItems)
        {
            neighbourItems = allItems.Where(x => dbscan.MetricFunction(queryObject, x.ClusterItem)).ToArray();
        }

        /***************************************************/
    }
}
