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
using BH.Engine.Geometry;
using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BHG.Polyline> ClosedShellGeometry(this List<BuildingElement> buildingElements)
        {
            List<BHG.Polyline> pLinesCurtainWall = new List<BHG.Polyline>();
            List<BHG.Polyline> pLinesOther = new List<BHG.Polyline>();

            //Merge curtain panels
            foreach (BuildingElement element in buildingElements)
            {
                BH.oM.Environment.Properties.ElementProperties beProperty = element.ElementProperties() as BH.oM.Environment.Properties.ElementProperties;
                BHG.Polyline pline = new BHG.Polyline() { ControlPoints = element.PanelCurve.IControlPoints() };

                if (beProperty != null && beProperty.BuildingElementType == BuildingElementType.CurtainWall)
                    pLinesCurtainWall.Add(pline);
                else
                    pLinesOther.Add(pline);
            }

            //Add the rest of the geometries
            List<BHG.Polyline> mergedPolyLines = BH.Engine.Geometry.Compute.BooleanUnion(pLinesCurtainWall);
            mergedPolyLines.AddRange(pLinesOther);

            return mergedPolyLines;
        }
    }
}
