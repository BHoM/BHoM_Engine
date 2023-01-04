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

        [Description("Defines whether two panels are geometrically identical")]
        [Input("panel", "An Environment Panel")]
        [Input("panelToCompare", "An Environment Panel to compare with the first panel")]
        [Output("isidentical", "True if the two panels are geometrically identical, false if not")]
        public static bool IsIdentical(this Panel panel, Panel panelToCompare)
        {
            //Go through building elements and compare vertices and centre points
            if (panel == null || panelToCompare == null) return false;

            List<Point> controlPoints = panel.Polyline().IControlPoints();
            List<Point> measurePoints = panelToCompare.Polyline().IControlPoints();

            if (controlPoints.Count != measurePoints.Count) return false;

            bool allPointsMatch = true;
            foreach(Point p in controlPoints)
                allPointsMatch &= measurePoints.IsContaining(p);

            return allPointsMatch;  
        }
    }
}




