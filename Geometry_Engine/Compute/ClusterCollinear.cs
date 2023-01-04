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

using BH.Engine.Base;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****          public Methods - Lines           ****/
        /***************************************************/

        public static List<List<Line>> ClusterCollinear(this List<Line> lines, double tolerance = Tolerance.Distance)
        {
            List<List<Line>> lineClusters = new List<List<Line>>();
            foreach (Line l in lines)
            {
                bool collinear = false;
                foreach (List<Line> ll in lineClusters)
                {
                    if (l.IsCollinear(ll[0], tolerance))
                    {
                        ll.Add(l.DeepClone());
                        collinear = true;
                        break;
                    }
                }

                if (!collinear)
                    lineClusters.Add(new List<Line> { l.DeepClone() });
            }

            return lineClusters;
        }

        /***************************************************/
    }
}




