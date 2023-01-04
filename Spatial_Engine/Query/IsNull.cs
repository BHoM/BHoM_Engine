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

using BH.oM.Base.Attributes;
using BH.oM.Spatial.Layouts;
using BH.oM.Spatial.ShapeProfiles;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if an Profile is null and outputs relevant error message.")]
        [Input("profile", "The Profile to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Profile is null.")]
        public static bool IsNull(this IProfile profile, string msg = "", [CallerMemberName] string methodName = "")
        {
            if (profile == null)
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    methodName = "Method";
                }
                Base.Compute.RecordError($"Cannot evaluate {methodName} because the Profile failed a null check. {msg}");

                return true;
            }

            return false;
        }

        [Description("Checks if an Layout2D is null and outputs relevant error message.")]
        [Input("layout", "The Layout2D to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the Layout2D is null.")]
        public static bool IsNull(this ILayout2D layout, string msg = "", [CallerMemberName] string methodName = "")
        {
            if (layout == null)
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    methodName = "Method";
                }
                Base.Compute.RecordError($"Cannot evaluate {methodName} because the Layout2D failed a null check. {msg}");

                return true;
            }

            return false;
        }

        [Description("Checks if an CurveLayout is null and outputs relevant error message.")]
        [Input("layout", "The CurveLayout to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the CurveLayout is null.")]
        public static bool IsNull(this ICurveLayout layout, string msg = "", [CallerMemberName] string methodName = "")
        {
            if (layout == null)
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    methodName = "Method";
                }
                Base.Compute.RecordError($"Cannot evaluate {methodName} because the CurveLayout failed a null check. {msg}");

                return true;
            }

            return false;
        }

    }
}


