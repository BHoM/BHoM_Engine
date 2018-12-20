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
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural.Elements;

namespace BH.oM.Structural
{
    public static class XNode
    {
        public static List<Node> MergeWithin(List<Node> nodes, double tolerance)
        {
            int removed = 0;
            List<Node> result = new List<Node>();
            nodes.Sort(delegate (Node n1, Node n2)
            {
                return n1.Point.DistanceTo(BH.oM.Geometry.Point.Origin).CompareTo(n2.Point.DistanceTo(BH.oM.Geometry.Point.Origin));
            });
            result.AddRange(nodes);

            for (int i = 0; i < nodes.Count; i++)
            {
                double distance = nodes[i].Point.DistanceTo(BH.oM.Geometry.Point.Origin);
                int j = i + 1;
                while (j < nodes.Count && Math.Abs(nodes[j].Point.DistanceTo(BH.oM.Geometry.Point.Origin) - distance) < tolerance)
                {
                    if (nodes[i].Point.DistanceTo(nodes[j].Point) < tolerance)
                    {
                        nodes[j] = nodes[j].Merge(nodes[i]);
                        result.RemoveAt(i - removed++);
                        break;
                    }
                    j++;
                }
            }
            return result;
        }
    }
}
