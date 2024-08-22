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

using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BH.oM.Base.Attributes;
using BH.oM.Ground;
using BH.oM.Quantities.Attributes;
using BH.Engine.Base;
using BH.Engine.Geometry;


namespace BH.Engine.Ground
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if a Borehole or its defining properties are valid and outputs relevant error message.")]
        [Input("borehole", "The Borehole to test for validity.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("isNull", "True if the Borehole or its defining properties are valid.")]
        public static bool IsValid(this Borehole borehole, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (borehole == null)
            {
                Base.Compute.RecordError("The borehole is null.");
                return false;
            }

            if (borehole.Id == "")
            {
                Base.Compute.RecordError("The borehole does not contain an ID.");
                return false;
            }

            if (borehole.Top == null || borehole.Bottom == null)
            {
                Base.Compute.RecordError("The top or bottom of the Borehole is null.");
                return false;
            }

            return true;
        }

        [Description("Checks if a Strata or its defining properties are valid and outputs relevant error message.")]
        [Input("strata", "The Strata to test for validity.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("isNull", "True if the Strata or its defining properties are valid.")]
        public static bool IsValid(this Stratum strata, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (strata == null)
            {
                Base.Compute.RecordError("The stratum is null.");
                return false;
            }

            if (strata.LogDescription.Trim() == "")
            {
                Base.Compute.RecordError("The LogDescription is empty.");
                return false;
            }

            return true;
        }

        [Description("Checks if a IBoreholeProperty is valid and outputs relevant error message.")]
        [Input("property", "The IBoreholeProperty to test for validity.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("isNull", "True if the IBoreholeProperty or its defining properties are valid.")]
        public static bool IsValid(this IBoreholeProperty property, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (property == null)
            {
                ErrorMessage(methodName, property.GetType().ToString(), msg);
                return false;
            }

            return true;
        }

        [Description("Checks if a IStratumProperties is valid and outputs relevant error message.")]
        [Input("property", "The IStratumProperties to test for validity.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("isNull", "True if the IStratumProperties or its defining properties are valid.")]
        public static bool IsValid(this IStratumProperty property, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (property == null)
            {
                ErrorMessage(methodName, property.GetType().ToString(), msg);
                return false;
            }

            return true;
        }

        [Description("Checks if a IContaminantProperty is valid and outputs relevant error message.")]
        [Input("property", "The IContaminantProperty to test for validity.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("isNull", "True if the IContaminantProperty or its defining properties are valid.")]
        public static bool IsValid(this IContaminantProperty property, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (property == null)
            {
                ErrorMessage(methodName, property.GetType().ToString(), msg);
                return false;
            }

            return true;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ErrorMessage(string methodName = "Method", string type = "type", string msg = "")
        {
            Base.Compute.RecordError($"Cannot evaluate {methodName} because the {type} is null. {msg}");
        }

        /***************************************************/

    }
}



