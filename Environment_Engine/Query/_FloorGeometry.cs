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

using BH.Engine.Environment;
using BHG = BH.oM.Geometry;
using BH.Engine.Geometry;
using BHE = BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BHG.Polyline FloorGeometry(this BHE.Space space)
        {
            throw new NotImplementedException("Calculating the floor geometry in the space has not been implemented");
        }

        public static BHG.Polyline FloorGeometry(this List<BHE.BuildingElement> space)
        {
            BHE.BuildingElement floor = null;

            foreach(BHE.BuildingElement be in space)
            {
                double tilt = Tilt(be);
                if (tilt == 0 || tilt == 180)
                {
                    if(floor == null)
                        floor = be;
                    else
                    {
                        //Multiple elements could be a floor - assign the one with the lowest Z
                        if (floor.MinimumLevel() > be.MinimumLevel())
                            floor = be;
                    }
                }
            }

            if (floor == null) return null;

            BHG.Polyline floorGeometry = floor.PanelCurve as BHG.Polyline;

            if (floorGeometry.ControlPoints.Count < 3)
                return null;

            return floorGeometry;
        }
    }
}
