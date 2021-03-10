/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.oM.Architecture.BuildersWork;
using BH.oM.Architecture.Elements;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Spatial.ShapeProfiles;
using BH.Engine.Geometry;
using BH.oM.Geometry.CoordinateSystem;

namespace BH.Engine.Architecture
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Distance(this Opening opening1, Opening opening2)
        {

            double minDistance = double.MaxValue;

            if (opening1 == null || opening2 == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Opening is null");
                return double.NaN;
            }

            else
            {               
                foreach (ICurve edge1 in opening1.Profile.Edges)
                {
                    foreach (ICurve edge2 in opening2.Profile.Edges)
                    {
                        Cartesian global = new Cartesian();
                        ICurve edge1O = edge1.Orient(global, opening1.CoordinateSystem);
                        ICurve edge2O = edge2.Orient(global, opening2.CoordinateSystem);

                        double dist = edge1O.Distance(edge2O);
                        if (dist < minDistance)
                        {
                            minDistance = dist;
                        }
                    }
                }
            }

            return minDistance;
            
        }

        /***************************************************/


    }
}


