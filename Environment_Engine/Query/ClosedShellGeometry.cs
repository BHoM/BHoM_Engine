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

using System;
using System.Collections.Generic;

using System.Linq;
using BH.oM.Environment.Elements;

using BH.Engine.Geometry;
using BH.oM.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the closed shell geometry for a collection of panels representing a space")]
        [Input("panelsAsSpace", "A collection of Environment Panels representing a single space")]
        [Output("polylines", "A collection of BHoM Geometry Polylines which represent the shell of the space")]
        public static List<Polyline> ClosedShellGeometry(this List<Panel> panelsAsSpace)
        {
            List<Polyline> pLinesCurtainWall = new List<Polyline>();
            List<Polyline> pLinesOther = new List<Polyline>();

            //Merge curtain panels
            foreach (Panel element in panelsAsSpace)
            {
                if (element.Type == PanelType.CurtainWall)
                    pLinesCurtainWall.Add(element.Polyline());
                else
                    pLinesOther.Add(element.Polyline());
            }

            //Add the rest of the geometries
            List<Polyline> mergedPolyLines = BH.Engine.Geometry.Compute.BooleanUnion(pLinesCurtainWall);
            mergedPolyLines.AddRange(pLinesOther);

            return mergedPolyLines;
        }
    }
}





