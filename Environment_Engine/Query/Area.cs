/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;

using System.Linq;
using BH.oM.Geometry;

using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the surface area of an Environment Panel")]
        [Input("panel", "An Environment Panel object")]
        [Output("area", "The area of the panel")]
        public static double Area(this Panel panel)
        {
            return BH.Engine.Spatial.Query.Area(panel);
        }

        [Description("Returns the floor area of a space represented by Environment Panels")]
        [Input("panelsAsSpace", "A collection of Environment Panels that represent a closed space")]
        [Output("floorArea", "The floor area of the space")]
        public static double Area(this List<Panel> panelsAsSpace)
        {
            if (panelsAsSpace.FloorGeometry() == null)
                return 0;
            else
                return panelsAsSpace.FloorGeometry().Area();
        }
    }
}





