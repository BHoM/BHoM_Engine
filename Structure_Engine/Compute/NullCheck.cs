/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using System;

namespace BH.Engine.Structure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if a Bar or one of its Nodes are null and outputs relevant error message.")]
        [Input("bar", "The Bar to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("pass", "A boolean which is true if the bar passes the null check.")]
        public static bool NullCheck(this Bar bar, string methodName = "Method")
        {
            string errorMessage = $"Cannot run {methodName} because {"{0}"} is null";

            if (bar == null)
            {
                Engine.Reflection.Compute.RecordError(String.Format(errorMessage, "Bar"));
                return false;
            }
            if (bar?.StartNode?.Position == null)
            {
                Engine.Reflection.Compute.RecordError(String.Format(errorMessage, "StartNode"));
                return false;
            }
            if (bar?.EndNode?.Position == null)
            {
                Engine.Reflection.Compute.RecordError(String.Format(errorMessage, "EndNode"));
                return false;
            }

            return true;
        }

        /***************************************************/
    }
}

