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

using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.Engine.Geometry;
using System.ComponentModel;

using BH.oM.Reflection.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if a Bar is vertical. A Bar is vertical if its projected length to the horizontal plane is less than 0.0001, i.e. a tolerance of 0.1mm on verticality. \n" +
                     "For general structural conventions please see  https://github.com/BHoM/documentation/wiki/BHoM-Structural-Conventions.")]
        [Input("bar", "The Bar to check for verticality.")]
        [Output("result", "Returns true if the Bar is vertical.")]
        public static bool IsVertical(this Bar bar)
        {
            return bar.IsNull() ? false : Engine.Geometry.Query.IsVertical(bar.Centreline());
        }

        /***************************************************/

        [Deprecated("3.1", "Moved to Geometry_Engine.")]
        public static bool IsVertical(this Line line)
        {
            return Engine.Geometry.Query.IsVertical(line);
        }

        /***************************************************/

    }
}

