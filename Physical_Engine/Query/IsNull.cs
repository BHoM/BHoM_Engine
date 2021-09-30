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

using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BH.Engine.Geometry;
using BH.oM.Physical.Reinforcement;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if a Reinforcement is null and outputs relevant error message.")]
        [Input("reinforcement", "The Reinforcement to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Reinforcement or its defining properties are null.")]
        public static bool IsNull(this Reinforcement reinforcement, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (reinforcement == null)
            {
                ErrorMessage(methodName, "Reinforcement", msg);
                return true;
            }
            else if (reinforcement.ShapeCode == null)
            {
                ErrorMessage(methodName, "Reinforcement ShapeCode", msg);
                return true;
            }
            else if (reinforcement.CoordinateSystem.IsNull())
            {
                ErrorMessage(methodName, "Reinforcement ShapeCode", msg);
                return true;
            }

            return false;
        }

        /***************************************************/

        [Description("Checks if a ShapeCode is null and outputs relevant error message.")]
        [Input("shapeCode", "The ShapeCode to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the ShapeCode or its defining properties are null.")]
        public static bool IsNull(this IShapeCode shapeCode, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (shapeCode == null)
            {
                ErrorMessage(methodName, "Reinforcement", msg);
                return true;
            }

            return false;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ErrorMessage(string methodName = "Method", string type = "type", string msg = "")
        {
            Reflection.Compute.RecordError($"Cannot evaluate {methodName} because the {type} is null. {msg}");
        }

    }
}