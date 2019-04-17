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

using BHG = BH.oM.Geometry;
using BHE = BH.oM.Environment.Elements;
using BH.Engine.Geometry;
using BH.Engine.Environment;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BHG.Point> UnmatchedElementPoints(BHE.Space space, double tolerance = BHG.Tolerance.MacroDistance)
        {
            List<BHG.Point> nonMatchingPoints = new List<oM.Geometry.Point>();

            /*foreach(BHE.BuildingElement be in space.BuildingElements)
            {
                if (be == null) continue;

                foreach(BHG.Point pt in be.BuildingElementGeometry.ICurve().ICollapseToPolyline(BHG.Tolerance.Angle).IDiscontinuityPoints())
                {
                    List<BHE.BuildingElement> elementCompare = space.BuildingElements.Where(x => x != null).ToList().FindAll(x => x.BHoM_Guid != be.BHoM_Guid);

                    BHE.BuildingElement matchingBE = elementCompare.Find(x => x.BuildingElementGeometry.ICurve().ICollapseToPolyline(BHG.Tolerance.Angle).DiscontinuityPoints().ClosestDistance(new List<BHG.Point>() { pt }) < tolerance && (x.BHoM_Guid != be.BHoM_Guid));

                    if (matchingBE == null)
                        nonMatchingPoints.Add(pt);
                }
            }*/

            return nonMatchingPoints;
        }
    }
}
