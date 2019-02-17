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
            DBSCAN<T> DBSCAN = new DBSCAN<T>(metricFunction);
            return DBSCAN.ComputeClusters(items, minCount);
        }


        /***************************************************/
        /**** Private Helper Classes                    ****/
        /***************************************************/

        private class DBScanObject<T>
        {
            internal bool IsVisited;
            internal T ClusterItem;
            internal int ClusterId;

            internal DBScanObject(T obj)
            {
                ClusterItem = obj;
                IsVisited = false;
                ClusterId = 0;
            }
        }

        /***************************************************/

        private class DBSCAN<T>
        {
            private readonly Func<T, T, bool> _metricFunction;

            internal DBSCAN(Func<T, T, bool> metricFunction)
            {
                this._metricFunction = metricFunction;
            }

            internal List<List<T>> ComputeClusters(List<T> allItems, int minCount)
            {
                DBScanObject<T>[] DBSCANItems = allItems.Select(x => new DBScanObject<T>(x)).ToArray();
                int c = 0;
                for (int i = 0; i < DBSCANItems.Length; i++)
                {
                    DBScanObject<T> p = DBSCANItems[i];
                    if (p.IsVisited)
                        continue;
                    p.IsVisited = true;

                    DBScanObject<T>[] neighbourItems = null;
                    RegionQuery(DBSCANItems, p.ClusterItem, out neighbourItems);

                    if (neighbourItems.Length < minCount)
                        p.ClusterId = -1;
                    else
                    {
                        c++;
                        ExpandCluster(DBSCANItems, p, neighbourItems, c, minCount);
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

            private void ExpandCluster(DBScanObject<T>[] allItems, DBScanObject<T> item, DBScanObject<T>[] neighbourItems, int c, int minCount)
            {
                item.ClusterId = c;
                for (int i = 0; i < neighbourItems.Length; i++)
                {
                    DBScanObject<T> neighbourItem = neighbourItems[i];

                    if (!neighbourItem.IsVisited)
                    {
                        neighbourItem.IsVisited = true;
                        DBScanObject<T>[] neighbourItems2 = null;
                        RegionQuery(allItems, neighbourItem.ClusterItem, out neighbourItems2);
                        if (neighbourItems2.Length >= minCount)
                            neighbourItems = neighbourItems.Union(neighbourItems2).ToArray();
                    }

                    if (neighbourItem.ClusterId == 0)
                        neighbourItem.ClusterId = c;
                }
            }

            private void RegionQuery(DBScanObject<T>[] allItems, T queryObject, out DBScanObject<T>[] neighbourItems)
            {
                neighbourItems = allItems.Where(x => this._metricFunction(queryObject, x.ClusterItem)).ToArray();
            }
        }

        /***************************************************/
    }
}
